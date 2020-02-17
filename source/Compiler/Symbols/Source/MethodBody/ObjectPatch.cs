using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Immutable;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class ObjectPatch : SymbolBase, IObjectPatch
	{
		private readonly Object_patchContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;
		private readonly Lazy<IMethod?> _method;
		private readonly Lazy<IExpression?> _mixedInExpression;

		public ObjectPatch(
			Object_patchContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;
			_method = new Lazy<IMethod?>(BindMethod);
			_mixedInExpression = new Lazy<IExpression?>(BindMixedInExpression);
		}

		private IMethod? BindMethod()
		{
			if (_context.method_reference() is { } methodReference)
			{
				var method = _methodBodySymbolContext.SourceSymbolContext.GetMethodOrError(
					methodReference.qualified_name().GetQualifiedName(),
					methodReference.type_argument_list().BindTypeArgumentList(
                        _methodBodySymbolContext.SourceSymbolContext,
                        _diagnostics),
					out var diagnostic);
				if (diagnostic != null)
					_diagnostics.Add(diagnostic(new Location(methodReference)));
				return method;
			}
			return null;
		}

		private IExpression? BindMixedInExpression()
		{
			if (_context.expression() is { } expressionContext)
			{
				var expression = expressionContext.BindExpression(_methodBodySymbolContext, _diagnostics);
				if (!(expression.Type is IInterface)) //TODO: do we want to make this check lazy?
				{
					_diagnostics.Add(new Diagnostic(new Location(expressionContext), ErrorCode.CannotMixInNonInterface, ImmutableArray.Create<object?>(expression.Type)));
				}
				return expression;
			}
			return null;
		}

		public IMethod? Method => _method.Value;
		public IExpression? MixedInExpression => _mixedInExpression.Value;



		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _method.Value;
			_ = _mixedInExpression.Value;
		}
	}
}
