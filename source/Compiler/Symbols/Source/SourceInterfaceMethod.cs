using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal class SourceInterfaceMethod : IInterfaceMethod
	{
		private readonly Interface_member_declarationContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;

		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;

		private readonly DiagnosticBag _diagnostics;
		private readonly Lazy<ImmutableArray<Diagnostic>> _allDiagnostics;

		public SourceInterfaceMethod(Interface_member_declarationContext context, SourceSymbolContext sourceSymbolContext, DiagnosticBag diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			_diagnostics = diagnostics.CreateChildBag(this);
			Name = context.method_signature().UPPERCASE_IDENTIFIER().Symbol.Text;

			_returnType = new Lazy<IType>(BindReturnType);
			_parameters = new Lazy<ImmutableArray<IParameter>>(BindParameters);

			_allDiagnostics = new Lazy<ImmutableArray<Diagnostic>>(() =>
			{
				_diagnostics.EnsureAllDiagnosticsCollectedForSymbol();
				return _diagnostics.ToImmutableArray();
			});
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

		public string Name { get; }

		public IType ReturnType => _returnType.Value;

		public ImmutableArray<IParameter> Parameters => _parameters.Value;

		public ImmutableArray<Diagnostic> AllDiagnostics => _allDiagnostics.Value;

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _returnType.Value;
			_ = _parameters.Value;
		}
	}
}

