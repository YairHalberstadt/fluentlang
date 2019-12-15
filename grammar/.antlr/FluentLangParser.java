// Generated from c:\git\fluentlang\grammar\FluentLangParser.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class FluentLangParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.7.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		SINGLE_LINE_COMMENT=1, WHITESPACES=2, PLUS=3, MINUS=4, STAR=5, DIV=6, 
		PERCENT=7, LT=8, GT=9, OP_EQ=10, OP_NE=11, OP_LE=12, OP_GE=13, ASSIGNMENT=14, 
		OPEN_BRACE=15, CLOSE_BRACE=16, OPEN_PARENS=17, CLOSE_PARENS=18, DOT=19, 
		COMMA=20, COLON=21, SEMICOLON=22, DISCARD=23, BOOL=24, INT=25, DOUBLE=26, 
		CHAR=27, STRING=28, LITERAL_TRUE=29, LITERAL_FALSE=30, INTEGER_LITERAL=31, 
		REAL_LITERAL=32, CHARACTER_LITERAL=33, REGULAR_STRING=34, NAMESPACE=35, 
		INTERFACE=36, RETURN=37, IF=38, ELSE=39, MIXIN=40, EXPORT=41, OPEN=42, 
		LET=43, UPPERCASE_IDENTIFIER=44, LOWERCASE_IDENTIFIER=45;
	public static final int
		RULE_compilation_unit = 0, RULE_open_directives = 1, RULE_open_directive = 2, 
		RULE_qualified_name = 3, RULE_namespace_member_declaration = 4, RULE_namespace_declaration = 5, 
		RULE_interface_declaration = 6, RULE_anonymous_interface_declaration = 7, 
		RULE_simple_anonymous_interface_declaration = 8, RULE_interface_member_declaration = 9, 
		RULE_method_signature = 10, RULE_parameters = 11, RULE_parameter = 12, 
		RULE_type_declaration = 13, RULE_type = 14, RULE_primitive_type = 15, 
		RULE_method_declaration = 16, RULE_method_body = 17, RULE_method_statement = 18, 
		RULE_declaration_statement = 19, RULE_return_statement = 20, RULE_expression = 21, 
		RULE_empty_interface = 22, RULE_object_patch = 23, RULE_fully_qualified_method = 24, 
		RULE_operator = 25, RULE_prefix_unary_operator = 26, RULE_literal = 27, 
		RULE_invocation = 28, RULE_arguments = 29, RULE_parameter_metadata = 30, 
		RULE_return_type_metadata = 31, RULE_interface_method_metadata = 32, RULE_full_qualified_name_metadata = 33;
	public static final String[] ruleNames = {
		"compilation_unit", "open_directives", "open_directive", "qualified_name", 
		"namespace_member_declaration", "namespace_declaration", "interface_declaration", 
		"anonymous_interface_declaration", "simple_anonymous_interface_declaration", 
		"interface_member_declaration", "method_signature", "parameters", "parameter", 
		"type_declaration", "type", "primitive_type", "method_declaration", "method_body", 
		"method_statement", "declaration_statement", "return_statement", "expression", 
		"empty_interface", "object_patch", "fully_qualified_method", "operator", 
		"prefix_unary_operator", "literal", "invocation", "arguments", "parameter_metadata", 
		"return_type_metadata", "interface_method_metadata", "full_qualified_name_metadata"
	};

	private static final String[] _LITERAL_NAMES = {
		null, null, null, "'+'", "'-'", "'*'", "'/'", "'%'", "'<'", "'>'", "'=='", 
		"'!='", "'<='", "'>='", "'='", "'{'", "'}'", "'('", "')'", "'.'", "','", 
		"':'", "';'", "'_'", "'bool'", "'int'", "'double'", "'char'", "'string'", 
		"'true'", "'false'", null, null, null, null, "'namespace'", "'interface'", 
		"'return'", "'if'", "'else'", "'mixin'", "'export'", "'open'", "'let'"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, "SINGLE_LINE_COMMENT", "WHITESPACES", "PLUS", "MINUS", "STAR", "DIV", 
		"PERCENT", "LT", "GT", "OP_EQ", "OP_NE", "OP_LE", "OP_GE", "ASSIGNMENT", 
		"OPEN_BRACE", "CLOSE_BRACE", "OPEN_PARENS", "CLOSE_PARENS", "DOT", "COMMA", 
		"COLON", "SEMICOLON", "DISCARD", "BOOL", "INT", "DOUBLE", "CHAR", "STRING", 
		"LITERAL_TRUE", "LITERAL_FALSE", "INTEGER_LITERAL", "REAL_LITERAL", "CHARACTER_LITERAL", 
		"REGULAR_STRING", "NAMESPACE", "INTERFACE", "RETURN", "IF", "ELSE", "MIXIN", 
		"EXPORT", "OPEN", "LET", "UPPERCASE_IDENTIFIER", "LOWERCASE_IDENTIFIER"
	};
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "FluentLangParser.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public FluentLangParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}
	public static class Compilation_unitContext extends ParserRuleContext {
		public Open_directivesContext open_directives() {
			return getRuleContext(Open_directivesContext.class,0);
		}
		public TerminalNode EOF() { return getToken(FluentLangParser.EOF, 0); }
		public List<Namespace_member_declarationContext> namespace_member_declaration() {
			return getRuleContexts(Namespace_member_declarationContext.class);
		}
		public Namespace_member_declarationContext namespace_member_declaration(int i) {
			return getRuleContext(Namespace_member_declarationContext.class,i);
		}
		public Compilation_unitContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_compilation_unit; }
	}

	public final Compilation_unitContext compilation_unit() throws RecognitionException {
		Compilation_unitContext _localctx = new Compilation_unitContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_compilation_unit);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(68);
			open_directives();
			setState(72);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << NAMESPACE) | (1L << INTERFACE) | (1L << EXPORT) | (1L << UPPERCASE_IDENTIFIER))) != 0)) {
				{
				{
				setState(69);
				namespace_member_declaration();
				}
				}
				setState(74);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(75);
			match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Open_directivesContext extends ParserRuleContext {
		public List<Open_directiveContext> open_directive() {
			return getRuleContexts(Open_directiveContext.class);
		}
		public Open_directiveContext open_directive(int i) {
			return getRuleContext(Open_directiveContext.class,i);
		}
		public Open_directivesContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_open_directives; }
	}

	public final Open_directivesContext open_directives() throws RecognitionException {
		Open_directivesContext _localctx = new Open_directivesContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_open_directives);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(80);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==OPEN) {
				{
				{
				setState(77);
				open_directive();
				}
				}
				setState(82);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Open_directiveContext extends ParserRuleContext {
		public TerminalNode OPEN() { return getToken(FluentLangParser.OPEN, 0); }
		public Qualified_nameContext qualified_name() {
			return getRuleContext(Qualified_nameContext.class,0);
		}
		public TerminalNode SEMICOLON() { return getToken(FluentLangParser.SEMICOLON, 0); }
		public Open_directiveContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_open_directive; }
	}

	public final Open_directiveContext open_directive() throws RecognitionException {
		Open_directiveContext _localctx = new Open_directiveContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_open_directive);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(83);
			match(OPEN);
			setState(84);
			qualified_name();
			setState(85);
			match(SEMICOLON);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Qualified_nameContext extends ParserRuleContext {
		public List<TerminalNode> UPPERCASE_IDENTIFIER() { return getTokens(FluentLangParser.UPPERCASE_IDENTIFIER); }
		public TerminalNode UPPERCASE_IDENTIFIER(int i) {
			return getToken(FluentLangParser.UPPERCASE_IDENTIFIER, i);
		}
		public List<TerminalNode> DOT() { return getTokens(FluentLangParser.DOT); }
		public TerminalNode DOT(int i) {
			return getToken(FluentLangParser.DOT, i);
		}
		public Qualified_nameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_qualified_name; }
	}

	public final Qualified_nameContext qualified_name() throws RecognitionException {
		Qualified_nameContext _localctx = new Qualified_nameContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_qualified_name);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(87);
			match(UPPERCASE_IDENTIFIER);
			setState(92);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,2,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(88);
					match(DOT);
					setState(89);
					match(UPPERCASE_IDENTIFIER);
					}
					} 
				}
				setState(94);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,2,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Namespace_member_declarationContext extends ParserRuleContext {
		public Namespace_declarationContext namespace_declaration() {
			return getRuleContext(Namespace_declarationContext.class,0);
		}
		public Interface_declarationContext interface_declaration() {
			return getRuleContext(Interface_declarationContext.class,0);
		}
		public Method_declarationContext method_declaration() {
			return getRuleContext(Method_declarationContext.class,0);
		}
		public Namespace_member_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_namespace_member_declaration; }
	}

	public final Namespace_member_declarationContext namespace_member_declaration() throws RecognitionException {
		Namespace_member_declarationContext _localctx = new Namespace_member_declarationContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_namespace_member_declaration);
		try {
			setState(98);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,3,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(95);
				namespace_declaration();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(96);
				interface_declaration();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(97);
				method_declaration();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Namespace_declarationContext extends ParserRuleContext {
		public TerminalNode NAMESPACE() { return getToken(FluentLangParser.NAMESPACE, 0); }
		public Qualified_nameContext qualified_name() {
			return getRuleContext(Qualified_nameContext.class,0);
		}
		public TerminalNode OPEN_BRACE() { return getToken(FluentLangParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(FluentLangParser.CLOSE_BRACE, 0); }
		public List<Namespace_member_declarationContext> namespace_member_declaration() {
			return getRuleContexts(Namespace_member_declarationContext.class);
		}
		public Namespace_member_declarationContext namespace_member_declaration(int i) {
			return getRuleContext(Namespace_member_declarationContext.class,i);
		}
		public Namespace_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_namespace_declaration; }
	}

	public final Namespace_declarationContext namespace_declaration() throws RecognitionException {
		Namespace_declarationContext _localctx = new Namespace_declarationContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_namespace_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(100);
			match(NAMESPACE);
			setState(101);
			qualified_name();
			setState(102);
			match(OPEN_BRACE);
			setState(106);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << NAMESPACE) | (1L << INTERFACE) | (1L << EXPORT) | (1L << UPPERCASE_IDENTIFIER))) != 0)) {
				{
				{
				setState(103);
				namespace_member_declaration();
				}
				}
				setState(108);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(109);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interface_declarationContext extends ParserRuleContext {
		public TerminalNode INTERFACE() { return getToken(FluentLangParser.INTERFACE, 0); }
		public TerminalNode UPPERCASE_IDENTIFIER() { return getToken(FluentLangParser.UPPERCASE_IDENTIFIER, 0); }
		public Anonymous_interface_declarationContext anonymous_interface_declaration() {
			return getRuleContext(Anonymous_interface_declarationContext.class,0);
		}
		public TerminalNode EXPORT() { return getToken(FluentLangParser.EXPORT, 0); }
		public Interface_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interface_declaration; }
	}

	public final Interface_declarationContext interface_declaration() throws RecognitionException {
		Interface_declarationContext _localctx = new Interface_declarationContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_interface_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(112);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==EXPORT) {
				{
				setState(111);
				match(EXPORT);
				}
			}

			setState(114);
			match(INTERFACE);
			setState(115);
			match(UPPERCASE_IDENTIFIER);
			setState(116);
			anonymous_interface_declaration();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Anonymous_interface_declarationContext extends ParserRuleContext {
		public List<Simple_anonymous_interface_declarationContext> simple_anonymous_interface_declaration() {
			return getRuleContexts(Simple_anonymous_interface_declarationContext.class);
		}
		public Simple_anonymous_interface_declarationContext simple_anonymous_interface_declaration(int i) {
			return getRuleContext(Simple_anonymous_interface_declarationContext.class,i);
		}
		public List<TerminalNode> PLUS() { return getTokens(FluentLangParser.PLUS); }
		public TerminalNode PLUS(int i) {
			return getToken(FluentLangParser.PLUS, i);
		}
		public Anonymous_interface_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_anonymous_interface_declaration; }
	}

	public final Anonymous_interface_declarationContext anonymous_interface_declaration() throws RecognitionException {
		Anonymous_interface_declarationContext _localctx = new Anonymous_interface_declarationContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_anonymous_interface_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(118);
			simple_anonymous_interface_declaration();
			setState(123);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==PLUS) {
				{
				{
				setState(119);
				match(PLUS);
				setState(120);
				simple_anonymous_interface_declaration();
				}
				}
				setState(125);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Simple_anonymous_interface_declarationContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(FluentLangParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(FluentLangParser.CLOSE_BRACE, 0); }
		public List<Interface_member_declarationContext> interface_member_declaration() {
			return getRuleContexts(Interface_member_declarationContext.class);
		}
		public Interface_member_declarationContext interface_member_declaration(int i) {
			return getRuleContext(Interface_member_declarationContext.class,i);
		}
		public Qualified_nameContext qualified_name() {
			return getRuleContext(Qualified_nameContext.class,0);
		}
		public Simple_anonymous_interface_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_simple_anonymous_interface_declaration; }
	}

	public final Simple_anonymous_interface_declarationContext simple_anonymous_interface_declaration() throws RecognitionException {
		Simple_anonymous_interface_declarationContext _localctx = new Simple_anonymous_interface_declarationContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_simple_anonymous_interface_declaration);
		int _la;
		try {
			setState(135);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OPEN_BRACE:
				enterOuterAlt(_localctx, 1);
				{
				setState(126);
				match(OPEN_BRACE);
				setState(130);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==UPPERCASE_IDENTIFIER) {
					{
					{
					setState(127);
					interface_member_declaration();
					}
					}
					setState(132);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(133);
				match(CLOSE_BRACE);
				}
				break;
			case UPPERCASE_IDENTIFIER:
				enterOuterAlt(_localctx, 2);
				{
				setState(134);
				qualified_name();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interface_member_declarationContext extends ParserRuleContext {
		public Method_signatureContext method_signature() {
			return getRuleContext(Method_signatureContext.class,0);
		}
		public TerminalNode SEMICOLON() { return getToken(FluentLangParser.SEMICOLON, 0); }
		public Interface_member_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interface_member_declaration; }
	}

	public final Interface_member_declarationContext interface_member_declaration() throws RecognitionException {
		Interface_member_declarationContext _localctx = new Interface_member_declarationContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_interface_member_declaration);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(137);
			method_signature();
			setState(138);
			match(SEMICOLON);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Method_signatureContext extends ParserRuleContext {
		public TerminalNode UPPERCASE_IDENTIFIER() { return getToken(FluentLangParser.UPPERCASE_IDENTIFIER, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(FluentLangParser.OPEN_PARENS, 0); }
		public ParametersContext parameters() {
			return getRuleContext(ParametersContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(FluentLangParser.CLOSE_PARENS, 0); }
		public Type_declarationContext type_declaration() {
			return getRuleContext(Type_declarationContext.class,0);
		}
		public Method_signatureContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_method_signature; }
	}

	public final Method_signatureContext method_signature() throws RecognitionException {
		Method_signatureContext _localctx = new Method_signatureContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_method_signature);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(140);
			match(UPPERCASE_IDENTIFIER);
			setState(141);
			match(OPEN_PARENS);
			setState(142);
			parameters();
			setState(143);
			match(CLOSE_PARENS);
			setState(144);
			type_declaration();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ParametersContext extends ParserRuleContext {
		public List<ParameterContext> parameter() {
			return getRuleContexts(ParameterContext.class);
		}
		public ParameterContext parameter(int i) {
			return getRuleContext(ParameterContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(FluentLangParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(FluentLangParser.COMMA, i);
		}
		public ParametersContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameters; }
	}

	public final ParametersContext parameters() throws RecognitionException {
		ParametersContext _localctx = new ParametersContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_parameters);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(154);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==LOWERCASE_IDENTIFIER) {
				{
				setState(146);
				parameter();
				setState(151);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==COMMA) {
					{
					{
					setState(147);
					match(COMMA);
					setState(148);
					parameter();
					}
					}
					setState(153);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ParameterContext extends ParserRuleContext {
		public TerminalNode LOWERCASE_IDENTIFIER() { return getToken(FluentLangParser.LOWERCASE_IDENTIFIER, 0); }
		public Type_declarationContext type_declaration() {
			return getRuleContext(Type_declarationContext.class,0);
		}
		public ParameterContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameter; }
	}

	public final ParameterContext parameter() throws RecognitionException {
		ParameterContext _localctx = new ParameterContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_parameter);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(156);
			match(LOWERCASE_IDENTIFIER);
			setState(157);
			type_declaration();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Type_declarationContext extends ParserRuleContext {
		public TerminalNode COLON() { return getToken(FluentLangParser.COLON, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public Type_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type_declaration; }
	}

	public final Type_declarationContext type_declaration() throws RecognitionException {
		Type_declarationContext _localctx = new Type_declarationContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_type_declaration);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(159);
			match(COLON);
			setState(160);
			type();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class TypeContext extends ParserRuleContext {
		public Qualified_nameContext qualified_name() {
			return getRuleContext(Qualified_nameContext.class,0);
		}
		public Primitive_typeContext primitive_type() {
			return getRuleContext(Primitive_typeContext.class,0);
		}
		public Anonymous_interface_declarationContext anonymous_interface_declaration() {
			return getRuleContext(Anonymous_interface_declarationContext.class,0);
		}
		public TypeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type; }
	}

	public final TypeContext type() throws RecognitionException {
		TypeContext _localctx = new TypeContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_type);
		try {
			setState(165);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,11,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(162);
				qualified_name();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(163);
				primitive_type();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(164);
				anonymous_interface_declaration();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Primitive_typeContext extends ParserRuleContext {
		public TerminalNode BOOL() { return getToken(FluentLangParser.BOOL, 0); }
		public TerminalNode INT() { return getToken(FluentLangParser.INT, 0); }
		public TerminalNode DOUBLE() { return getToken(FluentLangParser.DOUBLE, 0); }
		public TerminalNode CHAR() { return getToken(FluentLangParser.CHAR, 0); }
		public TerminalNode STRING() { return getToken(FluentLangParser.STRING, 0); }
		public Primitive_typeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_primitive_type; }
	}

	public final Primitive_typeContext primitive_type() throws RecognitionException {
		Primitive_typeContext _localctx = new Primitive_typeContext(_ctx, getState());
		enterRule(_localctx, 30, RULE_primitive_type);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(167);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << BOOL) | (1L << INT) | (1L << DOUBLE) | (1L << CHAR) | (1L << STRING))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Method_declarationContext extends ParserRuleContext {
		public Method_signatureContext method_signature() {
			return getRuleContext(Method_signatureContext.class,0);
		}
		public Method_bodyContext method_body() {
			return getRuleContext(Method_bodyContext.class,0);
		}
		public TerminalNode EXPORT() { return getToken(FluentLangParser.EXPORT, 0); }
		public Method_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_method_declaration; }
	}

	public final Method_declarationContext method_declaration() throws RecognitionException {
		Method_declarationContext _localctx = new Method_declarationContext(_ctx, getState());
		enterRule(_localctx, 32, RULE_method_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(170);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==EXPORT) {
				{
				setState(169);
				match(EXPORT);
				}
			}

			setState(172);
			method_signature();
			setState(173);
			method_body();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Method_bodyContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(FluentLangParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(FluentLangParser.CLOSE_BRACE, 0); }
		public List<Method_statementContext> method_statement() {
			return getRuleContexts(Method_statementContext.class);
		}
		public Method_statementContext method_statement(int i) {
			return getRuleContext(Method_statementContext.class,i);
		}
		public List<Method_declarationContext> method_declaration() {
			return getRuleContexts(Method_declarationContext.class);
		}
		public Method_declarationContext method_declaration(int i) {
			return getRuleContext(Method_declarationContext.class,i);
		}
		public List<Interface_declarationContext> interface_declaration() {
			return getRuleContexts(Interface_declarationContext.class);
		}
		public Interface_declarationContext interface_declaration(int i) {
			return getRuleContext(Interface_declarationContext.class,i);
		}
		public Method_bodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_method_body; }
	}

	public final Method_bodyContext method_body() throws RecognitionException {
		Method_bodyContext _localctx = new Method_bodyContext(_ctx, getState());
		enterRule(_localctx, 34, RULE_method_body);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(175);
			match(OPEN_BRACE);
			setState(181);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << DISCARD) | (1L << INTERFACE) | (1L << RETURN) | (1L << EXPORT) | (1L << LET) | (1L << UPPERCASE_IDENTIFIER))) != 0)) {
				{
				setState(179);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,13,_ctx) ) {
				case 1:
					{
					setState(176);
					method_statement();
					}
					break;
				case 2:
					{
					setState(177);
					method_declaration();
					}
					break;
				case 3:
					{
					setState(178);
					interface_declaration();
					}
					break;
				}
				}
				setState(183);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(184);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Method_statementContext extends ParserRuleContext {
		public Declaration_statementContext declaration_statement() {
			return getRuleContext(Declaration_statementContext.class,0);
		}
		public Return_statementContext return_statement() {
			return getRuleContext(Return_statementContext.class,0);
		}
		public Method_statementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_method_statement; }
	}

	public final Method_statementContext method_statement() throws RecognitionException {
		Method_statementContext _localctx = new Method_statementContext(_ctx, getState());
		enterRule(_localctx, 36, RULE_method_statement);
		try {
			setState(188);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case DISCARD:
			case LET:
				enterOuterAlt(_localctx, 1);
				{
				setState(186);
				declaration_statement();
				}
				break;
			case RETURN:
				enterOuterAlt(_localctx, 2);
				{
				setState(187);
				return_statement();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Declaration_statementContext extends ParserRuleContext {
		public TerminalNode LET() { return getToken(FluentLangParser.LET, 0); }
		public TerminalNode LOWERCASE_IDENTIFIER() { return getToken(FluentLangParser.LOWERCASE_IDENTIFIER, 0); }
		public TerminalNode ASSIGNMENT() { return getToken(FluentLangParser.ASSIGNMENT, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode SEMICOLON() { return getToken(FluentLangParser.SEMICOLON, 0); }
		public Type_declarationContext type_declaration() {
			return getRuleContext(Type_declarationContext.class,0);
		}
		public TerminalNode DISCARD() { return getToken(FluentLangParser.DISCARD, 0); }
		public Declaration_statementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_declaration_statement; }
	}

	public final Declaration_statementContext declaration_statement() throws RecognitionException {
		Declaration_statementContext _localctx = new Declaration_statementContext(_ctx, getState());
		enterRule(_localctx, 38, RULE_declaration_statement);
		int _la;
		try {
			setState(204);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case LET:
				enterOuterAlt(_localctx, 1);
				{
				setState(190);
				match(LET);
				setState(191);
				match(LOWERCASE_IDENTIFIER);
				setState(193);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==COLON) {
					{
					setState(192);
					type_declaration();
					}
				}

				setState(195);
				match(ASSIGNMENT);
				setState(196);
				expression(0);
				setState(197);
				match(SEMICOLON);
				}
				break;
			case DISCARD:
				enterOuterAlt(_localctx, 2);
				{
				setState(199);
				match(DISCARD);
				setState(200);
				match(ASSIGNMENT);
				setState(201);
				expression(0);
				setState(202);
				match(SEMICOLON);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Return_statementContext extends ParserRuleContext {
		public TerminalNode RETURN() { return getToken(FluentLangParser.RETURN, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode SEMICOLON() { return getToken(FluentLangParser.SEMICOLON, 0); }
		public Return_statementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_return_statement; }
	}

	public final Return_statementContext return_statement() throws RecognitionException {
		Return_statementContext _localctx = new Return_statementContext(_ctx, getState());
		enterRule(_localctx, 40, RULE_return_statement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(206);
			match(RETURN);
			setState(207);
			expression(0);
			setState(208);
			match(SEMICOLON);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ExpressionContext extends ParserRuleContext {
		public ExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expression; }
	 
		public ExpressionContext() { }
		public void copyFrom(ExpressionContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class Local_reference_expressionContext extends ExpressionContext {
		public TerminalNode LOWERCASE_IDENTIFIER() { return getToken(FluentLangParser.LOWERCASE_IDENTIFIER, 0); }
		public Local_reference_expressionContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	public static class Binary_operator_expressionContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public OperatorContext operator() {
			return getRuleContext(OperatorContext.class,0);
		}
		public Binary_operator_expressionContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	public static class Conditional_expressionContext extends ExpressionContext {
		public TerminalNode IF() { return getToken(FluentLangParser.IF, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(FluentLangParser.OPEN_PARENS, 0); }
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(FluentLangParser.CLOSE_PARENS, 0); }
		public TerminalNode ELSE() { return getToken(FluentLangParser.ELSE, 0); }
		public Conditional_expressionContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	public static class Parenthesized_expressionContext extends ExpressionContext {
		public TerminalNode OPEN_PARENS() { return getToken(FluentLangParser.OPEN_PARENS, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(FluentLangParser.CLOSE_PARENS, 0); }
		public Parenthesized_expressionContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	public static class Prefix_unary_operator_expressionContext extends ExpressionContext {
		public Prefix_unary_operatorContext prefix_unary_operator() {
			return getRuleContext(Prefix_unary_operatorContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Prefix_unary_operator_expressionContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	public static class Member_invocation_expressionContext extends ExpressionContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode DOT() { return getToken(FluentLangParser.DOT, 0); }
		public TerminalNode UPPERCASE_IDENTIFIER() { return getToken(FluentLangParser.UPPERCASE_IDENTIFIER, 0); }
		public InvocationContext invocation() {
			return getRuleContext(InvocationContext.class,0);
		}
		public Member_invocation_expressionContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	public static class Literal_expressionContext extends ExpressionContext {
		public LiteralContext literal() {
			return getRuleContext(LiteralContext.class,0);
		}
		public Literal_expressionContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	public static class Static_invocation_expressionContext extends ExpressionContext {
		public Qualified_nameContext qualified_name() {
			return getRuleContext(Qualified_nameContext.class,0);
		}
		public InvocationContext invocation() {
			return getRuleContext(InvocationContext.class,0);
		}
		public Static_invocation_expressionContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	public static class New_object_expressionContext extends ExpressionContext {
		public Empty_interfaceContext empty_interface() {
			return getRuleContext(Empty_interfaceContext.class,0);
		}
		public New_object_expressionContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	public static class Object_patching_expressionContext extends ExpressionContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode PLUS() { return getToken(FluentLangParser.PLUS, 0); }
		public List<Object_patchContext> object_patch() {
			return getRuleContexts(Object_patchContext.class);
		}
		public Object_patchContext object_patch(int i) {
			return getRuleContext(Object_patchContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(FluentLangParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(FluentLangParser.COMMA, i);
		}
		public Object_patching_expressionContext(ExpressionContext ctx) { copyFrom(ctx); }
	}

	public final ExpressionContext expression() throws RecognitionException {
		return expression(0);
	}

	private ExpressionContext expression(int _p) throws RecognitionException {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = getState();
		ExpressionContext _localctx = new ExpressionContext(_ctx, _parentState);
		ExpressionContext _prevctx = _localctx;
		int _startState = 42;
		enterRecursionRule(_localctx, 42, RULE_expression, _p);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(232);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OPEN_BRACE:
				{
				_localctx = new New_object_expressionContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;

				setState(211);
				empty_interface();
				}
				break;
			case MINUS:
				{
				_localctx = new Prefix_unary_operator_expressionContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(212);
				prefix_unary_operator();
				setState(213);
				expression(7);
				}
				break;
			case LITERAL_TRUE:
			case LITERAL_FALSE:
			case INTEGER_LITERAL:
			case REAL_LITERAL:
			case CHARACTER_LITERAL:
			case REGULAR_STRING:
				{
				_localctx = new Literal_expressionContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(215);
				literal();
				}
				break;
			case UPPERCASE_IDENTIFIER:
				{
				_localctx = new Static_invocation_expressionContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(216);
				qualified_name();
				setState(217);
				invocation();
				}
				break;
			case IF:
				{
				_localctx = new Conditional_expressionContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(219);
				match(IF);
				setState(220);
				match(OPEN_PARENS);
				setState(221);
				expression(0);
				setState(222);
				match(CLOSE_PARENS);
				setState(223);
				expression(0);
				setState(224);
				match(ELSE);
				setState(225);
				expression(3);
				}
				break;
			case OPEN_PARENS:
				{
				_localctx = new Parenthesized_expressionContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(227);
				match(OPEN_PARENS);
				setState(228);
				expression(0);
				setState(229);
				match(CLOSE_PARENS);
				}
				break;
			case LOWERCASE_IDENTIFIER:
				{
				_localctx = new Local_reference_expressionContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(231);
				match(LOWERCASE_IDENTIFIER);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			_ctx.stop = _input.LT(-1);
			setState(254);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,21,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) triggerExitRuleEvent();
					_prevctx = _localctx;
					{
					setState(252);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,20,_ctx) ) {
					case 1:
						{
						_localctx = new Binary_operator_expressionContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(234);
						if (!(precpred(_ctx, 8))) throw new FailedPredicateException(this, "precpred(_ctx, 8)");
						setState(235);
						operator();
						setState(236);
						expression(9);
						}
						break;
					case 2:
						{
						_localctx = new Object_patching_expressionContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(238);
						if (!(precpred(_ctx, 9))) throw new FailedPredicateException(this, "precpred(_ctx, 9)");
						setState(239);
						match(PLUS);
						setState(240);
						object_patch();
						setState(245);
						_errHandler.sync(this);
						_alt = getInterpreter().adaptivePredict(_input,19,_ctx);
						while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
							if ( _alt==1 ) {
								{
								{
								setState(241);
								match(COMMA);
								setState(242);
								object_patch();
								}
								} 
							}
							setState(247);
							_errHandler.sync(this);
							_alt = getInterpreter().adaptivePredict(_input,19,_ctx);
						}
						}
						break;
					case 3:
						{
						_localctx = new Member_invocation_expressionContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(248);
						if (!(precpred(_ctx, 4))) throw new FailedPredicateException(this, "precpred(_ctx, 4)");
						setState(249);
						match(DOT);
						setState(250);
						match(UPPERCASE_IDENTIFIER);
						setState(251);
						invocation();
						}
						break;
					}
					} 
				}
				setState(256);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,21,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			unrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public static class Empty_interfaceContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(FluentLangParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(FluentLangParser.CLOSE_BRACE, 0); }
		public Empty_interfaceContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_empty_interface; }
	}

	public final Empty_interfaceContext empty_interface() throws RecognitionException {
		Empty_interfaceContext _localctx = new Empty_interfaceContext(_ctx, getState());
		enterRule(_localctx, 44, RULE_empty_interface);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(257);
			match(OPEN_BRACE);
			setState(258);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Object_patchContext extends ParserRuleContext {
		public Fully_qualified_methodContext fully_qualified_method() {
			return getRuleContext(Fully_qualified_methodContext.class,0);
		}
		public TerminalNode MIXIN() { return getToken(FluentLangParser.MIXIN, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Object_patchContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_object_patch; }
	}

	public final Object_patchContext object_patch() throws RecognitionException {
		Object_patchContext _localctx = new Object_patchContext(_ctx, getState());
		enterRule(_localctx, 46, RULE_object_patch);
		try {
			setState(263);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case UPPERCASE_IDENTIFIER:
				enterOuterAlt(_localctx, 1);
				{
				setState(260);
				fully_qualified_method();
				}
				break;
			case MIXIN:
				enterOuterAlt(_localctx, 2);
				{
				setState(261);
				match(MIXIN);
				setState(262);
				expression(0);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Fully_qualified_methodContext extends ParserRuleContext {
		public Qualified_nameContext qualified_name() {
			return getRuleContext(Qualified_nameContext.class,0);
		}
		public Fully_qualified_methodContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fully_qualified_method; }
	}

	public final Fully_qualified_methodContext fully_qualified_method() throws RecognitionException {
		Fully_qualified_methodContext _localctx = new Fully_qualified_methodContext(_ctx, getState());
		enterRule(_localctx, 48, RULE_fully_qualified_method);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(265);
			qualified_name();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OperatorContext extends ParserRuleContext {
		public TerminalNode PLUS() { return getToken(FluentLangParser.PLUS, 0); }
		public TerminalNode MINUS() { return getToken(FluentLangParser.MINUS, 0); }
		public TerminalNode STAR() { return getToken(FluentLangParser.STAR, 0); }
		public TerminalNode DIV() { return getToken(FluentLangParser.DIV, 0); }
		public TerminalNode PERCENT() { return getToken(FluentLangParser.PERCENT, 0); }
		public TerminalNode LT() { return getToken(FluentLangParser.LT, 0); }
		public TerminalNode GT() { return getToken(FluentLangParser.GT, 0); }
		public TerminalNode OP_EQ() { return getToken(FluentLangParser.OP_EQ, 0); }
		public TerminalNode OP_NE() { return getToken(FluentLangParser.OP_NE, 0); }
		public TerminalNode OP_LE() { return getToken(FluentLangParser.OP_LE, 0); }
		public TerminalNode OP_GE() { return getToken(FluentLangParser.OP_GE, 0); }
		public OperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operator; }
	}

	public final OperatorContext operator() throws RecognitionException {
		OperatorContext _localctx = new OperatorContext(_ctx, getState());
		enterRule(_localctx, 50, RULE_operator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(267);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << PLUS) | (1L << MINUS) | (1L << STAR) | (1L << DIV) | (1L << PERCENT) | (1L << LT) | (1L << GT) | (1L << OP_EQ) | (1L << OP_NE) | (1L << OP_LE) | (1L << OP_GE))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Prefix_unary_operatorContext extends ParserRuleContext {
		public TerminalNode MINUS() { return getToken(FluentLangParser.MINUS, 0); }
		public Prefix_unary_operatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_prefix_unary_operator; }
	}

	public final Prefix_unary_operatorContext prefix_unary_operator() throws RecognitionException {
		Prefix_unary_operatorContext _localctx = new Prefix_unary_operatorContext(_ctx, getState());
		enterRule(_localctx, 52, RULE_prefix_unary_operator);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(269);
			match(MINUS);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class LiteralContext extends ParserRuleContext {
		public TerminalNode LITERAL_TRUE() { return getToken(FluentLangParser.LITERAL_TRUE, 0); }
		public TerminalNode LITERAL_FALSE() { return getToken(FluentLangParser.LITERAL_FALSE, 0); }
		public TerminalNode INTEGER_LITERAL() { return getToken(FluentLangParser.INTEGER_LITERAL, 0); }
		public TerminalNode REAL_LITERAL() { return getToken(FluentLangParser.REAL_LITERAL, 0); }
		public TerminalNode CHARACTER_LITERAL() { return getToken(FluentLangParser.CHARACTER_LITERAL, 0); }
		public TerminalNode REGULAR_STRING() { return getToken(FluentLangParser.REGULAR_STRING, 0); }
		public LiteralContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_literal; }
	}

	public final LiteralContext literal() throws RecognitionException {
		LiteralContext _localctx = new LiteralContext(_ctx, getState());
		enterRule(_localctx, 54, RULE_literal);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(271);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << LITERAL_TRUE) | (1L << LITERAL_FALSE) | (1L << INTEGER_LITERAL) | (1L << REAL_LITERAL) | (1L << CHARACTER_LITERAL) | (1L << REGULAR_STRING))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class InvocationContext extends ParserRuleContext {
		public TerminalNode OPEN_PARENS() { return getToken(FluentLangParser.OPEN_PARENS, 0); }
		public ArgumentsContext arguments() {
			return getRuleContext(ArgumentsContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(FluentLangParser.CLOSE_PARENS, 0); }
		public InvocationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_invocation; }
	}

	public final InvocationContext invocation() throws RecognitionException {
		InvocationContext _localctx = new InvocationContext(_ctx, getState());
		enterRule(_localctx, 56, RULE_invocation);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(273);
			match(OPEN_PARENS);
			setState(274);
			arguments();
			setState(275);
			match(CLOSE_PARENS);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ArgumentsContext extends ParserRuleContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(FluentLangParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(FluentLangParser.COMMA, i);
		}
		public ArgumentsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_arguments; }
	}

	public final ArgumentsContext arguments() throws RecognitionException {
		ArgumentsContext _localctx = new ArgumentsContext(_ctx, getState());
		enterRule(_localctx, 58, RULE_arguments);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(285);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << MINUS) | (1L << OPEN_BRACE) | (1L << OPEN_PARENS) | (1L << LITERAL_TRUE) | (1L << LITERAL_FALSE) | (1L << INTEGER_LITERAL) | (1L << REAL_LITERAL) | (1L << CHARACTER_LITERAL) | (1L << REGULAR_STRING) | (1L << IF) | (1L << UPPERCASE_IDENTIFIER) | (1L << LOWERCASE_IDENTIFIER))) != 0)) {
				{
				setState(277);
				expression(0);
				setState(282);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==COMMA) {
					{
					{
					setState(278);
					match(COMMA);
					setState(279);
					expression(0);
					}
					}
					setState(284);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Parameter_metadataContext extends ParserRuleContext {
		public ParameterContext parameter() {
			return getRuleContext(ParameterContext.class,0);
		}
		public TerminalNode EOF() { return getToken(FluentLangParser.EOF, 0); }
		public Parameter_metadataContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameter_metadata; }
	}

	public final Parameter_metadataContext parameter_metadata() throws RecognitionException {
		Parameter_metadataContext _localctx = new Parameter_metadataContext(_ctx, getState());
		enterRule(_localctx, 60, RULE_parameter_metadata);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(287);
			parameter();
			setState(288);
			match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Return_type_metadataContext extends ParserRuleContext {
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public TerminalNode EOF() { return getToken(FluentLangParser.EOF, 0); }
		public Return_type_metadataContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_return_type_metadata; }
	}

	public final Return_type_metadataContext return_type_metadata() throws RecognitionException {
		Return_type_metadataContext _localctx = new Return_type_metadataContext(_ctx, getState());
		enterRule(_localctx, 62, RULE_return_type_metadata);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(290);
			type();
			setState(291);
			match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interface_method_metadataContext extends ParserRuleContext {
		public Method_signatureContext method_signature() {
			return getRuleContext(Method_signatureContext.class,0);
		}
		public TerminalNode EOF() { return getToken(FluentLangParser.EOF, 0); }
		public Interface_method_metadataContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interface_method_metadata; }
	}

	public final Interface_method_metadataContext interface_method_metadata() throws RecognitionException {
		Interface_method_metadataContext _localctx = new Interface_method_metadataContext(_ctx, getState());
		enterRule(_localctx, 64, RULE_interface_method_metadata);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(293);
			method_signature();
			setState(294);
			match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Full_qualified_name_metadataContext extends ParserRuleContext {
		public Qualified_nameContext qualified_name() {
			return getRuleContext(Qualified_nameContext.class,0);
		}
		public Full_qualified_name_metadataContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_full_qualified_name_metadata; }
	}

	public final Full_qualified_name_metadataContext full_qualified_name_metadata() throws RecognitionException {
		Full_qualified_name_metadataContext _localctx = new Full_qualified_name_metadataContext(_ctx, getState());
		enterRule(_localctx, 66, RULE_full_qualified_name_metadata);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(296);
			qualified_name();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public boolean sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 21:
			return expression_sempred((ExpressionContext)_localctx, predIndex);
		}
		return true;
	}
	private boolean expression_sempred(ExpressionContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0:
			return precpred(_ctx, 8);
		case 1:
			return precpred(_ctx, 9);
		case 2:
			return precpred(_ctx, 4);
		}
		return true;
	}

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\3/\u012d\4\2\t\2\4"+
		"\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13\t"+
		"\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\4\27\t\27\4\30\t\30\4\31\t\31"+
		"\4\32\t\32\4\33\t\33\4\34\t\34\4\35\t\35\4\36\t\36\4\37\t\37\4 \t \4!"+
		"\t!\4\"\t\"\4#\t#\3\2\3\2\7\2I\n\2\f\2\16\2L\13\2\3\2\3\2\3\3\7\3Q\n\3"+
		"\f\3\16\3T\13\3\3\4\3\4\3\4\3\4\3\5\3\5\3\5\7\5]\n\5\f\5\16\5`\13\5\3"+
		"\6\3\6\3\6\5\6e\n\6\3\7\3\7\3\7\3\7\7\7k\n\7\f\7\16\7n\13\7\3\7\3\7\3"+
		"\b\5\bs\n\b\3\b\3\b\3\b\3\b\3\t\3\t\3\t\7\t|\n\t\f\t\16\t\177\13\t\3\n"+
		"\3\n\7\n\u0083\n\n\f\n\16\n\u0086\13\n\3\n\3\n\5\n\u008a\n\n\3\13\3\13"+
		"\3\13\3\f\3\f\3\f\3\f\3\f\3\f\3\r\3\r\3\r\7\r\u0098\n\r\f\r\16\r\u009b"+
		"\13\r\5\r\u009d\n\r\3\16\3\16\3\16\3\17\3\17\3\17\3\20\3\20\3\20\5\20"+
		"\u00a8\n\20\3\21\3\21\3\22\5\22\u00ad\n\22\3\22\3\22\3\22\3\23\3\23\3"+
		"\23\3\23\7\23\u00b6\n\23\f\23\16\23\u00b9\13\23\3\23\3\23\3\24\3\24\5"+
		"\24\u00bf\n\24\3\25\3\25\3\25\5\25\u00c4\n\25\3\25\3\25\3\25\3\25\3\25"+
		"\3\25\3\25\3\25\3\25\5\25\u00cf\n\25\3\26\3\26\3\26\3\26\3\27\3\27\3\27"+
		"\3\27\3\27\3\27\3\27\3\27\3\27\3\27\3\27\3\27\3\27\3\27\3\27\3\27\3\27"+
		"\3\27\3\27\3\27\3\27\3\27\5\27\u00eb\n\27\3\27\3\27\3\27\3\27\3\27\3\27"+
		"\3\27\3\27\3\27\7\27\u00f6\n\27\f\27\16\27\u00f9\13\27\3\27\3\27\3\27"+
		"\3\27\7\27\u00ff\n\27\f\27\16\27\u0102\13\27\3\30\3\30\3\30\3\31\3\31"+
		"\3\31\5\31\u010a\n\31\3\32\3\32\3\33\3\33\3\34\3\34\3\35\3\35\3\36\3\36"+
		"\3\36\3\36\3\37\3\37\3\37\7\37\u011b\n\37\f\37\16\37\u011e\13\37\5\37"+
		"\u0120\n\37\3 \3 \3 \3!\3!\3!\3\"\3\"\3\"\3#\3#\3#\2\3,$\2\4\6\b\n\f\16"+
		"\20\22\24\26\30\32\34\36 \"$&(*,.\60\62\64\668:<>@BD\2\5\3\2\32\36\3\2"+
		"\5\17\3\2\37$\2\u012c\2F\3\2\2\2\4R\3\2\2\2\6U\3\2\2\2\bY\3\2\2\2\nd\3"+
		"\2\2\2\ff\3\2\2\2\16r\3\2\2\2\20x\3\2\2\2\22\u0089\3\2\2\2\24\u008b\3"+
		"\2\2\2\26\u008e\3\2\2\2\30\u009c\3\2\2\2\32\u009e\3\2\2\2\34\u00a1\3\2"+
		"\2\2\36\u00a7\3\2\2\2 \u00a9\3\2\2\2\"\u00ac\3\2\2\2$\u00b1\3\2\2\2&\u00be"+
		"\3\2\2\2(\u00ce\3\2\2\2*\u00d0\3\2\2\2,\u00ea\3\2\2\2.\u0103\3\2\2\2\60"+
		"\u0109\3\2\2\2\62\u010b\3\2\2\2\64\u010d\3\2\2\2\66\u010f\3\2\2\28\u0111"+
		"\3\2\2\2:\u0113\3\2\2\2<\u011f\3\2\2\2>\u0121\3\2\2\2@\u0124\3\2\2\2B"+
		"\u0127\3\2\2\2D\u012a\3\2\2\2FJ\5\4\3\2GI\5\n\6\2HG\3\2\2\2IL\3\2\2\2"+
		"JH\3\2\2\2JK\3\2\2\2KM\3\2\2\2LJ\3\2\2\2MN\7\2\2\3N\3\3\2\2\2OQ\5\6\4"+
		"\2PO\3\2\2\2QT\3\2\2\2RP\3\2\2\2RS\3\2\2\2S\5\3\2\2\2TR\3\2\2\2UV\7,\2"+
		"\2VW\5\b\5\2WX\7\30\2\2X\7\3\2\2\2Y^\7.\2\2Z[\7\25\2\2[]\7.\2\2\\Z\3\2"+
		"\2\2]`\3\2\2\2^\\\3\2\2\2^_\3\2\2\2_\t\3\2\2\2`^\3\2\2\2ae\5\f\7\2be\5"+
		"\16\b\2ce\5\"\22\2da\3\2\2\2db\3\2\2\2dc\3\2\2\2e\13\3\2\2\2fg\7%\2\2"+
		"gh\5\b\5\2hl\7\21\2\2ik\5\n\6\2ji\3\2\2\2kn\3\2\2\2lj\3\2\2\2lm\3\2\2"+
		"\2mo\3\2\2\2nl\3\2\2\2op\7\22\2\2p\r\3\2\2\2qs\7+\2\2rq\3\2\2\2rs\3\2"+
		"\2\2st\3\2\2\2tu\7&\2\2uv\7.\2\2vw\5\20\t\2w\17\3\2\2\2x}\5\22\n\2yz\7"+
		"\5\2\2z|\5\22\n\2{y\3\2\2\2|\177\3\2\2\2}{\3\2\2\2}~\3\2\2\2~\21\3\2\2"+
		"\2\177}\3\2\2\2\u0080\u0084\7\21\2\2\u0081\u0083\5\24\13\2\u0082\u0081"+
		"\3\2\2\2\u0083\u0086\3\2\2\2\u0084\u0082\3\2\2\2\u0084\u0085\3\2\2\2\u0085"+
		"\u0087\3\2\2\2\u0086\u0084\3\2\2\2\u0087\u008a\7\22\2\2\u0088\u008a\5"+
		"\b\5\2\u0089\u0080\3\2\2\2\u0089\u0088\3\2\2\2\u008a\23\3\2\2\2\u008b"+
		"\u008c\5\26\f\2\u008c\u008d\7\30\2\2\u008d\25\3\2\2\2\u008e\u008f\7.\2"+
		"\2\u008f\u0090\7\23\2\2\u0090\u0091\5\30\r\2\u0091\u0092\7\24\2\2\u0092"+
		"\u0093\5\34\17\2\u0093\27\3\2\2\2\u0094\u0099\5\32\16\2\u0095\u0096\7"+
		"\26\2\2\u0096\u0098\5\32\16\2\u0097\u0095\3\2\2\2\u0098\u009b\3\2\2\2"+
		"\u0099\u0097\3\2\2\2\u0099\u009a\3\2\2\2\u009a\u009d\3\2\2\2\u009b\u0099"+
		"\3\2\2\2\u009c\u0094\3\2\2\2\u009c\u009d\3\2\2\2\u009d\31\3\2\2\2\u009e"+
		"\u009f\7/\2\2\u009f\u00a0\5\34\17\2\u00a0\33\3\2\2\2\u00a1\u00a2\7\27"+
		"\2\2\u00a2\u00a3\5\36\20\2\u00a3\35\3\2\2\2\u00a4\u00a8\5\b\5\2\u00a5"+
		"\u00a8\5 \21\2\u00a6\u00a8\5\20\t\2\u00a7\u00a4\3\2\2\2\u00a7\u00a5\3"+
		"\2\2\2\u00a7\u00a6\3\2\2\2\u00a8\37\3\2\2\2\u00a9\u00aa\t\2\2\2\u00aa"+
		"!\3\2\2\2\u00ab\u00ad\7+\2\2\u00ac\u00ab\3\2\2\2\u00ac\u00ad\3\2\2\2\u00ad"+
		"\u00ae\3\2\2\2\u00ae\u00af\5\26\f\2\u00af\u00b0\5$\23\2\u00b0#\3\2\2\2"+
		"\u00b1\u00b7\7\21\2\2\u00b2\u00b6\5&\24\2\u00b3\u00b6\5\"\22\2\u00b4\u00b6"+
		"\5\16\b\2\u00b5\u00b2\3\2\2\2\u00b5\u00b3\3\2\2\2\u00b5\u00b4\3\2\2\2"+
		"\u00b6\u00b9\3\2\2\2\u00b7\u00b5\3\2\2\2\u00b7\u00b8\3\2\2\2\u00b8\u00ba"+
		"\3\2\2\2\u00b9\u00b7\3\2\2\2\u00ba\u00bb\7\22\2\2\u00bb%\3\2\2\2\u00bc"+
		"\u00bf\5(\25\2\u00bd\u00bf\5*\26\2\u00be\u00bc\3\2\2\2\u00be\u00bd\3\2"+
		"\2\2\u00bf\'\3\2\2\2\u00c0\u00c1\7-\2\2\u00c1\u00c3\7/\2\2\u00c2\u00c4"+
		"\5\34\17\2\u00c3\u00c2\3\2\2\2\u00c3\u00c4\3\2\2\2\u00c4\u00c5\3\2\2\2"+
		"\u00c5\u00c6\7\20\2\2\u00c6\u00c7\5,\27\2\u00c7\u00c8\7\30\2\2\u00c8\u00cf"+
		"\3\2\2\2\u00c9\u00ca\7\31\2\2\u00ca\u00cb\7\20\2\2\u00cb\u00cc\5,\27\2"+
		"\u00cc\u00cd\7\30\2\2\u00cd\u00cf\3\2\2\2\u00ce\u00c0\3\2\2\2\u00ce\u00c9"+
		"\3\2\2\2\u00cf)\3\2\2\2\u00d0\u00d1\7\'\2\2\u00d1\u00d2\5,\27\2\u00d2"+
		"\u00d3\7\30\2\2\u00d3+\3\2\2\2\u00d4\u00d5\b\27\1\2\u00d5\u00eb\5.\30"+
		"\2\u00d6\u00d7\5\66\34\2\u00d7\u00d8\5,\27\t\u00d8\u00eb\3\2\2\2\u00d9"+
		"\u00eb\58\35\2\u00da\u00db\5\b\5\2\u00db\u00dc\5:\36\2\u00dc\u00eb\3\2"+
		"\2\2\u00dd\u00de\7(\2\2\u00de\u00df\7\23\2\2\u00df\u00e0\5,\27\2\u00e0"+
		"\u00e1\7\24\2\2\u00e1\u00e2\5,\27\2\u00e2\u00e3\7)\2\2\u00e3\u00e4\5,"+
		"\27\5\u00e4\u00eb\3\2\2\2\u00e5\u00e6\7\23\2\2\u00e6\u00e7\5,\27\2\u00e7"+
		"\u00e8\7\24\2\2\u00e8\u00eb\3\2\2\2\u00e9\u00eb\7/\2\2\u00ea\u00d4\3\2"+
		"\2\2\u00ea\u00d6\3\2\2\2\u00ea\u00d9\3\2\2\2\u00ea\u00da\3\2\2\2\u00ea"+
		"\u00dd\3\2\2\2\u00ea\u00e5\3\2\2\2\u00ea\u00e9\3\2\2\2\u00eb\u0100\3\2"+
		"\2\2\u00ec\u00ed\f\n\2\2\u00ed\u00ee\5\64\33\2\u00ee\u00ef\5,\27\13\u00ef"+
		"\u00ff\3\2\2\2\u00f0\u00f1\f\13\2\2\u00f1\u00f2\7\5\2\2\u00f2\u00f7\5"+
		"\60\31\2\u00f3\u00f4\7\26\2\2\u00f4\u00f6\5\60\31\2\u00f5\u00f3\3\2\2"+
		"\2\u00f6\u00f9\3\2\2\2\u00f7\u00f5\3\2\2\2\u00f7\u00f8\3\2\2\2\u00f8\u00ff"+
		"\3\2\2\2\u00f9\u00f7\3\2\2\2\u00fa\u00fb\f\6\2\2\u00fb\u00fc\7\25\2\2"+
		"\u00fc\u00fd\7.\2\2\u00fd\u00ff\5:\36\2\u00fe\u00ec\3\2\2\2\u00fe\u00f0"+
		"\3\2\2\2\u00fe\u00fa\3\2\2\2\u00ff\u0102\3\2\2\2\u0100\u00fe\3\2\2\2\u0100"+
		"\u0101\3\2\2\2\u0101-\3\2\2\2\u0102\u0100\3\2\2\2\u0103\u0104\7\21\2\2"+
		"\u0104\u0105\7\22\2\2\u0105/\3\2\2\2\u0106\u010a\5\62\32\2\u0107\u0108"+
		"\7*\2\2\u0108\u010a\5,\27\2\u0109\u0106\3\2\2\2\u0109\u0107\3\2\2\2\u010a"+
		"\61\3\2\2\2\u010b\u010c\5\b\5\2\u010c\63\3\2\2\2\u010d\u010e\t\3\2\2\u010e"+
		"\65\3\2\2\2\u010f\u0110\7\6\2\2\u0110\67\3\2\2\2\u0111\u0112\t\4\2\2\u0112"+
		"9\3\2\2\2\u0113\u0114\7\23\2\2\u0114\u0115\5<\37\2\u0115\u0116\7\24\2"+
		"\2\u0116;\3\2\2\2\u0117\u011c\5,\27\2\u0118\u0119\7\26\2\2\u0119\u011b"+
		"\5,\27\2\u011a\u0118\3\2\2\2\u011b\u011e\3\2\2\2\u011c\u011a\3\2\2\2\u011c"+
		"\u011d\3\2\2\2\u011d\u0120\3\2\2\2\u011e\u011c\3\2\2\2\u011f\u0117\3\2"+
		"\2\2\u011f\u0120\3\2\2\2\u0120=\3\2\2\2\u0121\u0122\5\32\16\2\u0122\u0123"+
		"\7\2\2\3\u0123?\3\2\2\2\u0124\u0125\5\36\20\2\u0125\u0126\7\2\2\3\u0126"+
		"A\3\2\2\2\u0127\u0128\5\26\f\2\u0128\u0129\7\2\2\3\u0129C\3\2\2\2\u012a"+
		"\u012b\5\b\5\2\u012bE\3\2\2\2\33JR^dlr}\u0084\u0089\u0099\u009c\u00a7"+
		"\u00ac\u00b5\u00b7\u00be\u00c3\u00ce\u00ea\u00f7\u00fe\u0100\u0109\u011c"+
		"\u011f";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}