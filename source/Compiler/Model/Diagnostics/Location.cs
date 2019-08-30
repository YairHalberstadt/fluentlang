using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace FluentLang.Compiler.Model.Diagnostic
{
	public struct Location
	{
		public IToken? Token { get; }

		public IParseTree? ParseTree { get; }

		public Location(IToken token)
		{
			Token = token;
			ParseTree = null;
		}

		public Location(IParseTree parseTree)
		{
			Token = null;
			ParseTree = parseTree;
		}
	}
}
