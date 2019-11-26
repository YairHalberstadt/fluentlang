using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FluentLang.Compiler.Symbols.Visitor
{
	public interface ISymbolVisitor<T>
	{
		[return: MaybeNull] T Visit(IBinaryOperatorExpression binaryOperatorExpression);
		[return: MaybeNull] T Visit(IStaticInvocationExpression staticInvocationExpression);
		[return: MaybeNull] T Visit(IMethod method);
		[return: MaybeNull] T Visit(IReturnStatement returnStatement);
		[return: MaybeNull] T Visit(IPrefixUnaryOperatorExpression prefixUnaryOperatorExpression);
		[return: MaybeNull] T Visit(INewObjectExpression newObjectExpression);
		[return: MaybeNull] T Visit(IObjectPatchingExpression objectPatchingExpression);
		[return: MaybeNull] T Visit(IMemberInvocationExpression memberInvocationExpression);
		[return: MaybeNull] T Visit(ILocalReferenceExpression localReferenceExpression);
		[return: MaybeNull] T Visit(ILiteralExpression literalExpression);
		[return: MaybeNull] T Visit(IDeclarationStatement declarationStatement);
		[return: MaybeNull] T Visit(IConditionalExpression conditionalExpression);
	}
}
