using FluentLang.Compiler.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public class InterfaceMethodBuilder
	{
		public string? Name { get; set;  }

		public TypeKeyBuilder? ReturnType { get; set; }

		public List<(string name, TypeKeyBuilder type)> Parameters { get; } = new List<(string name, TypeKeyBuilder type)>();

		public InterfaceMethod Build()
		{
			return new InterfaceMethod(
				Name ?? throw new InvalidOperationException($"{nameof(Name)} is null"),
				ReturnType?.Build() ?? throw new InvalidOperationException($"{nameof(ReturnType)} is null"),
				Parameters.Select(x => new Parameter(x.name, x.type.Build())).ToImmutableArray());
		}
	}
}