using FluentLang.Compiler.Model;
using System;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public class TypeKeyBuilder
	{
		public TypeKeyBuilder(Primitive primitive)
		{
			Primitive = primitive;
		}

		public TypeKeyBuilder(InterfaceBuilder interfaceBuilder)
		{
			InterfaceBuilder = interfaceBuilder;
		}

		public TypeKeyBuilder(InterfaceReferenceBuilder interfaceReferenceBuilder)
		{
			InterfaceReferenceBuilder = interfaceReferenceBuilder;
		}

		public Primitive? Primitive { get; }

		public InterfaceBuilder? InterfaceBuilder { get; }

		public InterfaceReferenceBuilder? InterfaceReferenceBuilder { get; }

		public TypeKey Build()
		{
			if (Primitive != null)
				return new TypeKey(Primitive);
			if (InterfaceBuilder != null)
				return new TypeKey(InterfaceBuilder.Build());
			if (InterfaceReferenceBuilder != null)
				return new TypeKey(InterfaceReferenceBuilder.Build());
			throw new InvalidOperationException("Unreachable");
		}
	}
}