using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class MatchExpressionArm : SymbolBase, IMatchExpressionArm
	{
		private readonly Match_expression_armContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;
		private readonly Lazy<IType> _type;
		private readonly Lazy<IExpression> _expression;

		public MatchExpressionArm(
			Match_expression_armContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;

			IdentifierName = _context.LOWERCASE_IDENTIFIER()?.Symbol.Text;
			Local = IdentifierName is null ? null : new MatchExpressionArmLocal(this);
			_type = new Lazy<IType>(BindType);
			_expression = new Lazy<IExpression>(BindExpression);
		}

		private IType BindType()
		{
			return _context.type().BindType(_methodBodySymbolContext.SourceSymbolContext, false, _diagnostics);
		}

		private IExpression BindExpression()
		{
			return _context.expression().BindExpression(
				Local is { } local ? _methodBodySymbolContext.WithLocal(local) : _methodBodySymbolContext,
				_diagnostics);
		}

		public IType Type => _type.Value;

		public string? IdentifierName { get; }

		public ILocal? Local { get; }

		public IExpression Expression => _expression.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			_ = _type.Value;
			_ = _expression.Value;
		}

		private class MatchExpressionArmLocal : ILocal
		{
			private readonly MatchExpressionArm _matchExpressionArm;

			public MatchExpressionArmLocal(MatchExpressionArm matchExpressionArm)
			{
				_matchExpressionArm = matchExpressionArm;
			}

			public string Identifier => _matchExpressionArm.IdentifierName ?? throw Release.Fail("Unreachable");

			public IType Type => _matchExpressionArm.Type;
		}
	}
}
