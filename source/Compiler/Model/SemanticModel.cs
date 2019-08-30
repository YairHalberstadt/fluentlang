using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Model
{
	public class SemanticModel : ISemanticModel
	{
		private readonly ImmutableDictionary<QualifiedName, Interface> _interfaces;

		private SemanticModel(ImmutableDictionary<QualifiedName, Interface> interfaces)
		{
			_interfaces = interfaces;
		}

		public static SemanticModel Empty { get; } = new SemanticModel(ImmutableDictionary<QualifiedName, Interface>.Empty);

		public ISemanticModel With(Interface i)
		{
			var model = TryWith(i);
			if (model is null)
				throw new ArgumentException($"Interface with name of {i.FullyQualifiedName} has already been added");
			return model;

		}

		public ISemanticModel? TryWith(Interface i)
		{
			if (i.FullyQualifiedName is null)
				throw new ArgumentException($"{nameof(i)} must be a named type to add it to a semantic model");

			if (_interfaces.ContainsKey(i.FullyQualifiedName))
			{
				return null;
			}
			var updatedInterfaces = _interfaces.Add(i.FullyQualifiedName, i);
			return new SemanticModel(updatedInterfaces);
		}

		public bool TryGetInterface(QualifiedName fullyQualifiedName, out Interface i)
		{
			return _interfaces.TryGetValue(fullyQualifiedName, out i);
		}
	}
}
