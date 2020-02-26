using FluentLang.Compiler.Symbols.Visitor;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface ILocalReferenceExpression : IExpression
	{
		[return: MaybeNull]
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		string Identifier { get; }
		ILocal Local { get; }
	}
}