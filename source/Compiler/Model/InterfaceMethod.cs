using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Model
{
	public class InterfaceMethod
	{
		public InterfaceMethod(
			string name,
			TypeKey returnType,
			ImmutableArray<Parameter> parameters)
		{
			Name = name;
			ReturnType = returnType;
			Parameters = parameters;
		}

		public string Name { get; }

		public TypeKey ReturnType { get; }

		public ImmutableArray<Parameter> Parameters { get; }

		public bool IsEquivalentTo(InterfaceMethod other, ISemanticModel model)
		{
			return IsEquivalentTo(other, null, model);
		}

		internal bool IsEquivalentTo(InterfaceMethod? other, Stack<(Type, Type)>? dependantEqualities, ISemanticModel model)
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
