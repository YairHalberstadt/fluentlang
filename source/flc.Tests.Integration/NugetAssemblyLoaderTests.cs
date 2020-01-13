using FluentLang.flc.DependencyLoading;
using FluentLang.Runtime;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Xunit;

namespace FluentLang.Compiler.Tests.Integration
{
	public class NugetAssemblyLoaderTests
	{
		[Fact]
		public async Task CanDownloadNugetPackageFromNuget()
		{
			var packagesFolder = Path.GetTempPath() + "/" + Guid.NewGuid();
			Directory.CreateDirectory(packagesFolder);
			try
			{
				var loader = new NugetAssemblyLoader(NullLogger<NugetAssemblyLoader>.Instance, packagesFolder, default);
				var assemblyLoadContext = new AssemblyLoadContext(null, true);
				var assembly = await loader.TryLoadAssemblyAsync(
					assemblyLoadContext,
					new Dependency("FluentLang.Runtime", "1.0.0-CI-20191218-193719"));
				Assert.NotNull(assembly?.Assembly.GetType(typeof(FLObject).FullName!));
				assemblyLoadContext.Unload();
			}
			finally
			{
				Directory.Delete(packagesFolder, recursive: true);
			}
		}

		[Fact]
		public async Task ReturnNullForNonExistentPackage()
		{
			var packagesFolder = Path.GetTempPath() + "/" + Guid.NewGuid();
			Directory.CreateDirectory(packagesFolder);
			try
			{
				var loader = new NugetAssemblyLoader(NullLogger<NugetAssemblyLoader>.Instance, packagesFolder, default);
				var assemblyLoadContext = new AssemblyLoadContext(null, true);
				var assembly = await loader.TryLoadAssemblyAsync(
					assemblyLoadContext,
					new Dependency("FluentLang.abcdefg", "1.0.0-CI-20191218-193719"));
				Assert.Null(assembly);
				assemblyLoadContext.Unload();
			}
			finally
			{
				Directory.Delete(packagesFolder, recursive: true);
			}
		}

		[Fact]
		public async Task ReturnNullForNonExistentVersion()
		{
			var packagesFolder = Path.GetTempPath() + "/" + Guid.NewGuid();
			Directory.CreateDirectory(packagesFolder);
			try
			{
				var loader = new NugetAssemblyLoader(NullLogger<NugetAssemblyLoader>.Instance, packagesFolder, default);
				var assemblyLoadContext = new AssemblyLoadContext(null, true);
				var assembly = await loader.TryLoadAssemblyAsync(
					assemblyLoadContext,
					new Dependency("FluentLang.Runtime", "0.0.0"));
				Assert.Null(assembly);
				assemblyLoadContext.Unload();
			}
			finally
			{
				Directory.Delete(packagesFolder, recursive: true);
			}
		}

		[Fact]
		public async Task ReturnNullForNonExistentFeed()
		{
			var packagesFolder = Path.GetTempPath() + "/" + Guid.NewGuid();
			Directory.CreateDirectory(packagesFolder);
			try
			{
				var loader = new NugetAssemblyLoader(
					NullLogger<NugetAssemblyLoader>.Instance,
					packagesFolder,
					ImmutableArray.Create("notAFeed"));
				var assemblyLoadContext = new AssemblyLoadContext(null, true);
				var assembly = await loader.TryLoadAssemblyAsync(
					assemblyLoadContext,
					new Dependency("FluentLang.Runtime", "1.0.0-CI-20191218-193719"));
				Assert.Null(assembly);
				assemblyLoadContext.Unload();
			}
			finally
			{
				Directory.Delete(packagesFolder, recursive: true);
			}
		}

		[Fact]
		public async Task IfNotFoundInFirstFeedLooksInSecond()
		{
			var packagesFolder = Path.GetTempPath() + "/" + Guid.NewGuid();
			Directory.CreateDirectory(packagesFolder);
			try
			{
				var loader = new NugetAssemblyLoader(
					NullLogger<NugetAssemblyLoader>.Instance, 
					packagesFolder,
					ImmutableArray.Create("notAFeed", "https://api.nuget.org/v3/index.json"));
				var assemblyLoadContext = new AssemblyLoadContext(null, true);
				var assembly = await loader.TryLoadAssemblyAsync(
					assemblyLoadContext,
					new Dependency("FluentLang.Runtime", "1.0.0-CI-20191218-193719"));
				Assert.NotNull(assembly?.Assembly.GetType(typeof(FLObject).FullName!));
				assemblyLoadContext.Unload();
			}
			finally
			{
				Directory.Delete(packagesFolder, recursive: true);
			}
		}
	}
}
