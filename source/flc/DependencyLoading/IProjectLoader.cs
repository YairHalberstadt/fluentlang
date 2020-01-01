using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.flc.ProjectSystem;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace FluentLang.flc.DependencyLoading
{
	public interface IProjectLoader
	{
		ValueTask<IAssembly> LoadProjectAsync(ProjectInfo projectInfo, AssemblyLoadContext assemblyLoadContext, IEnumerable<IAssembly> alreadyLoadedProjects, CancellationToken cancellationToken = default);
	}
}