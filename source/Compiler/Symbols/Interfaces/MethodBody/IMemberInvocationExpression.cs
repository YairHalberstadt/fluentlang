namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	internal interface IMemberInvocationExpression : IInvocationExpression
	{
		IExpression Expression { get; }

		IInterfaceMethod Method { get; }

		string MemberName { get; }
	}
}