using FluentLang.Compiler.Model;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public static class AssertHelpers
	{
		public static void Equal(Interface expected, Interface actual)
		{
			if (ModelEqualityHelpers.AreEqual(expected, actual))
				return;

			var expectedSB = new StringBuilder();
			InterfaceBuilderGenerator.Generate(expected, expectedSB);
			var expectedBuilder = InterfaceBuilderGenerator.Indent(expectedSB.ToString());

			var actualSB = new StringBuilder();
			InterfaceBuilderGenerator.Generate(actual, actualSB);
			var actualBuilder = InterfaceBuilderGenerator.Indent(actualSB.ToString());

			Assert.False(true, $@"interfaces are not equal
expected:
{expectedBuilder}

actual:
{actualBuilder}");
		}

		public static void Fail()
		{
			Assert.False(true, "test failed");
		}

		public static void Fail(string userMessage)
		{
			Assert.False(true, userMessage);
		}
	}
}
