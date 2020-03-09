using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.ErrorSymbols;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Source.MethodBody;
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

		public static IType BindType(this TypeContext context, SourceSymbolContext sourceSymbolContext, bool isExported, DiagnosticBag diagnostics)
		{
			if (context.primitive_type() is { } primitive)
			{
				return primitive.BindPrimitive();
			}
			if (context.named_type_reference() is { } namedTypeReference)
			{
				return namedTypeReference.BindNamedTypeReference(sourceSymbolContext, isExported, diagnostics);
			}
			if (context.anonymous_interface_declaration() is { } interfaceContext)
			{
				return new SourceAnonymousInterface(
					interfaceContext,
					sourceSymbolContext,
					isExported: isExported,
					diagnostics);
			}
			if (context.union() is { } union)
			{
				return new SourceUnion(union, sourceSymbolContext, isExported, diagnostics);
			}

			diagnostics.Add(new Diagnostic(new Location(context), ErrorCode.InvalidParseTree, ImmutableArray.Create<object?>(context)));
			return ErrorType.Instance;
		}

		public static IType BindUnionPartType(this Union_part_typeContext context, SourceSymbolContext sourceSymbolContext, bool isExported, DiagnosticBag diagnostics)
		{
			if (context.primitive_type() is { } primitive)
			{
				return primitive.BindPrimitive();
			}
			if (context.named_type_reference() is { } namedTypeReference)
			{
				return namedTypeReference.BindNamedTypeReference(sourceSymbolContext, isExported, diagnostics);
			}
			if (context.anonymous_interface_declaration() is { } interfaceContext)
			{
				return new SourceAnonymousInterface(
					interfaceContext,
					sourceSymbolContext,
					isExported: isExported,
					diagnostics);
			}

			diagnostics.Add(new Diagnostic(new Location(context), ErrorCode.InvalidParseTree, ImmutableArray.Create<object?>(context)));
			return ErrorType.Instance;
		}

		public static ImmutableArray<IType> BindTypeArgumentList(
			this Type_argument_listContext context,
			SourceSymbolContext sourceSymbolContext,
			DiagnosticBag diagnostics)
		{
			return
				context
				.type()
				.Select(x => x.BindType(sourceSymbolContext, false, diagnostics))
				.ToImmutableArray();
		}

		private static IType BindNamedTypeReference(
			this Named_type_referenceContext context,
			SourceSymbolContext sourceSymbolContext,
			bool isExported,
			DiagnosticBag diagnostics)
		{
			var qualifiedName = context.qualified_name();
			var typeArguments =
				context
				.type_argument_list()
				.BindTypeArgumentList(sourceSymbolContext, diagnostics);

			var type = sourceSymbolContext.GetTypeOrError(
				qualifiedName.GetQualifiedName(),
				typeArguments,
				out var diagnostic);
			if (diagnostic != null)
				diagnostics.Add(diagnostic(new Location(qualifiedName)));
			if (diagnostic is null && isExported && type is IInterface { IsExported: false })
				diagnostics.Add(new Diagnostic(new Location(qualifiedName), ErrorCode.CannotUseUnexportedInterfaceFromExportedMember));
			return type;
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

		public static IType BindReturnType(this Method_signatureContext context, SourceSymbolContext sourceSymbolContext, bool isExported, DiagnosticBag diagnostics)
		{
			return context.type_declaration().type().BindType(sourceSymbolContext, isExported, diagnostics);
		}

		public static ImmutableArray<IParameter> BindParameters(
			this ParametersContext context,
			SourceSymbolContext sourceSymbolContext,
			bool isExported,
			DiagnosticBag diagnostics)
		{
			var parameters =
				context
				.parameter()
				.Select(x => new SourceParameter(x, sourceSymbolContext, isExported, diagnostics))
				.ToImmutableArray<IParameter>();

			foreach (var (param, index) in parameters.Select((x, i) => (x, i)))
			{
				if (parameters.Take(index).FirstOrDefault(x => x.Name == param.Name) is { } sameName)
				{
					diagnostics.Add(new Diagnostic(
						new Location(context.parameter(index)),
						ErrorCode.ParametersShareNames,
						ImmutableArray.Create<object?>(param, sameName)));
				}
			}
			return parameters;
		}

		public static ImmutableArray<ITypeParameter> BindTypeParameters(
			this Type_parameter_listContext context,
			SourceSymbolContext sourceSymbolContext,
			bool isExported,
			DiagnosticBag diagnostics)
		{
			var parameters =
				context
				.type_parameter()
				.Select(x => new SourceTypeParameter(x, sourceSymbolContext, isExported, diagnostics))
				.ToImmutableArray<ITypeParameter>();

			foreach (var (param, index) in parameters.Select((x, i) => (x, i)))
			{
				if ((parameters.Take(index).FirstOrDefault(x => x.Name == param.Name)
					?? sourceSymbolContext.GetTypeParameter(param.Name)) is { } sameName)
				{
					diagnostics.Add(new Diagnostic(
						new Location(context.type_parameter(index)),
						ErrorCode.TypeParametersShareNames,
						ImmutableArray.Create<object?>(param, sameName)));
				}
			}
			return parameters;
		}

		public static IExpression BindExpression(
			this ExpressionContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics)
		{
			return context switch
			{
				New_object_expressionContext _ => NewObjectExpression.Instance,
				Object_patching_expressionContext opec
					=> new ObjectPatchingExpression(opec, methodBodySymbolContext, diagnostics),
				Binary_operator_expressionContext boec
					=> new BinaryOperatorExpression(boec, methodBodySymbolContext, diagnostics),
				Prefix_unary_operator_expressionContext puoec
					=> new PrefixUnaryOperatorExpression(puoec, methodBodySymbolContext, diagnostics),
				Literal_expressionContext lec => new LiteralExpression(lec, diagnostics),
				Static_invocation_expressionContext siec
					=> new StaticInvocationExpression(siec, methodBodySymbolContext, diagnostics),
				Member_invocation_expressionContext miec
					=> new MemberInvocationExpression(miec, methodBodySymbolContext, diagnostics),
				Conditional_expressionContext cec => new ConditionalExpression(cec, methodBodySymbolContext, diagnostics),
				Parenthesized_expressionContext pec => pec.expression().BindExpression(methodBodySymbolContext, diagnostics),
				Local_reference_expressionContext lrec => new LocalReferenceExpression(lrec, methodBodySymbolContext, diagnostics),
				Match_expressionContext mec => new MatchExpression(mec, methodBodySymbolContext, diagnostics),
				_ => throw Release.Fail($"unexpected expression: {context}")
			};
		}

		public static ImmutableArray<IExpression> BindArguments(
			this InvocationContext context,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics)
		{
			return context
				.arguments()
				.expression()
				.Select(x => x.BindExpression(methodBodySymbolContext, diagnostics))
				.ToImmutableArray();
		}

		public static IStatement BindStatement(
			this Method_statementContext context,
			int ordinalPositionInMethod,
			MethodBodySymbolContext methodBodySymbolContext,
			DiagnosticBag diagnostics,
			out ILocal? local)
		{
			if (context.declaration_statement() is { } declarationStatement)
			{
				var result = new DeclarationStatement(declarationStatement, ordinalPositionInMethod, methodBodySymbolContext, diagnostics);
				local = result.Local;
				return result;
			}
			if (context.return_statement() is { } returnStatement)
			{
				local = null;
				return new ReturnStatement(returnStatement, ordinalPositionInMethod, methodBodySymbolContext, diagnostics);
			}
			throw Release.Fail($"unexpected statement: {context}");
		}
	}
}
