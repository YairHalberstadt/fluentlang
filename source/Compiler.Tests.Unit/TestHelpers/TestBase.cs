using FluentLang.Compiler.Symbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Source;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public abstract class TestBase
	{
		protected IAssembly CreateAssembly(string source)
		{
			var document = new TestDocument(source);
			return new SourceAssembly(
				QualifiedName("Test"),
				new Version("1.0.0"),
				ImmutableArray<IAssembly>.Empty,
				ImmutableArray.Create<IDocument>(document));
		}

		protected IAssembly CreateAssembly(IEnumerable<string> sources)
		{
			return new SourceAssembly(
				QualifiedName("Test"),
				new Version("1.0.0"),
				ImmutableArray<IAssembly>.Empty,
				sources.Select(x => new TestDocument(x)).ToImmutableArray<IDocument>());
		}

		protected static QualifiedName QualifiedName(string qualifiedName)
		{
			return qualifiedName.Split('.').Aggregate((QualifiedName?)null, (l, r) => new QualifiedName(r, l))!;
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
	}
}
