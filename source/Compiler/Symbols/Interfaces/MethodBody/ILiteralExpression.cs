using FluentLang.Compiler.Symbols.Visitor;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface ILiteralExpression : IExpression
	{
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		object? Value { get; }
	}
}