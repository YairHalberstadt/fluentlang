using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.flc.ProjectSystem;
using FluentLang.flc.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using static FluentLang.flc.ProjectSystem.Reference.ReferenceType;

namespace FluentLang.flc.DependencyLoading
{
	public class DependencyLoader : IDependencyLoader
	{
		private readonly ImmutableArray<IAssemblyLoader> _assemblyLoaders;
		private readonly DependencyAttributeReader _dependencyAttributeReader;
		private readonly AssemblyFactory _assemblyFactory;
		private readonly ILogger<DependencyLoader> _logger;
		/// <summary>
		/// https://andrewlock.net/making-getoradd-on-concurrentdictionary-thread-safe-using-lazy/
		/// </summary>
		private readonly ConcurrentDictionary<Dependency, Lazy<Task<IAssembly>>> _assemblies
			= new ConcurrentDictionary<Dependency, Lazy<Task<IAssembly>>>();

		public DependencyLoader(
			IEnumerable<IAssemblyLoader> assemblyLoaders,
			DependencyAttributeReader dependencyAttributeReader,
			AssemblyFactory assemblyFactory,
			ILogger<DependencyLoader> logger)
		{
			_assemblyLoaders = assemblyLoaders?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(assemblyLoaders));
			_dependencyAttributeReader = dependencyAttributeReader ?? throw new ArgumentNullException(nameof(dependencyAttributeReader));
			_assemblyFactory = assemblyFactory ?? throw new ArgumentNullException(nameof(assemblyFactory));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public ValueTask<ImmutableArray<IAssembly>> LoadDependenciesAsync(
			ProjectInfo project,
			AssemblyLoadContext assemblyLoadContext,
			IEnumerable<IAssembly> alreadyLoadedProjects,
			CancellationToken cancellationToken = default)
		{
			return
				project
					.References
					.ToAsyncEnumerable()
					.SelectAwait(x => LoadDependencyAsync(
						assemblyLoadContext,
						x,
						alreadyLoadedProjects,
						project,
						cancellationToken))
					.ToImmutableArrayAsync();
		}

		private ValueTask<IAssembly> LoadDependencyAsync(
			AssemblyLoadContext assemblyLoadContext,
			Reference reference,
			IEnumerable<IAssembly> alreadyLoadedProjects,
			ProjectInfo project,
			CancellationToken cancellationToken)
		{
			if (reference.Type == Project)
			{
				var name = QualifiedName.Parse(reference.Name);
				var assembly = alreadyLoadedProjects.FirstOrDefault(x => x.Name == name)
					?? throw new InvalidOperationException($"{name} is a dependency of {project}, so must be built first");
				return new ValueTask<IAssembly>(assembly);
			}

			Release.Assert(reference.Version != null);

			return new ValueTask<IAssembly>(LoadDependencyAsync(
				assemblyLoadContext,
				new Dependency(reference.Name, reference.Version),
				cancellationToken));
		}

		private Task<IAssembly> LoadDependencyAsync(
			AssemblyLoadContext assemblyLoadContext,
			Dependency dependency,
			CancellationToken cancellationToken)
		{
			return
				_assemblies.GetOrAdd(
					dependency,
					_ => new Lazy<Task<IAssembly>>(LoadDependencyAsyncInternal))
				.Value;

			// Cannot return ValueTask since possible to await multiple times.
			async Task<IAssembly> LoadDependencyAsyncInternal()
			{
				var assemblyLoadResult =
					(await
						_assemblyLoaders
						.ToAsyncEnumerable()
						.SelectAwait(x => x.TryLoadAssemblyAsync(
							assemblyLoadContext,
							dependency,
							cancellationToken))
						.FirstOrDefaultAsync(x => x != null)
						.ConfigureAwait(false))
					?? throw new FlcException(
							$"Could not find assembly {dependency.Name} {dependency.Version} in any location");

				var subDependencies =
					await
						_dependencyAttributeReader
						.ReadDependencies(assemblyLoadResult.Assembly)
						.ToAsyncEnumerable()
						.SelectAwait(x => new ValueTask<IAssembly>(
							LoadDependencyAsync(assemblyLoadContext, x, cancellationToken)))
						.ToImmutableArrayAsync()
						.ConfigureAwait(false);

				return _assemblyFactory.FromMetadata(
					assemblyLoadResult.Assembly,
					assemblyLoadResult.Bytes,
					subDependencies);
			}
		}
	}
}
