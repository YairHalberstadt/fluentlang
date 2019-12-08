using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class LiteralExpressionTests
	{
		public class BooleanLiterals : TestBase
		{
			public BooleanLiterals(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Fact]
			public void LiteralTrue()
			{
				var assembly = CreateAssembly(@"M() : bool { return true }")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<ILiteralExpression>(statement.Expression);
				Assert.Equal(Primitive.Bool, exp.Type);
				Assert.Equal(true, exp.Value);
			}

			[Fact]
			public void LiteralFalse()
			{
				var assembly = CreateAssembly(@"M() : bool { return false }")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<ILiteralExpression>(statement.Expression);
				Assert.Equal(Primitive.Bool, exp.Type);
				Assert.Equal(false, exp.Value);
			}

			[Fact]
			public void IsCaseSensitive()
			{
				CreateAssembly(@"M() : bool { return False }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"}")), ErrorCode.SyntaxError));
			}
		}

		public class IntLiterals : TestBase
		{
			public IntLiterals(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Theory]
			[InlineData(0)]
			[InlineData(42)]
			[InlineData(int.MaxValue)]
			public void CorrectlyParsesIntLiterals(int val)
			{
				var assembly = CreateAssembly($"M() : int {{ return {val} }}")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<ILiteralExpression>(statement.Expression);
				Assert.Equal(Primitive.Int, exp.Type);
				Assert.Equal(val, exp.Value);
			}

			[Fact]
			public void IntegerLiteralsCantBeOutOfSigned32BitRange()
			{
				CreateAssembly($"M() : int {{ return {(long)int.MaxValue + 1} }}")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"2147483648")), ErrorCode.IntegerLiteralOutOfRange));
			}
		}

		public class DoubleLiterals : TestBase
		{
			public DoubleLiterals(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Theory]
			[InlineData(0.0, "0.0")]
			[InlineData(42.0, "42.0")]
			[InlineData(99.732, "99.732")]
			[InlineData(0.000123456789123456789123456789123456789123456789, "0.000123456789123456789123456789123456789123456789")]
			[InlineData(double.PositiveInfinity, "200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.0")]
			public void CorrectlyParsesDoubleLiteral(double val, string literal)
			{
				var assembly = CreateAssembly($"M() : double {{ return {literal} }}")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<ILiteralExpression>(statement.Expression);
				Assert.Equal(Primitive.Double, exp.Type);
				Assert.Equal(val, exp.Value);
			}
		}

		public class CharLiterals : TestBase
		{
			public CharLiterals(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Theory]
			[InlineData('a')]
			[InlineData('1')]
			[InlineData('{')]
			[InlineData('"')]
			[InlineData(char.MaxValue)]
			public void CorrectlyParsesSimpleCharLiterals(char val)
			{
				var assembly = CreateAssembly($"M() : char {{ return '{val}' }}")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<ILiteralExpression>(statement.Expression);
				Assert.Equal(Primitive.Char, exp.Type);
				Assert.Equal(val, exp.Value);
			}

			[Theory]
			[InlineData(@"\\",'\\')]
			[InlineData(@"\n", '\n')]
			[InlineData(@"\r", '\r')]
			[InlineData(@"\t",'\t')]
			public void CorrectlyParsesEscapedCharLiterals(string escapeString, char expected)
			{
				var assembly = CreateAssembly($"M() : char {{ return '{escapeString}' }}")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<ILiteralExpression>(statement.Expression);
				Assert.Equal(Primitive.Char, exp.Type);
				Assert.Equal(expected, exp.Value);
			}

			[Theory]
			[InlineData(@"\u0000", '\u0000')]
			[InlineData(@"\u00ff", '\u00ff')]
			[InlineData(@"\uff00", '\uff00')]
			[InlineData(@"\uffff", '\uffff')]
			public void CorrectlyParsesUnicodeEscapedCharLiterals(string escapeString, char expected)
			{
				var assembly = CreateAssembly($"M() : char {{ return '{escapeString}' }}")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<ILiteralExpression>(statement.Expression);
				Assert.Equal(Primitive.Char, exp.Type);
				Assert.Equal(expected, exp.Value);
			}

			[Fact]
			public void ErrorOnEmptyLiteral()
			{
				CreateAssembly(@"M() : char { return '' }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"}")), ErrorCode.SyntaxError));
			}

			[Fact]
			public void ErrorOnEmptyEscapeSequence()
			{
				CreateAssembly(@"M() : char { return '\' }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"}")), ErrorCode.SyntaxError));
			}

			[Fact]
			public void ErrorOnInvalidEscapeSequence()
			{
				CreateAssembly(@"M() : char { return '\z' }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError),
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError));
			}

			[Fact]
			public void ErrorOnInvalidUnicodeEscapeSequence1()
			{
				CreateAssembly(@"M() : char { return '\uaaa' }")
					.VerifyDiagnostics(new Diagnostic(new Location(new TextToken(@"}")), ErrorCode.SyntaxError));
			}

			[Fact]
			public void ErrorOnInvalidUnicodeEscapeSequence2()
			{
				CreateAssembly(@"M() : char { return '\u000g' }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError),
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError));
			}

			[Fact]
			public void ErrorOnTooLongUnicodeEscapeSequence()
			{
				CreateAssembly(@"M() : char { return '\u00000' }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError),
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError));
			}

			[Fact]
			public void ErrorOnTooManyChars()
			{
				CreateAssembly(@"M() : char { return 'aa' }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError),
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError));
			}

			[Fact]
			public void ErrorOnTooManyCharsAFterEscapeSequence()
			{
				CreateAssembly(@"M() : char { return '\na' }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError),
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError));
			}
		}

		public class StringLiterals : TestBase
		{
			public StringLiterals(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Theory]
			[InlineData(@"", "")]
			[InlineData(@"abc", "abc")]
			[InlineData(@"\n\f\a\u0000\'\\", "\n\f\a\u0000\'\\")]
			[InlineData(@"{;?>!@#", "{;?>!@#")]
			public void CanParseStringsCorrectly(string literal, string expected)
			{
				var assembly = CreateAssembly($"M() : string {{ return \"{literal}\" }}")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);
				var m = AssertGetMethod(assembly, "M");
				var statement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var exp = Assert.IsAssignableFrom<ILiteralExpression>(statement.Expression);
				Assert.Equal(Primitive.String, exp.Type);
				Assert.Equal(expected, exp.Value);
			}

			[Fact]
			public void ErrorOnStringWithInvalidEscapeSequence0()
			{
				CreateAssembly(@"M() : string { return ""a\"" }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError),
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError));
			}

			[Fact]
			public void ErrorOnStringWithInvalidEscapeSequence1()
			{
				CreateAssembly(@"M() : string { return ""a\z"" }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError),
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError));
			}

			[Fact]
			public void ErrorOnStringWithInvalidEscapeSequence2()
			{
				CreateAssembly(@"M() : string { return ""a\u000"" }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"}")), ErrorCode.SyntaxError));
			}

			[Fact]
			public void ErrorOnStringWithInvalidEscapeSequence3()
			{
				CreateAssembly(@"M() : string { return ""\u000g"" }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError),
						new Diagnostic(new Location(new TextToken(@"<EOF>")), ErrorCode.SyntaxError));
			}
		}
	}
}
