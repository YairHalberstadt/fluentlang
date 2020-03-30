using FluentLang.flc;
using FluentLang.flc.ProjectSystem;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.ProjectSystem
{
	public class SolutionFactoryTests
	{
		private readonly string _validSolutionFile = @"
{
  ""Projects"": [
    {
      ""Name"": ""Project1"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""./""
      ],
      ""ExcludedFilesAndFolders"": [
        ""bin/""
      ]
	},
    {
      ""Name"": ""Project2"",
      ""Version"": { ""Major"": 1, ""Minor"": 2, ""VersionSuffix"": ""alpha"" },
      ""IncludedFilesAndFolders"": [
        ""./""
      ],
      ""IsTest"": true,
      ""References"": [
        {
          ""Type"": ""Assembly"",
          ""Name"": ""assembly"",
          ""Version"": ""1.2.3""
        },
        {
          ""Type"": ""Project"",
          ""Name"": ""Project1""
        }
      ]
    }
  ]
}";

		private readonly SolutionInfo _expected = new SolutionInfo(
				ImmutableArray.Create(
					new ProjectInfo(
						"Project1",
						new Version(0, 0),
						ImmutableArray.Create("./"),
						ImmutableArray.Create("bin/"),
						ImmutableArray<Reference>.Empty,
						isTest: false),
					new ProjectInfo(
						"Project2",
						new Version(1, 2, "alpha"),
						ImmutableArray.Create("./"),
						ImmutableArray<string>.Empty,
						ImmutableArray.Create(
							new Reference(Reference.ReferenceType.Assembly, "assembly", "1.2.3"),
							new Reference(Reference.ReferenceType.Project, "Project1")),
						isTest: true)),
				default,
				default);
		[Fact]
		public void CanParseValidSolutionFromString()
		{
			var solutionFactory = new SolutionFactory(
				NullLogger<SolutionFactory>.Instance,
				new MockFileSystem());

			var solution = solutionFactory.Parse(_validSolutionFile);

			AssertEqual(solution, _expected);
		}

		[Fact]
		public async Task CanParseValidSolutionFromFile()
		{
			var solutionFactory = new SolutionFactory(
				NullLogger<SolutionFactory>.Instance,
				new MockFileSystem(new Dictionary<string, MockFileData>{
					{ "c:/path.flsln", new MockFileData(_validSolutionFile) }
				}));

			var solution = await solutionFactory.ParseFromFileAsync("c:/path.flsln");

			AssertEqual(solution, _expected);
		}

		[Fact]
		public void ProjectReferenceMustExistInSolution()
		{
			var solutionFile = @"
{
  ""Projects"": [
    {
      ""Name"": ""Project"",
      ""IncludedFilesAndFolders"": [
        ""./""
      ],
      ""References"": [
        {
          ""Type"": ""Project"",
          ""ProjectName"": ""proj""
        }
      ]
    }
  ]
}";
			var solutionFactory = new SolutionFactory(
				NullLogger<SolutionFactory>.Instance,
				new MockFileSystem());

			Assert.Throws<FlcException>(() => solutionFactory.Parse(solutionFile));
		}

		[Fact]
		public void ProjectCannotReferenceItself()
		{
			var solutionFile = @"
{
  ""Projects"": [
    {
      ""Name"": ""Project"",
      ""IncludedFilesAndFolders"": [
        ""./""
      ],
      ""References"": [
        {
          ""Type"": ""Project"",
          ""ProjectName"": ""Project""
        }
      ]
    }
  ]
}";
			var solutionFactory = new SolutionFactory(
				NullLogger<SolutionFactory>.Instance,
				new MockFileSystem());

			Assert.Throws<FlcException>(() => solutionFactory.Parse(solutionFile));
		}

		[Fact]
		public void CanParseLibDirectories()
		{
			var solutionInfo = @"
{
	""LibDirectories"": [ ""a.dll"", ""b.dll"" ]
}
";
			var solutionFactory = new SolutionFactory(
				NullLogger<SolutionFactory>.Instance,
				new MockFileSystem());

			var solution = solutionFactory.Parse(solutionInfo);

			AssertEqual(solution, new SolutionInfo(default, ImmutableArray.Create("a.dll", "b.dll"), default));
		}

		[Fact]
		public void CanParseNugetFeeds()
		{
			var solutionInfo = @"
{
	""NugetFeeds"": [ ""http://nuget.org"", ""http://myget.org"" ]
}
";
			var solutionFactory = new SolutionFactory(
				NullLogger<SolutionFactory>.Instance,
				new MockFileSystem());

			var solution = solutionFactory.Parse(solutionInfo);

			AssertEqual(solution, new SolutionInfo(default, default, ImmutableArray.Create("http://nuget.org", "http://myget.org")));
		}

		private static void AssertEqual(SolutionInfo a, SolutionInfo b)
		{
			Assert.Equal(a.Projects.Length, b.Projects.Length);
			foreach (var (ap, bp) in a.Projects.Zip(b.Projects))
			{
				AssertEqual(ap, bp);
			}

			static void AssertEqual(ProjectInfo a, ProjectInfo b)
			{
				Assert.Equal(a.Name, b.Name);
				Assert.Equal(a.IncludedFilesAndFolders.Length, b.IncludedFilesAndFolders.Length);
				Assert.Equal(a.ExcludedFilesAndFolders.Length, b.ExcludedFilesAndFolders.Length);
				Assert.Equal(a.References.Length, b.References.Length);

				foreach (var (aif, bif) in a.IncludedFilesAndFolders.Zip(b.IncludedFilesAndFolders))
				{
					Assert.Equal(aif, bif);
				}

				foreach (var (aef, bef) in a.ExcludedFilesAndFolders.Zip(b.ExcludedFilesAndFolders))
				{
					Assert.Equal(aef, bef);
				}


				foreach (var (ar, br) in a.References.Zip(b.References))
				{
					AssertEqual(ar, br);
				}

				static void AssertEqual(Reference a, Reference b)
				{
					Assert.Equal(a.Type, b.Type);
					Assert.Equal(a.Name, b.Name);
					Assert.Equal(a.Version, b.Version);
				}
			}

		}
	}
}
