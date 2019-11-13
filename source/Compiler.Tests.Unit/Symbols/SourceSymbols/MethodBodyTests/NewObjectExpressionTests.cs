using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Source.MethodBody;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class NewObjectExpressionTests : TestBase
	{
		[Fact]
		public void CanParseNewObjectExpression()
		{
			var assembly = CreateAssembly(@"
M() : {} {
    return {};
}").VerifyDiagnostics();
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			Assert.IsAssignableFrom<INewObjectExpression>(returnStatement.Expression);
		}
	}
}
