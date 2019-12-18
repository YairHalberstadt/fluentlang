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
			ImmutableArray<string> includedFilesAndFolders,
			ImmutableArray<string> excludedFilesAndFolders = default,
			ImmutableArray<Reference> references = default)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("is null or empty", nameof(name));
			if (includedFilesAndFolders.IsDefaultOrEmpty)
				throw new ArgumentException("must include at least one file", nameof(includedFilesAndFolders));
			IncludedFilesAndFolders = includedFilesAndFolders;
			ExcludedFilesAndFolders = excludedFilesAndFolders.IsDefault ? ImmutableArray<string>.Empty : excludedFilesAndFolders;
			Name = name;
			References = references.IsDefault ? ImmutableArray<Reference>.Empty: references;
		}

		public ImmutableArray<string> IncludedFilesAndFolders { get; }
		public ImmutableArray<string> ExcludedFilesAndFolders { get; }
		public string Name { get; }
		public ImmutableArray<Reference> References { get; }
	}
}