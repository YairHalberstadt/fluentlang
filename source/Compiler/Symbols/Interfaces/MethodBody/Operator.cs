namespace FluentLang.Compiler.Symbols.Interfaces.MethodBody
{
	public sealed class Operator
	{
		private readonly string _name;

		private Operator(string name) {
			_name = name;
		}

		public static Operator Plus { get; } = new Operator(nameof(Plus));
		public static Operator Minus { get; } = new Operator(nameof(Minus));
		public static Operator Multiply { get; } = new Operator(nameof(Multiply));
		public static Operator Divide { get; } = new Operator(nameof(Divide));
		public static Operator Remainder { get; } = new Operator(nameof(Remainder));
		public static Operator LessThan { get; } = new Operator(nameof(LessThan));
		public static Operator GreaterThan { get; } = new Operator(nameof(GreaterThan));
		public static Operator Equal { get; } = new Operator(nameof(Equal));
		public static Operator NotEqual { get; } = new Operator(nameof(NotEqual));
		public static Operator LessThanOrEqualTo { get; } = new Operator(nameof(LessThanOrEqualTo));
		public static Operator GreaterThanOrEqualTo { get; } = new Operator(nameof(GreaterThanOrEqualTo));

		public override string ToString()
		{
			return _name;
		}
	}
}
