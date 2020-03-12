using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Visitor;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FluentLang.Compiler.Symbols
{
	public sealed class Primitive : IType, IEquatable<Primitive>
	{
		[return: MaybeNull]
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);

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

		bool IType.IsEquivalentTo(IType other, Stack<(IType, IType)>? dependantEqualities)
		{
			return ReferenceEquals(this, other);
		}

		public bool IsSubtypeOf(IType other)
		{
			if (other is IUnion union)
				return union.Options.Any(x => IsSubtypeOf(x));

			return ReferenceEquals(this, other);
		}

		public override string ToString()
		{
			return FullyQualifiedName.ToString();
		}

		ImmutableArray<Diagnostic> ISymbol.AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{
		}

		IType IType.Substitute(ImmutableArrayDictionary<ITypeParameter, IType> substitutions, Dictionary<IType, IType> substituted)
		{
			return this;
		}
	}
}

