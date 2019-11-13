using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class NewObjectExpression : INewObjectExpression
	{
		private NewObjectExpression() { }
		public static NewObjectExpression Instance { get; } = new NewObjectExpression();

		public IType Type => EmptyInterface.Instance;

		ImmutableArray<Diagnostic> ISymbol.AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{
		}
	}
}
