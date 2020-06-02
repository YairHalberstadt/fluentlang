using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;

namespace FluentLang.Compiler.Diagnostics
{
	public struct Location
	{
		public IToken? Token { get; }

		public IParseTree? ParseTree { get; }

		public Range TokenRange { get; }

		public Location(IToken token, Range tokenRange = default)
		{
			Token = token;
			TokenRange = tokenRange.Equals(default) ? .. : tokenRange;
			ParseTree = null;
		}

		public Location(IParseTree parseTree)
		{
			Token = null;
			TokenRange = default;
			ParseTree = parseTree;
		}

		public ReadOnlySpan<char> GetText()
		{
			if (ParseTree is { } parseTree)
				return parseTree.GetText().AsSpan();
			if (Token is { } token)
				return token.Text.AsSpan()[TokenRange];
			return "";
		}
	}
}
