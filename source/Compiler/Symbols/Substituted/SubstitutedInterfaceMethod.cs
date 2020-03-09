using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace FluentLang.Compiler.Symbols.Substituted
{
	internal class SubstitutedInterfaceMethod : IInterfaceMethod
	{
		private readonly IInterfaceMethod _original;
		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;

		public SubstitutedInterfaceMethod(IInterfaceMethod original, ImmutableArrayDictionary<ITypeParameter, IType> substitutions)
		{
			_original = original;
			_returnType = new Lazy<IType>(_original.ReturnType.Substitute(substitutions));
			_parameters = new Lazy<ImmutableArray<IParameter>>(
				() => _original.Parameters.Select(x => x.Substitute(substitutions)).ToImmutableArray());
		}

		public string Name => _original.Name;

		public IType ReturnType => _returnType.Value;

		public ImmutableArray<IParameter> Parameters => _parameters.Value;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected() { }
	}
}
