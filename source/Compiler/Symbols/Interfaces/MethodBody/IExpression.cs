namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IExpression : ISymbol
	{
		public IType Type { get; }
	}
}
