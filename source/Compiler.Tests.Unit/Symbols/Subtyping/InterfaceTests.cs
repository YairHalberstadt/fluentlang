using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.TestUtils;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.Subtyping
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
		public void InterfaceIsSubtypeOf_IdenticalInterfaceWithDifferentNames()
		{
			var assembly = CreateAssembly(@"
interface I1 {}
interface I2 {}").VerifyDiagnostics().VerifyEmit();
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsSubtypeOf(i2!));
			Assert.True(i2.IsSubtypeOf(i1));
		}

		[Fact]
		public void InterfaceIsSubtypeOf_InterfaceWithSameMethods_EvenIfFromAdditive_AndEvenWithDuplicateMethods()
		{
			var assembly = CreateAssembly(@"
interface Empty {}
interface OneMethod1 { M() : int; }
interface OneMethod2 { M() : int; }
interface AdditiveOneMethod1 { M() : int; } + Empty
interface AdditiveOneMethod2 OneMethod1 + Empty
interface AdditiveOneMethod3 OneMethod1 + OneMethod2
").VerifyDiagnostics().VerifyEmit();
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
					Assert.True(a.IsSubtypeOf(b));
				}
			}
		}

		[Fact]
		public void InterfaceWithOneMethod_IsSubtypeOf_EmptyInterface_AndNotViceVersa()
		{
			var assembly = CreateAssembly(@"
interface Empty {}
interface OneMethod { M() : int; }").VerifyDiagnostics().VerifyEmit();
			Assert.True(assembly.TryGetInterface(QualifiedName("Empty"), out var empty));
			Assert.True(assembly.TryGetInterface(QualifiedName("OneMethod"), out var oneMethod));
			Debug.Assert(empty != null);
			Debug.Assert(oneMethod != null);
			Assert.True(oneMethod.IsSubtypeOf(empty));
			Assert.False(empty.IsSubtypeOf(oneMethod));
		}

		[Fact]
		public void InterfaceWithOneMethod_IsNotSubtypeOf_InterfaceWithOneDifferentMethod()
		{
			var assembly = CreateAssembly(@"
interface OneMethod1 { M() : int; }
interface OneMethod2 { M() : bool; }
interface OneMethod3 { M1() : int; }
interface OneMethod4 { M(param1 : int) : int; }").VerifyDiagnostics().VerifyEmit();
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
						Assert.True(a.IsSubtypeOf(b));
					}
					else
					{
						Assert.False(a.IsSubtypeOf(b));
					}
				}
			}
		}

		[Fact]
		public void InterfaceWithOneExtraMethod_IsSubtypeOf_InterfaceWithOneMethod_AndNotViceVersa()
		{
			var assembly = CreateAssembly(@"
interface OneMethod { M() : int; }
interface TwoMethods { M() : int; M1() : int; }").VerifyDiagnostics().VerifyEmit();
			Assert.True(assembly.TryGetInterface(QualifiedName("OneMethod"), out var oneMethod));
			Assert.True(assembly.TryGetInterface(QualifiedName("TwoMethods"), out var twoMethods));
			Debug.Assert(twoMethods != null);
			Debug.Assert(oneMethod != null);
			Assert.True(twoMethods.IsSubtypeOf(oneMethod));
			Assert.False(oneMethod.IsSubtypeOf(twoMethods));
		}

		[Fact]
		public void InterfacesWithTwoMethods_IsNotSubtypeOf_InterfaceWithOneDifferentMethod_AndViceVersa()
		{
			var assembly = CreateAssembly(@"
interface OneMethod { M() : int; }
interface TwoMethods { M() : bool; M1() : int; }").VerifyDiagnostics().VerifyEmit();
			Assert.True(assembly.TryGetInterface(QualifiedName("OneMethod"), out var oneMethod));
			Assert.True(assembly.TryGetInterface(QualifiedName("TwoMethods"), out var twoMethods));
			Debug.Assert(twoMethods != null);
			Debug.Assert(oneMethod != null);
			Assert.False(twoMethods.IsSubtypeOf(oneMethod));
			Assert.False(oneMethod.IsSubtypeOf(twoMethods));
		}

		[Fact]
		public void ParameterNamesDoNotEffectSubtyping()
		{
			var assembly = CreateAssembly(@"
interface I1 { M(param1 : int) : bool; }
interface I2 { M(parameter1 : int) : bool; }").VerifyDiagnostics().VerifyEmit();
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsSubtypeOf(i2!));
			Assert.True(i2.IsSubtypeOf(i1));
		}

		[Fact]
		public void Interface_IsSubtypeRecursively_A()
		{
			var assembly = CreateAssembly(@"
interface Empty {}
interface AdditiveEmpty Empty + {}
interface I1 { M(param1 : AdditiveEmpty()) : Empty; }
interface I2 { M(param1 : Empty()) : AdditiveEmpty; }").VerifyDiagnostics().VerifyEmit();
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsSubtypeOf(i2!));
			Assert.True(i2.IsSubtypeOf(i1));
		}

		[Fact]
		public void Interface_IsSubtypeRecursively_B()
		{
			var assembly = CreateAssembly(@"
interface I1 { M(param1 : I1) : I1; }
interface I2 { M(param1 : I2) : I2; }").VerifyDiagnostics().VerifyEmit();
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsSubtypeOf(i2!));
			Assert.True(i2.IsSubtypeOf(i1));
		}

		[Fact]
		public void Interface_IsSubtypeRecursively_C()
		{
			var assembly = CreateAssembly(@"
interface I1 { M(param1 : I2) : I2; }
interface I2 { M(param1 : I1) : I1; }").VerifyDiagnostics().VerifyEmit();
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsSubtypeOf(i2!));
			Assert.True(i2.IsSubtypeOf(i1));
		}

		[Fact]
		public void Interface_IsSubtypeRecursively_D()
		{
			var assembly = CreateAssembly(@"
interface I1 { M(param1 : I2) : I1; }
interface I2 { M(param1 : I1) : I2; }");
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsSubtypeOf(i2!));
			Assert.True(i2.IsSubtypeOf(i1));
		}

		[Fact]
		public void Interface_IsSubtypeRecursively_E()
		{
			var assembly = CreateAssembly(@"
interface I1 { M(param1 : I2) : int; }
interface I2 { M(param1 : I3) : int; }
interface I3 { M(param1 : I4) : int; }
interface I4 { M(param1 : I1) : int; }").VerifyDiagnostics().VerifyEmit();
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.True(i1.IsSubtypeOf(i2!));
			Assert.True(i2.IsSubtypeOf(i1));
		}

		[Fact]
		public void Interface_IsNotSubtypeRecursively()
		{
			var assembly = CreateAssembly(@"
interface Empty {}
interface I1 { M(param1 : I2) : int; }
interface I2 { M(param1 : I3) : int; }
interface I3 { M(param1 : I4) : int; }
interface I4 { M(param1 : Empty) : int; }").VerifyDiagnostics().VerifyEmit();
			Assert.True(assembly.TryGetInterface(QualifiedName("I1"), out var i1));
			Assert.True(assembly.TryGetInterface(QualifiedName("I2"), out var i2));
			Debug.Assert(i1 != null);
			Debug.Assert(i2 != null);
			Assert.False(i1.IsSubtypeOf(i2!));
			Assert.False(i2.IsSubtypeOf(i1));
		}
	}
}
