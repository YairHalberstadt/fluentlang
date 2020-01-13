using System.Collections.Immutable;
using System.Reflection;

namespace FluentLang.flc.DependencyLoading
{
	public class AssemblyLoadResult
	{
		public AssemblyLoadResult(Assembly assembly, ImmutableArray<byte> bytes)
		{
			Assembly = assembly;
			Bytes = bytes;
		}

		public Assembly Assembly { get; }

		public ImmutableArray<byte> Bytes { get; }
	}
}