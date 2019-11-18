using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Immutable;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class PrefixUnaryOperatorExpression : SymbolBase, IExpression, IPrefixUnaryOperatorExpression
	{
		private readonly Lazy<IExpression> _expression;
		private readonly Lazy<Operator> _operator;
		private readonly Lazy<IType> _type;
		private readonly Prefix_unary_operator_expressionContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;

		public PrefixUnaryOperatorExpression(
			Prefix_unary_operator_expressionContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;

			_expression = new Lazy<IExpression>(BindExpression);
			_operator = new Lazy<Operator>(BindOperator);
			_type = new Lazy<IType>(BindType);
		}

		private IExpression BindExpression()
		{
			return _context.expression().BindExpression(_methodBodySymbolContext, _diagnostics);
		}

		private Operator BindOperator()
		{
			return _context.prefix_unary_operator().Start.Type switch
			{
				MINUS => Operator.Minus,
				var type => throw new InvalidOperationException($"Unexpected operator: {type}")
			};
		}

		private static bool OperatorIsDefinedOnType(Operator op, Primitive primitive)
		{
			if (op.Equals(Operator.Minus))
				return primitive.Equals(Primitive.Double)
					|| primitive.Equals(Primitive.Int);

			throw Release.Fail($"unexpected operator: {op}");
		}

		private IType BindType()
		{
			if (!(Expression.Type is Primitive expType)
				|| !OperatorIsDefinedOnType(Operator, expType))
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.expression()),
					ErrorCode.InvalidArgument,
					ImmutableArray.Create<object?>(Expression.Type, Operator)));
			}

			return Expression.Type;
		}

		public IExpression Expression => _expression.Value;
		public Operator Operator => _operator.Value;
		public IType Type => _type.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _expression.Value;
			_ = _operator.Value;
			_ = _type.Value;
		}
	}
}
