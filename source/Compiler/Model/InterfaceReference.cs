using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Model
{
	public struct InterfaceReference
	{
		public InterfaceReference(ImmutableArray<QualifiedName> importedNamespaces, QualifiedName partiallyQualifiedName)
		{
			if (importedNamespaces.IsDefault) throw new ArgumentNullException(nameof(importedNamespaces));
			ImportedNamespaces = importedNamespaces;
			PartiallyQualifiedName = partiallyQualifiedName ?? throw new System.ArgumentNullException(nameof(partiallyQualifiedName));
		}

		public ImmutableArray<QualifiedName> ImportedNamespaces { get; }

		public QualifiedName PartiallyQualifiedName { get; }

		public IEnumerable<Interface> GetPossibleInterfaces(ISemanticModel model)
		{
			if (PartiallyQualifiedName is null)
				throw new InvalidOperationException($"Do not use the default constructor for {nameof(InterfaceReference)}");
			var name = PartiallyQualifiedName;
			var possibleNames = ImportedNamespaces.Select(x => x.Append(name)).Append(name);
			return possibleNames.Select(x => model.TryGetInterface(x, out var i) ? i : null).Where(x => x != null)!;
		}

		public bool IsEquivalentTo(Type other, ISemanticModel model)
		{
			return IsEquivalentTo(other, null, model);
		}

		public bool IsEquivalentTo(InterfaceReference other, ISemanticModel model)
		{
			return IsEquivalentTo(other, null, model);
		}

		public bool IsEquivalentTo(Type other, Stack<(Type, Type)>? dependantEqualities, ISemanticModel model)
		{
			if (!(other is Interface otherInterface))
				return false;
			var thisInterface = GetPossibleInterfaces(model).SingleOrDefault();
			if (thisInterface is null)
				return false;
			return thisInterface.IsEquivalentTo(otherInterface, dependantEqualities, model);
		}

		public bool IsEquivalentTo(InterfaceReference other, Stack<(Type, Type)>? dependantEqualities, ISemanticModel model)
		{
			var thisInterface = GetPossibleInterfaces(model).SingleOrDefault();
			if (thisInterface is null)
				return false;
			var otherInterface = other.GetPossibleInterfaces(model).SingleOrDefault();
			if (otherInterface is null)
				return false;
			return thisInterface.IsEquivalentTo(otherInterface, dependantEqualities, model);
		}
	}
}
