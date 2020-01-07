using Autofac;
using Dependable.Abstractions;
using FluentLang.flc;
using FluentLang.flc.DependencyInjection;
using FluentLang.flc.DependencyLoading;
using FluentLang.flc.ProjectSystem;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit
{
	public class DependencyInjectionTests
	{
		[Fact]
		public void CanResolveCompiler()
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule(new FlcModule(LogLevel.Information));
			using var container = builder.Build();
			var compiler = container.Resolve<FluentLangCompiler>();
			Assert.NotNull(compiler);
		}

		[Fact]
		public void CanResolveProjectLoader()
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule(new FlcModule(LogLevel.Information));
			using var container = builder.Build();
			var projectLoaderFactory = container.Resolve<IScopeFactory<SolutionInfo, IProjectLoader>>();
			var solutionInfo = new SolutionInfo(default, default, default);
			Assert.NotNull(projectLoaderFactory.CreateScope(solutionInfo).Value);
		}
	}
}
