using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentLang.Compiler.Model
{
	public class TypeEqualityComparer : IEqualityComparer<IType>, IEqualityComparer<TypeKey>
	{
		private readonly Stack<(IType, IType)>? _dependantEqualities;
		private readonly ISemanticModel _model;

		public TypeEqualityComparer(ISemanticModel model)
		{
			_model = model;
		}

		internal TypeEqualityComparer(Stack<(IType, IType)>? dependantEqualities, ISemanticModel model)
		{
			_dependantEqualities = dependantEqualities;
			_model = model;
		}

		public bool Equals(IType x, IType y)
		{
			if (x is null)
				return y is null;
			return x.IsEquivalentTo(y, _dependantEqualities, _model);
		}

		public int GetHashCode(IType obj)
		{
			return obj switch
			{
				Primitive _ => obj.GetHashCode(),
				IInterface i => i
					.AllInterfaceMethods(_dependantEqualities, _model)
					.Sum(x=> x.Name.GetHashCode() * 17 + x.Parameters.Length),
				_ => throw new InvalidOperationException(obj.GetType().ToString())
			};
		}

		public bool Equals(TypeKey x, TypeKey y)
		{
			return x.IsEquivalentTo(y, _dependantEqualities, _model);
		}

		public int GetHashCode(TypeKey obj)
		{
			var type = obj.GetPossibleTypes(_model).SingleOrDefault();
			if (type is null)
				return obj.GetHashCode();
			return GetHashCode(type);

		}
	}
}
