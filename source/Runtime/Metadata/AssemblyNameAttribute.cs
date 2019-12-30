using System;

namespace FluentLang.Runtime.Metadata
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public class AssemblyNameAttribute : Attribute
	{
		public AssemblyNameAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; }
	}
}
