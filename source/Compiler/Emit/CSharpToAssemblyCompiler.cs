﻿using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace FluentLang.Compiler.Emit
{
	public class CSharpToAssemblyCompiler
	{
		private readonly ILogger<CSharpToAssemblyCompiler> _logger;

		public CSharpToAssemblyCompiler(ILogger<CSharpToAssemblyCompiler> logger)
		{
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
				_metadataReferences
					.Concat(GetReferences(assembly)),
				new CSharpCompilationOptions(
					assembly.Methods.Any(x => x.Name == "Main") ? OutputKind.ConsoleApplication : OutputKind.DynamicallyLinkedLibrary, 
					moduleName: assembly.Name.ToString(), 
					optimizationLevel: OptimizationLevel.Release));

			var emitResult = compilation.Emit(outputStream, pdbStream, cancellationToken: cancellationToken);

			_logger.LogInformation(
				$"compilation {(emitResult.Success ? "succeeded" : "failed")} with following diagnostics:"
				+ string.Join('\n', emitResult.Diagnostics));

			return emitResult;
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

		private static ImmutableArray<PortableExecutableReference> _metadataReferences = 
			Directory.GetFiles(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.*.dll")
			.Append(typeof(FLObject).Assembly.Location)
			.Select(x => MetadataReference.CreateFromFile(x))
			.ToImmutableArray();
	}
}