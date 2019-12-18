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
			string? assemblyUrl = null,
			string? assemblyFilePath = null,
			string? projectName = null)
		{
			switch (type)
			{
				case ReferenceType.Url:
					if (assemblyUrl is null)
						throw new ArgumentNullException(assemblyUrl);
					if (assemblyFilePath != null)
						throw new ArgumentException(
							$"parameter cannot be set when reference type is {type}", nameof(assemblyFilePath));
					if (projectName != null)
						throw new ArgumentException(
							$"parameter cannot be set when reference type is {type}", nameof(ProjectName));
					break;
				case ReferenceType.FilePath:
					if (assemblyFilePath is null)
						throw new ArgumentNullException(assemblyFilePath);
					if (assemblyUrl != null)
						throw new ArgumentException(
							$"parameter cannot be set when reference type is {type}", nameof(assemblyUrl));
					if (projectName != null)
						throw new ArgumentException(
							$"parameter cannot be set when reference type is {type}", nameof(ProjectName));
					break;
				case ReferenceType.Project:
					if (projectName is null)
						throw new ArgumentNullException(projectName);
					if (assemblyUrl != null)
						throw new ArgumentException(
							$"parameter cannot be set when reference type is {type}", nameof(assemblyUrl));
					if (assemblyFilePath != null)
						throw new ArgumentException(
							$"parameter cannot be set when reference type is {type}", nameof(assemblyFilePath));
					break;
				default: throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(ReferenceType));
			}
			AssemblyUrl = assemblyUrl;
			AssemblyFilePath = assemblyFilePath;
			ProjectName = projectName;
			Type = type;
		}

		public string? AssemblyUrl { get; }

		public string? AssemblyFilePath { get; }

		public string? ProjectName { get; }

		public ReferenceType Type { get; }

		public enum ReferenceType
		{
			Url,
			FilePath,
			Project,
		}
	}
}