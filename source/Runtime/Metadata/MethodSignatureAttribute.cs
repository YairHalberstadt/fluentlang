using System;

namespace FluentLang.Runtime.Metadata
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class MethodSignatureAttribute : Attribute
	{
		public MethodSignatureAttribute(string fullyQualifiedName, string returnType, string[] parameters, string[] typeParameters)
		{
			FullyQualifiedName = fullyQualifiedName;
			ReturnType = returnType;
			Parameters = parameters;
			TypeParameters = typeParameters;
		}

		public string FullyQualifiedName { get; }
		public string ReturnType { get; }
		public string[] TypeParameters { get; }
		public string[] Parameters { get; }
	}
}
