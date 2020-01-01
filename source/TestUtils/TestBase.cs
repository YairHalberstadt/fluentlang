﻿using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Source;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.TestUtils
{
	public abstract class TestBase
	{
		protected readonly ITestOutputHelper _testOutputHelper;

		protected TestBase(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		protected IAssembly CreateAssembly(string source, string? name = null, Version? version = null, params IAssembly[] references)
		{
			var document = DocumentFactory.FromString(source);
			return new SourceAssembly(
				QualifiedName(name ?? "Test"),
				version: version ?? new Version(1, 0, 0),
				references.ToImmutableArray(),
				ImmutableArray.Create<IDocument>(document));
		}

		protected IAssembly CreateAssembly(IEnumerable<string> sources)
		{
			return new SourceAssembly(
				QualifiedName("Test"),
				version: new Version(1, 0, 0),
				ImmutableArray<IAssembly>.Empty,
				sources.Select(x => DocumentFactory.FromString(x)).ToImmutableArray<IDocument>());
		}

		protected static QualifiedName QualifiedName(string qualifiedName)
		{
			return Compiler.Symbols.QualifiedName.Parse(qualifiedName);
		}

		protected static IMethod AssertGetMethod(IAssembly assembly, string qualifiedName)
		{
			Assert.True(assembly.TryGetMethod(QualifiedName(qualifiedName), out var method));
			return method!;
		}

		protected static IInterface AssertGetInterface(IAssembly assembly, string qualifiedName)
		{
			Assert.True(assembly.TryGetInterface(QualifiedName(qualifiedName), out var @interface));
			return @interface!;
		}

		public ILogger<T> GetLogger<T>() => new XunitLogger<T>(_testOutputHelper);
	}
}
