using FluentLang.Compiler.Symbols.Visitor;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IMatchExpressionArm : IVisitableSymbol
	{
		[return: MaybeNull]
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		IType Type { get; }
		string? IdentifierName { get; }
		ILocal? Local { get; }
		IExpression Expression { get; }
	}
}
