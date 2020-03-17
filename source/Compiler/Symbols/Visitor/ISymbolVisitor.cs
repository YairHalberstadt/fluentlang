using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Symbols.Visitor
{
	public interface ISymbolVisitor<T>
	{
		[return: MaybeNull] T Visit(IBinaryOperatorExpression binaryOperatorExpression);
		[return: MaybeNull] T Visit(IMixinPatch mixinPatch);
		[return: MaybeNull] T Visit(IMethodPatch methodPatch);
		[return: MaybeNull] T Visit(IStaticInvocationExpression staticInvocationExpression);
		[return: MaybeNull] T Visit(IParameter parameter);
		[return: MaybeNull] T Visit(ITypeParameter typeParameter);
		[return: MaybeNull] T Visit(IInterfaceMethod interfaceMethod);
		[return: MaybeNull] T Visit(IUnion union);
		[return: MaybeNull] T Visit(IInterface @interface);
		[return: MaybeNull] T Visit(IMatchExpression matchExpression);
		[return: MaybeNull] T Visit(Primitive primitive);
		[return: MaybeNull] T Visit(IMethod method);
		[return: MaybeNull] T Visit(IReturnStatement returnStatement);
		[return: MaybeNull] T Visit(IPrefixUnaryOperatorExpression prefixUnaryOperatorExpression);
		[return: MaybeNull] T Visit(IMatchExpressionArm matchExpressionArm);
		[return: MaybeNull] T Visit(INewObjectExpression newObjectExpression);
		[return: MaybeNull] T Visit(IObjectPatchingExpression objectPatchingExpression);
		[return: MaybeNull] T Visit(IMemberInvocationExpression memberInvocationExpression);
		[return: MaybeNull] T Visit(ILocalReferenceExpression localReferenceExpression);
		[return: MaybeNull] T Visit(ILiteralExpression literalExpression);
		[return: MaybeNull] T Visit(IDeclarationStatement declarationStatement);
		[return: MaybeNull] T Visit(IConditionalExpression conditionalExpression);
	}
}
