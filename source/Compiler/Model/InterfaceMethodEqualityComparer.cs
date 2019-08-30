using System.Collections.Generic;

namespace FluentLang.Compiler.Model
{
	public class InterfaceMethodEqualityComparer : IEqualityComparer<InterfaceMethod>
	{
		private readonly Stack<(Type, Type)>? _dependantEqualities;
		private readonly ISemanticModel _model;

		public InterfaceMethodEqualityComparer(ISemanticModel model)
		{
			_model = model;
		}

		internal InterfaceMethodEqualityComparer(Stack<(Type, Type)>? dependantEqualities, ISemanticModel model)
		{
			_dependantEqualities = dependantEqualities;
			_model = model;
		}

		public bool Equals(InterfaceMethod x, InterfaceMethod y)
		{
			if (x is null) return y is null;
			return x.IsEquivalentTo(y, _dependantEqualities, _model);
		}

		public int GetHashCode(InterfaceMethod obj)
		{
			return obj.Name.GetHashCode() * 17 + (obj.Parameters.Length);
		}
	}
}
