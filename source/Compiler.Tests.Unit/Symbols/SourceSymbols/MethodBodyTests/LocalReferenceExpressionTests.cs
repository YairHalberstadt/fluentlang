using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.TestUtils;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class LocalReferenceExpressionTests : TestBase
	{
		public LocalReferenceExpressionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void CanReferenceDeclaredLocal()
		{
			var assembly = CreateAssembly(@"
M() : {} {
	let x = {};
	return x; 
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
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
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Last());
			var exp = Assert.IsAssignableFrom<ILocalReferenceExpression>(returnStatement.Expression);
			Assert.Equal(m.ParameterLocals.Single(), exp.Local);
			Assert.Equal(m.Parameters.Single().Name, exp.Local.Identifier);
		}

		[Fact]
		public void ErrorWhenReferencingNonExistantLocal()
		{
			CreateAssembly(@"M() : {} { return x; }")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"returnx;")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.InvalidLocalReference));
		}

		[Fact]
		public void CanReferenceParentMethodParameterLocal()
		{
			var assembly = CreateAssembly(@"
M(x : {}) : {} {
	LocalM() : {} {
		return x;
	}
	return {}; 
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var m = AssertGetMethod(assembly, "M");
			var localM = m.LocalMethods.Single();
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(localM.Statements.Last());
			var exp = Assert.IsAssignableFrom<ILocalReferenceExpression>(returnStatement.Expression);
			Assert.Equal(m.ParameterLocals.Single(), exp.Local);
			Assert.Equal(m.Parameters.Single().Name, exp.Local.Identifier);
		}

		[Fact]
		public void CanReferenceParentParentMethodParameterLocal()
		{
			var assembly = CreateAssembly(@"
M(x : {}) : {} {
	M1() : {} {
		LocalM() : {} {
			return x;
		}
		return {};
	}
	return {}; 
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var m = AssertGetMethod(assembly, "M");
			var localM = m.LocalMethods.Single().LocalMethods.Single();
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(localM.Statements.Last());
			var exp = Assert.IsAssignableFrom<ILocalReferenceExpression>(returnStatement.Expression);
			Assert.Equal(m.ParameterLocals.Single(), exp.Local);
			Assert.Equal(m.Parameters.Single().Name, exp.Local.Identifier);
		}

		[Fact]
		public void ErrorWhenReferencingParameterOfChildMethod()
		{
			CreateAssembly(@"
M() : {} { 
	Local(x : {}) : {} { return {}; }
	return x; 
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"returnx;")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.InvalidLocalReference));
		}

		[Fact]
		public void ErrorWhenReferencingParameterOfParentsChildMethod()
		{
			CreateAssembly(@"
M() : {} { 
	Local1(x : {}) : {} { return {}; }
	Local2() : {} { return x; }
	return {}; 
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"returnx;")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.InvalidLocalReference));
		}

		[Fact]
		public void CanReferenceParentMethodDeclaredLocal()
		{
			var assembly = CreateAssembly(@"
M() : {} {
	let x = {};
	LocalM() : {} {
		return x;
	}
	return {}; 
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var m = AssertGetMethod(assembly, "M");
			var localM = m.LocalMethods.Single();
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(localM.Statements.Last());
			var exp = Assert.IsAssignableFrom<ILocalReferenceExpression>(returnStatement.Expression);
			var declarationStatement = Assert.IsAssignableFrom<IDeclarationStatement>(m.Statements.First());
			var local = declarationStatement.Local;
			Assert.Equal(local, exp.Local);
		}

		[Fact]
		public void CanReferenceParentParentMethodDeclaredLocal()
		{
			var assembly = CreateAssembly(@"
M() : {} {
	let x = {};
	M1() : {} {
		LocalM() : {} {
			return x;
		}
		return {};
	}
	return {}; 
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var m = AssertGetMethod(assembly, "M");
			var localM = m.LocalMethods.Single().LocalMethods.Single();
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(localM.Statements.Last());
			var exp = Assert.IsAssignableFrom<ILocalReferenceExpression>(returnStatement.Expression);
			var declarationStatement = Assert.IsAssignableFrom<IDeclarationStatement>(m.Statements.First());
			var local = declarationStatement.Local;
			Assert.Equal(local, exp.Local);
		}

		[Fact]
		public void ErrorWhenReferencingDeclaredLocalOfChildMethod()
		{
			CreateAssembly(@"
M() : {} { 
	Local() : {} { let x = {}; return {}; }
	return x; 
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"returnx;")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.InvalidLocalReference));
		}

		[Fact]
		public void ErrorWhenReferencingDeclaredLocalOfParentsChildMethod()
		{
			CreateAssembly(@"
M() : {} { 
	Local1() : {} { let x = {}; return {}; }
	Local2() : {} { return x; }
	return {}; 
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"returnx;")), ErrorCode.ReturnTypeDoesNotMatch),
					new Diagnostic(new Location(new TextToken(@"x")), ErrorCode.InvalidLocalReference));
		}

		[Fact]
		public void ErrorWhenLocalMethodUsedBeforeStatementWhereCapturedLocalDeclared()
		{
			CreateAssembly(@"
M() : {} { 
	Local() : {} { return x; }
	_ = Local();
	let x = {};
	return x; 
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"Local()")), ErrorCode.UseOfMethodWhichCapturesUnassignedLocals));
		}

		[Fact]
		public void ErrorWhenLocalMethodUsedBeforeLatestStatementWhereCapturedLocalDeclared()
		{
			CreateAssembly(@"
M() : {} { 
	Local() : int { return x + y; }
	let x = 5;
	_ = Local();
	let y = 10;
	return {}; 
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"Local()")), ErrorCode.UseOfMethodWhichCapturesUnassignedLocals));
		}

		[Fact]
		public void ErrorWhenLocalMethodUsedInStatementWhereCapturedLocalDeclared()
		{
			CreateAssembly(@"
M() : {} { 
	Local() : {} { return x; }
	let x = Local();
	return x; 
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"Local()")), ErrorCode.UseOfMethodWhichCapturesUnassignedLocals));
		}

		[Fact]
		public void NoErrorWhenLocalMethodUsedAfterStatementWhereCapturedLocalDeclared()
		{
			CreateAssembly(@"
M() : {} { 
	Local() : {} { return x; }
	let x = {};
	return Local(); 
}")
				.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
		}

		[Fact]
		public void ErrorWhenLocalMethodUsedInStatementWhereIndirectlyCapturedLocalDeclared1()
		{
			CreateAssembly(@"
M() : {} { 
	Local() : {} { InnerLocal() : {} { return x; } return InnerLocal(); }
	let x = Local();
	return x; 
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"Local()")), ErrorCode.UseOfMethodWhichCapturesUnassignedLocals));
		}

		[Fact]
		public void ErrorWhenLocalMethodUsedInStatementWhereIndirectlyCapturedLocalDeclared2()
		{
			CreateAssembly(@"
M() : {} {
	Local1() : {} { return x; }
	Local2() : {} { InnerLocal() : {} { return Local1(); } return InnerLocal(); }
	let x = Local2();
	return x; 
}")
				.VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"Local2()")), ErrorCode.UseOfMethodWhichCapturesUnassignedLocals));
		}


		[Fact]
		public void NoErrorWhenLocalMethodUsedInStatementWhereNonCapturedLocalDeclared1()
		{
			CreateAssembly(@"
M() : {} { 
	Local() : {} { InnerLocal() : {} { return x; } return {}; }
	let x = Local();
	return x; 
}")
				.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
		}

		[Fact]
		public void NoErrorWhenLocalMethodUsedInStatementWhereNonCapturedLocalDeclared2()
		{
			CreateAssembly(@"
M() : {} { 
	Local() : {} { let x = Local(); return x; }
	return {}; 
}")
				.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
		}
	}
}
