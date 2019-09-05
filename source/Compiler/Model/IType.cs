using Antlr4.Runtime.Tree.Xpath;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FluentLang.Compiler.Model
{
	public interface IType
	{
		public sealed bool IsNamedType => FullyQualifiedName != null;
		public abstract QualifiedName? FullyQualifiedName { get; }

		public sealed bool IsEquivalentTo(IType other, ISemanticModel model) => IsEquivalentTo(other, null, model);
		internal abstract bool IsEquivalentTo(IType? other, Stack<(IType, IType)>? dependantEqualities, ISemanticModel model);

		public abstract bool IsSubTypeOf(IType other, ISemanticModel model);
	}
}
