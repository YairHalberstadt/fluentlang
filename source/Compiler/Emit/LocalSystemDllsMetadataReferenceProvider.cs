using FluentLang.Runtime;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace FluentLang.Compiler.Emit
{
	public class LocalSystemDllsMetadataReferenceProvider : IMetadataReferenceProvider
	{
		private LocalSystemDllsMetadataReferenceProvider()
		{

		}

		public static LocalSystemDllsMetadataReferenceProvider Instance { get; } = new LocalSystemDllsMetadataReferenceProvider();

		public ImmutableArray<MetadataReference> MetadataReferences { get; } =
			Directory.GetFiles(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.*.dll")
			.Concat(Directory.GetFiles(Path.GetDirectoryName(typeof(object).Assembly.Location), "netstandard.dll"))
			.Append(typeof(FLObject).Assembly.Location)
			.Select(x => MetadataReference.CreateFromFile(x))
			.ToImmutableArray<MetadataReference>();
	}
}
