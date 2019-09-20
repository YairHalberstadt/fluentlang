using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.ErrorTypes;
using FluentLang.Compiler.Tests.Unit.TestHelpers;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.Symbols.SourceSymbols
{
	public class LookupRulesTests : TestBase
	{
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
					M() : I {}
				}
			}
		}
	}
}").VerifyDiagnostics();
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
					M() : I {}
				}
			}
		}
	}
}").VerifyDiagnostics();
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
					M() : I {}
				}
			}
		}
	}
}").VerifyDiagnostics();
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
					M() : I {}
				}
			}
		}
	}
}").VerifyDiagnostics();
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
					M() : I {}
				}
			}
		}
	}
}").VerifyDiagnostics();
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
					M() : I {}
				}
			}
		}
	}
}").VerifyDiagnostics();
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
					M() : I {}
				}
			}
		}
	}
}").VerifyDiagnostics();
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

M() : I {}").VerifyDiagnostics(new Diagnostic(new Location(), ErrorCode.InterfaceNotFound));
			Assert.True(assembly.Methods.Single().ReturnType is IErrorType);
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

M() : I {}").VerifyDiagnostics(new Diagnostic(new Location(), ErrorCode.InterfaceNotFound));

			Assert.True(assembly.Methods.Single().ReturnType is IErrorType);
		}
	}
}
