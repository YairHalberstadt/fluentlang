using FluentLang.Compiler.Symbols.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static MoreLinq.Extensions.MaxByExtension;
using static FluentLang.Compiler.Generated.FluentLangParser;
using System.Diagnostics.CodeAnalysis;
using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Source.MethodBody;

namespace FluentLang.Compiler.Symbols.Source
{
	internal sealed class SourceAssembly : IAssembly
	{
		private readonly Lazy<ImmutableArray<IAssembly>> _referencedAssemblies;
		private readonly Lazy<ImmutableArray<IDocument>> _documents;
		private readonly Lazy<IReadOnlyDictionary<QualifiedName, IInterface>> _interfacesByName;
		private readonly Lazy<ImmutableArray<IInterface>> _interfaces;
		private readonly Lazy<IReadOnlyDictionary<QualifiedName, IMethod>> _methodsByName;
		private readonly Lazy<ImmutableArray<IMethod>> _methods;

		private readonly DiagnosticBag _diagnostics;
		private readonly Lazy<ImmutableArray<Diagnostic>> _allDiagnostics;

		public SourceAssembly(
			QualifiedName name,
			Interfaces.Version version,
			ImmutableArray<IAssembly> directlyReferencedAssemblies,
			ImmutableArray<IDocument> documents)
		{
			_diagnostics = new DiagnosticBag(this);

			_referencedAssemblies = new Lazy<ImmutableArray<IAssembly>>(() =>
				directlyReferencedAssemblies
				.SelectMany(x => x.ReferencedAssemblies)
				.Concat(directlyReferencedAssemblies)
				.GroupBy(x => x.Name)
				.Select(x => x.MaxBy(x => x.Version).First())
				.Append(this)
				.ToImmutableArray());

			_documents = new Lazy<ImmutableArray<IDocument>>(() =>
				documents.Select(x =>
				{
					if (x.Diagnostics.Length > 0)
					{
						_diagnostics.AddRange(x.Diagnostics);
						return null; //TODO: Come up with a better error recovery strategy
					}
					return x;
				}).Where(x => x != null)
				.ToImmutableArray()!);

			_interfacesByName = new Lazy<IReadOnlyDictionary<QualifiedName, IInterface>>(GenerateInterfaces);
			_methodsByName = new Lazy<IReadOnlyDictionary<QualifiedName, IMethod>>(GenerateMethods);
			_interfaces = new Lazy<ImmutableArray<IInterface>>(() => _interfacesByName.Value.Values.ToImmutableArray());
			_methods = new Lazy<ImmutableArray<IMethod>>(() => _methodsByName.Value.Values.ToImmutableArray());
			Name = name;
			Version = version;

			_allDiagnostics = new Lazy<ImmutableArray<Diagnostic>>(() => 
			{
				_diagnostics.EnsureAllDiagnosticsCollectedForSymbol();
				return _diagnostics.ToImmutableArray();
			});
		}

		public ImmutableArray<IAssembly> ReferencedAssemblies => _referencedAssemblies.Value;

		public ImmutableArray<IInterface> Interfaces => _interfaces.Value;

		public ImmutableArray<IMethod> Methods => _methods.Value;

		public QualifiedName Name { get; }

		public Interfaces.Version Version { get; }

		public ImmutableArray<Diagnostic> AllDiagnostics => _allDiagnostics.Value;

		private IReadOnlyDictionary<QualifiedName, IInterface> GenerateInterfaces()
		{
			var dictionary = new Dictionary<QualifiedName, IInterface>();

			VisitNamespaces(bindInterface: BindInterface);

			return dictionary;

			void BindInterface(
				Interface_declarationContext interfaceDeclaration,
				SourceSymbolContext  sourceSymbolContext)
			{
				var name = new QualifiedName(interfaceDeclaration.UPPERCASE_IDENTIFIER().Symbol.Text, sourceSymbolContext.NameSpace);
				var @interface = new SourceInterface(
					interfaceDeclaration.anonymous_interface_declaration(),
					sourceSymbolContext,
					name,
					isExported: interfaceDeclaration.EXPORT() is { },
					_diagnostics);
				if (!dictionary.TryAdd(name, @interface))
				{
					_diagnostics.Add(
						new Diagnostic(
							new Location(interfaceDeclaration.UPPERCASE_IDENTIFIER()),
							ErrorCode.DuplicateInterfaceDeclaration,
							ImmutableArray.Create<object?>(@interface.FullyQualifiedName)));
				}
			}
		}

		private IReadOnlyDictionary<QualifiedName, IMethod> GenerateMethods()
		{
			var dictionary = new Dictionary<QualifiedName, IMethod>();

			VisitNamespaces(bindMethod: BindMethod);

			return dictionary;

			void BindMethod(
				Method_declarationContext methodDeclaration,
				SourceSymbolContext sourceSymbolContext)
			{
				var method = new SourceMethod(methodDeclaration, new MethodBodySymbolContext(sourceSymbolContext), _diagnostics);
				if (!dictionary.TryAdd(method.FullyQualifiedName, method))
				{
					_diagnostics.Add(
						new Diagnostic(
							new Location(methodDeclaration.method_signature().UPPERCASE_IDENTIFIER()),
							ErrorCode.DuplicateMethodDeclaration,
							ImmutableArray.Create<object?>(method.FullyQualifiedName)));
				}
			}
		}

		private void VisitNamespaces(
			Action<Interface_declarationContext, SourceSymbolContext>? bindInterface = null,
			Action<Method_declarationContext, SourceSymbolContext>? bindMethod = null)
		{
			foreach (var document in _documents.Value)
			{
				var imports = BindImports(document);
				BindNameSpace(null, imports, document.SyntaxTree.namespace_member_declaration());
			}

			void BindNameSpace(QualifiedName? @namespace, ImmutableArray<QualifiedName> imports, Namespace_member_declarationContext[] context)
			{
				var sourceSymbolContext = new SourceSymbolContext(null, this, imports, @namespace);

				foreach (var declaration in context)
				{
					if (declaration.namespace_declaration() is { } namespaceDeclaration)
					{
						BindNameSpace(
							namespaceDeclaration.qualified_name().GetQualifiedName().Prepend(@namespace),
							imports,
							namespaceDeclaration.namespace_member_declaration());
					}
					else if (declaration.interface_declaration() is { } interfaceDeclaration)
					{
						bindInterface?.Invoke(interfaceDeclaration, sourceSymbolContext);
					}
					else if (declaration.method_declaration() is { } methodDeclaration)
					{
						bindMethod?.Invoke(methodDeclaration, sourceSymbolContext);
					}
				}
			}
		}

		private ImmutableArray<QualifiedName> BindImports(IDocument document)
		{
			return
				document
				.SyntaxTree
				.open_directives()
				.open_directive()
				.Select(x => x.qualified_name().GetQualifiedName())
				.ToImmutableArray();
		}

		public bool TryGetInterface(QualifiedName fullyQualifiedName, [NotNullWhen(true)] out IInterface? @interface)
		{
			return _interfacesByName.Value.TryGetValue(fullyQualifiedName, out @interface);
		}

		public bool TryGetMethod(QualifiedName fullyQualifiedName, [NotNullWhen(true)] out IMethod? method)
		{
			return _methodsByName.Value.TryGetValue(fullyQualifiedName, out method);
		}

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _referencedAssemblies.Value;
			_ = _documents.Value;
			_ = _interfacesByName.Value;
			_ = _interfaces.Value;
			_ = _methodsByName.Value;
			_ = _methods.Value;
		}
	}
}

