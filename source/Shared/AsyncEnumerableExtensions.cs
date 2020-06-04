using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentLang.Shared
{
	public static class AsyncEnumerableExtensions
	{
		public static async ValueTask<ImmutableArray<T>> ToImmutableArrayAsync<T>(this IAsyncEnumerable<T> @this, CancellationToken cancellationToken = default)
		{
			return (await @this.ToArrayAsync(cancellationToken)).UnsafeAsImmutableArray();
		}
	}
}
