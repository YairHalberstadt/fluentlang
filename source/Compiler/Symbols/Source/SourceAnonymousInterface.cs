﻿using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.ErrorSymbols;
using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.Symbols.Source
{
	internal sealed class SourceAnonymousInterface : SymbolBase, IInterface
	{
		private readonly Anonymous_interface_declarationContext _context;
		private readonly SourceSymbolContext _sourceSymbolContext;

		private readonly Lazy<ImmutableArray<IInterfaceMethod>> _methods;

		public SourceAnonymousInterface(
			Anonymous_interface_declarationContext context,
			SourceSymbolContext sourceSymbolContext,
			bool isExported,
			DiagnosticBag diagnostics) : base(diagnostics)
		{
			_context = context;
			_sourceSymbolContext = sourceSymbolContext;
			IsExported = isExported;

			_methods = new Lazy<ImmutableArray<IInterfaceMethod>>(GenerateMethods);
		}

		public bool IsExported { get; }
		public QualifiedName? FullyQualifiedName => null;
		public ImmutableArray<ITypeParameter> TypeParameters => ImmutableArray<ITypeParameter>.Empty;
		public ImmutableArray<IType> TypeArguments => ImmutableArray<IType>.Empty;
		public ImmutableArray<IInterfaceMethod> Methods => _methods.Value;

		private ImmutableArray<IInterfaceMethod> GenerateMethods()
		{
			return

				// Get methods defined in method sets
				_context
				.simple_anonymous_interface_declaration()
				.SelectMany(x => x.interface_member_declaration())
				.Select(x => new SourceInterfaceMethod(x.method_signature(), _sourceSymbolContext, IsExported, _diagnostics))

				// add methods defined in additive interfaces
				.Concat(
					_context
					.simple_anonymous_interface_declaration()
					.Select(x => x.named_type_reference())
					.OfType<Named_type_referenceContext>()
					.Select(x =>
					{
						var type = _sourceSymbolContext.GetTypeOrError(
							x.qualified_name().GetQualifiedName(),
							x.type_argument_list().BindTypeArgumentList(_sourceSymbolContext, _diagnostics),
							out var diagnostic); ;
						if (diagnostic != null)
						{
							_diagnostics.Add(diagnostic(new Location(x)));
						}

						if (!(type is IInterface @interface))
						{
							_diagnostics.Add(new Diagnostic(
								new Location(x),
								ErrorCode.CanOnlyCombineInterfaces,
								ImmutableArray.Create<object?>(x.qualified_name().GetQualifiedName(), type)));
							@interface = ErrorInterface.Instance;
						}
						return (@interface, name: x);
					})
					.Where(x =>
					{
						// We have to do this check here, or we'll end up with infinite recursion
						if (x.@interface.FullyQualifiedName == FullyQualifiedName)
						{
							_diagnostics.Add(new Diagnostic(
								new Location(x.name),
								ErrorCode.CannotReferenceSelfAsAdditiveInterface,
								ImmutableArray.Create<object?>(x.name)));
							return false;
						}
						return true;
					})
					.SelectMany(x => x.@interface.Methods))
				.ToImmutableArray();

			/* We don't remove duplicate methods, since:
			 * a) this is inneficient.
			 * b) this can force binding, which we want to avoid if possible.
			 * c) It is very tricky to avoid infinite recursion. For example:
			 * 
			 * ```fluentlang
			 * interface I 
			 * {
			 *     M1(param1 : I) : bool;
			 *     M1(param1 : J) : bool;
			 * }
			 * 
			 * interface J
			 * {
			 *     M1(param1 : I) : bool;
			 *     M1(param1 : J) : bool;
			 * }
			 * ```
			 * 
			 * In order to check if I.M1(I) and I.M1(J) are duplicates you need to check if I is equavalent to J.
			 * But to do that you're going to ask for all of I's methods. 
			 * And even if you avoid the direct recursion there, you're going to ask for all of J's methods,
			 * which requires deduplicating J's methods. Which requires checking if I.M1(I) and I.M1(J) are duplicates.
			 * 
			 * d) In most cases we don't actually care. To see if interfaces are equavalent, we check that for every method a defines,
			 * b defines an equavalent method. But we get the same result even if these methods are duplicated.
			 * Similiarly applies for subtyping.
			 */
		}

		protected override void EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _methods.Value;
		}
	}
}