using FluentLang.Compiler.Symbols.Source.MethodBody;
using FluentLang.Compiler.Symbols.Visitor;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IObjectPatchingExpression : IExpression
	{
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		IExpression Expression { get; }
		ImmutableArray<IObjectPatch> Patches { get; }
	}
}