namespace FluentLang.Compiler.Model
{
	public interface ISemanticModel
	{
		bool TryGetInterface(QualifiedName fullyQualifiedName, out Interface i);
		ISemanticModel? TryWith(Interface i);
		ISemanticModel With(Interface i);
	}
}