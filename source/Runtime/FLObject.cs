using System;
using System.Collections.Immutable;

namespace FluentLang.Runtime
{
	public class FLObject
	{
		public static FLObject Empty { get; } = new FLObject();
		private FLObject() => _methods = ImmutableDictionary<MethodKey, Delegate>.Empty;

		private FLObject(ImmutableDictionary<MethodKey, Delegate> methods)
		{
			_methods = methods;
		}

		private readonly ImmutableDictionary<MethodKey, Delegate> _methods;

		public T GetMethod<T>(MethodKey methodKey) where T : Delegate
		{
			if (!_methods.TryGetValue(methodKey, out var method))
			{
				throw new InvalidOperationException(
$@"Method with MethodKey {methodKey.ToString()} does not exist on this.
Make sure this is running with the same versions of dependencies as it was compiled against.");
			}
			if (!(method is T t))
			{ 
				throw new InvalidOperationException(
$@"Method with MethodKey {methodKey.ToString()} is of type {method.GetType()} but was expected to be of type {typeof(T)}.
Make sure this is running with the same versions of dependencies as it was compiled against.");
			}
			return t;
		}

		public FLObject With(MethodKey methodKey, Delegate method)
		{
			return new FLObject(_methods.SetItem(methodKey, method));
		}
	}
}
