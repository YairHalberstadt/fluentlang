namespace FluentLang.Compiler.Diagnostics
{
	public interface IDiagnosticFormatter
	{
		string CreateDiagnosticMessage(Diagnostic diagnostic);
		string CreateLocationMessage(Diagnostic diagnostic);
	}
}