using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.SemanticPhase
{
	public static class ContextExtensions
	{
		public static IInterface BindInterface(
			this Interface_declarationContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod? scope,
			QualifiedName? currentNamespace)
		{
			var nameSymbol = context.UPPERCASE_IDENTIFIER().Symbol;
			var name = new QualifiedName(nameSymbol.Text, currentNamespace);

			return context.anonymous_interface_declaration().BindInterface(openedNamespaces, scope, name);
		}

		public static IInterface BindInterface(
			this Anonymous_interface_declarationContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod? scope,
			QualifiedName? fullyQualifiedName)
		{
			return new SourceInterface(context, openedNamespaces, fullyQualifiedName, scope);
		}

		public static ImmutableArray<InterfaceReference> BindAdditiveInterfaces(
			this Anonymous_interface_declarationContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod? scope)
		{
			return context
				.simple_anonymous_interface_declaration()
				.Select(x => x.qualified_name())
				.Where(x => x != null)
				.Select(x => new InterfaceReference(openedNamespaces, x.GetQualifiedName(), scope))
				.ToImmutableArray();
		}

		public static ImmutableArray<IInterfaceMethodSet> BindMethodSets(
			this Anonymous_interface_declarationContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod? scope)
		{
			return context
				.simple_anonymous_interface_declaration()
				.Select(x => x.qualified_name() is null ? x.interface_member_declaration() : null)
				.Where(x => x != null)
				.Select(x => (IInterfaceMethodSet)new SourceInterfaceMethodSet(x!, openedNamespaces, scope))
				.ToImmutableArray();
		}

		public static IInterfaceMethod BindMethod(
			this Interface_member_declarationContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod? scope)
		{
			return new SourceInterfaceMethod(context, openedNamespaces, scope);
		}

		public static string GetName(this Method_signatureContext context) => context.UPPERCASE_IDENTIFIER().Symbol.Text;

		public static TypeKey BindReturnType(
			this Method_signatureContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod? scope)
		{
			return context.type_declaration().type().BindType(openedNamespaces, scope);
		}

		public static ImmutableArray<Parameter> BindParameters(
			this Method_signatureContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod? scope)
		{
			return context.parameters()?.BindParameters(openedNamespaces, scope) ?? ImmutableArray<Parameter>.Empty;
		}

		public static ImmutableArray<Parameter> BindParameters(
			this ParametersContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod? scope)
		{
			return context.parameter().Select(x => x.BindParameter(openedNamespaces, scope)).ToImmutableArray();
		}

		public static Parameter BindParameter(
			this ParameterContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod? scope)
		{
			return new Parameter(
				context.LOWERCASE_IDENTIFIER().Symbol.Text,
				context.type_declaration().type().BindType(openedNamespaces, scope));
		}

		public static TypeKey BindType(
			this TypeContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod? scope)
		{
			if (context.qualified_name() is { } name)
				return new TypeKey(new InterfaceReference(openedNamespaces, name.GetQualifiedName(), scope));
			if (context.primitive_type() is { } primitive)
				return new TypeKey(primitive.BindPrimitive());
			if (context.anonymous_interface_declaration() is { } @interface)
				return new TypeKey(@interface.BindInterface(openedNamespaces, scope, null));
			throw Release.Fail($"unexpected type: {context}");
		}

		public static QualifiedName GetQualifiedName(this Qualified_nameContext context)
		{
			return context
				.UPPERCASE_IDENTIFIER()
				.Aggregate((QualifiedName?)null, (l, r) => new QualifiedName(r.Symbol.Text, l))!;
		}

		public static Primitive BindPrimitive(this Primitive_typeContext context)
		{
			if (context.BOOL() != null)
				return Model.Primitive.Bool;
			if (context.INT() != null)
				return Model.Primitive.Int;
			if (context.DOUBLE() != null)
				return Model.Primitive.Double;
			if (context.STRING() != null)
				return Model.Primitive.String;
			if (context.CHAR() != null)
				return Model.Primitive.Char;
			throw Release.Fail($"unexpected primitive: {context}");
		}

		public static IMethod BindMethod(
			this Method_declarationContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod? scope,
			QualifiedName? currentNamespace)
		{
			var name = context.method_signature().GetName();
			var fullyQualifiedName = new QualifiedName(name, currentNamespace);

			return new SourceMethod(context, openedNamespaces, fullyQualifiedName, scope);
		}

		public static ImmutableArray<IInterface> BindInterfaces(
			this Method_declarationContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod scope)
		{
			return context
				.method_body()
				.interface_declaration()
				.Select(x => x.BindInterface(openedNamespaces, scope, null))
				.ToImmutableArray();
		}

		public static ImmutableArray<IMethod> BindMethods(
			this Method_declarationContext context,
			ImmutableArray<QualifiedName> openedNamespaces,
			IMethod scope)
		{
			return context
				.method_body()
				.method_declaration()
				.Select(x => x.BindMethod(openedNamespaces, scope, null))
				.ToImmutableArray();
		}
	}
}
