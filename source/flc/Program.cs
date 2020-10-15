using FluentLang.flc.DependencyInjection;
using Microsoft.Extensions.Logging;
using StrongInject;
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
					await using var container = new FlcContainer(verbosity);
					var success = await container.RunAsync(compiler => compiler.Build(
						solutionFile.FullName,
						outputDirectory.FullName,
						outputCSharp,
						test,
						cancellationToken: default));
					return success ? 0 : 1;
				});

			runCommand.Handler = CommandHandler.Create(
				async (FileInfo solutionFile, string projectName, LogLevel verbosity) =>
				{
					await using var container = new FlcContainer(verbosity);
					await container.RunAsync(compiler => compiler.Run(solutionFile.FullName, projectName, default));
				});

			await rootCommand.InvokeAsync(args);
		}
	}
}
