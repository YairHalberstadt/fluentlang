using Newtonsoft.Json;
using System;
using System.Collections.Immutable;

namespace FluentLang.flc.ProjectSystem
{
	public class ProjectInfo
	{
		[JsonConstructor]
		public ProjectInfo(
			string name,
			Version version,
			ImmutableArray<string> includedFilesAndFolders,
			ImmutableArray<string> excludedFilesAndFolders = default,
			ImmutableArray<Reference> references = default)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("is null or empty", nameof(name));
			Name = name;
			Version = version ?? throw new ArgumentNullException(nameof(version));
			if (includedFilesAndFolders.IsDefaultOrEmpty)
				throw new ArgumentException("must include at least one file", nameof(includedFilesAndFolders));
			IncludedFilesAndFolders = includedFilesAndFolders;
			ExcludedFilesAndFolders = excludedFilesAndFolders.IsDefault ? ImmutableArray<string>.Empty : excludedFilesAndFolders;
			References = references.IsDefault ? ImmutableArray<Reference>.Empty: references;
		}

		public ImmutableArray<string> IncludedFilesAndFolders { get; }
		public ImmutableArray<string> ExcludedFilesAndFolders { get; }
		public string Name { get; }
		public ImmutableArray<Reference> References { get; }
		public Version Version { get; }
	}
}