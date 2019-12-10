using FluentLang.Compiler.Symbols;

namespace FluentLang.Compiler.Emit
{
	public static class CSharpNameEscaper
	{
		public static string Escape(QualifiedName name) =>
			name.ToString().Replace("_", "__").Replace(".", "_dot_");
		public static QualifiedName Unescape(string name) => 
			QualifiedName.Parse(name.Replace("__", "$").Replace("_dot_", ".").Replace("$", "_"));

	}
}
