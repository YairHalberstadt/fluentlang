using Antlr4.Runtime;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.Parsing;
using FluentLang.Compiler.Symbols.Source;
using System;
using System.IO;

namespace FluentLang.Compiler.Symbols.Metadata
{
	internal abstract class MetadataSymbolBase : SymbolBase
	{
		public MetadataSymbolBase(DiagnosticBag diagnostics) : base(diagnostics)
		{
		}

		protected static T Parse<T>(string source, Func<FluentLangParser, T> getT) where T : ParserRuleContext
		{
			using var reader = new StringReader(source);

			var input = new AntlrInputStream(reader);
			var lexer = new FluentLangLexer(input);

			var tokenStream = new CommonTokenStream(lexer);
			var parser = new FluentLangParser(tokenStream);

			// pick up any syntax errors
			var diagnostics = new DiagnosticBag(null!);
			var errorStrategy = new ErrorStrategy(diagnostics);
			parser.ErrorHandler = errorStrategy;
			var t = getT(parser);
			diagnostics.AddRange(diagnostics);
			return t;
		}
	}
}