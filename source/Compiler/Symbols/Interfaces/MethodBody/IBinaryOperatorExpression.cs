namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IBinaryOperatorExpression : IExpression
	{
		IExpression Left { get; }
		Operator Operator { get; }
		IExpression Right { get; }
	}
}