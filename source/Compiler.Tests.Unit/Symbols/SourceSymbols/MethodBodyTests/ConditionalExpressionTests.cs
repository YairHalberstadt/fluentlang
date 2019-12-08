using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Source.MethodBody;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class ConditionalExpressionTests : TestBase
	{
		public ConditionalExpressionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void CanCompileConditional()
		{
			var assembly = CreateAssembly(@"M() : int { return if (true) 0 else 1; }")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var m = AssertGetMethod(assembly, "M");
			var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var exp = Assert.IsAssignableFrom<IConditionalExpression>(statement.Expression);
			Assert.Equal(Primitive.Int, exp.Type);
		}

		[Fact]
		public void ErrorIfConditionIsNotBoolean()
		{
			CreateAssembly(@"M() : int { return if (4) 0 else 1; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"4")), ErrorCode.NonBooleanCondition));
		}

		[Fact]
		public void ErrorIfOneBranchOfConditionalIsNotSubtypeOfOtherBranch()
		{
			CreateAssembly(@"M() : int { return if (true) 0 else 1.0; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"returnif(true)0else1.0;")), ErrorCode.ReturnTypeDoesNotMatch),
						new Diagnostic(new Location(new TextToken(@"true")), ErrorCode.NoBestType));
		}

		[Fact]
		public void TrueBranchCanBeSubtypeOfFalseBranch()
		{
			var assembly = CreateAssembly(@"
M() : {} { return if (true) {} + M1 else {}; }
M1(param : {}) : bool { return true; }")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var m = AssertGetMethod(assembly, "M");
			var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var exp = Assert.IsAssignableFrom<IConditionalExpression>(statement.Expression);
			Assert.Equal(EmptyInterface.Instance, exp.Type);
		}

		[Fact]
		public void FalseBranchCanBeSubtypeOfTrueBranch()
		{
			var assembly = CreateAssembly(@"
M() : {} { return if (true) {} else {} + M1; }
M1(param : {}) : bool { return true; }")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var m = AssertGetMethod(assembly, "M");
			var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var exp = Assert.IsAssignableFrom<IConditionalExpression>(statement.Expression);
			Assert.Equal(EmptyInterface.Instance, exp.Type);
		}
	}
}
