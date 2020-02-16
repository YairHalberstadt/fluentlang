using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Substituted
{
	internal class SubstitutedUnion : IUnion
	{
		private readonly Lazy<ImmutableArray<IType>> _options;

		public SubstitutedUnion(IUnion original, IReadOnlyDictionary<ITypeParameter, IType> substitutions)
		{
			_options = new Lazy<ImmutableArray<IType>>(
				original.Options.Select(x => x.Substitute(substitutions)).ToImmutableArray());
		}

		public ImmutableArray<IType> Options => _options.Value;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected() { }
	}
}
