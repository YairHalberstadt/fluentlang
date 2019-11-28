using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class ReturnStatement : SymbolBase, IReturnStatement
	{
		private readonly Return_statementContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;
		private readonly Lazy<IExpression> _expression;

		public ReturnStatement(
			Return_statementContext context,
			int ordinalPositionInMethod,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			OrdinalPositionInMethod = ordinalPositionInMethod;
			_methodBodySymbolContext = methodBodySymbolContext.WithStatement(this);
			_expression = new Lazy<IExpression>(BindExpression);
		}

		private IExpression BindExpression()
		{
			return _context.expression().BindExpression(_methodBodySymbolContext, _diagnostics);
		}

		public IExpression Expression => _expression.Value;

		public int OrdinalPositionInMethod { get; }

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _expression.Value;
		}
	}
}
