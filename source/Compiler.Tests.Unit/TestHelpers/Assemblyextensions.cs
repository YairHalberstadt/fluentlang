using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Linq;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public static class Assemblyextensions
	{
		public static IAssembly VerifyDiagnostics(this IAssembly assembly, params Diagnostic[] expectedDiagnostics)
		{
			var actualDiagnostics = assembly.AllDiagnostics;

			Assert.True(expectedDiagnostics.Length == actualDiagnostics.Length, ErrorMessage());

			Assert.True(
				expectedDiagnostics.Zip(actualDiagnostics, (x, y) => x.ErrorCode == y.ErrorCode).All(x => x),
				ErrorMessage());

			return assembly;

			string ErrorMessage()
			{
				return
					$@"
Expected: 
{string.Join("\n", expectedDiagnostics.Select(DiagnosticToString))}

Actual:
{string.Join("\n", actualDiagnostics.Select(DiagnosticToString))}
";
				static string DiagnosticToString(Diagnostic diagnostic)
					=> $"new Diagnostic(new Location(), {nameof(ErrorCode)}.{diagnostic.ErrorCode})";
			}
		}
	}
}
