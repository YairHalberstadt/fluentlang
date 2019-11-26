using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentLang.Compiler.Symbols.Visitor
{
	public interface IVisitableSymbol : ISymbol
	{
		public T Visit<T>(ISymbolVisitor<T> visitor);
	}
}
