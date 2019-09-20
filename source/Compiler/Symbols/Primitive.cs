using FluentLang.Compiler.Symbols.Interfaces;
using System;

namespace FluentLang.Compiler.Symbols
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

		public bool Equals(Primitive? other)
		{
			return ReferenceEquals(this, other);
		}

		public override bool Equals(object? obj)
		{
			return ReferenceEquals(this, obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		bool IType.IsEquivalentTo(IType other, System.Collections.Generic.Stack<(IType, IType)>? dependantEqualities)
		{
			return ReferenceEquals(this, other);
		}

		bool IType.IsSubtypeOf(IType other)
		{
			return ReferenceEquals(this, other);
		}

		public override string? ToString()
		{
			return FullyQualifiedName.ToString();
		}
	}
}

