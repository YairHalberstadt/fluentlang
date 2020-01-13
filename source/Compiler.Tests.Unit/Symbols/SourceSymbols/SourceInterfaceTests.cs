using FluentLang.Compiler.Diagnostics;
using FluentLang.TestUtils;
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
interface I2 {}").VerifyDiagnostics().VerifyEmit();
			var i1 = AssertGetInterface(assembly, "I1");
			Assert.True(i1.IsExported);
			var i2 = AssertGetInterface(assembly, "I2");
			Assert.False(i2.IsExported);
		}

		[Fact]
		public void ExportedInterfaceCannotReferenceNonExported()
		{
			CreateAssembly(@"
export interface I1 { M() : I2 }
interface I2 {}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"I2")), ErrorCode.CannotUseUnexportedInterfaceFromExportedMember));
		}

		[Fact]
		public void ExportedInterfaceCanReferenceExported()
		{
			CreateAssembly(@"
export interface I1 { M() : I2 }
export interface I2 {}").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void NonExportedInterfaceCanReferenceExported()
		{
			CreateAssembly(@"
interface I1 { M() : I2 }
export interface I2 {}").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void NonExportedInterfaceCanReferenceNonExported()
		{
			CreateAssembly(@"
interface I1 { M() : I2 }
interface I2 {}").VerifyDiagnostics().VerifyEmit();
		}
	}
}
