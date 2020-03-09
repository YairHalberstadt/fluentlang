using System;

namespace FluentLang.Runtime.Metadata
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class InterfaceAttribute : Attribute
	{
		public InterfaceAttribute(string fullyQualifiedName, string anonymousInterfaceDeclaration, string[] typeParameters)
		{
			FullyQualifiedName = fullyQualifiedName;
			AnonymousInterfaceDeclaration = anonymousInterfaceDeclaration;
			TypeParameters = typeParameters;
		}

		public string FullyQualifiedName { get; }
		public string AnonymousInterfaceDeclaration { get; }
		public string[] TypeParameters { get; }
	}
}
