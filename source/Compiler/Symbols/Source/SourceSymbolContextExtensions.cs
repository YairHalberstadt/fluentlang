using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Symbols.ErrorTypes;
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
			List<IInterface> possibleInterfaces;

			if (name.Parent is null)
			{
				// local interfaces have priority over top level interfaces, 
				// and an inner local interface hides an outer local interface

				// local interfaces can only have simple names, so we can skip this check if the name is not simple

				var scope = context.Scope;
				while (scope != null)
				{
					possibleInterfaces = scope.LocalInterfaces.Where(x => x.FullyQualifiedName == name).ToList();
					if (possibleInterfaces.Count > 0)
						return possibleInterfaces;

					scope = scope.DeclaringMethod;
				}
			}

			var assemblies = context.Assembly.ReferencedAssemblies;

			// types in the innermost namespace have priority over types in its parent namespace, 
			// which have priority over types in its parent etc. 
			var @namespace = context.NameSpace;
			while(@namespace != null)
			{
				var possibleName = name.Prepend(@namespace);

				possibleInterfaces = LookupPossibleName(possibleName).ToList();

				if (possibleInterfaces.Count > 0)
					return possibleInterfaces;

				@namespace = @namespace?.Parent;
			}

			// Types in the global namespace have priority over types defined in imports
			possibleInterfaces = LookupPossibleName(name).ToList();

			if (possibleInterfaces.Count > 0)
				return possibleInterfaces;

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

			return Array.Empty<IInterface>();

			IEnumerable<IInterface> LookupPossibleName(QualifiedName possibleName)
			{
				return assemblies.Select(x =>
				{
					x.TryGetInterface(possibleName, out var @interface);
					return @interface;
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
				diagnostic = l => new Diagnostic(l, ErrorCode.InterfaceNotFound, ImmutableArray.Create<object?>(possibleInterfaces.Cast<object?>().Prepend(name).ToImmutableArray()));
				return ErrorInterface.Instance;
			}

			diagnostic = default;
			return possibleInterfaces[0];
		}
	}
}

