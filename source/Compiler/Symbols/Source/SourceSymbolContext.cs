using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Source
{
	internal class SourceSymbolContext
	{
		public SourceSymbolContext(IMethod? scope, IAssembly assembly, ImmutableArray<QualifiedName> imports, QualifiedName? nameSpace)
		{
			Scope = scope;
			Assembly = assembly;
			Imports = imports.IsDefault ? throw Release.Fail("imports cannot be default") : imports;
			NameSpace = nameSpace;
		}

		public IMethod? Scope { get; }
		public IAssembly Assembly { get; }
		public ImmutableArray<QualifiedName> Imports { get; }
		public QualifiedName? NameSpace { get; }


		public SourceSymbolContext WithScope(IMethod? scope)
		{
			return new SourceSymbolContext(scope, Assembly, Imports, NameSpace);
		}
	}
}

