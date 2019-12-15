using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System;
using System.Collections.Generic;
using System.Text;
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
				.VerifyEmit(_testOutputHelper, testEmittedAssembly: AssemblyExtensions.VerifyMetadata);
		}

		[Fact]
		public void CorrectMetadataEmittedForMethodAcceptingAndReturningInterface()
		{
			CreateAssembly("export M(a: { M(a : {}) : int; }) : { M(a : {}) : int; }  { return a; }")
				.VerifyDiagnostics()
				.VerifyEmit(_testOutputHelper, testEmittedAssembly: AssemblyExtensions.VerifyMetadata);
		}

		[Fact]
		public void CorrectMetadataEmittedForMethodWithMultipleParameters()
		{
			CreateAssembly("export M(a: int, b: bool) : string  { return \"\"; }")
				.VerifyDiagnostics()
				.VerifyEmit(_testOutputHelper, testEmittedAssembly: AssemblyExtensions.VerifyMetadata);
		}
	}
}
