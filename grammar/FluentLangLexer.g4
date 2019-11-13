lexer grammar FluentLangLexer;

channels { COMMENTS_CHANNEL }

SINGLE_LINE_COMMENT:     '//'  InputCharacter*    -> channel(COMMENTS_CHANNEL);

WHITESPACES:   (Whitespace | NewLine)+            -> channel(HIDDEN);

//operators and punctuators
PLUS:                     '+';
MINUS:                    '-';
STAR:                     '*';
DIV:                      '/';
PERCENT:                  '%';
LT:                       '<';
GT:                       '>';
OP_EQ:                    '==';
OP_NE:                    '!=';
OP_LE:                    '<=';
OP_GE:                    '>=';
ASSIGNMENT:               '=';
OPEN_BRACE:               '{';
CLOSE_BRACE:              '}';
OPEN_PARENS:              '(';
CLOSE_PARENS:             ')';
DOT:                      '.';
COMMA:                    ',';
COLON:                    ':';
SEMICOLON:                ';';
DISCARD:                  '_';

//baseTypes
BOOL:                     'bool';
INT:                      'int';
DOUBLE:                   'double';
CHAR:                     'char';
STRING:                   'string';

//literals
LITERAL_TRUE:                        'true';
LITERAL_FALSE:                       'false';
INTEGER_LITERAL:                     [0-9]+;
REAL_LITERAL:                        [0-9]* '.' [0-9]+;
CHARACTER_LITERAL:                   '\'' (~['\\\r\n\u0085\u2028\u2029] | CommonCharacter) '\'';
REGULAR_STRING:                      '"'  (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter)* '"';

//other keywords
NAMESPACE: 'namespace';
INTERFACE: 'interface';
RETURN: 'return';
IF: 'if';
ELSE: 'else';
MIXIN: 'mixin';
EXPORT: 'export';
OPEN: 'open';
LET: 'let';

//identifiers

UPPERCASE_IDENTIFIER: [A-Z] IdentifierTail;
LOWERCASE_IDENTIFIER: [a-z] IdentifierTail;

//fragments

fragment IdentifierTail: ([a-z] | [A-Z] | [0-9] | '_')*;

fragment InputCharacter:       ~[\r\n\u0085\u2028\u2029];

fragment CommonCharacter
	: SimpleEscapeSequence
	| HexEscapeSequence
	| UnicodeEscapeSequence
	;

fragment SimpleEscapeSequence
	: '\\\''
	| '\\"'
	| '\\\\'
	| '\\0'
	| '\\a'
	| '\\b'
	| '\\f'
	| '\\n'
	| '\\r'
	| '\\t'
	| '\\v'
	;
fragment HexEscapeSequence
	: '\\x' HexDigit
	| '\\x' HexDigit HexDigit
	| '\\x' HexDigit HexDigit HexDigit
	| '\\x' HexDigit HexDigit HexDigit HexDigit
	;

fragment NewLine
	: '\r\n' | '\r' | '\n'
	| '\u0085' // <Next Line CHARACTER (U+0085)>'
	| '\u2028' //'<Line Separator CHARACTER (U+2028)>'
	| '\u2029' //'<Paragraph Separator CHARACTER (U+2029)>'
	;

fragment Whitespace
	: UnicodeClassZS //'<Any Character With Unicode Class Zs>'
	| '\u0009' //'<Horizontal Tab Character (U+0009)>'
	| '\u000B' //'<Vertical Tab Character (U+000B)>'
	| '\u000C' //'<Form Feed Character (U+000C)>'
	;

fragment UnicodeClassZS
	: '\u0020' // SPACE
	| '\u00A0' // NO_BREAK SPACE
	| '\u1680' // OGHAM SPACE MARK
	| '\u180E' // MONGOLIAN VOWEL SEPARATOR
	| '\u2000' // EN QUAD
	| '\u2001' // EM QUAD
	| '\u2002' // EN SPACE
	| '\u2003' // EM SPACE
	| '\u2004' // THREE_PER_EM SPACE
	| '\u2005' // FOUR_PER_EM SPACE
	| '\u2006' // SIX_PER_EM SPACE
	| '\u2008' // PUNCTUATION SPACE
	| '\u2009' // THIN SPACE
	| '\u200A' // HAIR SPACE
	| '\u202F' // NARROW NO_BREAK SPACE
	| '\u3000' // IDEOGRAPHIC SPACE
	| '\u205F' // MEDIUM MATHEMATICAL SPACE
	;

fragment UnicodeEscapeSequence
	: '\\u' HexDigit HexDigit HexDigit HexDigit
	| '\\U' HexDigit HexDigit HexDigit HexDigit HexDigit HexDigit HexDigit HexDigit
	;

fragment HexDigit : [0-9] | [A-F] | [a-f];