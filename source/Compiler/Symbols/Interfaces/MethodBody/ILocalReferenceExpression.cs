namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	internal interface ILocalReferenceExpression : IExpression
	{
		string Identifier { get; }
		ILocal Local { get; }
	}
}