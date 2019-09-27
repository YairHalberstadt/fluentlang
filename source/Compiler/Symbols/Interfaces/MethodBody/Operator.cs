namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public sealed class Operator
	{
		private Operator() { }

		public static Operator Plus { get; } = new Operator();
		public static Operator Minus { get; } = new Operator();
		public static Operator Multiply { get; } = new Operator();
		public static Operator Divide { get; } = new Operator();
		public static Operator Remainder { get; } = new Operator();
		public static Operator LessThan { get; } = new Operator();
		public static Operator GreaterThan { get; } = new Operator();
		public static Operator Equal { get; } = new Operator();
		public static Operator NotEqual { get; } = new Operator();
		public static Operator LessThanOrEqualTo { get; } = new Operator();
		public static Operator GreaterThanOrEqualTo { get; } = new Operator();
	}
}
