namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IMixinPatch : IObjectPatch
	{
		IExpression Expression { get; }
	}
}