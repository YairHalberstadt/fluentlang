using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Emit;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Metadata;
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

		public static IAssembly VerifyEmit(
			this IAssembly assembly,
			ITestOutputHelper testOutputHelper,
			string? expectedCSharp = null,
			object? expectedResult = null,
			Action<IAssembly, Assembly>? testEmittedAssembly = null)
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
				var actual = reader.ReadToEnd();
				Assert.True(
					expectedCSharp == actual,
					"expected:\n" + expectedCSharp + "\n\nactual:\n" + actual);
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
			var emittedAssembly = assemblyLoadContext.LoadFromStream(ilStream);

			if (expectedResult is { })
			{
				Assert.NotNull(emittedAssembly.EntryPoint);
				Assert.Equal(expectedResult, emittedAssembly.EntryPoint!.Invoke(null, null));
			}

			testEmittedAssembly?.Invoke(assembly, emittedAssembly);

			assemblyLoadContext.Unload();

			return assembly;
		}

		public static void VerifyMetadata(IAssembly assembly, Assembly emmitedAssembly)
		{
			var metadataAssembly = new MetadataAssembly(emmitedAssembly);
			metadataAssembly.VerifyDiagnostics();

			var exportedMethods = 
				assembly
				.Methods
				.Where(x => x.IsExported)
				.ToDictionary(x => x.FullyQualifiedName);
			var metadataMethods = metadataAssembly.Methods;
			Assert.Equal(exportedMethods.Count, metadataMethods.Length);

			foreach(var metadataMethod in metadataMethods)
			{
				Assert.True(exportedMethods.TryGetValue(
					metadataMethod.FullyQualifiedName, 
					out var exportedMethod));
				Assert.True(metadataMethod.ReturnType.IsEquivalentTo(exportedMethod!.ReturnType));
				Assert.Equal(exportedMethod.Parameters.Length, metadataMethod.Parameters.Length);
				foreach(var (exportedParam, metadataParam) in 
					exportedMethod.Parameters.Zip(metadataMethod.Parameters))
				{
					Assert.Equal(exportedParam.Name, metadataParam.Name);
					Assert.True(metadataParam.Type.IsEquivalentTo(exportedParam.Type));
				}
			}

			var exportedInterfaces = 
				assembly
				.Interfaces
				.Where(x => x.IsExported)
				.ToDictionary(x => x.FullyQualifiedName);
			var metadataInterfaces = metadataAssembly.Interfaces;
			Assert.Equal(exportedInterfaces.Count, metadataInterfaces.Length);

			foreach (var metadataInterface in metadataInterfaces)
			{
				Assert.True(exportedInterfaces.TryGetValue(
					metadataInterface.FullyQualifiedName,
					out var exportedInterface));
				Assert.True(metadataInterface.IsEquivalentTo(exportedInterface!));
			}
		}
	}
}
