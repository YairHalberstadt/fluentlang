namespace FluentLang.Compiler.Diagnostics
{
	public enum ErrorCode
	{
		SyntaxError,
		DuplicateInterfaceDeclaration,
		DuplicateMethodDeclaration,
		InterfaceNotFound,
		AmbigiousReference,
		CannotReferenceSelfAsAdditiveInterface,
		InvalidParseTree,
	}
}
