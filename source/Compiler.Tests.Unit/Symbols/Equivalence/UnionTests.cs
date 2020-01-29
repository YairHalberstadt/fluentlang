using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.TestUtils;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.Equivalence
{
	public class UnionTests : TestBase
	{
		public UnionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void UnionIsEquivalentToUnionWithSameTypesInSameOrder()
		{
			var assembly = CreateAssembly(@"
M1() : int | {} { return 42; }
M2() : int | {} { return {}; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.True(u1.IsEquivalentTo(u2));
			Assert.True(u2.IsEquivalentTo(u1));
		}

		[Fact]
		public void UnionIsEquivalentToUnionWithSameTypesInDifferentOrder()
		{
			var assembly = CreateAssembly(@"
M1() : int | {} { return 42; }
M2() : {} | int { return {}; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.True(u1.IsEquivalentTo(u2));
			Assert.True(u2.IsEquivalentTo(u1));
		}

		[Fact]
		public void UnionIsEquivalentToUnionWithSameTypesDuplicated()
		{
			var assembly = CreateAssembly(@"
M1() : int | {} { return 42; }
M2() : {} | int | {} { return {}; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.True(u1.IsEquivalentTo(u2));
			Assert.True(u2.IsEquivalentTo(u1));
		}

		[Fact]
		public void UnionIsNotEquivalentToUnionWithMoreTypes()
		{
			var assembly = CreateAssembly(@"
M1() : int | {} { return 42; }
M2() : {} | int | string { return {}; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.False(u1.IsEquivalentTo(u2));
			Assert.False(u2.IsEquivalentTo(u1));
		}

		[Fact]
		public void UnionIsNotEquivalentToUnionWithDifferentTypes()
		{
			var assembly = CreateAssembly(@"
M1() : int | {} { return 42; }
M2() : {} | string { return {}; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.False(u1.IsEquivalentTo(u2));
			Assert.False(u2.IsEquivalentTo(u1));
		}

		[Fact]
		public void UnionIsNotEquivalentToInterface()
		{
			var assembly = CreateAssembly(@"
M1() : int | {} { return 42; }
M2() : {} { return {}; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M1").ReturnType);
			var u2 = Assert.IsAssignableFrom<IInterface>(AssertGetMethod(assembly, "M2").ReturnType);
			Assert.False(u1.IsEquivalentTo(u2));
			Assert.False(u2.IsEquivalentTo(u1));
		}

		[Fact]
		public void UnionIsNotEquivalentToPrimitive()
		{
			var assembly = CreateAssembly(@"M() : int | {} { return 42; }").VerifyDiagnostics();

			var u1 = Assert.IsAssignableFrom<IUnion>(AssertGetMethod(assembly, "M").ReturnType);
			Assert.False(u1.IsEquivalentTo(Primitive.Int));
			Assert.False(((IType)Primitive.Int).IsEquivalentTo(u1));
		}
	}
}
