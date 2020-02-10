using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FluentLang.Compiler.Emit
{
	public class MethodKeyGenerator
	{
		private readonly ConcurrentDictionary<IInterfaceMethod, string> _interfaceMethodCache = new ConcurrentDictionary<IInterfaceMethod, string>();
		private readonly ConcurrentDictionary<IMethod, string> _methodExcludingFirstParameterCache = new ConcurrentDictionary<IMethod, string>();
		private readonly ConcurrentDictionary<IInterface, string> _interfaceCache = new ConcurrentDictionary<IInterface, string>();

		public string GenerateMethodKey(IInterfaceMethod method)
		{
			return _interfaceMethodCache.GetOrAdd(
				method,
				method => GenerateMethodKey(method, new Stack<(IInterface @interface, int index)>()));
		}

		public string GenerateMethodKeyExcludingFirstParameter(IMethod method)
		{
			return _methodExcludingFirstParameterCache.GetOrAdd(method, method =>
			{
				var parentInterfaces = new Stack<(IInterface @interface, int index)>();
				return
					method.Name
					+ GenerateTypeKey(method.ReturnType, parentInterfaces)
					+ string.Concat(method.Parameters.Skip(1).Select(x => GenerateTypeKey(x.Type, parentInterfaces)));
			});
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
				return typeParameter.Name;
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
