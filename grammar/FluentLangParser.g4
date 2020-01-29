parser grammar FluentLangParser;

options {   tokenVocab = FluentLangLexer; }

compilation_unit
	: open_directives namespace_member_declaration* EOF
	;

open_directives
	: open_directive*
	;

open_directive
	: OPEN qualified_name SEMICOLON
	;

qualified_name
    : UPPERCASE_IDENTIFIER (DOT UPPERCASE_IDENTIFIER)*
    ;

namespace_member_declaration
	: namespace_declaration
	| interface_declaration
	| method_declaration
	;

namespace_declaration
    : NAMESPACE qualified_name OPEN_BRACE namespace_member_declaration* CLOSE_BRACE
    ;

interface_declaration
    : EXPORT? INTERFACE UPPERCASE_IDENTIFIER anonymous_interface_declaration
    ;

anonymous_interface_declaration
    : simple_anonymous_interface_declaration (PLUS simple_anonymous_interface_declaration)*
    ;

simple_anonymous_interface_declaration
    : OPEN_BRACE interface_member_declaration* CLOSE_BRACE
    | qualified_name
    ;

interface_member_declaration
    : method_signature SEMICOLON
    ;

method_signature
    : UPPERCASE_IDENTIFIER OPEN_PARENS parameters CLOSE_PARENS type_declaration
    ;

parameters
    : (parameter (COMMA parameter)*)?
    ;

parameter
    : LOWERCASE_IDENTIFIER type_declaration
    ;

type_declaration
    : COLON type
    ;

type
    : qualified_name
    | primitive_type
    | anonymous_interface_declaration
    | union
    ;

primitive_type
    : BOOL
    | INT
    | DOUBLE
    | CHAR
    | STRING
    ;

union
    : union_part_type (LOGICAL_OR union_part_type)+
    ;

union_part_type
    : qualified_name
    | primitive_type
    | anonymous_interface_declaration
    ;

method_declaration
    : EXPORT? method_signature method_body
    ;

method_body
    : OPEN_BRACE (method_statement | method_declaration | interface_declaration)* CLOSE_BRACE
    ;

 method_statement
    : declaration_statement
    | return_statement
    ;

declaration_statement
    : LET LOWERCASE_IDENTIFIER type_declaration? ASSIGNMENT expression SEMICOLON
    | DISCARD ASSIGNMENT expression SEMICOLON
    ;

return_statement
    : RETURN expression SEMICOLON
    ;

expression
    : empty_interface                                                     #new_object_expression
    | expression PLUS object_patch (COMMA object_patch)*                  #object_patching_expression
    | expression operator expression                                      #binary_operator_expression
    | prefix_unary_operator expression                                    #prefix_unary_operator_expression
    | literal                                                             #literal_expression
    | qualified_name invocation                                           #static_invocation_expression
    | expression DOT UPPERCASE_IDENTIFIER invocation                      #member_invocation_expression
    | IF OPEN_PARENS expression CLOSE_PARENS expression ELSE expression   #conditional_expression
    | OPEN_PARENS expression CLOSE_PARENS                                 #parenthesized_expression
    | LOWERCASE_IDENTIFIER                                                #local_reference_expression
    ;

empty_interface
    : OPEN_BRACE CLOSE_BRACE
    ;

object_patch
    : fully_qualified_method
    | MIXIN expression
    ;

fully_qualified_method
    : qualified_name
//  | qualified_name OPEN_PARENS fully_qualified_method_parameters CLOSE_PARENS type_declaration
    ;

//fully_qualified_method_parameters
//    : type (COMMA type)*
//    ;

operator
    : PLUS
    | MINUS
    | STAR
    | DIV
    | PERCENT
    | LT
    | GT
    | OP_EQ
    | OP_NE
    | OP_LE
    | OP_GE
    ;

prefix_unary_operator
    : MINUS
    ;

literal
    : LITERAL_TRUE
    | LITERAL_FALSE
    | INTEGER_LITERAL
    | REAL_LITERAL
    | CHARACTER_LITERAL
    | REGULAR_STRING
    ;

invocation
    : OPEN_PARENS arguments CLOSE_PARENS
    ;

arguments
    : (expression (COMMA expression)*)?
    ;

/*
 ***********************************************************************************
 *                                   METADATA RULES                                *
 *********************************************************************************** 
 */

 parameter_metadata
    : parameter EOF
    ;

return_type_metadata
    : type EOF
    ;

anonymous_interface_declaration_metadata
    : anonymous_interface_declaration EOF
    ;

full_qualified_name_metadata
    : qualified_name
    ;
