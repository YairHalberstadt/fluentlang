using Dependable.Abstractions;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.flc.DependencyLoading;
using FluentLang.flc.ProjectSystem;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace FluentLang.flc
{
	public class FluentLangCompiler
	{
		private readonly ILogger<FluentLangCompiler> _logger;
		private readonly SolutionFactory _solutionFactory;
		private readonly IScopeFactory<SolutionInfo, IProjectLoader> _projectLoader;

		public FluentLangCompiler(
			ILogger<FluentLangCompiler> logger,
			SolutionFactory solutionFactory,
			IScopeFactory<SolutionInfo, IProjectLoader> projectLoader)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_solutionFactory = solutionFactory ?? throw new ArgumentNullException(nameof(solutionFactory));
			_projectLoader = projectLoader ?? throw new ArgumentNullException(nameof(projectLoader));
		}

		public async ValueTask Build(string filePath, CancellationToken cancellationToken = default)
		{
			_logger.LogDebug("Loading solution from {0}", filePath);
			var solution = await _solutionFactory.ParseFromFileAsync(filePath).ConfigureAwait(false);
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
				var projects = await buildOrder.ToAsyncEnumerable().AggregateAwaitAsync(
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
			finally
			{
				assemblyLoadContext.Unload();
			}
		}
	}
}
