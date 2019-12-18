using FluentLang.flc;
using FluentLang.flc.ProjectSystem;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace flc.Tests.Unit
{
	public class SolutionFactoryTests
	{
		private readonly string _validSolutionFile = @"
{
  ""Projects"": [
    {
      ""Name"": ""Project1"",
      ""IncludedFilesAndFolders"": [
        ""./""
      ],
      ""ExcludedFilesAndFolders"": [
        ""bin/""
      ]
	},
    {
      ""Name"": ""Project2"",
      ""IncludedFilesAndFolders"": [
        ""./""
      ],
      ""References"": [
        {
          ""Type"": ""Url"",
          ""AssemblyUrl"": ""https://example.com/assembly""
		},
        {
          ""Type"": ""FilePath"",
          ""AssemblyFilePath"": ""bin/assembly""
        },
        {
          ""Type"": ""Project"",
          ""ProjectName"": ""Project1""
        }
      ]
    }
  ]
}";

		private readonly SolutionInfo _expected = new SolutionInfo(
				ImmutableArray.Create(
					new ProjectInfo(
						"Project1",
						ImmutableArray.Create("./"),
						ImmutableArray.Create("bin/"),
						ImmutableArray<Reference>.Empty),
					new ProjectInfo(
						"Project2",
						ImmutableArray.Create("./"),
						ImmutableArray<string>.Empty,
						ImmutableArray.Create(
							new Reference(Reference.ReferenceType.Url, assemblyUrl: "https://example.com/assembly"),
							new Reference(Reference.ReferenceType.FilePath, assemblyFilePath: "bin/assembly"),
							new Reference(Reference.ReferenceType.Project, projectName: "Project1")))));
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
					Assert.Equal(a.AssemblyUrl, b.AssemblyUrl);
					Assert.Equal(a.AssemblyFilePath, b.AssemblyFilePath);
					Assert.Equal(a.ProjectName, b.ProjectName);
				}
			}

		}
	}
}
