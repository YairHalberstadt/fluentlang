using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Source;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Substituted
{
	internal class ConstructedInterface : IInterface
	{
		private readonly SubstitutedInterface _substituted;

		public ConstructedInterface(IInterface original, ImmutableArray<IType> typeArguments)
		{
			_substituted = new SubstitutedInterface(
				original,
				SourceSymbolContextExtensions.CreateTypeMap(typeArguments, original.TypeParameters),
				new Dictionary<IType, IType>());

			TypeArguments = typeArguments;
		}

		public bool IsExported => _substituted.IsExported;

		public QualifiedName? FullyQualifiedName => _substituted.FullyQualifiedName;

		public ImmutableArray<IInterfaceMethod> Methods => _substituted.Methods;

		public ImmutableArray<ITypeParameter> TypeParameters => ImmutableArray<ITypeParameter>.Empty;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		public ImmutableArray<IType> TypeArguments { get; }

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{

		}
	}
}
