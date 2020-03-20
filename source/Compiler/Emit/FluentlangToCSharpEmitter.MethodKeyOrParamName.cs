namespace FluentLang.Compiler.Emit
{
	public partial class FluentlangToCSharpEmitter
	{
		private abstract class MethodKeyOrParamName
		{
			private MethodKeyOrParamName() { }

			public sealed class MethodKey : MethodKeyOrParamName
			{
				public MethodKey(string value)
				{
					Value = value;
				}

				public string Value { get; }
			}

			public sealed class ParamName : MethodKeyOrParamName
			{
				public ParamName(string value)
				{
					Value = value;
				}

				public string Value { get; }
			}
		}
	}
}
