﻿using FluentLang.Compiler.Diagnostics;
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
		public static IReadOnlyList<IInterface> GetPossibleInterfaces(this SourceSymbolContext context, QualifiedName name)
		{
			return GetPossibleTs<IInterface>(
				context,
				name,
				(IAssembly x, QualifiedName n, out IInterface? i) => x.TryGetInterface(n, out i),
				x => x.LocalInterfaces,
				x => x.FullyQualifiedName);
		}

		public static IReadOnlyList<IMethod> GetPossibleMethods(this SourceSymbolContext context, QualifiedName name)
		{
			return GetPossibleTs<IMethod>(
				context,
				name,
				(IAssembly x, QualifiedName n, out IMethod? i) => x.TryGetMethod(n, out i),
				x => x.LocalMethods,
				x => x.FullyQualifiedName);
		}

		private delegate bool TryGetT<T>(IAssembly assembly, QualifiedName qualifiedName, [NotNullWhen(true)] out T? t) where T : class;
		private delegate ImmutableArray<T> GetLocalTs<T>(IMethod method);
		private delegate QualifiedName? GetFullyQualifiedName<T>(T t);

		private static IReadOnlyList<T> GetPossibleTs<T>(
			SourceSymbolContext context,
			QualifiedName name,
			TryGetT<T> tryGetT,
			GetLocalTs<T> getLocalTs,
			GetFullyQualifiedName<T> getFullyQualifiedName) where T : class
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
					possibleTs = getLocalTs(scope).Where(x => getFullyQualifiedName(x) == name).ToList();
					if (possibleTs.Count > 0)
						return possibleTs;

					scope = scope.DeclaringMethod;
				}
			}

			var assemblies = context.Assembly.ReferencedAssemblies;

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
				}).Where(x => x != null)!;
			}
		}

		public static IInterface GetInterfaceOrError(this SourceSymbolContext context, QualifiedName name, out Func<Location, Diagnostic>? diagnostic)
		{
			var possibleInterfaces = context.GetPossibleInterfaces(name);

			if (possibleInterfaces.Count == 0)
			{
				diagnostic = l => new Diagnostic(l, ErrorCode.InterfaceNotFound, ImmutableArray.Create<object?>(name));
				return ErrorInterface.Instance;
			}


			if (possibleInterfaces.Count > 1)
			{
				diagnostic = l => new Diagnostic(l, ErrorCode.AmbigiousInterfaceReference, ImmutableArray.Create<object?>(possibleInterfaces.Cast<object?>().Prepend(name).ToImmutableArray()));
				return ErrorInterface.Instance;
			}

			diagnostic = default;
			return possibleInterfaces[0];
		}


		public static IMethod GetMethodOrError(this SourceSymbolContext context, QualifiedName name, out Func<Location, Diagnostic>? diagnostic)
		{
			var possibleMethods = context.GetPossibleMethods(name);

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

			diagnostic = default;
			return possibleMethods[0];
		}
	}
}

