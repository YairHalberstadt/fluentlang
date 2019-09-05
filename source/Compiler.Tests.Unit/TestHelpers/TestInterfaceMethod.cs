using FluentLang.Compiler.Model;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public class TestInterfaceMethod : IInterfaceMethod
	{
		public TestInterfaceMethod(string name, TypeKey returnType, ImmutableArray<Parameter> parameters)
		{
			Name = name;
			ReturnType = returnType;
			Parameters = parameters;
		}

		public string Name { get; }

		public ImmutableArray<Parameter> Parameters { get; }

		public TypeKey ReturnType { get; }
	}
}