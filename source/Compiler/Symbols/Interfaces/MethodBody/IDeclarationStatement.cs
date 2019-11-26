using FluentLang.Compiler.Symbols.Visitor;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IDeclarationStatement : IStatement
	{
		[return: MaybeNull]
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		IExpression Expression { get; }
		string? IdentifierName { get; }
		IType? DeclaredType { get; }
		IType Type { get; }
		IDeclaredLocal? Local { get; }
	}
}