using FluentLang.TestUtils;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Emit
{
	public class UnionAndMatchExpressionTests : TestBase
	{
		public UnionAndMatchExpressionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void MatchesCorrectly1()
		{
			CreateAssembly(@"
Main() : int {
	let u : int | {} = 42;
	return u match { x : int => x; {} => 41; };
}")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void MatchesCorrectly2()
		{
			CreateAssembly(@"
Main() : int {
	let u : int | {} = 42;
	return u match { {} => 41;  x : int => x; };
}")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void MatchesCorrectly3()
		{
			CreateAssembly(@"
Main() : int {
	let u : {} | int = 42;
	return u match { x : int => x; {} => 41; };
}")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void MatchesCorrectly4()
		{
			CreateAssembly(@"
Main() : int {
	let u : int | {} = 42;
	return u match { int => 42; {} => 41; };
}")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void MatchesCorrectly5()
		{
			CreateAssembly(@"
Main() : int {
	let u1 : int | {} = 42;
	let u2 : int | {} | string = u1;
	return u2 match { int => 42; {} => 41; string => 40; };
}")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void MatchesCorrectly6()
		{
			CreateAssembly(@"
Main() : int {
	
	let u : { M1(): int; } | { M2(): int; } = {} + M1, M2;
	return u match { {} => 42; };
}

M1(a : {}) : int { return 42; }
M2(a : {}) : int { return 42; }")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void MatchesCorrectly7()
		{
			CreateAssembly(@"
Main() : int {
	
	let u : { M1(): int; } | { M2(): int; } = {} + M1, M2;
	return u match { { M1(): int; } => 42; { M2(): int; } => 41; };
}

M1(a : {}) : int { return 42; }
M2(a : {}) : int { return 42; }")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void MatchesCorrectly8()
		{
			CreateAssembly(@"
Main() : int {
	
	let u : { M1(): int; } | { M2(): int; } = {} + M1, M2;
	return u match { { M2(): int; } => 42; { M1(): int; } => 41; };
}

M1(a : {}) : int { return 42; }
M2(a : {}) : int { return 42; }")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void MatchesCorrectly9()
		{
			CreateAssembly(@"
Main() : int {
	
	let u1 : { M1(): int; } | { M2(): int; } = {} + M1, M2;
	let u2 : { M1(): int; } | { M2(): int; } | int = u1;
	return u2 match { { M2(): int; } => 42; { M1(): int; } => 41; int => 40; };
}

M1(a : {}) : int { return 42; }
M2(a : {}) : int { return 42; }")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void MatchesCorrectly10()
		{
			CreateAssembly(@"
Main() : int {
	
	let u : { M1(): int; } | { M2(): int; } = {} + M1, M2;
	return u match { x : { M2(): int; } => x.M2(); x : { M1(): int; } => x.M1(); };
}

M1(a : {}) : int { return 42; }
M2(a : {}) : int { return 42; }")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}
	}
}
