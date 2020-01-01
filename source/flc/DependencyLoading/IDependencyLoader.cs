using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.flc.ProjectSystem;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace FluentLang.flc.DependencyLoading
{
	public interface IDependencyLoader
	{
		ValueTask<ImmutableArray<IAssembly>> LoadDependenciesAsync(ProjectInfo project, AssemblyLoadContext assemblyLoadContext, IEnumerable<IAssembly> alreadyLoadedProjects, CancellationToken cancellationToken = default);
	}
}