using FluentLang.Compiler.Symbols.Visitor;
using FluentLang.Shared;
using System.Collections.Generic;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IType : IVisitableSymbol
	{
		public sealed bool IsEquivalentTo(IType other) => IsEquivalentTo(other, null);

		internal bool IsEquivalentTo(IType other, Stack<(IType, IType)>? dependantEqualities);

		public bool IsSubtypeOf(IType other);

		internal IType Substitute(ImmutableArrayDictionary<ITypeParameter, IType> substitutions, Dictionary<IType, IType> substituted);
	}
}