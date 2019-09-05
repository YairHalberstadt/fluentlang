using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentLang.Compiler.Model
{
	public struct InterfaceReference
	{
		private readonly Method? _scope;

		public InterfaceReference(ImmutableArray<QualifiedName> importedNamespaces, QualifiedName partiallyQualifiedName, Method? scope)
		{
			if (importedNamespaces.IsDefault) throw new ArgumentNullException(nameof(importedNamespaces));
			ImportedNamespaces = importedNamespaces;
			PartiallyQualifiedName = partiallyQualifiedName ?? throw new System.ArgumentNullException(nameof(partiallyQualifiedName));
			_scope = scope;
		}

		public ImmutableArray<QualifiedName> ImportedNamespaces { get; }

		public QualifiedName PartiallyQualifiedName { get; }

		public IEnumerable<IInterface> GetPossibleInterfaces(ISemanticModel model)
		{
			if (PartiallyQualifiedName is null)
				throw new InvalidOperationException($"Do not use the default constructor for {nameof(InterfaceReference)}");
			var name = PartiallyQualifiedName;
			var possibleNames = ImportedNamespaces.Select(x => x.Append(name)).Append(name);
			return possibleNames.Select(x => model.TryGetInterface(x, out var i) ? i : null).Where(x => x != null)!;
		}

		public bool IsEquivalentTo(IType other, ISemanticModel model)
		{
			return IsEquivalentTo(other, null, model);
		}

		public bool IsEquivalentTo(InterfaceReference other, ISemanticModel model)
		{
			return IsEquivalentTo(other, null, model);
		}

		public bool IsEquivalentTo(IType other, Stack<(IType, IType)>? dependantEqualities, ISemanticModel model)
		{
			if (!(other is IInterface otherInterface))
				return false;
			var thisInterface = GetPossibleInterfaces(model).SingleOrDefault();
			return thisInterface is null 
				? false 
				: thisInterface.IsEquivalentTo(otherInterface, dependantEqualities, model);
		}

		public bool IsEquivalentTo(InterfaceReference other, Stack<(IType, IType)>? dependantEqualities, ISemanticModel model)
		{
			var thisInterface = GetPossibleInterfaces(model).SingleOrDefault();
			if (thisInterface is null)
				return false;
			var otherInterface = other.GetPossibleInterfaces(model).SingleOrDefault();
			return otherInterface is null 
				? false 
				: thisInterface.IsEquivalentTo(otherInterface, dependantEqualities, model);
		}
	}
}
