using System.Collections.Generic;

namespace FluentLang.Compiler.Model
{
	public class InterfaceMethodEqualityComparer : IEqualityComparer<IInterfaceMethod>
	{
		private readonly Stack<(IType, IType)>? _dependantEqualities;
		private readonly ISemanticModel _model;

		public InterfaceMethodEqualityComparer(ISemanticModel model)
		{
			_model = model;
		}

		internal InterfaceMethodEqualityComparer(Stack<(IType, IType)>? dependantEqualities, ISemanticModel model)
		{
			_dependantEqualities = dependantEqualities;
			_model = model;
		}

		public bool Equals(IInterfaceMethod x, IInterfaceMethod y)
		{
			if (x is null) return y is null;
			return x.IsEquivalentTo(y, _dependantEqualities, _model);
		}

		public int GetHashCode(IInterfaceMethod obj)
		{
			return obj.Name.GetHashCode() * 17 + (obj.Parameters.Length);
		}
	}
}
