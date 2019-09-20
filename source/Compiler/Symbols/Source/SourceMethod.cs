using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal class SourceMethod : IMethod
	{
		private readonly Method_declarationContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;
		private SourceSymbolContext? _childSourceSymbolContext;
		private readonly DiagnosticBag _diagnostics;

		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;
		private readonly Lazy<ImmutableArray<IInterface>> _localInterfaces;
		private readonly Lazy<ImmutableArray<IMethod>> _localMethods;

		public SourceMethod(
			Method_declarationContext context,
			SourceSymbolContext sourceSymbolContext,
			DiagnosticBag diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			var @namespace = _sourceSymbolContext.Scope is null ? _sourceSymbolContext.NameSpace : null;
			FullyQualifiedName = new QualifiedName(context.method_signature().UPPERCASE_IDENTIFIER().Symbol.Text, @namespace);
			_diagnostics = diagnostics.CreateChildBag();

			_returnType = new Lazy<IType>(BindReturnType);
			_parameters = new Lazy<ImmutableArray<IParameter>>(BindParameters);
			_localInterfaces = new Lazy<ImmutableArray<IInterface>>(BindLocalInterfaces);
			_localMethods = new Lazy<ImmutableArray<IMethod>>(BindLocalMethods);
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
			return
				_context
				.method_body()
				.interface_declaration()
				.Select(x => new SourceInterface(
					x.anonymous_interface_declaration(),
					_childSourceSymbolContext ??= _sourceSymbolContext.WithScope(this),
					fullyQualifiedName: new QualifiedName(x.UPPERCASE_IDENTIFIER().Symbol.Text),
					_diagnostics))
				.ToImmutableArray<IInterface>();
		}

		private ImmutableArray<IMethod> BindLocalMethods()
		{
			return
				_context
				.method_body()
				.method_declaration()
				.Select(x => new SourceMethod(
					x,
					_childSourceSymbolContext ??= _sourceSymbolContext.WithScope(this),
					_diagnostics))
				.ToImmutableArray<IMethod>();
		}

		public QualifiedName FullyQualifiedName { get; }

		public IType ReturnType => _returnType.Value;

		public ImmutableArray<IParameter> Parameters => _parameters.Value;

		public ImmutableArray<IInterface> LocalInterfaces => _localInterfaces.Value;

		public ImmutableArray<IMethod> LocalMethods => _localMethods.Value;

		public IMethod? DeclaringMethod => _sourceSymbolContext.Scope;
	}
}

