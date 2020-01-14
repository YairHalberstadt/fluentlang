using FluentLang.flc.DependencyLoading;
using FluentLang.Runtime;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Immutable;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Xunit;

namespace FluentLang.Compiler.Tests.Integration
{
	public class FileAssemblyLoaderTests
	{
		[Fact]
		public async Task CanLoadAssemblyFromFile()
		{
			var loader = new FileAssemblyLoader(NullLogger<FileAssemblyLoader>.Instance, ImmutableArray.Create("Data"));
			var assemblyLoadContext = new AssemblyLoadContext(null, true);
			var assembly = await loader.TryLoadAssemblyAsync(
				assemblyLoadContext,
				new Dependency("FluentLang.Runtime", "1.0.0.0"));
			Assert.NotNull(assembly?.Assembly.GetType(typeof(FLObject).FullName!));
			assemblyLoadContext.Unload();
		}

		[Fact]
		public async Task ReturnsNullWhenNameNotFound()
		{
			var loader = new FileAssemblyLoader(NullLogger<FileAssemblyLoader>.Instance, ImmutableArray.Create("Data"));
			var assemblyLoadContext = new AssemblyLoadContext(null, true);
			var assembly = await loader.TryLoadAssemblyAsync(
				assemblyLoadContext,
				new Dependency("FluentLang.abcdefg", "1.0.0.0"));
			Assert.Null(assembly);
			assemblyLoadContext.Unload();
		}

		[Fact]
		public async Task ReturnsNullWhenVersionNotFound()
		{
			var loader = new FileAssemblyLoader(NullLogger<FileAssemblyLoader>.Instance, ImmutableArray.Create("data"));
			var assemblyLoadContext = new AssemblyLoadContext(null, true);
			var assembly = await loader.TryLoadAssemblyAsync(
				assemblyLoadContext,
				new Dependency("FluentLang.Runtime", "1.2.3.4"));
			Assert.Null(assembly);
			assemblyLoadContext.Unload();
		}

		[Fact]
		public async Task IfNotFoundInFirstPathLooksInSecond()
		{
			var loader = new FileAssemblyLoader(NullLogger<FileAssemblyLoader>.Instance, ImmutableArray.Create("notADirectory", "data"));
			var assemblyLoadContext = new AssemblyLoadContext(null, true);
			var assembly = await loader.TryLoadAssemblyAsync(
				assemblyLoadContext,
				new Dependency("FluentLang.Runtime", "1.0.0.0"));
			Assert.NotNull(assembly?.Assembly.GetType(typeof(FLObject).FullName!));
			assemblyLoadContext.Unload();
		}
	}
}
