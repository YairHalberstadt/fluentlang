using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Immutable;

namespace FluentLang.TestUtils
{
	public class TestInterface : IInterface
	{
		public QualifiedName? FullyQualifiedName { get; set; }

		public bool IsExported => false;

		public ImmutableArray<ITypeParameter> TypeParameters => ImmutableArray<ITypeParameter>.Empty;

		public ImmutableArray<IInterfaceMethod> Methods { get; set; } = ImmutableArray<IInterfaceMethod>.Empty;

		public ImmutableArray<Diagnostic> AllDiagnostics { get; set; } = ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected() { }
	}
}
