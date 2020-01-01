using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.flc.ProjectSystem;
using FluentLang.flc.Utils;
using Microsoft.Extensions.Logging;
using System;
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
		private readonly ILogger<DependencyLoader> _logger;

		public DependencyLoader(
			ImmutableArray<IAssemblyLoader> assemblyLoaders,
			DependencyAttributeReader dependencyAttributeReader,
			ILogger<DependencyLoader> logger)
		{
			_assemblyLoaders = assemblyLoaders;
			_dependencyAttributeReader = dependencyAttributeReader;
			_logger = logger;
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

			return LoadDependencyAsync(
				assemblyLoadContext,
				new Dependency(reference.Name, reference.Version),
				cancellationToken);
		}

		private async ValueTask<IAssembly> LoadDependencyAsync(
			AssemblyLoadContext assemblyLoadContext,
			Dependency dependency,
			CancellationToken cancellationToken)
		{
			var assembly =
				assemblyLoadContext
				.Assemblies
				.FirstOrDefault(x => x.GetName().Name == $"{dependency.Name}${dependency.Version}");

			assembly ??=
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
					.ReadDependencies(assembly)
					.ToAsyncEnumerable()
					.SelectAwait(x => LoadDependencyAsync(assemblyLoadContext, x, cancellationToken))
					.ToImmutableArrayAsync()
					.ConfigureAwait(false);

			return AssemblyFactory.FromMetadata(assembly, subDependencies);
		}
	}
}
