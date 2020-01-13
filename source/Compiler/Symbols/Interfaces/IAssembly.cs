using FluentLang.Compiler.Compilation;
using FluentLang.Compiler.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static MoreLinq.Extensions.DistinctByExtension;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IAssembly : ISymbol
	{
		public QualifiedName Name { get; }
		public Version Version { get; }
		public ImmutableArray<IAssembly> ReferencedAssemblies { get; }
		public ImmutableArray<IAssembly> ReferencedAssembliesAndSelf { get; }
		public ImmutableArray<IInterface> Interfaces { get; }
		public ImmutableArray<IMethod> Methods { get; }

		internal sealed IEnumerable<IAssembly> CalculateReferencedAssemblies(ImmutableArray<IAssembly> directlyReferencedAssemblies, DiagnosticBag diagnostics)
		{
			{
				var allByName =
					directlyReferencedAssemblies
					.SelectMany(x => x.ReferencedAssembliesAndSelf)
					.Concat(directlyReferencedAssemblies)
					.GroupBy(x => x.Name)
					.Select(x => x.DistinctBy(x => x.Version)).ToList();

				foreach (var multipleVersions in allByName.Where(x => x.Count() > 1))
				{
					diagnostics.Add(new Diagnostic(
						new Location(),
						ErrorCode.MultipleVersionsOfSameAssembly,
						multipleVersions.ToImmutableArray<object?>()));
				}

				return
					allByName
					.Select(x => x.First());
			}
		}

		public bool TryGetInterface(QualifiedName fullyQualifiedName, [NotNullWhen(true)] out IInterface? @interface);
		public bool TryGetMethod(QualifiedName fullyQualifiedName, [NotNullWhen(true)] out IMethod? method);
		public CompilationResult CompileAssembly(
			out ImmutableArray<byte> assemblyBytes,
			out ImmutableArray<byte> csharpBytes,
			out ImmutableArray<byte> pdbBytes);

		public bool TryGetAssemblyBytes(out ImmutableArray<byte> bytes);
	}
}

