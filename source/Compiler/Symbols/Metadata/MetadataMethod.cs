using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Source;
using FluentLang.Runtime.Metadata;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Metadata
{
	internal sealed class MetadataMethod : SymbolBase, IMethod
	{
		private readonly MethodSignatureAttribute _attribute;
		private readonly Lazy<ImmutableArray<ITypeParameter>> _typeParameters;
		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;
		private readonly SourceSymbolContext _sourceSymbolContext;

		public MetadataMethod(
			IAssembly metadataAssembly,
			DiagnosticBag diagnostics,
			MethodSignatureAttribute attribute) : base(diagnostics)
		{
			DeclaringAssembly = metadataAssembly;
			_attribute = attribute;
			_sourceSymbolContext = new SourceSymbolContext(null, DeclaringAssembly, ImmutableArray<QualifiedName>.Empty, null, () => TypeParameters);
			FullyQualifiedName = QualifiedName.Parse(attribute.FullyQualifiedName);
			_typeParameters = new Lazy<ImmutableArray<ITypeParameter>>(GenerateTypeParameters);
			_returnType = new Lazy<IType>(GenerateReturnType);
			_parameters = new Lazy<ImmutableArray<IParameter>>(GenerateParameters);
		}

		private ImmutableArray<ITypeParameter> GenerateTypeParameters()
		{
			return
				_attribute
				.TypeParameters
				.Select(x =>
					new SourceTypeParameter(
						Utils.Parse(x, p => p.type_parameter_metadata(), _diagnostics).type_parameter(),
						_sourceSymbolContext,
						isExported: true,
						_diagnostics))
				.ToImmutableArray<ITypeParameter>();
		}

		private ImmutableArray<IParameter> GenerateParameters()
		{
			return
				_attribute
				.Parameters
				.Select(x =>
					new SourceParameter(
						Utils.Parse(x, p => p.parameter_metadata(), _diagnostics).parameter(),
						_sourceSymbolContext,
						isExported: true,
						_diagnostics))
				.ToImmutableArray<IParameter>();
		}

		private IType GenerateReturnType()
		{
			var source = _attribute.ReturnType;
			var typeContext = Utils.Parse(source, p => p.return_type_metadata(), _diagnostics).type();
			return typeContext.BindType(_sourceSymbolContext, isExported: true, _diagnostics);
		}

		public bool IsExported => true;

		public QualifiedName FullyQualifiedName { get; }

		public ImmutableArray<ITypeParameter> TypeParameters => _typeParameters.Value;

		public IType ReturnType => _returnType.Value;

		public ImmutableArray<IParameter> Parameters => _parameters.Value;

		ImmutableArray<IParameterLocal> IMethod.ParameterLocals
			=> ImmutableArray<IParameterLocal>.Empty;

		ImmutableArray<IInterface> IMethod.LocalInterfaces
			=> ImmutableArray<IInterface>.Empty;

		ImmutableArray<IMethod> IMethod.LocalMethods
			=> ImmutableArray<IMethod>.Empty;
		IMethod? IMethod.DeclaringMethod
			=> null;

		public IAssembly DeclaringAssembly { get; }

		ImmutableArray<IStatement> IMethod.Statements
			=> ImmutableArray<IStatement>.Empty;

		IDeclarationStatement? IMethod.InScopeAfter
			=> null;

		ImmutableArray<IDeclaredLocal> IMethod.DirectlyCapturedDeclaredLocals
			=> ImmutableArray<IDeclaredLocal>.Empty;

		ImmutableArray<IMethod> IMethod.InvokedLocalMethods
			=> ImmutableArray<IMethod>.Empty;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _parameters.Value;
			_ = _returnType.Value;
		}
	}
}