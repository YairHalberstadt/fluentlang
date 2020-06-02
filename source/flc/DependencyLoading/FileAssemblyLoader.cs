using FluentLang.Shared;
using Microsoft.Extensions.Logging;
using System;
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

		public FileAssemblyLoader(
			ILogger<FileAssemblyLoader> logger,
			IEnumerable<string> directoriesToCheck)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_directoriesToCheck = directoriesToCheck?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(directoriesToCheck));
		}
		public async ValueTask<AssemblyLoadResult?> TryLoadAssemblyAsync(AssemblyLoadContext assemblyLoadContext, Dependency dependency, CancellationToken cancellationToken = default)
		{
			var possiblePaths =
				_directoriesToCheck
				.Select(x => Path.Combine(x, dependency.Name + ".dll"));

			var assemblyLoadResult = await possiblePaths.ToAsyncEnumerable().SelectAwait(async x =>
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
							var bytes = (await File.ReadAllBytesAsync(x)).UnsafeAsImmutableArray();
							var assembly = assemblyLoadContext.LoadFromStream(bytes.ToStream());
							return new AssemblyLoadResult(assembly, bytes);
						}
					}
					catch (FileNotFoundException)
					{
					}
				}
				return null;
			}).FirstOrDefaultAsync(x => x != null);

			if (assemblyLoadResult is null)
			{
				_logger.LogInformation("Could not find assembly {0} {1} in any library directory", dependency.Name, dependency.Version);
			}

			return assemblyLoadResult;
		}
	}
}