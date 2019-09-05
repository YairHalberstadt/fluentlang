using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Model
{
	public sealed class Primitive : IType, IEquatable<Primitive>
	{
		public static Primitive Bool { get; } = new Primitive(new QualifiedName("bool"));
		public static Primitive Int { get; } = new Primitive(new QualifiedName("int"));
		public static Primitive Double { get; } = new Primitive(new QualifiedName("double"));
		public static Primitive Char { get; } = new Primitive(new QualifiedName("char"));
		public static Primitive String { get; } = new Primitive(new QualifiedName("string"));

		private Primitive(QualifiedName fullyQualifiedName)
		{
			FullyQualifiedName = fullyQualifiedName;
		}

		public QualifiedName FullyQualifiedName { get; }

		bool IType.IsEquivalentTo(IType? other, Stack<(IType, IType)>? dependantEqualities, ISemanticModel model)
		{
			return ReferenceEquals(this, other);
		}

		bool IType.IsSubTypeOf(IType other, ISemanticModel model)
		{
			return ((IType)this).IsEquivalentTo(other, null, model);
		}

		public bool Equals(Primitive? other)
		{
			return ReferenceEquals(this, other);
		}

		public override string? ToString()
		{
			return FullyQualifiedName.ToString();
		}
	}
}
