using Autofac;
using FluentLang.flc.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace FluentLang.flc
{
	internal class Program
	{
		private static async Task Main(string[] args)
		{
			var buildCommand = new Command("Build")
			{
				new Argument<FileInfo>("solution-file").ExistingOnly(),
				new Option<LogLevel>("--verbosity")
				{
					Argument = new Argument<LogLevel>(defaultValue: () => LogLevel.Information)
				}
			};

			var rootCommand = new RootCommand
			{
				buildCommand,
			};

			buildCommand.Handler = CommandHandler.Create(
				(FileInfo solutionFile, LogLevel verbosity) =>
				{
					var builder = new ContainerBuilder();
					builder.RegisterModule(new FlcModule(verbosity));
					using var container = builder.Build();
					var compiler = container.Resolve<FluentLangCompiler>();
					return compiler.Build(solutionFile.FullName).AsTask();
				});

			await rootCommand.InvokeAsync(args);
		}
	}
}
