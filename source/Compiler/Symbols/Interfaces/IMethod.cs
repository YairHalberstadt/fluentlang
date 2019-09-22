using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IMethod : ISymbol
	{
		public QualifiedName FullyQualifiedName { get; }
		public string Name => FullyQualifiedName.Name;
		public QualifiedName? Namespace => FullyQualifiedName.Parent;
		public IType ReturnType { get; }
		public ImmutableArray<IParameter> Parameters { get; }
		public ImmutableArray<IInterface> LocalInterfaces { get; }
		public ImmutableArray<IMethod> LocalMethods { get; }
		public IMethod? DeclaringMethod { get; }
	}
}

