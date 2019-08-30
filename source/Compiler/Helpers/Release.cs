using System;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Helpers
{
	internal static class Release
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

		[DoesNotReturn]
		public static void Fail(string? message)
		{
			Environment.FailFast(message);
		}
	}
}
