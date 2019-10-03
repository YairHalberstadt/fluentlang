using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.ErrorSymbols
{
	public class ErrorParameter : IParameter, IErrorSymbol
	{
		public static ErrorParameter Instance = new ErrorParameter();

		private ErrorParameter() { }

		public string Name => "<Error Parameter>";

		public IType Type => ErrorType.Instance;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{
			throw new System.NotImplementedException();
		}
	}
}
