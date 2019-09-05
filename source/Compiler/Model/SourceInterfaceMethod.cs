using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.SemanticPhase;

namespace FluentLang.Compiler.Model
{
	internal sealed class SourceInterfaceMethod : IInterfaceMethod
	{
		private readonly FluentLangParser.Interface_member_declarationContext _context;
		private readonly ImmutableArray<QualifiedName> _openedNamespaces;
		private readonly Method? _scope;
		private TypeKey? _returnType;
		private ImmutableArray<Parameter> _parameters;

		public SourceInterfaceMethod(FluentLangParser.Interface_member_declarationContext context, ImmutableArray<QualifiedName> openedNamespaces, Method? scope)
		{
			_context = context;
			_openedNamespaces = openedNamespaces;
			_scope = scope;

			Name = _context.method_signature().GetName();
		}

		public string Name { get; }

		public TypeKey ReturnType => _returnType ?? 
			(_returnType = _context.method_signature().BindReturnType(_openedNamespaces, _scope)).Value;

		public ImmutableArray<Parameter> Parameters => _parameters.IsDefault ? _parameters = _context.method_signature().BindParameters(_openedNamespaces, _scope) : _parameters;
	}
}
