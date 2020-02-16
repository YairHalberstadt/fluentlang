using FluentLang.Compiler.Symbols.Substituted;
using System.Collections.Generic;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IParameter : ISymbol
	{
		public string Name { get; }
		public IType Type { get; }

		internal IParameter Substitute(IReadOnlyDictionary<ITypeParameter, IType> substitutions)
			=> new SubstitutedParameter(this, substitutions);
	}
}

