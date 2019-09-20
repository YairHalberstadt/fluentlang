using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.ErrorTypes
{
	internal class ErrorInterface : IInterface, IErrorType
	{
		private ErrorInterface() { }

		public static ErrorInterface Instance { get; } = new ErrorInterface();

		public QualifiedName? FullyQualifiedName => new QualifiedName("<Error Interface>", null);

		public ImmutableArray<IInterfaceMethod> Methods => ImmutableArray<IInterfaceMethod>.Empty;

		bool IType.IsEquivalentTo(IType other, Stack<(IType, IType)>? dependantEqualities)
		{
			// We use ReferenceEquals since it ensures that AreEquavalent maintains an Equivalence Relation
			// If we want to treat ErrorTypes differently, we should explicitly check for IErrorType
			return ReferenceEquals(this, other);
		}

		bool IType.IsSubtypeOf(IType other)
		{
			// We use ReferenceEquals since it ensures that IsSubtypeOf maintains a partial ordering
			// If we want to treat ErrorTypes differently, we should explicitly check for IErrorType
			return ReferenceEquals(this, other);
		}
	}
}
