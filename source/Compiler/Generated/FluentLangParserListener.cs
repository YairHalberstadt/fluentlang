//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:\git\fluentlang\build-script\..\grammar\FluentLangParser.g4 by ANTLR 4.7.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace FluentLang.Compiler.Generated {
using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="FluentLangParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.2")]
[System.CLSCompliant(false)]
public interface IFluentLangParserListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.compilation_unit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCompilation_unit([NotNull] FluentLangParser.Compilation_unitContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.compilation_unit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCompilation_unit([NotNull] FluentLangParser.Compilation_unitContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.open_directives"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOpen_directives([NotNull] FluentLangParser.Open_directivesContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.open_directives"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOpen_directives([NotNull] FluentLangParser.Open_directivesContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.open_directive"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOpen_directive([NotNull] FluentLangParser.Open_directiveContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.open_directive"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOpen_directive([NotNull] FluentLangParser.Open_directiveContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.qualified_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterQualified_name([NotNull] FluentLangParser.Qualified_nameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.qualified_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitQualified_name([NotNull] FluentLangParser.Qualified_nameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.namespace_member_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNamespace_member_declaration([NotNull] FluentLangParser.Namespace_member_declarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.namespace_member_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNamespace_member_declaration([NotNull] FluentLangParser.Namespace_member_declarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.namespace_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNamespace_declaration([NotNull] FluentLangParser.Namespace_declarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.namespace_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNamespace_declaration([NotNull] FluentLangParser.Namespace_declarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.interface_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterface_declaration([NotNull] FluentLangParser.Interface_declarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.interface_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterface_declaration([NotNull] FluentLangParser.Interface_declarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.type_parameter_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType_parameter_list([NotNull] FluentLangParser.Type_parameter_listContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.type_parameter_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType_parameter_list([NotNull] FluentLangParser.Type_parameter_listContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.type_parameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType_parameter([NotNull] FluentLangParser.Type_parameterContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.type_parameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType_parameter([NotNull] FluentLangParser.Type_parameterContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.anonymous_interface_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnonymous_interface_declaration([NotNull] FluentLangParser.Anonymous_interface_declarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.anonymous_interface_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnonymous_interface_declaration([NotNull] FluentLangParser.Anonymous_interface_declarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.simple_anonymous_interface_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSimple_anonymous_interface_declaration([NotNull] FluentLangParser.Simple_anonymous_interface_declarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.simple_anonymous_interface_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSimple_anonymous_interface_declaration([NotNull] FluentLangParser.Simple_anonymous_interface_declarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.named_type_reference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNamed_type_reference([NotNull] FluentLangParser.Named_type_referenceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.named_type_reference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNamed_type_reference([NotNull] FluentLangParser.Named_type_referenceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.type_argument_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType_argument_list([NotNull] FluentLangParser.Type_argument_listContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.type_argument_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType_argument_list([NotNull] FluentLangParser.Type_argument_listContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.interface_member_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterface_member_declaration([NotNull] FluentLangParser.Interface_member_declarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.interface_member_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterface_member_declaration([NotNull] FluentLangParser.Interface_member_declarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.method_signature"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMethod_signature([NotNull] FluentLangParser.Method_signatureContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.method_signature"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMethod_signature([NotNull] FluentLangParser.Method_signatureContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.parameters"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParameters([NotNull] FluentLangParser.ParametersContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.parameters"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParameters([NotNull] FluentLangParser.ParametersContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.parameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParameter([NotNull] FluentLangParser.ParameterContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.parameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParameter([NotNull] FluentLangParser.ParameterContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.type_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType_declaration([NotNull] FluentLangParser.Type_declarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.type_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType_declaration([NotNull] FluentLangParser.Type_declarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType([NotNull] FluentLangParser.TypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType([NotNull] FluentLangParser.TypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.primitive_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrimitive_type([NotNull] FluentLangParser.Primitive_typeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.primitive_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrimitive_type([NotNull] FluentLangParser.Primitive_typeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.union"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnion([NotNull] FluentLangParser.UnionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.union"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnion([NotNull] FluentLangParser.UnionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.union_part_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnion_part_type([NotNull] FluentLangParser.Union_part_typeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.union_part_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnion_part_type([NotNull] FluentLangParser.Union_part_typeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.method_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMethod_declaration([NotNull] FluentLangParser.Method_declarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.method_declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMethod_declaration([NotNull] FluentLangParser.Method_declarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.method_body"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMethod_body([NotNull] FluentLangParser.Method_bodyContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.method_body"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMethod_body([NotNull] FluentLangParser.Method_bodyContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.method_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMethod_statement([NotNull] FluentLangParser.Method_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.method_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMethod_statement([NotNull] FluentLangParser.Method_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.declaration_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDeclaration_statement([NotNull] FluentLangParser.Declaration_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.declaration_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDeclaration_statement([NotNull] FluentLangParser.Declaration_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.return_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturn_statement([NotNull] FluentLangParser.Return_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.return_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturn_statement([NotNull] FluentLangParser.Return_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>local_reference_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLocal_reference_expression([NotNull] FluentLangParser.Local_reference_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>local_reference_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLocal_reference_expression([NotNull] FluentLangParser.Local_reference_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>binary_operator_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBinary_operator_expression([NotNull] FluentLangParser.Binary_operator_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>binary_operator_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBinary_operator_expression([NotNull] FluentLangParser.Binary_operator_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>conditional_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditional_expression([NotNull] FluentLangParser.Conditional_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>conditional_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditional_expression([NotNull] FluentLangParser.Conditional_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>parenthesized_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParenthesized_expression([NotNull] FluentLangParser.Parenthesized_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>parenthesized_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParenthesized_expression([NotNull] FluentLangParser.Parenthesized_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>prefix_unary_operator_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrefix_unary_operator_expression([NotNull] FluentLangParser.Prefix_unary_operator_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>prefix_unary_operator_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrefix_unary_operator_expression([NotNull] FluentLangParser.Prefix_unary_operator_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>piped_static_invocation_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPiped_static_invocation_expression([NotNull] FluentLangParser.Piped_static_invocation_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>piped_static_invocation_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPiped_static_invocation_expression([NotNull] FluentLangParser.Piped_static_invocation_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>member_invocation_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMember_invocation_expression([NotNull] FluentLangParser.Member_invocation_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>member_invocation_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMember_invocation_expression([NotNull] FluentLangParser.Member_invocation_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>match_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMatch_expression([NotNull] FluentLangParser.Match_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>match_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMatch_expression([NotNull] FluentLangParser.Match_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>literal_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLiteral_expression([NotNull] FluentLangParser.Literal_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>literal_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLiteral_expression([NotNull] FluentLangParser.Literal_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>static_invocation_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatic_invocation_expression([NotNull] FluentLangParser.Static_invocation_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>static_invocation_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatic_invocation_expression([NotNull] FluentLangParser.Static_invocation_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>new_object_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNew_object_expression([NotNull] FluentLangParser.New_object_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>new_object_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNew_object_expression([NotNull] FluentLangParser.New_object_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>object_patching_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterObject_patching_expression([NotNull] FluentLangParser.Object_patching_expressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>object_patching_expression</c>
	/// labeled alternative in <see cref="FluentLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitObject_patching_expression([NotNull] FluentLangParser.Object_patching_expressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.empty_interface"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEmpty_interface([NotNull] FluentLangParser.Empty_interfaceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.empty_interface"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEmpty_interface([NotNull] FluentLangParser.Empty_interfaceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.object_patch"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterObject_patch([NotNull] FluentLangParser.Object_patchContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.object_patch"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitObject_patch([NotNull] FluentLangParser.Object_patchContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.method_reference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMethod_reference([NotNull] FluentLangParser.Method_referenceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.method_reference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMethod_reference([NotNull] FluentLangParser.Method_referenceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOperator([NotNull] FluentLangParser.OperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOperator([NotNull] FluentLangParser.OperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.prefix_unary_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrefix_unary_operator([NotNull] FluentLangParser.Prefix_unary_operatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.prefix_unary_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrefix_unary_operator([NotNull] FluentLangParser.Prefix_unary_operatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLiteral([NotNull] FluentLangParser.LiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLiteral([NotNull] FluentLangParser.LiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.invocation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInvocation([NotNull] FluentLangParser.InvocationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.invocation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInvocation([NotNull] FluentLangParser.InvocationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.arguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArguments([NotNull] FluentLangParser.ArgumentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.arguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArguments([NotNull] FluentLangParser.ArgumentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.match_expression_arm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMatch_expression_arm([NotNull] FluentLangParser.Match_expression_armContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.match_expression_arm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMatch_expression_arm([NotNull] FluentLangParser.Match_expression_armContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.anonymous_interface_declaration_metadata"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnonymous_interface_declaration_metadata([NotNull] FluentLangParser.Anonymous_interface_declaration_metadataContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.anonymous_interface_declaration_metadata"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnonymous_interface_declaration_metadata([NotNull] FluentLangParser.Anonymous_interface_declaration_metadataContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.type_parameter_metadata"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType_parameter_metadata([NotNull] FluentLangParser.Type_parameter_metadataContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.type_parameter_metadata"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType_parameter_metadata([NotNull] FluentLangParser.Type_parameter_metadataContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.method_signature_metadata"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMethod_signature_metadata([NotNull] FluentLangParser.Method_signature_metadataContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.method_signature_metadata"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMethod_signature_metadata([NotNull] FluentLangParser.Method_signature_metadataContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FluentLangParser.interface_method_metadata"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterface_method_metadata([NotNull] FluentLangParser.Interface_method_metadataContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FluentLangParser.interface_method_metadata"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterface_method_metadata([NotNull] FluentLangParser.Interface_method_metadataContext context);
}
} // namespace FluentLang.Compiler.Generated
