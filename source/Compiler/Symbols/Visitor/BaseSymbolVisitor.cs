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
		protected abstract T MergeResults(IEnumerable<T> results);

		[return: MaybeNull]
		protected virtual T DefaultVisit<TSymbol>(TSymbol symbol, Func<TSymbol, IEnumerable<T>> visitChildren) where TSymbol : ISymbol
		{
			return MergeResults(visitChildren(symbol));
		}

		[return: MaybeNull] 
		public virtual T Visit(IMethod method)
		{
			return DefaultVisit(
				method,
				x => 
					x.Parameters.Select(x => x.Visit(this))
					.Append(x.ReturnType.Visit(this))
					.Concat(x.Statements.Select(s => s.Visit(this))));
		}

		[return: MaybeNull]
		public virtual T Visit(IBinaryOperatorExpression binaryOperatorExpression)
		{
			return DefaultVisit(
				binaryOperatorExpression,
				x => new[] 
				{ 
					x.Left.Visit(this), 
					x.Right.Visit(this) 
				});
		}

		[return: MaybeNull]
		public virtual T Visit(IStaticInvocationExpression staticInvocationExpression)
		{
			return DefaultVisit(
				staticInvocationExpression,
				x => x.Arguments.Select(a => a.Visit(this)));
		}

		[return: MaybeNull]
		public virtual T Visit(IReturnStatement returnStatement)
		{
			return DefaultVisit(
				returnStatement,
				x => new[] { x.Expression.Visit(this) });
		}

		[return: MaybeNull]
		public virtual T Visit(IPrefixUnaryOperatorExpression prefixUnaryOperatorExpression)
		{
			return DefaultVisit(
				prefixUnaryOperatorExpression,
				x => new[] { x.Expression.Visit(this) });
		}

		[return: MaybeNull]
		public virtual T Visit(INewObjectExpression newObjectExpression)
		{
			return DefaultVisit(
				newObjectExpression,
				x => new T[0]);
		}

		[return: MaybeNull]
		public virtual T Visit(IObjectPatchingExpression objectPatchingExpression)
		{
			return DefaultVisit(
				objectPatchingExpression,
				x => x
					.Patches
					.Select(p => p.MixedInExpression)
					.Where(m => m != null)
					.Select(m => m!.Visit(this))
					.Prepend(x.Expression.Visit(this)));
		}

		[return: MaybeNull]
		public virtual T Visit(IMemberInvocationExpression memberInvocationExpression)
		{
			return DefaultVisit(
				memberInvocationExpression,
				x => x
					.Arguments
					.Select(a => a.Visit(this))
					.Prepend(x.Expression.Visit(this)));
		}

		[return: MaybeNull]
		public virtual T Visit(ILocalReferenceExpression localReferenceExpression)
		{
			return DefaultVisit(
				localReferenceExpression,
				x => new T[0]);
		}

		[return: MaybeNull]
		public virtual T Visit(ILiteralExpression literalExpression)
		{
			return DefaultVisit(
				literalExpression,
				x => new T[0]);
		}

		[return: MaybeNull]
		public virtual T Visit(IDeclarationStatement declarationStatement)
		{
			return DefaultVisit(
				declarationStatement,
				x => new[] { x.Expression.Visit(this) });
		}

		[return: MaybeNull]
		public virtual T Visit(IConditionalExpression conditionalExpression)
		{
			return DefaultVisit(
				conditionalExpression,
				x => new[] { 
					x.Condition.Visit(this),
					x.IfTrue.Visit(this),
					x.IfFalse.Visit(this)});
		}

		[return: MaybeNull]
		public virtual T Visit(IMatchExpression matchExpression)
		{
			return DefaultVisit(
				matchExpression,
				x =>
					x.Arms.Select(a => a.Visit(this))
					.Prepend(x.Expression.Visit(this)));
		}

		[return: MaybeNull]
		public virtual T Visit(IMatchExpressionArm matchExpressionArm)
		{
			return DefaultVisit(
				matchExpressionArm,
				x => new[] { x.Expression.Visit(this) });
		}

		[return: MaybeNull]
		public virtual T Visit(Primitive primitive)
		{
			return DefaultVisit(
				primitive,
				x => new T[0]);
		}

		[return: MaybeNull]
		public virtual T Visit(IUnion union)
		{
			return DefaultVisit(
				union,
				x => x.Options.Select(x => x.Visit(this)));
		}

		[return: MaybeNull]
		public virtual T Visit(IInterface @interface)
		{
			return DefaultVisit(
				@interface,
				x => x.Methods.Select(x => x.Visit(this)));
		}

		[return: MaybeNull]
		public virtual T Visit(IInterfaceMethod method)
		{
			return DefaultVisit(
				method,
				x => x.Parameters.Select(x => x.Visit(this)).Append(x.ReturnType.Visit(this)));
		}

		[return:MaybeNull]
		public virtual T Visit(IParameter parameter)
		{
			return DefaultVisit(
				parameter,
				x => new[] { x.Type.Visit(this) });
		}

		[return: MaybeNull]
		public virtual T Visit(ITypeParameter typeParameter)
		{
			return DefaultVisit(typeParameter, x => new T[0]);
		}
	}
}
