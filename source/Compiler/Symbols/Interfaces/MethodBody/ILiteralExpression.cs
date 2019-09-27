namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface ILiteralExpression
	{
		Primitive Type { get; }
		object? Value { get; }
	}
}