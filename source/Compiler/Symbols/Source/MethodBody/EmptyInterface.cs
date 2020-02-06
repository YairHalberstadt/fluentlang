using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class EmptyInterface : IInterface
	{
		private EmptyInterface() { }

		public static EmptyInterface Instance { get; } = new EmptyInterface();

		public QualifiedName? FullyQualifiedName => null;

		public bool IsExported => false;

		public ImmutableArray<ITypeParameter> TypeParameters => ImmutableArray<ITypeParameter>.Empty;

		public ImmutableArray<IInterfaceMethod> Methods => ImmutableArray<IInterfaceMethod>.Empty;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{
		}
	}
}
