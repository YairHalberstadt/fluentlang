using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace FluentLang.Compiler.Model
{
	public sealed class Interface : Type
	{
		private ImmutableHashSet<InterfaceMethod>? _allMethodDefinitions;

		public Interface(
			ImmutableArray<InterfaceReference> additiveInterfaces,
			ImmutableArray<InterfaceMethodSet> methodSets,
			QualifiedName? fullyQualifiedName = null)
		{
			AdditiveInterfaces = additiveInterfaces;
			MethodSets = methodSets;
			FullyQualifiedName = fullyQualifiedName;
		}

		public ImmutableArray<InterfaceReference> AdditiveInterfaces { get; }

		public ImmutableArray<InterfaceMethodSet> MethodSets { get; }

		public override QualifiedName? FullyQualifiedName { get; }

		internal override bool IsEquivalentTo(Type? other, Stack<(Type, Type)>? dependantEqualities, ISemanticModel model)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (!(other is Interface iOther))
			{
				return false;
			}

			if (dependantEqualities?.Contains((this, other)) ?? false)
			{
				return true;
			}

			dependantEqualities ??= new Stack<(Type, Type)>();
			dependantEqualities.Push((this, other));

			var methods = AllInterfaceMethods(dependantEqualities, model);
			var otherMethods = iOther.AllInterfaceMethods(dependantEqualities, model);

			(Type, Type) top;
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

		public override bool IsSubTypeOf(Type other, ISemanticModel model)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (!(other is Interface iOther))
			{
				return false;
			}

			var methods = AllInterfaceMethods(model);
			var otherMethods = iOther.AllInterfaceMethods(model);

			return methods.IsSubsetOf(otherMethods);
		}

		public ImmutableHashSet<InterfaceMethod> AllInterfaceMethods(ISemanticModel model)
		{
			if (_allMethodDefinitions is null)
			{
				var mutable = AllInterfaceMethods(null, model);
				_allMethodDefinitions = mutable.ToImmutableHashSet(mutable.Comparer);
			}

			return _allMethodDefinitions;

		}

		internal HashSet<InterfaceMethod> AllInterfaceMethods(Stack<(Type, Type)>? dependantEqualities, ISemanticModel model)
		{
			return
				MethodSets.SelectMany(x => x.Methods)
				.Concat(AdditiveInterfaces
					.Select(x => x.GetPossibleInterfaces(model).SingleOrDefault())
					.Where(x => x != null && (x.FullyQualifiedName is null || x.FullyQualifiedName != FullyQualifiedName))
					.SelectMany(x => x.AllInterfaceMethods(dependantEqualities, model)))
				.ToHashSet(new InterfaceMethodEqualityComparer(dependantEqualities, model));
		}
	}
}
