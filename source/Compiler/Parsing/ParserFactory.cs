using Antlr4.Runtime;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Generated;
using System.IO;

namespace FluentLang.Compiler.Parsing
{
	public static class ParserFactory
	{
		public static FluentLangParser Create(string source, DiagnosticBag diagnostics)
		{
			using var reader = new StringReader(source);

			var input = new AntlrInputStream(reader);
			var lexer = new FluentLangLexer(input);
			var tokenStream = new CommonTokenStream(lexer);
			var parser = new FluentLangParser(tokenStream);

			var errorListener = new DiagnosticErrorListener(diagnostics);
			parser.RemoveErrorListeners();
			parser.AddErrorListener(errorListener);

			return parser;
		}
	}
}