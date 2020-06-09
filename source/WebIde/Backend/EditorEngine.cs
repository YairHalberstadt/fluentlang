using FluentLang.Compiler.Compilation;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Diagnostic = FluentLang.Compiler.Diagnostics.Diagnostic;

namespace FluentLang.WebIde.Backend
{
	public class EditorEngine
	{
		private readonly AssemblyFactory _assemblyFactory;

		private readonly IAssemblyCompiler _assemblyCompiler;

		public EditorEngine(AssemblyFactory assemblyFactory, IAssemblyCompiler assemblyCompiler)
		{
			_assemblyFactory = assemblyFactory;
			_assemblyCompiler = assemblyCompiler;
		}

		public async Task<Result> CompileAndRun(string source, CancellationToken token)
		{
			var result = new Result();

			using var assemblyStream = new MemoryStream();
			using var csharpStream = new MemoryStream();
			var compilationResult = _assemblyCompiler.CompileAssembly(
				_assemblyFactory.FromSource(
					QualifiedName.Parse("WebIde"),
					(0, 0, null),
					ImmutableArray<IAssembly>.Empty,
					ImmutableArray.Create(DocumentFactory.FromString(source))),
				assemblyStream,
				csharpStream,
				cancellationToken: token);

			await Task.Yield();
			token.ThrowIfCancellationRequested();

			if (compilationResult.AssemblyDiagnostics.Any())
			{
				result.Diagnostics = compilationResult
					.AssemblyDiagnostics;
			}
			else
			{
				csharpStream.Position = 0;
				result.EmittedCSharp = CSharpSyntaxTree.ParseText(SourceText.From(csharpStream)).GetRoot().NormalizeWhitespace().ToFullString();

				await Task.Yield();
				token.ThrowIfCancellationRequested();

				try
				{
					var assembly = Assembly.Load(assemblyStream.ToArray());

					await Task.Yield();
					token.ThrowIfCancellationRequested();

					var entryPoint = assembly.EntryPoint;
					if (entryPoint != null)
					{
						result.RunResult = entryPoint.Invoke(null, null)?.ToString() ?? "";
					}
				}
				catch (Exception e)
				{
					result.RuntimeError = e.ToString();
				}
			}

			return result;
		}

		public class Result
		{
			public string? EmittedCSharp { get; set; }
			public string? RunResult { get; set; }
			public string? RuntimeError { get; set; }
			public ImmutableArray<Diagnostic> Diagnostics { get; set; }
		}
	}
}
