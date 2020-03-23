using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Substituted;
using FluentLang.Compiler.Symbols.Visitor;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IInterfaceMethod : IVisitableSymbol
	{
		[return: MaybeNull]
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);

		public string Name { get; }
		public IType ReturnType { get; }
		public ImmutableArray<IParameter> Parameters { get; }
		public IInterfaceMethod OriginalDefinition => this;

		internal bool IsEquivalentTo(IInterfaceMethod otherMethod, Stack<(IType, IType)>? dependantEqualities)
		{
			if (Name != otherMethod.Name)
				return false;

			if (Parameters.Length != otherMethod.Parameters.Length)
				return false;

			if (!ReturnType.IsEquivalentTo(otherMethod.ReturnType, dependantEqualities))
				return false;

			return Parameters.SequenceEqual(otherMethod.Parameters, (x, y) => x.Type.IsEquivalentTo(y.Type, dependantEqualities));
		}

		internal IInterfaceMethod Substitute(ImmutableArrayDictionary<ITypeParameter, IType> substitutions, Dictionary<IType, IType> substituted)
			=> new SubstitutedInterfaceMethod(this, substitutions, substituted);
	}
}

