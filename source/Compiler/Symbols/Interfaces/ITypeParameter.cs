using System.Collections.Generic;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface ITypeParameter : IType 
	{
		public string Name { get; }
		public IType? ConstrainedTo { get; }

		bool IType.IsEquivalentTo(IType other, Stack<(IType, IType)>? dependantEqualities)
		{
			return ReferenceEquals(this, other);
		}

		bool IType.IsSubtypeOf(IType other)
		{
			return ConstrainedTo?.IsSubtypeOf(other) ?? false;
		}
	}
}

