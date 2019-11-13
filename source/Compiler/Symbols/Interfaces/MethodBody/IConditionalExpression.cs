namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	internal interface IConditionalExpression : IExpression
	{
		IExpression Condition { get; }
		IExpression IfFalse { get; }
		IExpression IfTrue { get; }
	}
}