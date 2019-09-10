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
		public static void Equal(IInterface expected, IInterface actual)
		{
			if (ModelEqualityHelpers.AreEqual(expected, actual))
				return;

			var expectedSB = new StringBuilder();
			ModelBuilderGenerator.Generate(expected, expectedSB);
			var expectedBuilder = ModelBuilderGenerator.Indent(expectedSB.ToString());

			var actualSB = new StringBuilder();
			ModelBuilderGenerator.Generate(actual, actualSB);
			var actualBuilder = ModelBuilderGenerator.Indent(actualSB.ToString());

			Assert.False(true, $@"interfaces are not equal
expected:
{expectedBuilder}

actual:
{actualBuilder}");
		}

		public static void Equal(IMethod expected, IMethod actual)
		{
			if (ModelEqualityHelpers.AreEqual(expected, actual))
				return;

			var expectedSB = new StringBuilder();
			ModelBuilderGenerator.Generate(expected, expectedSB);
			var expectedBuilder = ModelBuilderGenerator.Indent(expectedSB.ToString());

			var actualSB = new StringBuilder();
			ModelBuilderGenerator.Generate(actual, actualSB);
			var actualBuilder = ModelBuilderGenerator.Indent(actualSB.ToString());

			Assert.False(true, $@"methods are not equal
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
