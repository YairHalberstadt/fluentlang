using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System.Linq;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class ReturnStatementTests : TestBase
	{
		[Fact]
		public void CanReturnSameTypeAsMethod()
		{
			var assembly = CreateAssembly("M() : {} { return {}; }")
				.VerifyDiagnostics();
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			Assert.True(returnStatement.Expression.Type.IsEquivalentTo(m.ReturnType));
		}

		[Fact]
		public void CanReturnSubTypeOfMethod()
		{
			var assembly = CreateAssembly("M(param : { M() : bool; }) : {} { return param; }")
				.VerifyDiagnostics();
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			Assert.True(returnStatement.Expression.Type.IsSubtypeOf(m.ReturnType));
		}

		[Fact]
		public void CantReturnNonSubTypeOfMethod()
		{
			CreateAssembly("M() : {} { return 5; }")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"return5;")), ErrorCode.ReturnTypeDoesNotMatch));
		}

		[Fact]
		public void ReturnStatementMustBeLastStatement()
		{
			CreateAssembly("M() : int { return 5; _ = 5; }")
				.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"return5;")), ErrorCode.OnlyLastStatementCanBeReturnStatement),
						new Diagnostic(new Location(new TextToken(@"_=5;")), ErrorCode.LastStatementMustBeReturnStatement));
		}

		[Fact]
		public void MustHaveReturnStatement()
		{
			CreateAssembly("M() : int { _ = 5; }")
				.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"_=5;")), ErrorCode.LastStatementMustBeReturnStatement));
		}

		[Fact]
		public void CannotHaveMultipleReturnStatements()
		{
			CreateAssembly("M() : int { return 5; return 5; }")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"return5;")), ErrorCode.OnlyLastStatementCanBeReturnStatement));
		}
	}
}
