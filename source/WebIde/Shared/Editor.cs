using FluentLang.Compiler.Compilation;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.JSInterop;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FluentLang.WebIde.Shared
{
	public partial class Editor : LayoutComponentBase
	{
		private string _source = "";

		[Inject]
		private AssemblyFactory AssemblyFactory { get; set; } = default!;

		[Inject]
		private IAssemblyCompiler AssemblyCompiler { get; set; } = default!;

		[Inject]
		private IDiagnosticFormatter DiagnosticFormatter { get; set; } = default!;

		private readonly object _lock = new object();
		private CancellationTokenSource? _cancellationTokenSource;

		private string Source
		{
			get => _source;
			set
			{
				CancellationToken token;
				lock (_lock)
				{
					_cancellationTokenSource?.Cancel();
					_cancellationTokenSource?.Dispose();
					_cancellationTokenSource = new CancellationTokenSource();
					token = _cancellationTokenSource.Token;
					_source = value;

					EmittedCSharp = default;
					Result = default;
					Diagnostics = default;
					RuntimeError = default;
				}
				StateHasChanged();

				Task.Run(async () =>
				{
					using var assemblyStream = new MemoryStream();
					using var csharpStream = new MemoryStream();
					var compilationResult = AssemblyCompiler.CompileAssembly(
						AssemblyFactory.FromSource(
							QualifiedName.Parse("WebIde"),
							(0, 0, null),
							ImmutableArray<IAssembly>.Empty,
							ImmutableArray.Create(DocumentFactory.FromString(Source))),
						assemblyStream,
						csharpStream,
						cancellationToken: token);

					await Task.Yield();
					token.ThrowIfCancellationRequested();

					if (compilationResult.AssemblyDiagnostics.Any())
					{
						var diagnostics = compilationResult
							.AssemblyDiagnostics
							.Select(error => $"ERROR! Location: {DiagnosticFormatter.CreateLocationMessage(error)}, Message: {DiagnosticFormatter.CreateDiagnosticMessage(error)}")
							.ToImmutableArray();

						await Task.Yield();
						lock (_lock)
						{
							token.ThrowIfCancellationRequested();
							Diagnostics = diagnostics;
						}
					}
					else
					{
						csharpStream.Position = 0;
						var emittedCSharp = CSharpSyntaxTree.ParseText(SourceText.From(csharpStream)).GetRoot().NormalizeWhitespace().ToFullString();
						await Task.Yield();
						lock (_lock)
						{
							token.ThrowIfCancellationRequested();
							EmittedCSharp = emittedCSharp;
						}

						try
						{
							var assembly = Assembly.Load(assemblyStream.ToArray());

							token.ThrowIfCancellationRequested();

							var entryPoint = assembly.EntryPoint;
							if (entryPoint != null)
							{
								var result = entryPoint.Invoke(null, null)?.ToString() ?? "";

								await Task.Yield();
								lock (_lock)
								{
									token.ThrowIfCancellationRequested();
									Result = result;
								}
							}
						}
						catch (Exception e)
						{
							var runtimeError = e.ToString();

							await Task.Yield();
							lock (_lock)
							{
								token.ThrowIfCancellationRequested();
								RuntimeError = runtimeError;
							}
						}
					}
					StateHasChanged();
				}, token);
			}
		}

		private string? EmittedCSharp { get; set; }
		private string? Result { get; set; }
		private string? RuntimeError { get; set; }
		private ImmutableArray<string> Diagnostics { get; set; }
	}
}
