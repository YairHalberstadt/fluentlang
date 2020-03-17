using System;
using System.Collections.Generic;
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
			var methods = _methodBodySymbolContext.SourceSymbolContext.GetPossibleMethods(MethodName, TypeArguments);

			var matching =
				methods
				.Where(x => x.Parameters.Length == Arguments.Length)
				.Select(x =>
				{
					Diagnostic? diagnostic = null;
					if (TypeArguments.Length > 0)
					{
						var typeParameters = x.TypeParameters;
						if (!SourceSymbolContextExtensions.HasValidTypeArguments(
							TypeArguments,
							typeParameters,
							out var diagnosticFunc))
						{
							diagnostic = diagnosticFunc(new Location(_context.method_reference()));
						}
						var substituted = x.Substitute(
							SourceSymbolContextExtensions.CreateTypeMap(TypeArguments, typeParameters),
							new Dictionary<IType, IType>());
						
						return (method: substituted, diagnostic);
					}
					return (method: x, diagnostic: null);
				})
				.Where(x => x.method.Parameters.Zip(Arguments, (p, a) => a.Type.IsSubtypeOf(p.Type)).All(x => x))
				.ToList();

			if (matching.Count == 1)
			{
				var (target, diagnostic) = matching[0];

				if (diagnostic != null)
				{
					_diagnostics.Add(diagnostic);
				}

				_methodBodySymbolContext.WarnIfUseOfMethodWhichCapturesUnassignedLocals(target, _diagnostics, _context.method_reference());
				return target;
			}
				

			if (matching.Count == 0)
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.method_reference()),
					ErrorCode.MethodNotFound,
					ImmutableArray.Create<object?>(MethodName, Arguments)));
			}
			else
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.method_reference()),
					ErrorCode.AmbigiousMethodReference,
					ImmutableArray.Create<object?>(matching)));
			}

			return new ErrorMethod(MethodName, Arguments.Length);
		}

		public QualifiedName MethodName { get; }

		public ImmutableArray<IExpression> Arguments => _arguments.Value;

		public IMethod Method => _method.Value;

		public IType Type => Method.ReturnType;

		public ImmutableArray<IType> TypeArguments => _typeArguments.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			_ = _arguments.Value;
			_ = _method.Value;
		}
	}
}
