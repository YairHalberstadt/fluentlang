using System;

namespace FluentLang.Runtime.Metadata
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class InterfaceAttribute : Attribute
	{
		public InterfaceAttribute(string fullyQualifiedName, string[] methods)
		{
			FullyQualifiedName = fullyQualifiedName;
			Methods = methods;
		}

		public string FullyQualifiedName { get; }
		public string[] Methods { get; }
	}
}
