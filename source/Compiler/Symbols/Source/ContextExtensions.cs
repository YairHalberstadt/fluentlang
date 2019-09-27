using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.ErrorSymbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Source.MethodBody;
using System;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal static class ContextExtensions
	{
		public static QualifiedName GetQualifiedName(this Qualified_nameContext context)
		{
			return context
				.UPPERCASE_IDENTIFIER()
				.Aggregate((QualifiedName?)null, (l, r) => new QualifiedName(r.Symbol.Text, l))!;
		}

		public static IType BindType(this TypeContext context, SourceSymbolContext sourceSymbolContext, DiagnosticBag diagnostics)
		{
			if (context.primitive_type() is { } primitive)
			{
				return primitive.BindPrimitive();
			}
			if (context.qualified_name() is { } qualifiedName)
			{
				var @interface = sourceSymbolContext.GetInterfaceOrError(qualifiedName.GetQualifiedName(), out var diagnostic);
				if (diagnostic != null)
					diagnostics.Add(diagnostic(new Location(qualifiedName)));
				return @interface;
			}
			if (context.anonymous_interface_declaration() is { } interfaceContext)
			{
				return new SourceInterface(interfaceContext, sourceSymbolContext, fullyQualifiedName: null, diagnostics);
			}

			diagnostics.Add(new Diagnostic(new Location(context), ErrorCode.InvalidParseTree, ImmutableArray.Create<object?>(context)));
			return ErrorType.Instance;
		}

		public static Primitive BindPrimitive(this Primitive_typeContext context)
		{
			if (context.BOOL() != null)
				return Primitive.Bool;
			if (context.INT() != null)
				return Primitive.Int;
			if (context.DOUBLE() != null)
				return Primitive.Double;
			if (context.STRING() != null)
				return Primitive.String;
			if (context.CHAR() != null)
				return Primitive.Char;
			throw Release.Fail($"unexpected primitive: {context}");
		}

		public static IType BindReturnType(this Method_signatureContext context, SourceSymbolContext sourceSymbolContext, DiagnosticBag diagnostics)
		{
			return context.type_declaration().type().BindType(sourceSymbolContext, diagnostics);
		}

		public static ImmutableArray<IParameter> BindParameters(this Method_signatureContext context, SourceSymbolContext sourceSymbolContext, DiagnosticBag diagnostics)
		{
			return
				context
				.parameters()
				.parameter()
				.Select(x => new SourceParameter(x, sourceSymbolContext, diagnostics))
				.ToImmutableArray<IParameter>();
		}

		public static IExpression BindExpression(
			this ExpressionContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics)
		{
			throw new NotImplementedException();
		}
	}
}
