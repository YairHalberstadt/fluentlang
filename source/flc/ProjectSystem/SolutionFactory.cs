using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentLang.flc.ProjectSystem
{
	public class SolutionFactory
	{
		private readonly ILogger _logger;
		private readonly IFileSystem _fileSystem;

		public SolutionFactory(ILogger<SolutionFactory> logger, IFileSystem fileSystem)
		{
			_logger = logger;
			_fileSystem = fileSystem;
		}

		public SolutionInfo Parse(string text)
		{
			_logger.LogDebug("Found Solution File:\n{0}", text);
			SolutionInfo solution;
			try
			{
				solution = JsonConvert.DeserializeObject<SolutionInfo>(text);
			}
			catch (Exception e)
			{
				throw new FlcException("Invalid Solution File", e);
			}
			ValidateSolution(solution);
			return solution;
		}

		public async ValueTask<SolutionInfo> ParseFromFileAsync(
			string filePath,
			CancellationToken cancellationToken = default)
		{
			return Parse(await _fileSystem.File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false));
		}

		private void ValidateSolution(SolutionInfo solutionInfo)
		{
			var projectNames = solutionInfo.Projects.Select(x => x.Name).ToHashSet();

			foreach(var project in solutionInfo.Projects)
			{
				var references = 
					project
					.References
					.Where(x => x.Type == Reference.ReferenceType.Project)
					.Select(x => x.Name);

				foreach (var reference in references)
				{
					if (project.Name == reference)
						throw new FlcException(
							$"{project.Name} cannot reference itself");
					if (!projectNames.Contains(reference!))
						throw new FlcException(
							$"{project.Name} cannot reference {reference} as no project exists with that name in this solution");
				}
			}
		}
	}
}
