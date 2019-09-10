using FluentLang.Compiler.Model;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public class TestInterfaceMethodSet : IInterfaceMethodSet
	{
		public TestInterfaceMethodSet(ImmutableArray<IInterfaceMethod> methods)
		{
			Methods = methods;
		}

		public ImmutableArray<IInterfaceMethod> Methods { get; }
	}
}
