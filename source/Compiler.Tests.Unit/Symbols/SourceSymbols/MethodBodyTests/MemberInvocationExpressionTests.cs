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
	public class MemberInvocationExpressionTests : TestBase
	{
		public MemberInvocationExpressionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void CanInvokeMemberWithNoArguments()
		{
			var assembly = CreateAssembly(@"M(param : { M() : int; }) : int { return param.M(); }")
				.VerifyDiagnostics().VerifyEmit();

			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var memberInvocationExpression = Assert.IsAssignableFrom<IMemberInvocationExpression>(returnStatement.Expression);
			Assert.Equal("M", memberInvocationExpression.MemberName);
			Assert.Equal("M", memberInvocationExpression.Method.Name);
			Assert.Empty(memberInvocationExpression.Method.Parameters);
			Assert.Empty(memberInvocationExpression.Arguments);
			Assert.Equal(Primitive.Int, memberInvocationExpression.Method.ReturnType);
			Assert.Equal(Primitive.Int, memberInvocationExpression.Type);
		}

		[Fact]
		public void CanInvokeMemberWithArguments()
		{
			var assembly = CreateAssembly(@"
M(param : { M(a : int, b : bool) : int; }) : int { 
	return param.M(5, false); 
}")
				.VerifyDiagnostics().VerifyEmit();

			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var memberInvocationExpression = Assert.IsAssignableFrom<IMemberInvocationExpression>(returnStatement.Expression);
			Assert.Equal("M", memberInvocationExpression.MemberName);
			Assert.Equal("M", memberInvocationExpression.Method.Name);
			Assert.Equal(2, memberInvocationExpression.Method.Parameters.Length);
			Assert.Equal(2, memberInvocationExpression.Arguments.Length);
			Assert.Equal(Primitive.Int, memberInvocationExpression.Method.ReturnType);
			Assert.Equal(Primitive.Int, memberInvocationExpression.Type);
		}

		[Fact]
		public void CanInvokeMemberWithArgumentsWhichAreASubtypeOfParameters()
		{
			var assembly = CreateAssembly(@"
M(param : { M(a : {}) : int; }) : int { 
	return param.M(param); 
}")
				.VerifyDiagnostics().VerifyEmit();

			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var memberInvocationExpression = Assert.IsAssignableFrom<IMemberInvocationExpression>(returnStatement.Expression);
			Assert.Equal("M", memberInvocationExpression.MemberName);
			Assert.Equal("M", memberInvocationExpression.Method.Name);
			Assert.True(memberInvocationExpression.Method.Parameters.Single().Type.IsEquivalentTo(EmptyInterface.Instance));
			Assert.Equal(m.Parameters.Single().Type, memberInvocationExpression.Arguments.Single().Type);
			Assert.Equal(Primitive.Int, memberInvocationExpression.Method.ReturnType);
			Assert.Equal(Primitive.Int, memberInvocationExpression.Type);
		}

		[Fact]
		public void CannotInvokeMemberWithArgumentsWhichAreNotASubtypeOfParameters()
		{
			CreateAssembly(@"
M(param : { M(a : { M() : bool; }) : int; }) : int { 
	return param.M(param); 
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"returnparam.M(param);")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"M")), ErrorCode.MethodNotFound));
		}

		[Fact]
		public void CanInvokeMember_WithArgumentsWhichAreASubtypeOfParameters_EvenWhenThereExistsMethods_WhichDoNotMatch()
		{
			var assembly = CreateAssembly(@"
M(param : { M(a : {}) : int; M(a : { M() : bool; }) : bool; }) : int { 
	return param.M(param); 
}")
				.VerifyDiagnostics().VerifyEmit();

			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var memberInvocationExpression = Assert.IsAssignableFrom<IMemberInvocationExpression>(returnStatement.Expression);
			Assert.Equal("M", memberInvocationExpression.MemberName);
			Assert.Equal("M", memberInvocationExpression.Method.Name);
			Assert.True(memberInvocationExpression.Method.Parameters.Single().Type.IsEquivalentTo(EmptyInterface.Instance));
			Assert.Equal(m.Parameters.Single().Type, memberInvocationExpression.Arguments.Single().Type);
			Assert.Equal(Primitive.Int, memberInvocationExpression.Method.ReturnType);
			Assert.Equal(Primitive.Int, memberInvocationExpression.Type);
		}

		[Fact]
		public void ErrorWhenMultipleMatching()
		{
			CreateAssembly(@"
M(param : { M(a : { M(a : {}) : bool; }) : int; M(a : {}) : bool; }) : int { 
	return param.M(param); 
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"returnparam.M(param);")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"M")), ErrorCode.AmbigiousMethodReference));
		}

		[Fact]
		public void NoErrorWhenMultipleMatchingHaveSameSignature()
		{
			CreateAssembly(@"
M(param : { M() : int; M() : int; }) : int { 
	return param.M(); 
}")
				.VerifyDiagnostics()
				.VerifyEmit();
		}
	}
}
