using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	interface IUnion : IType
	{
		ImmutableArray<IType> Options { get; }

		bool IType.IsEquivalentTo(IType other, Stack<(IType, IType)>? dependantEqualities)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (other is IUnion union && Options.Length == union.Options.Length)
			{
				return 
					Options
					.Zip(union.Options, (a, b) => a.IsEquivalentTo(b, dependantEqualities))
					.All(x => x);
			}

			return false;
		}

		bool IType.IsSubtypeOf(IType other)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Options.All(x => x.IsSubtypeOf(other));
		}
	}
}
