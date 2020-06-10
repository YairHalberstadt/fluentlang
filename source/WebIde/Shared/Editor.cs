using FluentLang.Compiler.Diagnostics;
using FluentLang.Shared;
using FluentLang.WebIde.Backend;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diagnostic = FluentLang.Compiler.Diagnostics.Diagnostic;

namespace FluentLang.WebIde.Shared
{
	public partial class Editor : LayoutComponentBase, IDisposable
	{
		[Inject]
		private EditorEngine EditorEngine { get; set; } = default!;

		private readonly DotNetObjectReference<Editor> _thisRef;

		private string _source = "";

		private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
		private CancellationTokenSource? _cancellationTokenSource;

		public Editor()
		{
			_thisRef = DotNetObjectReference.Create(this);
		}

		[JSInvokable]
		public async Task SetSource(string value)
		{
			CancellationToken token;

			await _lock.WaitAsync();
			try
			{
				_cancellationTokenSource?.Cancel();
				_cancellationTokenSource?.Dispose();
				_cancellationTokenSource = new CancellationTokenSource();
				token = _cancellationTokenSource.Token;
				_source = value;
			}
			finally
			{
				_lock.Release();
			}


			var result = await EditorEngine.CompileAndRun(_source, token);
			token.ThrowIfCancellationRequested();

			await _lock.WaitAsync();
			try
			{
				EmittedCSharp = result.EmittedCSharp;
				Result = result.RunResult;
				Diagnostics = result.Diagnostics;
				RuntimeError = result.RuntimeError;

				await JsRuntime.InvokeVoidAsync("annotate", Diagnostics.EmptyIfDefault().Select(x =>
				{
					var (startRow, startColumn, endRow, endColumn) = x.Location.TextRange;
					return new AceDiagnostic(startRow - 1, startColumn, endRow - 1, endColumn, DiagnosticFormatter.CreateDiagnosticMessage(x));
				}));
			}
			finally
			{
				_lock.Release();
			}

			token.ThrowIfCancellationRequested();
			StateHasChanged();
		}

		private string? EmittedCSharp { get; set; }
		private string? Result { get; set; }
		private string? RuntimeError { get; set; }
		private ImmutableArray<Diagnostic> Diagnostics { get; set; }

		public void Dispose()
		{
			_thisRef.Dispose();
		}

		private struct AceDiagnostic
		{
			public AceDiagnostic(int row, int column, int endRow, int endColumn, string text)
			{
				Row = row;
				Column = column;
				EndRow = endRow;
				EndColumn = endColumn;
				Text = text;
			}

			public int Row { get; }
			public int Column { get; }
			public int EndRow { get; }
			public int EndColumn { get; }
			public string Text { get; }
			public string Type => "error";
		}
	}
}
