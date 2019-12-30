using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FluentLang.flc.Utils
{
	static class AsyncEnumerableExtensions
	{
		public static async ValueTask<ImmutableArray<T>> ToImmutableArrayAsync<T>(this IAsyncEnumerable<T> @this, CancellationToken cancellationToken = default)
		{
			var array = await @this.ToArrayAsync(cancellationToken);
			return Unsafe.As<T[], ImmutableArray<T>>(ref array);
		}
	}
}
