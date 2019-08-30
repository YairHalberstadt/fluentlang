using FluentLang.Compiler.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.Subtyping.Equivalence
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

		public static Func<string, Interface>[][] InterfaceFuncs()
		{
			return new[]
			{
				new Func<string, Interface>[] { EmptyInterface },
				new Func<string, Interface>[] { s => AdditiveToEmptyInterface(s, s) },
				new Func<string, Interface>[] { EmptyInterfaceWithEmptyMethodSet },
				new Func<string, Interface> [] {s => InterfaceWithOneMethod(s, new TypeKey(Primitive.Bool), new TypeKey(Primitive.Bool)) },
				new Func<string, Interface> [] {s => InterfaceWithOneMethod(s, new TypeKey(EmptyInterface(null)), new TypeKey(EmptyInterface(null))) },
				new Func<string, Interface> [] {s => InterfaceWithOneMethod(s, TypeKey(s), TypeKey(s)) }
			};
		}

		public static Interface[][] InterfaceWithOneMethods()
		{
			return new[]
			{
				new Interface[] {InterfaceWithOneMethod("interfaceWithOneMethod", new TypeKey(Primitive.Bool), new TypeKey(Primitive.Bool)) },
				new Interface[] {InterfaceWithOneMethod("interfaceWithOneMethod", new TypeKey(EmptyInterface(null)), new TypeKey(EmptyInterface(null))) },
				new Interface[] {InterfaceWithOneMethod("interfaceWithOneMethod", TypeKey("interfaceWithOneMethod"), TypeKey("interfaceWithOneMethod")) }
			};
		}

		public static Interface[][] InterfaceWithOneMethodPairs()
		{
			return new[]
			{
				new Interface[]
				{
					InterfaceWithOneMethod("interfaceWithOneMethodA", new TypeKey(Primitive.Bool), new TypeKey(Primitive.Bool)),
					InterfaceWithOneMethod("interfaceWithOneMethodB", new TypeKey(EmptyInterface(null)), new TypeKey(EmptyInterface(null))),
				},
				new Interface[]
				{
					InterfaceWithOneMethod("interfaceWithOneMethodA", new TypeKey(Primitive.Bool), new TypeKey(Primitive.Bool)),
					InterfaceWithOneMethod("interfaceWithOneMethodB", TypeKey("interfaceWithOneMethodB"), TypeKey("interfaceWithOneMethodB")),
				},
				new Interface[]
				{
					InterfaceWithOneMethod("interfaceWithOneMethodA", new TypeKey(EmptyInterface(null)), new TypeKey(EmptyInterface(null))),
					InterfaceWithOneMethod("interfaceWithOneMethodB", new TypeKey(Primitive.Bool), new TypeKey(Primitive.Bool)),

				},
				new Interface[]
				{
					InterfaceWithOneMethod("interfaceWithOneMethodA", new TypeKey(EmptyInterface(null)), new TypeKey(EmptyInterface(null))),
					InterfaceWithOneMethod("interfaceWithOneMethodB", TypeKey("interfaceWithOneMethodB"), TypeKey("interfaceWithOneMethodB")),
				},
				new Interface[]
				{
					InterfaceWithOneMethod("interfaceWithOneMethodA", TypeKey("interfaceWithOneMethodA"), TypeKey("interfaceWithOneMethodA")),
					InterfaceWithOneMethod("interfaceWithOneMethodB", new TypeKey(Primitive.Bool), new TypeKey(Primitive.Bool)),

				},
				new Interface[]
				{
					InterfaceWithOneMethod("interfaceWithOneMethodA", TypeKey("interfaceWithOneMethodA"), TypeKey("interfaceWithOneMethodA")),
					InterfaceWithOneMethod("interfaceWithOneMethodB", new TypeKey(EmptyInterface(null)), new TypeKey(EmptyInterface(null))),
				},
			};
		}

		public static TypeKey TypeKey(string name) => new TypeKey(new InterfaceReference(ImmutableArray<QualifiedName>.Empty, new QualifiedName(name)));


		public static Interface EmptyInterface(string? name)
			=> new Interface(
				ImmutableArray<InterfaceReference>.Empty,
				ImmutableArray<InterfaceMethodSet>.Empty,
				name is null ? null : new QualifiedName(name));

		public static Interface AdditiveToEmptyInterface(string? name, string additiveInterfaceName)
			=> new Interface(
				ImmutableArray.Create(new InterfaceReference(ImmutableArray<QualifiedName>.Empty, new QualifiedName(additiveInterfaceName))),
				ImmutableArray<InterfaceMethodSet>.Empty,
				name is null ? null : new QualifiedName(name));

		public static Interface EmptyInterfaceWithEmptyMethodSet(string? name)
			=> new Interface(
				ImmutableArray<InterfaceReference>.Empty,
				ImmutableArray.Create(new InterfaceMethodSet(ImmutableArray<InterfaceMethod>.Empty)),
				name is null ? null : new QualifiedName(name));

		public static Interface InterfaceWithOneMethod(string? name, TypeKey returnType, TypeKey parameterType, string methodName = "M")
			=> new Interface(
				ImmutableArray<InterfaceReference>.Empty,
				ImmutableArray.Create(new InterfaceMethodSet(ImmutableArray.Create(new InterfaceMethod(methodName, returnType, ImmutableArray.Create(new Parameter("p", parameterType)))))),
				name is null ? null : new QualifiedName(name));

		[Theory]
		[MemberData(nameof(Primitives))]
		public void InterfacesAreNotEquivalentToPrimitives_EvenWithSameName(Primitive primitive)
		{
			var i = EmptyInterface(primitive.FullyQualifiedName.ToString());
			Assert.False(i.IsSubTypeOf(primitive, SemanticModel.Empty.With(i)));
		}

		[Theory]
		[MemberData(nameof(InterfaceFuncs))]
		public void InterfaceIsSubTypeOfItself_EvenWithDifferentNames(Func<string, Interface> interfaceFunc)
		{
			var a = interfaceFunc("a");
			var b = interfaceFunc("b");

			var model = SemanticModel.Empty.With(a).With(b);

			Assert.True(a.IsSubTypeOf(a, model));
			Assert.True(a.IsSubTypeOf(b, model));

		}

		[Theory]
		[MemberData(nameof(InterfaceFuncs))]
		public void InterfaceIsSubTypeOfAdditiveToEmtyInterface_EvenWithDifferentNames_AndViceVersa(Func<string, Interface> interfaceFunc)
		{
			var @interface = interfaceFunc("interface");
			var additive = AdditiveToEmptyInterface("additive", "interface");

			var model = SemanticModel.Empty.With(@interface).With(additive);

			Assert.True(@interface.IsSubTypeOf(additive, model));
			Assert.True(additive.IsSubTypeOf(@interface, model));
		}

		[Theory]
		[MemberData(nameof(InterfaceWithOneMethods))]
		public void EmptyInterfaceIsSubTypeOfInterfaceWithOneMethod(Interface interfaceWithOneMethod)
		{
			var empty = EmptyInterface("empty");

			var model = SemanticModel.Empty.With(empty).With(interfaceWithOneMethod);

			Assert.True(empty.IsSubTypeOf(interfaceWithOneMethod, model));
			Assert.False(interfaceWithOneMethod.IsSubTypeOf(empty, model));
		}

		[Theory]
		[MemberData(nameof(InterfaceWithOneMethodPairs))]
		public void InterfaceWithOneMethodIsNotSubTypeOfDifferentInterfaceWithOneMethod(Interface interfaceWithOneMethoda, Interface interfaceWithOneMethodb)
		{
			var model = SemanticModel.Empty.With(interfaceWithOneMethoda).With(interfaceWithOneMethodb);

			Assert.False(interfaceWithOneMethoda.IsSubTypeOf(interfaceWithOneMethodb, model));
		}

		[Theory]
		[MemberData(nameof(InterfaceWithOneMethods))]
		public void InterfaceWithOneMethodIsNotSubTypeOfMatchingInterfaceWithOneMethodWithDifferentMethodName(Interface interfaceWithOneMethod)
		{
			var method = interfaceWithOneMethod.MethodSets.Single().Methods.Single();
			var methodCopy = new InterfaceMethod(method.Name + "1", method.ReturnType, method.Parameters);
			var interfaceCopy = new Interface(interfaceWithOneMethod.AdditiveInterfaces, ImmutableArray.Create(new InterfaceMethodSet(ImmutableArray.Create(methodCopy))), new QualifiedName("copy"));
			var model = SemanticModel.Empty.With(interfaceWithOneMethod).With(interfaceCopy);

			Assert.False(interfaceWithOneMethod.IsEquivalentTo(interfaceCopy, model));
			Assert.False(interfaceCopy.IsEquivalentTo(interfaceWithOneMethod, model));
		}

		[Fact]
		public void InterfaceWithOneMethodIsSubtypeRecursivelyA()
		{
			var interfaceWithOneMethodA = InterfaceWithOneMethod("interfaceWithOneMethodA", TypeKey("empty"), TypeKey("empty"));
			var interfaceWithOneMethodB = InterfaceWithOneMethod("interfaceWithOneMethodB", TypeKey("additiveEmpty"), TypeKey("additiveEmpty"));

			var model = SemanticModel.Empty
				.With(interfaceWithOneMethodA)
				.With(interfaceWithOneMethodB)
				.With(EmptyInterface("empty"))
				.With(AdditiveToEmptyInterface("additiveEmpty", "empty"));

			Assert.True(interfaceWithOneMethodA.IsSubTypeOf(interfaceWithOneMethodB, model));
			Assert.True(interfaceWithOneMethodA.IsSubTypeOf(interfaceWithOneMethodB, model));
		}

		[Fact]
		public void InterfaceWithOneMethodIsSubtypeRecursivelyB()
		{
			var interfaceWithOneMethodA = InterfaceWithOneMethod("interfaceWithOneMethodA", TypeKey("interfaceWithOneMethodA"), TypeKey("interfaceWithOneMethodA"));
			var interfaceWithOneMethodB = InterfaceWithOneMethod("interfaceWithOneMethodB", TypeKey("interfaceWithOneMethodB"), TypeKey("interfaceWithOneMethodB"));

			var model = SemanticModel.Empty
				.With(interfaceWithOneMethodA)
				.With(interfaceWithOneMethodB);

			Assert.True(interfaceWithOneMethodA.IsSubTypeOf(interfaceWithOneMethodB, model));
			Assert.True(interfaceWithOneMethodA.IsSubTypeOf(interfaceWithOneMethodB, model));
		}

		[Fact]
		public void InterfaceWithOneMethodIsSubtypeRecursivelyC()
		{
			var interfaceWithOneMethodA = InterfaceWithOneMethod("interfaceWithOneMethodB", TypeKey("interfaceWithOneMethodA"), TypeKey("interfaceWithOneMethodA"));
			var interfaceWithOneMethodB = InterfaceWithOneMethod("interfaceWithOneMethodA", TypeKey("interfaceWithOneMethodB"), TypeKey("interfaceWithOneMethodB"));

			var model = SemanticModel.Empty
				.With(interfaceWithOneMethodA)
				.With(interfaceWithOneMethodB);

			Assert.True(interfaceWithOneMethodA.IsSubTypeOf(interfaceWithOneMethodB, model));
			Assert.True(interfaceWithOneMethodA.IsSubTypeOf(interfaceWithOneMethodB, model));
		}
	}
}
