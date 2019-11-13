using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.ErrorSymbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Immutable;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class ConditionalExpression : SymbolBase, IConditionalExpression
	{
		private readonly Conditional_expressionContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;

		private readonly Lazy<IExpression> _condition;
		private readonly Lazy<IExpression> _ifTrue;
		private readonly Lazy<IExpression> _ifFalse;
		private readonly Lazy<IType> _type;

		public ConditionalExpression(Conditional_expressionContext context, MethodBodySymbolContext methodBodySymbolContext, DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;

			_condition = new Lazy<IExpression>(BindCondition);
			_ifTrue = new Lazy<IExpression>(BindIfTrue);
			_ifFalse = new Lazy<IExpression>(BindIfFalse);
			_type = new Lazy<IType>(BindType);
		}

		private IExpression BindCondition()
		{
			return _context.expression(0).BindExpression(_methodBodySymbolContext, _diagnostics);
		}

		private IExpression BindIfTrue()
		{
			return _context.expression(1).BindExpression(_methodBodySymbolContext, _diagnostics);
		}

		private IExpression BindIfFalse()
		{
			return _context.expression(2).BindExpression(_methodBodySymbolContext, _diagnostics);
		}

		private IType BindType()
		{
			if (!Condition.Type.Equals(Primitive.Bool))
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.expression(0)),
					ErrorCode.NonBooleanCondition,
					ImmutableArray.Create<object?>(Condition.Type)));
			}
			if (SymbolHelpers.TryFindBestType(IfTrue.Type, IfFalse.Type, out var type))
			{
				return type;
			}

			_diagnostics.Add(new Diagnostic(
				new Location(_context.expression(0)),
				ErrorCode.NoBestType,
				ImmutableArray.Create<object?>(IfTrue.Type, IfFalse.Type)));

			return ErrorType.Instance;
		}

		public IExpression Condition => _condition.Value;
		public IExpression IfTrue => _ifTrue.Value;
		public IExpression IfFalse => _ifFalse.Value;

		public IType Type => _type.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _condition.Value;
			_ = _ifTrue.Value;
			_ = _ifFalse.Value;
			_ = _type.Value;
		}
	}
}
