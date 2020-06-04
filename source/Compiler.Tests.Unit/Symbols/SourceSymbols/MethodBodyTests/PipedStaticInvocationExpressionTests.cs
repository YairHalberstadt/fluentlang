using FluentLang.Compiler.Diagnostics;
using FluentLang.TestUtils;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class PipedStaticInvocationExpressionTests : TestBase
	{
		public PipedStaticInvocationExpressionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void CanBindPipedStaticInvocationExpression()
		{
			CreateAssembly(@"
Main() : int {
	return 5..M(37);
}
M(a : int, b : int) : int { 
	return a + b;
}")
				.VerifyDiagnostics().VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void ErrorIfMethodNotFound()
		{
			CreateAssembly(@"
Main() : int {
	return 5..M(37);
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"return5..M(37);")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"M")), ErrorCode.MethodNotFound));
		}

		[Fact]
		public void ErrorIfQualifiedNameUsed()
		{
			CreateAssembly(@"
Main() : int {
	return 5..N.M(37);
}
namespace N {
	M(a : int, b : int) : int { 
		return a + b;
	}
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"N.M")), ErrorCode.PipedStaticInvocationExpressionCannotHaveQualifiedName));
		}
	}
}
