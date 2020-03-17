using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Immutable;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class MethodPatch : SymbolBase, IMethodPatch
	{
		private readonly Method_referenceContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;
		private readonly Lazy<IMethod> _method;
		private readonly Lazy<ImmutableArray<IType>> _typeArguments;

		public MethodPatch(
			Method_referenceContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;
			_method = new Lazy<IMethod>(BindMethod);
			_typeArguments = new Lazy<ImmutableArray<IType>>(BindTypeArguments);
		}

		private ImmutableArray<IType> BindTypeArguments()
		{
			return
				_context
				.type_argument_list()
				.BindTypeArgumentList(
					_methodBodySymbolContext.SourceSymbolContext,
					_diagnostics);
		}

		private IMethod BindMethod()
		{
			var method = _methodBodySymbolContext.SourceSymbolContext.GetMethodOrError(
				_context.qualified_name().GetQualifiedName(),
				TypeArguments,
				out var diagnostic);
			if (diagnostic != null)
				_diagnostics.Add(diagnostic(new Location(_context)));

			_methodBodySymbolContext.WarnIfUseOfMethodWhichCapturesUnassignedLocals(method, _diagnostics, _context);

			return method;
		}

		public IMethod Method => _method.Value;
		public ImmutableArray<IType> TypeArguments => _typeArguments.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _method.Value;
			_ = _typeArguments.Value;
		}
	}
}
