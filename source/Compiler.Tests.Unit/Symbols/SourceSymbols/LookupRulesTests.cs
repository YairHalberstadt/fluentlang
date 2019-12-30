using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.ErrorSymbols;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.TestUtils;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols
{
	public class LookupRulesTests : TestBase
	{
		public LookupRulesTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public void CurrentMethodHasHighestPriority()
		{
			var assembly = CreateAssembly(@"
open A;

interface I {}

namespace A {
	interface I {}
}

namespace B {
	interface I {}
	namespace C {
		interface I {}
		M() : int {
			interface I {}
			M() : int {
				interface I {}
				M() : int {
					interface I {}
					M() : I { return {}; }
					return 42;
				}
				return 42;
			}
			return 42;
		}
	}
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var expected = assembly.Methods.Single().LocalMethods.Single().LocalMethods.Single().LocalInterfaces.Single();
			var actual = assembly.Methods.Single().LocalMethods.Single().LocalMethods.Single().LocalMethods.Single().ReturnType;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ParentMethodHasNextHighestPriority()
		{
			var assembly = CreateAssembly(@"
open A;

interface I {}

namespace A {
	interface I {}
}

namespace B {
	interface I {}
	namespace C {
		interface I {}
		M() : int {
			interface I {}
			M() : int {
				interface I {}
				M() : int {
					M() : I { return {}; }
					return 42;
				}
				return 42;
			}
			return 42;
		}
	}
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var expected = assembly.Methods.Single().LocalMethods.Single().LocalInterfaces.Single();
			var actual = assembly.Methods.Single().LocalMethods.Single().LocalMethods.Single().LocalMethods.Single().ReturnType;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ParentsParentsMethodHasNextHighestPriority()
		{
			var assembly = CreateAssembly(@"
open A;

interface I {}

namespace A {
	interface I {}
}

namespace B {
	interface I {}
	namespace C {
		interface I {}
		M() : int {
			interface I {}
			M() : int {
				M() : int {
					M() : I { return {}; }
					return 42;
				}
				return 42;
			}
			return 42;
		}
	}
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var expected = assembly.Methods.Single().LocalInterfaces.Single();
			var actual = assembly.Methods.Single().LocalMethods.Single().LocalMethods.Single().LocalMethods.Single().ReturnType;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void CurrentNamespaceHasNextHighestPriority()
		{
			var assembly = CreateAssembly(@"
open A;

interface I {}

namespace A {
	interface I {}
}

namespace B {
	interface I {}
	namespace C {
		interface I {}
		M() : int {
			M() : int {
				M() : int {
					M() : I { return {}; }
					return 42;
				}
				return 42;
			}
			return 42;
		}
	}
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("B.C.I"), out var expected));
			var actual = assembly.Methods.Single().LocalMethods.Single().LocalMethods.Single().LocalMethods.Single().ReturnType;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ParentNamespaceHasNextHighestPriority()
		{
			var assembly = CreateAssembly(@"
open A;

interface I {}

namespace A {
	interface I {}
}

namespace B {
	interface I {}
	namespace C {
		M() : int {
			M() : int {
				M() : int {
					M() : I { return {}; }
					return 42;
				}
				return 42;
			}
			return 42;
		}
	}
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("B.I"), out var expected));
			var actual = assembly.Methods.Single().LocalMethods.Single().LocalMethods.Single().LocalMethods.Single().ReturnType;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void GlobalNamespaceHasNextHighestPriority()
		{
			var assembly = CreateAssembly(@"
open A;

interface I {}

namespace A {
	interface I {}
}

namespace B {
	namespace C {
		M() : int {
			M() : int {
				M() : int {
					M() : I { return {}; }
					return 42;
				}
				return 42;
			}
			return 42;
		}
	}
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("I"), out var expected));
			var actual = assembly.Methods.Single().LocalMethods.Single().LocalMethods.Single().LocalMethods.Single().ReturnType;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ImportedNamespaceHasLowestPriority()
		{
			var assembly = CreateAssembly(@"
open A;

namespace A {
	interface I {}
}

namespace B {
	namespace C {
		M() : int {
			M() : int {
				M() : int {
					M() : I { return {}; }
					return 42;
				}
				return 42;
			}
			return 42;
		}
	}
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			Assert.True(assembly.TryGetInterface(QualifiedName("A.I"), out var expected));
			var actual = assembly.Methods.Single().LocalMethods.Single().LocalMethods.Single().LocalMethods.Single().ReturnType;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ImportedNamespaceHasToDirectlyContainMember()
		{
			var assembly = CreateAssembly(@"
open A;

namespace A.B {
	interface I {}
}

M() : I { return {}; }").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"I")), ErrorCode.InterfaceNotFound));
			Assert.True(assembly.Methods.Single().ReturnType is IErrorSymbol);
		}

		[Fact]
		public void ErrorIfMultipleMatchingMembers()
		{
			var assembly = CreateAssembly(@"
open A;
open B;

namespace A {
	interface I {}
}

namespace B {
	interface I {}
}

M() : I { return {}; }").VerifyDiagnostics(new Diagnostic(new Location(new TextToken(@"I")), ErrorCode.AmbigiousInterfaceReference));

			Assert.True(assembly.Methods.Single().ReturnType is IErrorSymbol);
		}

		[Fact]
		public void CanFindLocalMethodDeclaredInCurrentMethod()
		{
			var assembly = CreateAssembly(@"
M() : int {
	M1() : int { return 5; }
	return M1();
}").VerifyDiagnostics().VerifyEmit(_testOutputHelper);
			var m = AssertGetMethod(assembly, "M");
			var localMethod = m.LocalMethods.Single();
			var returnStatement = Assert.IsAssignableFrom<IReturnStatement>(m.Statements.Single());
			var invocation = Assert.IsAssignableFrom<IStaticInvocationExpression>(returnStatement.Expression);
			Assert.Equal(localMethod, invocation.Method);

		}
	}
}
