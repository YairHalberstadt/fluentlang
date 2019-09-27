using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.ErrorSymbols
{
	internal class ErrorMethod : IMethod, IErrorSymbol
	{
		public static ErrorMethod Instance { get; }

		private ErrorMethod()
		{

		}

		public QualifiedName FullyQualifiedName => new QualifiedName("<Error Method>", null);

		public IType ReturnType => ErrorType.Instance;

		public ImmutableArray<IParameter> Parameters => ImmutableArray<IParameter>.Empty;

		public ImmutableArray<IInterface> LocalInterfaces => ImmutableArray<IInterface>.Empty;

		public ImmutableArray<IMethod> LocalMethods => ImmutableArray<IMethod>.Empty;

		public IMethod? DeclaringMethod => null;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{
		}
	}
}
