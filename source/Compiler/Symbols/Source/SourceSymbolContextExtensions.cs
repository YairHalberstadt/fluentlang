using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.ErrorSymbols;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
namespace FluentLang.Compiler.Symbols.Source
{
	internal static class SourceSymbolContextExtensions
	{
		public static IType? GetTypeParameter(this SourceSymbolContext context, string name)
		{
			{
				if (context.CurrentLevelTypeParameters().FirstOrDefault(x => x.Name == name) is { } typeParameter)
				{
					return typeParameter;
				}
			}

			var scope = context.Scope;
			while (scope != null)
			{
				if (scope.TypeParameters.FirstOrDefault(x => x.Name == name) is { } typeParameter)
				{
					return typeParameter;
				}
				scope = scope.DeclaringMethod;
			}

			return null;
		}

		public static IReadOnlyList<IType> GetPossibleTypes(
			this SourceSymbolContext context,
			QualifiedName name,
			ImmutableArray<IType> typeArguments)
		{
			if (name.Parent is null && typeArguments.Length == 0)
			{
				if (context.GetTypeParameter(name.Name) is { } typeParameter)
				{
					return new[] { typeParameter };
				}
			}

			return GetPossibleTs(
				context,
				name,
				typeArguments,
				(IAssembly x, QualifiedName n, out IInterface? i) => x.TryGetInterface(n, out i),
				x => x.LocalInterfaces,
				x => x.FullyQualifiedName,
				x => x.TypeParameters);
		}

		public static IReadOnlyList<IMethod> GetPossibleMethods(this SourceSymbolContext context, QualifiedName name, ImmutableArray<IType> typeArguments)
		{
			return GetPossibleTs(
				context,
				name,
				typeArguments,
				(IAssembly x, QualifiedName n, out IMethod? i) => x.TryGetMethod(n, out i),
				x => x.LocalMethods,
				x => x.FullyQualifiedName,
				x => x.TypeParameters);
		}

		private delegate bool TryGetT<T>(IAssembly assembly, QualifiedName qualifiedName, [NotNullWhen(true)] out T? t) where T : class;
		private delegate ImmutableArray<T> GetLocalTs<T>(IMethod method);
		private delegate QualifiedName? GetFullyQualifiedName<T>(T t);
		private delegate ImmutableArray<ITypeParameter> GetTypeParameters<T>(T t);

		private static IReadOnlyList<T> GetPossibleTs<T>(
			SourceSymbolContext context,
			QualifiedName name,
			ImmutableArray<IType> typeArguments,
			TryGetT<T> tryGetT,
			GetLocalTs<T> getLocalTs,
			GetFullyQualifiedName<T> getFullyQualifiedName,
			GetTypeParameters<T> getTypeParameters) where T : class
		{
			List<T> possibleTs;

			if (name.Parent is null)
			{
				// local ts have priority over top level ts, 
				// and an inner local t hides an outer local t

				// local ts can only have simple names, so we can skip this check if the name is not simple

				var scope = context.Scope;
				while (scope != null)
				{
					possibleTs = getLocalTs(scope)
						.Where(
							x => getFullyQualifiedName(x) == name 
							&& getTypeParameters(x).Length == typeArguments.Length)
						.ToList();
					if (possibleTs.Count > 0)
						return possibleTs;

					scope = scope.DeclaringMethod;
				}
			}

			var assemblies = context.Assembly.ReferencedAssembliesAndSelf;

			// types in the innermost namespace have priority over types in its parent namespace, 
			// which have priority over types in its parent etc. 
			var @namespace = context.NameSpace;
			while (@namespace != null)
			{
				var possibleName = name.Prepend(@namespace);

				possibleTs = LookupPossibleName(possibleName).ToList();

				if (possibleTs.Count > 0)
					return possibleTs;

				@namespace = @namespace?.Parent;
			}

			// Types in the global namespace have priority over types defined in imports
			possibleTs = LookupPossibleName(name).ToList();

			if (possibleTs.Count > 0)
				return possibleTs;

			// look for types in imports
			if (name.Parent is null)
			{
				// imports only bring the top level types into scope, so they are only relevant if the name is simple;

				var possibleNames = context.Imports.Select(x => x.Append(name)).ToList();

				return
					possibleNames
					.SelectMany(LookupPossibleName)
					.ToList();
			}

			return Array.Empty<T>();

			IEnumerable<T> LookupPossibleName(QualifiedName possibleName)
			{
				return assemblies.Select(x =>
				{
					tryGetT(x, possibleName, out var t);
					return t;
				}).Where(x => x != null && getTypeParameters(x).Length == typeArguments.Length)!;
			}
		}

		public static IType GetTypeOrError(
			this SourceSymbolContext context,
			QualifiedName name,
			ImmutableArray<IType> typeArguments,
			out Func<Location, Diagnostic>? diagnostic)
		{
			diagnostic = default;
			var possibleTypes = context.GetPossibleTypes(name, typeArguments);

			if (possibleTypes.Count == 0)
			{
				diagnostic = l => new Diagnostic(l, ErrorCode.InterfaceNotFound, ImmutableArray.Create<object?>(name));
				return ErrorInterface.Instance;
			}

			if (possibleTypes.Count > 1)
			{
				diagnostic = l => new Diagnostic(l, ErrorCode.AmbigiousInterfaceReference, ImmutableArray.Create<object?>(possibleTypes.Cast<object?>().Prepend(name).ToImmutableArray()));
				return ErrorInterface.Instance;
			}

			var type = possibleTypes[0];
			if (typeArguments.Length > 0)
			{
				Release.Assert(type is IInterface);
				var @interface = (IInterface)type;
				var typeParameters = @interface.TypeParameters;
				HasValidTypeArguments(typeArguments, typeParameters, out diagnostic);
				type = type.Substitute(CreateTypeMap(typeArguments, typeParameters));
			}

			return type;
		}

		public static ImmutableArrayDictionary<ITypeParameter, IType> CreateTypeMap(
			ImmutableArray<IType> typeArguments,
			ImmutableArray<ITypeParameter> typeParameters)
		{
			return new ImmutableArrayDictionary<ITypeParameter, IType>(
				typeParameters
					.Zip(
						typeArguments,
						(a, b) => new KeyValuePair<ITypeParameter, IType>(a, b))
					.ToImmutableArray());
		}

		public static bool HasValidTypeArguments(
			ImmutableArray<IType> typeArguments,
			ImmutableArray<ITypeParameter> typeParameters,
			[NotNullWhenAttribute(false)] out Func<Location, Diagnostic>? diagnostic)
		{
			Release.Assert(typeArguments.Length == typeParameters.Length);
			for (var i = 0; i < typeArguments.Length; i++)
			{
				var typeArgument = typeArguments[i];
				var typeParameter = typeParameters[i];
				if (typeParameter.ConstrainedTo is { } constrainedTo
					&& !typeArgument.IsSubtypeOf(constrainedTo))
				{
					diagnostic = l => new Diagnostic(
						l,
						ErrorCode.TypeArgumentDoesntMatchConstraints,
						ImmutableArray.Create<object?>(typeArgument, typeParameter));
					return false;
				}
			}
			diagnostic = default;
			return true;
		}

		public static IMethod GetMethodOrError(
			this SourceSymbolContext context,
			QualifiedName name,
			ImmutableArray<IType> typeArguments,
			out Func<Location, Diagnostic>? diagnostic)
		{
			diagnostic = null;
			var possibleMethods = context.GetPossibleMethods(name, typeArguments);

			if (possibleMethods.Count == 0)
			{
				diagnostic = l => new Diagnostic(l, ErrorCode.MethodNotFound, ImmutableArray.Create<object?>(name));
				return ErrorMethod.Instance;
			}


			if (possibleMethods.Count > 1)
			{
				diagnostic = l => new Diagnostic(
					l,
					ErrorCode.AmbigiousMethodReference,
					ImmutableArray.Create<object?>(possibleMethods.Cast<object?>().Prepend(name).ToImmutableArray()));
				return ErrorMethod.Instance;
			}

			var method = possibleMethods[0];
			if (typeArguments.Length > 0)
			{
				var typeParameters = method.TypeParameters;
				HasValidTypeArguments(typeArguments, typeParameters, out diagnostic);
				method = method.Substitute(CreateTypeMap(typeArguments, typeParameters));
			}

			return method;
		}
	}
}