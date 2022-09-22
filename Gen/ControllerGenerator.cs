using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Gen
{
    [Generator]
    public class ControllerGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }        
#endif

        }

        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.Compilation.SyntaxTrees;
            var handlers = syntaxTrees.Where(x => x.GetText().ToString().Contains("[Http"));

            foreach (var handler in handlers)
            {
                var usings = handler.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();
                var usingsText = string.Join("\r\n", usings);
                var sourceBuilder = new StringBuilder(usingsText);

                var classDeclarationSyntax =
                    handler.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();

                var classname = classDeclarationSyntax.Identifier.ToString();
                var generatedClassName = $"{classname}Controller";
                var splitClass = classDeclarationSyntax.ToString().Split(new[] { '{' }, 2);

                sourceBuilder.Append($@"
namespace GeneratedControllers
{{
    [ApiController]
    public class {generatedClassName} : ControllerBase
    {{
");
                sourceBuilder.AppendLine(splitClass[1]);
                sourceBuilder.AppendLine("}");
                context.AddSource("ControllerGenerator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }
    }
}