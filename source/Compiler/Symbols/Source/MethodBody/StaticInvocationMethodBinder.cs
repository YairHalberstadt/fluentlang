using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.ErrorSymbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source.MethodBody
{
	internal static class StaticInvocationMethodBinder
	{
		public static IMethod BindMethod(
			QualifiedName MethodName,
			ImmutableArray<IType> TypeArguments,
			ImmutableArray<IExpression> Arguments,
			Method_referenceContext method_reference,
			MethodBodySymbolContext _methodBodySymbolContext,
			DiagnosticBag _diagnostics)
        {
			var methods = _methodBodySymbolContext.SourceSymbolContext.GetPossibleMethods(MethodName, TypeArguments);

			var matching =
				methods
				.Where(x => x.Parameters.Length == Arguments.Length)
				.Select(x =>
				{
					Diagnostic? diagnostic = null;
					if (TypeArguments.Length > 0)
					{
						var typeParameters = x.TypeParameters;
						if (!SourceSymbolContextExtensions.HasValidTypeArguments(
							TypeArguments,
							typeParameters,
							out var diagnosticFunc))
						{
							diagnostic = diagnosticFunc(new Location(method_reference));
						}
						var substituted = x.Substitute(
							SourceSymbolContextExtensions.CreateTypeMap(TypeArguments, typeParameters),
							new Dictionary<IType, IType>());

						return (method: substituted, diagnostic);
					}
					return (method: x, diagnostic: null);
				})
				.Where(x => x.method.Parameters.Zip(Arguments, (p, a) => a.Type.IsSubtypeOf(p.Type)).All(x => x))
				.ToList();

			if (matching.Count == 1)
			{
				var (target, diagnostic) = matching[0];

				if (diagnostic != null)
				{
					_diagnostics.Add(diagnostic);
				}

				_methodBodySymbolContext.WarnIfUseOfMethodWhichCapturesUnassignedLocals(target, _diagnostics, method_reference);
				return target;
			}


			if (matching.Count == 0)
			{
				_diagnostics.Add(new Diagnostic(
					new Location(method_reference),
					ErrorCode.MethodNotFound,
					ImmutableArray.Create<object?>(MethodName, Arguments)));
			}
			else
			{
				_diagnostics.Add(new Diagnostic(
					new Location(method_reference),
					ErrorCode.AmbigiousMethodReference,
					ImmutableArray.Create<object?>(matching)));
			}

			return new ErrorMethod(MethodName, Arguments.Length);
		}
	}
}
