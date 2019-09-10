using FluentLang.Compiler.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public static class SemanticModelExtensions
	{
		public static ISemanticModel With(this ISemanticModel @this, IInterface i)
		{
			var model = @this.TryWith(i);
			if (model is null)
				throw new ArgumentException($"Interface with name of {i.FullyQualifiedName} has already been added");
			return model;
		}
	}
}
