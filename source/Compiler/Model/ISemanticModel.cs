namespace FluentLang.Compiler.Model
{
	public interface ISemanticModel
	{
		bool TryGetInterface(QualifiedName fullyQualifiedName, out IInterface i);
		bool TryGetMethod(QualifiedName fullyQualifiedName, out IMethod m);
		ISemanticModel? TryWith(IInterface i);
		ISemanticModel? TryWith(IMethod m);
	}
}