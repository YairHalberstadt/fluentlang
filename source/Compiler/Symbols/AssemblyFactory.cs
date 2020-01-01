using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Metadata;
using FluentLang.Compiler.Symbols.Source;
using System.Collections.Immutable;
using System.Reflection;

namespace FluentLang.Compiler.Symbols
{
	public static class AssemblyFactory
	{
		public static IAssembly FromSource(
			QualifiedName name,
			(int major, int minor, string? suffix) version,
			ImmutableArray<IAssembly> directlyReferencedAssemblies,
			ImmutableArray<IDocument> documents) => new SourceAssembly(
				name,
				new Version(version.major, version.minor, version.suffix ?? ""),
				directlyReferencedAssemblies,
				documents);

		public static IAssembly FromMetadata(
			Assembly assembly,
			ImmutableArray<IAssembly> dependencies) =>
				new MetadataAssembly(assembly, dependencies);
	}
}
