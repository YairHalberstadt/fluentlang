using System;
using System.Linq;

namespace FluentLang.Compiler.Symbols
{
	public sealed class QualifiedName : IEquatable<QualifiedName>
	{
		public QualifiedName(string name, QualifiedName? parent = null)
		{
			Name = name;
			Parent = parent;
		}

		public string Name { get; }

		public QualifiedName? Parent { get; }

		public QualifiedName Append(QualifiedName? qualifiedName)
		{
			if (qualifiedName is null)
				return this;

			var name = qualifiedName.Name;
			if (qualifiedName.Parent is null)
				return new QualifiedName(name, this);
			return new QualifiedName(name, Append(qualifiedName.Parent));
		}

		public QualifiedName Prepend(QualifiedName? qualifiedName)
		{
			if (qualifiedName is null)
				return this;

			return qualifiedName.Append(this);
		}

		public bool Equals(QualifiedName? other)
		{
			if (other is null)
				return false;

			if (!Name.Equals(other.Name, StringComparison.Ordinal))
				return false;

			if (Parent is null)
				return other.Parent is null;

			return Parent.Equals(other.Parent);
		}

		public override int GetHashCode()
		{
			return (Parent?.GetHashCode() ?? 0) * 17 + Name.GetHashCode();
		}

		public override bool Equals(object? obj)
		{
			if (obj is QualifiedName other)
			{
				return Equals(other);
			}
			return false;
		}

		public override string ToString()
		{
			if (Parent is null)
				return Name;
			return Parent.ToString() + "." + Name;
		}

		public static bool operator ==(QualifiedName? a, QualifiedName? b)
		{
			if (a is null)
				return b is null;
			return a.Equals(b);
		}

		public static bool operator !=(QualifiedName? a, QualifiedName? b)
		{
			return !(a == b);
		}

		public static QualifiedName Parse(string name) => 
			name.Split('.').Aggregate((QualifiedName?)null, (l, r) => new QualifiedName(r, l))!;
	}
}

