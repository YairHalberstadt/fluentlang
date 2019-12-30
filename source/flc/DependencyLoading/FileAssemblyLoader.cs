using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace FluentLang.flc.DependencyLoading
{
	public class FileAssemblyLoader : IAssemblyLoader
	{
		private readonly ILogger<FileAssemblyLoader> _logger;
		private readonly ImmutableArray<string> _directoriesToCheck;

		public FileAssemblyLoader(ILogger<FileAssemblyLoader> logger, ImmutableArray<string> directoriesToCheck)
		{
			_logger = logger;
			_directoriesToCheck = directoriesToCheck;
		}
		public ValueTask<Assembly?> TryLoadAssemblyAsync(AssemblyLoadContext assemblyLoadContext, Dependency dependency, CancellationToken cancellationToken = default)
		{
			var possiblePaths =
				_directoriesToCheck
				.Select(x => Path.Combine(x, dependency.Name + ".dll"));

			var assembly = possiblePaths.Select(x =>
			{
				// Check if file exists to reduce nr of exceptions
				// no point injecting IFileSystem since it doesn't have 
				// an abstraction for FileVersionInfo anyway
				if (File.Exists(x))
				{
					try
					{
						if (FileVersionInfo.GetVersionInfo(x).FileVersion == dependency.Version)
						{
							return assemblyLoadContext.LoadFromAssemblyPath(Path.GetFullPath(x));
						}
					}
					catch (FileNotFoundException)
					{
					}
				}
				return null;
			}).FirstOrDefault(x => x != null);

			if (assembly is null)
			{
				_logger.LogInformation("Could not find assembly {0} {1} in any library directory", dependency.Name, dependency.Version);
			}

			return new ValueTask<Assembly?>(assembly);
		}
	}
}