using System;
using System.Diagnostics;
using FluentLang.Compiler.Model;

namespace FluentLang.Compiler.Tests.Unit.TestHelpers
{
	public static class ModelEqualityHelpers
	{
		public static bool AreEqual(Interface? a, Interface? b)
		{
			if (a is null || b is null)
				return ReferenceEquals(a, b);

			if (!AreEqual(a.FullyQualifiedName, b.FullyQualifiedName))
				return false;

			if (a.AdditiveInterfaces.Length != b.AdditiveInterfaces.Length)
				return false;

			if (a.MethodSets.Length != b.MethodSets.Length)
				return false;

			for (var i = 0; i < a.AdditiveInterfaces.Length; i++)
			{
				var aIntRef = a.AdditiveInterfaces[i];
				var bIntRef = b.AdditiveInterfaces[i];

				if (!AreEqual(aIntRef, bIntRef))
					return false;
			}

			for (var i = 0; i < a.MethodSets.Length; i++)
			{
				var aMethSet = a.MethodSets[i];
				var bMethSet = b.MethodSets[i];

				if (!AreEqual(aMethSet, bMethSet))
					return false;
			}

			return true;
		}

		public static bool AreEqual(InterfaceMethodSet? a, InterfaceMethodSet? b)
		{
			if (a is null || b is null)
				return ReferenceEquals(a, b);

			if (a.Methods.Length != b.Methods.Length)
				return false;

			for (var i = 0; i < a.Methods.Length; i++)
			{
				var aMeth = a.Methods[i];
				var bMeth = b.Methods[i];

				if (!AreEqual(aMeth, bMeth))
					return false;
			}

			return true;
		}

		public static bool AreEqual(InterfaceMethod? a, InterfaceMethod? b)
		{
			if (a is null || b is null)
				return ReferenceEquals(a, b);

			if (a.Name != b.Name)
				return false;

			if (a.Parameters.Length != b.Parameters.Length)
				return false;

			if (!AreEqual(a.ReturnType, b.ReturnType))
				return false;

			for (var i = 0; i < a.Parameters.Length; i++)
			{
				var aParam = a.Parameters[i];
				var bParam = b.Parameters[i];

				if (!AreEqual(aParam, bParam))
					return false;
			}

			return true;
		}

		public static bool AreEqual(Parameter? a, Parameter? b)
		{
			if (a is null || b is null)
				return ReferenceEquals(a, b);

			if (a.Name != b.Name)
				return false;

			return AreEqual(a.Type, b.Type);
		}

		public static bool AreEqual(TypeKey a, TypeKey b)
		{
			if (!ReferenceEquals(a.Primitive, b.Primitive))
				return false;

			if (a.InterfaceReference is null != b.InterfaceReference is null)
				return false;

			if (a.InterfaceReference != null)
			{
				Debug.Assert(b.InterfaceReference != null);
				if (!AreEqual(a.InterfaceReference.Value, b.InterfaceReference.Value))
					return false;
			}

			return AreEqual(a.Interface, b.Interface);
		}

		public static bool AreEqual(InterfaceReference a, InterfaceReference b)
		{
			if (!AreEqual(a.PartiallyQualifiedName, b.PartiallyQualifiedName))
				return false;
			if (a.ImportedNamespaces.Length != b.ImportedNamespaces.Length)
				return false;

			for (var i = 0; i < a.ImportedNamespaces.Length; i++)
			{
				var aImpNS = a.ImportedNamespaces[i];
				var bImpNs = b.ImportedNamespaces[i];
				if (!AreEqual(aImpNS, bImpNs))
					return false;
			}
			return true;
		}

		public static bool AreEqual(QualifiedName? a, QualifiedName? b)
		{
			return a?.Equals(b) ?? b is null;
		}
	}
}
