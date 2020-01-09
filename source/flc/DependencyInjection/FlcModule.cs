using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dependable.Implementations.Autofac;
using FluentLang.Compiler.Compilation;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Emit;
using FluentLang.flc.DependencyLoading;
using FluentLang.flc.ProjectSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.IO.Abstractions;

namespace FluentLang.flc.DependencyInjection
{
	public class FlcModule : Autofac.Module
	{
		private readonly LogLevel _logLevel;

		public FlcModule(LogLevel logLevel)
		{
			_logLevel = logLevel;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule<ScopeFactoryModule>();

			var serviceCollection = new ServiceCollection();
			ConfigureServices(serviceCollection, _logLevel);
			builder.Populate(serviceCollection);

			builder.RegisterType<FluentLangCompiler>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<FileSystem>()
				.As<IFileSystem>()
				.SingleInstance();

			builder.RegisterType<SolutionFactory>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<FileAssemblyLoader>()
				.As<IAssemblyLoader>()
				.InstancePerMatchingLifetimeScope(Tag.Create<SolutionInfo>())
				.WithParameter(
					(p, c) => p.ParameterType == typeof(ImmutableArray<string>),
					(p, c) => c.Resolve<SolutionInfo>().LibDirectories);

			builder.RegisterType<NugetAssemblyLoader>()
				.As<IAssemblyLoader>()
				.InstancePerMatchingLifetimeScope(Tag.Create<SolutionInfo>())
				.WithParameter(
					(p, c) => p.ParameterType == typeof(ImmutableArray<string>),
					(p, c) => c.Resolve<SolutionInfo>().NugetFeeds)
				.WithParameter("globalPackagesFolder", GlobalPackagesFolderProvider.GlobalPackagesFolder);

			builder.RegisterType<DependencyAttributeReader>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<DependencyLoader>()
				.As<IDependencyLoader>()
				.InstancePerMatchingLifetimeScope(Tag.Create<SolutionInfo>());

			builder.RegisterType<ProjectLoader>()
				.As<IProjectLoader>()
				.InstancePerMatchingLifetimeScope(Tag.Create<SolutionInfo>());

			builder.RegisterType<MethodKeyGenerator>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<FluentlangToCSharpEmitter>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<CSharpToAssemblyCompiler>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<AssemblyCompiler>()
				.As<IAssemblyCompiler>()
				.SingleInstance();

			builder.RegisterType<DiagnosticFormatter>()
				.As<IDiagnosticFormatter>()
				.SingleInstance();
		}

		private static void ConfigureServices(IServiceCollection services, LogLevel logLevel)
		{
			services.AddLogging(builder => builder
				.AddConsole()
				.AddFilter(level => level >= logLevel));
		}
	}
}
