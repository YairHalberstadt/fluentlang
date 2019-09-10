using FluentLang.Compiler.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public class InterfaceBuilder
	{
		public List<InterfaceReferenceBuilder> AdditiveInterfaces { get; } = new List<InterfaceReferenceBuilder>();

		public List<InterfaceMethodBuilder[]> MethodSets { get; } = new List<InterfaceMethodBuilder[]>();

		public string? FullyQualifiedName { get; set; }

		public IInterface Build(Func<IMethod>? scope = null)
		{
			return new TestInterface(
				AdditiveInterfaces.Select(x => x.Build()).ToImmutableArray(),
				MethodSets.Select(x => (IInterfaceMethodSet)new TestInterfaceMethodSet(x.Select(x => x.Build()).ToImmutableArray())).ToImmutableArray(),
				scope,
				FullyQualifiedName is null ? null : TestModelFactory.QualifiedName(FullyQualifiedName));
		}
	}
}
