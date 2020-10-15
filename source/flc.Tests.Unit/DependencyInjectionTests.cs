using FluentLang.flc.DependencyInjection;
using FluentLang.flc.DependencyLoading;
using FluentLang.flc.ProjectSystem;
using Microsoft.Extensions.Logging;
using StrongInject;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit
{
	public partial class DependencyInjectionTests
	{
		[Fact]
		public async Task CanResolveCompiler()
		{
			await using var container = new FlcContainer(LogLevel.Information);
			await container.RunAsync(compiler => Assert.NotNull(compiler));
		}

		[RegisterModule(typeof(FlcModule))]
		public partial class ProjectLoaderContainer : IContainer<Func<SolutionInfo, IProjectLoader>>
		{
			[Instance] private readonly LogLevel _logLevel;

			public ProjectLoaderContainer(LogLevel logLevel)
			{
				_logLevel = logLevel;
			}
		}

		[Fact]
		public void CanResolveProjectLoader()
		{
			using var container = new ProjectLoaderContainer(LogLevel.Information);
			container.Run(func => Assert.NotNull(func(new SolutionInfo(default, default, default))));
		}
	}
}
