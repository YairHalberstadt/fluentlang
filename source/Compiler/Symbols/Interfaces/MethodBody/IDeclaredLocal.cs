namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IDeclaredLocal : ILocal
	{
		public IDeclarationStatement Declaration { get; }
	}
}
