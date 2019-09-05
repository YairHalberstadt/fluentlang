using FluentLang.Compiler.Model;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.Subtyping
{
	public class InterfaceTests
	{
		public static Primitive[][] Primitives =
		{
			new [] { Primitive.Bool },
			new [] { Primitive.Int },
			new [] { Primitive.Double },
			new [] { Primitive.Char },
			new [] { Primitive.String },
		};

		public static Func<string, IInterface>[][] InterfaceFuncs()
		{
			return new[]
			{
				new Func<string, IInterface>[] { s => EmptyInterface(s).Build() },
				new Func<string, IInterface>[] { s => AdditiveToEmptyInterface(s, s).Build() },
				new Func<string, IInterface>[] { s => EmptyInterfaceWithEmptyMethodSet(s).Build() },
				new Func<string, IInterface> [] {s => InterfaceWithOneMethod(s, new TypeKeyBuilder(Primitive.Bool), new TypeKeyBuilder(Primitive.Bool)).Build() },
				new Func<string, IInterface> [] {s => InterfaceWithOneMethod(s, new TypeKeyBuilder(EmptyInterface(null)), new TypeKeyBuilder(EmptyInterface(null))).Build() },
				new Func<string, IInterface> [] {s => InterfaceWithOneMethod(s, TypeKey(s), TypeKey(s)).Build() }
			};
		}

		public static IInterface[][] InterfaceWithOneMethods()
		{
			return new[]
			{
				new IInterface[] {InterfaceWithOneMethod("interfaceWithOneMethod", new TypeKeyBuilder(Primitive.Bool), new TypeKeyBuilder(Primitive.Bool)).Build() },
				new IInterface[] {InterfaceWithOneMethod("interfaceWithOneMethod", new TypeKeyBuilder(EmptyInterface(null)), new TypeKeyBuilder(EmptyInterface(null))).Build() },
				new IInterface[] {InterfaceWithOneMethod("interfaceWithOneMethod", TypeKey("interfaceWithOneMethod"), TypeKey("interfaceWithOneMethod")).Build() }
			};
		}

		public static IInterface[][] InterfaceWithOneMethodPairs()
		{
			return new[]
			{
				new IInterface[]
				{
					InterfaceWithOneMethod("interfaceWithOneMethodA", new TypeKeyBuilder(Primitive.Bool), new TypeKeyBuilder(Primitive.Bool)).Build(),
					InterfaceWithOneMethod("interfaceWithOneMethodB", new TypeKeyBuilder(EmptyInterface(null)), new TypeKeyBuilder(EmptyInterface(null))).Build(),
				},
				new IInterface[]
				{
					InterfaceWithOneMethod("interfaceWithOneMethodA", new TypeKeyBuilder(Primitive.Bool), new TypeKeyBuilder(Primitive.Bool)).Build(),
					InterfaceWithOneMethod("interfaceWithOneMethodB", TypeKey("interfaceWithOneMethodB"), TypeKey("interfaceWithOneMethodB")).Build(),
				},
				new IInterface[]
				{
					InterfaceWithOneMethod("interfaceWithOneMethodA", new TypeKeyBuilder(EmptyInterface(null)), new TypeKeyBuilder(EmptyInterface(null))).Build(),
					InterfaceWithOneMethod("interfaceWithOneMethodB", new TypeKeyBuilder(Primitive.Bool), new TypeKeyBuilder(Primitive.Bool)).Build(),

				},
				new IInterface[]
				{
					InterfaceWithOneMethod("interfaceWithOneMethodA", new TypeKeyBuilder(EmptyInterface(null)), new TypeKeyBuilder(EmptyInterface(null))).Build(),
					InterfaceWithOneMethod("interfaceWithOneMethodB", TypeKey("interfaceWithOneMethodB"), TypeKey("interfaceWithOneMethodB")).Build(),
				},
				new IInterface[]
				{
					InterfaceWithOneMethod("interfaceWithOneMethodA", TypeKey("interfaceWithOneMethodA"), TypeKey("interfaceWithOneMethodA")).Build(),
					InterfaceWithOneMethod("interfaceWithOneMethodB", new TypeKeyBuilder(Primitive.Bool), new TypeKeyBuilder(Primitive.Bool)).Build(),

				},
				new IInterface[]
				{
					InterfaceWithOneMethod("interfaceWithOneMethodA", TypeKey("interfaceWithOneMethodA"), TypeKey("interfaceWithOneMethodA")).Build(),
					InterfaceWithOneMethod("interfaceWithOneMethodB", new TypeKeyBuilder(EmptyInterface(null)), new TypeKeyBuilder(EmptyInterface(null))).Build(),
				},
			};
		}

		public static TypeKeyBuilder TypeKey(string name) => new TypeKeyBuilder(new InterfaceReferenceBuilder(name));


		public static InterfaceBuilder EmptyInterface(string? name) => new InterfaceBuilder
		{
			FullyQualifiedName = name,
		};

		public static InterfaceBuilder AdditiveToEmptyInterface(string? name, string additiveInterfaceName)
		{
			return new InterfaceBuilder
			{
				FullyQualifiedName = name,
				AdditiveInterfaces =
				{
					new InterfaceReferenceBuilder(additiveInterfaceName),
				},
			};
		}

		public static InterfaceBuilder EmptyInterfaceWithEmptyMethodSet(string? name)
		{
			return new InterfaceBuilder
			{
				FullyQualifiedName = name,
				MethodSets =
				{
					new InterfaceMethodBuilder[0]
				}
			};
		}

		public static InterfaceBuilder InterfaceWithOneMethod(string? name, TypeKeyBuilder returnType, TypeKeyBuilder parameterType, string methodName = "M")
		{
			return new InterfaceBuilder
			{
				FullyQualifiedName = name,
				MethodSets =
				{
					new []
					{
						new InterfaceMethodBuilder
						{
							Name = methodName,
							ReturnType = returnType,
							Parameters =
							{
								(
									"p",
									parameterType
								)
							}
						}
					}
				}
			};
		}

		[Theory]
		[MemberData(nameof(Primitives))]
		public void InterfacesAreNotEquivalentToPrimitives_EvenWithSameName(Primitive primitive)
		{
			var i = EmptyInterface(primitive.FullyQualifiedName.ToString()).Build();
			Assert.False(i.IsSubTypeOf(primitive, SemanticModel.Empty.With(i)));
		}

		[Theory]
		[MemberData(nameof(InterfaceFuncs))]
		public void InterfaceIsSubTypeOfItself_EvenWithDifferentNames(Func<string, IInterface> interfaceFunc)
		{
			var a = interfaceFunc("a");
			var b = interfaceFunc("b");

			var model = SemanticModel.Empty.With(a).With(b);

			Assert.True(a.IsSubTypeOf(a, model));
			Assert.True(a.IsSubTypeOf(b, model));

		}

		[Theory]
		[MemberData(nameof(InterfaceFuncs))]
		public void InterfaceIsSubTypeOfAdditiveToEmtyInterface_EvenWithDifferentNames_AndViceVersa(Func<string, IInterface> interfaceFunc)
		{
			var @interface = interfaceFunc("interface");
			var additive = AdditiveToEmptyInterface("additive", "interface").Build();

			var model = SemanticModel.Empty.With(@interface).With(additive);

			Assert.True(@interface.IsSubTypeOf(additive, model));
			Assert.True(additive.IsSubTypeOf(@interface, model));
		}

		[Theory]
		[MemberData(nameof(InterfaceWithOneMethods))]
		public void EmptyInterfaceIsSubTypeOfInterfaceWithOneMethod(IInterface interfaceWithOneMethod)
		{
			var empty = EmptyInterface("empty").Build();

			var model = SemanticModel.Empty.With(empty).With(interfaceWithOneMethod);

			Assert.True(empty.IsSubTypeOf(interfaceWithOneMethod, model));
			Assert.False(interfaceWithOneMethod.IsSubTypeOf(empty, model));
		}

		[Theory]
		[MemberData(nameof(InterfaceWithOneMethodPairs))]
		public void InterfaceWithOneMethodIsNotSubTypeOfDifferentInterfaceWithOneMethod(IInterface interfaceWithOneMethoda, IInterface interfaceWithOneMethodb)
		{
			var model = SemanticModel.Empty.With(interfaceWithOneMethoda).With(interfaceWithOneMethodb);

			Assert.False(interfaceWithOneMethoda.IsSubTypeOf(interfaceWithOneMethodb, model));
		}

		[Theory]
		[MemberData(nameof(InterfaceWithOneMethods))]
		public void InterfaceWithOneMethodIsNotSubTypeOfMatchingInterfaceWithOneMethodWithDifferentMethodName(IInterface interfaceWithOneMethod)
		{
			var method = interfaceWithOneMethod.MethodSets.Single().Methods.Single();
			var methodCopy = (IInterfaceMethod)new TestInterfaceMethod(method.Name + "1", method.ReturnType, method.Parameters);
			var interfaceCopy = (IInterface)new TestInterface(
				interfaceWithOneMethod.AdditiveInterfaces,
				ImmutableArray.Create((IInterfaceMethodSet)new TestInterfaceMethodSet(ImmutableArray.Create(methodCopy))),
				scope: null,
				new QualifiedName("copy"));
			var model = SemanticModel.Empty.With(interfaceWithOneMethod).With(interfaceCopy);

			Assert.False(interfaceWithOneMethod.IsEquivalentTo(interfaceCopy, model));
			Assert.False(interfaceCopy.IsEquivalentTo(interfaceWithOneMethod, model));
		}

		[Fact]
		public void InterfaceWithOneMethodIsSubtypeRecursivelyA()
		{
			var interfaceWithOneMethodA = InterfaceWithOneMethod("interfaceWithOneMethodA", TypeKey("empty"), TypeKey("empty")).Build();
			var interfaceWithOneMethodB = InterfaceWithOneMethod("interfaceWithOneMethodB", TypeKey("additiveEmpty"), TypeKey("additiveEmpty")).Build();

			var model = SemanticModel.Empty
				.With(interfaceWithOneMethodA)
				.With(interfaceWithOneMethodB)
				.With(EmptyInterface("empty").Build())
				.With(AdditiveToEmptyInterface("additiveEmpty", "empty").Build());

			Assert.True(interfaceWithOneMethodA.IsSubTypeOf(interfaceWithOneMethodB, model));
			Assert.True(interfaceWithOneMethodA.IsSubTypeOf(interfaceWithOneMethodB, model));
		}

		[Fact]
		public void InterfaceWithOneMethodIsSubtypeRecursivelyB()
		{
			var interfaceWithOneMethodA = InterfaceWithOneMethod("interfaceWithOneMethodA", TypeKey("interfaceWithOneMethodA"), TypeKey("interfaceWithOneMethodA")).Build();
			var interfaceWithOneMethodB = InterfaceWithOneMethod("interfaceWithOneMethodB", TypeKey("interfaceWithOneMethodB"), TypeKey("interfaceWithOneMethodB")).Build();

			var model = SemanticModel.Empty
				.With(interfaceWithOneMethodA)
				.With(interfaceWithOneMethodB);

			Assert.True(interfaceWithOneMethodA.IsSubTypeOf(interfaceWithOneMethodB, model));
			Assert.True(interfaceWithOneMethodA.IsSubTypeOf(interfaceWithOneMethodB, model));
		}

		[Fact]
		public void InterfaceWithOneMethodIsSubtypeRecursivelyC()
		{
			var interfaceWithOneMethodA = InterfaceWithOneMethod("interfaceWithOneMethodB", TypeKey("interfaceWithOneMethodA"), TypeKey("interfaceWithOneMethodA")).Build();
			var interfaceWithOneMethodB = InterfaceWithOneMethod("interfaceWithOneMethodA", TypeKey("interfaceWithOneMethodB"), TypeKey("interfaceWithOneMethodB")).Build();

			var model = SemanticModel.Empty
				.With(interfaceWithOneMethodA)
				.With(interfaceWithOneMethodB);

			Assert.True(interfaceWithOneMethodA.IsSubTypeOf(interfaceWithOneMethodB, model));
			Assert.True(interfaceWithOneMethodA.IsSubTypeOf(interfaceWithOneMethodB, model));
		}
	}
}
