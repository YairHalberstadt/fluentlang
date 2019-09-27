using System;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using Antlr4.Runtime.Tree.Xpath;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class MemberInvocationExpression : SymbolBase, IMemberInvocationExpression
	{
		private readonly Member_invocation_expressionContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;

		private readonly Lazy<IExpression> _expression;
		private readonly Lazy<ImmutableArray<IExpression>> _arguments;
		private readonly Lazy<IInterfaceMethod> _method;
		private readonly Lazy<IType> _type;

		public MemberInvocationExpression(
			Member_invocation_expressionContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;

			MemberName = _context.UPPERCASE_IDENTIFIER().Symbol.Text;
			_expression = new Lazy<IExpression>(BindExpression);
			_arguments = new Lazy<ImmutableArray<IExpression>>(BindArguments);
			_method = new Lazy<IInterfaceMethod>(BindMethod);
			_type = new Lazy<IType>(BindType);
		}

		private IExpression BindExpression()
		{
			return _context.expression().BindExpression(_methodBodySymbolContext, _diagnostics);
		}

		private ImmutableArray<IExpression> BindArguments()
		{
			return 
				_context
				.invocation()
				.arguments()
				.expression()
				.Select(x => x.BindExpression(_methodBodySymbolContext, _diagnostics))
				.ToImmutableArray();
		}

		private IInterfaceMethod BindMethod()
		{
			var type = Expression.Type;

			var methods = type is IInterface @interface ? @interface.Methods : ImmutableArray<IInterfaceMethod>.Empty;

			var matching = methods.Where(
				x =>
					x.Name == MemberName &&
					x.Parameters.Length == Arguments.Length &&
					x.Parameters.Zip(Arguments, (p, a) => a.Type.IsSubtypeOf(p.Type)).All(x => x)).ToList();

			if (matching.Count == 1)
				return matching[0];

			if (matching.Count == 0)
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.UPPERCASE_IDENTIFIER()),
					ErrorCode.MemberNotFound,
					ImmutableArray.Create<object?>(type, MemberName, Arguments)));
			}
			else if(matching.Count == 2)
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.UPPERCASE_IDENTIFIER()),
					ErrorCode.AmbigiousMethodReference,
					ImmutableArray.Create<object?>(matching)));
			}
		}

		private IType BindType()
		{
			throw new NotImplementedException();
		}

		public string MemberName { get; }

		public IExpression Expression => _expression.Value;

		public ImmutableArray<IExpression> Arguments => _arguments.Value;

		public IInterfaceMethod Method => _method.Value;

		public IType Type => _type.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			throw new System.NotImplementedException();
		}
	}
}
