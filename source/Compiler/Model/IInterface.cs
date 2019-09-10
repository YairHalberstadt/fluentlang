using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace FluentLang.Compiler.Model
{
	public interface IInterface : IType
	{
		ImmutableArray<InterfaceReference> AdditiveInterfaces { get; }
		ImmutableArray<IInterfaceMethodSet> MethodSets { get; }
		IMethod? Scope { get; }
		ImmutableHashSet<IInterfaceMethod> AllInterfaceMethods(ISemanticModel model)
		{
			var mutable = AllInterfaceMethods(null, model);
			return mutable.ToImmutableHashSet(mutable.Comparer);
		}

		internal HashSet<IInterfaceMethod> AllInterfaceMethods(Stack<(IType, IType)>? dependantEqualities, ISemanticModel model)
		{
			return
				MethodSets.SelectMany(x => x.Methods)
				.Concat(AdditiveInterfaces
				.Select(x => x.GetPossibleInterfaces(model).SingleOrDefault())
				.Where(x => x != null && (x.FullyQualifiedName is null || x.FullyQualifiedName != FullyQualifiedName))
				.SelectMany(x => x.AllInterfaceMethods(dependantEqualities, model)))
				.ToHashSet(new InterfaceMethodEqualityComparer(dependantEqualities, model));
		}

		bool IType.IsSubTypeOf(IType other, ISemanticModel model)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (!(other is IInterface iOther))
			{
				return false;
			}

			var methods = AllInterfaceMethods(model);
			var otherMethods = iOther.AllInterfaceMethods(model);

			return methods.IsSubsetOf(otherMethods);
		}

		bool IType.IsEquivalentTo(IType? other, Stack<(IType, IType)>? dependantEqualities, ISemanticModel model)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (!(other is IInterface iOther))
			{
				return false;
			}

			if (dependantEqualities?.Contains((this, other)) ?? false)
			{
				return true;
			}

			dependantEqualities ??= new Stack<(IType, IType)>();
			dependantEqualities.Push((this, other));

			var methods = AllInterfaceMethods(dependantEqualities, model);
			var otherMethods = iOther.AllInterfaceMethods(dependantEqualities, model);

			(IType, IType) top;
			if (methods.Count != otherMethods.Count)
			{
				top = dependantEqualities.Pop();
				Debug.Assert((this, other) == top);
				return false;
			}

			var result = methods.SetEquals(otherMethods);
			top = dependantEqualities.Pop();
			Debug.Assert((this, other) == top);
			return result; ;
		}
	}
}