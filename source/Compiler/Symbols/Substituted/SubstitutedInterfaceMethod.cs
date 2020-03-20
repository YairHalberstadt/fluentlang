using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Substituted
{
	[DebuggerDisplay("{Name}")]
	internal class SubstitutedInterfaceMethod : IInterfaceMethod
	{
		private readonly IInterfaceMethod _original;
		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;

		public SubstitutedInterfaceMethod(IInterfaceMethod original, ImmutableArrayDictionary<ITypeParameter, IType> substitutions, Dictionary<IType, IType> substituted)
		{
			_original = original;
			_returnType = new Lazy<IType>(_original.ReturnType.Substitute(substitutions, substituted));
			_parameters = new Lazy<ImmutableArray<IParameter>>(
				() => _original.Parameters.Select(x => x.Substitute(substitutions, substituted)).ToImmutableArray());
		}

		public string Name => _original.Name;

		public IType ReturnType => _returnType.Value;

		public ImmutableArray<IParameter> Parameters => _parameters.Value;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected() { }
	}
}
