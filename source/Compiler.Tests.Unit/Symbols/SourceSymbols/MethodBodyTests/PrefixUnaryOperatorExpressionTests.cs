using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Source.MethodBody;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class PrefixUnaryOperatorExpressionTests
	{
		public class MinusTests : TestBase
		{
			public MinusTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
			{
			}

			[Fact]
			public void CanMinusInt()
			{
				var assembly = CreateAssembly(@"
M(param : int) : int { return -param; }")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<PrefixUnaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Minus, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Int, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CanMinusDouble()
			{
				var assembly = CreateAssembly(@"
M(param : double) : double { return -param; }")
					.VerifyDiagnostics().VerifyEmit(_testOutputHelper);

				var m = AssertGetMethod(assembly, "M");
				var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
				var binaryOperatorExpression = Assert.IsAssignableFrom<PrefixUnaryOperatorExpression>(returnStatement.Expression);
				Assert.Equal(Operator.Minus, binaryOperatorExpression.Operator);
				Assert.Equal(Primitive.Double, binaryOperatorExpression.Type);
			}

			[Fact]
			public void CantMinusChar()
			{
				CreateAssembly(@"
M(param : char) : char { return -param; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"param")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantMinusBool()
			{
				CreateAssembly(@"
M(param : bool) : bool { return -param; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"param")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantMinusString()
			{
				CreateAssembly(@"
M(param : string) : string { return -param; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"param")), ErrorCode.InvalidArgument));
			}

			[Fact]
			public void CantMinusInterface()
			{
				CreateAssembly(@"
M(param : {}) : {} { return -param; }")
					.VerifyDiagnostics(
						new Diagnostic(new Location(new TextToken(@"param")), ErrorCode.InvalidArgument));
			}
		}
	}
}
