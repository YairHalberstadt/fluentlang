using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal class DeclaredLocal : IDeclaredLocal
	{
		public DeclaredLocal(IDeclarationStatement declaration)
		{
			Declaration = declaration;
		}

		public IDeclarationStatement Declaration { get; }

		public string Identifier => Declaration.IdentifierName;

		public IType Type => Declaration.Type;
	}
}
