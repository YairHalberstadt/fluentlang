using Antlr4.Runtime.Tree.Xpath;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FluentLang.Compiler.Model
{
	public abstract class Type
	{
		public bool IsNamedType => FullyQualifiedName != null;
		public abstract QualifiedName? FullyQualifiedName { get; }
		public bool IsEquivalentTo(Type other, ISemanticModel model) => IsEquivalentTo(other, null, model);
		internal abstract bool IsEquivalentTo(Type? other, Stack<(Type, Type)>? dependantEqualities, ISemanticModel model);
		public abstract bool IsSubTypeOf(Type other, ISemanticModel model);
	}
}
