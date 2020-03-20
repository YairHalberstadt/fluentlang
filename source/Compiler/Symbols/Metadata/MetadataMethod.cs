using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Source;
using FluentLang.Runtime.Metadata;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Metadata
{
	[DebuggerDisplay("{FullyQualifiedName}")]
	internal sealed class MetadataMethod : SymbolBase, IMethod
	{
		private readonly SourceSymbolContext _sourceSymbolContext;
		private readonly SourceSymbolContext _sourceSymbolContextWithTypeParams;
		private readonly Method_signature_metadataContext _methodSignature;
		private readonly MethodSignatureAttribute _attribute;

		private readonly Lazy<ImmutableArray<ITypeParameter>> _typeParameters;
		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;
		private readonly Lazy<ImmutableArray<MethodOrInterfaceMethod>> _requiredMethodKeys;

		public MetadataMethod(
			IAssembly metadataAssembly,
			DiagnosticBag diagnostics,
			MethodSignatureAttribute attribute) : base(diagnostics)
		{
			DeclaringAssembly = metadataAssembly;
			_attribute = attribute;
			_methodSignature = Utils.Parse(
				attribute.Signature,
				p => p.method_signature_metadata(),
				_diagnostics);
			_sourceSymbolContext = new SourceSymbolContext(null, DeclaringAssembly, ImmutableArray<QualifiedName>.Empty, null, () => ImmutableArray<ITypeParameter>.Empty);
			_sourceSymbolContextWithTypeParams = _sourceSymbolContext.WithTypeParameters(() => TypeParameters);
			FullyQualifiedName = _methodSignature.qualified_name().GetQualifiedName();
			_typeParameters = new Lazy<ImmutableArray<ITypeParameter>>(GenerateTypeParameters);
			_returnType = new Lazy<IType>(GenerateReturnType);
			_parameters = new Lazy<ImmutableArray<IParameter>>(GenerateParameters);
			_requiredMethodKeys = new Lazy<ImmutableArray<MethodOrInterfaceMethod>>(GenerateRequiredMethodKeys);
		}

		private ImmutableArray<ITypeParameter> GenerateTypeParameters()
		{
			return
				_methodSignature
				.type_parameter_list()
				.BindTypeParameters(_sourceSymbolContext, true, _diagnostics);
		}

		private ImmutableArray<IParameter> GenerateParameters()
		{
			return
				_methodSignature
				.parameters()
				.BindParameters(_sourceSymbolContextWithTypeParams, true, _diagnostics);
		}

		private IType GenerateReturnType()
		{
			return
				_methodSignature
				.type_declaration()
				.type()
				.BindType(_sourceSymbolContextWithTypeParams, true, _diagnostics);
		}

		private ImmutableArray<MethodOrInterfaceMethod> GenerateRequiredMethodKeys()
		{
			return
				_attribute
				.RequireMethodKeySignatures
				.Select(x => Utils.Parse(x, p => p.interface_method_metadata(), _diagnostics))
				.Select( x => new MethodOrInterfaceMethod(new SourceInterfaceMethod(
					x.method_signature(),
					_sourceSymbolContextWithTypeParams,
					true,
					_diagnostics)))
				.ToImmutableArray();
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

		ImmutableArray<IMethod> IMethod.UsedLocalMethods
			=> ImmutableArray<IMethod>.Empty;

		ImmutableArray<MethodOrInterfaceMethod> IMethod.RequiredMethodKeys => _requiredMethodKeys.Value;

		ImmutableArray<MethodOrInterfaceMethod> IMethod.DirectlyRequiredMethodKeys => _requiredMethodKeys.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _typeParameters.Value;
			_ = _parameters.Value;
			_ = _returnType.Value;
			_ = _requiredMethodKeys.Value;
		}
	}
}