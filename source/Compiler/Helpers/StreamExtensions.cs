

using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;

namespace FluentLang.Compiler.Helpers
{
	public static class StreamExtensions
	{
		public static ImmutableArray<byte> ToImmutableArray(this MemoryStream stream)
		{
			return stream.ToArray().UnsafeAsImmutableArray();
		}
	}
}
