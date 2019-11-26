using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Visitor
{
	public static class SymbolExtensions
	{
		public static IEnumerable<ISymbol> DescendantNodesAndSelf(this IVisitableSymbol symbol)
		{
			return symbol.Visit(CollectAllSymbolsVisitor.DefaultInstance);
		}

		public static IEnumerable<ISymbol> DescendantNodes(this IVisitableSymbol symbol)
		{
			return symbol.Visit(CollectAllSymbolsVisitor.DefaultInstance).Skip(1);
		}

		public static bool IsDescendantOrThis(this IVisitableSymbol symbol, ISymbol target)
		{
			return symbol.Visit(new IsDescendantVisitor(target));
		}
	}
}