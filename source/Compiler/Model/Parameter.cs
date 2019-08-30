namespace FluentLang.Compiler.Model
{
	public class Parameter
	{
		public Parameter(string name, TypeKey type)
		{
			Type = type;
			Name = name;
		}

		public string Name { get; }

		public TypeKey Type { get; }
	}
}