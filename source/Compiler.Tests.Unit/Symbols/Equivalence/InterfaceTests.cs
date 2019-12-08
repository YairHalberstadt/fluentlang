using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Model.Equivalence
{
	public class InterfaceTests : TestBase
	{
		public static Primitive[][] Primitives =
		{
			new [] { Primitive.Bool },
			new [] { Primitive.Int },
			new [] { Primitive.Double },
			new [] { Primitive.Char },
			new [] { Primitive.String },
		};

		public InterfaceTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void InterfaceIsEquivalentTo_IdenticalInterfaceWithDifferentNames()
		{
			var assembly = CreateAssembly(@"
interface I1 {}
interface I2 {}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsEquivalentTo(i2!));
			Assert.True(i2.IsEquivalentTo(i1));
		}

		[Fact]
		public void InterfaceIsEquivalent_ToInterfaceWithSameMethods_EvenIfFromAdditive_AndEvenWithDuplicateMethods()
		{
			var assembly = CreateAssembly(@"
interface Empty {}
interface OneMethod1 { M() : int; }
interface OneMethod2 { M() : int; }
interface AdditiveOneMethod1 { M() : int; } + Empty
interface AdditiveOneMethod2 OneMethod1 + Empty
interface AdditiveOneMethod3 OneMethod1 + OneMethod2
").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("OneMethod1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("OneMethod2"), out var i2));
			Assert.True(assembly.TryGetInterface(QualifiedName("AdditiveOneMethod1"), out var i3));
			Assert.True(assembly.TryGetInterface(QualifiedName("AdditiveOneMethod2"), out var i4));
			Assert.True(assembly.TryGetInterface(QualifiedName("AdditiveOneMethod3"), out var i5));

			var interfaces = new IInterface[] { i1!, i2!, i3!, i4!, i5! };

			foreach (var a in interfaces)
			{
				foreach (var b in interfaces)
				{
					Assert.True(a.IsEquivalentTo(b));
				}
			}
		}

		[Fact]
		public void EmptyInterfaceIsNotEquivalentToInterfaceWithOneMethod()
		{
			var assembly = CreateAssembly(@"
interface Empty {}
interface OneMethod { M() : int; }").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("Empty"), out var empty));
			Assert.True(assembly.TryGetInterface(QualifiedName("OneMethod"), out var oneMethod));
			Debug.Assert(empty != null);
			Debug.Assert(oneMethod != null);
			Assert.False(empty.IsEquivalentTo(oneMethod));
			Assert.False(oneMethod.IsEquivalentTo(empty));
		}

		[Fact]
		public void InterfaceWithOneMethodIsNotEquivalentToInterfaceWithOneDifferentMethod()
		{
			var assembly = CreateAssembly(@"
interface OneMethod1 { M() : int; }
interface OneMethod2 { M() : bool; }
interface OneMethod3 { M1() : int; }
interface OneMethod4 { M(param1 : int) : int; }").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("OneMethod1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("OneMethod1"), out var i2));
			Assert.True(assembly.TryGetInterface(QualifiedName("OneMethod1"), out var i3));
			Assert.True(assembly.TryGetInterface(QualifiedName("OneMethod1"), out var i4));

			var interfaces = new IInterface[] { i1!, i2!, i3!, i4! };

			foreach (var a in interfaces)
			{
				foreach (var b in interfaces)
				{
					if (ReferenceEquals(a, b))
					{
						Assert.True(a.IsEquivalentTo(b));
					}
					else
					{
						Assert.False(a.IsEquivalentTo(b));
					}
				}
			}
		}

		[Fact]
		public void InterfacesWithDifferentNumbersOfMethods_AreNotEquivalent()
		{
			var assembly = CreateAssembly(@"
interface OneMethod { M() : int; }
interface TwoMethods { M() : int; M1() : int; }").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("OneMethod"), out var oneMethod));
			Assert.True(assembly.TryGetInterface(QualifiedName("TwoMethods"), out var twoMethods));
			Debug.Assert(twoMethods != null);
			Debug.Assert(oneMethod != null);
			Assert.False(oneMethod.IsEquivalentTo(twoMethods));
			Assert.False(twoMethods.IsEquivalentTo(oneMethod));
		}

		[Fact]
		public void ParameterNamesDoNotEffectEquality()
		{
			var assembly = CreateAssembly(@"
interface I1 { M(param1 : int) : bool; }
interface I2 { M(parameter1 : int) : bool; }").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsEquivalentTo(i2!));
			Assert.True(i2.IsEquivalentTo(i1));
		}

		[Fact]
		public void InterfaceWithOneMethodIsEquivalentRecursivelyA()
		{
			var assembly = CreateAssembly(@"
interface Empty {}
interface AdditiveEmpty Empty + {}
interface I1 { M(param1 : AdditiveEmpty()) : Empty; }
interface I2 { M(param1 : Empty()) : AdditiveEmpty; }").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsEquivalentTo(i2!));
			Assert.True(i2.IsEquivalentTo(i1));
		}

		[Fact]
		public void InterfaceWithOneMethodIsEquivalentRecursivelyB()
		{
			var assembly = CreateAssembly(@"
interface I1 { M(param1 : I1) : I1; }
interface I2 { M(param1 : I2) : I2; }").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsEquivalentTo(i2!));
			Assert.True(i2.IsEquivalentTo(i1));
		}

		[Fact]
		public void InterfaceWithOneMethodIsEquivalentRecursivelyC()
		{
			var assembly = CreateAssembly(@"
interface I1 { M(param1 : I2) : I2; }
interface I2 { M(param1 : I1) : I1; }").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsEquivalentTo(i2!));
			Assert.True(i2.IsEquivalentTo(i1));
		}

		[Fact]
		public void InterfaceWithOneMethodIsEquivalentRecursivelyD()
		{
			var assembly = CreateAssembly(@"
interface I1 { M(param1 : I2) : I1; }
interface I2 { M(param1 : I1) : I2; }").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsEquivalentTo(i2!));
			Assert.True(i2.IsEquivalentTo(i1));
		}

		[Fact]
		public void InterfaceWithOneMethodIsEquivalentRecursivelyE()
		{
			var assembly = CreateAssembly(@"
interface I1 { M(param1 : I2) : int; }
interface I2 { M(param1 : I3) : int; }
interface I3 { M(param1 : I4) : int; }
interface I4 { M(param1 : I1) : int; }").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsEquivalentTo(i2!));
			Assert.True(i2.IsEquivalentTo(i1));
		}

		[Fact]
		public void InterfaceWithOneMethodIsNotEquivalentRecursively()
		{
			var assembly = CreateAssembly(@"
interface Empty {}
interface I1 { M(param1 : I2) : int; }
interface I2 { M(param1 : I3) : int; }
interface I3 { M(param1 : I4) : int; }
interface I4 { M(param1 : Empty) : int; }").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.False(i1.IsEquivalentTo(i2!));
			Assert.False(i2.IsEquivalentTo(i1));
		}
	}
}
