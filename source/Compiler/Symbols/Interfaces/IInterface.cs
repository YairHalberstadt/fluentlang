﻿using FluentLang.Compiler.Symbols.Substituted;
using FluentLang.Compiler.Symbols.Visitor;
using FluentLang.Shared;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IInterface : IType
	{
		[return: MaybeNull]
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);

		public bool IsExported { get; }
		public QualifiedName? FullyQualifiedName { get; }
		public ImmutableArray<IInterfaceMethod> Methods { get; }
		public ImmutableArray<ITypeParameter> TypeParameters { get; }
		public ImmutableArray<IType> TypeArguments { get; }
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

		IType IType.Substitute(ImmutableArrayDictionary<ITypeParameter, IType> substitutions, Dictionary<IType, IType> substituted)
			=> substituted.TryGetValue(this, out var substitution) ? substitution : new SubstitutedInterface(this, substitutions, substituted);

		internal sealed IInterface Construct(ImmutableArray<IType> typeArguments) => new ConstructedInterface(this, typeArguments);
	}
}

