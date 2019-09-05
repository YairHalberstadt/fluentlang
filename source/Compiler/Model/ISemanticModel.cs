namespace FluentLang.Compiler.Model
{
	public interface ISemanticModel
	{
		bool TryGetInterface(QualifiedName fullyQualifiedName, out IInterface i);
		ISemanticModel? TryWith(IInterface i);
		ISemanticModel? TryWith(Method m);
		ISemanticModel With(IInterface i);
	}
}