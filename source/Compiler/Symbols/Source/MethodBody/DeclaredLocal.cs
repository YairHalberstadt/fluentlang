using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal class DeclaredLocal : IDeclaredLocal
	{
		public DeclaredLocal(IDeclarationStatement declaration)
		{
			Declaration = declaration;
			if (Declaration.IdentifierName is null)
				throw new ArgumentNullException(nameof(Declaration) + "." + nameof(Declaration.IdentifierName));
		}

		public IDeclarationStatement Declaration { get; }

		public string Identifier => Declaration.IdentifierName!;

		public IType Type => Declaration.Type;
	}
}
