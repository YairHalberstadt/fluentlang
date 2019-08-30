using Antlr4.Runtime.Misc;
using FluentLang.Compiler.Generated;
using FluentLang.Compiler.Helpers;
using FluentLang.Compiler.Model;
using FluentLang.Compiler.Model.Diagnostic;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static FluentLang.Compiler.Generated.FluentLangParser;

namespace FluentLang.Compiler.SemanticPhase
{
	public class MetadataListener : FluentLangParserBaseListener
	{
		public ISemanticModel SemanticModel { get; private set; }
		public ImmutableList<Diagnostic> Diagnostics { get; private set; }

		private QualifiedName? _currentNameSpace;
		private Interface? _parsedInterface;
		private TypeKey? _parsedType;

		private ImmutableArray<QualifiedName> _openDirectives = ImmutableArray<QualifiedName>.Empty;

		private readonly Stack<ImmutableArray<InterfaceMethod>.Builder> _interfaceMethods = new Stack<ImmutableArray<InterfaceMethod>.Builder>();

		private readonly Stack<ImmutableArray<InterfaceMethodSet>.Builder> _interfaceMethodSets = new Stack<ImmutableArray<InterfaceMethodSet>.Builder>();

		private readonly Stack<ImmutableArray<InterfaceReference>.Builder> _additiveInterfaces = new Stack<ImmutableArray<InterfaceReference>.Builder>();

		private readonly Stack<ImmutableArray<Parameter>.Builder> _parameters = new Stack<ImmutableArray<Parameter>.Builder>();

		private ImmutableArray<QualifiedName> OpenedNamespaces => 
			_openDirectives.Concat(
				_currentNameSpace?.Flatten(x => x.Parent) 
				?? Array.Empty<QualifiedName>()).ToImmutableArray();

		public MetadataListener(ISemanticModel semanticModel, ImmutableList<Diagnostic> diagnostics)
		{
			SemanticModel = semanticModel;
			Diagnostics = diagnostics;
		}

		public override void EnterOpen_directives([NotNull] Open_directivesContext context)
		{
			_openDirectives = context.GetRuleContexts<Open_directiveContext>()
					.Select(x => ModelFactory.QualifiedName(x.qualified_name())).ToImmutableArray()!;
		}

		public override void EnterNamespace_declaration([NotNull] Namespace_declarationContext context)
		{
			var name = ModelFactory.QualifiedName(context.qualified_name());
			_currentNameSpace = _currentNameSpace?.Append(name) ?? name;
		}

		public override void ExitNamespace_declaration([NotNull] Namespace_declarationContext context)
		{
			var countToAscend = context.qualified_name().UPPERCASE_IDENTIFIER().Length;
			for(var i = 0; i < countToAscend; i++)
				_currentNameSpace = _currentNameSpace?.Parent;
		}

		public override void EnterAnonymous_interface_declaration([NotNull] Anonymous_interface_declarationContext context)
		{
			_interfaceMethodSets.Push(ImmutableArray.CreateBuilder<InterfaceMethodSet>());
			_additiveInterfaces.Push(ImmutableArray.CreateBuilder<InterfaceReference>());
		}

		public override void ExitAnonymous_interface_declaration([NotNull] Anonymous_interface_declarationContext context)
		{
			var methodSets = _interfaceMethodSets.Pop();
			var additiveInterfaces = _additiveInterfaces.Pop();
			if (context.Parent is Interface_declarationContext idc)
			{
				//TODO: deal with named interface declared inside method
				//TODO: deal with exports
				var nameSymbol = idc.UPPERCASE_IDENTIFIER().Symbol;
				var name = new QualifiedName(nameSymbol.Text, _currentNameSpace);

				var @interface = new Interface(additiveInterfaces.ToImmutable(), methodSets.ToImmutable(), name);

				var model = SemanticModel.TryWith(@interface);
				if (model is null)
				{
					Diagnostics = Diagnostics.Add(new Diagnostic(
						new Location(nameSymbol),
						ErrorCode.DuplicateInterfaceDeclaration,
						ImmutableArray.Create<object>(name)));
				}
				else
				{
					SemanticModel = model;
				}
			}
			else
			{
				_parsedInterface = new Interface(additiveInterfaces.ToImmutable(), methodSets.ToImmutable());
			}
		}

		public override void EnterSimple_anonymous_interface_declaration([NotNull] Simple_anonymous_interface_declarationContext context)
		{
			if (context.qualified_name() is { } name)
			{
				_additiveInterfaces.Peek().Add(new InterfaceReference(OpenedNamespaces, ModelFactory.QualifiedName(name)));
			}
			else
			{
				_interfaceMethods.Push(ImmutableArray.CreateBuilder<InterfaceMethod>());
			}
		}

		public override void ExitSimple_anonymous_interface_declaration([NotNull] Simple_anonymous_interface_declarationContext context)
		{
			if (!(context.qualified_name() is { } name))
			{
				var methods = _interfaceMethods.Pop();
				if (methods.Count > 0)
				{
					_interfaceMethodSets.Peek().Add(new InterfaceMethodSet(methods.ToImmutable()));
				}
			}
		}

		public override void EnterMethod_signature([NotNull] Method_signatureContext context)
		{
			_parameters.Push(ImmutableArray.CreateBuilder<Parameter>());
		}

		public override void ExitMethod_signature([NotNull] Method_signatureContext context)
		{
			var parameters = _parameters.Pop();
			Release.Assert(_parsedType != null);
			var returnType = _parsedType!.Value;
			_parsedType = null;
			if (context.Parent is Interface_member_declarationContext)
			{
				var method = new InterfaceMethod(context.UPPERCASE_IDENTIFIER().Symbol.Text, returnType, parameters.ToImmutable());
				_interfaceMethods.Peek().Add(method);
			}
			//TODO: method declarations
		}

		public override void ExitParameter([NotNull] ParameterContext context)
		{
			Release.Assert(_parsedType != null);
			_parameters.Peek().Add(new Parameter(context.LOWERCASE_IDENTIFIER().Symbol.Text, _parsedType!.Value));
			_parsedType = null;
		}

		public override void ExitType([NotNull] TypeContext context)
		{
			if (context.qualified_name() is { } qualifiedName)
			{
				_parsedType = new TypeKey(new InterfaceReference(OpenedNamespaces, ModelFactory.QualifiedName(qualifiedName)));
			}
			else if (context.base_type() is { } primitive)
			{
				_parsedType = new TypeKey(ModelFactory.Primitive(primitive));
			}
			else if (context.anonymous_interface_declaration() is { })
			{
				Release.Assert(_parsedInterface != null);
				_parsedType = new TypeKey(_parsedInterface!);
				_parsedInterface = null;
			}
			else
			{
				Release.Fail($"unexpected type: {context}");
			}
		}
	}
}