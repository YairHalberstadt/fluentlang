using System;

namespace FluentLang.Runtime.Metadata
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class MethodSignatureAttribute : Attribute
	{
		public MethodSignatureAttribute(string signature, string[] requireMethodKeySignatures)
		{
			Signature = signature;
			RequireMethodKeySignatures = requireMethodKeySignatures;
		}

		public string Signature { get; }
		public string[] RequireMethodKeySignatures { get; }
	}
}
