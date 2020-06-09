using FluentLang.Compiler.Diagnostics;
using FluentLang.WebIde.Backend;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Immutable;
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

		private readonly object _lock = new object();
		private CancellationTokenSource? _cancellationTokenSource;

		public Editor()
		{
			_thisRef = DotNetObjectReference.Create(this);
		}

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
				}

				Task.Run(async () =>
				{
					var result = await EditorEngine.CompileAndRun(_source, token);
					token.ThrowIfCancellationRequested();
					lock (_lock)
					{
						EmittedCSharp = result.EmittedCSharp;
						Result = result.RunResult;
						Diagnostics = result.Diagnostics;
						RuntimeError = result.RuntimeError;
					}
					StateHasChanged();
				}, token);
			}
		}

		private string? EmittedCSharp { get; set; }
		private string? Result { get; set; }
		private string? RuntimeError { get; set; }
		private ImmutableArray<Diagnostic> Diagnostics { get; set; }

		public void Dispose()
		{
			_thisRef.Dispose();
		}
	}
}
