using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Immutable;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal sealed class SourceNamedInterface : SymbolBase, IInterface
	{
		private readonly Interface_declarationContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;
		private readonly SourceAnonymousInterface _anonymousSourceInterface;
		private readonly Lazy<ImmutableArray<ITypeParameter>> _typeParameters;

		public SourceNamedInterface(
			Interface_declarationContext context,
			SourceSymbolContext sourceSymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_anonymousSourceInterface = new SourceAnonymousInterface(
				context.anonymous_interface_declaration(),
				sourceSymbolContext.WithTypeParameters(() => TypeParameters),
				context.EXPORT() is { },
				_diagnostics);
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			FullyQualifiedName = new QualifiedName(context.UPPERCASE_IDENTIFIER().Symbol.Text, sourceSymbolContext.NameSpace);
			_typeParameters = new Lazy<ImmutableArray<ITypeParameter>>(GenerateTypeParameters);
		}

		public bool IsExported => _anonymousSourceInterface.IsExported;
		public QualifiedName? FullyQualifiedName { get; }
		public ImmutableArray<ITypeParameter> TypeParameters => _typeParameters.Value;
		public ImmutableArray<IInterfaceMethod> Methods => _anonymousSourceInterface.Methods;

		private ImmutableArray<ITypeParameter> GenerateTypeParameters()
		{
			return _context.type_parameter_list().BindTypeParameters(_sourceSymbolContext, IsExported, _diagnostics);
		}

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _typeParameters.Value;
		}
	}
}