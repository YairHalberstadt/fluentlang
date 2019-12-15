using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace FluentLang.Compiler.Symbols.Metadata
{
	public static class Utils
	{
		public static T? GetAttribute<T>(this MethodInfo method) where T : Attribute
		{
			return Attribute.GetCustomAttribute(method, typeof(T)) as T;
		}
	}
}
