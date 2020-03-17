using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.Interfaces;
using System.Collections.Immutable;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal static class MethodBodySymbolContextExtensions
	{
		public static void WarnIfUseOfMethodWhichCapturesUnassignedLocals(this MethodBodySymbolContext methodBodySymbolContext, IMethod target, DiagnosticBag diagnostics, Method_referenceContext syntax)
		{
			var currentMethod = methodBodySymbolContext.SourceSymbolContext.Scope;
			Release.Assert(currentMethod != null);
			if (target.DeclaringMethod == currentMethod)
			{
				if (target.InScopeAfter is { } declarationStatement)
				{
					var currentStatement = methodBodySymbolContext.CurrentStatement;
					Release.Assert(currentStatement != null);
					if (declarationStatement.OrdinalPositionInMethod >= currentStatement.OrdinalPositionInMethod)
					{
						diagnostics.Add(new Diagnostic(
							new Location(syntax),
							ErrorCode.UseOfMethodWhichCapturesUnassignedLocals,
							ImmutableArray.Create<object?>(target)));
					}
				}
			}
		}
	}
}