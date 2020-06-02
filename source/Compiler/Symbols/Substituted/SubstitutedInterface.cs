using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Substituted
{
	[DebuggerDisplay("{FullyQualifiedName}")]
	internal class SubstitutedInterface : IInterface
	{
		private readonly IInterface _original;

		private readonly Lazy<ImmutableArray<IInterfaceMethod>> _methods;
		private readonly Lazy<ImmutableArray<IType>> _typeArguments;

		public SubstitutedInterface(IInterface original, ImmutableArrayDictionary<ITypeParameter, IType> substitutions, Dictionary<IType, IType> substituted)
		{
			substituted.Add(original, this);
			substituted.Add(this, this);
			_original = original;
			_methods = new Lazy<ImmutableArray<IInterfaceMethod>>(
				() => original.Methods.Select(x => x.Substitute(substitutions, substituted)).ToImmutableArray());
			_typeArguments = new Lazy<ImmutableArray<IType>>(
				() => original.TypeArguments.Select(x => x.Substitute(substitutions, substituted)).ToImmutableArray());
		}

		public bool IsExported => _original.IsExported;

		public QualifiedName? FullyQualifiedName => _original.FullyQualifiedName;

		public ImmutableArray<IInterfaceMethod> Methods => _methods.Value;

		public ImmutableArray<ITypeParameter> TypeParameters => ImmutableArray<ITypeParameter>.Empty;

		public ImmutableArray<IType> TypeArguments => _typeArguments.Value;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{

		}
	}
}
