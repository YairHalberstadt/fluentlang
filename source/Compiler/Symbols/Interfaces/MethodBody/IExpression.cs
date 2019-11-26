using FluentLang.Compiler.Symbols.Visitor;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IExpression : IVisitableSymbol
	{
		public IType Type { get; }
	}
}
