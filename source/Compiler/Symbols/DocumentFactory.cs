using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Source;

namespace FluentLang.Compiler.Symbols
{
	public static class DocumentFactory
	{
		public static IDocument FromString(string source)
		{
			return new SourceDocument(source);
		}
	}
}
