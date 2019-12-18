using flc;
using FluentLang.flc.ProjectSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;

namespace FluentLang.flc
{
	public static class DependencyInjection
	{
		public static void ConfigureServices(IServiceCollection services, LogLevel logLevel = LogLevel.Information)
		{
			services.AddLogging(builder => builder
				.AddConsole()
				.AddFilter(level => level >= logLevel));
			services.AddSingleton<FluentLangCompiler>();
			services.AddSingleton<IFileSystem, FileSystem>();
			services.AddSingleton<SolutionFactory>();
		}
	}
}
