namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IObjectPatch : ISymbol
	{
		IMethod? Method { get; }
		IExpression? MixedInExpression { get; }
	}
}