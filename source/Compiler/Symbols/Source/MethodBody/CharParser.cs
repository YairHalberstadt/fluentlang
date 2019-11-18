using System;
using FluentLang.Compiler.Helpers;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal static class CharParser
	{
		public enum ParseCharResult
		{
			Succeeded,
			CharsEmpty,
			InvalidEscapeSequence,
		}
		
		public static ParseCharResult TryParseNextChar(
			ReadOnlySpan<char> chars,
			out int charsConsumed,
			out char parsedChar)
		{
			if (chars.IsEmpty)
			{
				charsConsumed = 0;
				parsedChar = default;
				return ParseCharResult.CharsEmpty;
			}

			if (chars[0] != '\\')
			{
				parsedChar = chars[0];
				charsConsumed = 1;
				return ParseCharResult.Succeeded;
			}

			if (chars.Length == 1)
			{
				parsedChar = chars[0];
				charsConsumed = 1;
				return ParseCharResult.InvalidEscapeSequence;
			}

			switch (chars[1])
			{
				// escaped characters that translate to themselves
				case '"':
				case '\\':
				case '\'':
					parsedChar = chars[1];
					goto onParsed;
				case '0':
					parsedChar = '\u0000';
					goto onParsed;
				case 'a':
					parsedChar = '\u0007';
					goto onParsed;
				case 'b':
					parsedChar = '\u0008';
					goto onParsed;
				case 'f':
					parsedChar = '\u000c';
					goto onParsed;
				case 'n':
					parsedChar = '\u000a';
					goto onParsed;
				case 'r':
					parsedChar = '\u000d';
					goto onParsed;
				case 't':
					parsedChar = '\u0009';
					goto onParsed;
				case 'v':
					parsedChar = '\u000b';
onParsed:
					charsConsumed = 2;
					return ParseCharResult.Succeeded;
				case 'u':
					if (chars.Length < 6 
						|| chars[2..4].Any(x => (x < '0' || x > '9') && (x < 'a' || x > 'f')))
					{
						parsedChar = default;
						charsConsumed = Math.Min(6, chars.Length);
						return ParseCharResult.InvalidEscapeSequence;
					}
					parsedChar = (char)int.Parse(
						chars[2..6],
						System.Globalization.NumberStyles.HexNumber);
					charsConsumed = 6;
					return ParseCharResult.Succeeded;
				default:
					parsedChar = default;
					charsConsumed = 2;
					return ParseCharResult.InvalidEscapeSequence;
			}

		}
	}
}
