using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Symbols.ErrorSymbols
{
	public class ErrorInterfaceMethod : IInterfaceMethod, IErrorSymbol
	{
		public ErrorInterfaceMethod(string name, int numParameters)
		{
			Name = name;
			Parameters = Enumerable.Repeat<IParameter>(ErrorParameter.Instance, numParameters).ToImmutableArray();
		}

		public string Name { get; }

		public IType ReturnType => ErrorType.Instance;

		public ImmutableArray<IParameter> Parameters { get; }

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected(){ }
	}
}
