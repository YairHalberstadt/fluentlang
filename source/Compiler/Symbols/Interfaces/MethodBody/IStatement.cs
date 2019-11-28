using FluentLang.Compiler.Symbols.Visitor;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IStatement : IVisitableSymbol
	{
		int OrdinalPositionInMethod { get; }
	}
}
