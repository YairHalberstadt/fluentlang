using FluentLang.Compiler.Model;
using System;
using System.Text;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public static class InterfaceBuilderGenerator
	{
		public static void Generate(IInterface @interface, StringBuilder stringBuilder)
		{
			stringBuilder.AppendLine("new InterfaceBuilder");
			stringBuilder.AppendLine("{");
			if (@interface.FullyQualifiedName != null)
			{
				stringBuilder.AppendLine($"FullyQualifiedName = \"{@interface.FullyQualifiedName}\",");
			}
			if (@interface.MethodSets.Length != 0)
			{
				stringBuilder.AppendLine("MethodSets =");
				stringBuilder.AppendLine("{");
				foreach(var methodSet in @interface.MethodSets)
				{
					Generate(methodSet, stringBuilder);
					stringBuilder.AppendLine(",");
				}
				stringBuilder.AppendLine("},");
			}
			if (@interface.AdditiveInterfaces.Length != 0)
			{
				stringBuilder.AppendLine("AdditiveInterfaces =");
				stringBuilder.AppendLine("{");
				foreach (var interfaceRef in @interface.AdditiveInterfaces)
				{
					Generate(interfaceRef, stringBuilder);
					stringBuilder.AppendLine(",");
				}
				stringBuilder.AppendLine("},");
			}
			stringBuilder.Append("}");
		}

		public static void Generate(InterfaceReference interfaceRef, StringBuilder stringBuilder)
		{
			stringBuilder.AppendLine($"new InterfaceReferenceBuilder(\"{interfaceRef.PartiallyQualifiedName}\")");
			stringBuilder.AppendLine("{");
			if (interfaceRef.ImportedNamespaces.Length != 0)
			{
				stringBuilder.AppendLine("ImportedNamespaces =");
				stringBuilder.AppendLine("{");
				foreach (var @namespace in interfaceRef.ImportedNamespaces)
				{
					stringBuilder.AppendLine($"\"{@namespace}\",");
				}
				stringBuilder.AppendLine("},");
			}
			stringBuilder.Append("}");
		}

		public static void Generate(IInterfaceMethodSet methodSet, StringBuilder stringBuilder)
		{
			if (methodSet.Methods.Length > 0)
			{
				stringBuilder.AppendLine("new[]");
			}
			else
			{
				stringBuilder.AppendLine("new InterfaceMethodBuilder[]");
			}
			stringBuilder.AppendLine("{");
			foreach (var method in methodSet.Methods)
			{
				Generate(method, stringBuilder);
				stringBuilder.AppendLine(",");
			}
			stringBuilder.Append("}");
		}

		public static void Generate(IInterfaceMethod method, StringBuilder stringBuilder)
		{
			stringBuilder.AppendLine("new InterfaceMethodBuilder");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine($"Name = \"{method.Name}\",");
			stringBuilder.Append($"ReturnType = ");
			Generate(method.ReturnType, stringBuilder);
			stringBuilder.AppendLine(",");
			if (method.Parameters.Length != 0)
			{
				stringBuilder.AppendLine("Parameters =");
				stringBuilder.AppendLine("{");
				foreach (var param in method.Parameters)
				{
					stringBuilder.AppendLine("(");
					stringBuilder.AppendLine($"\"{param.Name}\",");
					Generate(param.Type, stringBuilder);
					stringBuilder.AppendLine();
					stringBuilder.AppendLine("),");
				}
				stringBuilder.AppendLine("},");
			}
			stringBuilder.Append("}");
		}

		public static void Generate(TypeKey returnType, StringBuilder stringBuilder)
		{
			stringBuilder.Append("new TypeKeyBuilder(");
			if (returnType.Interface is { } @interface)
				Generate(@interface, stringBuilder);
			else if (returnType.InterfaceReference is { } interfaceRef)
				Generate(interfaceRef, stringBuilder);
			else if (returnType.Primitive is { } primitive)
				Generate(primitive, stringBuilder);
			stringBuilder.Append(")");
		}

		public static void Generate(Primitive primitive, StringBuilder stringBuilder)
		{
			stringBuilder.Append($"Primitive.");
			stringBuilder.Append(primitive switch
			{
				var p when p == Primitive.Bool => nameof(Primitive.Bool),
				var p when p == Primitive.Char => nameof(Primitive.Char),
				var p when p == Primitive.Double => nameof(Primitive.Double),
				var p when p == Primitive.Int => nameof(Primitive.Int),
				var p when p == Primitive.String => nameof(Primitive.String),
				_ => throw new InvalidOperationException("This location is thought to be unreachable")
			});
		}

		public static string Indent(string code)
		{
			var countTabs = 0;
			var lines = code.Split(Environment.NewLine);
			for (var i = 0; i < lines.Length; i++)
			{
				var line = lines[i];
				if (line.StartsWith("}") || line.StartsWith(")"))
				{
					countTabs--;
				}
				lines[i] = new string('\t', countTabs) + line;
				if (line.StartsWith("{") || line.StartsWith("("))
				{
					countTabs++;
				}
			}
			return string.Join(Environment.NewLine, lines);
		}
	}
}
