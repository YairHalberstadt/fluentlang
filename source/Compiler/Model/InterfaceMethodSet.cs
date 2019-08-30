using System.Collections.Immutable;

namespace FluentLang.Compiler.Model
{
	public sealed class InterfaceMethodSet
	{
		public InterfaceMethodSet(ImmutableArray<InterfaceMethod> methods)
		{
			Methods = methods;
		}

		public ImmutableArray<InterfaceMethod> Methods { get; }
	}
}
