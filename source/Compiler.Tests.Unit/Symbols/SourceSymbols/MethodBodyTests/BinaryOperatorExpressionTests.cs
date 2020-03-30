using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.TestUtils;
using System;
using System.Linq;

using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class BinaryOperatorExpressionTests
	{
		public class PlusTests : TestBase
		{
			public PlusTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Fact]
			public void CanPlusStrings()
			{
				var assembly = CreateAssembly(@"
M(a : string, b : string) : string { return a + b; }")
					.VerifyDiagnostics().VerifyEmit();

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Plus, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.String, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CanPlusInts()
			{
				var assembly = CreateAssembly(@"
M(a : int, b : int) : int { return a + b; }")
					.VerifyDiagnostics().VerifyEmit();

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Plus, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Int, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CanPlusDoubles()
			{
				var assembly = CreateAssembly(@"
M(a : double, b : double) : double { return a + b; }")
					.VerifyDiagnostics().VerifyEmit();

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Plus, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Double, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CantPlusChar()
			{
				CreateAssembly(@"
M(a : char, b : char) : char { return a + b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantPlusBool()
			{
				CreateAssembly(@"
M(a : bool, b : bool) : bool { return a + b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantPlusInterface()
			{
				CreateAssembly(@"
M(a : {}, b : {}) : {} { return a + b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantPlusTwoDifferentTypes()
			{
				CreateAssembly(@"
M(a : int, b : double) : int { return a + b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}
		}

		public class MinusTests : TestBase
		{
			public MinusTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Fact]
			public void CanMinusInts()
			{
				var assembly = CreateAssembly(@"
M(a : int, b : int) : int { return a - b; }")
					.VerifyDiagnostics().VerifyEmit();

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Minus, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Int, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CanMinusDoubles()
			{
				var assembly = CreateAssembly(@"
M(a : double, b : double) : double { return a - b; }")
					.VerifyDiagnostics().VerifyEmit();

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Minus, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Double, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CantMinusChar()
			{
				CreateAssembly(@"
M(a : char, b : char) : char { return a - b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantMinusBool()
			{
				CreateAssembly(@"
M(a : bool, b : bool) : bool { return a - b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantMinusString()
			{
				CreateAssembly(@"
M(a : string, b : string) : string { return a - b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantMinusInterface()
			{
				CreateAssembly(@"
M(a : {}, b : {}) : {} { return a - b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantMinusTwoDifferentTypes()
			{
				CreateAssembly(@"
M(a : int, b : double) : int { return a - b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}
		}

		public class MultiplyTests : TestBase
		{
			public MultiplyTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Fact]
			public void CanMultiplyInts()
			{
				var assembly = CreateAssembly(@"
M(a : int, b : int) : int { return a * b; }")
					.VerifyDiagnostics().VerifyEmit();

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Multiply, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Int, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CanMultiplyDoubles()
			{
				var assembly = CreateAssembly(@"
M(a : double, b : double) : double { return a * b; }")
					.VerifyDiagnostics().VerifyEmit();

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Multiply, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Double, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CantMultiplyChar()
			{
				CreateAssembly(@"
M(a : char, b : char) : char { return a * b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantMultiplyBool()
			{
				CreateAssembly(@"
M(a : bool, b : bool) : bool { return a * b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantMultiplyString()
			{
				CreateAssembly(@"
M(a : string, b : string) : string { return a * b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantMultiplyInterface()
			{
				CreateAssembly(@"
M(a : {}, b : {}) : {} { return a * b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantMultiplyTwoDifferentTypes()
			{
				CreateAssembly(@"
M(a : int, b : double) : int { return a * b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}
		}

		public class DivideTests : TestBase
		{
			public DivideTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Fact]
			public void CanDivideInts()
			{
				var assembly = CreateAssembly(@"
M(a : int, b : int) : int { return a / b; }")
					.VerifyDiagnostics().VerifyEmit();

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Divide, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Int, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CanDivideDoubles()
			{
				var assembly = CreateAssembly(@"
M(a : double, b : double) : double { return a / b; }")
					.VerifyDiagnostics().VerifyEmit();

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Divide, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Double, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CantDivideChar()
			{
				CreateAssembly(@"
M(a : char, b : char) : char { return a / b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantDivideBool()
			{
				CreateAssembly(@"
M(a : bool, b : bool) : bool { return a / b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantDivideString()
			{
				CreateAssembly(@"
M(a : string, b : string) : string { return a / b; }")
					.VerifyDiagnostics(new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantDivideInterface()
			{
				CreateAssembly(@"
M(a : {}, b : {}) : {} { return a / b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantDivideTwoDifferentTypes()
			{
				CreateAssembly(@"
M(a : int, b : double) : int { return a / b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}
		}

		public class RemainderTests : TestBase
		{
			public RemainderTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Fact]
			public void CanRemainderInts()
			{
				var assembly = CreateAssembly(@"
M(a : int, b : int) : int { return a % b; }")
					.VerifyDiagnostics().VerifyEmit();

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Remainder, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Int, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CantRemainderDouble()
			{
				CreateAssembly(@"
M(a : double, b : double) : double { return a % b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantRemainderChar()
			{
				CreateAssembly(@"
M(a : char, b : char) : char { return a % b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantRemainderBool()
			{
				CreateAssembly(@"
M(a : bool, b : bool) : bool { return a % b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantRemainderString()
			{
				CreateAssembly(@"
M(a : string, b : string) : string { return a % b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantRemainderInterface()
			{
				CreateAssembly(@"
M(a : {}, b : {}) : {} { return a % b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantRemainderTwoDifferentTypes()
			{
				CreateAssembly(@"
M(a : int, b : double) : int { return a % b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}
		}

		public class EqualityOperatorsTests : TestBase
		{
			public EqualityOperatorsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			public static object[][] EqualityOperators { get; } =
			{
				new object[]{ Operator.Equal },
				new object[]{ Operator.GreaterThan },
				new object[]{ Operator.GreaterThanOrEqualTo },
				new object[]{ Operator.LessThan },
				new object[]{ Operator.LessThanOrEqualTo },
				new object[]{ Operator.NotEqual },
			};

			public static Primitive[] Primitives { get; } =
			{
				Primitive.Bool,
				Primitive.Char,
				Primitive.Double,
				Primitive.Int,
				Primitive.String,
			};

			public static object[][] CrossMultiplyEqualityOperatorsAndPrimitives =>
				EqualityOperators.SelectMany(o => Primitives.Select(p => new object[] { o[0], p })).ToArray();

			private string OperatorSymbol(Operator op)
			{
				return op.ToString() switch
				{
					nameof(Operator.Equal) => "==",
					nameof(Operator.GreaterThan) => ">",
					nameof(Operator.GreaterThanOrEqualTo) => ">=",
					nameof(Operator.LessThan) => "<",
					nameof(Operator.LessThanOrEqualTo) => "<=",
					nameof(Operator.NotEqual) => "!=",
					_ => throw new NotImplementedException(),
				};
			}

			[Theory]
			[MemberData(nameof(CrossMultiplyEqualityOperatorsAndPrimitives))]
			public void AllEqualityOperatorsApplyToAllPrimitives(Operator op, Primitive primitive)
			{
				var primitiveString = primitive.ToString().ToLower();
				var assembly = CreateAssembly($@"
M(a : {primitiveString}, b : {primitiveString}) : bool {{ return a {OperatorSymbol(op)} b; }}")
					.VerifyDiagnostics().VerifyEmit();

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(op, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Bool, binaryOperatorExpression.Type);
			}

			[Theory]
			[MemberData(nameof(EqualityOperators))]
			public void AllEqualityOperatorsDontApplyToInterface(Operator op)
			{
				CreateAssembly($@"
M(a : {{}}, b : {{}}) : bool {{ return a {OperatorSymbol(op)} b; }}")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Theory]
			[MemberData(nameof(EqualityOperators))]
			public void CantApplyEqualityOperatorsToTwoDifferentTypes(Operator op)
			{
				CreateAssembly($@"
M(a : int, b : double) : bool {{ return a {OperatorSymbol(op)} b; }}")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}
		}

		public class AndTests : TestBase
		{
			public AndTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Theory]
			[InlineData(true, true)]
			[InlineData(true, false)]
			[InlineData(false, true)]
			[InlineData(false, false)]
			public void CanAndBool(bool a, bool b)
			{
				var assembly = CreateAssembly($@"
Main() : int {{ return if(M({a.ToString().ToLower()}, {b.ToString().ToLower()}) 1 else 0; }}
M(a : bool, b : bool) : bool {{ return a && b; }}")
					.VerifyDiagnostics().VerifyEmit(expectedResult: a && b ? 1 : 0);

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.And, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Bool, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CantAndInt()
			{
				CreateAssembly(@"
M(a : int, b : int) : int { return a && b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantAndDouble()
			{
				CreateAssembly(@"
M(a : double, b : double) : double { return a && b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantAndChar()
			{
				CreateAssembly(@"
M(a : char, b : char) : char { return a && b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantAndString()
			{
				CreateAssembly(@"
M(a : string, b : string) : string { return a && b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantAndInterface()
			{
				CreateAssembly(@"
M(a : {}, b : {}) : {} { return a && b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantAndTwoDifferentTypes()
			{
				CreateAssembly(@"
M(a : int, b : double) : int { return a && b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}
		}

		public class OrTests : TestBase
		{
			public OrTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Theory]
			[InlineData(true, true)]
			[InlineData(true, false)]
			[InlineData(false, true)]
			[InlineData(false, false)]
			public void CanOrBool(bool a, bool b)
			{
				var assembly = CreateAssembly($@"
Main() : int {{ return if(M({a.ToString().ToLower()}, {b.ToString().ToLower()}) 1 else 0; }}
M(a : bool, b : bool) : bool {{ return a || b; }}")
					.VerifyDiagnostics().VerifyEmit(expectedResult: a || b ? 1 : 0);

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<IBinaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Or, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Bool, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CantOrInt()
			{
				CreateAssembly(@"
M(a : int, b : int) : int { return a || b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantOrDouble()
			{
				CreateAssembly(@"
M(a : double, b : double) : double { return a || b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantOrChar()
			{
				CreateAssembly(@"
M(a : char, b : char) : char { return a || b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantOrString()
			{
				CreateAssembly(@"
M(a : string, b : string) : string { return a || b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantOrInterface()
			{
				CreateAssembly(@"
M(a : {}, b : {}) : {} { return a || b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantOrTwoDifferentTypes()
			{
				CreateAssembly(@"
M(a : int, b : double) : int { return a || b; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"a")), ErrorCode.InvalidArgument));
			}
		}
	}
}
