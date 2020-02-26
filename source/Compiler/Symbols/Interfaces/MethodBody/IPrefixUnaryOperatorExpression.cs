using FluentLang.Compiler.Symbols.Visitor;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IPrefixUnaryOperatorExpression : IExpression
	{
		[return: MaybeNull]
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		Operator Operator { get; }
		IExpression Expression { get; }
	}
}