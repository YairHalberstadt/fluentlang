using Autofac;
using FluentLang.flc;
using FluentLang.flc.DependencyInjection;
using FluentLang.TestUtils;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit
{
	public class FluentLangCompilerTests : TestBase
	{
		[Theory]
		[InlineData("")]
		[InlineData("artifacts")]
		[InlineData("artifacts/")]
		[InlineData("artifacts/bin")]
		public async Task FluentLangCompilerWritesAssemblyToOutputDirectory(string outputDirectory)
		{
			var fileSystem = new MockFileSystem(
				new Dictionary<string, MockFileData>
				{ 
					{
						"solutionFile",
						@"
{
  ""Projects"": [
    {
      ""Name"": ""Project"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""./""
      ]
	}
	]
}"
					},
					{
						"file.fl",
						@"M() : int { return 42; }"
					}
				});

			var builder = new ContainerBuilder();
			builder.RegisterModule(new FlcModule(LogLevel.Trace));
			builder.RegisterInstance(fileSystem).As<IFileSystem>();
			using var container = builder.Build();
			var compiler = container.Resolve<FluentLangCompiler>();
			await compiler.Build("solutionFile", outputDirectory, outputCSharp: false);
			Assert.Equal(2560, fileSystem.File.ReadAllBytes(fileSystem.Path.Combine(outputDirectory, "Project.dll")).Length);
		}

		[Theory]
		[InlineData("")]
		[InlineData("artifacts")]
		[InlineData("artifacts/")]
		[InlineData("artifacts/bin")]
		public async Task FluentLangCompilerWritesCSharpToOutputDirectory(string outputDirectory)
		{
			var fileSystem = new MockFileSystem(
				new Dictionary<string, MockFileData>
				{
					{
						"solutionFile",
						@"
{
  ""Projects"": [
    {
      ""Name"": ""Project"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""./""
      ]
	}
	]
}"
					},
					{
						"file.fl",
						@"M() : int { return 42; }"
					}
				});

			var builder = new ContainerBuilder();
			builder.RegisterModule(new FlcModule(LogLevel.Trace));
			builder.RegisterInstance(fileSystem).As<IFileSystem>();
			using var container = builder.Build();
			var compiler = container.Resolve<FluentLangCompiler>();
			await compiler.Build("solutionFile", outputDirectory, outputCSharp: true);
			Assert.Equal(2560, fileSystem.File.ReadAllBytes(fileSystem.Path.Combine(outputDirectory, "Project.dll")).Length);

			var expectedCSharp = @"
using FluentLang.Runtime;
using FluentLang.Runtime.Metadata;
using System;
using AssemblyFileVersionAttribute = System.Reflection.AssemblyFileVersionAttribute;
[assembly: AssemblyName(""\u0050\u0072\u006F\u006A\u0065\u0063\u0074"")]
[assembly: AssemblyFileVersion(""\u0030\u002E\u0030\u002E\u0031\u0035\u0037\u0038\u0035\u0039\u0036\u0031\u0038\u0034"")]
public static class Project_AssemblyLevelMethods
{
	static int M()
	{
		return (42);
	}
}";
			var actual = 
				fileSystem.File.ReadAllText(fileSystem.Path.Combine(outputDirectory, "Project.cs"));
			Assert.Equal(
				Normalise(expectedCSharp),
				Normalise(actual));

			static string Normalise(string csharp)
			{
				csharp = Regex.Replace(
					csharp,
					"\\[assembly: AssemblyFileVersion\\(\"(\\\\u[0-9A-F]*)*\"\\)\\]",
					"");
				csharp = Regex.Replace(csharp, "\\s+", "");
				return csharp;
			}
		}

		public FluentLangCompilerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}
	}
}
