using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Model
{
	public sealed class Method
	{
		private readonly Method_bodyContext _method_BodyContext;

		public Method(
			QualifiedName fullyQualifiedName,
			TypeKey returnType,
			ImmutableArray<Parameter> parameters,
			Method? scope,
			Method_bodyContext method_bodyContext)
		{
			FullyQualifiedName = fullyQualifiedName;
			ReturnType = returnType;
			Parameters = parameters;
			Scope = scope;
			_method_BodyContext = method_bodyContext;
		}

		public string Name => FullyQualifiedName.Name;

		public QualifiedName? Namespace => FullyQualifiedName.Parent;

		public QualifiedName FullyQualifiedName { get; }

		public TypeKey ReturnType { get; }

		public ImmutableArray<Parameter> Parameters { get; }

		public Method? Scope { get; }
	}
}
