using FluentLang.Compiler.Symbols.Substituted;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IUnion : IType
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

		IType IType.Substitute(IReadOnlyDictionary<ITypeParameter, IType> substitutions)
			=> new SubstitutedUnion(this, substitutions);
	}
}
