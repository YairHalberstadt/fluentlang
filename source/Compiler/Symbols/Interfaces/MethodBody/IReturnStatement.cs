namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IReturnStatement : IStatement
	{
		IExpression Expression { get; }
	}
}