using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace FluentLang.flc.ProjectCompiler
{
	public class NugetAssemblyLoader
	{
		private readonly ILogger<NugetAssemblyLoader> _logger;
		private readonly NugetLoggerAdapter _nugetLogger;
		private readonly string _globalPackagesFolder;

		private readonly SourceCacheContext _sourceCacheContext = new SourceCacheContext
		{
			DirectDownload = false,
			NoCache = false,
		};

		public NugetAssemblyLoader(ILogger<NugetAssemblyLoader> logger, string globalPackagesFolder)
		{
			_logger = logger;
			_nugetLogger = new NugetLoggerAdapter(logger);
			_globalPackagesFolder = globalPackagesFolder;
		}

		public async ValueTask<Assembly> LoadDependency(
			AssemblyLoadContext assemblyLoadContext,
			string id,
			string version,
			string? nugetFeedUrl = null,
			CancellationToken cancellationToken = default)
		{
			if (!NuGetVersion.TryParse(version, out var nuGetVersion))
			{
				throw new ArgumentException($"invalid version {version} for nuget package {id}", nameof(version));
			}

			nugetFeedUrl ??= "https://api.nuget.org/v3/index.json";
			_logger.LogInformation(
				"Downloading Nuget Package ID: {0}, Version: {1} from {2}",
				id,
				version,
				nugetFeedUrl);
			var repository = Repository.Factory.GetCoreV3(nugetFeedUrl);
			var downloadResource = await repository.GetResourceAsync<DownloadResource>();
			using var downloadResourceResult = await downloadResource.GetDownloadResourceResultAsync(
				new PackageIdentity(id, nuGetVersion),
				new PackageDownloadContext(_sourceCacheContext),
				globalPackagesFolder: _globalPackagesFolder,
				logger: _nugetLogger,
				token: cancellationToken);

			if (downloadResourceResult.Status != DownloadResourceResultStatus.Available)
			{
				throw new FlcException($"Download of NuGet package failed. DownloadResult Status: {downloadResourceResult.Status}");
			}

			_logger.LogInformation("Successfully Downloaded Nuget Package");

			var reader = downloadResourceResult.PackageReader;

			if (_logger.IsEnabled(LogLevel.Debug))
			{
				_logger.LogDebug("Nuget Package Metadata: {0}", reader.ToJson());
			}

			var libFile =
				reader
				.GetLibItems()
				.SelectMany(x => x.Items)
				.FirstOrDefault(x => Path.GetExtension(x) == ".dll");

			if (libFile is null)
			{
				throw new FlcException("No dll found in Nuget Package");
			}

			_logger.LogInformation("Found dll {0}", libFile);
			_logger.LogInformation("Decompressing {0}", libFile);

			var archive = new ZipArchive(downloadResourceResult.PackageStream);
			var entry = archive.GetEntry(libFile);
			using var decompressed = new MemoryStream();
			entry.Open().CopyTo(decompressed);
			decompressed.Position = 0;

			_logger.LogInformation("Loading {0}", libFile);
			var assembly = assemblyLoadContext.LoadFromStream(decompressed);
			_logger.LogInformation("Successfully loaded assembly {0}", assembly.FullName);
			return assembly;
		}

		private class NugetLoggerAdapter : NuGet.Common.ILogger
		{
			private readonly ILogger _logger;

			public NugetLoggerAdapter(ILogger logger)
			{
				_logger = logger;
			}

			public void Log(NuGet.Common.LogLevel level, string data)
			{
				switch (level)
				{
					case NuGet.Common.LogLevel.Debug:
					case NuGet.Common.LogLevel.Verbose:
						_logger.LogDebug(data);
						break;
					case NuGet.Common.LogLevel.Information:
					case NuGet.Common.LogLevel.Minimal:
						_logger.LogInformation(data);
						break;
					case NuGet.Common.LogLevel.Warning:
						_logger.LogWarning(data);
						break;
					case NuGet.Common.LogLevel.Error:
						_logger.LogWarning(data);
						break;
						throw new InvalidEnumArgumentException(
							nameof(level),
							(int)level,
							typeof(NuGet.Common.LogLevel));
				}
			}

			public void Log(ILogMessage message)
			{
				Log(message.Level, message.Message);
			}

			public Task LogAsync(NuGet.Common.LogLevel level, string data)
			{
				Log(level, data);
				return Task.CompletedTask;
			}

			public Task LogAsync(ILogMessage message)
			{
				return LogAsync(message.Level, message.Message);
			}

			public void LogDebug(string data)
			{
				_logger.LogDebug(data);
			}

			public void LogError(string data)
			{
				_logger.LogError(data);
			}

			public void LogInformation(string data)
			{
				_logger.LogInformation(data);
			}

			public void LogInformationSummary(string data)
			{
				_logger.LogInformation(data);
			}

			public void LogMinimal(string data)
			{
				_logger.LogInformation(data);
			}

			public void LogVerbose(string data)
			{
				_logger.LogDebug(data);
			}

			public void LogWarning(string data)
			{
				_logger.LogWarning(data);
			}
		}
	}
}