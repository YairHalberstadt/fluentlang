using FluentLang.Compiler.SemanticPhase;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Model
{
	public sealed class SourceMethod : IMethod
	{
		private readonly Method_declarationContext _context;
		private readonly ImmutableArray<QualifiedName> _openedNamespaces;
		private TypeKey? _returnType;
		private ImmutableArray<Parameter> _parameters;
		private ImmutableArray<IInterface> _localInterfaces;
		private ImmutableArray<IMethod> _localMethods;

		public SourceMethod(
			Method_declarationContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			QualifiedName fullyQualifiedName,
			IMethod? scope = null)
		{
			_context = context;
			_openedNamespaces = openedNamespaces;
			FullyQualifiedName = fullyQualifiedName;
			Scope = scope;
		}

		public QualifiedName FullyQualifiedName { get; }

		public TypeKey ReturnType => _returnType 
			?? (_returnType = _context.method_signature().BindReturnType(_openedNamespaces, Scope)).Value;

		public ImmutableArray<Parameter> Parameters => _parameters.IsDefault 
			? _parameters = _context.method_signature().BindParameters(_openedNamespaces, Scope) 
			: _parameters;

		public ImmutableArray<IInterface> LocalInterfaces => _localInterfaces.IsDefault 
			? _localInterfaces = _context.BindInterfaces(_openedNamespaces, this)
			: _localInterfaces;

		public ImmutableArray<IMethod> LocalMethods => _localMethods.IsDefault
			? _localMethods = _context.BindMethods(_openedNamespaces, this)
			: _localMethods;

		public IMethod? Scope { get; }
	}
}
