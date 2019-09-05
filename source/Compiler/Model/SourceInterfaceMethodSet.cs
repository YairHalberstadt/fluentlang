using System;
using System.Collections.Immutable;
using System.Linq;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.SemanticPhase;

namespace FluentLang.Compiler.Model
{
	internal sealed class SourceInterfaceMethodSet : IInterfaceMethodSet
	{
		private readonly FluentLangParser.Interface_member_declarationContext[] _context;
		private readonly ImmutableArray<QualifiedName> _openedNamespaces;
		private readonly Method? _scope;

		private ImmutableArray<IInterfaceMethod> _methods;

		public SourceInterfaceMethodSet(FluentLangParser.Interface_member_declarationContext[] context, ImmutableArray<QualifiedName> openedNamespaces, Method? scope)
		{
			_context = context;
			_openedNamespaces = openedNamespaces;
			_scope = scope;
		}

		public ImmutableArray<IInterfaceMethod> Methods => _methods.IsDefault ? _methods = BindMethods() : _methods;

		private ImmutableArray<IInterfaceMethod> BindMethods()
		{
			return _context.Select(x => x.BindMethod(_openedNamespaces, _scope)).ToImmutableArray();
		}
	}
}
