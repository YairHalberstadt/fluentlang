using System;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Helpers
{
	public static class Release
	{
		public static void Assert([DoesNotReturnIf(false)] bool condition)
		{
			Assert(condition, string.Empty);
		}

		public static void Assert([DoesNotReturnIf(false)] bool condition, string? message)
		{
			if (!condition)
				Fail(message);
		}

		/// <summary>
		/// Throws an <see cref="InvalidOperationException"/>
		/// 
		/// Returns an exception so that for flow control analysis you can write `throw Release.Fail()`
		/// However this will never actually occur.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		[DoesNotReturn]
		public static Exception Fail(string? message)
		{
			throw new InvalidOperationException(message);
		}
	}
}
