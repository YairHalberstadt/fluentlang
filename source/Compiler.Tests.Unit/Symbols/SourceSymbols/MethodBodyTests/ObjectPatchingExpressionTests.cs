﻿using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System.Linq;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class ObjectPatchingExpressionTests : TestBase
	{
		[Fact]
		public void CanPatchMethod()
		{
			var assembly = CreateAssembly(@"
Patch(p : {}) : {} { return {}; }
M() : {} {
	return {} + Patch;
}").VerifyDiagnostics();
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var patch = Assert.IsAssignableFrom<IObjectPatchingExpression>(returnStatement.Expression);
			Assert.Equal(AssertGetMethod(assembly, "Patch"), patch.Patches.Single().Method);
		}

		[Fact]
		public void PatchedObjectHasCorrectInterface()
		{
			var assembly = CreateAssembly(@"
Patch(p : {}) : {} { return {}; }
M() : Patched {
	return {} + Patch;
}

interface Patched {
    Patch() : {};
}").VerifyDiagnostics();
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var patchedInterface = AssertGetInterface(assembly, "Patched");
			Assert.True(returnStatement.Expression.Type.IsEquivalentTo(patchedInterface));
		}

		[Fact]
		public void CanPatchTwoMethodsAtOnce()
		{
			var assembly = CreateAssembly(@"
Patch1(p : {}) : {} { return {}; }
Patch2(p : {}) : {} { return {}; }

M() : Patched {
	return {} + Patch1, Patch2;
}

interface Patched {
    Patch1() : {};
    Patch2() : {};
}").VerifyDiagnostics();

			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var patch = Assert.IsAssignableFrom<IObjectPatchingExpression>(returnStatement.Expression);
			var patchedInterface = AssertGetInterface(assembly, "Patched");
			Assert.Equal(2, patch.Patches.Length);
			Assert.True(patch.Type.IsEquivalentTo(patchedInterface));
		}

		[Fact]
		public void ErrorIfPatchedInMethodHasIncorrectFirstParameter()
		{
			var assembly = CreateAssembly(@"
Patch(p : I) : {} { return {}; }
M() : {} {
	return {} + Patch;
}

interface I {
    M() : {};
}").VerifyDiagnostics(new Diagnostic(new Location(), ErrorCode.ResultantTypeOfObjectPatchingExpressionIsNotSubtypeOfFirstParameterOfPatchedInMethod));
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var patch = Assert.IsAssignableFrom<IObjectPatchingExpression>(returnStatement.Expression);
			Assert.Equal(AssertGetMethod(assembly, "Patch"), patch.Patches.Single().Method);
		}

		[Fact]
		public void PatchedInMethodsFirstParameterCanBeASubtypeOfTheResultantExpression1()
		{
			var assembly = CreateAssembly(@"
Patch(p : Patched) : {} { return {}; }
M() : Patched {
	return {} + Patch;
}

interface Patched {
    Patch() : {};
}").VerifyDiagnostics();
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var patchedInterface = AssertGetInterface(assembly, "Patched");
			Assert.True(returnStatement.Expression.Type.IsEquivalentTo(patchedInterface));
		}

		[Fact]
		public void PatchedInMethodsFirstParameterCanBeASubtypeOfTheResultantExpression2()
		{
			var assembly = CreateAssembly(@"
Patch1(p : Patched) : {} { return {}; }
Patch2(p : Patched) : {} { return {}; }

M() : Patched {
	return {} + Patch1, Patch2;
}

interface Patched {
    Patch1() : {};
    Patch2() : {};
}").VerifyDiagnostics();
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var patchedInterface = AssertGetInterface(assembly, "Patched");
			Assert.True(returnStatement.Expression.Type.IsEquivalentTo(patchedInterface));
		}

		[Fact]
		public void CanMixinObject()
		{
			var assembly = CreateAssembly(@"
Patch(p : {}) : {} { return {}; }

M() : Patched {
	let temp = {} + Patch;
	return {} + mixin temp;
}

interface Patched {
    Patch() : {};
}").VerifyDiagnostics();
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Last());
			var patch = Assert.IsAssignableFrom<IObjectPatchingExpression>(returnStatement.Expression);
			var local = Assert.IsAssignableFrom<ILocalReferenceExpression>(patch.Patches.Single().MixedInExpression);
			Assert.True(local.Type.IsEquivalentTo(patch.Type));
			Assert.True(patch.Type.IsEquivalentTo(m.ReturnType));
		}

		[Fact]
		public void CanMixinMultipleObjects()
		{
			var assembly = CreateAssembly(@"
Patch1(p : {}) : {} { return {}; }
Patch2(p : {}) : {} { return {}; }

M() : Patched {
	let temp1 = {} + Patch1;
	let temp2 = {} + Patch2;
	return {} + mixin temp1, mixin temp2;
}

interface Patched {
    Patch1() : {};
    Patch2() : {};
}").VerifyDiagnostics();
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Last());
			var patch = Assert.IsAssignableFrom<IObjectPatchingExpression>(returnStatement.Expression);
			var local1 = Assert.IsAssignableFrom<ILocalReferenceExpression>(patch.Patches.First().MixedInExpression);
			Assert.Equal("temp1", local1.Local.Identifier);
			var local2 = Assert.IsAssignableFrom<ILocalReferenceExpression>(patch.Patches.Last().MixedInExpression);
			Assert.Equal("temp2", local2.Local.Identifier);
			Assert.True(patch.Type.IsEquivalentTo(m.ReturnType));
		}

		[Fact]
		public void CanPatchMethodAndMixinObjectInSameExpression()
		{
			var assembly = CreateAssembly(@"
Patch1(p : {}) : {} { return {}; }
Patch2(p : Patched) : {} { return {}; }

M() : Patched {
	let temp = {} + Patch1;
	return {} + mixin temp, Patch2;
}

interface Patched {
    Patch1() : {};
    Patch2() : {};
}").VerifyDiagnostics();
			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Last());
			var patch = Assert.IsAssignableFrom<IObjectPatchingExpression>(returnStatement.Expression);
			var local1 = Assert.IsAssignableFrom<ILocalReferenceExpression>(patch.Patches.First().MixedInExpression);
			Assert.Equal("temp", local1.Local.Identifier);
			Assert.Equal(AssertGetMethod(assembly, "Patch2"), patch.Patches.Last().Method);
			Assert.True(patch.Type.IsEquivalentTo(m.ReturnType));
		}

		[Fact]
		public void CanPatchTwoMethodsWithSameSignatureAtOnce()
		{
			var assembly = CreateAssembly(@"
Patch(p : {}) : {} { return {}; }
namespace Inner {
	Patch(p : Patched) : {} { return {}; }
}

M() : Patched {
	return {} + Patch, Inner.Patch;
}

interface Patched {
    Patch() : {};
}").VerifyDiagnostics();

			var m = AssertGetMethod(assembly, "M");
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var patch = Assert.IsAssignableFrom<IObjectPatchingExpression>(returnStatement.Expression);
			var patchedInterface = AssertGetInterface(assembly, "Patched");
			Assert.Equal(2, patch.Patches.Length);
			Assert.True(patch.Type.IsEquivalentTo(patchedInterface));
		}
	}
}