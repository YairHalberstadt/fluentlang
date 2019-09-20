using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal class SourceParameter : IParameter
	{
		private readonly ParameterContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;
		private readonly DiagnosticBag _diagnostics;
		private readonly Lazy<IType> _type;

		public SourceParameter(ParameterContext context, SourceSymbolContext sourceSymbolContext, DiagnosticBag diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			_diagnostics = diagnostics.CreateChildBag();
			Name = _context.LOWERCASE_IDENTIFIER().Symbol.Text;
			_type = new Lazy<IType>(() => _context.type_declaration().type().BindType(_sourceSymbolContext, _diagnostics));
		}
		public string Name { get; }

		public IType Type => _type.Value;
	}
}

