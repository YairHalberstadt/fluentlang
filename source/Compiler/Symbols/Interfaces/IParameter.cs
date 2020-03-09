﻿using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Substituted;
using FluentLang.Compiler.Symbols.Visitor;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IParameter : IVisitableSymbol
	{
		[return: MaybeNull]
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		public string Name { get; }
		public IType Type { get; }

		internal IParameter Substitute(ImmutableArrayDictionary<ITypeParameter, IType> substitutions)
			=> new SubstitutedParameter(this, substitutions);
	}
}

