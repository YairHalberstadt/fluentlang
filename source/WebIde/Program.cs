using FluentLang.Compiler.Compilation;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Emit;
using FluentLang.Compiler.Symbols;
using FluentLang.WebIde.Backend;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FluentLang.WebIde
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
			builder.Services.AddLogging();

			builder.Services.AddSingletonWhichRequiresAsyncInitialization<IMetadataReferenceProvider, WasmMetadataReferenceProvider>();
			builder.Services.AddSingleton<CSharpToAssemblyCompiler>();
			builder.Services.AddSingleton<FluentlangToCSharpEmitter>();
			builder.Services.AddSingleton<IAssemblyCompiler, AssemblyCompiler>();
			builder.Services.AddSingleton<AssemblyFactory>();
			builder.Services.AddSingleton<IDiagnosticFormatter, DiagnosticFormatter>();
			builder.Services.AddSingleton<EditorEngine>();
			var container = builder.Build();
			foreach (var requiresAsyncLoad in container.Services.GetServices<IRequiresAsyncInitialize>())
			{
				await requiresAsyncLoad.InitializeAsync();
			}
			await container.RunAsync();
		}
	}

	internal static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddSingletonWhichRequiresAsyncInitialization<TService, TImplementation>(this IServiceCollection services)
			 where TService : class where TImplementation : class, IRequiresAsyncInitialize, TService
		{
			services.AddSingleton<TImplementation>();
			services.AddSingleton<IRequiresAsyncInitialize>(x => x.GetRequiredService<TImplementation>());
			services.AddSingleton<TService>(x => x.GetRequiredService<TImplementation>());
			return services;
		}
	}
}
