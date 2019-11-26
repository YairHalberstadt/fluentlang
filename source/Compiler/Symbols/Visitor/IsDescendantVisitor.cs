using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Visitor
{
	public sealed class IsDescendantVisitor : IBaseSymbolVisitor<bool>
	{
		private readonly ISymbol _target;

		public IsDescendantVisitor(ISymbol target)
		{
			_target = target;
		}

		[return: MaybeNull]
		bool IBaseSymbolVisitor<bool>.MergeResults(IEnumerable<bool> results)
		{
			throw Release.Fail("unreachable");
		}

		[return: MaybeNull]
		bool IBaseSymbolVisitor<bool>.DefaultVisit<T>(T symbol, Func<T, IEnumerable<bool>> visitChildren)
		{
			return _target.Equals(symbol) ? true : visitChildren(symbol).Any();
		}
	}
}