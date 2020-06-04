using Antlr4.Runtime;
using FluentLang.Compiler.Diagnostics;
using System.Collections.Immutable;
using System.IO;

namespace FluentLang.Compiler.Parsing
{
	public class DiagnosticErrorListener : BaseErrorListener
	{
		private readonly DiagnosticBag _diagnostics;

		public DiagnosticErrorListener(DiagnosticBag diagnostics)
		{
			_diagnostics = diagnostics;
		}

		public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
		{
			_diagnostics.Add(new Diagnostic(new Location(offendingSymbol), ErrorCode.SyntaxError, ImmutableArray.Create<object?>(msg)));
		}
	}
}