using Antlr4.Runtime;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.Parsing;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Immutable;
using System.IO;
using static FluentLang.Compiler.Generated.FluentLangParser;
using DiagnosticErrorListener = FluentLang.Compiler.Parsing.DiagnosticErrorListener;

namespace FluentLang.Compiler.Symbols.Source
{
	internal sealed class SourceDocument : IDocument
	{
		private readonly string _source;
		private readonly Lazy<Compilation_unitContext> _syntaxTree;
		private ImmutableArray<Diagnostic> _diagnostics;

		public SourceDocument(string source)
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
			var diagnostics = new DiagnosticBag(null!);
			var parser = ParserFactory.Create(_source, diagnostics);

			var compilationUnit = parser.compilation_unit();
			_diagnostics = diagnostics.ToImmutableArray();
			return compilationUnit;
		}
	}
}
