using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class PipedStaticInvocationExpression : SymbolBase, IPipedStaticInvocationExpression
	{
		private readonly Piped_static_invocation_expressionContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;
		private readonly Lazy<ImmutableArray<IExpression>> _arguments;
		private readonly Lazy<IMethod> _method;
		private readonly Lazy<ImmutableArray<IType>> _typeArguments;

		public PipedStaticInvocationExpression(
			Piped_static_invocation_expressionContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;

			_methodName = _context.method_reference().qualified_name().GetQualifiedName();
			if (_methodName.Parent != null)
			{
				_diagnostics.Add(
					new Diagnostic(
						new Location(_context.method_reference().qualified_name()),
						ErrorCode.PipedStaticInvocationExpressionCannotHaveQualifiedName,
						ImmutableArray.Create<object?>(_methodName.ToString())));
			}

			_arguments = new Lazy<ImmutableArray<IExpression>>(BindArguments);
			_method = new Lazy<IMethod>(BindMethod);
			_typeArguments = new Lazy<ImmutableArray<IType>>(BindTypeArguments);
		}

		private ImmutableArray<IExpression> BindArguments()
		{
			return _context
				.invocation()
				.arguments()
				.expression()
				.Prepend(_context.expression())
				.Select(x => x.BindExpression(_methodBodySymbolContext, _diagnostics))
				.ToImmutableArray();
		}

		private ImmutableArray<IType> BindTypeArguments()
		{
			return _context.method_reference().type_argument_list().BindTypeArgumentList(
				_methodBodySymbolContext.SourceSymbolContext,
				_diagnostics);
		}

		private IMethod BindMethod()
		{
			return StaticInvocationMethodBinder.BindMethod(
				_methodName,
				TypeArguments,
				Arguments,
				_context.method_reference(),
				_methodBodySymbolContext,
				_diagnostics);
		}

		private readonly QualifiedName _methodName;

		public string MethodName => _methodName.Name;

		QualifiedName IStaticInvocationExpression.MethodName => _methodName;

		public ImmutableArray<IExpression> Arguments => _arguments.Value;

		public IMethod Method => _method.Value;

		public IType Type => Method.ReturnType;

		public ImmutableArray<IType> TypeArguments => _typeArguments.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			_ = _arguments.Value;
			_ = _typeArguments.Value;
			_ = _method.Value;
		}
	}
}
