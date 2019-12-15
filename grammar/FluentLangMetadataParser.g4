parser grammar FluentLangMetadataParser;

options {   tokenVocab = FluentLangLexer; }

parameter_metadata
    : parameter EOF
    ;

return_type_metadata
    : type_declaration EOF
    ;

interface_method_metadata
    : interface_method_signature EOF
    ;

full_qualified_name_metadata
    : qualified_name
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
    ;

qualified_name
    : UPPERCASE_IDENTIFIER (DOT UPPERCASE_IDENTIFIER)*
    ;

primitive_type
    : BOOL
    | INT
    | DOUBLE
    | CHAR
    | STRING
    ;

anonymous_interface_declaration
    : simple_anonymous_interface_declaration (PLUS simple_anonymous_interface_declaration)*
    ;

simple_anonymous_interface_declaration
    : OPEN_BRACE interface_member_declaration* CLOSE_BRACE
    | qualified_name
    ;

interface_member_declaration
    : interface_method_signature SEMICOLON
    ;

interface_method_signature
    : UPPERCASE_IDENTIFIER OPEN_PARENS parameters CLOSE_PARENS type_declaration
    ;