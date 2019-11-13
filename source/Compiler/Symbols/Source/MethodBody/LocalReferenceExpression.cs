using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.ErrorSymbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{

	internal sealed class LocalReferenceExpression : SymbolBase, ILocalReferenceExpression
	{
		private readonly Local_reference_expressionContext _context;
		private readonly MethodBodySymbolContext _methodBodySymbolContext;
		private readonly Lazy<ILocal> _local;

		public LocalReferenceExpression(
			Local_reference_expressionContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_methodBodySymbolContext = methodBodySymbolContext;

			Identifier = context.LOWERCASE_IDENTIFIER().Symbol.Text;
			_local = new Lazy<ILocal>(BindLocal);
		}

		private ILocal BindLocal()
		{
			var local = _methodBodySymbolContext.Locals.FirstOrDefault(x => x.Identifier == Identifier);
			if (local != null)
				return local;

			_diagnostics.Add(new Diagnostic(
				new Location(_context.LOWERCASE_IDENTIFIER()),
				ErrorCode.InvalidLocalReference,
				ImmutableArray.Create<object?>(Identifier)));

			return new ErrorLocal(Identifier);
		}

		public string Identifier { get; }
		public ILocal Local => _local.Value;
		public IType Type => Local.Type;

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			_ = _local.Value;
		}

		class ErrorLocal : ILocal, IErrorSymbol
		{
			public ErrorLocal(string identifier)
			{
				Identifier = identifier;
			}

			public string Identifier { get; }

			public IType Type => ErrorType.Instance;

			public ImmutableArray<Diagnostic> AllDiagnostics => ImmutableArray<Diagnostic>.Empty;

			void ISymbol.EnsureAllLocalDiagnosticsCollected() { }
		}
	}
}
