using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.ErrorSymbols
{
	internal class ErrorInterface : IInterface, IErrorSymbol
	{
		private ErrorInterface() { }

		public static ErrorInterface Instance { get; } = new ErrorInterface();

		public QualifiedName? FullyQualifiedName => new QualifiedName("<Error Interface>", null);

		public bool IsExported => false;

		public ImmutableArray<ITypeParameter> TypeParameters => ImmutableArray<ITypeParameter>.Empty;

		public ImmutableArray<IType> TypeArguments => ImmutableArray<IType>.Empty;

		public ImmutableArray<IInterfaceMethod> Methods => ImmutableArray<IInterfaceMethod>.Empty;

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
	}
}
