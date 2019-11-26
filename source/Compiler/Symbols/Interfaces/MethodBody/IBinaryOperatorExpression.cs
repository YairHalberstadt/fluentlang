using FluentLang.Compiler.Symbols.Visitor;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IBinaryOperatorExpression : IExpression
	{
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);

		IExpression Left { get; }
		Operator Operator { get; }
		IExpression Right { get; }
	}
}