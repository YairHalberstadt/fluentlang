using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.flc;
using FluentLang.flc.DependencyLoading;
using FluentLang.flc.ProjectSystem;
using FluentLang.TestUtils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Version = FluentLang.flc.ProjectSystem.Version;

namespace FluentLang.Compiler.Tests.Unit.DependencyLoading
{
	public class ProjectLoaderTests : TestBase
	{
		public ProjectLoaderTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		private class MockDependencyLoader : IDependencyLoader
		{
			private readonly ImmutableArray<IAssembly> _assemblies;

			public MockDependencyLoader(params IAssembly[] assemblies)
			{
				_assemblies = assemblies.ToImmutableArray();
			}

			public ValueTask<ImmutableArray<IAssembly>> LoadDependenciesAsync(ProjectInfo project, AssemblyLoadContext assemblyLoadContext, IEnumerable<IAssembly> alreadyLoadedProjects, CancellationToken cancellationToken = default)
			{
				return new ValueTask<ImmutableArray<IAssembly>>(_assemblies);
			}
		}

		[Fact]
		public async Task CanLoadProjectReferencingFile()
		{
			var projectLoader = new ProjectLoader(
				GetLogger<ProjectLoader>(),
				new MockDependencyLoader(),
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{"sourceFile", "interface I {}"}
				}));
			var assemblyLoadContext = new AssemblyLoadContext(name: null);
			var result = await projectLoader.LoadProjectAsync(
				new ProjectInfo("p", new Version(0, 0), ImmutableArray.Create("sourceFile")),
				assemblyLoadContext,
				ImmutableArray<IAssembly>.Empty);
			result.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.Equal("I", result.Interfaces.SingleOrDefault().FullyQualifiedName!.ToString());
		}

		[Fact]
		public async Task CanLoadProjectReferencingDirectory()
		{
			var projectLoader = new ProjectLoader(
				GetLogger<ProjectLoader>(),
				new MockDependencyLoader(),
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{"dir/sourceFile1.fl", "interface I1 {}"},
					{"dir/sourceFile2.fl", "interface I2 {}"}
				}));
			var assemblyLoadContext = new AssemblyLoadContext(name: null);
			var result = await projectLoader.LoadProjectAsync(
				new ProjectInfo("p", new Version(0, 0), ImmutableArray.Create("dir")),
				assemblyLoadContext,
				ImmutableArray<IAssembly>.Empty);
			result.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.Equal(
				new[] { "I1", "I2" }, 
				result.Interfaces.Select(x => x.FullyQualifiedName!.ToString()));
		}

		[Fact]
		public async Task LoadsFromDirectoryRecursively()
		{
			var projectLoader = new ProjectLoader(
				GetLogger<ProjectLoader>(),
				new MockDependencyLoader(),
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{"dir/sourceFile1.fl", "interface I1 {}"},
					{"dir/subDir/sourceFile2.fl", "interface I2 {}"}
				}));
			var assemblyLoadContext = new AssemblyLoadContext(name: null);
			var result = await projectLoader.LoadProjectAsync(
				new ProjectInfo("p", new Version(0, 0), ImmutableArray.Create("dir")),
				assemblyLoadContext,
				ImmutableArray<IAssembly>.Empty);
			result.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.Equal(
				new[] { "I1", "I2" },
				result.Interfaces.Select(x => x.FullyQualifiedName!.ToString()));
		}

		[Fact]
		public async Task DeduplicatesFileIncludedMultipleTimes()
		{
			var projectLoader = new ProjectLoader(
				GetLogger<ProjectLoader>(),
				new MockDependencyLoader(),
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{"c:/dir/sourceFile.fl", "interface I {}"}
				}));
			var assemblyLoadContext = new AssemblyLoadContext(name: null);
			var result = await projectLoader.LoadProjectAsync(
				new ProjectInfo("p", new Version(0, 0), ImmutableArray.Create(
					"dir/sourceFile.fl",
					"dir/sourceFile.fl",
					"DIR/sourceFile.fl",
					"/dir/sourceFile.fl",
					"dir/sourceFile.fl/",
					"dir\\sourceFile.fl",
					"c:/dir/sourceFile.fl",
					"C:/dir/sourceFile.fl",
					"dir/../dir/sourceFile.fl",
					"dir")),
				assemblyLoadContext,
				ImmutableArray<IAssembly>.Empty);
			result.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.Equal("I", result.Interfaces.SingleOrDefault().FullyQualifiedName!.ToString());
		}

		[Theory]
		[InlineData("sourceFile")]
		[InlineData("directory/sourceFile")]
		[InlineData("../sourceFile")]
		[InlineData("sourceFile/")]
		public async Task ThrowsIfFileOrDirectoryNotFound(string path)
		{
			var projectLoader = new ProjectLoader(
				GetLogger<ProjectLoader>(),
				new MockDependencyLoader(),
				new MockFileSystem());
			var assemblyLoadContext = new AssemblyLoadContext(name: null);
			await Assert.ThrowsAsync<FlcException>(() => projectLoader.LoadProjectAsync(
				new ProjectInfo("p", new Version(0, 0), ImmutableArray.Create(path)),
				assemblyLoadContext,
				ImmutableArray<IAssembly>.Empty).AsTask());
		}

		[Fact]
		public async Task RemovesExcludedFile()
		{
			var projectLoader = new ProjectLoader(
				GetLogger<ProjectLoader>(),
				new MockDependencyLoader(),
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{"dir/sourceFile1.fl", "interface I1 {}"},
					{"dir/sourceFile2.fl", "interface I2 {}"}
				}));
			var assemblyLoadContext = new AssemblyLoadContext(name: null);
			var result = await projectLoader.LoadProjectAsync(
				new ProjectInfo(
					"p",
					new Version(0, 0),
					ImmutableArray.Create("dir"),
					ImmutableArray.Create("dir/sourceFile2.fl")),
				assemblyLoadContext,
				ImmutableArray<IAssembly>.Empty);
			result.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.Equal(
				new[] { "I1" },
				result.Interfaces.Select(x => x.FullyQualifiedName!.ToString()));
		}

		[Fact]
		public async Task RemovesExcludedFolder()
		{
			var projectLoader = new ProjectLoader(
				GetLogger<ProjectLoader>(),
				new MockDependencyLoader(),
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{"dir/sourceFile1.fl", "interface I1 {}"},
					{"dir/subDir/sourceFile2.fl", "interface I2 {}"}
				}));
			var assemblyLoadContext = new AssemblyLoadContext(name: null);
			var result = await projectLoader.LoadProjectAsync(
				new ProjectInfo(
					"p",
					new Version(0, 0),
					ImmutableArray.Create("dir"),
					ImmutableArray.Create("dir/subDir")),
				assemblyLoadContext,
				ImmutableArray<IAssembly>.Empty);
			result.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.Equal(
				new[] { "I1" },
				result.Interfaces.Select(x => x.FullyQualifiedName!.ToString()));
		}

		[Fact]
		public async Task IncludedDirectoryOnlyIncludesFilesEndingWithFl()
		{
			var projectLoader = new ProjectLoader(
				GetLogger<ProjectLoader>(),
				new MockDependencyLoader(),
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{"dir/sourceFile1.fl", "interface I1 {}"},
					{"dir/sourceFile2", "interface I2 {}"}
				}));
			var assemblyLoadContext = new AssemblyLoadContext(name: null);
			var result = await projectLoader.LoadProjectAsync(
				new ProjectInfo(
					"p",
					new Version(0, 0),
					ImmutableArray.Create("dir")),
				assemblyLoadContext,
				ImmutableArray<IAssembly>.Empty);
			result.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.Equal(
				new[] { "I1" },
				result.Interfaces.Select(x => x.FullyQualifiedName!.ToString()));
		}

		[Fact]
		public void ReferencesAssembliesReturnedByDependencyLoader()
		{
			var name = "a";
			var version = new Symbols.Interfaces.Version(1, 2, 3);
			CreateAssembly(
				"",
				name,
				version)
				.VerifyDiagnostics()
				.VerifyEmit(_testOutputHelper, testEmittedAssembly: (original, _) =>
				{
					var projectLoader = new ProjectLoader(
					GetLogger<ProjectLoader>(),
					new MockDependencyLoader(original),
					new MockFileSystem(new Dictionary<string, MockFileData>
					{
						{"sourceFile", "interface I {}"}
					}));
					var assemblyLoadContext = new AssemblyLoadContext(name: null);
					var result = projectLoader.LoadProjectAsync(
						new ProjectInfo("p", new Version(0, 0), ImmutableArray.Create("sourceFile")),
						assemblyLoadContext,
						ImmutableArray<IAssembly>.Empty).Result;
					result.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
					var dependency = result.ReferencedAssembliesAndSelf.First();
					Assert.Equal(original.Name, dependency.Name);
					Assert.Equal(original.Version, dependency.Version);
				});
		}
	}
}
