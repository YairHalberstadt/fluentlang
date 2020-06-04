using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Emit
{
	public interface IMetadataReferenceProvider
	{
		public ImmutableArray<MetadataReference> MetadataReferences { get; }
	}
}
