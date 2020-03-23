using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FluentLang.Compiler.Emit
{
	public partial class FluentlangToCSharpEmitter
	{
		private class MethodKeyGenerator
		{
			private readonly Dictionary<IInterfaceMethod, List<(IInterfaceMethod method, string paramName)>> _interfaceMethodParamNames = new Dictionary<IInterfaceMethod, List<(IInterfaceMethod, string)>>();
			private readonly Dictionary<IMethod, List<(IMethod method, string paramName)>> _methodParamNames = new Dictionary<IMethod, List<(IMethod, string)>>();
			private readonly Dictionary<IInterfaceMethod, MethodKeyOrParamName.MethodKey> _interfaceMethodCache = new Dictionary<IInterfaceMethod, MethodKeyOrParamName.MethodKey>();
			private readonly Dictionary<IMethod, MethodKeyOrParamName.MethodKey> _methodExcludingFirstParameterCache = new Dictionary<IMethod, MethodKeyOrParamName.MethodKey>();
			private readonly Dictionary<IInterface, string> _interfaceCache = new Dictionary<IInterface, string>();

			private int _paramIndex;

			public IEnumerable<string> EnterMethodAndReturnMethodKeyParamNames(IMethod method)
			{
				var paramNames = new List<string>();
				foreach (var requiredMethod in method.RequiredMethodKeys)
				{
					var paramName = "__p" + _paramIndex++; ;
					if (requiredMethod.TryGetMethod(out var m))
					{
						_methodParamNames.GetOrAdd(m.OriginalDefinition, x => new List<(IMethod, string)>(1)).Add((m, paramName));
					}
					else if (requiredMethod.TryGetInterfaceMethod(out var im))
					{
						_interfaceMethodParamNames.GetOrAdd(im.OriginalDefinition, x => new List<(IInterfaceMethod, string)>(1)).Add((im, paramName));
					}
					else
					{
						throw Release.Fail("RequiredMethodKey must be Method or InterfaceMethod");
					}
					paramNames.Add(paramName);
				}

				return paramNames;
			}

			public void ExitMethod(IMethod method)
			{
				foreach (var requiredMethod in method.RequiredMethodKeys)
				{
					if (requiredMethod.TryGetMethod(out var m))
					{
						var list = _methodParamNames[m.OriginalDefinition];
						list.RemoveAt(list.Count - 1);
					}
					else if (requiredMethod.TryGetInterfaceMethod(out var im))
					{
						var list = _interfaceMethodParamNames[im.OriginalDefinition];
						list.RemoveAt(list.Count - 1);
					}
				}
				_paramIndex -= method.RequiredMethodKeys.Length;
			}

			public MethodKeyOrParamName GenerateMethodKey(IInterfaceMethod method)
			{
				if (_interfaceMethodParamNames.TryGetValue(method.OriginalDefinition, out var list))
				{
					var (_, paramKey) = list.FirstOrDefault(x => x.method == method || x.method.IsEquivalentTo(method, null));
					if (paramKey != null)
						return new MethodKeyOrParamName.ParamName(paramKey);
				}

				return _interfaceMethodCache.GetOrAdd(
					method,
					method => new MethodKeyOrParamName.MethodKey(GenerateMethodKey(method, new Stack<(IInterface @interface, int index)>())));
			}

			public MethodKeyOrParamName GenerateMethodKeyExcludingFirstParameter(IMethod method)
			{
				if (_methodParamNames.TryGetValue(method.OriginalDefinition, out var list))
				{
					var (_, paramKey) = list.FirstOrDefault(x => AreEquivalent(x.method, method));
					if (paramKey != null)
						return new MethodKeyOrParamName.ParamName(paramKey);
				}

				return _methodExcludingFirstParameterCache.GetOrAdd(method, method =>
				{
					var parentInterfaces = new Stack<(IInterface @interface, int index)>();
					return
						new MethodKeyOrParamName.MethodKey(
							method.Name
							+ GenerateTypeKey(method.ReturnType, parentInterfaces)
							+ string.Concat(method.Parameters.Skip(1).Select(x => GenerateTypeKey(x.Type, parentInterfaces))));
				});

				static bool AreEquivalent(IMethod a, IMethod b)
				{
					Release.Assert(a.OriginalDefinition == b.OriginalDefinition);
					if (!a.ReturnType.IsEquivalentTo(b.ReturnType))
						return false;

					return a.Parameters.Zip(b.Parameters, (a, b) => a.Type.IsEquivalentTo(b.Type)).All(x => x);
				}
			}

			private string GenerateInterfaceKey(IInterface @interface, Stack<(IInterface @interface, int index)> parentInterfaces)
			{
				if (parentInterfaces.Count == 0)
					return _interfaceCache.GetOrAdd(@interface, GenerateInterfaceKeyInternal);
				return GenerateInterfaceKeyInternal(@interface);
				string GenerateInterfaceKeyInternal(IInterface @interface)
				{
					var matching = parentInterfaces.FirstOrDefault(x => @interface.IsEquivalentTo(x.@interface));
					if (matching.@interface is { })
						return $"${matching.index}";

					parentInterfaces.Push((@interface, parentInterfaces.TryPeek(out var top) ? top.index + 1 : 0));

					var result = "{" + string.Join(';',
						@interface
						.Methods
						.Select(x => GenerateMethodKey(x, parentInterfaces))
						.Distinct()
						.OrderBy(x => x)) + "}";

					var popped = parentInterfaces.Pop();
					Release.Assert(popped.@interface == @interface);

					return result;
				}
			}

			private string GenerateMethodKey(IInterfaceMethod method, Stack<(IInterface @interface, int index)> parentInterfaces)
			{
				return
					method.Name
					+ GenerateTypeKey(method.ReturnType, parentInterfaces)
					+ string.Concat(method.Parameters.Select(x => GenerateTypeKey(x.Type, parentInterfaces)));
			}

			private string GenerateTypeKey(IType type, Stack<(IInterface @interface, int index)> parentInterfaces)
			{
				if (type is Primitive primitive)
					return GeneratePrimitiveKey(primitive);
				if (type is IInterface @interface)
					return GenerateInterfaceKey(@interface, parentInterfaces);
				if (type is IUnion union)
					return GenerateUnionKey(union, parentInterfaces);
				if (type is ITypeParameter typeParameter)
					throw Release.Fail($"Cannot use TypeParameter in methodKey: {typeParameter.Name}");
				throw Release.Fail("this location is thought to be unreachable");
			}

			private string GenerateUnionKey(IUnion union, Stack<(IInterface @interface, int index)> parentInterfaces)
			{
				return "(" + string.Join(
					"|",
					union.Options.Select(x => GenerateTypeKey(x, parentInterfaces))) + ")";
			}

			private string GeneratePrimitiveKey(Primitive primitive)
			{
				return primitive switch
				{
					_ when primitive == Primitive.Bool => "$b",
					_ when primitive == Primitive.Char => "$c",
					_ when primitive == Primitive.Double => "$d",
					_ when primitive == Primitive.Int => "$i",
					_ when primitive == Primitive.String => "$s",
					_ => throw Release.Fail("this location is thought to be unreachable"),
				};
			}
		}
	}
}
