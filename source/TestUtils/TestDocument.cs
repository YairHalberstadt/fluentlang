using Antlr4.Runtime;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.Parsing;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Immutable;
using System.IO;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.TestUtils
{
	public class TestDocument : IDocument
	{
		private readonly string _source;
		private readonly Lazy<Compilation_unitContext> _syntaxTree;
		private ImmutableArray<Diagnostic> _diagnostics;

		public TestDocument(string source)
		{
			_source = source;
			_syntaxTree = new Lazy<Compilation_unitContext>(GetSyntaxTree);
		}

		public string FullName => "test.cs";

		public Compilation_unitContext SyntaxTree => _syntaxTree.Value;

		public ImmutableArray<Diagnostic> Diagnostics
		{
			get
			{
				_ = _syntaxTree.Value;
				return _diagnostics;
			}
		}

		public Compilation_unitContext GetSyntaxTree()
		{
			using var reader = new StringReader(_source);

			var input = new AntlrInputStream(reader);
			var lexer = new FluentLangLexer(input);

			var tokenStream = new CommonTokenStream(lexer);
			var parser = new FluentLangParser(tokenStream);

			// pick up any syntax errors
			var diagnostics = new DiagnosticBag(null!);
			var errorStrategy = new ErrorStrategy(diagnostics);
			parser.ErrorHandler = errorStrategy;
			var compilationUnit = parser.compilation_unit();
			_diagnostics = diagnostics.ToImmutableArray();
			return compilationUnit;
		}
	}
}
