using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.Model;
using FluentLang.Compiler.Model.Diagnostic;
using FluentLang.Compiler.Parsing;
using FluentLang.Compiler.SemanticPhase;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Xunit;
using static FluentLang.Compiler.Tests.Unit.TestHelpers.TestModelFactory;

namespace FluentLang.Compiler.Tests.Unit.SemanticPhase
{
	public class InterfaceAndMethodBindingTests
	{

		#region interfaces
		[Fact]
		public void VisitEmptyInterface1()
		{
			var source = @"interface Empty {}";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetInterface(QualifiedName("Empty"), out var @interface));
			Assert.Equal(QualifiedName("Empty"), @interface.FullyQualifiedName);
			Assert.Empty(@interface.AdditiveInterfaces);
			Assert.Empty(@interface.MethodSets.Single().Methods);
		}

		[Fact]
		public void VisitEmptyInterface2()
		{
			var source = @"namespace N { interface Empty {} }";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetInterface(QualifiedName("N.Empty"), out var @interface));
			Assert.Equal(QualifiedName("N.Empty"), @interface.FullyQualifiedName);
			Assert.Empty(@interface.AdditiveInterfaces);
			Assert.Empty(@interface.MethodSets.Single().Methods);
		}

		[Fact]
		public void VisitEmptyInterface3()
		{
			var source = @"namespace N { } interface Empty {}";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetInterface(QualifiedName("Empty"), out var @interface));
			Assert.Equal(QualifiedName("Empty"), @interface.FullyQualifiedName);
			Assert.Empty(@interface.AdditiveInterfaces);
			Assert.Empty(@interface.MethodSets.Single().Methods);
		}

		[Fact]
		public void VisitAdditiveInterface1()
		{
			var source = @"
namespace N {
	interface Empty {}
}
interface Additive N.Empty
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetInterface(QualifiedName("N.Empty"), out var emptyInterface));
			Assert.True(model.TryGetInterface(QualifiedName("Additive"), out var additiveInterface));
			Assert.Equal(QualifiedName("Additive"), additiveInterface.FullyQualifiedName);
			Assert.Equal(emptyInterface, additiveInterface.AdditiveInterfaces.Single().GetPossibleInterfaces(model).Single());
			Assert.Empty(additiveInterface.MethodSets);
		}

		[Fact]
		public void VisitAdditiveInterface2()
		{
			var source = @"
open N;
namespace N {
	interface Empty {}
}
interface Additive Empty
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetInterface(QualifiedName("N.Empty"), out var emptyInterface));
			Assert.True(model.TryGetInterface(QualifiedName("Additive"), out var additiveInterface));
			Assert.Equal(QualifiedName("Additive"), additiveInterface.FullyQualifiedName);
			Assert.Equal(emptyInterface, additiveInterface.AdditiveInterfaces.Single().GetPossibleInterfaces(model).Single());
			Assert.Empty(additiveInterface.MethodSets);
		}

		[Fact]
		public void VisitAdditiveInterface3()
		{
			var source = @"
namespace N {
	interface Empty {}
}
namespace N.M {
	interface Additive Empty
}
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetInterface(QualifiedName("N.Empty"), out var emptyInterface));
			Assert.True(model.TryGetInterface(QualifiedName("N.M.Additive"), out var additiveInterface));
			Assert.Equal(QualifiedName("N.M.Additive"), additiveInterface.FullyQualifiedName);
			Assert.Equal(emptyInterface, additiveInterface.AdditiveInterfaces.Single().GetPossibleInterfaces(model).Single());
			Assert.Empty(additiveInterface.MethodSets);
		}

		[Fact]
		public void VisitAdditiveInterface4()
		{
			var source = @"
namespace N.M {
	interface Empty {}
}
namespace N {
	interface Additive M.Empty
}
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetInterface(QualifiedName("N.M.Empty"), out var emptyInterface));
			Assert.True(model.TryGetInterface(QualifiedName("N.Additive"), out var additiveInterface));
			Assert.Equal(QualifiedName("N.Additive"), additiveInterface.FullyQualifiedName);
			Assert.Equal(emptyInterface, additiveInterface.AdditiveInterfaces.Single().GetPossibleInterfaces(model).Single());
			Assert.Empty(additiveInterface.MethodSets);
		}

		[Fact]
		public void VisitAdditiveInterface5()
		{
			var source = @"
namespace N {
	interface Empty {}
}
interface Additive Empty
";
			var model = Visit(source);
			model.VerifyDiagnosics();
			Assert.True(model.TryGetInterface(QualifiedName("N.Empty"), out _));
			Assert.True(model.TryGetInterface(QualifiedName("Additive"), out var additiveInterface));
			Assert.Equal(QualifiedName("Additive"), additiveInterface.FullyQualifiedName);
			Assert.False(additiveInterface.AdditiveInterfaces.Single().GetPossibleInterfaces(model).Any());
			Assert.Empty(additiveInterface.MethodSets);
		}

		[Fact]
		public void VisitAdditiveInterface6()
		{
			var source = @"
interface Empty {}

interface Additive Empty + Empty + Additive
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetInterface(QualifiedName("Empty"), out var emptyInterface));
			Assert.True(model.TryGetInterface(QualifiedName("Additive"), out var additiveInterface));
			Assert.Equal(QualifiedName("Additive"), additiveInterface.FullyQualifiedName);
			Assert.Equal(emptyInterface, additiveInterface.AdditiveInterfaces.ElementAt(0).GetPossibleInterfaces(model).Single());
			Assert.Equal(emptyInterface, additiveInterface.AdditiveInterfaces.ElementAt(1).GetPossibleInterfaces(model).Single());
			Assert.Equal(additiveInterface, additiveInterface.AdditiveInterfaces.ElementAt(2).GetPossibleInterfaces(model).Single());
			Assert.Empty(additiveInterface.MethodSets);
		}

		[Fact]
		public void VisitInterfaceWithOneMethod1()
		{
			var source = @"
interface Interface {
	M(param1 : int, param2 : string) : bool;
}
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetInterface(QualifiedName("Interface"), out var @interface));
			var builder = new InterfaceBuilder
			{
				FullyQualifiedName = "Interface",
				MethodSets =
				{
					new[]
					{
						new InterfaceMethodBuilder
						{
							Name = "M",
							ReturnType = new TypeKeyBuilder(Primitive.Bool),
							Parameters =
							{
								(
									"param1",
									new TypeKeyBuilder(Primitive.Int)
								),
								(
									"param2",
									new TypeKeyBuilder(Primitive.String)
								),
							},
						},
					},
				},
			};
			AssertHelpers.Equal(builder.Build(), @interface);
		}

		[Fact]
		public void VisitInterfaceWithOneMethod2()
		{
			var source = @"
interface Interface {
	M(param1 : { M() : bool; M1(param : {}) : { M() : int; } }, param2 : { }) : Interface;
}
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetInterface(QualifiedName("Interface"), out var @interface));
			var builder = new InterfaceBuilder
			{
				FullyQualifiedName = "Interface",
				MethodSets =
				{
					new[]
					{
						new InterfaceMethodBuilder
						{
							Name = "M",
							ReturnType = new TypeKeyBuilder(new InterfaceReferenceBuilder("Interface")
							{
							}),
							Parameters =
							{
								(
									"param1",
									new TypeKeyBuilder(new InterfaceBuilder
									{
										MethodSets =
										{
											new[]
											{
												new InterfaceMethodBuilder
												{
													Name = "M",
													ReturnType = new TypeKeyBuilder(Primitive.Bool),
												},
												new InterfaceMethodBuilder
												{
													Name = "M1",
													ReturnType = new TypeKeyBuilder(new InterfaceBuilder
													{
														MethodSets =
														{
															new[]
															{
																new InterfaceMethodBuilder
																{
																	Name = "M",
																	ReturnType = new TypeKeyBuilder(Primitive.Int),
																},
															},
														},
													}),
													Parameters =
													{
														(
															"param",
															new TypeKeyBuilder(new InterfaceBuilder
															{
																MethodSets =
																{
																	new InterfaceMethodBuilder[]
																	{
																	},
																},
															})
														),
													},
												},
											},
										},
									})
								),
								(
									"param2",
									new TypeKeyBuilder(new InterfaceBuilder
									{
										MethodSets =
										{
											new InterfaceMethodBuilder[]
											{
											},
										},
									})
								),
							},
						},
					},
				},
			};
			AssertHelpers.Equal(builder.Build(), @interface);
		}

		[Fact]
		public void VisitInterfaceWithMultipleMethodSets()
		{
			var source = @"
interface Interface {
	M() : { M() : bool; } + { M1() : bool; };
} + {
	M() : { M() : bool; } + { M1() : bool; };
}";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetInterface(QualifiedName("Interface"), out var @interface));
			var builder = new InterfaceBuilder
			{
				FullyQualifiedName = "Interface",
				MethodSets =
				{
					new[]
					{
						new InterfaceMethodBuilder
						{
							Name = "M",
							ReturnType = new TypeKeyBuilder(new InterfaceBuilder
							{
								MethodSets =
								{
									new[]
									{
										new InterfaceMethodBuilder
										{
											Name = "M",
											ReturnType = new TypeKeyBuilder(Primitive.Bool),
										},
									},
									new[]
									{
										new InterfaceMethodBuilder
										{
											Name = "M1",
											ReturnType = new TypeKeyBuilder(Primitive.Bool),
										},
									},
								},
							}),
						},
					},
					new[]
					{
						new InterfaceMethodBuilder
						{
							Name = "M",
							ReturnType = new TypeKeyBuilder(new InterfaceBuilder
							{
								MethodSets =
								{
									new[]
									{
										new InterfaceMethodBuilder
										{
											Name = "M",
											ReturnType = new TypeKeyBuilder(Primitive.Bool),
										},
									},
									new[]
									{
										new InterfaceMethodBuilder
										{
											Name = "M1",
											ReturnType = new TypeKeyBuilder(Primitive.Bool),
										},
									},
								},
							}),
						},
					},
				},
			};
			AssertHelpers.Equal(builder.Build(), @interface);
		}

		#endregion

		#region methods

		[Fact]
		public void VisitMethodWithNoParametersAndPrimitiveReturnType()
		{
			var source = @"
M() : bool {}
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetMethod(QualifiedName("M"), out var method));

			var expected = new MethodBuilder("M", new TypeKeyBuilder(Primitive.Bool)).Build();

			AssertHelpers.Equal(expected, method);
		}

		[Fact]
		public void VisitMethodWithNoParametersAndInterfaceReturnType()
		{
			var source = @"
M() : { M1() : bool; } {
}
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetMethod(QualifiedName("M"), out var method));

			var expected = new MethodBuilder("M", new TypeKeyBuilder(new InterfaceBuilder
			{
				MethodSets =
				{
					new[]
					{
						new InterfaceMethodBuilder
						{
							Name = "M1",
							ReturnType = new TypeKeyBuilder(Primitive.Bool),
						},
					},
				},
			}))
			{
			}.Build();

			AssertHelpers.Equal(expected, method);
		}

		[Fact]
		public void VisitMethodWithPrimitiveParameters()
		{
			var source = @"
M(param1 : bool, param2 : int) : string {
}
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetMethod(QualifiedName("M"), out var method));

			var expected = new MethodBuilder("M", new TypeKeyBuilder(Primitive.String))
			{
				Parameters =
				{
					(
						"param1",
						new TypeKeyBuilder(Primitive.Bool)
					),
					(
						"param2",
						new TypeKeyBuilder(Primitive.Int)
					),
				},
			}.Build();

			AssertHelpers.Equal(expected, method);
		}

		[Fact]
		public void VisitMethodWithInterfaceParameters()
		{
			var source = @"
M(param1 : {}, param2 : { M1() : bool }) : string {
}
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetMethod(QualifiedName("M"), out var method));

			var expected = new MethodBuilder("M", new TypeKeyBuilder(Primitive.String))
			{
				Parameters =
				{
					(
						"param1",
						new TypeKeyBuilder(new InterfaceBuilder
						{
							MethodSets =
							{
								new InterfaceMethodBuilder[]
								{
								},
							},
						})
					),
					(
						"param2",
						new TypeKeyBuilder(new InterfaceBuilder
						{
							MethodSets =
							{
								new[]
								{
									new InterfaceMethodBuilder
									{
										Name = "M1",
										ReturnType = new TypeKeyBuilder(Primitive.Bool),
									},
								},
							},
						})
					),
				},
			}.Build();

			AssertHelpers.Equal(expected, method);
		}

		[Fact]
		public void VisitMethodWithLocalMethods()
		{
			var source = @"
M() : bool {
	Inner1() : string {
		InnerInner() : int {
		}
	}
	Inner2() : double {
	}
}
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetMethod(QualifiedName("M"), out var method));

			var expected = new MethodBuilder("M", new TypeKeyBuilder(Primitive.Bool))
			{
				LocalMethods =
				{
					new MethodBuilder("Inner1", new TypeKeyBuilder(Primitive.String))
					{
						LocalMethods =
						{
							new MethodBuilder("InnerInner", new TypeKeyBuilder(Primitive.Int))
							{
							},
						},
					},
					new MethodBuilder("Inner2", new TypeKeyBuilder(Primitive.Double))
					{
					},
				},
			}.Build();

			AssertHelpers.Equal(expected, method);
		}

		[Fact]
		public void VisitMethodWithLocalInterfaces()
		{
			var source = @"
M() : bool {
	interface Inner {}
	Inner() : Inner {
		interface InnerInner {
			M1() : Inner;
		}
		InnerInner() : Inner {
			interface InnerInnerInner {
				M1() : Inner;
			}
		}
	}
}
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetMethod(QualifiedName("M"), out var method));

			var expected = new MethodBuilder("M", new TypeKeyBuilder(Primitive.Bool))
			{
				LocalMethods =
				{
					new MethodBuilder("Inner", new TypeKeyBuilder(new InterfaceReferenceBuilder("Inner")
					{
					}))
					{
						LocalMethods =
						{
							new MethodBuilder("InnerInner", new TypeKeyBuilder(new InterfaceReferenceBuilder("Inner")
							{
							}))
							{
								LocalInterfaces =
								{
									new InterfaceBuilder
									{
										FullyQualifiedName = "InnerInnerInner",
										MethodSets =
										{
											new[]
											{
												new InterfaceMethodBuilder
												{
													Name = "M1",
													ReturnType = new TypeKeyBuilder(new InterfaceReferenceBuilder("Inner")
													{
													}),
												},
											},
										},
									},
								},
							},
						},
						LocalInterfaces =
						{
							new InterfaceBuilder
							{
								FullyQualifiedName = "InnerInner",
								MethodSets =
								{
									new[]
									{
										new InterfaceMethodBuilder
										{
											Name = "M1",
											ReturnType = new TypeKeyBuilder(new InterfaceReferenceBuilder("Inner")
											{
											}),
										},
									},
								},
							},
						},
					},
				},
				LocalInterfaces =
				{
					new InterfaceBuilder
					{
						FullyQualifiedName = "Inner",
						MethodSets =
						{
							new InterfaceMethodBuilder[]
							{
							},
						},
					},
				},
			}.Build();

			AssertHelpers.Equal(expected, method);

			var innerInterface = method.LocalInterfaces.Single();
			var innerMethod = method.LocalMethods.Single();
			Assert.Equal(innerInterface, innerMethod.ReturnType.GetPossibleTypes(model).Single());
			var innerInnerInterface = innerMethod.LocalInterfaces.Single();
			Assert.Equal(innerInterface, innerInnerInterface.MethodSets.Single().Methods.Single().ReturnType.GetPossibleTypes(model).Single());
			var innerInnerMethod = innerMethod.LocalMethods.Single();
			Assert.Equal(innerInterface, innerInnerMethod.ReturnType.GetPossibleTypes(model).Single());
			var innerInnerInnerInterface = innerInnerMethod.LocalInterfaces.Single();
			Assert.Equal(innerInterface, innerInnerInnerInterface.MethodSets.Single().Methods.Single().ReturnType.GetPossibleTypes(model).Single());
		}

		[Fact]
		public void ChildScopedInterfacesArePrefferedOverParentScopedWhichArePrefferedToTopLevel()
		{
			var source = @"
interface I {}
M() : I {
	interface I {}
	Inner() : I {
		interface I {
			M1() : I;
		}
		InnerInner() : I {
		}
	}
}
";
			var model = Visit(source);
			model.VerifyDiagnosics();

			Assert.True(model.TryGetMethod(QualifiedName("M"), out var method));

			var expected = new MethodBuilder("M", new TypeKeyBuilder(new InterfaceReferenceBuilder("I")
			{
			}))
			{
				LocalMethods =
				{
					new MethodBuilder("Inner", new TypeKeyBuilder(new InterfaceReferenceBuilder("I")
					{
					}))
					{
						LocalMethods =
						{
							new MethodBuilder("InnerInner", new TypeKeyBuilder(new InterfaceReferenceBuilder("I")
							{
							}))
							{
							},
						},
						LocalInterfaces =
						{
							new InterfaceBuilder
							{
								FullyQualifiedName = "I",
								MethodSets =
								{
									new[]
									{
										new InterfaceMethodBuilder
										{
											Name = "M1",
											ReturnType = new TypeKeyBuilder(new InterfaceReferenceBuilder("I")
											{
											}),
										},
									},
								},
							},
						},
					},
				},
				LocalInterfaces =
				{
					new InterfaceBuilder
					{
						FullyQualifiedName = "I",
						MethodSets =
						{
							new InterfaceMethodBuilder[]
							{
							},
						},
					},
				},
			}.Build();

			AssertHelpers.Equal(expected, method);

			Assert.True(model.TryGetInterface(QualifiedName("I"), out var outerInterface));
			Assert.Equal(outerInterface, method.ReturnType.GetPossibleTypes(model).Single());

			var innerInterface = method.LocalInterfaces.Single();
			var innerMethod = method.LocalMethods.Single();
			Assert.Equal(innerInterface, innerMethod.ReturnType.GetPossibleTypes(model).Single());

			var innerInnerInterface = innerMethod.LocalInterfaces.Single();
			Assert.Equal(innerInnerInterface, innerInnerInterface.MethodSets.Single().Methods.Single().ReturnType.GetPossibleTypes(model).Single());

			var innerInnerMethod = innerMethod.LocalMethods.Single();
			Assert.Equal(innerInnerInterface, innerInnerMethod.ReturnType.GetPossibleTypes(model).Single());
		}

		#endregion


		private TestCompilation Visit(string source, ISemanticModel? model = null)
		{
			model ??= SemanticModel.Empty;

			using var reader = new StringReader(source);

			var input = new AntlrInputStream(reader);
			var lexer = new FluentLangLexer(input);
			var tokenStream = new CommonTokenStream(lexer);
			var parser = new FluentLangParser(tokenStream);

			// pick up any syntax errors
			var errorStrategy = new ErrorStrategy(ImmutableList<Diagnostic>.Empty);
			parser.ErrorHandler = errorStrategy;
			var compilationUnit = parser.compilation_unit();
			var dummyListener = new FluentLangParserBaseListener();
			ParseTreeWalker.Default.Walk(dummyListener, compilationUnit);
			Assert.Empty(errorStrategy.Diagnostics);

			var metadatavisitor = new TopLevelVisitor(model, ImmutableList<Diagnostic>.Empty);
			metadatavisitor.Visit(compilationUnit);
			return new TestCompilation(metadatavisitor.SemanticModel, metadatavisitor.Diagnostics);
		}
	}
}