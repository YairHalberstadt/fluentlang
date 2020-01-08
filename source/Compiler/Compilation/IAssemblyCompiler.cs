using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FluentLang.Compiler.Compilation
{
	public interface IAssemblyCompiler
	{
		CompilationResult CompileAssembly(
			IAssembly assembly,
			IEnumerable<Assembly> referencedAssemblies,
			Stream outputStream,
			Stream? csharpOutputStream = null,
			Stream? pdbStream = null);
	}
}