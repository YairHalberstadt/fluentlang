using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Visitor;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FluentLang.Compiler.Symbols.Source
{
	internal sealed partial class SourceMethod
	{
		private class RequiredMethodKeysVisitor : BaseSymbolVisitor<object>
		{
			public HashSet<MethodOrInterfaceMethod>? _methods;
			private readonly IMethod _method;
			private HashSet<MethodOrInterfaceMethod>? _alreadyRequiredByLocalMethods;

			public RequiredMethodKeysVisitor(IMethod method)
			{
				_method = method;
			}

			[return: MaybeNull]
			public override object Visit(IMethod method)
			{
				if (method != _method)
				{
					(_alreadyRequiredByLocalMethods ??= new HashSet<MethodOrInterfaceMethod>())
						.UnionWith(method.RequiredMethodKeys);
					// no need to remove them afterwards as they must contain Type Parameters
					// defined in the local method,
					// so cannot appear outside it.
				}

				VisitMany(method.LocalMethods);
				return base.Visit(method);
			}

			[return: MaybeNull]
			public override object Visit(IMemberInvocationExpression memberInvocationExpression)
			{
				AddInterfaceMethodIfNecessary(memberInvocationExpression.Method);
				return base.Visit(memberInvocationExpression);
			}

			[return: MaybeNull]
			public override object Visit(IStaticInvocationExpression staticInvocationExpression)
			{
				AddMethodIfNecessary(staticInvocationExpression.TypeArguments, staticInvocationExpression.Method);

				return base.Visit(staticInvocationExpression);
			}

			[return: MaybeNull]
			public override object Visit(IObjectPatchingExpression objectPatchingExpression)
			{
				foreach (var patch in objectPatchingExpression.Patches)
				{
					if (patch is IMixinPatch { Expression : {Type: IInterface mixedInInterface } })
					{
						foreach(var method in mixedInInterface.Methods)
						{
							AddInterfaceMethodIfNecessary(method);
						}
					}
					else if (patch is IMethodPatch { TypeArguments: var typeArguments, Method: var method})
					{
						AddMethodIfNecessary(typeArguments, method);
					}
				}
				return base.Visit(objectPatchingExpression);
			}

			private void AddMethodIfNecessary(ImmutableArray<IType> typeArguments, IMethod method)
			{
				if (typeArguments.Any(
					x => _method.TypeParameters.Contains(x)))
				{
					var requiredMethods = method.RequiredMethodKeys.AsEnumerable();
					if (_alreadyRequiredByLocalMethods != null)
					{
						requiredMethods = requiredMethods
							.Where(x => !_alreadyRequiredByLocalMethods.Contains(x));
					}
					(_methods ??= new HashSet<MethodOrInterfaceMethod>())
						.UnionWith(method.RequiredMethodKeys);
				}
			}

			private void AddInterfaceMethodIfNecessary(IInterfaceMethod method)
			{
				var tps = GetTypeParameters(method);
				if (tps.Any(x => _method.TypeParameters.Contains(x)))
				{
					var requiredMethod = new MethodOrInterfaceMethod(method);
					if (!_alreadyRequiredByLocalMethods?.Contains(requiredMethod) ?? true)
					{
						(_methods ??= new HashSet<MethodOrInterfaceMethod>())
							.Add(requiredMethod);
					}
				}
			}

			public static IEnumerable<ITypeParameter> GetTypeParameters(IMethod method)
			{
				var visitor = new GetTypeParametersInSignatureVisitor();
				visitor.Visit(method);
				return visitor.TypeParameters ?? Enumerable.Empty<ITypeParameter>();
			}

			public static IEnumerable<ITypeParameter> GetTypeParameters(IInterfaceMethod method)
			{
				var visitor = new GetTypeParametersInSignatureVisitor();
				visitor.Visit(method);
				return visitor.TypeParameters ?? Enumerable.Empty<ITypeParameter>();
			}

			private class GetTypeParametersInSignatureVisitor : BaseSymbolVisitor<object>
			{
				public List<ITypeParameter>? TypeParameters;

				[return: MaybeNull]
				public override object Visit(ITypeParameter typeParameter)
				{
					(TypeParameters ??= new List<ITypeParameter>()).Add(typeParameter);
					return null;
				}

				[return: MaybeNull]
				public override object Visit(IMethod method)
				{
					return VisitMany(
						method.Parameters.AsEnumerable<IVisitableSymbol>()
							.Append(method.ReturnType));
				}
			}
		}
	}
}