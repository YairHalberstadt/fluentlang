using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Source.MethodBody;
using FluentLang.TestUtils;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class MatchExpressionTests : TestBase
	{
		public MatchExpressionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		public class ExhaustablityTests : TestBase
		{
			public ExhaustablityTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Fact]
			public void IsExhaustiveWhenAllOptionsAreMatched()
			{
				var assembly = CreateAssembly(@"
M(a : string | { M() : int; } | { M() : bool; }) : int { 
	return a match  {
		string => 1;
		x : { M() : int; } => 2;
		{ M() : bool; } => 3;
	};
}").VerifyDiagnostics().VerifyEmit();
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<IMatchExpression>(statement.Expression);
				Assert.Equal(Primitive.Int, exp.Type);
				Assert.Equal(3, exp.Arms.Length);
				Assert.Equal(Primitive.String, exp.Arms[0].Type);
			}

			[Fact]
			public void IsExhaustiveWhenAllOptionsAreMatchedBySuperTypes()
			{
				var assembly = CreateAssembly(@"
M(a : string | { M() : int; } | { M() : bool; }) : int { 
	return a match  {
		string => 1;
		x : { M() : int; } => 2;
		{} => 3;
	};
}").VerifyDiagnostics().VerifyEmit();
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<IMatchExpression>(statement.Expression);
				Assert.Equal(Primitive.Int, exp.Type);
				Assert.Equal(3, exp.Arms.Length);
				Assert.Equal(Primitive.String, exp.Arms[0].Type);
			}

			[Fact]
			public void SingleSupertypeCanMatchMultipleOptions()
			{
				var assembly = CreateAssembly(@"
M(a : string | { M() : int; } | { M() : bool; }) : int { 
	return a match  {
		string => 1;
		{} => 2;
	};
}").VerifyDiagnostics().VerifyEmit();
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<IMatchExpression>(statement.Expression);
				Assert.Equal(Primitive.Int, exp.Type);
				Assert.Equal(2, exp.Arms.Length);
				Assert.Equal(Primitive.String, exp.Arms[0].Type);
			}

			[Fact]
			public void ErrorWhenNotExhaustive()
			{
				CreateAssembly(@"
M(a : string | { M() : int; } | { M() : bool; }) : int { 
	return a match  {
		string => 1;
		x : { M() : int; } => 2;
	};
}").VerifyDiagnostics(new Diagnostic(new Location(new TextToken(@"amatch{string=>1;x:{M():int;}=>2;}")), ErrorCode.MatchNotExhaustive));
			}
		}

		public class BestTypeTests : TestBase
		{
			public BestTypeTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Fact]
			public void BestType1()
			{
				var assembly = CreateAssembly(@"
M(a : string | { M() : int; } | { M() : bool; }) : int { 
	return a match  {
		string => 1;
		x : { M() : int; } => 2;
		{ M() : bool; } => 3;
	};
}").VerifyDiagnostics().VerifyEmit();
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<IMatchExpression>(statement.Expression);
				Assert.Equal(Primitive.Int, exp.Type);
			}

			[Fact]
			public void BestType2()
			{
				var assembly = CreateAssembly(@"
M(a : string | { M() : int; } | { M() : bool; }, b : { M() : {}; }, c : { M() : int; }) : {} { 
	return a match  {
		string => {};
		x : { M() : int; } => b;
		{ M() : bool; } => c;
	};
}").VerifyDiagnostics().VerifyEmit();
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<IMatchExpression>(statement.Expression);
				Assert.Equal(EmptyInterface.Instance, exp.Type);
			}

			[Fact]
			public void ErrorWhenNoBestType()
			{
				CreateAssembly(@"
M(a : string | { M() : int; } | { M() : bool; }, b : { M() : {}; }, c : { M() : int; }) : {} { 
	return a match  {
		string => b;
		x : { M() : int; } => b;
		{ M() : bool; } => c;
	};
}").VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"returnamatch{string=>b;x:{M():int;}=>b;{M():bool;}=>c;};")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"amatch{string=>b;x:{M():int;}=>b;{M():bool;}=>c;}")), ErrorCode.NoBestType));
			}
		}

		public class LocalTests : TestBase
		{
			public LocalTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Fact]
			public void CanUseLocalInsideScope()
			{
				var assembly = CreateAssembly(@"
M(a : string | int) : int { 
	return a match  {
		string => 1;
		x : int => x;
	};
}").VerifyDiagnostics().VerifyEmit();
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<IMatchExpression>(statement.Expression);
				Assert.Equal(Primitive.Int, exp.Type);
			}

			[Fact]
			public void CannotUseLocalOutsideScope()
			{
				CreateAssembly(@"
M(a : string | int) : int { 
	return a match  {
		x : string => 1;
		int => x;
	};
}").VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"returnamatch{x:string=>1;int=>x;};")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"amatch{x:string=>1;int=>x;}")), ErrorCode.NoBestType),
					new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.InvalidLocalReference));
			}

			[Fact]
			public void LocalCannotHideAnotherLocal()
			{
				CreateAssembly(@"
M(a : string | int) : int { 
	return a match  {
		string => 1;
		a : int => a;
	};
}").VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"returnamatch{string=>1;a:int=>a;};")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.HidesLocal));
			}

			[Fact]
			public void LocalCanHaveSameNameAsLocaInOtherArm()
			{
				var assembly = CreateAssembly(@"
M(a : string | int) : int { 
	return a match  {
		x : string => 1;
		x : int => x;
	};
}").VerifyDiagnostics().VerifyEmit();

				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<IMatchExpression>(statement.Expression);
				Assert.Equal(Primitive.Int, exp.Type);
			}
		}

		[Fact]
		public void ErrorWhenExpressionNotUnion()
		{
			CreateAssembly(@"
M() : {} { 
	return 42 match  {
		int => {};
	};
}").VerifyDiagnostics(new Diagnostic(new Location(new TextToken(@"42")), ErrorCode.CannotMatchOnNonUnion));
		}
	}
}
