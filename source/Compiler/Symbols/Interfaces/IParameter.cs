namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IParameter
	{
		public string Name { get; }
		public IType Type { get; }
	}
}

