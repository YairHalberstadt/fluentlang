using Antlr4.Runtime;
using System.Linq;

namespace FluentLang.Compiler.Diagnostics
{
	public class DiagnosticFormatter : IDiagnosticFormatter
	{
		public string CreateDiagnosticMessage(Diagnostic diagnostic)
		{
			return diagnostic.ErrorCode switch
			{
				//TODO: explicitly handle more errors.
				ErrorCode.InvalidMetadataAssembly =>
					diagnostic.AdditionalContext.FirstOrDefault()?.ToString() ?? "",
				var errorCode =>
					$"code: {errorCode}, text: {diagnostic.Location.GetText().ToString()},  aditionalContext: {string.Join(", ", diagnostic.AdditionalContext)}",
			};
		}

		public string CreateLocationMessage(Diagnostic diagnostic)
		{
			//TODO: store file number.
			var location = diagnostic.Location.TextRange;
			return $"Start: [Ln {location.Start.Line}, Col {location.Start.Column}] End: [Ln {location.End.Line}, Col {location.End.Column}]";
		}
	}
}
