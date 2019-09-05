using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace FluentLang.Compiler.Model
{
	public class SemanticModel : ISemanticModel
	{
		private readonly ImmutableDictionary<QualifiedName, IInterface> _interfaces;
		private readonly ImmutableDictionary<QualifiedName, Method> _methods;

		private SemanticModel(
			ImmutableDictionary<QualifiedName, IInterface> interfaces,
			ImmutableDictionary<QualifiedName, Method> methods)
		{
			_interfaces = interfaces;
			_methods = methods;
		}

		public static SemanticModel Empty { get; } = new SemanticModel(
			ImmutableDictionary<QualifiedName, IInterface>.Empty,
			ImmutableDictionary<QualifiedName, Method>.Empty);

		public ISemanticModel With(IInterface i)
		{
			var model = TryWith(i);
			if (model is null)
				throw new ArgumentException($"Interface with name of {i.FullyQualifiedName} has already been added");
			return model;

		}

		public ISemanticModel? TryWith(IInterface i)
		{
			if (i.FullyQualifiedName is null)
				throw new ArgumentException($"{nameof(i)} must be a named type to add it to a semantic model");

			if (_interfaces.ContainsKey(i.FullyQualifiedName))
			{
				return null;
			}
			var updatedInterfaces = _interfaces.Add(i.FullyQualifiedName, i);
			return new SemanticModel(updatedInterfaces, _methods);
		}

		public ISemanticModel? TryWith(Method m)
		{
			if (m.FullyQualifiedName is null)
				throw new ArgumentException($"{nameof(m)} must be a named type to add it to a semantic model");

			if (_methods.ContainsKey(m.FullyQualifiedName))
			{
				return null;
			}
			var updatedMethods = _methods.Add(m.FullyQualifiedName, m);
			return new SemanticModel(_interfaces, updatedMethods);
		}

		public bool TryGetInterface(QualifiedName fullyQualifiedName, out IInterface i)
		{
			return _interfaces.TryGetValue(fullyQualifiedName, out i);
		}
	}
}
