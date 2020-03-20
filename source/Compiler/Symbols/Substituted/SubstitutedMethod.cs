using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Substituted
{
	[DebuggerDisplay("{FullyQualifiedName}")]
	internal class SubstitutedMethod : IMethod
	{
		private readonly IMethod _original;
		private readonly ImmutableArrayDictionary<ITypeParameter, IType> _substitutions;
		private readonly Dictionary<IType, IType> _substituted;
		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;
		private readonly Lazy<ImmutableArray<MethodOrInterfaceMethod>> _requiredMethodKeys;
		private readonly Lazy<ImmutableArray<MethodOrInterfaceMethod>> _directlyRequiredMethodKeys;

		public SubstitutedMethod(IMethod original, ImmutableArrayDictionary<ITypeParameter, IType> substitutions, Dictionary<IType, IType> substituted)
		{
			_original = original;
			_substitutions = substitutions;
			_substituted = substituted;
			_returnType = new Lazy<IType>(_original.ReturnType.Substitute(substitutions, substituted));
			_parameters = new Lazy<ImmutableArray<IParameter>>(
				() => _original.Parameters.Select(x => x.Substitute(substitutions, substituted)).ToImmutableArray());
			_requiredMethodKeys = new Lazy<ImmutableArray<MethodOrInterfaceMethod>>(CalculateRequiredMethodKeys);
			_directlyRequiredMethodKeys = new Lazy<ImmutableArray<MethodOrInterfaceMethod>>(
				() => _original.DirectlyRequiredMethodKeys.Select(x => x.SubstituteTypeParameters(substitutions, substituted)).ToImmutableArray());
		}

		private ImmutableArray<MethodOrInterfaceMethod> CalculateRequiredMethodKeys()
		{
			var index = 0;
			var builder = ImmutableArray.CreateBuilder<MethodOrInterfaceMethod>(_original.RequiredMethodKeys.Length);

			var originalDirectlyRequiredMethodKeys = _original.DirectlyRequiredMethodKeys;
			var directlyRequiredMethodKeys = _directlyRequiredMethodKeys.Value;

			foreach(var requiredMethod in _original.RequiredMethodKeys)
			{
				if (index < originalDirectlyRequiredMethodKeys.Length && requiredMethod.Equals(originalDirectlyRequiredMethodKeys[index]))
				{
					builder.Add(directlyRequiredMethodKeys[index]);
					index++;
				}
				else
				{
					builder.Add(requiredMethod.SubstituteTypeParameters(_substitutions, _substituted));
				}
			}

			Release.Assert(index == directlyRequiredMethodKeys.Length);

			return builder.MoveToImmutable();
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

		public ImmutableArray<IStatement> Statements => _original.Statements;

		public IDeclarationStatement? InScopeAfter => _original.InScopeAfter;

		public IMethod OriginalDefinition => _original.OriginalDefinition;

		public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

		ImmutableArray<IDeclaredLocal> IMethod.DirectlyCapturedDeclaredLocals
			=> _original.DirectlyCapturedDeclaredLocals;

		ImmutableArray<IMethod> IMethod.UsedLocalMethods => _original.UsedLocalMethods;

		ImmutableArray<MethodOrInterfaceMethod> IMethod.RequiredMethodKeys => _requiredMethodKeys.Value;

		ImmutableArray<MethodOrInterfaceMethod> IMethod.DirectlyRequiredMethodKeys => _directlyRequiredMethodKeys.Value;

		void ISymbol.EnsureAllLocalDiagnosticsCollected() {}
	}
}
