using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace FluentLang.Compiler.Helpers
{
	public static class ArrayExtensions
	{
		public static ImmutableArray<T> UnsafeAsImmutableArray<T>(this T[] array)
		{
			return Unsafe.As<T[], ImmutableArray<T>>(ref array);
		}
	}
}
