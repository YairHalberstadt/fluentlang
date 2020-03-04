using System;
using System.Collections.Generic;

namespace FluentLang.Compiler.Emit
{
	internal static class DictionaryExtensions
	{
		public static TValue GetOrAdd<TKey, TValue>(
			this Dictionary<TKey, TValue> dict,
			TKey key,
			Func<TKey, TValue> valueGenerator)
		{
			if (dict.TryGetValue(key, out var value))
			{
				return value;
			}
			var result = valueGenerator(key);
			dict.Add(key, result);
			return result;
		}
	}
}
