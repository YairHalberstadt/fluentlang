using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Misc;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Model;
using FluentLang.Compiler.Model.Diagnostic;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.SemanticPhase
{
	public class TopLevelMetadataVisitor : FluentLangParserBaseVisitor<object>
	{
		private static object _dummyReturn = new object();

		public ISemanticModel SemanticModel { get; private set; }
		public ImmutableList<Diagnostic> Diagnostics { get; private set; }

		private QualifiedName? _currentNameSpace;

		private readonly List<QualifiedName> _openDirectives = new List<QualifiedName>();

		public TopLevelMetadataVisitor(ISemanticModel semanticModel, ImmutableList<Diagnostic> diagnostics)
		{
			SemanticModel = semanticModel;
			Diagnostics = diagnostics;
		}

		private ImmutableArray<QualifiedName> OpenedNamespaces =>
			_openDirectives.Concat(
				_currentNameSpace?.Flatten(x => x.Parent)
				?? Array.Empty<QualifiedName>()).ToImmutableArray();

		public override object VisitOpen_directives([NotNull] FluentLangParser.Open_directivesContext context)
		{
			_openDirectives.AddRange(
				context
				.GetRuleContexts<Open_directiveContext>()
				.Select(x => x.qualified_name().GetQualifiedName()));

			return DefaultResult;
		}

		public override object VisitNamespace_declaration([NotNull] Namespace_declarationContext context)
		{
			var name = context.qualified_name().GetQualifiedName();
			_currentNameSpace = _currentNameSpace?.Append(name) ?? name;

			var result = base.VisitNamespace_declaration(context);

			var countToAscend = context.qualified_name().UPPERCASE_IDENTIFIER().Length;
			for (var i = 0; i < countToAscend; i++)
				_currentNameSpace = _currentNameSpace?.Parent;

			return result;
		}

		public override object VisitInterface_declaration([NotNull] Interface_declarationContext context)
		{
			var @interface = context.BindInterface(OpenedNamespaces, null, _currentNameSpace);

			var model = SemanticModel.TryWith(@interface);
			if (model is null)
			{
				Diagnostics = Diagnostics.Add(new Diagnostic(
					new Location(context.UPPERCASE_IDENTIFIER()),
					ErrorCode.DuplicateInterfaceDeclaration,
					ImmutableArray.Create<object>(@interface.FullyQualifiedName!)));
			}
			else
			{
				SemanticModel = model;
			}

			return DefaultResult;
		}

		public override object VisitMethod_declaration([NotNull] Method_declarationContext context)
		{
			return DefaultResult;
		}

		protected override object DefaultResult => _dummyReturn;

		protected override object AggregateResult(object aggregate, object nextResult) => _dummyReturn;
	}
}
