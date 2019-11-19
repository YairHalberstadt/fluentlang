using Antlr4.Runtime;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
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
				expectedDiagnostics
					.Zip(
						actualDiagnostics,
						(x, y) =>
							x.ErrorCode == y.ErrorCode
							&& x.Location.GetText().Equals(
								y.Location.GetText(),
								StringComparison.Ordinal))
					.All(x => x),
				ErrorMessage());

			return assembly;

			string ErrorMessage()
			{
				return
					$@"
Expected: 
{string.Join(",\n", expectedDiagnostics.Select(DiagnosticToString))}

Actual:
{string.Join(",\n", actualDiagnostics.Select(DiagnosticToString))}
";
				static string DiagnosticToString(Diagnostic diagnostic)
					=> $"new Diagnostic(new Location(new TextToken(@\"{diagnostic.Location.GetText().ToString()}\")), {nameof(ErrorCode)}.{diagnostic.ErrorCode})";
			}
		}
	}

	internal class TextToken : IToken
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
