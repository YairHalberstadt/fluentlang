using FluentLang.Compiler.Diagnostics;
using FluentLang.TestUtils;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols
{
	public class SourceMethodTests : TestBase
	{
		public SourceMethodTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

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

		[Fact]
		public void MethodSymbolHasIsExportTrueOnlyIfItHasExportModifier()
		{
			var assembly = CreateAssembly(@"
export M1() : bool { return true; }
M2() : bool { return true; }").VerifyDiagnostics().VerifyEmit();
			var m1 = AssertGetMethod(assembly, "M1");
			Assert.True(m1.IsExported);
			var m2 = AssertGetMethod(assembly, "M2");
			Assert.False(m2.IsExported);
		}

		[Fact]
		public void ExportedMethodCanNotReferenceNonExportedInterface()
		{
			CreateAssembly(@"
interface I {}
export M(a : I) : I { return {}; }").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"I")), ErrorCode.CannotUseUnexportedInterfaceFromExportedMember),
				new Diagnostic(new Location(new TextToken(@"I")), ErrorCode.CannotUseUnexportedInterfaceFromExportedMember));
		}

		[Fact]
		public void ExportedMethodCanReferenceExportedInterface()
		{
			CreateAssembly(@"
export interface I {}
export M(a : I) : I { return {}; }").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void NonExportedMethodCanReferenceExportedInterface()
		{
			CreateAssembly(@"
export interface I {}
M(a : I) : I { return {}; }").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void NonExportedMethodCanReferenceNonExportedInterface()
		{
			CreateAssembly(@"
interface I {}
M(a : I) : I { return {}; }").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void CanUseTypeParametersInMethodBodyAndSignature()
		{
			var assembly = CreateAssembly(@"
M<T>(a : { M(): T; }) : T {
	let result: T = a.M(); 
	return result;
}").VerifyDiagnostics().VerifyEmit();

			var m = AssertGetMethod(assembly, "M");
			var tp = m.TypeParameters.SingleOrDefault();
			Assert.Equal(tp, m.ReturnType);
		}

		[Fact]
		public void TypeParametersAreVisibleInLocalMethodsAndInterfaces()
		{
			CreateAssembly(@"
M<T>() : int {
	interface I { M() : T; }
    M(a : I): T {
		let result: T = a.M(); 
		return result;
	}
	return 42;
}").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void LocalMethodsAndInterfacesCanDeclareTheirOwnTypeParameters()
		{
			CreateAssembly(@"
M<T>() : int {
	interface I<T1> { M(a : T1) : T; }
    M<T1>(a : { M(a : T1) : T; }, b: T1): T {
		let result: T = a.M(b); 
		return result;
	}
	return 42;
}").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void LocalInterfaceCannotHideTypeParameters()
		{
			CreateAssembly(@"
M<T>() : int {
	interface I<T> { }
	return 42;
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"T")), ErrorCode.TypeParametersShareNames));
		}

		[Fact]
		public void LocalMethodCannotHideTypeParameters()
		{
			CreateAssembly(@"
M<T>() : int {
	M<T>() : int { return 42; }
	return 42;
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"T")), ErrorCode.TypeParametersShareNames));
		}

		[Fact]
		public void TypeParameterCanSubtypeTypeParameters()
		{
			CreateAssembly(@"
M<T>() : int {
	M<T1 : T>(a : T1) : T { return a; }
	return 42;
}").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void TypeParameterCanSubtypeUnion()
		{
			CreateAssembly(@"
M<T : int | string>(a : T) : int | string {
	return a;
}").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void TypeParameterCanSubtypeInterface()
		{
			CreateAssembly(@"
M<T : {}>(a : T) : {} {
	return a;
}").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void TypeParameterCannotBeConstrainedToPrimitive()
		{
			CreateAssembly(@"
M<T : int>(a : T) : int {
	return a;
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@":int")), ErrorCode.CannotConstrainToPrimitive));
		}

		[Fact]
		public void CanHaveMultipleTypeParametersWithConstraints()
		{
			CreateAssembly(@"
M<T1 : {}, T2 : int | string>(a : T1) : {} {
	return a;
}").VerifyDiagnostics().VerifyEmit();
		}
	}
}
