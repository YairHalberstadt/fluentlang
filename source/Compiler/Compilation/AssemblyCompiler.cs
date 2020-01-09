using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Emit;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;

namespace FluentLang.Compiler.Compilation
{
	public class AssemblyCompiler : IAssemblyCompiler
	{
		private readonly FluentlangToCSharpEmitter _fluentlangToCSharpEmitter;
		private readonly CSharpToAssemblyCompiler _cSharpToAssemblyCompiler;

		public AssemblyCompiler(
			FluentlangToCSharpEmitter fluentlangToCSharpEmitter,
			CSharpToAssemblyCompiler cSharpToAssemblyCompiler)
		{
			_fluentlangToCSharpEmitter = fluentlangToCSharpEmitter;
			_cSharpToAssemblyCompiler = cSharpToAssemblyCompiler;
		}

		public CompilationResult CompileAssembly(
			IAssembly assembly,
			IEnumerable<Assembly> referencedAssemblies,
			Stream outputStream,
			Stream? csharpOutputStream = null,
			Stream? pdbStream = null)
		{
			var diagnostics = assembly.AllDiagnostics;
			if (diagnostics.Length > 0)
				return new CompilationResult(
					CompilationResultStatus.CodeErrors,
					diagnostics,
					ImmutableArray<Microsoft.CodeAnalysis.Diagnostic>.Empty);

			csharpOutputStream ??= new MemoryStream();
			var writer = new StreamWriter(csharpOutputStream);
			var reader = new StreamReader(csharpOutputStream);

			_fluentlangToCSharpEmitter.Emit(assembly, writer);

			csharpOutputStream.Position = 0;

			var emitResult = _cSharpToAssemblyCompiler.Compile(
				reader,
				assembly,
				referencedAssemblies,
				outputStream,
				pdbStream);

			if (!emitResult.Success)
			{
				return new CompilationResult(
					CompilationResultStatus.InternalErrors,
					ImmutableArray<Diagnostic>.Empty,
					emitResult.Diagnostics);
			}

			return new CompilationResult(
				CompilationResultStatus.Succeeded,
				ImmutableArray<Diagnostic>.Empty,
				emitResult.Diagnostics);
		}
	}
}
