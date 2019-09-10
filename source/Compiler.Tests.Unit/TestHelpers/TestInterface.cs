using FluentLang.Compiler.Model;
using System;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public class TestInterface : IInterface
	{
		public TestInterface(
			ImmutableArray<InterfaceReference> additiveInterfaces,
			ImmutableArray<IInterfaceMethodSet> methodSets,
			Func<IMethod>? scope,
			QualifiedName? fullyQualifiedName)
		{
			AdditiveInterfaces = additiveInterfaces;
			MethodSets = methodSets;
			_scope = scope;
			FullyQualifiedName = fullyQualifiedName;
		}

		public ImmutableArray<InterfaceReference> AdditiveInterfaces { get; }

		public ImmutableArray<IInterfaceMethodSet> MethodSets { get; }

		private readonly Func<IMethod>? _scope;
		public IMethod? Scope => _scope?.Invoke();

		public QualifiedName? FullyQualifiedName { get; }
	}
}
