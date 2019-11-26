using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Visitor
{
	public sealed class CollectAllSymbolsVisitor : IBaseSymbolVisitor<IEnumerable<ISymbol>>
	{
		private readonly Func<ISymbol, bool> _visitChildren;

		public CollectAllSymbolsVisitor(Func<ISymbol, bool> visitChildren)
		{
			_visitChildren = visitChildren;
		}

		[return: MaybeNull]
		IEnumerable<ISymbol> IBaseSymbolVisitor<IEnumerable<ISymbol>>.MergeResults(IEnumerable<IEnumerable<ISymbol>> results)
		{
			throw Release.Fail("unreachable");
		}

		[return: MaybeNull]
		IEnumerable<ISymbol> IBaseSymbolVisitor<IEnumerable<ISymbol>>.DefaultVisit<T>(T symbol, Func<T, IEnumerable<IEnumerable<ISymbol>>> visitChildren)
		{
			return _visitChildren(symbol)
					? visitChildren(symbol).SelectMany(x => x).Prepend(symbol)
					: new ISymbol[] { symbol };
		}

		public static CollectAllSymbolsVisitor DefaultInstance =
			new CollectAllSymbolsVisitor(_ => true);
	}
}