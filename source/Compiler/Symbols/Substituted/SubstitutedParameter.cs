using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Substituted
{
	internal class SubstitutedParameter : IParameter
	{
		private readonly IParameter _original;
		private readonly Lazy<IType> _type;

		public SubstitutedParameter(IParameter original, ImmutableArrayDictionary<ITypeParameter, IType> substitutions, Dictionary<IType, IType> substituted)
		{
			_original = original;
			_type = new Lazy<IType>(() => _original.Type.Substitute(substitutions, substituted));
		}

		public string Name => _original.Name;

		public IType Type => _type.Value;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected() { }
	}
}
