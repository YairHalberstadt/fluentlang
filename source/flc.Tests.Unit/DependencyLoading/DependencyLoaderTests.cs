using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.flc;
using FluentLang.flc.DependencyLoading;
using FluentLang.flc.ProjectSystem;
using FluentLang.TestUtils;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Version = FluentLang.Compiler.Symbols.Interfaces.Version;

namespace FluentLang.Compiler.Tests.Unit.DependencyLoading
{
	public class DependencyLoaderTests : TestBase
	{
		public DependencyLoaderTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void ReturnsDirectDependenciesOfProject()
		{
			var name = "a";
			var version = new Version(1, 2, 3);
			CreateAssembly(
				"",
				name,
				version)
				.VerifyDiagnostics()
				.VerifyEmit(testEmittedAssembly: (original, assembly, assemblyBytes) =>
			  {
				  var assemblyLoadContext = new AssemblyLoadContext(null, true);
				  var assemblyLoader = new MockAssemblyLoader(
					  new AssemblyLoadResult(assembly, assemblyBytes));
				  var dependencyLoader = new DependencyLoader(
					  ImmutableArray.Create<IAssemblyLoader>(assemblyLoader),
					  new DependencyAttributeReader(),
					  _assemblyFactory,
					  GetLogger<DependencyLoader>());
				  var dependencies = dependencyLoader.LoadDependenciesAsync(
					  new ProjectInfo(
						  "p",
						  new flc.ProjectSystem.Version(0, 0),
						  ImmutableArray.Create(""),
						  references: ImmutableArray.Create(new Reference(
							  Reference.ReferenceType.Assembly,
							  name,
							  version.ToString()))),
					  assemblyLoadContext,
					  Array.Empty<IAssembly>()).Result;
				  var dependency = dependencies.Single();
				  Assert.Equal(original.Name, dependency.Name);
				  Assert.Equal(original.Version, dependency.Version);
			  });
		}

		[Fact]
		public void TakesFromSecondAssemblyLoaderIfNotInFirst()
		{
			var name = "a";
			var version = new Version(1, 2, 3);
			CreateAssembly(
				"",
				name,
				version)
				.VerifyDiagnostics()
				.VerifyEmit(testEmittedAssembly: (original, assembly, assemblyBytes) =>
				{
					var assemblyLoadContext = new AssemblyLoadContext(null, true);
					var assemblyLoader1 = new MockAssemblyLoader();
					var assemblyLoader2 = new MockAssemblyLoader(
						new AssemblyLoadResult(assembly, assemblyBytes));
					var dependencyLoader = new DependencyLoader(
						ImmutableArray.Create<IAssemblyLoader>(assemblyLoader1, assemblyLoader2),
						new DependencyAttributeReader(),
						_assemblyFactory,
						GetLogger<DependencyLoader>());
					var dependencies = dependencyLoader.LoadDependenciesAsync(
						new ProjectInfo(
							"p",
							new flc.ProjectSystem.Version(0, 0),
							ImmutableArray.Create(""),
							references: ImmutableArray.Create(new Reference(
								Reference.ReferenceType.Assembly,
								name,
								version.ToString()))),
						assemblyLoadContext,
						Array.Empty<IAssembly>()).Result;
					var dependency = dependencies.Single();
					Assert.Equal(original.Name, dependency.Name);
					Assert.Equal(original.Version, dependency.Version);
				});
		}

		[Fact]
		public void ThrowsIfNotInAnyAssemblyLoader()
		{
			var name = "a";
			var version = new Version(1, 2, 3);
			CreateAssembly(
				"",
				name,
				version)
				.VerifyDiagnostics()
				.VerifyEmit(testEmittedAssembly: (original, assembly, assemblyBytes) =>
				{
					var assemblyLoadContext = new AssemblyLoadContext(null, true);
					var assemblyLoader = new MockAssemblyLoader();
					var dependencyLoader = new DependencyLoader(
						ImmutableArray.Create<IAssemblyLoader>(assemblyLoader),
						new DependencyAttributeReader(),
						_assemblyFactory,
						GetLogger<DependencyLoader>());
					Assert.Throws<FlcException>(() => dependencyLoader.LoadDependenciesAsync(
						new ProjectInfo(
							"p",
							new flc.ProjectSystem.Version(0, 0),
							ImmutableArray.Create(""),
							references: ImmutableArray.Create(new Reference(
								Reference.ReferenceType.Assembly,
								name,
								version.ToString()))),
						assemblyLoadContext,
						Array.Empty<IAssembly>()).Result);
				});
		}

		[Fact]
		public void CanLoadDependeciesWithSubdependencies()
		{
			CreateAssembly(
				"",
				"sub",
				new Version(1, 2, 3))
				.VerifyDiagnostics()
				.VerifyEmit(testEmittedAssembly: (_, subDependency, subDependencyBytes) =>
				{
					var name = "a";
					var version = new Version(1, 2, 3);
					CreateAssembly(
						"",
						name,
						version)
						.VerifyDiagnostics()
						.VerifyEmit(testEmittedAssembly: (original, assembly, assemblyBytes) =>
						{
							var assemblyLoadContext = new AssemblyLoadContext(null, true);
							var assemblyLoader = new MockAssemblyLoader(
								new AssemblyLoadResult(subDependency, subDependencyBytes),
								new AssemblyLoadResult(assembly, assemblyBytes));
							var dependencyLoader = new DependencyLoader(
								ImmutableArray.Create<IAssemblyLoader>(assemblyLoader),
								new DependencyAttributeReader(),
								_assemblyFactory,
								GetLogger<DependencyLoader>());
							var dependencies = dependencyLoader.LoadDependenciesAsync(
								new ProjectInfo(
									"p",
									new flc.ProjectSystem.Version(0, 0),
									ImmutableArray.Create(""),
									references: ImmutableArray.Create(new Reference(
										Reference.ReferenceType.Assembly,
										name,
										version.ToString()))),
								assemblyLoadContext,
								Array.Empty<IAssembly>()).Result;
							var dependency = dependencies.Single();
							Assert.Equal(original.Name, dependency.Name);
							Assert.Equal(original.Version, dependency.Version);
						});
				});
		}

		[Fact]
		public void ThrowsIfSubDependencyNotInAnyAssemblyLoader()
		{
			var subDependency = CreateAssembly("");
			var name = "a";
			var version = new Version(1, 2, 3);
			CreateAssembly(
				"",
				name,
				version,
				subDependency)
				.VerifyDiagnostics()
				.VerifyEmit(testEmittedAssembly: (original, assembly, assemblyBytes) =>
				{
					var assemblyLoadContext = new AssemblyLoadContext(null, true);
					var assemblyLoader = new MockAssemblyLoader(
						new AssemblyLoadResult(assembly, assemblyBytes));
					var dependencyLoader = new DependencyLoader(
						ImmutableArray.Create<IAssemblyLoader>(assemblyLoader),
						new DependencyAttributeReader(),
						_assemblyFactory,
						GetLogger<DependencyLoader>());
					Assert.Throws<FlcException>(() => dependencyLoader.LoadDependenciesAsync(
						new ProjectInfo(
							"p",
							new flc.ProjectSystem.Version(0, 0),
							ImmutableArray.Create(""),
							references: ImmutableArray.Create(new Reference(
								Reference.ReferenceType.Assembly,
								name,
								version.ToString()))),
						assemblyLoadContext,
						Array.Empty<IAssembly>()).Result);
				});
		}

		public class MockAssemblyLoader : IAssemblyLoader
		{
			private readonly ImmutableArray<AssemblyLoadResult> _assemblies;

			public MockAssemblyLoader(params AssemblyLoadResult[] assemblies)
			{
				_assemblies = assemblies.ToImmutableArray();
			}
			public ValueTask<AssemblyLoadResult?> TryLoadAssemblyAsync(AssemblyLoadContext assemblyLoadContext, Dependency dependency, CancellationToken cancellationToken = default)
			{
				return new ValueTask<AssemblyLoadResult?>(_assemblies.FirstOrDefault(
					x => x.Assembly.GetName().Name == $"{dependency.Name}${dependency.Version}"));
			}
		}

		[Fact]
		public void LoadsAssemblyAtMostOnceEvenWhenCalledInParallel()
		{
			var name = "a";
			var version = new Version(1, 2, 3);
			CreateAssembly(
				"",
				name,
				version)
				.VerifyDiagnostics()
				.VerifyEmit(testEmittedAssembly: (original, assembly, assemblyBytes) =>
				{
					var assemblyLoadContext = new AssemblyLoadContext(null, true);
					var assemblyLoader = new MockAssemblyLoader(
						new AssemblyLoadResult(assembly, assemblyBytes));
					var dependencyLoader = new DependencyLoader(
						ImmutableArray.Create<IAssemblyLoader>(assemblyLoader),
						new DependencyAttributeReader(),
						_assemblyFactory,
						GetLogger<DependencyLoader>());
					var projectInfo = new ProjectInfo(
							"p",
							new flc.ProjectSystem.Version(0, 0),
							ImmutableArray.Create(""),
							references: ImmutableArray.Create(new Reference(
								Reference.ReferenceType.Assembly,
								name,
								version.ToString())));
					_ = dependencyLoader.LoadDependenciesAsync(
						projectInfo,
						assemblyLoadContext,
						Array.Empty<IAssembly>());
					var dependencies = dependencyLoader.LoadDependenciesAsync(
						projectInfo,
						assemblyLoadContext,
						Array.Empty<IAssembly>()).Result;
					var dependency = dependencies.Single();
					Assert.Equal(original.Name, dependency.Name);
					Assert.Equal(original.Version, dependency.Version);
				});
		}

		public class OnlyOneEntryAssemblyLoader : IAssemblyLoader
		{
			private readonly ImmutableArray<AssemblyLoadResult> _assemblies;

			public OnlyOneEntryAssemblyLoader(params AssemblyLoadResult[] assemblies)
			{
				_assemblies = assemblies.ToImmutableArray();
			}
			private int _enterCount;
			public async ValueTask<AssemblyLoadResult?> TryLoadAssemblyAsync(AssemblyLoadContext assemblyLoadContext, Dependency dependency, CancellationToken cancellationToken = default)
			{
				Interlocked.Increment(ref _enterCount);
				if (_enterCount > 1)
					throw new Exception("AssemblyLoader called more than once");

				await Task.Delay(100);
				return _assemblies.FirstOrDefault(
					x => x.Assembly.GetName().Name == $"{dependency.Name}${dependency.Version}");
			}
		}
	}
}
