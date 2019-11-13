using System.Collections.Immutable;

namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public interface IInvocationExpression : IExpression
	{
		public ImmutableArray<IExpression> Arguments { get; }
	}
}
