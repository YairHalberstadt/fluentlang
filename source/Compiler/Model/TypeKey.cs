using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentLang.Compiler.Model
{
	public struct TypeKey
	{
		public TypeKey(Primitive primitive) : this()
		{
			Primitive = primitive;
		}

		public TypeKey(Interface @interface) : this()
		{
			Interface = @interface;
		}

		public TypeKey(InterfaceReference interfaceReference) : this()
		{
			InterfaceReference = interfaceReference;
		}

		public Primitive? Primitive { get; }

		public Interface? Interface { get; }

		public InterfaceReference? InterfaceReference { get; }

		public IEnumerable<Type> GetPossibleTypes(ISemanticModel model)
		{
			if (Primitive != null)
				return new[] { Primitive };
			if (Interface != null)
				return new[] { Interface };
			if (InterfaceReference != null)
				return InterfaceReference.Value.GetPossibleInterfaces(model);
			throw new InvalidOperationException($"Do not use the default constructor for {nameof(TypeKey)}");
		}

		public bool IsEquivalentTo(Type other, ISemanticModel model)
		{
			return IsEquivalentTo(other, null, model);
		}

		public bool IsEquivalentTo(TypeKey other, ISemanticModel model)
		{
			return IsEquivalentTo(other, null, model);
		}

		public bool IsEquivalentTo(InterfaceReference other, ISemanticModel model)
		{
			return IsEquivalentTo(other, null, model);
		}

		internal bool IsEquivalentTo(Type other, Stack<(Type, Type)>? dependantEqualities, ISemanticModel model)
		{
			var thisType = GetPossibleTypes(model).SingleOrDefault();
			if (thisType is null)
				return false;
			return (thisType.IsEquivalentTo(other, dependantEqualities, model));
		}

		internal bool IsEquivalentTo(TypeKey other, Stack<(Type, Type)>? dependantEqualities, ISemanticModel model)
		{
			var thisType = GetPossibleTypes(model).SingleOrDefault();
			if (thisType is null)
				return false;
			var otherType = other.GetPossibleTypes(model).SingleOrDefault();
			if (otherType is null)
				return false;
			return (thisType.IsEquivalentTo(otherType, dependantEqualities, model));
		}

		internal bool IsEquivalentTo(InterfaceReference other, Stack<(Type, Type)>? dependantEqualities, ISemanticModel model)
		{
			var thisType = GetPossibleTypes(model).SingleOrDefault();
			if (thisType is null)
				return false;
			var otherInterface = other.GetPossibleInterfaces(model).SingleOrDefault();
			if (otherInterface is null)
				return false;
			return (thisType.IsEquivalentTo(otherInterface, dependantEqualities, model));
		}
	}
}