using FluentLang.Compiler.Symbols.Interfaces;
using System.IO;
using System.Threading;

namespace FluentLang.Compiler.Compilation
{
	public interface IAssemblyCompiler
	{
		CompilationResult CompileAssembly(
			IAssembly assembly,
			Stream outputStream,
			Stream? csharpOutputStream = null,
			Stream? pdbStream = null,
			CancellationToken cancellationToken = default);
	}
}