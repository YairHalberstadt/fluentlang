using FluentLang.flc.ProjectSystem;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace FluentLang.flc
{
	public class FluentLangCompiler
	{
		private readonly ILogger<FluentLangCompiler> _logger;
		private readonly SolutionFactory _solutionFactory;

		public FluentLangCompiler(ILogger<FluentLangCompiler> logger, SolutionFactory solutionFactory)
		{
			_logger = logger;
			_solutionFactory = solutionFactory;
		}

		public async ValueTask Build(string filePath)
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
		}
	}
}
