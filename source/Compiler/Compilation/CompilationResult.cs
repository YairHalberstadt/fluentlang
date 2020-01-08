using FluentLang.Compiler.Diagnostics;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Compilation
{
	public class CompilationResult
	{
		public CompilationResult(
			CompilationResultStatus status, ImmutableArray<Diagnostic> assemblyDiagnostics,
			ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> roslynDiagnostics)
		{
			Status = status;
			AssemblyDiagnostics = assemblyDiagnostics;
			RoslynDiagnostics = roslynDiagnostics;
		}

		public CompilationResultStatus Status { get; }

		public ImmutableArray<Diagnostic> AssemblyDiagnostics { get; }

		public ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> RoslynDiagnostics { get; }
	}
}
