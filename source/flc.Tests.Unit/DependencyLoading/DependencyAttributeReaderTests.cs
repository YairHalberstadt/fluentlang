using FluentLang.Compiler.Compilation;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Source;
using FluentLang.flc.DependencyLoading;
using FluentLang.TestUtils;
using System.Collections.Immutable;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.DependencyLoading
{
	public class DependencyAttributeReaderTests : TestBase
	{
		public DependencyAttributeReaderTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void CanReadDependencies()
		{
			var dep1Name = "a1";
			var dep2Name = "a2";
			var dep1Version = new Version(1, 2, 3);
			var dep2Version = new Version(4, 5, 6, "alpha1");
			var dep1 = new SourceAssembly(
				QualifiedName(dep1Name),
				dep1Version,
				ImmutableArray<IAssembly>.Empty,
				ImmutableArray<IDocument>.Empty,
				_assemblyCompiler);
			var dep2 = new SourceAssembly(
				QualifiedName(dep2Name),
				dep2Version,
				ImmutableArray<IAssembly>.Empty,
				ImmutableArray<IDocument>.Empty,
				_assemblyCompiler);
			var assembly = new SourceAssembly(
				QualifiedName("a"),
				new Version(0, 0, 0),
				ImmutableArray.Create<IAssembly>(dep1, dep2),
				ImmutableArray<IDocument>.Empty,
				_assemblyCompiler);
			assembly.VerifyDiagnostics().VerifyEmit(
                testEmittedAssembly: (_, a, _1) =>
                    Assert.Equal(new[]
                    {
                        new Dependency(dep1Name, dep1Version.ToString()),
                        new Dependency(dep2Name, dep2Version.ToString())
                    },
                        new DependencyAttributeReader().ReadDependencies(a)));
		}
	}
}