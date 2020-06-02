using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Shared;
using System;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Source
{
	internal class SourceSymbolContext
	{
		public SourceSymbolContext(
			IMethod? scope,
			IAssembly assembly,
			ImmutableArray<QualifiedName> imports,
			QualifiedName? nameSpace,
			Func<ImmutableArray<ITypeParameter>> currentLevelTypeParameters)
		{
			Scope = scope;
			Assembly = assembly;
			Imports = imports.IsDefault ? throw Release.Fail("imports cannot be default") : imports;
			NameSpace = nameSpace;
			CurrentLevelTypeParameters = currentLevelTypeParameters;
		}

		public IMethod? Scope { get; }
		public Func<ImmutableArray<ITypeParameter>> CurrentLevelTypeParameters { get; }
		public IAssembly Assembly { get; }
		public ImmutableArray<QualifiedName> Imports { get; }
		public QualifiedName? NameSpace { get; }
		public SourceSymbolContext WithScope(IMethod? scope)
		{
			return new SourceSymbolContext(scope, Assembly, Imports, NameSpace, () => ImmutableArray<ITypeParameter>.Empty);
		}
		public SourceSymbolContext WithTypeParameters(Func<ImmutableArray<ITypeParameter>> currentLevelTypeParameters)
		{
			return new SourceSymbolContext(Scope, Assembly, Imports, NameSpace, currentLevelTypeParameters);
		}
	}
}

