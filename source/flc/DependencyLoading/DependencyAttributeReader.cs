using FluentLang.Compiler.Symbols.Metadata;
using FluentLang.Runtime.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentLang.flc.DependencyLoading
{
	public class DependencyAttributeReader
	{
		public IEnumerable<Dependency> ReadDependencies(Assembly assembly)
		{
			return
				assembly
				.GetAttributes<DependencyAttribute>()
				.Select(x => new Dependency(x.Name, x.Version));
		}
	}
}
