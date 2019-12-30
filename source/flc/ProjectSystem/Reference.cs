using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace FluentLang.flc.ProjectSystem
{
	public sealed class Reference
	{
		[JsonConstructor]
		public Reference(
			ReferenceType type,
			string name,
			string? version = null)
		{
			if (type == ReferenceType.Project && version is { })
				throw new ArgumentException("Project reference cannot specify a version", nameof(version));

			if (type == ReferenceType.Assembly && version is null)
				throw new ArgumentException("Assembly reference must specify a version", nameof(version));

			if (type != ReferenceType.Assembly && type != ReferenceType.Project)
				throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(ReferenceType));

			Type = type;
			Name = name;
			Version = version;
		}

		public ReferenceType Type { get; }
		public string Name { get; }
		public string? Version { get; }

		public enum ReferenceType
		{
			Assembly,
			Project,
		}
	}
}