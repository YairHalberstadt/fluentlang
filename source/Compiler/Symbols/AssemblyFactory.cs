using FluentLang.Compiler.Compilation;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Metadata;
using FluentLang.Compiler.Symbols.Source;
using System.Collections.Immutable;
using System.Reflection;

namespace FluentLang.Compiler.Symbols
{
	public class AssemblyFactory
	{
		private readonly IAssemblyCompiler _assemblyCompiler;

		public AssemblyFactory(IAssemblyCompiler assemblyCompiler)
		{
			_assemblyCompiler = assemblyCompiler;
		}

		public IAssembly FromSource(
			QualifiedName name,
			(int major, int minor, string? suffix) version,
			ImmutableArray<IAssembly> directlyReferencedAssemblies,
			ImmutableArray<IDocument> documents) => new SourceAssembly(
				name,
				new Version(version.major, version.minor, version.suffix ?? ""),
				directlyReferencedAssemblies,
				documents,
				_assemblyCompiler);

		public IAssembly FromMetadata(
			Assembly assembly,
			ImmutableArray<byte> assemblyBytes,
			ImmutableArray<IAssembly> dependencies) =>
				new MetadataAssembly(assembly, assemblyBytes, dependencies);
	}
}
