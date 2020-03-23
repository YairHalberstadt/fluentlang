using FluentLang.TestUtils;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols.MethodBodyTests
{
	public class RecursiveGenericCallTests : TestBase
	{
		public RecursiveGenericCallTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void WithSameArgumentsWithoutRequiredMethodKeys()
		{
			CreateAssembly(@"
export Main() : int { 
	return M<int>(5);
}

export M<T>(a : int) : int {
	return M1<T>(a);
}

export M1<T>(a : int) : int {
	return M2<T>(a);
}

export M2<T>(a: int) : int {
	return M3<T>();
	M3<T1>() : int {
		return M4();
		M4() : int {
			return if (a == 0) 42 else M<T1>(a - 1);
		}
	}
}")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void WithDifferentArgumentsWithoutRequiredMethodKeys()
		{
			CreateAssembly(@"
export Main() : int { 
	return M<int>(5);
}

export M<T>(a : int) : int {
	return M1<T>(a);
}

export M1<T>(a : int) : int {
	return M2<T>(a);
}

export M2<T>(a: int) : int {
	return M3<T>();
	M3<T1>() : int {
		return M4();
		M4() : int {
			return if (a == 0) 42 else M<int>(a - 1);
		}
	}
}")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}

		[Fact]
		public void WithDifferentArgumentsWithRequiredMethodKeys()
		{
			CreateAssembly(@"
export Main() : int { 
	return M<int>(5, 42).GetT();
}

export M<T>(a : int, b: T) : { GetT() : T; } {
	return M1<T>(a, b);
}

export M1<T>(a : int, b: T) : { GetT() : T; } {
	return M2<T>(a, b);
}

export M2<T>(a: int, b: T) : { GetT() : T; } {
	return M3<T>(b);
	M3<T1>(c : T1) : { GetT() : T1; } {
		return M4();
		M4() : { GetT() : T1; } {
			_ = if (a == 0) {} else M<int>(a - 1, 0);
			return {} + GetT;
			GetT(d : {}) : T1 {
				return c;
			}
		}
	}
}")
	.VerifyDiagnostics()
	.VerifyEmit(expectedResult: 42);
		}
	}
}
