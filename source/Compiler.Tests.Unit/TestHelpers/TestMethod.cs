using FluentLang.Compiler.Model;
using System;
using System.Collections.Immutable;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public class TestMethod : IMethod
	{
		public TestMethod(
			QualifiedName fullyQualifiedName,
			TypeKey returnType,
			ImmutableArray<Parameter> parameters,
			Func<IMethod>? scope,
			ImmutableArray<IInterface> localInterfaces,
			ImmutableArray<IMethod> localMethods)
		{
			FullyQualifiedName = fullyQualifiedName;
			Parameters = parameters;
			ReturnType = returnType;
			_scope = scope;
			LocalInterfaces = localInterfaces;
			LocalMethods = localMethods;
		}

		public QualifiedName FullyQualifiedName { get; }

		public ImmutableArray<Parameter> Parameters { get; }

		public TypeKey ReturnType { get; }

		private readonly Func<IMethod>? _scope;

		public IMethod? Scope => _scope?.Invoke();

		public ImmutableArray<IInterface> LocalInterfaces { get; }

		public ImmutableArray<IMethod> LocalMethods { get; }
	}
}
