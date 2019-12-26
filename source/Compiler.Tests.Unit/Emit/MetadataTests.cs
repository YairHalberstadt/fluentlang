using FluentLang.Compiler.Tests.Unit.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Emit
{
	public class MetadataTests : TestBase
	{
		public MetadataTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void CorrectMetadataEmittedForMethodReturningPrimitive()
		{
			CreateAssembly("export M() : int { return 5; }")
				.VerifyDiagnostics()
				.VerifyEmit(_testOutputHelper);
		}

		[Fact]
		public void CorrectMetadataEmittedForMethodAcceptingAndReturningInterface()
		{
			CreateAssembly("export M(a: { M(a : {}) : int; }) : { M(a : {}) : int; }  { return a; }")
				.VerifyDiagnostics()
				.VerifyEmit(_testOutputHelper);
		}

		[Fact]
		public void CorrectMetadataEmittedForMethodWithMultipleParameters()
		{
			CreateAssembly("export M(a: int, b: bool) : string  { return \"\"; }")
				.VerifyDiagnostics()
				.VerifyEmit(_testOutputHelper);
		}

		[Fact]
		public void CorrectMetadataEmittedForInterface()
		{
			CreateAssembly("export interface I { M() : int; }")
				.VerifyDiagnostics()
				.VerifyEmit(_testOutputHelper);
		}

		[Fact]
		public void CorrectMetadataEmittedForInterfaceReferencingExportedInterface()
		{
			CreateAssembly("export interface I { M() : I1; } export interface I1 {}")
				.VerifyDiagnostics()
				.VerifyEmit(_testOutputHelper);
		}

		[Fact]
		public void CorrectMetadataEmittedForInterfaceReferencingItself()
		{
			CreateAssembly("export interface I { M() : I; }")
				.VerifyDiagnostics()
				.VerifyEmit(_testOutputHelper);
		}

		[Fact]
		public void CorrectMetadataEmittedForMethodReferencingExportedInterfaceInParameter()
		{
			CreateAssembly(@"
export interface I { }
export M(a : I, b : { M() : I; }) : int { return 42; }")
				.VerifyDiagnostics()
				.VerifyEmit(_testOutputHelper);
		}

		[Fact]
		public void CorrectMetadataEmittedForMethodReferencingExportedInterfaceInReturnType()
		{
			CreateAssembly(@"
export interface I { }
export M() : I { return {}; }")
				.VerifyDiagnostics()
				.VerifyEmit(_testOutputHelper);
		}
	}
}
