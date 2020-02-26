using FluentLang.Compiler.Symbols.Visitor;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IStaticInvocationExpression : IInvocationExpression
	{
		[return: MaybeNull]
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		IMethod Method { get; }
		QualifiedName MethodName { get; }
		ImmutableArray<IType> TypeArguments { get; }
	}
}