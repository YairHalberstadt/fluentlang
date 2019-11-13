namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	internal interface IStaticInvocationExpression : IInvocationExpression
	{
		IMethod Method { get; }
		QualifiedName MethodName { get; }
	}
}