using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	public class ParsedDocument
	{
		public ParsedDocument(string fullName, Compilation_unitContext syntaxTree)
		{
			FullName = fullName;
			SyntaxTree = syntaxTree;
		}

		public string FullName { get; }
		public Compilation_unitContext SyntaxTree { get; }
	}
}

