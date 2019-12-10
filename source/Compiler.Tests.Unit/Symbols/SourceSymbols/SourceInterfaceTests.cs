using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols
{
	public class SourceInterfaceTests : TestBase
	{
		public SourceInterfaceTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void InterfaceSymbolHasIsExportTrueOnlyIfItHasExportModifier()
		{
			var assembly = CreateAssembly(@"
export interface I1 {}
interface I2 {}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var i1 = AssertGetInterface(assembly, "I1");
			Assert.True(i1.IsExported);
			var i2 = AssertGetInterface(assembly, "I2");
			Assert.False(i2.IsExported);
		}
	}
}
