// Generated from c:\git\fluentlang\grammar/FluentLangLexer.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.Token;
import org.antlr.v4.runtime.TokenStream;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.misc.*;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class FluentLangLexer extends Lexer {
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
		COMMENTS_CHANNEL=2;
	public static String[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN", "COMMENTS_CHANNEL"
	};

	public static String[] modeNames = {
		"DEFAULT_MODE"
	};

	public static final String[] ruleNames = {
		"SINGLE_LINE_COMMENT", "WHITESPACES", "PLUS", "MINUS", "STAR", "DIV", 
		"PERCENT", "LT", "GT", "OP_EQ", "OP_NE", "OP_LE", "OP_GE", "ASSIGNMENT", 
		"OPEN_BRACE", "CLOSE_BRACE", "OPEN_PARENS", "CLOSE_PARENS", "DOT", "COMMA", 
		"COLON", "SEMICOLON", "DISCARD", "BOOL", "INT", "DOUBLE", "CHAR", "STRING", 
		"LITERAL_TRUE", "LITERAL_FALSE", "INTEGER_LITERAL", "REAL_LITERAL", "CHARACTER_LITERAL", 
		"REGULAR_STRING", "NAMESPACE", "INTERFACE", "RETURN", "IF", "ELSE", "MIXIN", 
		"EXPORT", "OPEN", "LET", "UPPERCASE_IDENTIFIER", "LOWERCASE_IDENTIFIER", 
		"IdentifierTail", "InputCharacter", "CommonCharacter", "SimpleEscapeSequence", 
		"HexEscapeSequence", "NewLine", "Whitespace", "UnicodeClassZS", "UnicodeEscapeSequence", 
		"HexDigit"
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


	public FluentLangLexer(CharStream input) {
		super(input);
		_interp = new LexerATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@Override
	public String getGrammarFileName() { return "FluentLangLexer.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public String[] getChannelNames() { return channelNames; }

	@Override
	public String[] getModeNames() { return modeNames; }

	@Override
	public ATN getATN() { return _ATN; }

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\2/\u018f\b\1\4\2\t"+
		"\2\4\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13"+
		"\t\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\4\27\t\27\4\30\t\30\4\31\t\31"+
		"\4\32\t\32\4\33\t\33\4\34\t\34\4\35\t\35\4\36\t\36\4\37\t\37\4 \t \4!"+
		"\t!\4\"\t\"\4#\t#\4$\t$\4%\t%\4&\t&\4\'\t\'\4(\t(\4)\t)\4*\t*\4+\t+\4"+
		",\t,\4-\t-\4.\t.\4/\t/\4\60\t\60\4\61\t\61\4\62\t\62\4\63\t\63\4\64\t"+
		"\64\4\65\t\65\4\66\t\66\4\67\t\67\48\t8\3\2\3\2\3\2\3\2\7\2v\n\2\f\2\16"+
		"\2y\13\2\3\2\3\2\3\3\3\3\6\3\177\n\3\r\3\16\3\u0080\3\3\3\3\3\4\3\4\3"+
		"\5\3\5\3\6\3\6\3\7\3\7\3\b\3\b\3\t\3\t\3\n\3\n\3\13\3\13\3\13\3\f\3\f"+
		"\3\f\3\r\3\r\3\r\3\16\3\16\3\16\3\17\3\17\3\20\3\20\3\21\3\21\3\22\3\22"+
		"\3\23\3\23\3\24\3\24\3\25\3\25\3\26\3\26\3\27\3\27\3\30\3\30\3\31\3\31"+
		"\3\31\3\31\3\31\3\32\3\32\3\32\3\32\3\33\3\33\3\33\3\33\3\33\3\33\3\33"+
		"\3\34\3\34\3\34\3\34\3\34\3\35\3\35\3\35\3\35\3\35\3\35\3\35\3\36\3\36"+
		"\3\36\3\36\3\36\3\37\3\37\3\37\3\37\3\37\3\37\3 \6 \u00db\n \r \16 \u00dc"+
		"\3!\7!\u00e0\n!\f!\16!\u00e3\13!\3!\3!\6!\u00e7\n!\r!\16!\u00e8\3\"\3"+
		"\"\3\"\5\"\u00ee\n\"\3\"\3\"\3#\3#\3#\7#\u00f5\n#\f#\16#\u00f8\13#\3#"+
		"\3#\3$\3$\3$\3$\3$\3$\3$\3$\3$\3$\3%\3%\3%\3%\3%\3%\3%\3%\3%\3%\3&\3&"+
		"\3&\3&\3&\3&\3&\3\'\3\'\3\'\3(\3(\3(\3(\3(\3)\3)\3)\3)\3)\3)\3*\3*\3*"+
		"\3*\3*\3*\3*\3+\3+\3+\3+\3+\3,\3,\3,\3,\3-\3-\3-\3.\3.\3.\3/\7/\u013c"+
		"\n/\f/\16/\u013f\13/\3\60\3\60\3\61\3\61\5\61\u0145\n\61\3\62\3\62\3\62"+
		"\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62"+
		"\3\62\3\62\3\62\3\62\3\62\5\62\u015d\n\62\3\63\3\63\3\63\3\63\3\63\3\63"+
		"\3\63\3\63\3\63\3\63\3\63\3\63\3\63\3\63\3\63\3\63\3\63\3\63\3\63\3\63"+
		"\3\63\3\63\3\63\3\63\3\63\5\63\u0178\n\63\3\64\3\64\3\64\5\64\u017d\n"+
		"\64\3\65\3\65\5\65\u0181\n\65\3\66\3\66\3\67\3\67\3\67\3\67\3\67\3\67"+
		"\3\67\3\67\38\58\u018e\n8\2\29\3\3\5\4\7\5\t\6\13\7\r\b\17\t\21\n\23\13"+
		"\25\f\27\r\31\16\33\17\35\20\37\21!\22#\23%\24\'\25)\26+\27-\30/\31\61"+
		"\32\63\33\65\34\67\359\36;\37= ?!A\"C#E$G%I&K\'M(O)Q*S+U,W-Y.[/]\2_\2"+
		"a\2c\2e\2g\2i\2k\2m\2o\2\3\2\f\3\2\62;\b\2\f\f\17\17))^^\u0087\u0087\u202a"+
		"\u202b\b\2\f\f\17\17$$^^\u0087\u0087\u202a\u202b\3\2C\\\3\2c|\6\2\62;"+
		"C\\aac|\6\2\f\f\17\17\u0087\u0087\u202a\u202b\4\2\13\13\r\16\13\2\"\""+
		"\u00a2\u00a2\u1682\u1682\u1810\u1810\u2002\u2008\u200a\u200c\u2031\u2031"+
		"\u2061\u2061\u3002\u3002\5\2\62;CHch\2\u019e\2\3\3\2\2\2\2\5\3\2\2\2\2"+
		"\7\3\2\2\2\2\t\3\2\2\2\2\13\3\2\2\2\2\r\3\2\2\2\2\17\3\2\2\2\2\21\3\2"+
		"\2\2\2\23\3\2\2\2\2\25\3\2\2\2\2\27\3\2\2\2\2\31\3\2\2\2\2\33\3\2\2\2"+
		"\2\35\3\2\2\2\2\37\3\2\2\2\2!\3\2\2\2\2#\3\2\2\2\2%\3\2\2\2\2\'\3\2\2"+
		"\2\2)\3\2\2\2\2+\3\2\2\2\2-\3\2\2\2\2/\3\2\2\2\2\61\3\2\2\2\2\63\3\2\2"+
		"\2\2\65\3\2\2\2\2\67\3\2\2\2\29\3\2\2\2\2;\3\2\2\2\2=\3\2\2\2\2?\3\2\2"+
		"\2\2A\3\2\2\2\2C\3\2\2\2\2E\3\2\2\2\2G\3\2\2\2\2I\3\2\2\2\2K\3\2\2\2\2"+
		"M\3\2\2\2\2O\3\2\2\2\2Q\3\2\2\2\2S\3\2\2\2\2U\3\2\2\2\2W\3\2\2\2\2Y\3"+
		"\2\2\2\2[\3\2\2\2\3q\3\2\2\2\5~\3\2\2\2\7\u0084\3\2\2\2\t\u0086\3\2\2"+
		"\2\13\u0088\3\2\2\2\r\u008a\3\2\2\2\17\u008c\3\2\2\2\21\u008e\3\2\2\2"+
		"\23\u0090\3\2\2\2\25\u0092\3\2\2\2\27\u0095\3\2\2\2\31\u0098\3\2\2\2\33"+
		"\u009b\3\2\2\2\35\u009e\3\2\2\2\37\u00a0\3\2\2\2!\u00a2\3\2\2\2#\u00a4"+
		"\3\2\2\2%\u00a6\3\2\2\2\'\u00a8\3\2\2\2)\u00aa\3\2\2\2+\u00ac\3\2\2\2"+
		"-\u00ae\3\2\2\2/\u00b0\3\2\2\2\61\u00b2\3\2\2\2\63\u00b7\3\2\2\2\65\u00bb"+
		"\3\2\2\2\67\u00c2\3\2\2\29\u00c7\3\2\2\2;\u00ce\3\2\2\2=\u00d3\3\2\2\2"+
		"?\u00da\3\2\2\2A\u00e1\3\2\2\2C\u00ea\3\2\2\2E\u00f1\3\2\2\2G\u00fb\3"+
		"\2\2\2I\u0105\3\2\2\2K\u010f\3\2\2\2M\u0116\3\2\2\2O\u0119\3\2\2\2Q\u011e"+
		"\3\2\2\2S\u0124\3\2\2\2U\u012b\3\2\2\2W\u0130\3\2\2\2Y\u0134\3\2\2\2["+
		"\u0137\3\2\2\2]\u013d\3\2\2\2_\u0140\3\2\2\2a\u0144\3\2\2\2c\u015c\3\2"+
		"\2\2e\u0177\3\2\2\2g\u017c\3\2\2\2i\u0180\3\2\2\2k\u0182\3\2\2\2m\u0184"+
		"\3\2\2\2o\u018d\3\2\2\2qr\7\61\2\2rs\7\61\2\2sw\3\2\2\2tv\5_\60\2ut\3"+
		"\2\2\2vy\3\2\2\2wu\3\2\2\2wx\3\2\2\2xz\3\2\2\2yw\3\2\2\2z{\b\2\2\2{\4"+
		"\3\2\2\2|\177\5i\65\2}\177\5g\64\2~|\3\2\2\2~}\3\2\2\2\177\u0080\3\2\2"+
		"\2\u0080~\3\2\2\2\u0080\u0081\3\2\2\2\u0081\u0082\3\2\2\2\u0082\u0083"+
		"\b\3\3\2\u0083\6\3\2\2\2\u0084\u0085\7-\2\2\u0085\b\3\2\2\2\u0086\u0087"+
		"\7/\2\2\u0087\n\3\2\2\2\u0088\u0089\7,\2\2\u0089\f\3\2\2\2\u008a\u008b"+
		"\7\61\2\2\u008b\16\3\2\2\2\u008c\u008d\7\'\2\2\u008d\20\3\2\2\2\u008e"+
		"\u008f\7>\2\2\u008f\22\3\2\2\2\u0090\u0091\7@\2\2\u0091\24\3\2\2\2\u0092"+
		"\u0093\7?\2\2\u0093\u0094\7?\2\2\u0094\26\3\2\2\2\u0095\u0096\7#\2\2\u0096"+
		"\u0097\7?\2\2\u0097\30\3\2\2\2\u0098\u0099\7>\2\2\u0099\u009a\7?\2\2\u009a"+
		"\32\3\2\2\2\u009b\u009c\7@\2\2\u009c\u009d\7?\2\2\u009d\34\3\2\2\2\u009e"+
		"\u009f\7?\2\2\u009f\36\3\2\2\2\u00a0\u00a1\7}\2\2\u00a1 \3\2\2\2\u00a2"+
		"\u00a3\7\177\2\2\u00a3\"\3\2\2\2\u00a4\u00a5\7*\2\2\u00a5$\3\2\2\2\u00a6"+
		"\u00a7\7+\2\2\u00a7&\3\2\2\2\u00a8\u00a9\7\60\2\2\u00a9(\3\2\2\2\u00aa"+
		"\u00ab\7.\2\2\u00ab*\3\2\2\2\u00ac\u00ad\7<\2\2\u00ad,\3\2\2\2\u00ae\u00af"+
		"\7=\2\2\u00af.\3\2\2\2\u00b0\u00b1\7a\2\2\u00b1\60\3\2\2\2\u00b2\u00b3"+
		"\7d\2\2\u00b3\u00b4\7q\2\2\u00b4\u00b5\7q\2\2\u00b5\u00b6\7n\2\2\u00b6"+
		"\62\3\2\2\2\u00b7\u00b8\7k\2\2\u00b8\u00b9\7p\2\2\u00b9\u00ba\7v\2\2\u00ba"+
		"\64\3\2\2\2\u00bb\u00bc\7f\2\2\u00bc\u00bd\7q\2\2\u00bd\u00be\7w\2\2\u00be"+
		"\u00bf\7d\2\2\u00bf\u00c0\7n\2\2\u00c0\u00c1\7g\2\2\u00c1\66\3\2\2\2\u00c2"+
		"\u00c3\7e\2\2\u00c3\u00c4\7j\2\2\u00c4\u00c5\7c\2\2\u00c5\u00c6\7t\2\2"+
		"\u00c68\3\2\2\2\u00c7\u00c8\7u\2\2\u00c8\u00c9\7v\2\2\u00c9\u00ca\7t\2"+
		"\2\u00ca\u00cb\7k\2\2\u00cb\u00cc\7p\2\2\u00cc\u00cd\7i\2\2\u00cd:\3\2"+
		"\2\2\u00ce\u00cf\7v\2\2\u00cf\u00d0\7t\2\2\u00d0\u00d1\7w\2\2\u00d1\u00d2"+
		"\7g\2\2\u00d2<\3\2\2\2\u00d3\u00d4\7h\2\2\u00d4\u00d5\7c\2\2\u00d5\u00d6"+
		"\7n\2\2\u00d6\u00d7\7u\2\2\u00d7\u00d8\7g\2\2\u00d8>\3\2\2\2\u00d9\u00db"+
		"\t\2\2\2\u00da\u00d9\3\2\2\2\u00db\u00dc\3\2\2\2\u00dc\u00da\3\2\2\2\u00dc"+
		"\u00dd\3\2\2\2\u00dd@\3\2\2\2\u00de\u00e0\t\2\2\2\u00df\u00de\3\2\2\2"+
		"\u00e0\u00e3\3\2\2\2\u00e1\u00df\3\2\2\2\u00e1\u00e2\3\2\2\2\u00e2\u00e4"+
		"\3\2\2\2\u00e3\u00e1\3\2\2\2\u00e4\u00e6\7\60\2\2\u00e5\u00e7\t\2\2\2"+
		"\u00e6\u00e5\3\2\2\2\u00e7\u00e8\3\2\2\2\u00e8\u00e6\3\2\2\2\u00e8\u00e9"+
		"\3\2\2\2\u00e9B\3\2\2\2\u00ea\u00ed\7)\2\2\u00eb\u00ee\n\3\2\2\u00ec\u00ee"+
		"\5a\61\2\u00ed\u00eb\3\2\2\2\u00ed\u00ec\3\2\2\2\u00ee\u00ef\3\2\2\2\u00ef"+
		"\u00f0\7)\2\2\u00f0D\3\2\2\2\u00f1\u00f6\7$\2\2\u00f2\u00f5\n\4\2\2\u00f3"+
		"\u00f5\5a\61\2\u00f4\u00f2\3\2\2\2\u00f4\u00f3\3\2\2\2\u00f5\u00f8\3\2"+
		"\2\2\u00f6\u00f4\3\2\2\2\u00f6\u00f7\3\2\2\2\u00f7\u00f9\3\2\2\2\u00f8"+
		"\u00f6\3\2\2\2\u00f9\u00fa\7$\2\2\u00faF\3\2\2\2\u00fb\u00fc\7p\2\2\u00fc"+
		"\u00fd\7c\2\2\u00fd\u00fe\7o\2\2\u00fe\u00ff\7g\2\2\u00ff\u0100\7u\2\2"+
		"\u0100\u0101\7r\2\2\u0101\u0102\7c\2\2\u0102\u0103\7e\2\2\u0103\u0104"+
		"\7g\2\2\u0104H\3\2\2\2\u0105\u0106\7k\2\2\u0106\u0107\7p\2\2\u0107\u0108"+
		"\7v\2\2\u0108\u0109\7g\2\2\u0109\u010a\7t\2\2\u010a\u010b\7h\2\2\u010b"+
		"\u010c\7c\2\2\u010c\u010d\7e\2\2\u010d\u010e\7g\2\2\u010eJ\3\2\2\2\u010f"+
		"\u0110\7t\2\2\u0110\u0111\7g\2\2\u0111\u0112\7v\2\2\u0112\u0113\7w\2\2"+
		"\u0113\u0114\7t\2\2\u0114\u0115\7p\2\2\u0115L\3\2\2\2\u0116\u0117\7k\2"+
		"\2\u0117\u0118\7h\2\2\u0118N\3\2\2\2\u0119\u011a\7g\2\2\u011a\u011b\7"+
		"n\2\2\u011b\u011c\7u\2\2\u011c\u011d\7g\2\2\u011dP\3\2\2\2\u011e\u011f"+
		"\7o\2\2\u011f\u0120\7k\2\2\u0120\u0121\7z\2\2\u0121\u0122\7k\2\2\u0122"+
		"\u0123\7p\2\2\u0123R\3\2\2\2\u0124\u0125\7g\2\2\u0125\u0126\7z\2\2\u0126"+
		"\u0127\7r\2\2\u0127\u0128\7q\2\2\u0128\u0129\7t\2\2\u0129\u012a\7v\2\2"+
		"\u012aT\3\2\2\2\u012b\u012c\7q\2\2\u012c\u012d\7r\2\2\u012d\u012e\7g\2"+
		"\2\u012e\u012f\7p\2\2\u012fV\3\2\2\2\u0130\u0131\7n\2\2\u0131\u0132\7"+
		"g\2\2\u0132\u0133\7v\2\2\u0133X\3\2\2\2\u0134\u0135\t\5\2\2\u0135\u0136"+
		"\5]/\2\u0136Z\3\2\2\2\u0137\u0138\t\6\2\2\u0138\u0139\5]/\2\u0139\\\3"+
		"\2\2\2\u013a\u013c\t\7\2\2\u013b\u013a\3\2\2\2\u013c\u013f\3\2\2\2\u013d"+
		"\u013b\3\2\2\2\u013d\u013e\3\2\2\2\u013e^\3\2\2\2\u013f\u013d\3\2\2\2"+
		"\u0140\u0141\n\b\2\2\u0141`\3\2\2\2\u0142\u0145\5c\62\2\u0143\u0145\5"+
		"m\67\2\u0144\u0142\3\2\2\2\u0144\u0143\3\2\2\2\u0145b\3\2\2\2\u0146\u0147"+
		"\7^\2\2\u0147\u015d\7)\2\2\u0148\u0149\7^\2\2\u0149\u015d\7$\2\2\u014a"+
		"\u014b\7^\2\2\u014b\u015d\7^\2\2\u014c\u014d\7^\2\2\u014d\u015d\7\62\2"+
		"\2\u014e\u014f\7^\2\2\u014f\u015d\7c\2\2\u0150\u0151\7^\2\2\u0151\u015d"+
		"\7d\2\2\u0152\u0153\7^\2\2\u0153\u015d\7h\2\2\u0154\u0155\7^\2\2\u0155"+
		"\u015d\7p\2\2\u0156\u0157\7^\2\2\u0157\u015d\7t\2\2\u0158\u0159\7^\2\2"+
		"\u0159\u015d\7v\2\2\u015a\u015b\7^\2\2\u015b\u015d\7x\2\2\u015c\u0146"+
		"\3\2\2\2\u015c\u0148\3\2\2\2\u015c\u014a\3\2\2\2\u015c\u014c\3\2\2\2\u015c"+
		"\u014e\3\2\2\2\u015c\u0150\3\2\2\2\u015c\u0152\3\2\2\2\u015c\u0154\3\2"+
		"\2\2\u015c\u0156\3\2\2\2\u015c\u0158\3\2\2\2\u015c\u015a\3\2\2\2\u015d"+
		"d\3\2\2\2\u015e\u015f\7^\2\2\u015f\u0160\7z\2\2\u0160\u0161\3\2\2\2\u0161"+
		"\u0178\5o8\2\u0162\u0163\7^\2\2\u0163\u0164\7z\2\2\u0164\u0165\3\2\2\2"+
		"\u0165\u0166\5o8\2\u0166\u0167\5o8\2\u0167\u0178\3\2\2\2\u0168\u0169\7"+
		"^\2\2\u0169\u016a\7z\2\2\u016a\u016b\3\2\2\2\u016b\u016c\5o8\2\u016c\u016d"+
		"\5o8\2\u016d\u016e\5o8\2\u016e\u0178\3\2\2\2\u016f\u0170\7^\2\2\u0170"+
		"\u0171\7z\2\2\u0171\u0172\3\2\2\2\u0172\u0173\5o8\2\u0173\u0174\5o8\2"+
		"\u0174\u0175\5o8\2\u0175\u0176\5o8\2\u0176\u0178\3\2\2\2\u0177\u015e\3"+
		"\2\2\2\u0177\u0162\3\2\2\2\u0177\u0168\3\2\2\2\u0177\u016f\3\2\2\2\u0178"+
		"f\3\2\2\2\u0179\u017a\7\17\2\2\u017a\u017d\7\f\2\2\u017b\u017d\t\b\2\2"+
		"\u017c\u0179\3\2\2\2\u017c\u017b\3\2\2\2\u017dh\3\2\2\2\u017e\u0181\5"+
		"k\66\2\u017f\u0181\t\t\2\2\u0180\u017e\3\2\2\2\u0180\u017f\3\2\2\2\u0181"+
		"j\3\2\2\2\u0182\u0183\t\n\2\2\u0183l\3\2\2\2\u0184\u0185\7^\2\2\u0185"+
		"\u0186\7w\2\2\u0186\u0187\3\2\2\2\u0187\u0188\5o8\2\u0188\u0189\5o8\2"+
		"\u0189\u018a\5o8\2\u018a\u018b\5o8\2\u018bn\3\2\2\2\u018c\u018e\t\13\2"+
		"\2\u018d\u018c\3\2\2\2\u018ep\3\2\2\2\24\2w~\u0080\u00dc\u00e1\u00e8\u00ed"+
		"\u00f4\u00f6\u013b\u013d\u0144\u015c\u0177\u017c\u0180\u018d\4\2\4\2\2"+
		"\3\2";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}