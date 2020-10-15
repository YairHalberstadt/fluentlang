using FluentLang.Compiler.Compilation;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Emit;
using FluentLang.Compiler.Symbols;
using FluentLang.flc.DependencyLoading;
using FluentLang.flc.ProjectSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StrongInject;
using StrongInject.Modules;
using System;
using System.IO.Abstractions;

namespace FluentLang.flc.DependencyInjection
{
	[Register(typeof(ServiceCollection), Scope.SingleInstance, typeof(IServiceCollection))]
	[Register(typeof(FluentLangCompiler), Scope.SingleInstance)]
	[Register(typeof(FileSystem), Scope.SingleInstance, typeof(IFileSystem))]
	[Register(typeof(SolutionFactory), Scope.SingleInstance)]
	[Register(typeof(DependencyAttributeReader), Scope.SingleInstance)]
	[Register(typeof(DependencyLoader), typeof(IDependencyLoader))]
	[Register(typeof(ProjectLoader), typeof(IProjectLoader))]
	[Register(typeof(FluentlangToCSharpEmitter), Scope.SingleInstance)]
	[Register(typeof(CSharpToAssemblyCompiler), Scope.SingleInstance)]
	[Register(typeof(AssemblyCompiler), Scope.SingleInstance, typeof(IAssemblyCompiler))]
	[Register(typeof(DiagnosticFormatter), Scope.SingleInstance, typeof(IDiagnosticFormatter))]
	[Register(typeof(AssemblyFactory), Scope.SingleInstance)]
	[RegisterModule(typeof(CollectionsModule))]
	public class FlcModule
	{
		[DecoratorFactory]
		public static IServiceCollection ConfigureServices(IServiceCollection services, LogLevel logLevel)
		{
			services.AddLogging(builder => builder
				.AddConsole()
				.AddFilter(level => level >= logLevel));

			return services;
		}

		[Factory(Scope.SingleInstance)]
		public static IServiceProvider GetServiceProvider(IServiceCollection services) => services.BuildServiceProvider();

		[Factory(Scope.SingleInstance)]
		public static ILogger<T> GetLogger<T>(IServiceProvider services) => services.GetRequiredService<ILogger<T>>();

		[Instance]
		public static IMetadataReferenceProvider MetadataReferenceProvider => LocalSystemDllsMetadataReferenceProvider.Instance;

		[Factory]
		public static IAssemblyLoader FileAssemblyLoader(ILogger<FileAssemblyLoader> logger, SolutionInfo solutionInfo)
		{
			return new FileAssemblyLoader(logger, solutionInfo.LibDirectories);
		}

		[Factory]
		public static IAssemblyLoader NugetAssemblyLoader(ILogger<NugetAssemblyLoader> logger, SolutionInfo solutionInfo)
		{
			return new NugetAssemblyLoader(logger, GlobalPackagesFolderProvider.GlobalPackagesFolder, solutionInfo.LibDirectories);
		}
	}
}
