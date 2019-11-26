using FluentLang.Compiler.Symbols.Visitor;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface ILocalReferenceExpression : IExpression
	{
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		string Identifier { get; }
		ILocal Local { get; }
	}
}