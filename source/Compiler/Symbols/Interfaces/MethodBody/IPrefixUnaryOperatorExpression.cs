namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IPrefixUnaryOperatorExpression : IExpression
	{
		Operator Operator { get; }
		IExpression Expression { get; }
	}
}