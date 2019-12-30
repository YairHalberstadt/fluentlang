using System;

namespace FluentLang.flc.DependencyLoading
{
	public class Dependency : IEquatable<Dependency?>
	{
		public Dependency(string name, string version)
		{
			Name = name;
			Version = version;
		}

		public string Name { get; }
		public string Version { get; }

		public override bool Equals(object? obj)
		{
			return Equals(obj as Dependency);
		}

		public bool Equals(Dependency? other)
		{
			return other != null &&
				   Name == other.Name &&
				   Version == other.Version;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, Version);
		}
	}
}
