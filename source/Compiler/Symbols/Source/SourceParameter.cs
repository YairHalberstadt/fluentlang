using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Immutable;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal class SourceParameter : IParameter
	{
		private readonly ParameterContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;

		private readonly Lazy<IType> _type;

		private readonly DiagnosticBag _diagnostics;
		private readonly Lazy<ImmutableArray<Diagnostic>> _allDiagnostics;

		public SourceParameter(ParameterContext context, SourceSymbolContext sourceSymbolContext, DiagnosticBag diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			_diagnostics = diagnostics.CreateChildBag(this);
			Name = _context.LOWERCASE_IDENTIFIER().Symbol.Text;
			_type = new Lazy<IType>(() => _context.type_declaration().type().BindType(_sourceSymbolContext, _diagnostics));

			_allDiagnostics = new Lazy<ImmutableArray<Diagnostic>>(() =>
			{
				_diagnostics.EnsureAllDiagnosticsCollectedForSymbol();
				return _diagnostics.ToImmutableArray();
			});
		}
		public string Name { get; }

		public IType Type => _type.Value;

		public ImmutableArray<Diagnostic> AllDiagnostics => _allDiagnostics.Value;

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _type.Value;
		}
	}
}

