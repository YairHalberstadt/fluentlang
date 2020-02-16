using FluentLang.Compiler.Diagnostics;
using FluentLang.Compiler.Symbols.Interfaces.MethodBody;
using FluentLang.Compiler.Symbols.Substituted;
using FluentLang.Compiler.Symbols.Visitor;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static MoreLinq.Extensions.MaxByExtension;

namespace FluentLang.Compiler.Symbols.Interfaces
{
	public interface IMethod : IVisitableSymbol
	{
		T IVisitableSymbol.Visit<T>(ISymbolVisitor<T> visitor)
			=> visitor.Visit(this);
		public bool IsExported { get; }
		public QualifiedName FullyQualifiedName { get; }
		public string Name => FullyQualifiedName.Name;
		public QualifiedName? Namespace => FullyQualifiedName.Parent;
		public IType ReturnType { get; }
		public ImmutableArray<ITypeParameter> TypeParameters { get; }
		public ImmutableArray<IParameter> Parameters { get; }
		public ImmutableArray<IParameterLocal> ParameterLocals { get; }
		public ImmutableArray<IInterface> LocalInterfaces { get; }
		public ImmutableArray<IMethod> LocalMethods { get; }
		public IMethod? DeclaringMethod { get; }
		public IAssembly DeclaringAssembly { get; }
		public ImmutableArray<IStatement> Statements { get; }
		public IDeclarationStatement? InScopeAfter { get; }
		protected ImmutableArray<IDeclaredLocal> DirectlyCapturedDeclaredLocals { get; }
		protected ImmutableArray<IMethod> InvokedLocalMethods { get; }
		private IEnumerable<IDeclaredLocal> CalculateTransitivelyCapturedDeclaredLocals(HashSet<IMethod> visitedMethods)
		{
			visitedMethods.Add(this);
			return
				DirectlyCapturedDeclaredLocals
				.Concat(InvokedLocalMethods
					.Where(x => !visitedMethods.Contains(x))
					.SelectMany(x => x.CalculateTransitivelyCapturedDeclaredLocals(visitedMethods)));
		}

		//TODO: once base calls are sorted make this protected.
		internal sealed IDeclarationStatement? CalculateInScopeAfter()
		{
			return
				DeclaringMethod is null
				? null
				: CalculateTransitivelyCapturedDeclaredLocals(new HashSet<IMethod>())
					.Select(x => x.Declaration)
					.Intersect(DeclaringMethod.Statements.OfType<IDeclarationStatement>())
					.MaxBy(x => x.OrdinalPositionInMethod)
					.FirstOrDefault();
		}

		internal IMethod Substitute(IReadOnlyDictionary<ITypeParameter, IType> substitutions)
			=> new SubstitutedMethod(this, substitutions);
	}
}

