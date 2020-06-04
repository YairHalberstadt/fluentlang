using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace FluentLang.Compiler.Emit
{
	public class CSharpToAssemblyCompiler
	{
		private readonly IMetadataReferenceProvider _defaultMetadataReferences;
		private readonly ILogger<CSharpToAssemblyCompiler> _logger;

		public CSharpToAssemblyCompiler(IMetadataReferenceProvider defaultMetadataReferences, ILogger<CSharpToAssemblyCompiler> logger)
		{
			_defaultMetadataReferences = defaultMetadataReferences;
			_logger = logger;
		}

		public EmitResult Compile(
			TextReader file,
			IAssembly assembly,
			Stream outputStream,
			Stream? pdbStream = null,
			CancellationToken cancellationToken = default)
		{
			var sourceText = SourceText.From(file, int.MaxValue);
			var syntaxTree = SyntaxFactory.ParseSyntaxTree(sourceText, new CSharpParseOptions(LanguageVersion.CSharp8), cancellationToken: cancellationToken);

			var compilation = CSharpCompilation.Create(
				$"{assembly.Name}${assembly.Version}",
				new[] { syntaxTree },
				_defaultMetadataReferences.MetadataReferences
					.Concat(GetReferences(assembly)),
				new CSharpCompilationOptions(
					IsConsoleApplication(assembly) ? OutputKind.ConsoleApplication : OutputKind.DynamicallyLinkedLibrary,
					moduleName: assembly.Name.ToString(),
					optimizationLevel: OptimizationLevel.Release));

			var emitResult = compilation.Emit(outputStream, pdbStream, cancellationToken: cancellationToken);

			_logger.LogInformation(
				$"compilation {(emitResult.Success ? "succeeded" : "failed")} with following diagnostics:"
				+ string.Join('\n', emitResult.Diagnostics));

			return emitResult;
		}

		private bool IsConsoleApplication(IAssembly assembly)
		{
			return assembly.Methods.FirstOrDefault(x => x.Name == "Main") is 
			{ 
				ReturnType: Primitive returnType, 
				Parameters: { IsEmpty: true } 
			} && returnType == Primitive.Int;
		}

		private IEnumerable<MetadataReference> GetReferences(IAssembly assembly)
		{
			return assembly
				.ReferencedAssemblies
				.Select(x =>
					x.TryGetAssemblyBytes(out var bytes)
						? bytes
						: throw new InvalidOperationException(
							$"Could not get assembly bytes for reference {x.Name}"))
				.Select(x => MetadataReference.CreateFromStream(x.ToStream()));
		}
	}
}
