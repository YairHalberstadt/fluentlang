using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace FluentLang.TestUtils
{
	public class XunitLogger<T> : ILogger<T>, IDisposable
	{
		private readonly ITestOutputHelper _output;

		public XunitLogger(ITestOutputHelper output)
		{
			_output = output;
		}
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) where TState : notnull
		{
			_output.WriteLine($"{logLevel.ToString().ToUpper()}: {formatter(state, exception)}");
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return this;
		}

		public void Dispose()
		{
		}
	}
}
