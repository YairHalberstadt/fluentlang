using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Substituted
{
	internal class SubstitutedMethod : IMethod
	{
		private readonly IMethod _original;
		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;

		public SubstitutedMethod(IMethod original, IReadOnlyDictionary<ITypeParameter, IType> substitutions)
		{
			_original = original;
			_returnType = new Lazy<IType>(_original.ReturnType.Substitute(substitutions));
			_parameters = new Lazy<ImmutableArray<IParameter>>(
				_original.Parameters.Select(x => x.Substitute(substitutions)).ToImmutableArray());
		}

		public bool IsExported => _original.IsExported;

		public QualifiedName FullyQualifiedName => _original.FullyQualifiedName;

		public IType ReturnType => _returnType.Value;

		public ImmutableArray<ITypeParameter> TypeParameters => ImmutableArray<ITypeParameter>.Empty;

		public ImmutableArray<IParameter> Parameters => _parameters.Value;

		public ImmutableArray<IParameterLocal> ParameterLocals => throw new InvalidOperationException();

		public ImmutableArray<IInterface> LocalInterfaces => throw new InvalidOperationException();

		public ImmutableArray<IMethod> LocalMethods => throw new InvalidOperationException();

		public IMethod? DeclaringMethod => _original.DeclaringMethod;

		public IAssembly DeclaringAssembly => _original.DeclaringAssembly;

		public ImmutableArray<IStatement> Statements => throw new InvalidOperationException();

		public IDeclarationStatement? InScopeAfter => throw new InvalidOperationException();

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		ImmutableArray<IDeclaredLocal> IMethod.DirectlyCapturedDeclaredLocals
			=> throw new InvalidOperationException();

		ImmutableArray<IMethod> IMethod.InvokedLocalMethods => throw new InvalidOperationException();

		void ISymbol.EnsureAllLocalDiagnosticsCollected() {}
	}
}
