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
		private readonly Interface_member_declarationContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;

		private readonly Lazy<IType> _returnType;
		private readonly Lazy<ImmutableArray<IParameter>> _parameters;

		public SourceInterfaceMethod(
			Interface_member_declarationContext context,
			SourceSymbolContext sourceSymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			Name = context.method_signature().UPPERCASE_IDENTIFIER().Symbol.Text;

			_returnType = new Lazy<IType>(BindReturnType);
			_parameters = new Lazy<ImmutableArray<IParameter>>(BindParameters);
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

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _returnType.Value;
			_ = _parameters.Value;
		}
	}
}

