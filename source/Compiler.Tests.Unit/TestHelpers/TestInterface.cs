using FluentLang.Compiler.Model;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public class TestInterface : IInterface
	{
		public TestInterface(ImmutableArray<InterfaceReference> additiveInterfaces, ImmutableArray<IInterfaceMethodSet> methodSets, Method? scope, QualifiedName? fullyQualifiedName)
		{
			AdditiveInterfaces = additiveInterfaces;
			MethodSets = methodSets;
			Scope = scope;
			FullyQualifiedName = fullyQualifiedName;
		}

		public ImmutableArray<InterfaceReference> AdditiveInterfaces { get; }

		public ImmutableArray<IInterfaceMethodSet> MethodSets { get; }

		public Method? Scope { get; }

		public QualifiedName? FullyQualifiedName { get; }
	}
}
