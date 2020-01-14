using Dependable.Abstractions;
using FluentLang.Compiler.Compilation;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.flc.DependencyLoading;
using FluentLang.flc.ProjectSystem;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using ProjectInfo = FluentLang.flc.ProjectSystem.ProjectInfo;
using SolutionInfo = FluentLang.flc.ProjectSystem.SolutionInfo;

namespace FluentLang.flc
{
	public class FluentLangCompiler
	{
		private readonly ILogger<FluentLangCompiler> _logger;
		private readonly SolutionFactory _solutionFactory;
		private readonly IScopeFactory<SolutionInfo, IProjectLoader> _projectLoader;
		private readonly IFileSystem _fileSystem;
		private readonly IDiagnosticFormatter _diagnosticFormatter;

		public FluentLangCompiler(
			ILogger<FluentLangCompiler> logger,
			SolutionFactory solutionFactory,
			IScopeFactory<SolutionInfo, IProjectLoader> projectLoader,
			IFileSystem fileSystem,
			IDiagnosticFormatter diagnosticFormatter)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_solutionFactory = solutionFactory ?? throw new ArgumentNullException(nameof(solutionFactory));
			_projectLoader = projectLoader ?? throw new ArgumentNullException(nameof(projectLoader));
			_fileSystem = fileSystem;
			_diagnosticFormatter = diagnosticFormatter;
		}

		public async ValueTask Build(string solutionFilePath, string outputDirectory, bool outputCSharp, CancellationToken cancellationToken = default)
		{
			_logger.LogDebug("Loading solution from {0}", solutionFilePath);
			var solution = await _solutionFactory.ParseFromFileAsync(solutionFilePath).ConfigureAwait(false);
			_logger.LogDebug("Found solution");

			_logger.LogDebug("Creating build order");
			var buildOrder = ProjectDependencyOrganizer.CreateBuildOrder(solution.Projects);
			if (_logger.IsEnabled(LogLevel.Debug))
			{
				_logger.LogDebug($"Build order is {string.Join(",", buildOrder.Select(x => x.Name))}");
			}

			_logger.LogDebug("Loading dependencies");
			var assemblyLoadContext = new AssemblyLoadContext(name: null, isCollectible: true);
			try
			{
				var projects = await LoadProjects(
					solution,
					buildOrder,
					assemblyLoadContext,
					cancellationToken);

				foreach (var project in projects)
				{
					if (!await TryCompileProject(
						outputDirectory,
						outputCSharp,
						assemblyLoadContext,
						project,
						cancellationToken))
					{
						return;
					}
				}
			}
			finally
			{
				assemblyLoadContext.Unload();
			}
		}

		private async ValueTask<bool> TryCompileProject(
			string outputDirectory,
			bool outputCSharp,
			AssemblyLoadContext assemblyLoadContext,
			IAssembly project,
			CancellationToken cancellationToken)
		{
			var compilationResult = project.CompileAssembly(
				out var assemblyBytes,
				out var csharpBytes,
				out _);

			if (compilationResult.Status == CompilationResultStatus.CodeErrors)
			{
				foreach (var error in compilationResult.AssemblyDiagnostics)
				{
					_logger.LogInformation($"ERROR! Location: {_diagnosticFormatter.CreateLocationMessage(error)}, Message: {_diagnosticFormatter.CreateDiagnosticMessage(error)}");
				}
				return false;
			}
			if (compilationResult.Status == CompilationResultStatus.InternalErrors)
			{
				throw new FlcException($@"An internal error occured whilst compiling {project.Name}.
Please report this to the fluentlang maintainers.
The following diagnostics were reported when compiling the emitted C# to a dll:
{string.Join('\n', compilationResult.RoslynDiagnostics.Select(x => x.ToString()))}");
			}

			assemblyLoadContext.LoadFromStream(assemblyBytes.ToStream());

			await WriteOutput(
				outputDirectory,
				outputCSharp,
				project,
				csharpBytes,
				cancellationToken);

			return true;
		}

		private async ValueTask WriteOutput(
			string outputDirectory,
			bool outputCSharp,
			IAssembly project,
			ImmutableArray<byte> csharpBytes,
			CancellationToken cancellationToken)
		{
			try
			{
				foreach (var assembly in project.ReferencedAssembliesAndSelf)
				{
					if (!assembly.TryGetAssemblyBytes(out var bytes))
						throw new InvalidOperationException(
							$"Cannot get bytes for {assembly.Name}. Unable to write output.");
					var outputPath = GetOutputPath(assembly);
					await WriteBytesToFile(bytes, outputPath, cancellationToken);
				}

				_logger.LogInformation("{0} --> {1}", project.Name, GetOutputPath(project));
				if (outputCSharp)
				{
					var csharpOutputPath = _fileSystem.Path.Combine(outputDirectory, project.Name + ".cs");
					await WriteBytesToFile(csharpBytes, csharpOutputPath, cancellationToken);
					_logger.LogInformation("{0} --> {1}", project.Name, csharpOutputPath);
				}

				_logger.LogInformation("Compilation for {0} suceeded.", project.Name);
			}
			catch (IOException exception)
			{
				throw new FlcException(
					$"Error whilst writing assembly for {project.Name} to disk",
					exception);
			}

			string GetOutputPath(IAssembly assembly)
			{
				return _fileSystem.Path.Combine(outputDirectory, assembly.Name + ".dll");
			}
		}

		private Task WriteBytesToFile(ImmutableArray<byte> bytes, string filePath, CancellationToken cancellationToken)
		{
			var fileInfo = _fileSystem.FileInfo.FromFileName(filePath);
			fileInfo.Directory.Create();
			return _fileSystem.File.WriteAllBytesAsync(
				filePath,
				bytes.UnsafeAsArray(),
				cancellationToken);
		}

		private async Task<List<IAssembly>> LoadProjects(SolutionInfo solution, IEnumerable<ProjectInfo> buildOrder, AssemblyLoadContext assemblyLoadContext, CancellationToken cancellationToken)
		{
			return await buildOrder.ToAsyncEnumerable().AggregateAwaitAsync(
				new List<IAssembly>(),
				async (projectAssemblies, projectInfo) =>
				{
					using var scope = _projectLoader.CreateScope(solution);

					var project = await scope.Value.LoadProjectAsync(
							projectInfo,
							assemblyLoadContext,
							projectAssemblies,
							cancellationToken);
					projectAssemblies.Add(project);
					return projectAssemblies;
				});
		}
	}
}
