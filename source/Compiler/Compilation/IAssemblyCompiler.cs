using FluentLang.Compiler.Symbols.Interfaces;
using System.IO;

namespace FluentLang.Compiler.Compilation
{
	public interface IAssemblyCompiler
	{
		CompilationResult CompileAssembly(
			IAssembly assembly,
			Stream outputStream,
			Stream? csharpOutputStream = null,
			Stream? pdbStream = null);
	}
}