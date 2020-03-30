using Autofac;
using FluentLang.Compiler.Helpers;
using FluentLang.flc;
using FluentLang.flc.DependencyInjection;
using FluentLang.flc.DependencyLoading;
using FluentLang.TestUtils;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit
{
	public class FluentLangCompilerTests
	{
		public class BuildTests : TestBase
		{
			[Theory]
			[InlineData("")]
			[InlineData("artifacts")]
			[InlineData("artifacts/")]
			[InlineData("artifacts/bin")]
			public async Task WritesAssemblyToOutputDirectory(string outputDirectory)
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

				var builder = CreateBuilder(fileSystem);
				using var container = builder.Build();
				var compiler = container.Resolve<FluentLangCompiler>();
				await compiler.Build("solutionFile", outputDirectory, outputCSharp: false, test: false);
				Assert.Equal(2560, fileSystem.File.ReadAllBytes(fileSystem.Path.Combine(outputDirectory, "Project", "Project.dll")).Length);
			}

			[Theory]
			[InlineData("")]
			[InlineData("artifacts")]
			[InlineData("artifacts/")]
			[InlineData("artifacts/bin")]
			public async Task WritesCSharpToOutputDirectory(string outputDirectory)
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

				var builder = CreateBuilder(fileSystem);
				using var container = builder.Build();
				var compiler = container.Resolve<FluentLangCompiler>();
				await compiler.Build("solutionFile", outputDirectory, outputCSharp: true, test: false);
				Assert.Equal(2560, fileSystem.File.ReadAllBytes(fileSystem.Path.Combine(outputDirectory, "Project", "Project.dll")).Length);

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
					fileSystem.File.ReadAllText(fileSystem.Path.Combine(outputDirectory, "Project", "Project.cs"));
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

			[Fact]
			public async Task WritesAllProjectsToOutputDirectory()
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
      ""Name"": ""Project1"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""./""
      ]
	},
    {
      ""Name"": ""Project2"",
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

				var builder = CreateBuilder(fileSystem);
				using var container = builder.Build();
				var compiler = container.Resolve<FluentLangCompiler>();
				await compiler.Build("solutionFile", outputDirectory: "", outputCSharp: false, test: false);
				Assert.Equal(2560, fileSystem.File.ReadAllBytes("Project1/Project1.dll").Length);
				Assert.Equal(2560, fileSystem.File.ReadAllBytes("Project2/Project2.dll").Length);
			}

			[Fact]
			public async Task StopsIfFirstProjectFailsToCompile()
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
      ""Name"": ""Project1"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""a.fl""
      ]
	},
    {
      ""Name"": ""Project2"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""b.fl""
      ]
	}
	]
}"
					},
					{
						"a.fl",
						@"M(}{};"
					},
					{
						"b.fl",
						@"M() : int { return 42; }"
					}
					});

				var builder = CreateBuilder(fileSystem);
				using var container = builder.Build();
				var compiler = container.Resolve<FluentLangCompiler>();
				await compiler.Build("solutionFile", outputDirectory: "", outputCSharp: false, test: false);
				Assert.False(fileSystem.File.Exists("Project1/Project1.dll"));
				Assert.False(fileSystem.File.Exists("Project2/Project2.dll"));
			}

			[Fact]
			public async Task WritesDependenciesToOutputFile()
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
      ],
	  ""References"": [
        {
          ""Type"": ""Assembly"",
          ""Name"": ""assembly"",
          ""Version"": ""1.0.0""
        }
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

				CreateAssembly("", "assembly").TryGetAssemblyBytes(out var bytes);
				var builder = CreateBuilder(fileSystem);

				builder.RegisterInstance<IAssemblyLoader>(new MockAssemblyLoader(bytes));
				using var container = builder.Build();
				var compiler = container.Resolve<FluentLangCompiler>();
				await compiler.Build("solutionFile", "", outputCSharp: false, test: false);

				Assert.Equal(bytes, fileSystem.File.ReadAllBytes("Project/assembly.dll"));
				Assert.Equal(2560, fileSystem.File.ReadAllBytes("Project/Project.dll").Length);
			}
			public class MockAssemblyLoader : IAssemblyLoader
			{
				private readonly ImmutableArray<byte> _bytes;

				public MockAssemblyLoader(ImmutableArray<byte> bytes)
				{
					_bytes = bytes;
				}

				public ValueTask<AssemblyLoadResult?> TryLoadAssemblyAsync(AssemblyLoadContext assemblyLoadContext, Dependency dependency, CancellationToken cancellationToken = default)
				{
					return new ValueTask<AssemblyLoadResult?>(
						new AssemblyLoadResult(
							assemblyLoadContext.LoadFromStream(_bytes.ToStream()),
							_bytes));
				}
			}

			public BuildTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}
		}

		public class RunTests : TestBase
		{
			[Fact]
			public async Task CanRunProject()
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
						@"Main() : int { return 42; }"
					}
					});

				var builder = CreateBuilder(fileSystem);
				using var container = builder.Build();
				var compiler = container.Resolve<FluentLangCompiler>();
				var result = await compiler.Run("solutionFile", "Project");
				Assert.Equal(42, result);
			}

			[Fact]
			public async Task RunsCorrectProject()
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
      ""Name"": ""Project1"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""a.fl""
      ]
	},
    {
      ""Name"": ""Project2"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""b.fl""
      ]
	}
	]
}"
					},
					{
						"a.fl",
						@"Main() : int { return 41; };"
					},
					{
						"b.fl",
						@"Main() : int { return 42; }"
					}
					});

				var builder = CreateBuilder(fileSystem);
				using var container = builder.Build();
				var compiler = container.Resolve<FluentLangCompiler>();
				await compiler.Build("solutionFile", outputDirectory: "", outputCSharp: false, test: false);
				Assert.Equal(41, await compiler.Run("solutionFile", "Project1"));
				Assert.Equal(42, await compiler.Run("solutionFile", "Project2"));
			}

			[Fact]
			public async Task DoesNotStopIfIndependantProjectFailsToCompile()
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
      ""Name"": ""Project1"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""a.fl""
      ]
	},
    {
      ""Name"": ""Project2"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""b.fl""
      ]
	}
	]
}"
					},
					{
						"a.fl",
						@"M(}{};"
					},
					{
						"b.fl",
						@"Main() : int { return 42; }"
					}
					});

				var builder = CreateBuilder(fileSystem);
				using var container = builder.Build();
				var compiler = container.Resolve<FluentLangCompiler>();
				await compiler.Build("solutionFile", outputDirectory: "", outputCSharp: false, test: false);
				Assert.Equal(42, await compiler.Run("solutionFile", "Project2"));
			}

			[Fact]
			public async Task StopsIfDependantProjectFailsToCompile()
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
      ""Name"": ""Project1"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""a.fl""
      ]
	},
    {
      ""Name"": ""Project2"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""b.fl""
      ],
	  ""References"": [
        {
          ""Type"": ""Project"",
          ""Name"": ""Project1""
        }
	  ]
	}
	]
}"
					},
					{
						"a.fl",
						@"M(}{};"
					},
					{
						"b.fl",
						@"Main() : int { return 42; }"
					}
					});

				var builder = CreateBuilder(fileSystem);
				using var container = builder.Build();
				var compiler = container.Resolve<FluentLangCompiler>();
				await compiler.Build("solutionFile", outputDirectory: "", outputCSharp: false, test: false);
				Assert.Null(await compiler.Run("solutionFile", "Project2"));
			}

			[Fact]
			public async Task SucceedsIfTestsSucceed()
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
      ""Name"": ""Project1"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""a.fl""
      ]
	},
    {
      ""Name"": ""Project2"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""b.fl""
      ],
	  ""References"": [
        {
          ""Type"": ""Project"",
          ""Name"": ""Project1""
        }
	  ],
	  ""IsTest"": true
	}
	]
}"
					},
					{
						"a.fl",
						@"export M(): bool { return true; };"
					},
					{
						"b.fl",
						@"export IsTrue() : bool { return M() == true; }"
					}
					});

				var builder = CreateBuilder(fileSystem);
				using var container = builder.Build();
				var compiler = container.Resolve<FluentLangCompiler>();
				Assert.True(await compiler.Build("solutionFile", outputDirectory: "", outputCSharp: false, test: true));
			}

			[Fact]
			public async Task FailsIfTestsFail()
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
      ""Name"": ""Project1"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""a.fl""
      ]
	},
    {
      ""Name"": ""Project2"",
      ""Version"": { ""Major"": 0, ""Minor"": 0 },
      ""IncludedFilesAndFolders"": [
        ""b.fl""
      ],
	  ""References"": [
        {
          ""Type"": ""Project"",
          ""Name"": ""Project1""
        }
	  ],
	  ""IsTest"": true
	}
	]
}"
					},
					{
						"a.fl",
						@"export M(): bool { return true; };"
					},
					{
						"b.fl",
						@"export IsTrue() : bool { return M() == false; }"
					}
					});

				var builder = CreateBuilder(fileSystem);
				using var container = builder.Build();
				var compiler = container.Resolve<FluentLangCompiler>();
				Assert.False(await compiler.Build("solutionFile", outputDirectory: "", outputCSharp: false, test: true));
			}

			public RunTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}
		}

		private static ContainerBuilder CreateBuilder(MockFileSystem fileSystem)
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule(new FlcModule(LogLevel.Trace));
			builder.RegisterInstance(fileSystem).As<IFileSystem>();
			return builder;
		}
	}
}
