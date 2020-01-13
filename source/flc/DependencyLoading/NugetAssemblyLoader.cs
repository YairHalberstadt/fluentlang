using FluentLang.Compiler.Helpers;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Immutable;
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

namespace FluentLang.flc.DependencyLoading
{
	public class NugetAssemblyLoader : IAssemblyLoader
	{
		private readonly ILogger<NugetAssemblyLoader> _logger;
		private readonly NugetLoggerAdapter _nugetLogger;
		private readonly string _globalPackagesFolder;

		private readonly SourceCacheContext _sourceCacheContext = new SourceCacheContext
		{
			DirectDownload = false,
			NoCache = false,
		};

		private static readonly ImmutableArray<string> _defaultFeeds 
			= ImmutableArray.Create("https://api.nuget.org/v3/index.json");

		private readonly ImmutableArray<string> _nugetFeedUrls;

		public NugetAssemblyLoader(
			ILogger<NugetAssemblyLoader> logger,
			string globalPackagesFolder,
			ImmutableArray<string> nugetFeedUrls)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_nugetLogger = new NugetLoggerAdapter(logger);
			_globalPackagesFolder = globalPackagesFolder ?? throw new ArgumentNullException(nameof(globalPackagesFolder));
			if (nugetFeedUrls.IsDefaultOrEmpty)
			{
				nugetFeedUrls = _defaultFeeds;
			}
			_nugetFeedUrls = nugetFeedUrls;
		}

		public async ValueTask<AssemblyLoadResult?> TryLoadAssemblyAsync(
			AssemblyLoadContext assemblyLoadContext,
			Dependency dependency,
			CancellationToken cancellationToken = default)
		{
			if (!NuGetVersion.TryParse(dependency.Version, out var nuGetVersion))
			{
				throw new ArgumentException($"invalid version {dependency.Version} for nuget package {dependency.Name}", nameof(dependency));
			}

			var assemblyLoadResult = await 
				_nugetFeedUrls
				.ToAsyncEnumerable()
				.SelectAwait(TryLoadAssemblyFromFeed)
				.FirstOrDefaultAsync(x => x != null, cancellationToken);

			if (assemblyLoadResult is null)
			{
				_logger.LogInformation("Could not download nuget package {0} {1} from any nuget feed", dependency.Name, dependency.Version);
			}

			return assemblyLoadResult;

			async ValueTask<AssemblyLoadResult?> TryLoadAssemblyFromFeed(string feedUrl)
			{
				_logger.LogInformation(
					"Attempting to download Nuget Package ID: {0}, Version: {1} from {2}",
					dependency.Name,
					dependency.Version,
					feedUrl);
				var repository = Repository.Factory.GetCoreV3(feedUrl);
				var downloadResource = await repository.GetResourceAsync<DownloadResource>();
				using var downloadResourceResult = await downloadResource.GetDownloadResourceResultAsync(
					new PackageIdentity(dependency.Name, nuGetVersion),
					new PackageDownloadContext(_sourceCacheContext),
					globalPackagesFolder: _globalPackagesFolder,
					logger: _nugetLogger,
					token: cancellationToken);

				if (downloadResourceResult.Status != DownloadResourceResultStatus.Available)
				{
					_logger.LogInformation(
						$"Download of NuGet package from {0} failed. DownloadResult Status: {1}",
						feedUrl,
						downloadResourceResult.Status);
					return null;
				}

				_logger.LogInformation("Successfully Downloaded Nuget Package from {0}", feedUrl);

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
				var bytes = decompressed.ToImmutableArray();
				_logger.LogInformation("Successfully loaded assembly {0}", assembly.FullName);
				return new AssemblyLoadResult(assembly, bytes);
			}
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