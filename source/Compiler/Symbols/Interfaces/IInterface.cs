using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Substituted;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IInterface : IType
	{
		public bool IsExported { get; }
		public QualifiedName? FullyQualifiedName { get; }
		public ImmutableArray<IInterfaceMethod> Methods { get; }
		public ImmutableArray<ITypeParameter> TypeParameters { get; }
		bool IType.IsEquivalentTo(IType other, Stack<(IType, IType)>? dependantEqualities)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (!(other is IInterface otherInterface))
				return false;

			if (dependantEqualities?.Contains((this, other)) ?? false)
			{
				return true;
			}

			// We don't check if number of methods are equal, as we don't deduplicate methods.

			dependantEqualities ??= new Stack<(IType, IType)>();
			dependantEqualities.Push((this, other));

			foreach (var method in Methods)
			{
				if (!otherInterface.Methods.Any(x => x.IsEquivalentTo(method, dependantEqualities)))
				{
					Pop(dependantEqualities);

					return false;
				}
			}

			foreach (var method in otherInterface.Methods)
			{
				if (!Methods.Any(x => x.IsEquivalentTo(method, dependantEqualities)))
				{
					Pop(dependantEqualities);

					return false;
				}
			}

			Pop(dependantEqualities);

			return true;

			void Pop(Stack<(IType, IType)> dependantEqualities)
			{
				var top = dependantEqualities.Pop();
				Release.Assert((this, other) == top);
			}
		}

		bool IType.IsSubtypeOf(IType other)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (other is IUnion union)
			{
				return union.Options.Any(x => IsSubtypeOf(x));
			}

			if (other is IInterface otherInterface)
			{
				foreach (var method in otherInterface.Methods)
				{
					if (!Methods.Any(x => x.IsEquivalentTo(method, null)))
					{
						return false;
					}
				}

				return true;
			}

			return false;
		}

		IType IType.Substitute(IReadOnlyDictionary<ITypeParameter, IType> substitutions)
			=> new SubstitutedInterface(this, substitutions);
		
	}
}

