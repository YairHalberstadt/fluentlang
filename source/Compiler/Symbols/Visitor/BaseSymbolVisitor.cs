using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Visitor
{
	public abstract class BaseSymbolVisitor<T> : ISymbolVisitor<T>
	{
		[return: MaybeNull]
		protected virtual T VisitMany(IEnumerable<IVisitableSymbol> symbols)
		{
			foreach(var symbol in symbols)
			{
				DefaultVisit(symbol);
			}
			return default;
		}

		[return: MaybeNull]
		protected virtual T VisitMany(params IVisitableSymbol[] symbols)
			=> VisitMany(symbols.AsEnumerable());

		[return: MaybeNull]
		protected virtual T DefaultVisit(IVisitableSymbol symbol)
		{
			return symbol.Visit(this);
		}

		[return: MaybeNull] 
		public virtual T Visit(IMethod method)
		{
			return VisitMany(
					method.Parameters.AsEnumerable<IVisitableSymbol>()
					.Append(method.ReturnType)
					.Concat(method.Statements));
		}

		[return: MaybeNull]
		public virtual T Visit(IBinaryOperatorExpression binaryOperatorExpression)
		{
			return VisitMany(binaryOperatorExpression.Left, binaryOperatorExpression.Right);
		}

		[return: MaybeNull]
		public virtual T Visit(IStaticInvocationExpression staticInvocationExpression)
		{
			return VisitMany(
				staticInvocationExpression.Arguments);
		}

		[return: MaybeNull]
		public virtual T Visit(IReturnStatement returnStatement)
		{
			return DefaultVisit(returnStatement.Expression);
		}

		[return: MaybeNull]
		public virtual T Visit(IPrefixUnaryOperatorExpression prefixUnaryOperatorExpression)
		{
			return DefaultVisit(prefixUnaryOperatorExpression.Expression);
		}

		[return: MaybeNull]
		public virtual T Visit(INewObjectExpression newObjectExpression)
		{
			return default;
		}

		[return: MaybeNull]
		public virtual T Visit(IObjectPatchingExpression objectPatchingExpression)
		{
			return VisitMany(
				objectPatchingExpression.Patches.AsEnumerable<IVisitableSymbol>()
					.Prepend(objectPatchingExpression.Expression));
		}

		[return: MaybeNull]
		public virtual T Visit(IMemberInvocationExpression memberInvocationExpression)
		{
			return VisitMany(
				memberInvocationExpression.Arguments
					.Select(a => a)
					.Prepend(memberInvocationExpression.Expression));
		}

		[return: MaybeNull]
		public virtual T Visit(ILocalReferenceExpression localReferenceExpression)
		{
			return default;
		}

		[return: MaybeNull]
		public virtual T Visit(ILiteralExpression literalExpression)
		{
			return default;
		}

		[return: MaybeNull]
		public virtual T Visit(IDeclarationStatement declarationStatement)
		{
			return DefaultVisit(declarationStatement.Expression);
		}

		[return: MaybeNull]
		public virtual T Visit(IConditionalExpression conditionalExpression)
		{
			return VisitMany(
					conditionalExpression.Condition,
					conditionalExpression.IfTrue,
					conditionalExpression.IfFalse);
		}

		[return: MaybeNull]
		public virtual T Visit(IMatchExpression matchExpression)
		{
			return VisitMany(
					matchExpression.Arms.AsEnumerable<IVisitableSymbol>()
					.Prepend(matchExpression.Expression));
		}

		[return: MaybeNull]
		public virtual T Visit(IMatchExpressionArm matchExpressionArm)
		{
			return DefaultVisit(matchExpressionArm.Expression);
		}

		[return: MaybeNull]
		public virtual T Visit(Primitive primitive)
		{
			return default;
		}

		[return: MaybeNull]
		public virtual T Visit(IUnion union)
		{
			return VisitMany(union.Options);
		}

		[return: MaybeNull]
		public virtual T Visit(IInterface @interface)
		{
			return VisitMany(@interface.Methods);
		}

		[return: MaybeNull]
		public virtual T Visit(IInterfaceMethod method)
		{
			return VisitMany(
				method.Parameters.AsEnumerable<IVisitableSymbol>()
				.Append(method.ReturnType));
		}

		[return:MaybeNull]
		public virtual T Visit(IParameter parameter)
		{
			return DefaultVisit(parameter.Type);
		}

		[return: MaybeNull]
		public virtual T Visit(ITypeParameter typeParameter)
		{
			return default;
		}

		[return: MaybeNull]
		public T Visit(IMixinPatch mixinPatch)
		{
			return DefaultVisit(mixinPatch.Expression);
		}

		[return: MaybeNull]
		public T Visit(IMethodPatch methodPatch)
		{
			return default;
		}
	}
}
