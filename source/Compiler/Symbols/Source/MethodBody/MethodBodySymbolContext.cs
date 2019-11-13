using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal class MethodBodySymbolContext
	{
		public MethodBodySymbolContext(SourceSymbolContext sourceSymbolContext, ImmutableList<ILocal> locals)
		{
			SourceSymbolContext = sourceSymbolContext;
			Locals = locals;
		}

		public SourceSymbolContext SourceSymbolContext { get; }
		public ImmutableList<ILocal> Locals { get; }

		public MethodBodySymbolContext WithLocal(ILocal local)
			=> new MethodBodySymbolContext(SourceSymbolContext, Locals.Add(local));
	}
}
