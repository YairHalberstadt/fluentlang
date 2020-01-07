using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.IO.Abstractions;
using FluentLang.flc.ProjectSystem;
using System.Collections.Generic;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Runtime.Loader;
using System.Threading;
using System.Linq;
using System.IO;
using FluentLang.Compiler.Symbols;
using FluentLang.flc.Utils;
using System.Collections.Immutable;

namespace FluentLang.flc.DependencyLoading
{
	public class ProjectLoader : IProjectLoader
	{
		private readonly ILogger<ProjectLoader> _logger;
		private readonly IDependencyLoader _dependencyLoader;
		private readonly IFileSystem _fileSystem;

		public ProjectLoader(
			ILogger<ProjectLoader> logger,
			IDependencyLoader dependencyLoader,
			IFileSystem fileSystem)
		{
			_logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
			_dependencyLoader = dependencyLoader ?? throw new System.ArgumentNullException(nameof(dependencyLoader));
			_fileSystem = fileSystem ?? throw new System.ArgumentNullException(nameof(fileSystem));
		}

		public async ValueTask<IAssembly> LoadProjectAsync(
			ProjectInfo projectInfo,
			AssemblyLoadContext assemblyLoadContext,
			IEnumerable<IAssembly> alreadyLoadedProjects,
			CancellationToken cancellationToken = default)
		{
			var dependencies = await _dependencyLoader.LoadDependenciesAsync(
				projectInfo,
				assemblyLoadContext,
				alreadyLoadedProjects,
				cancellationToken);

			ImmutableArray<IDocument> documents;
			try
			{
				var includedFilePaths =
					projectInfo
					.IncludedFilesAndFolders
					.Select(x =>
					(
						path: NormalizePath(x),
						isDirectory: IsDirectory(x)
					))
					.SelectMany<(string path, bool isDirectory), string>(x =>
					{
						if (x.isDirectory)
							return _fileSystem.Directory.GetFiles(x.path, "*.fl", SearchOption.AllDirectories);
						return new[] { x.path };
					})
					.Distinct();

				var excludedFilesAndFolders =
					projectInfo
					.ExcludedFilesAndFolders
					.Select(x =>
					(
						path: NormalizePath(x),
						isDirectory: IsDirectory(x)
					)).ToList();

				documents =
					await
					includedFilePaths
					.Where(x => excludedFilesAndFolders.All(y => !IsOrContains(y.path, x)))
					.ToAsyncEnumerable()
					.SelectAwait(x => new ValueTask<string>(_fileSystem.File.ReadAllTextAsync(x, cancellationToken)))
					.Select(x => DocumentFactory.FromString(x))
					.ToImmutableArrayAsync();
			}
			catch (IOException e)
			{
				throw new FlcException($"failed to load included files and folders for {projectInfo.Name}", e);
			}
			return AssemblyFactory.FromSource(
				QualifiedName.Parse(projectInfo.Name),
				(projectInfo.Version.Major, projectInfo.Version.Minor, projectInfo.Version.Suffix),
				dependencies,
				documents);

		}

		private string NormalizePath(string path)
		{
			var fullPath = _fileSystem.Path.GetFullPath(path);

			return NormalizeInternal(fullPath);
			string NormalizeInternal(string path)
			{
				path = path.TrimEnd(_fileSystem.Path.DirectorySeparatorChar, _fileSystem.Path.AltDirectorySeparatorChar); // if you type c:\foo\ instead of c:\foo

				var name = _fileSystem.Path.GetFileName(path);
				if (name == "")
					return path + _fileSystem.Path.DirectorySeparatorChar;

				var parent = _fileSystem.Path.GetDirectoryName(path);

				parent = NormalizeInternal(parent);

				var parentDirectoryInfo = _fileSystem.DirectoryInfo.FromDirectoryName(parent);
				var pathFileSystemInfo = 
					parentDirectoryInfo
					.GetFileSystemInfos(name)
					.FirstOrDefault()
						?? throw new FlcException($"File {fullPath} does not exist.");

				return pathFileSystemInfo.FullName; // coming from GetFileSystemImfos() this has the correct case
			}
		}

		/// <summary>
		/// Checks whether fileOrDirectory is or contains path
		/// Only works on paths which have already been normalised using <see cref="NormalizePath(string)"/> 
		/// </summary>
		/// <param name="fileOrDirectory"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		private bool IsOrContains(string fileOrDirectory, string path)
		{
			return path.StartsWith(fileOrDirectory);
		}

		private bool IsDirectory(string x)
		{
			return _fileSystem.File.GetAttributes(x).HasFlag(FileAttributes.Directory);
		}
	}
}
