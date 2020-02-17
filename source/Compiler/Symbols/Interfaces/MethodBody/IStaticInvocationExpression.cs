using FluentLang.Compiler.Symbols.Visitor;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IStaticInvocationExpression : IInvocationExpression
	{
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		IMethod Method { get; }
		QualifiedName MethodName { get; }
		ImmutableArray<IType> TypeArguments { get; }
	}
}