using FluentLang.Compiler.Symbols.Visitor;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IMethodPatch : IObjectPatch
	{
		[return: MaybeNull]
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);

		IMethod Method { get; }
		ImmutableArray<IType> TypeArguments { get; }
	}
}