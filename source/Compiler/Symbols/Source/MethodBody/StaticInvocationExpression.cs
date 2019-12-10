using System;
using System.Collections.Immutable;
using System.Linq;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.ErrorSymbols;
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

		public StaticInvocationExpression(
			Static_invocation_expressionContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;

			MethodName = _context.qualified_name().GetQualifiedName();
			_arguments = new Lazy<ImmutableArray<IExpression>>(BindArguments);
			_method = new Lazy<IMethod>(BindMethod);
		}

		private ImmutableArray<IExpression> BindArguments()
		{
			return
				_context
				.invocation()
				.BindArguments(_methodBodySymbolContext, _diagnostics);
		}

		private IMethod BindMethod()
		{
			var methods = _methodBodySymbolContext.SourceSymbolContext.GetPossibleMethods(MethodName);

			var matching = methods.Where(
				x =>
					x.Parameters.Length == Arguments.Length &&
					x.Parameters.Zip(Arguments, (p, a) => a.Type.IsSubtypeOf(p.Type)).All(x => x))
				.ToList();

			if (matching.Count == 1)
			{
				var target = matching[0];
				var currentMethod = _methodBodySymbolContext.SourceSymbolContext.Scope;
				Release.Assert(currentMethod != null);
				if (target.DeclaringMethod == currentMethod)
				{
					if (target.InScopeAfter is { } declarationStatement)
					{
						var currentStatement = _methodBodySymbolContext.CurrentStatement;
						Release.Assert(currentStatement != null);
						if (declarationStatement.OrdinalPositionInMethod >= currentStatement.OrdinalPositionInMethod)
						{
							_diagnostics.Add(new Diagnostic(
								new Location(_context),
								ErrorCode.UseOfMethodWhichCapturesUnassignedLocals,
								ImmutableArray.Create<object?>(target)));
						}
					}
				}
				return target;
			}
				

			if (matching.Count == 0)
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.qualified_name()),
					ErrorCode.MethodNotFound,
					ImmutableArray.Create<object?>(MethodName, Arguments)));
			}
			else
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.qualified_name()),
					ErrorCode.AmbigiousMethodReference,
					ImmutableArray.Create<object?>(matching)));
			}

			return new ErrorMethod(MethodName, Arguments.Length);
		}

		public QualifiedName MethodName { get; }

		public ImmutableArray<IExpression> Arguments => _arguments.Value;

		public IMethod Method => _method.Value;

		public IType Type => Method.ReturnType;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			_ = _arguments.Value;
			_ = _method.Value;
		}
	}
}
