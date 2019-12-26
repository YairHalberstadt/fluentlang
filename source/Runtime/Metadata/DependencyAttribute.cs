using System;

namespace FluentLang.Runtime.Metadata
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class DependencyAttribute : Attribute
	{
		public DependencyAttribute(string name, string version)
		{
			Name = name;
			Version = version;
		}

		public string Name { get; }
		public string Version { get; }
	}
}
