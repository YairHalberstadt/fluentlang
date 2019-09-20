namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IParameter : ISymbol
	{
		public string Name { get; }
		public IType Type { get; }
	}
}

