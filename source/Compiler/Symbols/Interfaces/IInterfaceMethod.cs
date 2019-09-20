using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IInterfaceMethod
	{
		public string Name { get; }
		public IType ReturnType { get; }
		public ImmutableArray<IParameter> Parameters { get; }

		internal bool IsEquivalentTo(IInterfaceMethod otherMethod, Stack<(IType, IType)>? dependantEqualities)
		{
			if (Name != otherMethod.Name)
				return false;

			if (Parameters.Length != otherMethod.Parameters.Length)
				return false;

			if (!ReturnType.IsEquivalentTo(otherMethod.ReturnType, dependantEqualities))
				return false;

			return Parameters.SequenceEqual(otherMethod.Parameters, (x, y) => x.Type.IsEquivalentTo(y.Type, dependantEqualities));
		}
	}
}

