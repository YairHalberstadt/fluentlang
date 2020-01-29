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

			if (other is IUnion union)
			{
				// We don't check if number of options are equal, as we don't deduplicate options.

				foreach (var option in Options)
				{
					if (!union.Options.Any(x => x.IsEquivalentTo(option, dependantEqualities)))
					{
						return false;
					}
				}

				foreach (var option in union.Options)
				{
					if (!Options.Any(x => x.IsEquivalentTo(option, dependantEqualities)))
					{
						return false;
					}
				}

				return true;
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
