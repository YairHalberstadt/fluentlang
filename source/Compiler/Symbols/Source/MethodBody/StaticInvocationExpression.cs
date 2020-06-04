using System;
using System.Collections.Immutable;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class StaticInvocationExpression : SymbolBase, IStaticInvocationExpression
	{
		private readonly Static_invocation_expressionContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;
		private readonly Lazy<ImmutableArray<IExpression>> _arguments;
		private readonly Lazy<IMethod> _method;
		private readonly Lazy<ImmutableArray<IType>> _typeArguments;

		public StaticInvocationExpression(
			Static_invocation_expressionContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;

			MethodName = _context.method_reference().qualified_name().GetQualifiedName();
			_arguments = new Lazy<ImmutableArray<IExpression>>(BindArguments);
			_method = new Lazy<IMethod>(BindMethod);
			_typeArguments = new Lazy<ImmutableArray<IType>>(BindTypeArguments);
		}

		private ImmutableArray<IExpression> BindArguments()
		{
			return
				_context
				.invocation()
				.BindArguments(_methodBodySymbolContext, _diagnostics);
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
                MethodName,
                TypeArguments,
                Arguments,
                _context.method_reference(),
                _methodBodySymbolContext,
                _diagnostics);
		}

		public QualifiedName MethodName { get; }

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
