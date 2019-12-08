using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Model.Equivalence
{
	public class PrimitiveTests : TestBase
	{
		public static Primitive[][] Primitives =
		{
			new [] { Primitive.Bool },
			new [] { Primitive.Int },
			new [] { Primitive.Double },
			new [] { Primitive.Char },
			new [] { Primitive.String },
		};

		public static Primitive[][] PrimitivePairs =
		{
			new [] { Primitive.Bool, Primitive.Int },
			new [] { Primitive.Bool, Primitive.Double },
			new [] { Primitive.Bool, Primitive.Char },
			new [] { Primitive.Bool, Primitive.String },
			new [] { Primitive.Int, Primitive.Bool },
			new [] { Primitive.Int, Primitive.Double },
			new [] { Primitive.Int, Primitive.Char },
			new [] { Primitive.Int, Primitive.String },
			new [] { Primitive.Double, Primitive.Bool },
			new [] { Primitive.Double, Primitive.Int },
			new [] { Primitive.Double, Primitive.Char },
			new [] { Primitive.Double, Primitive.String },
			new [] { Primitive.Char, Primitive.Bool },
			new [] { Primitive.Char, Primitive.Int },
			new [] { Primitive.Char, Primitive.Double },
			new [] { Primitive.Char, Primitive.String },
			new [] { Primitive.String, Primitive.Bool },
			new [] { Primitive.String, Primitive.Int },
			new [] { Primitive.String, Primitive.Double },
			new [] { Primitive.String, Primitive.Char },
		};

		public PrimitiveTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Theory]
		[MemberData(nameof(Primitives))]
		public void PrimitivesAreEqualToThemselves(Primitive primitive)
		{
			Assert.True(((IType)primitive).IsEquivalentTo(primitive));
		}

		[Theory]
		[MemberData(nameof(PrimitivePairs))]
		public void PrimitivesAreNotEqualToOtherPrimitives(Primitive a, Primitive b)
		{
			Assert.False(((IType)a).IsEquivalentTo(b));
		}

		[Theory]
		[MemberData(nameof(Primitives))]
		public void PrimitivesAreNotEquivalentToInterface_EvenWithSameName(Primitive primitive)
		{
			var @interface = (IInterface)new TestInterface { FullyQualifiedName = primitive.FullyQualifiedName };
			Assert.False(((IType)primitive).IsEquivalentTo(@interface));
			Assert.False(@interface.IsEquivalentTo(primitive));
		}
	}
}
