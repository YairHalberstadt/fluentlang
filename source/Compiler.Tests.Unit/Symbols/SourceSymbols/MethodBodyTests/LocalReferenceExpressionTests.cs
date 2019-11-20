using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System.Linq;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class LocalReferenceExpressionTests : TestBase
	{
		[Fact]
		public void CanReferenceDeclaredLocal()
		{
			var assembly = CreateAssembly(@"
M() : {} {
	let x = {};
	return x; 
}").VerifyDiagnostics();
			var m = AssertGetMethod(assembly, "M");
			var declarationStatement = Assert.IsAssignableFrom<IDeclarationStatement>(m.Statements.First());
			var local = declarationStatement.Local;
			Assert.NotNull(local);
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Last());
			var exp = Assert.IsAssignableFrom<ILocalReferenceExpression>(returnStatement.Expression);
			Assert.Equal(local, exp.Local);
			Assert.Equal(local!.Type, exp.Type);
		}

		[Fact]
		public void CanReferenceParameterLocal()
		{
			var assembly = CreateAssembly(@"
M(x : {}) : {} {
	return x; 
}").VerifyDiagnostics();
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Last());
			var exp = Assert.IsAssignableFrom<ILocalReferenceExpression>(returnStatement.Expression);
			Assert.Equal(m.Parameters.Single().Type, exp.Local.Type);
			Assert.Equal(m.Parameters.Single().Name, exp.Local.Identifier);
		}

		[Fact]
		public void ErrorWhenReferencingNonexistantLocal()
		{
			CreateAssembly(@"M() : {} { return x; }")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"returnx;")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.InvalidLocalReference));
		}
	}
}
