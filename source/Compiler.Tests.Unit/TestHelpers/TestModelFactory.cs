using FluentLang.Compiler.Model;
using System;
using System.Text;
using System.Linq;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public static class TestModelFactory
	{
		public static QualifiedName QualifiedName(string qualifiedName)
		{
			return qualifiedName.Split('.').Aggregate((QualifiedName?)null, (l, r) => new QualifiedName(r, l))!;
		}
	}
}
