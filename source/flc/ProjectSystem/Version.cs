using Newtonsoft.Json;
using System;

namespace FluentLang.flc.ProjectSystem
{
	public class Version
	{
		[JsonConstructor]
		public Version(int major, int minor, string? suffix = null)
		{
			Major = major;
			Minor = minor;
			Suffix = suffix;
		}

		public int Major { get; }
		public int Minor { get; }
		public string? Suffix { get; }
	}
}