using Antlr4.Runtime;
using FluentLang.Compiler.Diagnostics;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Parsing
{
	public class ErrorStrategy : DefaultErrorStrategy
	{
		private readonly DiagnosticBag _diagnostics;

		public ErrorStrategy(DiagnosticBag diagnostics)
		{
			_diagnostics = diagnostics;
		}

		public override void ReportError(Parser recognizer, RecognitionException e)
		{
			base.ReportError(recognizer, e);
			_diagnostics.Add(new Diagnostic(new Location(e.OffendingToken), ErrorCode.SyntaxError, ImmutableArray.Create<object?>(e.Message)));
		}
	}
}