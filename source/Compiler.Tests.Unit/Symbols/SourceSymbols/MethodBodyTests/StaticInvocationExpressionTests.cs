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
	public class StaticInvocationExpressionTests : TestBase
	{
		public StaticInvocationExpressionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void CanInvokeMethodWithNoArguments()
		{
			var assembly = CreateAssembly(@"M() : int { return M(); }")
				.VerifyDiagnostics().VerifyEmit(_testOutputHelper);

			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var staticInvocationExpression = Assert.IsAssignableFrom<IStaticInvocationExpression>(returnStatement.Expression);
			Assert.Equal("M", staticInvocationExpression.MethodName.ToString());
			Assert.Equal(m, staticInvocationExpression.Method);
			Assert.Equal(Primitive.Int, staticInvocationExpression.Type);
		}

		[Fact]
		public void CanInvokeMethodWithArguments()
		{
			var assembly = CreateAssembly(@"
M(a : int, b : bool) : int { 
	return M(5, false); 
}")
				.VerifyDiagnostics().VerifyEmit(_testOutputHelper);

			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var staticInvocationExpression = Assert.IsAssignableFrom<IStaticInvocationExpression>(returnStatement.Expression);
			Assert.Equal("M", staticInvocationExpression.MethodName.ToString());
			Assert.Equal(m, staticInvocationExpression.Method);
			Assert.Equal(2, staticInvocationExpression.Arguments.Length);
			Assert.Equal(Primitive.Int, staticInvocationExpression.Type);
		}

		[Fact]
		public void CanInvokeMethodWithArgumentsWhichAreASubtypeOfParameters()
		{
			var assembly = CreateAssembly(@"
M(a : { M(a : {}) : int; }, b : {}) : int { 
	return M(a, a); 
}")
				.VerifyDiagnostics().VerifyEmit(_testOutputHelper);

			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var staticInvocationExpression = Assert.IsAssignableFrom<IStaticInvocationExpression>(returnStatement.Expression);
			Assert.Equal("M", staticInvocationExpression.MethodName.ToString());
			Assert.Equal(m, staticInvocationExpression.Method);
			Assert.Equal(Primitive.Int, staticInvocationExpression.Type);
		}

		[Fact]
		public void CannotInvokeMethodWithArgumentsWhichAreNotASubtypeOfParameters()
		{
			CreateAssembly(@"
M(param : int) : int { 
	return M({}); 
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"returnM({});")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"M")), ErrorCode.MethodNotFound));
		}
	}
}
