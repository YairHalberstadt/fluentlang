using Microsoft.Extensions.Logging;
using StrongInject;

namespace FluentLang.flc.DependencyInjection
{
	[RegisterModule(typeof(FlcModule))]
	public partial class FlcContainer : IAsyncContainer<FluentLangCompiler>
	{
		[Instance] private readonly LogLevel _logLevel;

		public FlcContainer(LogLevel logLevel)
		{
			_logLevel = logLevel;
		}
	}
}
