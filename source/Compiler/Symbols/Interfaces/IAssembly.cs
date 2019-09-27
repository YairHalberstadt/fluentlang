using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IAssembly : ISymbol
	{
		public QualifiedName Name { get; }
		public Version Version { get; }
		public ImmutableArray<IAssembly> ReferencedAssemblies { get; }
		public ImmutableArray<IInterface> Interfaces { get; }
		public ImmutableArray<IMethod> Methods { get; }
		public bool TryGetInterface(QualifiedName fullyQualifiedName, [NotNullWhen(true)] out IInterface? @interface);
		public bool TryGetMethod(QualifiedName fullyQualifiedName, [NotNullWhen(true)] out IMethod? method);
	}
}

