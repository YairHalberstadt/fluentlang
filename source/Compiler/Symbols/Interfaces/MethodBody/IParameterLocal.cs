namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IParameterLocal : ILocal
	{
		public IParameter Parameter { get; }
	}
}
