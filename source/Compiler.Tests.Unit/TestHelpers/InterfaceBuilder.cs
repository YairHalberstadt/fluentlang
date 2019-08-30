using FluentLang.Compiler.Model;
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

		public Interface Build()
		{
			return new Interface(
				AdditiveInterfaces.Select(x => x.Build()).ToImmutableArray(),
				MethodSets.Select(x => new InterfaceMethodSet(x.Select(x => x.Build()).ToImmutableArray())).ToImmutableArray(),
				FullyQualifiedName is null ? null : TestModelFactory.QualifiedName(FullyQualifiedName));
		}
	}

	public class InterfaceReferenceBuilder
	{
		public InterfaceReferenceBuilder(string partiallyQualifiedName)
		{
			PartiallyQualifiedName = partiallyQualifiedName;
		}

		public string PartiallyQualifiedName { get; }

		public List<string> ImportedNamespaces { get; } = new List<string>();

		public InterfaceReference Build()
		{
			return new InterfaceReference(
				ImportedNamespaces.Select(x => TestModelFactory.QualifiedName(x)).ToImmutableArray(),
				TestModelFactory.QualifiedName(PartiallyQualifiedName));
		}
	}
}
