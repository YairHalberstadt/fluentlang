using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal class MethodBodySymbolContext
	{
		public MethodBodySymbolContext(SourceSymbolContext sourceSymbolContext)
		{
			SourceSymbolContext = sourceSymbolContext;
			Locals = ImmutableList<ILocal>.Empty;
		}

		private MethodBodySymbolContext(SourceSymbolContext sourceSymbolContext, ImmutableList<ILocal> locals, IStatement? currentStatement)
		{
			SourceSymbolContext = sourceSymbolContext;
			Locals = locals;
			CurrentStatement = currentStatement;
		}

		public SourceSymbolContext SourceSymbolContext { get; }
		public ImmutableList<ILocal> Locals { get; }
		public IStatement? CurrentStatement { get; }

		public MethodBodySymbolContext WithLocal(ILocal local)
			=> new MethodBodySymbolContext(SourceSymbolContext, Locals.Add(local), CurrentStatement);
		public MethodBodySymbolContext WithScope(IMethod method)
			=> new MethodBodySymbolContext(SourceSymbolContext.WithScope(method), Locals.AddRange(method.ParameterLocals), CurrentStatement);
		public MethodBodySymbolContext WithStatement(IStatement statement)
			=> new MethodBodySymbolContext(SourceSymbolContext, Locals, statement);

	}
}
