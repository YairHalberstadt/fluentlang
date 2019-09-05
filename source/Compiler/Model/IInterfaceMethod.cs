using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Model
{
	public interface IInterfaceMethod
	{
		string Name { get; }
		ImmutableArray<Parameter> Parameters { get; }
		TypeKey ReturnType { get; }

		public bool IsEquivalentTo(IInterfaceMethod other, ISemanticModel model)
		{
			return IsEquivalentTo(other, null, model);
		}

		internal bool IsEquivalentTo(IInterfaceMethod? other, Stack<(IType, IType)>? dependantEqualities, ISemanticModel model)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (Name != other.Name)
			{
				return false;
			}

			if (Parameters.Length != other.Parameters.Length)
			{
				return false;
			}

			return
				ReturnType.IsEquivalentTo(other.ReturnType, dependantEqualities, model)
				&& Parameters.Select(x => x.Type)
					.SequenceEqual(
					other.Parameters.Select(x => x.Type), new TypeEqualityComparer(dependantEqualities, model));
		}
	}
}