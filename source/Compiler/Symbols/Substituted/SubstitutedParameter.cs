using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Substituted
{
	internal class SubstitutedParameter : IParameter
	{
		private readonly IParameter _original;
		private readonly Lazy<IType> _type;

		public SubstitutedParameter(IParameter original, IReadOnlyDictionary<ITypeParameter, IType> substitutions)
		{
			_original = original;
			_type = new Lazy<IType>(_original.Type.Substitute(substitutions));
		}

		public string Name => _original.Name;

		public IType Type => _type.Value;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected() { }
	}
}
