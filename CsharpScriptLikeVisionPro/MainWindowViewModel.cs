using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using CsharpScriptLikeVisionPro.Annotations;
using Prism.Commands;

namespace CsharpScriptLikeVisionPro
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Code { get; set; } = ScriptTools.BaseCode;
        public DelegateCommand CommandRun { get; private set; }
        public DelegateCommand CommandCompile { get; private set; }
        public DelegateCommand CommandRecovery { get; private set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
