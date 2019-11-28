﻿using FluentLang.Compiler.Diagnostics;
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

		[Fact]
		public void ErrorOnMethodWithMultipleParametersSameName()
		{
			CreateAssembly(@"M(a : int, a : int) : int { return a; }")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"a:int")), ErrorCode.ParametersShareNames));
		}

		[Fact]
		public void ParameterCanNotHideLocalDeclaredInSameScope()
		{
			CreateAssembly(@"
M() : {} {
	Local(x : {}) : {} { return x; }
	let x = {};
	return x;
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.HidesLocal));
		}

		[Fact]
		public void ParameterCanNotHideParameter()
		{
			CreateAssembly(@"
M(x : {}) : {} {
	Local(x : {}) : {} { return x; }
	return x;
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.HidesLocal));
		}

		[Fact]
		public void ParameterCanNotHideLocalDeclaredInParentScope()
		{
			CreateAssembly(@"
M() : {} {
	Local() : {} { InnerLocal(x : {}) : {} { return x; } return {}; }
	let x = {};
	return x;
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.HidesLocal));
		}

		[Fact]
		public void ParameterCanNotHideParentMethodParameter()
		{
			CreateAssembly(@"
M(x : {}) : {} {
	Local() : {} { InnerLocal(x : {}) : {} { return x; } return {}; }
	return x;
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.HidesLocal));
		}
	}
}
