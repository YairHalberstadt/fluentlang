using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IMethodPatch : IObjectPatch
	{
		IMethod Method { get; }
		ImmutableArray<IType> TypeArguments { get; }
	}
}