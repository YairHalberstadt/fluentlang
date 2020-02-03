using FluentLang.Compiler.Symbols.Visitor;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IMatchExpression : IExpression
	{
		[return: MaybeNull]
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);

		IExpression Expression { get; }

		ImmutableArray<IMatchExpressionArm> Arms { get; }
	}
}
