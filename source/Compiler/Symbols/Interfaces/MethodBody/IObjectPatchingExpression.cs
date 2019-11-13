using FluentLang.Compiler.Symbols.Source.MethodBody;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IObjectPatchingExpression : IExpression
	{
		IExpression Expression { get; }
		ImmutableArray<IObjectPatch> Patches { get; }
	}
}