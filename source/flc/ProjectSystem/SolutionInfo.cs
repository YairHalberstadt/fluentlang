using Newtonsoft.Json;
using System.Collections.Immutable;

namespace FluentLang.flc.ProjectSystem
{
	public class SolutionInfo
	{
		[JsonConstructor]
		public SolutionInfo(ImmutableArray<ProjectInfo> projects, ImmutableArray<string> libDirectories, ImmutableArray<string> nugetFeeds)
		{
			Projects = projects.IsDefaultOrEmpty ? ImmutableArray<ProjectInfo>.Empty : projects;
			LibDirectories = libDirectories.IsDefault ? ImmutableArray<string>.Empty : libDirectories;
			NugetFeeds = nugetFeeds.IsDefault ? ImmutableArray<string>.Empty : nugetFeeds;
		}

		public ImmutableArray<ProjectInfo> Projects { get; }

		public ImmutableArray<string> LibDirectories { get; }

		public ImmutableArray<string> NugetFeeds { get; }
	}
}
