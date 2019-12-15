using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces;
using FluentLang.Runtime.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace FluentLang.Compiler.Symbols.Metadata
{
	public class MetadataAssembly : IAssembly
	{
		private readonly Assembly _assembly;
		private readonly Lazy<IReadOnlyDictionary<QualifiedName, IMethod>> _methodsByName;
		private readonly Lazy<ImmutableArray<IMethod>> _methods;

		private readonly DiagnosticBag _diagnostics;
		private readonly Lazy<ImmutableArray<Diagnostic>> _allDiagnostics;

		public MetadataAssembly(Assembly assembly)
		{
			_diagnostics = new DiagnosticBag(this);
			_assembly = assembly;

			if (assembly.FullName is null)
				_diagnostics.Add(new Diagnostic(
					new Location(),
					ErrorCode.InvalidMetadataAssembly,
					ImmutableArray.Create<object?>("Metadata Assembly has no name")));

			Name = QualifiedName.Parse(_assembly.GetName().Name ?? "");
			_methodsByName = new Lazy<IReadOnlyDictionary<QualifiedName, IMethod>>(GenerateMethods);
			_methods = new Lazy<ImmutableArray<IMethod>>(() => _methodsByName.Value.Values.ToImmutableArray());

			_allDiagnostics = new Lazy<ImmutableArray<Diagnostic>>(() =>
			{
				_diagnostics.EnsureAllDiagnosticsCollectedForSymbol();
				return _diagnostics.ToImmutableArray();
			});
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

		public Interfaces.Version Version => throw new NotImplementedException();

		public ImmutableArray<IAssembly> ReferencedAssemblies => throw new NotImplementedException();

		public ImmutableArray<IInterface> Interfaces => throw new NotImplementedException();

		public ImmutableArray<IMethod> Methods => _methods.Value;

		public ImmutableArray<Diagnostic> AllDiagnostics => _allDiagnostics.Value;

		public bool TryGetInterface(QualifiedName fullyQualifiedName, [NotNullWhen(true)] out IInterface? @interface)
		{
			throw new NotImplementedException();
		}

		public bool TryGetMethod(QualifiedName fullyQualifiedName, [NotNullWhen(true)] out IMethod? method)
		{
			return _methodsByName.Value.TryGetValue(fullyQualifiedName, out method);
		}

		void ISymbol.EnsureAllLocalDiagnosticsCollected()
		{
			// Touch all lazy fields to force binding;

			_ = _methodsByName.Value;
			_ = _methods.Value;
		}
	}
}
