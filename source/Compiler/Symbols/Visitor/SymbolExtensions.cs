using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Symbols.Visitor
{
	public static class SymbolExtensions
	{
		public static IEnumerable<ISymbol> DescendantNodes(this IVisitableSymbol symbol, Func<ISymbol, bool>? visitChildren = null, Func<ISymbol, bool>? includeSymbol = null)
		{
			var visitor = new CollectAllSymbolsVisitor(
				visitChildren ?? (_ => true),
				includeSymbol ?? (_ => true));
			symbol.Visit(visitor);
			return visitor.Symbols;
		}

		private sealed class CollectAllSymbolsVisitor : BaseSymbolVisitor<object>
		{
			public readonly List<ISymbol> Symbols = new List<ISymbol>();

			private readonly Func<ISymbol, bool> _visitChildren;
			private readonly Func<ISymbol, bool> _includeSymbol;

			public CollectAllSymbolsVisitor(Func<ISymbol, bool> visitChildren, Func<ISymbol, bool> includeSymbol)
			{
				_visitChildren = visitChildren;
				_includeSymbol = includeSymbol;
			}

			[return: MaybeNull]
			protected override object DefaultVisit(IVisitableSymbol symbol)
			{
				if (_includeSymbol(symbol))
				{
					Symbols.Add(symbol);
				}
				if (_visitChildren(symbol))
				{
					return symbol.Visit(this);
				}
				return default;
			}
		}
	}
}