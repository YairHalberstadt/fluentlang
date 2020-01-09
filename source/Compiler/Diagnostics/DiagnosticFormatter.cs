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
					$"code: {errorCode.ToString()}, text: {diagnostic.Location.GetText().ToString()},  aditionalContext: {string.Join(", ", diagnostic.AdditionalContext)}",
			};
		}

		public string CreateLocationMessage(Diagnostic diagnostic)
		{
			//TODO: store file number.
			var location = diagnostic.Location;
			if (location.Token is
			{
				Line: var line,
				Column: var column,
				StartIndex: var startIndex,
				StopIndex: var stopIndex
			})
			{
				var tokenLength = stopIndex - startIndex;
				var (offset, length) = location.TokenRange.GetOffsetAndLength(tokenLength);
				return $"Line: {line}, Column: {column + offset}..{column + offset + length}";
			}
			else if (location.ParseTree is { } parseTree)
			{
				//TODO: find a way to implement this
				return parseTree.GetText();
			}
			return "";
		}
	}
}
