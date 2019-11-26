using FluentLang.Compiler.Symbols.Visitor;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IConditionalExpression : IExpression
	{
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		IExpression Condition { get; }
		IExpression IfFalse { get; }
		IExpression IfTrue { get; }
	}
}