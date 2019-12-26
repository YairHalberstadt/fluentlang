using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Compiler.Symbols.Source;
using FluentLang.Runtime.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Version = FluentLang.Compiler.Symbols.Interfaces.Version;

namespace FluentLang.Compiler.Symbols.Metadata
{
	public class MetadataAssembly : IAssembly
	{
		private readonly Assembly _assembly;
		private readonly Lazy<IReadOnlyDictionary<QualifiedName, IMethod>> _methodsByName;
		private readonly Lazy<ImmutableArray<IMethod>> _methods;
		private readonly Lazy<IReadOnlyDictionary<QualifiedName, IInterface>> _interfacesByName;
		private readonly Lazy<ImmutableArray<IInterface>> _interfaces;
		private readonly Lazy<ImmutableArray<IAssembly>> _referencedAssembliesAndSelf;

		private readonly DiagnosticBag _diagnostics;
		private readonly Lazy<ImmutableArray<Diagnostic>> _allDiagnostics;


		public MetadataAssembly(Assembly assembly, ImmutableArray<IAssembly> dependencies)
		{
			_diagnostics = new DiagnosticBag(this);
			_assembly = assembly;

			if (assembly.FullName is null)
				_diagnostics.Add(new Diagnostic(
					new Location(),
					ErrorCode.InvalidMetadataAssembly,
					ImmutableArray.Create<object?>("Metadata Assembly has no name")));

			var assemblyFileVersionAttributes = assembly.GetAttributes<AssemblyFileVersionAttribute>();
			if (assemblyFileVersionAttributes.Length != 1)
			{
				_diagnostics.Add(new Diagnostic(
					new Location(),
					ErrorCode.InvalidMetadataAssembly,
					ImmutableArray.Create<object?>($"Metadata Assembly must have exactly one {nameof(AssemblyFileVersionAttribute)}")));
				Version = new Version(0, 0, 0);
			}
			else
			{
				var versionAttribute = assemblyFileVersionAttributes[0];
				if (!Version.TryParse(versionAttribute.Version, out var version))
				{
					_diagnostics.Add(new Diagnostic(
						new Location(),
						ErrorCode.InvalidMetadataAssembly,
						ImmutableArray.Create<object?>($"Version `{version}` is invalid")));
					Version = new Version(0, 0, 0);
				}
				else
				{
					Version = version;
				}
			}

			Name = QualifiedName.Parse(_assembly.GetName().Name ?? "");

			_referencedAssembliesAndSelf = new Lazy<ImmutableArray<IAssembly>>(
				() => ((IAssembly)this).CalculateReferencedAssembliesAndSelf(dependencies, _diagnostics));

			_methodsByName = new Lazy<IReadOnlyDictionary<QualifiedName, IMethod>>(GenerateMethods);
			_methods = new Lazy<ImmutableArray<IMethod>>(() => _methodsByName.Value.Values.ToImmutableArray());
			_interfacesByName = new Lazy<IReadOnlyDictionary<QualifiedName, IInterface>>(GenerateInterfaces);
			_interfaces = new Lazy<ImmutableArray<IInterface>>(() => _interfacesByName.Value.Values.ToImmutableArray());
			_allDiagnostics = new Lazy<ImmutableArray<Diagnostic>>(() =>
			{
				_diagnostics.EnsureAllDiagnosticsCollectedForSymbol();
				return _diagnostics.ToImmutableArray();
			});
		}

		private IReadOnlyDictionary<QualifiedName, IInterface> GenerateInterfaces()
		{
			var context = new SourceSymbolContext(
				scope: null,
				assembly: this,
				ImmutableArray<QualifiedName>.Empty,
				nameSpace: null);

			return
				_assembly
				.GetAttributes<InterfaceAttribute>()
				.Select(x => (IInterface)new SourceInterface(
					Utils.Parse(
						x.AnonymousInterfaceDeclaration,
						p => p.anonymous_interface_declaration_metadata().anonymous_interface_declaration(),
						_diagnostics),
					context,
					QualifiedName.Parse(x.FullyQualifiedName),
					isExported: true,
					_diagnostics))
				.ToDictionary(x => x.FullyQualifiedName!);
		}

		private IReadOnlyDictionary<QualifiedName, IMethod> GenerateMethods()
		{
			var assemblyLevelMethods = _assembly.ExportedTypes.FirstOrDefault(
				x =>
					x.Namespace is null
					&& x.Name == Emit.Utils.GetAssemblyLevelMethodsClassName(Name.ToString()));

			if (assemblyLevelMethods is null)
			{
				_diagnostics.Add(new Diagnostic(
					new Location(),
					ErrorCode.InvalidMetadataAssembly,
					ImmutableArray.Create<object?>(
						$"Metadata Assembly does not contain class named {Emit.Utils.GetAssemblyLevelMethodsClassName(Name.ToString())}")));
				return ImmutableDictionary<QualifiedName, IMethod>.Empty;
			}

			var methods = assemblyLevelMethods
				.GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Select(x => x.GetAttribute<MethodSignatureAttribute>()).ToList();

			if (methods.Any(x => x is null))
			{
				_diagnostics.Add(new Diagnostic(
					new Location(),
					ErrorCode.InvalidMetadataAssembly,
					ImmutableArray.Create<object?>(
						$"Metadata Assembly contains public method without {nameof(MethodSignatureAttribute)}")));
				return ImmutableDictionary<QualifiedName, IMethod>.Empty;
			}

			return
				methods
				.Select(x => (IMethod)new MetadataMethod(this, _diagnostics, x!))
				.ToDictionary(x => x.FullyQualifiedName);
		}

		public QualifiedName Name { get; }

		public Version Version { get; }

		public ImmutableArray<IAssembly> ReferencedAssembliesAndSelf => _referencedAssembliesAndSelf.Value;

		public ImmutableArray<IInterface> Interfaces => _interfaces.Value;

		public ImmutableArray<IMethod> Methods => _methods.Value;

		public ImmutableArray<Diagnostic> AllDiagnostics => _allDiagnostics.Value;

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
			_ = _referencedAssembliesAndSelf.Value;
			_ = _methodsByName.Value;
			_ = _methods.Value;
			_ = _interfacesByName.Value;
			_ = _interfaces.Value;
		}
	}
}
