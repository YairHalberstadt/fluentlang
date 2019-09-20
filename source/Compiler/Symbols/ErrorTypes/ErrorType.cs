using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.ErrorTypes
{
	internal class ErrorType : IErrorType
	{
		private ErrorType() { }

		public static ErrorType Instance { get; } = new ErrorType();

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
