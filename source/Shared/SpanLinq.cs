using System;

namespace FluentLang.Shared
{
	public static class SpanLinq
	{
		public static bool Any<T>(this ReadOnlySpan<T> span, Func<T, bool> func)
		{
			foreach (var t in span)
			{
				if (func(t))
					return true;
			}
			return false;
		}
	}
}
