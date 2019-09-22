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
    M1() : bool {}
	M1(param :  int) : int {}
}").VerifyDiagnostics(
					new Diagnostic(new Location(), ErrorCode.DuplicateMethodDeclaration),
					new Diagnostic(new Location(), ErrorCode.DuplicateMethodDeclaration));
		}

		[Fact]
		public void ReportErrorOnDuplicateLocalInterface()
		{
			CreateAssembly(@"
M() : bool {
    interface I {}
	interface I { M() : bool; }
}").VerifyDiagnostics(
					new Diagnostic(new Location(), ErrorCode.DuplicateInterfaceDeclaration),
					new Diagnostic(new Location(), ErrorCode.DuplicateInterfaceDeclaration));
		}
	}
}
