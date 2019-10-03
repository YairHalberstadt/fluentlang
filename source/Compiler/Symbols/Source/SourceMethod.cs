using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Source.MethodBody;
using System;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal sealed class SourceMethod : SymbolBase, IMethod
	{
		private readonly Method_declarationContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;
		private SourceSymbolContext? _childSourceSymbolContext;

		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;
		private readonly Lazy<ImmutableArray<IInterface>> _localInterfaces;
		private readonly Lazy<ImmutableArray<IMethod>> _localMethods;
		private readonly Lazy<ImmutableArray<IStatement>> _statements;

		public SourceMethod(
			Method_declarationContext context,
			SourceSymbolContext sourceSymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			var @namespace = _sourceSymbolContext.Scope is null ? _sourceSymbolContext.NameSpace : null;
			FullyQualifiedName = new QualifiedName(context.method_signature().UPPERCASE_IDENTIFIER().Symbol.Text, @namespace);

			_returnType = new Lazy<IType>(BindReturnType);
			_parameters = new Lazy<ImmutableArray<IParameter>>(BindParameters);
			_localInterfaces = new Lazy<ImmutableArray<IInterface>>(BindLocalInterfaces);
			_localMethods = new Lazy<ImmutableArray<IMethod>>(BindLocalMethods);
			_statements = new Lazy<ImmutableArray<IStatement>>(BindStatements);
		}

		private IType BindReturnType()
		{
			return _context.method_signature().BindReturnType(_sourceSymbolContext, _diagnostics);
		}

		private ImmutableArray<IParameter> BindParameters()
		{
			return
				_context
				.method_signature()
				.BindParameters(_sourceSymbolContext, _diagnostics);
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
				.Select(x => new SourceInterface(
					x.anonymous_interface_declaration(),
					_childSourceSymbolContext ??= _sourceSymbolContext.WithScope(this),
					fullyQualifiedName: new QualifiedName(x.UPPERCASE_IDENTIFIER().Symbol.Text),
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
					_childSourceSymbolContext ??= _sourceSymbolContext.WithScope(this),
					_diagnostics))
				.ToImmutableArray<IMethod>();
		}

		private ImmutableArray<IStatement> BindStatements()
		{
			var methodBodySymbolContext = new MethodBodySymbolContext(
				_sourceSymbolContext,
				Parameters.Select(x => new ParameterLocal(x)).ToImmutableList<ILocal>());
			var statementContexts = _context.method_body().method_statement();
			var builder = ImmutableArray.CreateBuilder<IStatement>(statementContexts.Length);

			foreach (var statement in statementContexts)
			{
				builder.Add(statement.BindStatement(methodBodySymbolContext, _diagnostics, out var local));
				if (local != null)
					methodBodySymbolContext = methodBodySymbolContext.WithLocal(local);
			}

			var statements = builder.MoveToImmutable();

			var lastStatement = statements.Last();
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

			for (var i = 0; i < statements.Length - 1; i++)
			{
				var statement = statements[i];
				if (statement is IReturnStatement)
				{
					_diagnostics.Add(new Diagnostic(
						new Location(_context.method_body().method_statement(i)),
						ErrorCode.OnlyLastStatementCanBeReturnStatement,
						ImmutableArray.Create<object?>(this, statement)));
				}
			}

			return statements;
		}

		public QualifiedName FullyQualifiedName { get; }

		public IType ReturnType => _returnType.Value;

		public ImmutableArray<IParameter> Parameters => _parameters.Value;

		public ImmutableArray<IInterface> LocalInterfaces => _localInterfaces.Value;

		public ImmutableArray<IMethod> LocalMethods => _localMethods.Value;

		public IMethod? DeclaringMethod => _sourceSymbolContext.Scope;

		public ImmutableArray<IStatement> Statements => _statements.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _returnType.Value;
			_ = _parameters.Value;
			_ = _localInterfaces.Value;
			_ = _localMethods.Value;
			_ = _statements.Value;
		}
	}
}

