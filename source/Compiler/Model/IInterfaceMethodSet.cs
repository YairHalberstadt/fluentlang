using System.Collections.Immutable;

namespace FluentLang.Compiler.Model
{
	public interface IInterfaceMethodSet
	{
		ImmutableArray<IInterfaceMethod> Methods { get; }
	}
}