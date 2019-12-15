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
	internal class MetadataMethod : MetadataSymbolBase, IMethod
	{
		private readonly MethodSignatureAttribute _attribute;
		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;
		private readonly SourceSymbolContext _childSourceSymbolContext;

		public MetadataMethod(
			IAssembly metadataAssembly,
			DiagnosticBag diagnostics,
			MethodSignatureAttribute attribute) : base(diagnostics)
		{
			DeclaringAssembly = metadataAssembly;
			_attribute = attribute;
			_childSourceSymbolContext = new SourceSymbolContext(null, DeclaringAssembly, ImmutableArray<QualifiedName>.Empty, null);
			FullyQualifiedName = QualifiedName.Parse(attribute.FullyQualifiedName);
			_returnType = new Lazy<IType>(GenerateReturnType);
			_parameters = new Lazy<ImmutableArray<IParameter>>(GenerateParameters);
		}

		private ImmutableArray<IParameter> GenerateParameters()
		{
			return
				_attribute
				.Parameters
				.Select(x =>
					new SourceParameter(
						Parse(x, p => p.parameter_metadata()).parameter(),
						_childSourceSymbolContext,
						_diagnostics))
				.ToImmutableArray<IParameter>();
		}

		private IType GenerateReturnType()
		{
			var source = _attribute.ReturnType;
			var typeContext = Parse(source, p => p.return_type_metadata()).type();
			return typeContext.BindType(_childSourceSymbolContext, _diagnostics);
		}

		public bool IsExported => true;

		public QualifiedName FullyQualifiedName { get; }

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