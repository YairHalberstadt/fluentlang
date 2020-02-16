using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.ErrorSymbols
{
	internal class ErrorType : IType, IErrorSymbol
	{
		private ErrorType() { }

		public static ErrorType Instance { get; } = new ErrorType();

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		bool IType.IsEquivalentTo(IType other, Stack<(IType, IType)>? dependantEqualities)
		{
			// We use ReferenceEquals since it ensures that AreEquavalent maintains an Equivalence Relation
			// If we want to treat ErrorTypes differently, we should explicitly check for IErrorSymbol
			return ReferenceEquals(this, other);
		}

		bool IType.IsSubtypeOf(IType other)
		{
			// We use ReferenceEquals since it ensures that IsSubtypeOf maintains a partial ordering
			// If we want to treat ErrorTypes differently, we should explicitly check for IErrorSymbol
			return ReferenceEquals(this, other);
		}

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{
		}

		IType IType.Substitute(IReadOnlyDictionary<ITypeParameter, IType> substitutions)
		{
			return this;
		}
	}
}