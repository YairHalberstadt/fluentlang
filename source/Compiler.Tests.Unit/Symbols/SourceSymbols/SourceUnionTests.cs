using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Source.MethodBody;
using FluentLang.TestUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols
{
	public class SourceUnionTests : TestBase
	{
		public SourceUnionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void CanParseUnion()
		{
			var assembly = CreateAssembly("M() : int | {} { return 42; }").VerifyDiagnostics().VerifyEmit();
			var m = AssertGetMethod(assembly, "M");
			var union = Assert.IsAssignableFrom<IUnion>(m.ReturnType);
			Assert.Equal(Primitive.Int, union.Options[0]);
			Assert.True(union.Options[1].IsEquivalentTo(EmptyInterface.Instance));
		}
	}
}
