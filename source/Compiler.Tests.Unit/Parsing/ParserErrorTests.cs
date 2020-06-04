using FluentLang.Compiler.Diagnostics;
using FluentLang.TestUtils;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Parsing
{
	public class ParserErrorTests : TestBase
	{
		public ParserErrorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void ReportInvalidToken()
		{
			CreateAssembly("#!~@`")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"#")), ErrorCode.SyntaxError));
		}
	}
}
