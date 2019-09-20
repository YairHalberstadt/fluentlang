using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FluentLang.Compiler.Helpers
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<T> Flatten<T>(this T item, Func<T, T?> selectInner) where T : class
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (selectInner is null)
			{
				throw new ArgumentNullException(nameof(selectInner));
			}

			return FlattenInternal(item, selectInner);

			static IEnumerable<T> FlattenInternal(T item, Func<T, T?> selectInner)
			{
			recurse:
				yield return item;
				var next = selectInner(item);
				if (next is null)
					yield break;
				item = next;
				goto recurse;
			}
		}
	}
}
