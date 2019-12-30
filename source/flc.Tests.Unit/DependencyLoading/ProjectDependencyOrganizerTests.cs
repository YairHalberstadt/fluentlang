using FluentLang.flc;
using FluentLang.flc.ProjectSystem;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.DependencyLoading
{
	public class ProjectDependencyOrganizerTests
	{
		private ProjectInfo Project(string name, params string[] references)
		{
			return new ProjectInfo(
				name,
				ImmutableArray.Create(""),
				references: references
					.Select(x => new Reference(Reference.ReferenceType.Project, name: x))
					.ToImmutableArray());
		}

		[Fact]
		public void ShouldDetectSimpleCycles()
		{
			var projects = new[]
			{
				Project("p1", "p2"),
				Project("p2", "p1"),
			};
			Assert.Throws<FlcException>(() => ProjectDependencyOrganizer.CreateBuildOrder(projects).ToList());
		}


		[Fact]
		public void ShouldDetectLongCycles()
		{
			var projects = new[]
			{
				Project("p1", "p3"),
				Project("p2", "p1"),
				Project("p3", "p2"),
			};
			Assert.Throws<FlcException>(() => ProjectDependencyOrganizer.CreateBuildOrder(projects).ToList());
		}

		[Fact]
		public void ShouldOrderDependenciesInOnlyOrderWhenOnlyOneAvailable()
		{
			var projects = new[]
			{
				Project("p4", "p2", "p3"),
				Project("p3", "p2"),
				Project("p2", "p1"),
				Project("p1"),
			};

			Assert.Equal(
				new[] { projects[3], projects[2], projects[1], projects[0] },
				ProjectDependencyOrganizer.CreateBuildOrder(projects));
		}

		[Fact]
		public void ShouldOrderDependenciesInPassedInOrderWhenMultipleAvailable()
		{
			var projects = new[]
			{
				Project("p1"),
				Project("p4", "p2", "p3"),
				Project("p2", "p1"),
				Project("p3", "p1"),
			};

			Assert.Equal(
				new[] { projects[0], projects[2], projects[3], projects[1] },
				ProjectDependencyOrganizer.CreateBuildOrder(projects));
		}

		[Fact]
		public void ShouldIgnoreNonProjectReferences()
		{
			var projects = new[]
			{
				new ProjectInfo(
					"p1",
					ImmutableArray.Create(""),
					references: ImmutableArray.Create(new Reference(Reference.ReferenceType.Assembly, "", ""))),
			};

			Assert.Equal(
				projects,
				ProjectDependencyOrganizer.CreateBuildOrder(projects));
		}
	}
}
