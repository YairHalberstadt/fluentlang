using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.TestUtils;
using System.Linq;
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
export interface I1 { M() : I2; }
interface I2 {}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"I2")), ErrorCode.CannotUseUnexportedInterfaceFromExportedMember));
		}

		[Fact]
		public void ExportedInterfaceCanReferenceExported()
		{
			CreateAssembly(@"
export interface I1 { M() : I2; }
export interface I2 {}").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void NonExportedInterfaceCanReferenceExported()
		{
			CreateAssembly(@"
interface I1 { M() : I2; }
export interface I2 {}").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void NonExportedInterfaceCanReferenceNonExported()
		{
			CreateAssembly(@"
interface I1 { M() : I2; }
interface I2 {}").VerifyDiagnostics().VerifyEmit();
		}

		[Fact]
		public void TypeParametersAreAvailableInsideInterface()
		{
			var assembly = CreateAssembly(@"interface I<T> { M(a : { M(a : T) : int; }) : T; }")
				.VerifyDiagnostics().VerifyEmit();

			var @interface = AssertGetInterface(assembly, "I");
			var tp = @interface.TypeParameters.SingleOrDefault();
			var m = @interface.Methods.SingleOrDefault();
			Assert.Equal(tp, m.ReturnType);
			var parameterInterface = Assert.IsAssignableFrom<IInterface>(m.Parameters.SingleOrDefault().Type);
			Assert.Equal(tp, parameterInterface.Methods.SingleOrDefault().Parameters.SingleOrDefault().Type);
		}

		[Fact]
		public void TypeParametersHideInterface()
		{
			var assembly = CreateAssembly(@"
interface I<T> { M() : T; }
interface T {}").VerifyDiagnostics().VerifyEmit();

			var @interface = AssertGetInterface(assembly, "I");
			var tp = @interface.TypeParameters.SingleOrDefault();
			var m = @interface.Methods.SingleOrDefault();
			Assert.Equal(tp, m.ReturnType);
		}

		[Fact]
		public void CannotAddTypeParameterToInterface()
		{
			CreateAssembly(@"
interface I<T> { } + T
interface T {}").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"T")), ErrorCode.CanOnlyCombineInterfaces));
		}

		[Fact]
		public void CannotHaveMultipleTypeParametersWithSameName()
		{
			CreateAssembly(@"
interface I<T, T> { }").VerifyDiagnostics(
				new Diagnostic(new Location(new TextToken(@"T")), ErrorCode.TypeParametersShareNames));
		}

		[Fact]
		public void CanHaveMultipleTypeParameters()
		{
			var assembly = CreateAssembly(@"interface I<T1, T2> { M(a : T2) : T1; }")
				.VerifyDiagnostics().VerifyEmit();

			var @interface = AssertGetInterface(assembly, "I");
			var t1 = @interface.TypeParameters.First();
			var t2 = @interface.TypeParameters.Last();
			var m = @interface.Methods.SingleOrDefault();
			Assert.Equal(t1, m.ReturnType);
			Assert.Equal(t2, m.Parameters.SingleOrDefault().Type);
		}

		[Fact]
		public void InterfaceReferenceCanNotHaveTooFewTypeArguments()
		{
			CreateAssembly(@"
interface I1<T1, T2> { M(a : T2) : T1; }
interface I2 I1<int>")
				.VerifyDiagnostics(new Diagnostic(new Location(new TextToken(@"I1<int>")), ErrorCode.TypeNotFound));
		}

		[Fact]
		public void InterfaceReferenceCanNotHaveNoTypeArgumentsWhenInterfaceHasTypeParameters()
		{
			CreateAssembly(@"
interface I1<T1, T2> { M(a : T2) : T1; }
interface I2 I1")
				.VerifyDiagnostics(new Diagnostic(new Location(new TextToken(@"I1")), ErrorCode.TypeNotFound));
		}

		[Fact]
		public void InterfaceReferenceCanNotHaveTooManyTypeArguments()
		{
			CreateAssembly(@"
interface I1<T1, T2> { M(a : T2) : T1; }
interface I2 I1<int, string, {}>")
				.VerifyDiagnostics(new Diagnostic(new Location(new TextToken(@"I1<int,string,{}>")), ErrorCode.TypeNotFound));
		}

		[Fact]
		public void InterfaceReferenceMustMatchAllConstraints()
		{
			CreateAssembly(@"
interface I1<T1 : { M() : int; }, T2> { M(a : T2) : T1; }
interface I2 I1<{}, string>")
				.VerifyDiagnostics(new Diagnostic(new Location(new TextToken(@"I1<{},string>")), ErrorCode.TypeArgumentDoesntMatchConstraints));
		}

		[Fact]
		public void InterfaceReferenceIsValidWhenItMatchesAllConstraints()
		{
			var assembly = CreateAssembly(@"
interface I1<T1 : { M() : int; }, T2> { M(a : T2) : T1; }
interface I2 I1<{ M() : int; M1() : bool; }, string>")
				.VerifyDiagnostics().VerifyEmit();

			var i = AssertGetInterface(assembly, "I2");
			var m = i.Methods.Single();
			var param = m.Parameters.Single();
			Assert.Equal(Primitive.String, param.Type);
		}
	}
}
