using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;

namespace FluentLang.Compiler.Diagnostics
{
	public struct Location
	{
		private readonly IToken? _token;

		private readonly ParserRuleContext? _parseTree;

		private readonly Range _tokenRange;

		public Location(IToken token, Range tokenRange = default)
		{
			_token = token;
			_tokenRange = tokenRange.Equals(default) ? .. : tokenRange;
			_parseTree = null;
		}

		public Location(ParserRuleContext parseTree)
		{
			_token = null;
			_tokenRange = default;
			_parseTree = parseTree;
		}

		public Location(ITerminalNode terminalNode) : this(terminalNode.Symbol)
		{
		}

		public ReadOnlySpan<char> GetText()
		{
			if (_parseTree is { } parseTree)
				return parseTree.GetText().AsSpan();
			if (_token is { } token)
				return token.Text.AsSpan()[_tokenRange];
			return "";
		}

		public TextRange TextRange {
			get
			{
				if (_parseTree is { } parseTree)
				{
					var startLine = parseTree.Start.Line;
					var startColumn = parseTree.Start.Column;
					var stopLine = parseTree.Stop.Line;
					var stopColumn = parseTree.Stop.Column + parseTree.Stop.Text.Length;

					return new TextRange(new TextPoint(startLine, startColumn), new TextPoint(stopLine, stopColumn));
				}
				if (_token is { } token)
				{
					var line = token.Line;
					var column = token.Column;
					var tokenLength = token.Text.Length;
					var (offset, length) = _tokenRange.GetOffsetAndLength(tokenLength);
					return new TextRange(new TextPoint(line, column + offset), new TextPoint(line, column + offset + length));
				}
				return default;
			}
		}

	}

	public struct TextRange
	{
		public TextRange(TextPoint start, TextPoint end)
		{
			Start = start;
			End = end;
		}

		public TextPoint Start { get; }
		public TextPoint End { get; }
	}

	public struct TextPoint
	{
		public TextPoint(int line, int column)
		{
			Line = line;
			Column = column;
		}

		public int Line { get; }
		public int Column { get; }
	}
}
