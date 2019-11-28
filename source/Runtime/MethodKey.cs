using System;

namespace FluentLang.Runtime
{
	public struct MethodKey : IEquatable<MethodKey>
	{
		private readonly string _key;

		public MethodKey(string key)
		{
			_key = key;
		}

		public bool Equals(MethodKey other)
		{
			return other._key?.Equals(_key) ??  
				throw new InvalidOperationException($"{nameof(_key)} is null. Default MethodKeys are invalid.");
		}

		public override bool Equals(object? obj)
		{
			if (obj is MethodKey methodKey)
				return Equals(methodKey);

			return false;
		}

		public override int GetHashCode()
		{
			return _key.GetHashCode();
		}

		public override string ToString()
		{
			return _key;
		}
	}
}
