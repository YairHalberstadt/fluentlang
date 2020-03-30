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
			var buildCommand = new Command("build")
			{
				new Argument<FileInfo>("solution-file").ExistingOnly(),
				new Argument<FileInfo>("output-directory"),
				new Option("--output-csharp")
				{
					Argument = new Argument<bool>(defaultValue: () => false)
				},
				new Option("--verbosity")
				{
					Argument = new Argument<LogLevel>(defaultValue: () => LogLevel.Information)
				},
				new Option("--test")
				{
					Argument = new Argument<bool>(defaultValue: () => true)
				},
			};

			var runCommand = new Command("run")
			{
				new Argument<FileInfo>("solution-file").ExistingOnly(),
				new Argument<string>("project-name"),
				new Option("--verbosity")
				{
					Argument = new Argument<LogLevel>(defaultValue: () => LogLevel.Information)
				},
			};

			var rootCommand = new RootCommand
			{
				buildCommand,
				runCommand,
			};

			buildCommand.Handler = CommandHandler.Create(
				async (FileInfo solutionFile, FileInfo outputDirectory, bool outputCSharp, LogLevel verbosity, bool test) =>
				{
					var builder = new ContainerBuilder();
					builder.RegisterModule(new FlcModule(verbosity));
					using var container = builder.Build();
					var compiler = container.Resolve<FluentLangCompiler>();
					var success = await compiler.Build(
						solutionFile.FullName,
						outputDirectory.FullName,
						outputCSharp,
						test,
						cancellationToken: default);
					return success ? 0 : 1;
				});

			runCommand.Handler = CommandHandler.Create(
				(FileInfo solutionFile, string projectName, LogLevel verbosity) =>
				{
					var builder = new ContainerBuilder();
					builder.RegisterModule(new FlcModule(verbosity));
					using var container = builder.Build();
					var compiler = container.Resolve<FluentLangCompiler>();
					return compiler.Run(solutionFile.FullName, projectName, default).AsTask();
				});

			await rootCommand.InvokeAsync(args);
		}
	}
}
