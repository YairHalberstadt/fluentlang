using FluentLang.Compiler.Symbols.Visitor;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IMemberInvocationExpression : IInvocationExpression
	{
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		IExpression Expression { get; }
		IInterfaceMethod Method { get; }
		string MemberName { get; }
	}
}