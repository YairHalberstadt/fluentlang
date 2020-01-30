using System;
using System.Text.RegularExpressions;

namespace FluentLang.Runtime
{
	public class Union
	{
		public Union(object inner, ulong matchingOptions)
		{
			Inner = inner;
			MatchingOptions = matchingOptions;
		}

		public Union(Union union, Span<ulong> matchingTargetOptionsPerOption) 
			: this(union.Inner, union.GetUpcastToUnionMatchingOptions(matchingTargetOptionsPerOption))
		{
		}

		public object Inner { get; }
		public ulong MatchingOptions { get; }

		private ulong GetUpcastToUnionMatchingOptions(Span<ulong> matchingTargetOptionsPerOption)
		{
			ulong bits = 0;
			for (var i = 0; i < matchingTargetOptionsPerOption.Length; i++)
			{
				if ((MatchingOptions & ((ulong)1 << i)) != 0)
				{
					bits |= matchingTargetOptionsPerOption[i];
				}
			}

			return bits;
		}
	}
}
