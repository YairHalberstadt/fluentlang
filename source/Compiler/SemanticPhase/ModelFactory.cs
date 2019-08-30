using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Model;
using System;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

public static class ModelFactory
{
	public static QualifiedName QualifiedName(Qualified_nameContext qualified_NameContext)
	{
		return qualified_NameContext
			.UPPERCASE_IDENTIFIER()
			.Aggregate((QualifiedName?)null, (l, r) => new QualifiedName(r.Symbol.Text, l))!;
	}

	public static Primitive Primitive(Base_typeContext base_TypeContext)
	{
		if (base_TypeContext.BOOL() != null)
			return FluentLang.Compiler.Model.Primitive.Bool;
		if (base_TypeContext.INT() != null)
			return FluentLang.Compiler.Model.Primitive.Int;
		if (base_TypeContext.DOUBLE() != null)
			return FluentLang.Compiler.Model.Primitive.Double;
		if (base_TypeContext.STRING() != null)
			return FluentLang.Compiler.Model.Primitive.String;
		if (base_TypeContext.CHAR() != null)
			return FluentLang.Compiler.Model.Primitive.Char;
		Release.Fail($"unexpected primitive: {base_TypeContext}");
		throw new Exception(); // unreachable
	}
}
