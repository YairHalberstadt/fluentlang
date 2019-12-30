using Antlr4.Runtime;

namespace FluentLang.TestUtils
{
	public class TextToken : IToken
	{
		public TextToken(string text)
		{
			Text = text;
		}

		public string Text { get; }

		public int Type => throw new System.NotImplementedException();

		public int Line => throw new System.NotImplementedException();

		public int Column => throw new System.NotImplementedException();

		public int Channel => throw new System.NotImplementedException();

		public int TokenIndex => throw new System.NotImplementedException();

		public int StartIndex => throw new System.NotImplementedException();

		public int StopIndex => throw new System.NotImplementedException();

		public ITokenSource TokenSource => throw new System.NotImplementedException();

		public ICharStream InputStream => throw new System.NotImplementedException();
	}
}
