using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace FluentLang.Compiler.Emit
{
	public class CSharpAssemblyCompiler
	{
		private readonly ILogger _logger;

		public CSharpAssemblyCompiler(ILogger logger)
		{
			_logger = logger;
		}

		public EmitResult Compile(
			TextReader file,
			IAssembly assembly,
			CompilationOptions compilationOptions,
			Stream outputStream,
			IEnumerable<Assembly> referencedAssemblies,
			CancellationToken cancellationToken = default)
		{
			var sourceText = SourceText.From(file, int.MaxValue);
			var syntaxTree = SyntaxFactory.ParseSyntaxTree(sourceText, new CSharpParseOptions(LanguageVersion.CSharp8), cancellationToken: cancellationToken);

			var compilation = CSharpCompilation.Create(
				assembly.Name.ToString(),
				new[] { syntaxTree },
				_metadataReferences
					.Concat(referencedAssemblies.Select(x => MetadataReference.CreateFromFile(x.Location))),
				new CSharpCompilationOptions(
					OutputKind.DynamicallyLinkedLibrary, 
					moduleName: assembly.Name.ToString(), 
					optimizationLevel: OptimizationLevel.Release));

			var emitResult = compilation.Emit(outputStream, cancellationToken: cancellationToken);

			_logger.LogInformation(
				$"compilation {(emitResult.Success ? "succeeded" : "failed")} with following diagnostics:"
				+ string.Join('\n', emitResult.Diagnostics));

			return emitResult;
		}

		private static ImmutableArray<PortableExecutableReference> _metadataReferences = 
			Directory.GetFiles(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.*.dll")
			.Append(typeof(FLObject).Assembly.Location)
			.Select(x => MetadataReference.CreateFromFile(x))
			.ToImmutableArray();
	}
}
