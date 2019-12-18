using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace FluentLang.flc.ProjectSystem
{
	public class SolutionInfo
	{
		[JsonConstructor]
		public SolutionInfo(ImmutableArray<ProjectInfo> projects)
		{
			Projects = projects.IsDefaultOrEmpty ? ImmutableArray<ProjectInfo>.Empty : projects;
		}

		public ImmutableArray<ProjectInfo> Projects { get; }
	}
}
