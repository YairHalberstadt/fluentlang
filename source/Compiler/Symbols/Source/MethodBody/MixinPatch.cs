using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Immutable;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class MixinPatch : SymbolBase, IMixinPatch
	{
		private readonly ExpressionContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;
		private readonly Lazy<IExpression> _expression;

		public MixinPatch(
			ExpressionContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;
			_expression = new Lazy<IExpression>(BindExpression);
		}

		private IExpression BindExpression()
		{
			var expression = _context.BindExpression(_methodBodySymbolContext, _diagnostics);
			if (!(expression.Type is IInterface)) //TODO: do we want to make this check lazy?
			{
				_diagnostics.Add(new Diagnostic(new Location(_context), ErrorCode.CannotMixInNonInterface, ImmutableArray.Create<object?>(expression.Type)));
			}
			return expression;
		}

		public IExpression Expression => _expression.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _expression.Value;
		}
	}
}
