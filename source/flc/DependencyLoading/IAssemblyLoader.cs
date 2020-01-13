using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace FluentLang.flc.DependencyLoading
{
	public interface IAssemblyLoader
	{
		ValueTask<AssemblyLoadResult?> TryLoadAssemblyAsync(AssemblyLoadContext assemblyLoadContext, Dependency dependency, CancellationToken cancellationToken = default);
	}
}