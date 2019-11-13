namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface ILocal
	{
		public string Identifier { get; }
		public IType Type { get; }
	}
}
