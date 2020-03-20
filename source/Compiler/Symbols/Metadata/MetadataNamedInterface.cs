using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Source;
using FluentLang.Runtime.Metadata;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Metadata
{
	internal sealed class MetadataNamedInterface : SymbolBase, IInterface
	{
		private readonly InterfaceAttribute _attribute;
		private readonly SourceSymbolContext _sourceSymbolContext;
		private readonly SourceAnonymousInterface _anonymousSourceInterface;
		private readonly Lazy<ImmutableArray<ITypeParameter>> _typeParameters;

		public MetadataNamedInterface(
			InterfaceAttribute attribute,
			SourceSymbolContext sourceSymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_anonymousSourceInterface = new SourceAnonymousInterface(
				Utils.Parse(
					attribute.AnonymousInterfaceDeclaration,
					p => p.anonymous_interface_declaration_metadata().anonymous_interface_declaration(),
					_diagnostics),
				sourceSymbolContext.WithTypeParameters(() => TypeParameters),
				true,
				_diagnostics);
			_attribute = attribute;
			_sourceSymbolContext = sourceSymbolContext;
			FullyQualifiedName = QualifiedName.Parse(attribute.FullyQualifiedName);
			_typeParameters = new Lazy<ImmutableArray<ITypeParameter>>(GenerateTypeParameters);
		}

		public bool IsExported => _anonymousSourceInterface.IsExported;
		public QualifiedName? FullyQualifiedName { get; }
		public ImmutableArray<ITypeParameter> TypeParameters => _typeParameters.Value;
		public ImmutableArray<IType> TypeArguments => ImmutableArray<IType>.Empty;
		public ImmutableArray<IInterfaceMethod> Methods => _anonymousSourceInterface.Methods;

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

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _typeParameters.Value;
		}
	}
}