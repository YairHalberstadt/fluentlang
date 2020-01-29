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

		public MemberInvocationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}
	}
}
