using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.Model;
using FluentLang.Compiler.Model.Diagnostic;
using FluentLang.Compiler.Parsing;
using FluentLang.Compiler.SemanticPhase;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using Xunit;
using static FluentLang.Compiler.Tests.Unit.TestHelpers.TestModelFactory;

namespace FluentLang.Compiler.Tests.Unit.SemanticPhase {
	public class MetadataListenerTests {
		[Fact]
		public void VisitEmptyInterface1 () {
			var source = @"interface Empty {}";
			var model = Visit (source);
			model.VerifyDiagnosics ();

			Assert.True (model.TryGetInterface (QualifiedName ("Empty"), out var @interface));
			Assert.Equal (QualifiedName ("Empty"), @interface.FullyQualifiedName);
			Assert.Empty (@interface.AdditiveInterfaces);
			Assert.Empty (@interface.MethodSets);
		}

		[Fact]
		public void VisitEmptyInterface2 () {
			var source = @"namespace N { interface Empty {} }";
			var model = Visit (source);
			model.VerifyDiagnosics ();

			Assert.True (model.TryGetInterface (QualifiedName ("N.Empty"), out var @interface));
			Assert.Equal (QualifiedName ("N.Empty"), @interface.FullyQualifiedName);
			Assert.Empty (@interface.AdditiveInterfaces);
			Assert.Empty (@interface.MethodSets);
		}

		[Fact]
		public void VisitEmptyInterface3 () {
			var source = @"namespace N { } interface Empty {}";
			var model = Visit (source);
			model.VerifyDiagnosics ();

			Assert.True (model.TryGetInterface (QualifiedName ("Empty"), out var @interface));
			Assert.Equal (QualifiedName ("Empty"), @interface.FullyQualifiedName);
			Assert.Empty (@interface.AdditiveInterfaces);
			Assert.Empty (@interface.MethodSets);
		}

		[Fact]
		public void VisitAdditiveInterface1 () {
			var source = @"
namespace N {
	interface Empty {}
}
interface Additive N.Empty
";
			var model = Visit (source);
			model.VerifyDiagnosics ();

			Assert.True (model.TryGetInterface (QualifiedName ("N.Empty"), out var emptyInterface));
			Assert.True (model.TryGetInterface (QualifiedName ("Additive"), out var additiveInterface));
			Assert.Equal (QualifiedName ("Additive"), additiveInterface.FullyQualifiedName);
			Assert.Equal (emptyInterface, additiveInterface.AdditiveInterfaces.Single ().GetPossibleInterfaces (model).Single ());
			Assert.Empty (additiveInterface.MethodSets);
		}

		[Fact]
		public void VisitAdditiveInterface2 () {
			var source = @"
open N;
namespace N {
	interface Empty {}
}
interface Additive Empty
";
			var model = Visit (source);
			model.VerifyDiagnosics ();

			Assert.True (model.TryGetInterface (QualifiedName ("N.Empty"), out var emptyInterface));
			Assert.True (model.TryGetInterface (QualifiedName ("Additive"), out var additiveInterface));
			Assert.Equal (QualifiedName ("Additive"), additiveInterface.FullyQualifiedName);
			Assert.Equal (emptyInterface, additiveInterface.AdditiveInterfaces.Single ().GetPossibleInterfaces (model).Single ());
			Assert.Empty (additiveInterface.MethodSets);
		}

		[Fact]
		public void VisitAdditiveInterface3 () {
			var source = @"
namespace N {
	interface Empty {}
}
namespace N.M {
	interface Additive Empty
}
";
			var model = Visit (source);
			model.VerifyDiagnosics ();

			Assert.True (model.TryGetInterface (QualifiedName ("N.Empty"), out var emptyInterface));
			Assert.True (model.TryGetInterface (QualifiedName ("N.M.Additive"), out var additiveInterface));
			Assert.Equal (QualifiedName ("N.M.Additive"), additiveInterface.FullyQualifiedName);
			Assert.Equal (emptyInterface, additiveInterface.AdditiveInterfaces.Single ().GetPossibleInterfaces (model).Single ());
			Assert.Empty (additiveInterface.MethodSets);
		}

		[Fact]
		public void VisitAdditiveInterface4 () {
			var source = @"
namespace N.M {
	interface Empty {}
}
namespace N {
	interface Additive M.Empty
}
";
			var model = Visit (source);
			model.VerifyDiagnosics ();

			Assert.True (model.TryGetInterface (QualifiedName ("N.M.Empty"), out var emptyInterface));
			Assert.True (model.TryGetInterface (QualifiedName ("N.Additive"), out var additiveInterface));
			Assert.Equal (QualifiedName ("N.Additive"), additiveInterface.FullyQualifiedName);
			Assert.Equal (emptyInterface, additiveInterface.AdditiveInterfaces.Single ().GetPossibleInterfaces (model).Single ());
			Assert.Empty (additiveInterface.MethodSets);
		}

		[Fact]
		public void VisitAdditiveInterface5 () {
			var source = @"
namespace N {
	interface Empty {}
}
interface Additive Empty
";
			var model = Visit (source);
			model.VerifyDiagnosics ();

			Assert.True (model.TryGetInterface (QualifiedName ("N.Empty"), out var emptyInterface));
			Assert.True (model.TryGetInterface (QualifiedName ("Additive"), out var additiveInterface));
			Assert.Equal (QualifiedName ("Additive"), additiveInterface.FullyQualifiedName);
			Assert.False (additiveInterface.AdditiveInterfaces.Single ().GetPossibleInterfaces (model).Any ());
			Assert.Empty (additiveInterface.MethodSets);
		}

		[Fact]
		public void VisitAdditiveInterface6 () {
			var source = @"
interface Empty {}

interface Additive Empty + Empty + Additive
";
			var model = Visit (source);
			model.VerifyDiagnosics ();

			Assert.True (model.TryGetInterface (QualifiedName ("Empty"), out var emptyInterface));
			Assert.True (model.TryGetInterface (QualifiedName ("Additive"), out var additiveInterface));
			Assert.Equal (QualifiedName ("Additive"), additiveInterface.FullyQualifiedName);
			Assert.Equal (emptyInterface, additiveInterface.AdditiveInterfaces.ElementAt (0).GetPossibleInterfaces (model).Single ());
			Assert.Equal (emptyInterface, additiveInterface.AdditiveInterfaces.ElementAt (1).GetPossibleInterfaces (model).Single ());
			Assert.Equal (additiveInterface, additiveInterface.AdditiveInterfaces.ElementAt (2).GetPossibleInterfaces (model).Single ());
			Assert.Empty (additiveInterface.MethodSets);
		}

		[Fact]
		public void VisitInterfaceWithOneMethod1 () {
			var source = @"
interface Interface {
	M(param1 : int, param2 : string) : bool;
}
";
			var model = Visit (source);
			model.VerifyDiagnosics ();

			Assert.True (model.TryGetInterface (QualifiedName ("Interface"), out var @interface));
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
		public void VisitInterfaceWithOneMethod2 () {
			var source = @"
interface Interface {
	M(param1 : { M() : bool; M1(param : {}) : { M() : int; } }, param2 : { }) : Interface;
}
";
			var model = Visit (source);
			model.VerifyDiagnosics ();

			Assert.True (model.TryGetInterface (QualifiedName ("Interface"), out var @interface));
			var builder = new InterfaceBuilder {
				FullyQualifiedName = "Interface",
					MethodSets = {
						new[] {
							new InterfaceMethodBuilder {
								Name = "M",
									ReturnType = new TypeKeyBuilder (new InterfaceReferenceBuilder ("Interface") { }),
									Parameters = {
										(
											"param1",
											new TypeKeyBuilder (new InterfaceBuilder {
												MethodSets = {
													new[] {
														new InterfaceMethodBuilder {
															Name = "M",
																ReturnType = new TypeKeyBuilder (Primitive.Bool),
														},
														new InterfaceMethodBuilder {
															Name = "M1",
																ReturnType = new TypeKeyBuilder (new InterfaceBuilder {
																	MethodSets = {
																		new[] {
																			new InterfaceMethodBuilder {
																				Name = "M",
																					ReturnType = new TypeKeyBuilder (Primitive.Int),
																			},
																		},
																	},
																}),
																Parameters = {
																	(
																		"param",
																		new TypeKeyBuilder (new InterfaceBuilder { })
																	),
																},
														},
													},
												},
											})
										),
										(
											"param2",
											new TypeKeyBuilder (new InterfaceBuilder { })
										),
									},
							},
						},
					},
			};
			AssertHelpers.Equal (builder.Build (), @interface);
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

		private TestCompilation Visit (string source, ISemanticModel? model = null) {
			model ??= SemanticModel.Empty;

			using var reader = new StringReader (source);

			var input = new AntlrInputStream (reader);
			var lexer = new FluentLangLexer (input);
			var tokenStream = new CommonTokenStream (lexer);
			var parser = new FluentLangParser (tokenStream);

			// pick up any syntax errors
			var errorStrategy = new ErrorStrategy (ImmutableList<Diagnostic>.Empty);
			parser.ErrorHandler = errorStrategy;
			var compilationUnit = parser.compilation_unit ();
			var dummyListener = new FluentLangParserBaseListener ();
			ParseTreeWalker.Default.Walk (dummyListener, compilationUnit);
			Assert.Empty (errorStrategy.Diagnostics);

			var metadataListener = new MetadataListener (model, ImmutableList<Diagnostic>.Empty);
			ParseTreeWalker.Default.Walk (metadataListener, compilationUnit);
			return new TestCompilation (metadataListener.SemanticModel, metadataListener.Diagnostics);
		}
	}

	public class TestCompilation : ISemanticModel {
		private readonly ISemanticModel _semanticModel;
		private readonly ImmutableList<Diagnostic> _diagnostics;

		public TestCompilation (ISemanticModel semanticModel, ImmutableList<Diagnostic> diagnostics) {
			_semanticModel = semanticModel;
			_diagnostics = diagnostics;
		}

		public bool TryGetInterface (QualifiedName fullyQualifiedName, out Interface i) {
			return _semanticModel.TryGetInterface (fullyQualifiedName, out i);
		}

		public ISemanticModel? TryWith (Interface i) {
			throw new System.NotImplementedException ();
		}

		public void VerifyDiagnosics (params Diagnostic[] diagnostics) {
			Assert.False (diagnostics.Select ((x, i) => {
					var y = _diagnostics[i];
					return x.ErrorCode == y.ErrorCode;
				}).Where (x => !x).Any (),
				$@"expected: 
{string.Concat("\n", diagnostics.Select(x => "new Diagnostic (new Location (), " + x.ErrorCode.ToString() + ")"))}

actual:
{string.Concat("\n", _diagnostics.Select(x => "new Diagnostic (new Location (), " + x.ErrorCode.ToString() + ")"))}
");
		}

		public ISemanticModel With (Interface i) {
			throw new System.NotImplementedException ();
		}
	}
}