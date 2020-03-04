using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Source.MethodBody;
using FluentLang.Compiler.Symbols.Visitor;
using System;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal sealed partial class SourceMethod : SymbolBase, IMethod
	{
		private readonly Method_declarationContext _context;
		private readonly MethodBodySymbolContext _parentMethodBodySymbolContext;
		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<ITypeParameter>> _typeParameters;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;
		private readonly Lazy<ImmutableArray<IInterface>> _localInterfaces;
		private readonly Lazy<ImmutableArray<IMethod>> _localMethods;
		private readonly Lazy<(ImmutableArray<IStatement> statements, MethodBodySymbolContext contextWithDeclaredLocals)> _bindStatementsResult;
		private readonly Lazy<ImmutableArray<IParameterLocal>> _parameterLocals;
		private readonly Lazy<ImmutableArray<IDeclaredLocal>> _directlyCapturedDeclaredLocals;
		private readonly Lazy<ImmutableArray<IMethod>> _invokedLocalMethods;
		private readonly Lazy<IDeclarationStatement?> _inScopeAfter;

		private readonly SourceSymbolContext _parentSourceSymbolContextWithTypeParameters;
		private readonly Lazy<MethodBodySymbolContext> _methodBodySymbolContext;
		private readonly Lazy<ImmutableArray<MethodOrInterfaceMethod>> _requiredMethodKeys;

		public SourceMethod(
			Method_declarationContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_parentMethodBodySymbolContext = methodBodySymbolContext;
			_parentSourceSymbolContextWithTypeParameters = _parentMethodBodySymbolContext.SourceSymbolContext.WithTypeParameters(() => TypeParameters);
			var @namespace = _parentSourceSymbolContextWithTypeParameters.Scope is null ? _parentSourceSymbolContextWithTypeParameters.NameSpace : null;
			FullyQualifiedName = new QualifiedName(context.method_signature().UPPERCASE_IDENTIFIER().Symbol.Text, @namespace);

			_methodBodySymbolContext =
				new Lazy<MethodBodySymbolContext>(
					() => _parentMethodBodySymbolContext.WithScope(this));

			_returnType = new Lazy<IType>(BindReturnType);
			_typeParameters = new Lazy<ImmutableArray<ITypeParameter>>(BindTypeParameters);
			_parameters = new Lazy<ImmutableArray<IParameter>>(BindParameters);
			_localInterfaces = new Lazy<ImmutableArray<IInterface>>(BindLocalInterfaces);
			_localMethods = new Lazy<ImmutableArray<IMethod>>(BindLocalMethods);
			_bindStatementsResult = new Lazy<(ImmutableArray<IStatement> statements, MethodBodySymbolContext contextWithDeclaredLocals)>(BindStatements);
			_parameterLocals = new Lazy<ImmutableArray<IParameterLocal>>(BindParameterLocals);
			_directlyCapturedDeclaredLocals = new Lazy<ImmutableArray<IDeclaredLocal>>(CalculateDirectlyCapturedDeclaredLocals);
			_invokedLocalMethods = new Lazy<ImmutableArray<IMethod>>(CalculateInvokedLocalMethods);
			_inScopeAfter = new Lazy<IDeclarationStatement?>(((IMethod)this).CalculateInScopeAfter);
			_requiredMethodKeys = new Lazy<ImmutableArray<MethodOrInterfaceMethod>>(CalculateRequiredMethodKeys);
		}

		private IType BindReturnType()
		{
			return _context.method_signature().BindReturnType(_parentSourceSymbolContextWithTypeParameters, IsExported, _diagnostics);
		}

		private ImmutableArray<ITypeParameter> BindTypeParameters()
		{
			return
				_context
				.method_signature()
				.type_parameter_list()
				.BindTypeParameters(_parentMethodBodySymbolContext.SourceSymbolContext, IsExported, _diagnostics);
		}

		private ImmutableArray<IParameter> BindParameters()
		{
			return
				_context
				.method_signature()
				.BindParameters(_parentSourceSymbolContextWithTypeParameters, IsExported, _diagnostics);
		}

		private ImmutableArray<IInterface> BindLocalInterfaces()
		{
			var interfaceDeclarations =
				_context
				.method_body()
				.interface_declaration();

			foreach (var duplicateGroup in interfaceDeclarations
				.GroupBy(x => x.UPPERCASE_IDENTIFIER().Symbol.Text)
				.Where(x => x.Count() > 1))
			{
				foreach (var duplicate in duplicateGroup)
				{
					_diagnostics.Add(new Diagnostic(
							new Location(duplicate.UPPERCASE_IDENTIFIER()),
							ErrorCode.DuplicateInterfaceDeclaration,
							ImmutableArray.Create<object?>(duplicateGroup.Key)));
				}
			}

			return
				interfaceDeclarations
				.Select(x => new SourceNamedInterface(
					x,
					_methodBodySymbolContext.Value.SourceSymbolContext,
					_diagnostics))
				.ToImmutableArray<IInterface>();
		}

		private ImmutableArray<IMethod> BindLocalMethods()
		{
			var methodDeclarations =
				_context
				.method_body()
				.method_declaration();

			foreach (var duplicateGroup in methodDeclarations
				.GroupBy(x => x.method_signature().UPPERCASE_IDENTIFIER().Symbol.Text)
				.Where(x => x.Count() > 1))
			{
				foreach (var duplicate in duplicateGroup)
				{
					_diagnostics.Add(new Diagnostic(
							new Location(duplicate.method_signature().UPPERCASE_IDENTIFIER()),
							ErrorCode.DuplicateMethodDeclaration,
							ImmutableArray.Create<object?>(duplicateGroup.Key)));
				}
			}

			return
				methodDeclarations
				.Select(x => new SourceMethod(
					x,
					_bindStatementsResult.Value.contextWithDeclaredLocals,
					_diagnostics))
				.ToImmutableArray<IMethod>();
		}

		private (ImmutableArray<IStatement>, MethodBodySymbolContext contextWithDeclaredLocals) BindStatements()
		{
			var methodBodySymbolContext = _methodBodySymbolContext.Value;
			var statementContexts = _context.method_body().method_statement();
			var builder = ImmutableArray.CreateBuilder<IStatement>(statementContexts.Length);

			foreach (var (statement, index) in statementContexts.Select((x, i) => (x, i)))
			{
				builder.Add(statement.BindStatement(index, methodBodySymbolContext, _diagnostics, out var local));
				if (local != null)
					methodBodySymbolContext = methodBodySymbolContext.WithLocal(local);
			}

			var statements = builder.MoveToImmutable();

			return (statements, methodBodySymbolContext);
		}

		private void CheckStatementsForDiagnostics()
		{
			if (Statements.Length == 0)
			{
				_diagnostics.Add(new Diagnostic(
						new Location(_context.method_signature().UPPERCASE_IDENTIFIER()),
						ErrorCode.MethodMustContainAtLeastOneStatement,
						ImmutableArray.Create<object?>(this)));

				return;
			}

			var lastStatement = Statements.Last();
			if (lastStatement is IReturnStatement returnStatement)
			{
				if (!returnStatement.Expression.Type.IsSubtypeOf(ReturnType))
				{
					_diagnostics.Add(new Diagnostic(
						new Location(_context.method_body().method_statement().Last()),
						ErrorCode.ReturnTypeDoesNotMatch,
						ImmutableArray.Create<object?>(this, ReturnType, returnStatement.Expression.Type)));
				}
			}
			else
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.method_body().method_statement().Last()),
					ErrorCode.LastStatementMustBeReturnStatement,
					ImmutableArray.Create<object?>(this, lastStatement)));
			}

			for (var i = 0; i < Statements.Length - 1; i++)
			{
				var statement = Statements[i];
				if (statement is IReturnStatement)
				{
					_diagnostics.Add(new Diagnostic(
						new Location(_context.method_body().method_statement(i)),
						ErrorCode.OnlyLastStatementCanBeReturnStatement,
						ImmutableArray.Create<object?>(this, statement)));
				}
			}
		}

		private ImmutableArray<IParameterLocal> BindParameterLocals()
		{
			return Parameters.Select((x, i) =>
			{
				var local = new ParameterLocal(x);
				if (_parentMethodBodySymbolContext.Locals.Any(x => x.Identifier == local.Identifier))
					_diagnostics.Add(new Diagnostic(
						new Location(_context.method_signature().parameters().parameter(i).LOWERCASE_IDENTIFIER()),
						ErrorCode.HidesLocal,
						ImmutableArray.Create<object?>(local, _parentMethodBodySymbolContext.Locals.First(x => x.Identifier == local.Identifier))));
				return local;
			}).ToImmutableArray<IParameterLocal>();
		}

		private ImmutableArray<IDeclaredLocal> CalculateDirectlyCapturedDeclaredLocals()
		{
			var declaredLocals =
				Statements
				.OfType<IDeclarationStatement>()
				.Select(x => x.Local)
				.Where(x => x != null)
				.ToHashSet();

			return
				Statements
				.SelectMany(x => x.DescendantNodes())
				.OfType<ILocalReferenceExpression>()
				.Select(x => x.Local)
				.OfType<IDeclaredLocal>()
				.Where(x => !declaredLocals.Contains(x))
				.Distinct()
				.ToImmutableArray();
		}

		private ImmutableArray<IMethod> CalculateInvokedLocalMethods()
		{
			return
				Statements
				.SelectMany(x => x.DescendantNodes())
				.OfType<IStaticInvocationExpression>()
				.Select(x => x.Method)
				.Where(x => x.DeclaringMethod != null)
				.Distinct()
				.ToImmutableArray();
		}

		private ImmutableArray<MethodOrInterfaceMethod> CalculateRequiredMethodKeys()
		{
			var visitor = new RequiredMethodKeysVisitor(this);
			visitor.Visit(this);
			return visitor._methods?.ToImmutableArray()  ?? ImmutableArray<MethodOrInterfaceMethod>.Empty;
		}

		public bool IsExported => _context.EXPORT() is { };

		public QualifiedName FullyQualifiedName { get; }

		public IType ReturnType => _returnType.Value;

		public ImmutableArray<ITypeParameter> TypeParameters => _typeParameters.Value;

		public ImmutableArray<IParameter> Parameters => _parameters.Value;

		public ImmutableArray<IInterface> LocalInterfaces => _localInterfaces.Value;

		public ImmutableArray<IMethod> LocalMethods => _localMethods.Value;

		public IMethod? DeclaringMethod => _parentSourceSymbolContextWithTypeParameters.Scope;

		public IAssembly DeclaringAssembly => _parentSourceSymbolContextWithTypeParameters.Assembly;

		public ImmutableArray<IStatement> Statements => _bindStatementsResult.Value.statements;

		public ImmutableArray<IParameterLocal> ParameterLocals => _parameterLocals.Value;

		public IDeclarationStatement? InScopeAfter => _inScopeAfter.Value;

		ImmutableArray<IDeclaredLocal> IMethod.DirectlyCapturedDeclaredLocals => _directlyCapturedDeclaredLocals.Value;

		ImmutableArray<IMethod> IMethod.InvokedLocalMethods => _invokedLocalMethods.Value;

		ImmutableArray<MethodOrInterfaceMethod> IMethod.RequiredMethodKeys => _requiredMethodKeys.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _returnType.Value;
			_ = _parameters.Value;
			_ = _localInterfaces.Value;
			_ = _localMethods.Value;
			_ = _bindStatementsResult.Value;
			_ = _parameterLocals.Value;
			_ = _inScopeAfter.Value;
			_ = _invokedLocalMethods.Value;
			_ = _directlyCapturedDeclaredLocals.Value;
			_ = _typeParameters.Value;
			CheckStatementsForDiagnostics();
		}
	}
}