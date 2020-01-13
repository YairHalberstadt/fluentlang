using FluentLang.Compiler.Compilation;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Metadata;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Diagnostic = FluentLang.Compiler.Diagnostics.Diagnostic;

namespace FluentLang.TestUtils
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
			string? expectedCSharp = null,
			object? expectedResult = null,
			Action<IAssembly, Assembly, ImmutableArray<byte>>? testEmittedAssembly = null)
		{
			var compilationResult = assembly.CompileAssembly(
				out var assemblyBytes,
				out var csharpBytes,
				out _);

			if (!compilationResult.AssemblyDiagnostics.IsEmpty)
				throw new InvalidOperationException("cannot emit assembly with errors");

			if (expectedCSharp is { })
			{
				var actual = Encoding.Default.GetString(csharpBytes.UnsafeAsArray());
				Assert.True(
					expectedCSharp == actual,
					"expected:\n" + expectedCSharp + "\n\nactual:\n" + actual);
			}

			if (compilationResult.Status != CompilationResultStatus.Succeeded)
			{
				Assert.False(true,
					"compiling and emitting csharp failed with diagnostics:\n"
					+ string.Join('\n', compilationResult.RoslynDiagnostics)
					+ "\n\ncsharp code was:\n\n"
					+ Encoding.Default.GetString(csharpBytes.UnsafeAsArray()));
			}

			var assemblyLoadContext = new System.Runtime.Loader.AssemblyLoadContext(null, isCollectible: true);
			// verify assembly is valid
			var emittedAssembly = assemblyLoadContext.LoadFromStream(assemblyBytes.ToStream());

			if (expectedResult is { })
			{
				Assert.NotNull(emittedAssembly.EntryPoint);
				Assert.Equal(expectedResult, emittedAssembly.EntryPoint!.Invoke(null, null));
			}

			VerifyMetadata(assembly, emittedAssembly, assemblyBytes);
			testEmittedAssembly?.Invoke(assembly, emittedAssembly, assemblyBytes);

			assemblyLoadContext.Unload();

			return assembly;
		}

		private static void VerifyMetadata(
			IAssembly assembly,
			Assembly emittedAssembly,
			ImmutableArray<byte> bytes)
		{
			var metadataAssembly = new MetadataAssembly(
				emittedAssembly,
				bytes,
				assembly
					.ReferencedAssembliesAndSelf
					.Where(x => x.Name != assembly.Name && x.Version != assembly.Version)
					.ToImmutableArray());
			metadataAssembly.VerifyDiagnostics();

			Assert.Equal(assembly.Name, metadataAssembly.Name);
			Assert.Equal(assembly.Version, metadataAssembly.Version);
			Assert.Equal(
				assembly.ReferencedAssembliesAndSelf.Where(x => x.Name != assembly.Name),
				metadataAssembly.ReferencedAssembliesAndSelf.Where(x => x.Name != metadataAssembly.Name));

			var exportedMethods =
				assembly
				.Methods
				.Where(x => x.IsExported)
				.ToDictionary(x => x.FullyQualifiedName);
			var metadataMethods = metadataAssembly.Methods;
			Assert.Equal(exportedMethods.Count, metadataMethods.Length);

			foreach (var metadataMethod in metadataMethods)
			{
				Assert.True(exportedMethods.TryGetValue(
					metadataMethod.FullyQualifiedName,
					out var exportedMethod));
				Assert.True(metadataMethod.ReturnType.IsEquivalentTo(exportedMethod!.ReturnType));
				Assert.Equal(exportedMethod.Parameters.Length, metadataMethod.Parameters.Length);
				foreach (var (exportedParam, metadataParam) in
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
