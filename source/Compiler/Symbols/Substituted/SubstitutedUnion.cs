using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Substituted
{
	internal class SubstitutedUnion : IUnion
	{
		private readonly Lazy<ImmutableArray<IType>> _options;

		public SubstitutedUnion(IUnion original, ImmutableArrayDictionary<ITypeParameter, IType> substitutions, Dictionary<IType, IType> substituted)
		{
			substituted.Add(original, this);
			substituted.Add(this, this);
			_options = new Lazy<ImmutableArray<IType>>(
				() => original.Options.Select(x => x.Substitute(substitutions, substituted)).ToImmutableArray());
		}

		public ImmutableArray<IType> Options => _options.Value;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected() { }
	}
}
