using FluentLang.TestUtils;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Emit
{
	public class MemberInvocationTests : TestBase
	{
		[Fact]
		public void CanInvokePatchedInMethod()
		{
			CreateAssembly(@"
Main() : int { return ({} + M).M(); }
M(a : {}) : int { return 42; }")
				.VerifyDiagnostics()
				.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void CallingPatchedInMethodActsOnResultOfPatchingExpression()
		{
			CreateAssembly(@"
Main() : int { return ({} + M1, M2).M1(); }
M1(a : { M2() : int; }) : int { return a.M2(); }
M2(a : {}) : int { return 42; }")
				.VerifyDiagnostics()
				.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void LaterPatchedInMethodReplacesEarlier()
		{
			CreateAssembly(@"
Main() : int { return ({} + M, N.M).M(); }
M(a : {}) : int { return 41; }
namespace N { M(a : {}) : int { return 42; } }")
				.VerifyDiagnostics()
				.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void MixedInMethodCallsMethodOnMixedInObject()
		{
			CreateAssembly(@"
Main() : int { return ({} + M, mixin ({} + N.M, M1), M).M1(); }
M(a : {}) : int { return 41; }
M1(a : { M() : int; }) : int { return a.M(); }
namespace N { M(a : {}) : int { return 42; } }")
				.VerifyDiagnostics()
				.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void LaterPatchOverridesMixedInMethod()
		{
			CreateAssembly(@"
Main() : int { return ({} + mixin ({} + M), N.M).M(); }
M(a : {}) : int { return 41; }
namespace N { M(a : {}) : int { return 42; } }")
				.VerifyDiagnostics()
				.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void RequiredMethodKeys1()
		{
			CreateAssembly(@"
Main() : int { return M<int>(42).GetT(); }
M<T>(t : T) : { GetT() : T; } {
	return {} + GetT;
	GetT(a : {}) : T { return t; }
}")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void RequiredMethodKeys2()
		{
			CreateAssembly(@"
Main() : int { return M1<int>(42); }
M1<T>(t: T) : T {
	_ = M2<T>(t).GetT();
	_ = M2<int>(42).GetT();
	return M2<T>(t).GetT();
}
M2<T>(t : T) : { GetT() : T; } {
	return {} + GetT;
	GetT(a : {}) : T { return t; }
}")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void RequiredMethodKeys3()
		{
			CreateAssembly(@"
Main() : int { return M<int>(42).GetT(); }
M<T>(t : T) : { GetT() : T; } {
	return {} + GetT;
	GetT(a : {}) : T {
		return GetTInternal<T>(t, t);
		GetTInternal<T1>(b : T, c : T1) : T1 { return c; }
	}
}")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void RequiredMethodKeys4()
		{
			CreateAssembly(@"
Main() : int { 
	
	return M<int>({} + M);
	M(a : {}) : int {
		return 42;
	}
}
M<T>(a : { M() : T; }) : T {
	return a.M();
}")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		public MemberInvocationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}
	}
}
