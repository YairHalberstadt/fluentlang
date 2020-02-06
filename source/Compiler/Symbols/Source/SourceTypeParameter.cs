using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal class SourceTypeParameter : SymbolBase, ITypeParameter
	{
		private readonly Type_parameterContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;
		private readonly Lazy<IType?> _constrainedTo;

		public SourceTypeParameter(
			Type_parameterContext context,
			SourceSymbolContext sourceSymbolContext,
			bool isExported,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			Name = _context.UPPERCASE_IDENTIFIER().Symbol.Text;
			_constrainedTo = new Lazy<IType?>(() => _context.type_declaration()?.type().BindType(_sourceSymbolContext, isExported, _diagnostics));
		}
		public string Name { get; }

		public IType? ConstrainedTo => _constrainedTo.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _constrainedTo.Value;
		}
	}
}
