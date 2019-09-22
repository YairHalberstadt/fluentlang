using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Immutable;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal class SourceParameter : SymbolBase, IParameter
	{
		private readonly ParameterContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;

		private readonly Lazy<IType> _type;

		public SourceParameter(
			ParameterContext context,
			SourceSymbolContext sourceSymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			Name = _context.LOWERCASE_IDENTIFIER().Symbol.Text;
			_type = new Lazy<IType>(() => _context.type_declaration().type().BindType(_sourceSymbolContext, _diagnostics));
		}
		public string Name { get; }

		public IType Type => _type.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _type.Value;
		}
	}
}

