using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal sealed class SourceInterfaceMethod : SymbolBase, IInterfaceMethod
	{
		private readonly Method_signatureContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;
		private readonly bool _isExported;
		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;

		public SourceInterfaceMethod(
			Method_signatureContext context,
			SourceSymbolContext sourceSymbolContext,
			bool isExported,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			_isExported = isExported;
			Name = context.UPPERCASE_IDENTIFIER().Symbol.Text;

			_returnType = new Lazy<IType>(BindReturnType);
			_parameters = new Lazy<ImmutableArray<IParameter>>(BindParameters);
		}

		private IType BindReturnType()
		{
			return _context.BindReturnType(_sourceSymbolContext, _isExported, _diagnostics);
		}

		private ImmutableArray<IParameter> BindParameters()
		{
			return
				_context
				.parameters()
				.BindParameters(_sourceSymbolContext, _isExported, _diagnostics);
		}

		public string Name { get; }

		public IType ReturnType => _returnType.Value;

		public ImmutableArray<IParameter> Parameters => _parameters.Value;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _returnType.Value;
			_ = _parameters.Value;
		}
	}
}

