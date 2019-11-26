using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Visitor
{
	public interface IBaseSymbolVisitor<T> : ISymbolVisitor<T>
	{
		[return: MaybeNull]
		public abstract T MergeResults(IEnumerable<T> results);

		[return: MaybeNull]
		public virtual T DefaultVisit<TSymbol>(TSymbol symbol, Func<TSymbol, IEnumerable<T>> visitChildren) where TSymbol : ISymbol
		{
			return MergeResults(visitChildren(symbol));
		}

		[return: MaybeNull] 
		T ISymbolVisitor<T>.Visit(IMethod method)
		{
			return DefaultVisit(
				method,
				x => x.Statements.Select(s => s.Visit(this)));
		}

		[return: MaybeNull]
		T ISymbolVisitor<T>.Visit(IBinaryOperatorExpression binaryOperatorExpression)
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
		T ISymbolVisitor<T>.Visit(IStaticInvocationExpression staticInvocationExpression)
		{
			return DefaultVisit(
				staticInvocationExpression,
				x => x.Arguments.Select(a => a.Visit(this)));
		}

		[return: MaybeNull]
		T ISymbolVisitor<T>.Visit(IReturnStatement returnStatement)
		{
			return DefaultVisit(
				returnStatement,
				x => new[] { x.Expression.Visit(this) });
		}

		[return: MaybeNull]
		T ISymbolVisitor<T>.Visit(IPrefixUnaryOperatorExpression prefixUnaryOperatorExpression)
		{
			return DefaultVisit(
				prefixUnaryOperatorExpression,
				x => new[] { x.Expression.Visit(this) });
		}

		[return: MaybeNull]
		T ISymbolVisitor<T>.Visit(INewObjectExpression newObjectExpression)
		{
			return DefaultVisit(
				newObjectExpression,
				x => new T[0]);
		}

		[return: MaybeNull]
		T ISymbolVisitor<T>.Visit(IObjectPatchingExpression objectPatchingExpression)
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
		T ISymbolVisitor<T>.Visit(IMemberInvocationExpression memberInvocationExpression)
		{
			return DefaultVisit(
				memberInvocationExpression,
				x => x
					.Arguments
					.Select(a => a.Visit(this))
					.Prepend(x.Expression.Visit(this)));
		}

		[return: MaybeNull]
		T ISymbolVisitor<T>.Visit(ILocalReferenceExpression localReferenceExpression)
		{
			return DefaultVisit(
				localReferenceExpression,
				x => new T[0]);
		}

		[return: MaybeNull]
		T ISymbolVisitor<T>.Visit(ILiteralExpression literalExpression)
		{
			return DefaultVisit(
				literalExpression,
				x => new T[0]);
		}

		[return: MaybeNull]
		T ISymbolVisitor<T>.Visit(IDeclarationStatement declarationStatement)
		{
			return DefaultVisit(
				declarationStatement,
				x => new[] { x.Expression.Visit(this) });
		}

		[return: MaybeNull]
		T ISymbolVisitor<T>.Visit(IConditionalExpression conditionalExpression)
		{
			return DefaultVisit(
				conditionalExpression,
				x => new[] { 
					x.Condition.Visit(this),
					x.IfTrue.Visit(this),
					x.IfFalse.Visit(this)});
		}
	}
}
