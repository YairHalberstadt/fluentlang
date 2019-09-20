using FluentLang.Compiler.Diagnostics;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface ISymbol
	{
		internal void EnsureAllLocalDiagnosticsCollected();

		public ImmutableArray<Diagnostic> AllDiagnostics { get; }
	}
}

