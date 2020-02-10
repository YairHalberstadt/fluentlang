using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Immutable;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal class SourceTypeParameter : SymbolBase, ITypeParameter
	{
		private readonly Type_parameterContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;
		private readonly bool _isExported;
		private readonly Lazy<IType?> _constrainedTo;

		public SourceTypeParameter(
			Type_parameterContext context,
			SourceSymbolContext sourceSymbolContext,
			bool isExported,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			_isExported = isExported;
			Name = _context.UPPERCASE_IDENTIFIER().Symbol.Text;
			_constrainedTo = new Lazy<IType?>(BindConstrainedTo);
		}

		private IType? BindConstrainedTo()
		{
			var type = _context.type_declaration()?.type().BindType(_sourceSymbolContext, _isExported, _diagnostics);
			if (type is Primitive)
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.type_declaration()),
					ErrorCode.CannotConstrainToPrimitive,
					ImmutableArray.Create<object?>(this, type)));
			}
			return type;
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
