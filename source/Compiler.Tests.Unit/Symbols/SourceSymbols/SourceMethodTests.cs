using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols
{
	public class SourceMethodTests : TestBase
	{
		[Fact]
		public void ReportErrorOnDuplicateLocalMethod()
		{
			CreateAssembly(@"
M() : bool {
    M1() : bool { return true; }
	M1(param :  int) : int { return 42; }
	return true;
}").VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"M1")), ErrorCode.DuplicateMethodDeclaration),
					new Diagnostic(new Location(new TextToken(@"M1")), ErrorCode.DuplicateMethodDeclaration));
		}

		[Fact]
		public void ReportErrorOnDuplicateLocalInterface()
		{
			CreateAssembly(@"
M() : bool {
    interface I {}
	interface I { M() : bool; }
	return true;
}").VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"I")), ErrorCode.DuplicateInterfaceDeclaration),
					new Diagnostic(new Location(new TextToken(@"I")), ErrorCode.DuplicateInterfaceDeclaration));
		}
	}
}
