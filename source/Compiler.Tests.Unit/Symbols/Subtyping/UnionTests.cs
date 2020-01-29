using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.TestUtils;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.Subtyping
{
	public class UnionTests : TestBase
	{
		public UnionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void UnionIsSubtypeOfUnionWithSameTypesInSameOrder()
		{
			var assembly = CreateAssembly(@"
M1() : int | {} { return 42; }
M2() : int | {} { return {}; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.True(u1.IsSubtypeOf(u2));
			Assert.True(u2.IsSubtypeOf(u1));
		}

		[Fact]
		public void UnionIsSubtypeOfUnionWithSameTypesInDifferentOrder()
		{
			var assembly = CreateAssembly(@"
M1() : int | {} { return 42; }
M2() : {} | int { return {}; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.True(u1.IsSubtypeOf(u2));
			Assert.True(u2.IsSubtypeOf(u1));
		}

		[Fact]
		public void UnionIsSubtypeOfUnionWithSameTypesDuplicated()
		{
			var assembly = CreateAssembly(@"
M1() : int | {} { return 42; }
M2() : {} | int | {} { return {}; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.True(u1.IsSubtypeOf(u2));
			Assert.True(u2.IsSubtypeOf(u1));
		}

		[Fact]
		public void UnionIsSubtypeOfUnionWithMoreTypesAndNotViceVersa()
		{
			var assembly = CreateAssembly(@"
M1() : int | {} { return 42; }
M2() : {} | int | string { return {}; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.True(u1.IsSubtypeOf(u2));
			Assert.False(u2.IsSubtypeOf(u1));
		}

		[Fact]
		public void UnionIsNotSubtypeOfUnionWithDifferentTypes()
		{
			var assembly = CreateAssembly(@"
M1() : int | {} { return 42; }
M2() : {} | string { return {}; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.False(u1.IsSubtypeOf(u2));
			Assert.False(u2.IsSubtypeOf(u1));
		}

		[Fact]
		public void UnionIsSubtypeOfInterfaceIfAllOptionsAreSubtypeOfInterface()
		{
			var assembly = CreateAssembly(@"
M1(a: { M() : int; }) : { M() : int; } | { M(): int; M1(): string } { return a; }
M2(a: { M() : int; }) : { M() : int; } { return a; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IInterface>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.True(u1.IsSubtypeOf(u2));
			Assert.True(u2.IsSubtypeOf(u1));
		}

		[Fact]
		public void InterfaceIsSubtypeOfUnionIfIsSubtypeOfAnyOption()
		{
			var assembly = CreateAssembly(@"
M1(a: { M() : int; }) : { M() : int; } | int { return a; }
M2(a: { M() : int; }) : { M() : int; } { return a; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IInterface>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.False(u1.IsSubtypeOf(u2));
			Assert.True(u2.IsSubtypeOf(u1));
		}

		[Fact]
		public void UnionIsSubtypeOfUnionIfAllOptionsAreSubtypeOfAnyOption()
		{
			var assembly = CreateAssembly(@"
M1() : int | { M(): int; M1(): string } { return 42; }
M2() : { M() : int; } | int { return 42; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.True(u1.IsSubtypeOf(u2));
			Assert.False(u2.IsSubtypeOf(u1));
		}

		[Fact]
		public void UnionIsNotSubtypeOfUnionIfAnyOptionsAreNotSubtypeOfAnyOption()
		{
			var assembly = CreateAssembly(@"
M1() : int | { M(): int; M1(): string } { return 42; }
M2() : { M() : int; } | string { return """"; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.False(u1.IsSubtypeOf(u2));
			Assert.False(u2.IsSubtypeOf(u1));
		}

		[Fact]
		public void UnionIsSubtypeOfPrimitiveIfAllOptionsAreSubtypeOfInterface()
		{
			var assembly = CreateAssembly(@"
M1() : int | int { return 42; }
M2() : int { return 42; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<Primitive>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.True(u1.IsSubtypeOf(u2));
			Assert.True(u2.IsSubtypeOf(u1));
		}

		[Fact]
		public void PrimtiveIsSubtypeOfUnionIfIsSubtypeOfAnyOption()
		{
			var assembly = CreateAssembly(@"
M1() : { M() : int; } | int { return 42; }
M2() : int { return 42; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<Primitive>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.False(u1.IsSubtypeOf(u2));
			Assert.True(u2.IsSubtypeOf(u1));
		}
	}
}
