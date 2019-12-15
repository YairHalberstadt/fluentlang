namespace FluentLang.Compiler.Emit
{
	internal static class Utils
	{
		private const string ASSEMBLY_LEVEL_METHODS = "_AssemblyLevelMethods";
		public static string GetAssemblyLevelMethodsClassName(string assemblyName)
			=> assemblyName + ASSEMBLY_LEVEL_METHODS;
	}
}
