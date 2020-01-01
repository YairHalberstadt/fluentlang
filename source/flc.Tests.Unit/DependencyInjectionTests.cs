using FluentLang.flc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit
{
	public class DependencyInjectionTests
	{
		[Fact]
		public void CanResolveCompiler()
		{
			var services = new ServiceCollection();
			DependencyInjection.ConfigureServices(services, LogLevel.Information);
			var compiler = services.BuildServiceProvider().GetRequiredService<FluentLangCompiler>();
			Assert.NotNull(compiler);
		}
	}
}
