using System;
using System.Runtime.Serialization;

namespace FluentLang.flc
{
	[Serializable]
	public class FlcException : Exception
	{
		public FlcException(string? message) : base(message)
		{
		}

		public FlcException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected FlcException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
