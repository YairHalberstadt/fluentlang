using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	public class ParameterLocal : IParameterLocal
	{
		public ParameterLocal(IParameter parameter)
		{
			Parameter = parameter;
		}

		public string Identifier => Parameter.Name;

		public IType Type => Parameter.Type;

		public IParameter Parameter { get; }
	}
}
