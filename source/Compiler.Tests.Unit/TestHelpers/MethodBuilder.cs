using FluentLang.Compiler.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public class MethodBuilder
	{
		public MethodBuilder(string fullyQualifiedName, TypeKeyBuilder returnType)
		{
			FullyQualifiedName = fullyQualifiedName;
			ReturnType = returnType;
		}

		public string FullyQualifiedName { get; }

		public List<(string name, TypeKeyBuilder type)> Parameters { get; } = new List<(string name, TypeKeyBuilder type)>();

		public TypeKeyBuilder ReturnType { get; }

		public List<InterfaceBuilder> LocalInterfaces { get; } = new List<InterfaceBuilder>();

		public List<MethodBuilder> LocalMethods { get; } = new List<MethodBuilder>();

		public IMethod Build(Func<IMethod>? scope = null)
		{
			IMethod? method = null;
			method = new TestMethod(
				TestModelFactory.QualifiedName(FullyQualifiedName),
				ReturnType.Build(),
				Parameters.Select(x => new Parameter(x.name, x.type.Build())).ToImmutableArray(),
				scope,
				LocalInterfaces.Select(x => x.Build(() => method!)).ToImmutableArray(),
				LocalMethods.Select(x => x.Build(() => method!)).ToImmutableArray());
			return method;
		}
	}
}
