using FluentLang.flc.ProjectCompiler;
using FluentLang.Runtime;
using Microsoft.Extensions.Logging.Abstractions;
using System;
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
				var loader = new NugetAssemblyLoader(NullLogger<NugetAssemblyLoader>.Instance, packagesFolder);
				var assemblyLoadContext = new AssemblyLoadContext(null, true);
				var assembly = await loader.LoadDependency(
					assemblyLoadContext,
					"FluentLang.Runtime",
					"1.0.0-CI-20191218-193719");
				Assert.NotNull(assembly.GetType(typeof(FLObject).FullName!));
				assemblyLoadContext.Unload();
			}
			finally
			{
				Directory.Delete(packagesFolder, recursive: true);
			}
		}
	}
}
