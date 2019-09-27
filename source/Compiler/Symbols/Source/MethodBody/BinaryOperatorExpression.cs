using System;
using System.Collections.Immutable;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.Symbols.ErrorSymbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class BinaryOperatorExpression : SymbolBase, IExpression, IBinaryOperatorExpression
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

		private IType BindType()
		{
			if (!(Left.Type is Primitive leftType) 
				|| leftType.Equals(Primitive.String) && !Operator.Equals(Operator.Plus))
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

			return Left.Type;
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
