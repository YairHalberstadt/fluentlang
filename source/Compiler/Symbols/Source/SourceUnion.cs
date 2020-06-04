using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal sealed class SourceUnion : SymbolBase, IUnion
	{
		private readonly UnionContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;
		private readonly bool _isExported;
		private readonly Lazy<ImmutableArray<IType>> _options;

		public SourceUnion(
			UnionContext context,
			SourceSymbolContext sourceSymbolContext,
			bool isExported,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			_isExported = isExported;
			_options = new Lazy<ImmutableArray<IType>>(BindOptions);
		}

		public ImmutableArray<IType> Options => _options.Value;

		private ImmutableArray<IType> BindOptions()
		{
			var options =
				_context
				.union_part_type()
				.Select(x => x.BindUnionPartType(_sourceSymbolContext, _isExported, _diagnostics))
				.ToImmutableArray();

			if (options.Length > 64)
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context),
					ErrorCode.TooManyOptionsInUnion,
					ImmutableArray.Create<object?>(this)));
			}

			return options;
		}

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _options.Value;
		}
	}
}

