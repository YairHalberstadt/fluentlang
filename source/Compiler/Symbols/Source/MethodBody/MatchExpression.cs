using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.ErrorSymbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class MatchExpression : SymbolBase, IMatchExpression
	{
		private readonly Match_expressionContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;
		private readonly Lazy<IExpression> _expression;
		private readonly Lazy<ImmutableArray<IMatchExpressionArm>> _matchExpressionArms;
		private readonly Lazy<IType> _type;

		public MatchExpression(
			Match_expressionContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;
			_expression = new Lazy<IExpression>(BindExpression);
			_matchExpressionArms = new Lazy<ImmutableArray<IMatchExpressionArm>>(BindMatchExpressionArms);
			_type = new Lazy<IType>(BindType);
		}

		private IExpression BindExpression()
		{
			var expression = _context.expression().BindExpression(_methodBodySymbolContext, _diagnostics);
			if (!(expression.Type is IUnion))
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.expression()),
					ErrorCode.CannotMatchOnNonUnion,
					ImmutableArray.Create<object?>(expression.Type)));
			}
			return expression;
		}

		private ImmutableArray<IMatchExpressionArm> BindMatchExpressionArms()
		{
			var arms =
				_context
				.match_expression_arm()
				.Select(x => new MatchExpressionArm(x, _methodBodySymbolContext, _diagnostics))
				.ToImmutableArray<IMatchExpressionArm>();

			if (Expression.Type is IUnion union)
			{
				foreach (var option in union.Options)
				{
					if (!arms.Any(x => option.IsSubtypeOf(x.Type)))
					{
						_diagnostics.Add(new Diagnostic(
							new Location(_context),
							ErrorCode.MatchNotExhaustive,
							ImmutableArray.Create<object?>(option)));
					}
				}
			}
			return arms;
		}

		private IType BindType()
		{
			var bestType =
				Arms
				.Select(x => x.Expression.Type)
				.FirstOrDefault(x =>
					Arms
					.All(y => y.Expression.Type.IsSubtypeOf(x)));

			if (bestType is null)
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context),
					ErrorCode.NoBestType,
					ImmutableArray.Create<object?>(Arms.Select(x => x.Expression.Type))));
			}

			return bestType ?? ErrorType.Instance;
		}

		public IExpression Expression => _expression.Value;

		public ImmutableArray<IMatchExpressionArm> Arms => _matchExpressionArms.Value;

		public IType Type => _type.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			_ = _expression.Value;
			_ = _matchExpressionArms.Value;
			_ = _type.Value;
		}
	}
}
