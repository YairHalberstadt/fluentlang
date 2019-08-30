using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace FluentLang.Compiler.Model.Diagnostic
{
	public class Diagnostic
	{
		public Diagnostic(Location location, ErrorCode errorCode, ImmutableArray<object> additionalContext = default)
		{
			Location = location;
			ErrorCode = errorCode;
			if (additionalContext.IsDefault)
				additionalContext = ImmutableArray<object>.Empty;
			AdditionalContext = additionalContext;
		}

		public Location Location { get; }
		public ErrorCode ErrorCode { get; }
		public ImmutableArray<object> AdditionalContext { get; } 
	}
}
