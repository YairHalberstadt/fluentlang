namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IDeclarationStatement : IStatement
	{
		IExpression Expression { get; }
		string IdentifierName { get; }
		IType? DeclaredType { get; }
		IType Type { get; }
		IDeclaredLocal Local { get; }
	}
}