using FluentLang.Compiler.Model;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
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
				TestModelFactory.QualifiedName(PartiallyQualifiedName), null);
		}
	}
}
