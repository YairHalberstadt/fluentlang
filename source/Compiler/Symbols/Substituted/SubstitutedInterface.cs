using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Substituted
{
	internal class SubstitutedInterface : IInterface
	{
		private readonly IInterface _original;

		private readonly Lazy<ImmutableArray<IInterfaceMethod>> _methods;

		public SubstitutedInterface(IInterface original, ImmutableArrayDictionary<ITypeParameter, IType> substitutions)
		{
			_original = original;
			_methods = new Lazy<ImmutableArray<IInterfaceMethod>>(
				() => original.Methods.Select(x => x.Substitute(substitutions)).ToImmutableArray());
		}

		public bool IsExported => _original.IsExported;

		public QualifiedName? FullyQualifiedName => _original.FullyQualifiedName;

		public ImmutableArray<IInterfaceMethod> Methods => _methods.Value;

		public ImmutableArray<ITypeParameter> TypeParameters => ImmutableArray<ITypeParameter>.Empty;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{

		}
	}
}
