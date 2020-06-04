using System;
using System.Collections.Immutable;
using System.Numerics;
using System.Text;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Shared;
using static FluentLang.Compiler.Generated.FluentLangParser;
using static FluentLang.Compiler.Symbols.Source.MethodBody.CharParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal sealed class LiteralExpression : SymbolBase, ILiteralExpression
	{
		private readonly Literal_expressionContext _context;
		private readonly Lazy<(Primitive type, object? value)> _bind;

		public LiteralExpression(Literal_expressionContext context, DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;

			_bind = new Lazy<(Primitive type, object? value)>(Bind);
		}

		private (Primitive type, object? value) Bind()
		{
			var token = _context.literal().Start;
			return token.Type switch
			{
				LITERAL_TRUE => (Primitive.Bool, true),
				LITERAL_FALSE => (Primitive.Bool, false),
				INTEGER_LITERAL => (Primitive.Int, ParseInteger(token.Text)),
				REAL_LITERAL => (Primitive.Double, ParseDouble(token.Text)),
				CHARACTER_LITERAL => (Primitive.Char, ParseChar(token.Text)),
				REGULAR_STRING => (Primitive.String, ParseString(token.Text)),
				var type => throw new InvalidOperationException($"Invalid Literal Token Type {type}")
			};
		}

		private object? ParseInteger(string text)
		{
			if (!BigInteger.TryParse(text, out var number))
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.literal()),
					ErrorCode.InvalidIntegerLiteral,
					ImmutableArray.Create<object?>(text)));

				return null;
			}
			else if (number > int.MaxValue || number < int.MinValue)
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.literal()),
					ErrorCode.IntegerLiteralOutOfRange,
					ImmutableArray.Create<object?>(number)));

				return null;
			}
			return (int)number;

		}

		private object? ParseDouble(string text)
		{
			if (!double.TryParse(text, out var number))
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.literal()),
					ErrorCode.InvalidRealLiteral,
					ImmutableArray.Create<object?>(text)));

				return null;
			}
			return number;
		}

		private object? ParseChar(string text)
		{
			Release.Assert(text[0] == '\'' && text[^1] == '\'');

			if (ParseCharResult.Succeeded != TryParseNextChar(
				text.AsSpan()[1..^1],
				out _,
				out var c))
			{
				_diagnostics.Add(new Diagnostic(
					new Location(_context.literal()),
					ErrorCode.InvalidCharLiteral,
					ImmutableArray.Create<object?>(text)));
				return null;
			}

			return c;
		}

		private string ParseString(string text)
		{
			Release.Assert(text[0] == '\"' && text[^1] == '\"');
			var span = text.AsSpan()[1..^1];
			var totalCharsConsumed = 0;
			var builder = new StringBuilder(text.Length);
			while(!span.IsEmpty)
			{
				var result = TryParseNextChar(span, out var charsConsumed, out var parsedChar);
				if (result == ParseCharResult.InvalidEscapeSequence)
				{
					_diagnostics.Add(new Diagnostic(
						new Location(
							_context.literal().Start,
							totalCharsConsumed..(totalCharsConsumed + charsConsumed)),
						ErrorCode.InvalidEscapeSequence));
				}
				else
				{
					builder.Append(parsedChar);
				}
				totalCharsConsumed += charsConsumed;
				span = span[charsConsumed..];
			}
			return builder.ToString();
		}

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			_ = _bind.Value;
		}

		public Primitive Type => _bind.Value.type;

		public object? Value => _bind.Value.value;

		IType IExpression.Type => Type;
	}
}
