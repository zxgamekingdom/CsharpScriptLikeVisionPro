using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CsharpScriptLikeVisionPro.Annotations;
using Microsoft.CodeAnalysis;
using Prism.Commands;

namespace CsharpScriptLikeVisionPro
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Assembly _assembly;
        public event PropertyChangedEventHandler PropertyChanged;
        public string Code { get; set; } = ScriptTools.BaseCode;
        public DelegateCommand CommandRun { get; private set; }
        public DelegateCommand CommandCompile { get; private set; }
        public DelegateCommand CommandRecovery { get; private set; }

        public MainWindowViewModel()
        {
            InitCommand();
        }

        private void InitCommand()
        {
            CommandRecovery = new DelegateCommand(() =>
            {
                Code = ScriptTools.BaseCode;
            });
            CommandCompile = new DelegateCommand(() =>
                {
                    try
                    {
                        IsCompiling = true;
                        var references = new List<MetadataReference>()
                        {
                            MetadataReference.CreateFromFile(typeof(CancellationToken)
                                .Assembly.Location),
                            MetadataReference.CreateFromFile(typeof(Debugger).Assembly
                                .Location),
                            MetadataReference.CreateFromFile(typeof(Task).Assembly
                                .Location),
                        };
                        foreach (Assembly assembly in
                            AppDomain.CurrentDomain.GetAssemblies())
                        {
                            string location = assembly.Location;
                            if (string.IsNullOrWhiteSpace(location) is false)
                                references.Add(
                                    MetadataReference.CreateFromFile(location));
                        }

                        _assembly = ScriptTools.CreateAssembly(Code, references);
                    }
                    finally
                    {
                        IsCompiling = false;
                    }
                },
                () => IsCompiling is false).ObservesProperty(() => IsCompiling);
            CommandRun = new DelegateCommand(async () =>
                {
                    try
                    {
                        Type t = _assembly.GetTypes()
                            .Single(type =>
                                type.BaseType!.FullName == typeof(InvokeBase).FullName);
                        var instance = (InvokeBase) Activator.CreateInstance(t);
                        await instance!.Init(
                            new Dictionary<string, object>() {{"1", 233}});
                        await instance!.Run(CancellationToken.None);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString(),
                            "运行出错",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                },
                () => IsCompiling is false).ObservesProperty(() => IsCompiling);
        }

        public bool IsCompiling { get; private set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
