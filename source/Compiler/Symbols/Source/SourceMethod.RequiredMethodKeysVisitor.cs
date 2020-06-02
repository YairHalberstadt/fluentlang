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
		private class DirectlyRequiredMethodKeysVisitor : BaseSymbolVisitor<object>
		{
			public HashSet<MethodOrInterfaceMethod>? _methods;
			private readonly IMethod _method;
			private HashSet<MethodOrInterfaceMethod>? _alreadyRequiredByLocalMethods;

			public DirectlyRequiredMethodKeysVisitor(IMethod method)
			{
				_method = method;
			}

			[return: MaybeNull]
			public override object Visit(IMethod method)
			{
				if (method != _method)
				{
					(_alreadyRequiredByLocalMethods ??= new HashSet<MethodOrInterfaceMethod>())
						.UnionWith(method.DirectlyRequiredMethodKeys);
					// no need to remove them afterwards as they must contain Type Parameters
					// defined in the local method,
					// so cannot appear outside it.
				}

				VisitMany(method.LocalMethods);
				return VisitMany(method.Statements);
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
				AddMethodIfNecessary(staticInvocationExpression.Method, staticInvocationExpression.TypeArguments);

				return base.Visit(staticInvocationExpression);
			}

			[return: MaybeNull]
			public override object Visit(IObjectPatchingExpression objectPatchingExpression)
			{
				foreach (var patch in objectPatchingExpression.Patches)
				{
					if (patch is IMixinPatch { Expression: { Type: IInterface mixedInInterface } })
					{
						foreach (var method in mixedInInterface.Methods)
						{
							AddInterfaceMethodIfNecessary(method);
						}
					}
					else if (patch is IMethodPatch { Method: var method, TypeArguments: var typeArguments })
					{
						AddMethodIfNecessary(method, typeArguments);
					}
				}
				return base.Visit(objectPatchingExpression);
			}

			private void AddMethodIfNecessary(IMethod method, ImmutableArray<IType> typeArguments)
			{
				var tps = GetTypeParameters(method);
				if (tps.Any(x => _method.TypeParameters.Contains(x)))
				{
					var requiredMethod = new MethodOrInterfaceMethod(method, typeArguments);
					if (!_alreadyRequiredByLocalMethods?.Contains(requiredMethod) ?? true)
					{
						(_methods ??= new HashSet<MethodOrInterfaceMethod>())
							.Add(requiredMethod);
					}
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
				public HashSet<IType> _visited = new HashSet<IType>();

				[return: MaybeNull]
				protected override object DefaultVisit(IVisitableSymbol symbol)
				{
					if (symbol is IType type)
					{
						if (_visited.Contains(type))
						{
							return default;
						}
						else
						{
							_visited.Add(type);
						}
					}
					return base.DefaultVisit(symbol);
				}

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