using Antlr4.Runtime;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using DiagnosticErrorListener = FluentLang.Compiler.Parsing.DiagnosticErrorListener;

namespace FluentLang.Compiler.Symbols.Metadata
{
	public static class Utils
	{
		public static T? GetAttribute<T>(this MethodInfo method) where T : Attribute
		{
			return Attribute.GetCustomAttribute(method, typeof(T)) as T;
		}

		public static T[] GetAttributes<T>(this Assembly assembly) where T : Attribute
		{
			return (T[])Attribute.GetCustomAttributes(assembly, typeof(T));
		}

		public static T Parse<T>(string source, Func<FluentLangParser, T> getT, DiagnosticBag diagnostics) where T : ParserRuleContext
		{
			using var reader = new StringReader(source);

			var input = new AntlrInputStream(reader);
			var lexer = new FluentLangLexer(input);
			var tokenStream = new CommonTokenStream(lexer);
			var parser = new FluentLangParser(tokenStream);

			parser.RemoveErrorListeners();
			parser.AddErrorListener(new DiagnosticErrorListener(diagnostics));

			var t = getT(parser);
			diagnostics.AddRange(diagnostics);
			return t;
		}
	}
}
