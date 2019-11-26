using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Immutable;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class BinaryOperatorExpression : SymbolBase, IBinaryOperatorExpression
	{
		private readonly Lazy<IExpression> _left;
		private readonly Lazy<IExpression> _right;
		private readonly Lazy<Operator> _operator;
		private readonly Lazy<IType> _type;
		private readonly Binary_operator_expressionContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;

		public BinaryOperatorExpression(
			Binary_operator_expressionContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;

			_left = new Lazy<IExpression>(BindLeft);
			_right = new Lazy<IExpression>(BindRight);
			_operator = new Lazy<Operator>(BindOperator);
			_type = new Lazy<IType>(BindType);
		}

		private IExpression BindLeft()
		{
			return _context.expression(0).BindExpression(_methodBodySymbolContext, _diagnostics);
		}

		private IExpression BindRight()
		{
			return _context.expression(1).BindExpression(_methodBodySymbolContext, _diagnostics);
		}

		private Operator BindOperator()
		{
			return _context.@operator().Start.Type switch
			{
				PLUS => Operator.Plus,
				MINUS => Operator.Minus,
				STAR => Operator.Multiply,
				DIV => Operator.Divide,
				PERCENT => Operator.Remainder,
				LT => Operator.LessThan,
				GT => Operator.GreaterThan,
				OP_EQ => Operator.Equal,
				OP_NE => Operator.NotEqual,
				OP_LE => Operator.LessThanOrEqualTo,
				OP_GE => Operator.GreaterThanOrEqualTo,
				var type => throw new InvalidOperationException($"Unexpected operator: {type}")
			};
		}

		private static bool OperatorIsDefinedOnType(Operator op, Primitive primitive)
		{
			if (op.Equals(Operator.Equal)
				|| op.Equals(Operator.GreaterThan)
				|| op.Equals(Operator.GreaterThanOrEqualTo)
				|| op.Equals(Operator.LessThan)
				|| op.Equals(Operator.LessThanOrEqualTo)
				|| op.Equals(Operator.NotEqual))
				return true;

			if (op.Equals(Operator.Plus))
				return primitive.Equals(Primitive.String)
					|| primitive.Equals(Primitive.Double)
					|| primitive.Equals(Primitive.Int);

			if (op.Equals(Operator.Minus)
				|| op.Equals(Operator.Multiply)
				|| op.Equals(Operator.Divide))
				return primitive.Equals(Primitive.Double)
					|| primitive.Equals(Primitive.Int);

			if (op.Equals(Operator.Remainder))
				return primitive.Equals(Primitive.Int);

			throw Release.Fail($"unexpected operator: {op}");
		}

		private static bool OperatorAlwaysReturnsBoolean(Operator op)
		{
			return op.Equals(Operator.Equal)
				|| op.Equals(Operator.GreaterThan)
				|| op.Equals(Operator.GreaterThanOrEqualTo)
				|| op.Equals(Operator.LessThan)
				|| op.Equals(Operator.LessThanOrEqualTo)
				|| op.Equals(Operator.NotEqual);
		}

		private IType BindType()
		{
			if (!(Left.Type is Primitive leftType)
				|| !OperatorIsDefinedOnType(Operator, leftType))
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.expression(0)),
					ErrorCode.InvalidArgument,
					ImmutableArray.Create<object?>(Left.Type, Operator)));
			}
			else
			{
				if (!leftType.Equals(Right.Type))
				{
					_diagnostics.Add(new Diagnostic(
						new Location(_context.expression(0)),
						ErrorCode.InvalidArgument,
						ImmutableArray.Create<object?>(Right.Type, Operator)));
				}
			}

			return OperatorAlwaysReturnsBoolean(Operator) ? Primitive.Bool : Left.Type;
		}

		public IExpression Left => _left.Value;
		public IExpression Right => _right.Value;
		public Operator Operator => _operator.Value;
		public IType Type => _type.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _left.Value;
			_ = _right.Value;
			_ = _operator.Value;
			_ = _type.Value;
		}
	}
}
