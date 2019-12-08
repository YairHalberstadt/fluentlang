using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Emit;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public static class AssemblyExtensions
	{
		public static IAssembly VerifyDiagnostics(this IAssembly assembly, params Diagnostic[] expectedDiagnostics)
		{
			var actualDiagnostics = assembly.AllDiagnostics;

			Assert.True(expectedDiagnostics.Length == actualDiagnostics.Length, ErrorMessage());

			Assert.True(
				expectedDiagnostics
					.Zip(
						actualDiagnostics,
						(x, y) =>
							x.ErrorCode == y.ErrorCode
							&& x.Location.GetText().Equals(
								y.Location.GetText(),
								StringComparison.Ordinal))
					.All(x => x),
				ErrorMessage());

			return assembly;

			string ErrorMessage()
			{
				return
					$@"
Expected: 
{string.Join(",\n", expectedDiagnostics.Select(DiagnosticToString))}

Actual:
{string.Join(",\n", actualDiagnostics.Select(DiagnosticToString))}
";
				static string DiagnosticToString(Diagnostic diagnostic)
					=> $"new Diagnostic(new Location(new TextToken(@\"{diagnostic.Location.GetText().ToString()}\")), {nameof(ErrorCode)}.{diagnostic.ErrorCode})";
			}
		}

		public static IAssembly VerifyEmit(this IAssembly assembly, ITestOutputHelper testOutputHelper, string? expectedCSharp = null)
		{
			if (!assembly.AllDiagnostics.IsEmpty)
				throw new InvalidOperationException("cannot emit assembly with errors");

			var csharpStream = new MemoryStream();
			var writer = new StreamWriter(csharpStream);
			var reader = new StreamReader(csharpStream);

			new CSharpEmitter(new MethodKeyGenerator()).Emit(assembly, writer);

			csharpStream.Position = 0;

			if (expectedCSharp is { })
			{
				Assert.Equal(expectedCSharp, reader.ReadToEnd());
				csharpStream.Position = 0;
			}

			var ilStream = new MemoryStream();

			var emitResult = new CSharpAssemblyCompiler(
				new XunitLogger<CSharpAssemblyCompiler>(testOutputHelper)).Compile(reader, assembly, new CompilationOptions(), ilStream, new Assembly[] { });

			if (!emitResult.Success)
			{
				csharpStream.Position = 0;

				Assert.False(true,
					"compiling and emitting csharp failed with diagnostics:\n"
					+ string.Join('\n', emitResult.Diagnostics)
					+ "\n\ncsharp code was:\n\n"
					+ reader.ReadToEnd());
			}

			ilStream.Position = 0;

			var assemblyLoadContext = new System.Runtime.Loader.AssemblyLoadContext(null, isCollectible: true);
			// verify assembly is valid
			assemblyLoadContext.LoadFromStream(ilStream);
			assemblyLoadContext.Unload();

			return assembly;
		}
	}
}
