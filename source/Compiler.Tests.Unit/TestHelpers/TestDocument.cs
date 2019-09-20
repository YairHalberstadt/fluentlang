using Antlr4.Runtime;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.Parsing;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public class TestDocument : IDocument
	{
		private readonly string _source;

		public TestDocument(string source)
		{
			_source = source;
		}

		public string FullName => "test.cs";

		public FluentLangParser.Compilation_unitContext GetSyntaxTree(DiagnosticBag diagnostics)
		{
			using var reader = new StringReader(_source);

			var input = new AntlrInputStream(reader);
			var lexer = new FluentLangLexer(input);

			var tokenStream = new CommonTokenStream(lexer);
			var parser = new FluentLangParser(tokenStream);

			// pick up any syntax errors
			var errorStrategy = new ErrorStrategy(diagnostics);
			parser.ErrorHandler = errorStrategy;
			return parser.compilation_unit();
		}
	}
}
