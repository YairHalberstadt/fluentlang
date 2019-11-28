using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class DeclarationStatement : SymbolBase, IDeclarationStatement
	{
		private readonly Declaration_statementContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;
		private readonly Lazy<IType?> _declaredType;
		private readonly Lazy<IExpression> _expression;
		private readonly Lazy<IType> _type;
		private readonly Lazy<IDeclaredLocal?> _local;

		public DeclarationStatement(
			Declaration_statementContext context,
			int ordinalPositionInMethod,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			OrdinalPositionInMethod = ordinalPositionInMethod;
			_methodBodySymbolContext = methodBodySymbolContext.WithStatement(this);
			IdentifierName = _context.LOWERCASE_IDENTIFIER()?.Symbol.Text ?? null;
			_declaredType = new Lazy<IType?>(BindDeclaredType);
			_expression = new Lazy<IExpression>(BindExpression);
			_type = new Lazy<IType>(BindType);
			_local = new Lazy<IDeclaredLocal?>(BindLocal);
		}

		private IDeclaredLocal? BindLocal()
		{
			if (_context.DISCARD() is { })
				return null;
			var local = new DeclaredLocal(this);
			if (_methodBodySymbolContext.Locals.Any(x => x.Identifier == IdentifierName))
				_diagnostics.Add(new Diagnostic(
					new Location(_context.LOWERCASE_IDENTIFIER()),
					ErrorCode.HidesLocal,
					ImmutableArray.Create<object?>(local, _methodBodySymbolContext.Locals.First(x => x.Identifier == IdentifierName))));
			return local;
		}

		private IType? BindDeclaredType()
		{
			return _context.type_declaration()?.type().BindType(_methodBodySymbolContext.SourceSymbolContext, _diagnostics);
		}

		private IExpression BindExpression()
		{
			return _context.expression().BindExpression(_methodBodySymbolContext, _diagnostics);
		}

		private IType BindType()
		{
			if (DeclaredType is null)
				return Expression.Type;
			if (!Expression.Type.IsSubtypeOf(DeclaredType))
			{
				_diagnostics.Add(
					new Diagnostic(
						new Location(_context.expression()), 
						ErrorCode.MismatchedTypes, 
						ImmutableArray.Create<object?>(DeclaredType, Expression.Type)));
			}
			return DeclaredType;
		}

		public string? IdentifierName { get; }
		public IType? DeclaredType => _declaredType.Value;
		public IExpression Expression => _expression.Value;
		public IType Type => _type.Value;
		public IDeclaredLocal? Local => _local.Value;

		public int OrdinalPositionInMethod { get; }

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _local.Value;
			_ = _declaredType.Value;
			_ = _expression.Value;
			_ = _type.Value;
		}
	}
}
