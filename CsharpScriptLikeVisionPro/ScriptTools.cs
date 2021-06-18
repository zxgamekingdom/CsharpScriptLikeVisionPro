using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace CsharpScriptLikeVisionPro
{
    public class ScriptTools
    {
        public static string BaseCode =>
            @"class InvokeBaseImpl : InvokeBase
{
    private Dictionary<string, object> _inputs;

    public override Task Init(Dictionary<string, object> inputs)
    {
        _inputs = inputs;
        return base.Init(inputs);
    }

    public override Task Run(CancellationToken token)
    {
        // #if DEBUG
        // if (Debugger.IsAttached) Debugger.Break();
        // #endif
        throw new NotImplementedException();
    }
}";

        public static Assembly CreateAssembly(string code,
            IEnumerable<MetadataReference> references)
        {
            Encoding encoding = Encoding.UTF8;
            byte[] buff = encoding.GetBytes(code);
            string assemblyName = Guid.NewGuid().ToString();
            string codePath = $"{assemblyName}.cs";
            SourceText sourceText =
                SourceText.From(buff, buff.Length, encoding, canBeEmbedded: true);
            SyntaxTree syntaxTree =
                CSharpSyntaxTree.ParseText(sourceText, path: codePath);
            var cSharpCompilation = CSharpCompilation.Create(assemblyName,
                new[] {syntaxTree},
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var pdbStream = new MemoryStream();
            using var peStream = new MemoryStream();
            EmitResult emitResult = cSharpCompilation.Emit(peStream,
                pdbStream,
                options: new EmitOptions(
                    debugInformationFormat: DebugInformationFormat.PortablePdb),
                embeddedTexts: new[] {EmbeddedText.FromSource(codePath, sourceText),});
            if (emitResult.Success is false)
            {
                var stringBuilder = new StringBuilder();
                foreach (Diagnostic diagnostic in emitResult.Diagnostics)
                {
                    stringBuilder.AppendLine(diagnostic.ToString());
                }

                MessageBox.Show(stringBuilder.ToString(),
                    "编译错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return default;
            }

            return Assembly.Load(peStream.ToArray(), pdbStream.ToArray());
        }
    }
}
