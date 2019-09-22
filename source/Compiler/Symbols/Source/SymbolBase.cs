using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Source
{
	public abstract class SymbolBase : ISymbol
	{
		protected readonly DiagnosticBag _diagnostics;
		private readonly Lazy<ImmutableArray<Diagnostic>> _allDiagnostics;

		public SymbolBase(DiagnosticBag diagnostics)
		{
			_diagnostics = diagnostics.CreateChildBag(this);
			_allDiagnostics = new Lazy<ImmutableArray<Diagnostic>>(() =>
			{
				_diagnostics.EnsureAllDiagnosticsCollectedForSymbol();
				return _diagnostics.ToImmutableArray();
			});
		}

		public ImmutableArray<Diagnostic> AllDiagnostics => _allDiagnostics.Value;

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{
			EnsureAllLocalDiagnosticsCollected();
		}

		protected abstract void EnsureAllLocalDiagnosticsCollected();
	}
}
