using FluentLang.Compiler.SemanticPhase;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Model
{
	internal sealed class SourceInterface : IInterface
	{
		private readonly Anonymous_interface_declarationContext _context;
		private readonly ImmutableArray<QualifiedName> _openedNamespaces;

		private ImmutableHashSet<IInterfaceMethod>? _allMethodDefinitions;
		private ImmutableArray<InterfaceReference> _additiveInterfaces;
		private ImmutableArray<IInterfaceMethodSet> _methodSets;

		public SourceInterface(Anonymous_interface_declarationContext context, ImmutableArray<QualifiedName> openedNamespaces, QualifiedName? fullyQualifiedName = null, Method? scope = null)
		{
			_context = context;
			_openedNamespaces = openedNamespaces;
			FullyQualifiedName = fullyQualifiedName;
			Scope = scope;
		}

		public ImmutableArray<InterfaceReference> AdditiveInterfaces => _additiveInterfaces.IsDefault ? _additiveInterfaces = BindAdditiveInterfaces() : _additiveInterfaces;

		public ImmutableArray<IInterfaceMethodSet> MethodSets => _methodSets.IsDefault ? _methodSets = BindMethodSets() : _methodSets;

		public QualifiedName? FullyQualifiedName { get; }

		public Method? Scope { get; }

		public ImmutableHashSet<IInterfaceMethod> AllInterfaceMethods(ISemanticModel model)
		{
			if (_allMethodDefinitions is null)
			{
				var mutable = ((IInterface)this).AllInterfaceMethods(null, model);
				_allMethodDefinitions = mutable.ToImmutableHashSet(mutable.Comparer);
			}

			return _allMethodDefinitions;
		}

		private ImmutableArray<InterfaceReference> BindAdditiveInterfaces()
		{
			return _context.BindAdditiveInterfaces(_openedNamespaces, Scope);
		}

		private ImmutableArray<IInterfaceMethodSet> BindMethodSets()
		{
			return _context.BindMethodSets(_openedNamespaces, Scope);
		}
	}
}
