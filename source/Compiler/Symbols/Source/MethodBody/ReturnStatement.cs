using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class ReturnStatement : SymbolBase, IReturnStatement
	{
		private readonly Declaration_statementContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;
		private readonly Lazy<IExpression> _expression;

		public ReturnStatement(
			Declaration_statementContext context,
			SourceSymbolContext sourceSymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			_expression = new Lazy<IExpression>(BindExpression);
		}

		private IExpression BindExpression()
		{
			return _context.expression().BindExpression(_sourceSymbolContext, _diagnostics);
		}

		public IExpression Expression => _expression.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _expression.Value;
		}
	}
}
