using System.Collections.Generic;

namespace FluentLang.Compiler.Model
{
	public sealed class Primitive : Type
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

		public override QualifiedName FullyQualifiedName { get; }

		internal override bool IsEquivalentTo(Type? other, Stack<(Type, Type)>? dependantEqualities, ISemanticModel model)
		{
			return ReferenceEquals(this, other);
		}

		public override bool IsSubTypeOf(Type other, ISemanticModel model)
		{
			return IsEquivalentTo(other, model);
		}
	}
}
