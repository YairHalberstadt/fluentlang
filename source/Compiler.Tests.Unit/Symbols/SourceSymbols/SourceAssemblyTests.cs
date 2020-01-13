using FluentLang.Compiler.Compilation;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Source;
using FluentLang.TestUtils;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Version = FluentLang.Compiler.Symbols.Interfaces.Version;

namespace FluentLang.Compiler.Tests.Unit.Symbols
{
	public class SourceAssemblyTests : TestBase
	{
		public SourceAssemblyTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void ContainsTransitiveReferencedAssemblies()
		{
			var assembly1 = new SourceAssembly(
				QualifiedName("1"),
				version: new Version(1, 0, 0),
				ImmutableArray<IAssembly>.Empty,
				ImmutableArray<IDocument>.Empty,
				new MockAssemblyCompiler());
			var assembly2 = new SourceAssembly(
				QualifiedName("2"),
				version: new Version(1, 0, 0),
				ImmutableArray.Create<IAssembly>(assembly1),
				ImmutableArray<IDocument>.Empty,
				new MockAssemblyCompiler());
			var assembly3 = new SourceAssembly(
				QualifiedName("3"),
				version: new Version(1, 0, 0),
				ImmutableArray.Create<IAssembly>(assembly2),
				ImmutableArray<IDocument>.Empty,
				new MockAssemblyCompiler());
			var assembly = (IAssembly)new SourceAssembly(
				QualifiedName("TestAssembly"),
				version: new Version(1, 0, 0),
				ImmutableArray.Create<IAssembly>(assembly3),
				ImmutableArray<IDocument>.Empty,
				new MockAssemblyCompiler());

			Assert.Equal(new[] { assembly1, assembly2, assembly3, assembly }, assembly.ReferencedAssembliesAndSelf);
		}

		[Fact]
		public void DeduplicatesReferencedAssemblies()
		{
			var assembly1a = new SourceAssembly(
				QualifiedName("1"),
				version: new Version(1, 0, 0),
				ImmutableArray<IAssembly>.Empty,
				ImmutableArray<IDocument>.Empty,
				new MockAssemblyCompiler());
			var assembly1b = new SourceAssembly(
				QualifiedName("1"),
				version: new Version(1, 0, 0),
				ImmutableArray<IAssembly>.Empty,
				ImmutableArray<IDocument>.Empty,
				new MockAssemblyCompiler());
			var assembly2 = new SourceAssembly(
				QualifiedName("2"),
				version: new Version(1, 0, 0),
				ImmutableArray.Create<IAssembly>(assembly1a),
				ImmutableArray<IDocument>.Empty,
				new MockAssemblyCompiler());
			var assembly3 = new SourceAssembly(
				QualifiedName("3"),
				version: new Version(1, 0, 0),
				ImmutableArray.Create<IAssembly>(assembly1b),
				ImmutableArray<IDocument>.Empty,
				new MockAssemblyCompiler());
			var assembly = (IAssembly)new SourceAssembly(
				QualifiedName("TestAssembly"),
				version: new Version(1, 0, 0),
				ImmutableArray.Create<IAssembly>(assembly2, assembly3),
				ImmutableArray<IDocument>.Empty,
				new MockAssemblyCompiler());

			Assert.Equal(new[] { assembly1a, assembly2, assembly3, assembly }, assembly.ReferencedAssembliesAndSelf);
		}

		[Fact]
		public void ErrorsIfReferencedAssembliesContainDifferentVersionOfSameAssembly()
		{
			var assembly1a = new SourceAssembly(
				QualifiedName("1"),
				version: new Version(1, 0, 0),
				ImmutableArray<IAssembly>.Empty,
				ImmutableArray<IDocument>.Empty,
				new MockAssemblyCompiler());
			var assembly1b = new SourceAssembly(
				QualifiedName("1"),
				version: new Version(1, 0, 1),
				ImmutableArray<IAssembly>.Empty,
				ImmutableArray<IDocument>.Empty,
				new MockAssemblyCompiler());
			new SourceAssembly(
				QualifiedName("TestAssembly"),
				version: new Version(1, 0, 0),
				ImmutableArray.Create<IAssembly>(assembly1a, assembly1b),
				ImmutableArray<IDocument>.Empty,
				new MockAssemblyCompiler()).VerifyDiagnostics(
					new Diagnostic(new Location(new TextToken(@"")), ErrorCode.MultipleVersionsOfSameAssembly));
		}

		[Fact]
		public void IgnoresDocumentsWithIrrecoverableSyntaxErrors()
		{
			var assembly = CreateAssembly(@"
interface I { M() : () bool; }").VerifyDiagnostics(new Diagnostic(new Location(new TextToken(@"(")), ErrorCode.SyntaxError));

			Assert.Empty(assembly.Interfaces);

			assembly = CreateAssembly(new string[] {
				"interface I1 { M() : () bool; }",
				"interface I2 { M() : bool; }",
			}).VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"(")), ErrorCode.SyntaxError));

			var @interface = Assert.Single(assembly.Interfaces);
			Assert.Equal("I2", @interface.FullyQualifiedName!.ToString());
		}

		[Fact]
		public void ContainsAllInterfacesInAssembly()
		{
			var assembly = CreateAssembly(@"
interface I1 { M() : bool; }
interface I2 { M() : int; }").VerifyDiagnostics().VerifyEmit();

			Assert.Equal(2, assembly.Interfaces.Length);
			Assert.Equal(new[] { "I1", "I2" }, assembly.Interfaces.Select(x => x.FullyQualifiedName!.ToString()).OrderBy(x => x));
		}

		[Fact]
		public void ContainsAllMethodsInAssembly()
		{
			var assembly = CreateAssembly(@"
M1() : int { return 42; }
M2() : int { return 42; }").VerifyDiagnostics().VerifyEmit();

			Assert.Equal(2, assembly.Methods.Length);
			Assert.Equal(new[] { "M1", "M2" }, assembly.Methods.Select(x => x.FullyQualifiedName.ToString()).OrderBy(x => x));
		}

		[Fact]
		public void CanAccessInterfaceInAssemblyByName()
		{
			var assembly = CreateAssembly(@"
interface I1 { M() : bool; }
namespace A.B.C
{
	interface I2 { M() : int; }
	namespace D
	{
		interface I3 { M() : string; }
	}
}").VerifyDiagnostics().VerifyEmit();

			var i1 = AssertGetInterface(assembly, "I1");
			Assert.Equal(i1.FullyQualifiedName, QualifiedName("I1"));

			var i2 = AssertGetInterface(assembly, "A.B.C.I2");
			Assert.Equal(i2.FullyQualifiedName, QualifiedName("A.B.C.I2"));

			var i3 = AssertGetInterface(assembly, "A.B.C.D.I3");
			Assert.Equal(i3.FullyQualifiedName, QualifiedName("A.B.C.D.I3"));
		}

		[Fact]
		public void CanAccessMethodInAssemblyByName()
		{
			var assembly = CreateAssembly(@"
M1() : int {}
namespace A.B.C
{
	M2() : int {}
	namespace D
	{
		M3() : int {}
	}
}");

			var m1 = AssertGetMethod(assembly, "M1");
			Assert.Equal(m1.FullyQualifiedName, QualifiedName("M1"));

			var m2 = AssertGetMethod(assembly, "A.B.C.M2");
			Assert.Equal(m2.FullyQualifiedName, QualifiedName("A.B.C.M2"));

			var m3 = AssertGetMethod(assembly, "A.B.C.D.M3");
			Assert.Equal(m3.FullyQualifiedName, QualifiedName("A.B.C.D.M3"));
		}

		private class MockAssemblyCompiler : IAssemblyCompiler
		{
			public CompilationResult CompileAssembly(IAssembly assembly, Stream outputStream, Stream? csharpOutputStream = null, Stream? pdbStream = null)
			{
				throw new System.NotImplementedException();
			}
		}
	}
}
