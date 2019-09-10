using System.Collections.Immutable;

namespace FluentLang.Compiler.Model
{
	public interface IMethod
	{
		QualifiedName FullyQualifiedName { get; }
		public string Name => FullyQualifiedName.Name;
		public QualifiedName? Namespace => FullyQualifiedName.Parent;
		ImmutableArray<Parameter> Parameters { get; }
		TypeKey ReturnType { get; }
		IMethod? Scope { get; }
		ImmutableArray<IInterface> LocalInterfaces { get; }
		ImmutableArray<IMethod> LocalMethods { get; }
	}
}