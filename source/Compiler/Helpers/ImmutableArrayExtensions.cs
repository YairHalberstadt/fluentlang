using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;

namespace FluentLang.Compiler.Helpers
{
	public static class ImmutableArrayExtensions
	{
		public static T[] UnsafeAsArray<T>(this ImmutableArray<T> array)
		{
			return Unsafe.As<ImmutableArray<T>, T[]>(ref array);
		}

		public static Stream ToStream(this ImmutableArray<byte> bytes)
		{
			return new MemoryStream(bytes.UnsafeAsArray(), writable: false);
		}
	}
}
