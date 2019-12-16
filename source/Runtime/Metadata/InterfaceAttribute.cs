using System;

namespace FluentLang.Runtime.Metadata
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class InterfaceAttribute : Attribute
	{
		public InterfaceAttribute(string fullyQualifiedName, string anonymousInterfaceDeclaration)
		{
			FullyQualifiedName = fullyQualifiedName;
			AnonymousInterfaceDeclaration = anonymousInterfaceDeclaration;
		}

		public string FullyQualifiedName { get; }
		public string AnonymousInterfaceDeclaration { get; }
	}
}
