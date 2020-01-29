using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Source.MethodBody;
using FluentLang.TestUtils;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class DeclarationStatementTests : TestBase
	{
		public DeclarationStatementTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void CanUseDeclaredVariable()
		{
			var assembly = CreateAssembly(@"
M() : {} {
	let x = {};
	return x; 
}").VerifyDiagnostics().VerifyEmit();
			var m = AssertGetMethod(assembly, "M");
			var declarationStatement = Assert.IsAssignableFrom<IDeclarationStatement>(m.Statements.First());
			var local = declarationStatement.Local;
			Assert.NotNull(local);
			Assert.Equal(declarationStatement.Type, local!.Type);
			Assert.True(local.Type.IsEquivalentTo(EmptyInterface.Instance));
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Last());
			var exp = Assert.IsAssignableFrom<ILocalReferenceExpression>(returnStatement.Expression);
			Assert.Equal(local, exp.Local);
		}

		[Fact]
		public void CanExplicitlySetTypeOfDeclaredVariable()
		{
			var assembly = CreateAssembly(@"
M() : {} {
	let x : {} = {};
	return x; 
}").VerifyDiagnostics().VerifyEmit();
			var m = AssertGetMethod(assembly, "M");
			var declarationStatement = Assert.IsAssignableFrom<IDeclarationStatement>(m.Statements.First());
			var local = declarationStatement.Local;
			Assert.NotNull(local);
			Assert.Equal(declarationStatement.Type, local!.Type);
			Assert.True(local.Type.IsEquivalentTo(EmptyInterface.Instance));
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Last());
			var exp = Assert.IsAssignableFrom<ILocalReferenceExpression>(returnStatement.Expression);
			Assert.Equal(local, exp.Local);
		}

		[Fact]
		public void CanSetTypeOfDeclaredVariableToSuperType()
		{
			var assembly = CreateAssembly(@"
M() : {} {
	let x : {} = {} + M1;
	return x; 
}
M1(param : {}) : {} { return param; }").VerifyDiagnostics().VerifyEmit();
			var m = AssertGetMethod(assembly, "M");
			var declarationStatement = Assert.IsAssignableFrom<IDeclarationStatement>(m.Statements.First());
			var local = declarationStatement.Local;
			Assert.NotNull(local);
			Assert.Equal(declarationStatement.Type, local!.Type);
			Assert.True(local.Type.IsEquivalentTo(EmptyInterface.Instance));
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Last());
			var exp = Assert.IsAssignableFrom<ILocalReferenceExpression>(returnStatement.Expression);
			Assert.Equal(local, exp.Local);
		}

		[Fact]
		public void CanNotSetTypeOfDeclaredVariableToNonSuperType()
		{
			CreateAssembly(@"
M() : {} {
	let x : { M2() : {}; } = {} + M1;
	return x; 
}
M1(param : {}) : {} { return param; }").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"{}+M1")), ErrorCode.MismatchedTypes));
		}

		[Fact]
		public void CanDiscardResultOfExpression()
		{
			var assembly = CreateAssembly(@"
M() : {} {
	_ = {};
	return {};
}").VerifyDiagnostics().VerifyEmit();
			var m = AssertGetMethod(assembly, "M");
			var declarationStatement = Assert.IsAssignableFrom<IDeclarationStatement>(m.Statements.First());
			Assert.Null(declarationStatement.Local);
			Assert.Equal(declarationStatement.Expression.Type, declarationStatement.Type);
		}

		[Fact]
		public void CanNotExplicitlySpecifyTypeOfDiscardExpression()
		{
			CreateAssembly(@"
M() : {} {
	_ : {} = {};
	return {};
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@":")), ErrorCode.SyntaxError));
		}

		[Fact]
		public void CanNotReferenceDiscardAsLocal()
		{
			CreateAssembly(@"
M() : {} {
	_ = {};
	return _;
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"_")), ErrorCode.SyntaxError));
		}

		[Fact]
		public void CanNotHideLocalDeclaredInSameScope()
		{
			CreateAssembly(@"
M() : {} {
	let x = {};
	let x = {};
	return x;
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.HidesLocal));
		}

		[Fact]
		public void CanNotHideParameter()
		{
			CreateAssembly(@"
M(x : {}) : {} {
	let x = {};
	return x;
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.HidesLocal));
		}

		[Fact]
		public void CanNotHideLocalDeclaredInParentScope()
		{
			CreateAssembly(@"
M() : {} {
	Local() : {} { let x = {}; return x; }
	let x = {};
	return x;
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.HidesLocal));
		}

		[Fact]
		public void CanNotHideParentMethodParameter()
		{
			CreateAssembly(@"
M(x : {}) : {} {
	Local() : {} { let x = {}; return x; }
	return x;
}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.HidesLocal));
		}
	}
}
