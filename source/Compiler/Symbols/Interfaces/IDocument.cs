using FluentLang.Compiler.Diagnostics;
using System.Collections.Immutable;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IDocument
	{
		public string FullName { get; }
		public Compilation_unitContext SyntaxTree { get; }
		public ImmutableArray<Diagnostic> Diagnostics { get; }
	}
}