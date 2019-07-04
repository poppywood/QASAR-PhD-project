// $ANTLR 3.1 EsperEPL2Grammar.g 2008-08-20 22:38:01
// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162
namespace 
	com.espertech.esper.epl.generated

{

using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;

using IDictionary	= System.Collections.IDictionary;
using Hashtable 	= System.Collections.Hashtable;

public partial class EsperEPL2GrammarLexer : Lexer {
    public const int FLOAT_SUFFIX = 237;
    public const int STAR = 188;
    public const int NUMERIC_PARAM_LIST = 77;
    public const int ISTREAM = 58;
    public const int MOD = 205;
    public const int OUTERJOIN_EXPR = 108;
    public const int BSR = 222;
    public const int LIB_FUNCTION = 129;
    public const int EOF = -1;
    public const int FULL_OUTERJOIN_EXPR = 111;
    public const int RPAREN = 190;
    public const int LNOT = 211;
    public const int INC = 215;
    public const int CREATE = 4;
    public const int STRING_LITERAL = 182;
    public const int STREAM_EXPR = 107;
    public const int BSR_ASSIGN = 223;
    public const int CAST_EXPR = 157;
    public const int T__247 = 247;
    public const int NOT_EQUAL = 198;
    public const int METADATASQL = 63;
    public const int T__246 = 246;
    public const int T__249 = 249;
    public const int T__248 = 248;
    public const int REGEXP = 9;
    public const int T__250 = 250;
    public const int FOLLOWED_BY_EXPR = 79;
    public const int FOLLOWED_BY = 208;
    public const int HOUR_PART = 134;
    public const int RBRACK = 192;
    public const int GE = 202;
    public const int MIN_LIMIT_EXPR = 124;
    public const int METHOD_JOIN_EXPR = 166;
    public const int ASC = 55;
    public const int IN_SET = 6;
    public const int EVENT_FILTER_EXPR = 81;
    public const int MINUS_ASSIGN = 216;
    public const int ELSE = 29;
    public const int EVENT_FILTER_NOT_IN = 87;
    public const int INSERTINTO_STREAM_NAME = 146;
    public const int NUM_DOUBLE = 174;
    public const int UNARY_MINUS = 130;
    public const int LCURLY = 206;
    public const int DBWHERE_CLAUSE = 144;
    public const int MEDIAN = 22;
    public const int EVENTS = 47;
    public const int AND_EXPR = 12;
    public const int EVENT_FILTER_NOT_RANGE = 85;
    public const int GROUP = 42;
    public const int WS = 231;
    public const int ESCAPECHAR = 209;
    public const int SL_COMMENT = 232;
    public const int NULL_TYPE = 173;
    public const int GT = 200;
    public const int BNOT = 212;
    public const int WHERE_EXPR = 96;
    public const int END = 32;
    public const int LAND = 229;
    public const int NOT_REGEXP = 141;
    public const int EVENT_PROP_EXPR = 115;
    public const int LBRACK = 191;
    public const int VIEW_EXPR = 93;
    public const int LONG_TYPE = 168;
    public const int ON_SELECT_EXPR = 162;
    public const int MINUTE_PART = 135;
    public const int SUM = 17;
    public const int SQL_NE = 197;
    public const int LPAREN = 189;
    public const int IN_SUBSELECT_EXPR = 151;
    public const int AS = 16;
    public const int OR_EXPR = 11;
    public const int THEN = 31;
    public const int NOT_IN_RANGE = 148;
    public const int AVG = 18;
    public const int LEFT = 36;
    public const int SECOND_PART = 136;
    public const int PREVIOUS = 64;
    public const int DATABASE_JOIN_EXPR = 95;
    public const int IDENT = 184;
    public const int CASE2 = 28;
    public const int PLUS = 181;
    public const int BXOR = 196;
    public const int EVENT_PROP_INDEXED = 118;
    public const int EXISTS = 66;
    public const int EVAL_NOTEQUALS_EXPR = 102;
    public const int CREATE_VARIABLE_EXPR = 165;
    public const int LIKE = 8;
    public const int OUTER = 34;
    public const int BY = 41;
    public const int T__239 = 239;
    public const int ARRAY_PARAM_LIST = 80;
    public const int RIGHT_OUTERJOIN_EXPR = 110;
    public const int T__238 = 238;
    public const int LAST_OPERATOR = 154;
    public const int EVAL_AND_EXPR = 99;
    public const int LEFT_OUTERJOIN_EXPR = 109;
    public const int HEX_DIGIT = 235;
    public const int EPL_EXPR = 175;
    public const int GROUP_BY_EXPR = 112;
    public const int SET = 74;
    public const int RIGHT = 37;
    public const int HAVING = 43;
    public const int INSTANCEOF = 69;
    public const int MIN = 20;
    public const int EVENT_PROP_SIMPLE = 116;
    public const int MINUS = 180;
    public const int T__245 = 245;
    public const int T__244 = 244;
    public const int SEMI = 230;
    public const int T__243 = 243;
    public const int STAR_ASSIGN = 218;
    public const int T__242 = 242;
    public const int T__241 = 241;
    public const int T__240 = 240;
    public const int COLON = 193;
    public const int MINUTES = 49;
    public const int BAND_ASSIGN = 228;
    public const int NOT_IN_SET = 138;
    public const int EVENT_PROP_DYNAMIC_SIMPLE = 119;
    public const int SL = 224;
    public const int WHEN = 30;
    public const int NOT_IN_SUBSELECT_EXPR = 152;
    public const int GUARD_EXPR = 91;
    public const int SR = 220;
    public const int RCURLY = 207;
    public const int PLUS_ASSIGN = 214;
    public const int DAY_PART = 133;
    public const int EXISTS_SUBSELECT_EXPR = 150;
    public const int EVENT_FILTER_IN = 86;
    public const int DIV = 204;
    public const int BETWEEN = 7;
    public const int MILLISECOND_PART = 137;
    public const int PRIOR = 65;
    public const int FIRST = 50;
    public const int SELECTION_EXPR = 104;
    public const int LOR = 203;
    public const int CAST = 70;
    public const int LW = 68;
    public const int WILDCARD_SELECT = 145;
    public const int EXPONENT = 236;
    public const int LT = 199;
    public const int PATTERN_INCL_EXPR = 94;
    public const int ORDER_BY_EXPR = 113;
    public const int BOOL_TYPE = 172;
    public const int MOD_ASSIGN = 219;
    public const int CASE = 27;
    public const int IN_SUBSELECT_QUERY_EXPR = 153;
    public const int EQUALS = 186;
    public const int COUNT = 25;
    public const int DIV_ASSIGN = 213;
    public const int SL_ASSIGN = 225;
    public const int PATTERN = 61;
    public const int SQL = 62;
    public const int WEEKDAY = 67;
    public const int FULL = 38;
    public const int INSERT = 52;
    public const int ESCAPE = 10;
    public const int ARRAY_EXPR = 132;
    public const int LAST = 51;
    public const int SELECT = 26;
    public const int INTO = 53;
    public const int COALESCE = 21;
    public const int EVENT_FILTER_BETWEEN = 88;
    public const int FLOAT_TYPE = 169;
    public const int SUBSELECT_EXPR = 149;
    public const int NUMERIC_PARAM_RANGE = 76;
    public const int CONCAT = 128;
    public const int CLASS_IDENT = 90;
    public const int ON_EXPR = 160;
    public const int CREATE_WINDOW_EXPR = 158;
    public const int ON_DELETE_EXPR = 161;
    public const int NUM_LONG = 177;
    public const int ON = 39;
    public const int TIME_PERIOD = 131;
    public const int DOUBLE_TYPE = 170;
    public const int DELETE = 72;
    public const int INT_TYPE = 167;
    public const int EVERY_EXPR = 14;
    public const int EVAL_BITWISE_EXPR = 98;
    public const int ORDER_ELEMENT_EXPR = 114;
    public const int VARIABLE = 75;
    public const int SUBSTITUTION = 156;
    public const int STRING_TYPE = 171;
    public const int NUM_INT = 176;
    public const int ON_SET_EXPR = 164;
    public const int STDDEV = 23;
    public const int ON_EXPR_FROM = 163;
    public const int NUM_FLOAT = 178;
    public const int FROM = 33;
    public const int DISTINCT = 44;
    public const int OUTPUT = 46;
    public const int WEEKDAY_OPERATOR = 155;
    public const int WHERE = 15;
    public const int DEC = 217;
    public const int SEC_LIMIT_EXPR = 123;
    public const int NUMERIC_PARAM_FREQUENCY = 78;
    public const int BXOR_ASSIGN = 226;
    public const int ORDER = 54;
    public const int SNAPSHOT = 73;
    public const int ESC = 234;
    public const int EVENT_PROP_DYNAMIC_MAPPED = 121;
    public const int EVENT_FILTER_PARAM = 83;
    public const int IRSTREAM = 59;
    public const int MAX = 19;
    public const int SECONDS = 48;
    public const int EVENT_FILTER_RANGE = 84;
    public const int ML_COMMENT = 233;
    public const int EVENT_PROP_DYNAMIC_INDEXED = 120;
    public const int BOR_ASSIGN = 227;
    public const int COMMA = 185;
    public const int IS = 40;
    public const int TIMEPERIOD_LIMIT_EXPR = 125;
    public const int ALL = 45;
    public const int BOR = 195;
    public const int EQUAL = 210;
    public const int EVENT_FILTER_NOT_BETWEEN = 89;
    public const int IN_RANGE = 147;
    public const int DOT = 187;
    public const int CURRENT_TIMESTAMP = 71;
    public const int INSERTINTO_EXPR = 126;
    public const int HAVING_EXPR = 97;
    public const int UNIDIRECTIONAL = 60;
    public const int EVAL_EQUALS_EXPR = 101;
    public const int RSTREAM = 57;
    public const int NOT_LIKE = 140;
    public const int EVENT_LIMIT_EXPR = 122;
    public const int NOT_BETWEEN = 139;
    public const int EVAL_OR_EXPR = 100;
    public const int BAND = 194;
    public const int QUOTED_STRING_LITERAL = 183;
    public const int JOIN = 35;
    public const int NOT_EXPR = 13;
    public const int QUESTION = 179;
    public const int OBSERVER_EXPR = 92;
    public const int EVENT_FILTER_IDENT = 82;
    public const int EVENT_PROP_MAPPED = 117;
    public const int AVEDEV = 24;
    public const int DBSELECT_EXPR = 142;
    public const int SELECTION_ELEMENT_EXPR = 105;
    public const int CREATE_WINDOW_SELECT_EXPR = 159;
    public const int INSERTINTO_EXPRCOL = 127;
    public const int WINDOW = 5;
    public const int DESC = 56;
    public const int SELECTION_STREAM = 106;
    public const int SR_ASSIGN = 221;
    public const int DBFROM_CLAUSE = 143;
    public const int LE = 201;
    public const int EVAL_IDENT = 103;

      protected virtual void mismatch(IIntStream input, int ttype, BitSet follow) {
        throw new MismatchedTokenException(ttype, input);  
      }

      public virtual void recoverFromMismatchedToken(IIntStream intStream, RecognitionException recognitionException, int i, BitSet bitSet) {
        throw recognitionException;
      }

      public virtual void recoverFromMismatchedSet(IIntStream intStream, RecognitionException recognitionException, BitSet bitSet) {
        throw recognitionException;
      }

      protected virtual bool recoverFromMismatchedElement(IIntStream intStream, RecognitionException recognitionException, BitSet bitSet) {
        throw new ApplicationException("Error recovering from mismatched element", recognitionException);
      }


    // delegates
    // delegators

    public EsperEPL2GrammarLexer() 
    {
		InitializeCyclicDFAs();
    }
    public EsperEPL2GrammarLexer(ICharStream input)
		: this(input, null) {
    }
    public EsperEPL2GrammarLexer(ICharStream input, RecognizerSharedState state)
		: base(input, state) {
		InitializeCyclicDFAs(); 

    }
    
    override public string GrammarFileName
    {
    	get { return "EsperEPL2Grammar.g";} 
    }

    // $ANTLR start "CREATE"
    public void mCREATE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = CREATE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:28:8: ( 'create' )
            // EsperEPL2Grammar.g:28:10: 'create'
            {
            	Match("create"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "CREATE"

    // $ANTLR start "WINDOW"
    public void mWINDOW() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WINDOW;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:29:8: ( 'window' )
            // EsperEPL2Grammar.g:29:10: 'window'
            {
            	Match("window"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WINDOW"

    // $ANTLR start "IN_SET"
    public void mIN_SET() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = IN_SET;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:30:8: ( 'in' )
            // EsperEPL2Grammar.g:30:10: 'in'
            {
            	Match("in"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "IN_SET"

    // $ANTLR start "BETWEEN"
    public void mBETWEEN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BETWEEN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:31:9: ( 'between' )
            // EsperEPL2Grammar.g:31:11: 'between'
            {
            	Match("between"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BETWEEN"

    // $ANTLR start "LIKE"
    public void mLIKE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LIKE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:32:6: ( 'like' )
            // EsperEPL2Grammar.g:32:8: 'like'
            {
            	Match("like"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LIKE"

    // $ANTLR start "REGEXP"
    public void mREGEXP() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = REGEXP;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:33:8: ( 'regexp' )
            // EsperEPL2Grammar.g:33:10: 'regexp'
            {
            	Match("regexp"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "REGEXP"

    // $ANTLR start "ESCAPE"
    public void mESCAPE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ESCAPE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:34:8: ( 'escape' )
            // EsperEPL2Grammar.g:34:10: 'escape'
            {
            	Match("escape"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ESCAPE"

    // $ANTLR start "OR_EXPR"
    public void mOR_EXPR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = OR_EXPR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:35:9: ( 'or' )
            // EsperEPL2Grammar.g:35:11: 'or'
            {
            	Match("or"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "OR_EXPR"

    // $ANTLR start "AND_EXPR"
    public void mAND_EXPR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = AND_EXPR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:36:10: ( 'and' )
            // EsperEPL2Grammar.g:36:12: 'and'
            {
            	Match("and"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "AND_EXPR"

    // $ANTLR start "NOT_EXPR"
    public void mNOT_EXPR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NOT_EXPR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:37:10: ( 'not' )
            // EsperEPL2Grammar.g:37:12: 'not'
            {
            	Match("not"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NOT_EXPR"

    // $ANTLR start "EVERY_EXPR"
    public void mEVERY_EXPR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = EVERY_EXPR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:38:12: ( 'every' )
            // EsperEPL2Grammar.g:38:14: 'every'
            {
            	Match("every"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "EVERY_EXPR"

    // $ANTLR start "WHERE"
    public void mWHERE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WHERE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:39:7: ( 'where' )
            // EsperEPL2Grammar.g:39:9: 'where'
            {
            	Match("where"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WHERE"

    // $ANTLR start "AS"
    public void mAS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = AS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:40:4: ( 'as' )
            // EsperEPL2Grammar.g:40:6: 'as'
            {
            	Match("as"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "AS"

    // $ANTLR start "SUM"
    public void mSUM() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SUM;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:41:5: ( 'sum' )
            // EsperEPL2Grammar.g:41:7: 'sum'
            {
            	Match("sum"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SUM"

    // $ANTLR start "AVG"
    public void mAVG() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = AVG;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:42:5: ( 'avg' )
            // EsperEPL2Grammar.g:42:7: 'avg'
            {
            	Match("avg"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "AVG"

    // $ANTLR start "MAX"
    public void mMAX() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MAX;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:43:5: ( 'max' )
            // EsperEPL2Grammar.g:43:7: 'max'
            {
            	Match("max"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MAX"

    // $ANTLR start "MIN"
    public void mMIN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MIN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:44:5: ( 'min' )
            // EsperEPL2Grammar.g:44:7: 'min'
            {
            	Match("min"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MIN"

    // $ANTLR start "COALESCE"
    public void mCOALESCE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = COALESCE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:45:10: ( 'coalesce' )
            // EsperEPL2Grammar.g:45:12: 'coalesce'
            {
            	Match("coalesce"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "COALESCE"

    // $ANTLR start "MEDIAN"
    public void mMEDIAN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MEDIAN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:46:8: ( 'median' )
            // EsperEPL2Grammar.g:46:10: 'median'
            {
            	Match("median"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MEDIAN"

    // $ANTLR start "STDDEV"
    public void mSTDDEV() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = STDDEV;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:47:8: ( 'stddev' )
            // EsperEPL2Grammar.g:47:10: 'stddev'
            {
            	Match("stddev"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "STDDEV"

    // $ANTLR start "AVEDEV"
    public void mAVEDEV() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = AVEDEV;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:48:8: ( 'avedev' )
            // EsperEPL2Grammar.g:48:10: 'avedev'
            {
            	Match("avedev"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "AVEDEV"

    // $ANTLR start "COUNT"
    public void mCOUNT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = COUNT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:49:7: ( 'count' )
            // EsperEPL2Grammar.g:49:9: 'count'
            {
            	Match("count"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "COUNT"

    // $ANTLR start "SELECT"
    public void mSELECT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SELECT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:50:8: ( 'select' )
            // EsperEPL2Grammar.g:50:10: 'select'
            {
            	Match("select"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SELECT"

    // $ANTLR start "CASE"
    public void mCASE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = CASE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:51:6: ( 'case' )
            // EsperEPL2Grammar.g:51:8: 'case'
            {
            	Match("case"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "CASE"

    // $ANTLR start "ELSE"
    public void mELSE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ELSE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:52:6: ( 'else' )
            // EsperEPL2Grammar.g:52:8: 'else'
            {
            	Match("else"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ELSE"

    // $ANTLR start "WHEN"
    public void mWHEN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WHEN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:53:6: ( 'when' )
            // EsperEPL2Grammar.g:53:8: 'when'
            {
            	Match("when"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WHEN"

    // $ANTLR start "THEN"
    public void mTHEN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = THEN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:54:6: ( 'then' )
            // EsperEPL2Grammar.g:54:8: 'then'
            {
            	Match("then"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "THEN"

    // $ANTLR start "END"
    public void mEND() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = END;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:55:5: ( 'end' )
            // EsperEPL2Grammar.g:55:7: 'end'
            {
            	Match("end"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "END"

    // $ANTLR start "FROM"
    public void mFROM() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = FROM;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:56:6: ( 'from' )
            // EsperEPL2Grammar.g:56:8: 'from'
            {
            	Match("from"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "FROM"

    // $ANTLR start "OUTER"
    public void mOUTER() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = OUTER;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:57:7: ( 'outer' )
            // EsperEPL2Grammar.g:57:9: 'outer'
            {
            	Match("outer"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "OUTER"

    // $ANTLR start "JOIN"
    public void mJOIN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = JOIN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:58:6: ( 'join' )
            // EsperEPL2Grammar.g:58:8: 'join'
            {
            	Match("join"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "JOIN"

    // $ANTLR start "LEFT"
    public void mLEFT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LEFT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:59:6: ( 'left' )
            // EsperEPL2Grammar.g:59:8: 'left'
            {
            	Match("left"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LEFT"

    // $ANTLR start "RIGHT"
    public void mRIGHT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = RIGHT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:60:7: ( 'right' )
            // EsperEPL2Grammar.g:60:9: 'right'
            {
            	Match("right"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "RIGHT"

    // $ANTLR start "FULL"
    public void mFULL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = FULL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:61:6: ( 'full' )
            // EsperEPL2Grammar.g:61:8: 'full'
            {
            	Match("full"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "FULL"

    // $ANTLR start "ON"
    public void mON() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ON;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:62:4: ( 'on' )
            // EsperEPL2Grammar.g:62:6: 'on'
            {
            	Match("on"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ON"

    // $ANTLR start "IS"
    public void mIS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = IS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:63:4: ( 'is' )
            // EsperEPL2Grammar.g:63:6: 'is'
            {
            	Match("is"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "IS"

    // $ANTLR start "BY"
    public void mBY() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BY;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:64:4: ( 'by' )
            // EsperEPL2Grammar.g:64:6: 'by'
            {
            	Match("by"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BY"

    // $ANTLR start "GROUP"
    public void mGROUP() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = GROUP;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:65:7: ( 'group' )
            // EsperEPL2Grammar.g:65:9: 'group'
            {
            	Match("group"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "GROUP"

    // $ANTLR start "HAVING"
    public void mHAVING() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = HAVING;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:66:8: ( 'having' )
            // EsperEPL2Grammar.g:66:10: 'having'
            {
            	Match("having"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "HAVING"

    // $ANTLR start "DISTINCT"
    public void mDISTINCT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = DISTINCT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:67:10: ( 'distinct' )
            // EsperEPL2Grammar.g:67:12: 'distinct'
            {
            	Match("distinct"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "DISTINCT"

    // $ANTLR start "ALL"
    public void mALL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ALL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:68:5: ( 'all' )
            // EsperEPL2Grammar.g:68:7: 'all'
            {
            	Match("all"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ALL"

    // $ANTLR start "OUTPUT"
    public void mOUTPUT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = OUTPUT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:69:8: ( 'output' )
            // EsperEPL2Grammar.g:69:10: 'output'
            {
            	Match("output"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "OUTPUT"

    // $ANTLR start "EVENTS"
    public void mEVENTS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = EVENTS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:70:8: ( 'events' )
            // EsperEPL2Grammar.g:70:10: 'events'
            {
            	Match("events"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "EVENTS"

    // $ANTLR start "SECONDS"
    public void mSECONDS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SECONDS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:71:9: ( 'seconds' )
            // EsperEPL2Grammar.g:71:11: 'seconds'
            {
            	Match("seconds"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SECONDS"

    // $ANTLR start "MINUTES"
    public void mMINUTES() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MINUTES;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:72:9: ( 'minutes' )
            // EsperEPL2Grammar.g:72:11: 'minutes'
            {
            	Match("minutes"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MINUTES"

    // $ANTLR start "FIRST"
    public void mFIRST() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = FIRST;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:73:7: ( 'first' )
            // EsperEPL2Grammar.g:73:9: 'first'
            {
            	Match("first"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "FIRST"

    // $ANTLR start "LAST"
    public void mLAST() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LAST;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:74:6: ( 'last' )
            // EsperEPL2Grammar.g:74:8: 'last'
            {
            	Match("last"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LAST"

    // $ANTLR start "INSERT"
    public void mINSERT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = INSERT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:75:8: ( 'insert' )
            // EsperEPL2Grammar.g:75:10: 'insert'
            {
            	Match("insert"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "INSERT"

    // $ANTLR start "INTO"
    public void mINTO() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = INTO;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:76:6: ( 'into' )
            // EsperEPL2Grammar.g:76:8: 'into'
            {
            	Match("into"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "INTO"

    // $ANTLR start "ORDER"
    public void mORDER() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ORDER;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:77:7: ( 'order' )
            // EsperEPL2Grammar.g:77:9: 'order'
            {
            	Match("order"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ORDER"

    // $ANTLR start "ASC"
    public void mASC() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ASC;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:78:5: ( 'asc' )
            // EsperEPL2Grammar.g:78:7: 'asc'
            {
            	Match("asc"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ASC"

    // $ANTLR start "DESC"
    public void mDESC() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = DESC;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:79:6: ( 'desc' )
            // EsperEPL2Grammar.g:79:8: 'desc'
            {
            	Match("desc"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "DESC"

    // $ANTLR start "RSTREAM"
    public void mRSTREAM() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = RSTREAM;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:80:9: ( 'rstream' )
            // EsperEPL2Grammar.g:80:11: 'rstream'
            {
            	Match("rstream"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "RSTREAM"

    // $ANTLR start "ISTREAM"
    public void mISTREAM() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ISTREAM;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:81:9: ( 'istream' )
            // EsperEPL2Grammar.g:81:11: 'istream'
            {
            	Match("istream"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ISTREAM"

    // $ANTLR start "IRSTREAM"
    public void mIRSTREAM() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = IRSTREAM;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:82:10: ( 'irstream' )
            // EsperEPL2Grammar.g:82:12: 'irstream'
            {
            	Match("irstream"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "IRSTREAM"

    // $ANTLR start "UNIDIRECTIONAL"
    public void mUNIDIRECTIONAL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = UNIDIRECTIONAL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:83:16: ( 'unidirectional' )
            // EsperEPL2Grammar.g:83:18: 'unidirectional'
            {
            	Match("unidirectional"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "UNIDIRECTIONAL"

    // $ANTLR start "PATTERN"
    public void mPATTERN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = PATTERN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:84:9: ( 'pattern' )
            // EsperEPL2Grammar.g:84:11: 'pattern'
            {
            	Match("pattern"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "PATTERN"

    // $ANTLR start "SQL"
    public void mSQL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SQL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:85:5: ( 'sql' )
            // EsperEPL2Grammar.g:85:7: 'sql'
            {
            	Match("sql"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SQL"

    // $ANTLR start "METADATASQL"
    public void mMETADATASQL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = METADATASQL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:86:13: ( 'metadatasql' )
            // EsperEPL2Grammar.g:86:15: 'metadatasql'
            {
            	Match("metadatasql"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "METADATASQL"

    // $ANTLR start "PREVIOUS"
    public void mPREVIOUS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = PREVIOUS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:87:10: ( 'prev' )
            // EsperEPL2Grammar.g:87:12: 'prev'
            {
            	Match("prev"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "PREVIOUS"

    // $ANTLR start "PRIOR"
    public void mPRIOR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = PRIOR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:88:7: ( 'prior' )
            // EsperEPL2Grammar.g:88:9: 'prior'
            {
            	Match("prior"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "PRIOR"

    // $ANTLR start "EXISTS"
    public void mEXISTS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = EXISTS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:89:8: ( 'exists' )
            // EsperEPL2Grammar.g:89:10: 'exists'
            {
            	Match("exists"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "EXISTS"

    // $ANTLR start "WEEKDAY"
    public void mWEEKDAY() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WEEKDAY;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:90:9: ( 'weekday' )
            // EsperEPL2Grammar.g:90:11: 'weekday'
            {
            	Match("weekday"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WEEKDAY"

    // $ANTLR start "LW"
    public void mLW() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LW;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:91:4: ( 'lastweekday' )
            // EsperEPL2Grammar.g:91:6: 'lastweekday'
            {
            	Match("lastweekday"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LW"

    // $ANTLR start "INSTANCEOF"
    public void mINSTANCEOF() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = INSTANCEOF;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:92:12: ( 'instanceof' )
            // EsperEPL2Grammar.g:92:14: 'instanceof'
            {
            	Match("instanceof"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "INSTANCEOF"

    // $ANTLR start "CAST"
    public void mCAST() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = CAST;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:93:6: ( 'cast' )
            // EsperEPL2Grammar.g:93:8: 'cast'
            {
            	Match("cast"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "CAST"

    // $ANTLR start "CURRENT_TIMESTAMP"
    public void mCURRENT_TIMESTAMP() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = CURRENT_TIMESTAMP;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:94:19: ( 'current_timestamp' )
            // EsperEPL2Grammar.g:94:21: 'current_timestamp'
            {
            	Match("current_timestamp"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "CURRENT_TIMESTAMP"

    // $ANTLR start "DELETE"
    public void mDELETE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = DELETE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:95:8: ( 'delete' )
            // EsperEPL2Grammar.g:95:10: 'delete'
            {
            	Match("delete"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "DELETE"

    // $ANTLR start "SNAPSHOT"
    public void mSNAPSHOT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SNAPSHOT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:96:10: ( 'snapshot' )
            // EsperEPL2Grammar.g:96:12: 'snapshot'
            {
            	Match("snapshot"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SNAPSHOT"

    // $ANTLR start "SET"
    public void mSET() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SET;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:97:5: ( 'set' )
            // EsperEPL2Grammar.g:97:7: 'set'
            {
            	Match("set"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SET"

    // $ANTLR start "VARIABLE"
    public void mVARIABLE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = VARIABLE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:98:10: ( 'variable' )
            // EsperEPL2Grammar.g:98:12: 'variable'
            {
            	Match("variable"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "VARIABLE"

    // $ANTLR start "T__238"
    public void mT__238() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__238;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:99:8: ( 'true' )
            // EsperEPL2Grammar.g:99:10: 'true'
            {
            	Match("true"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__238"

    // $ANTLR start "T__239"
    public void mT__239() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__239;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:100:8: ( 'false' )
            // EsperEPL2Grammar.g:100:10: 'false'
            {
            	Match("false"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__239"

    // $ANTLR start "T__240"
    public void mT__240() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__240;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:101:8: ( 'null' )
            // EsperEPL2Grammar.g:101:10: 'null'
            {
            	Match("null"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__240"

    // $ANTLR start "T__241"
    public void mT__241() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__241;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:102:8: ( 'days' )
            // EsperEPL2Grammar.g:102:10: 'days'
            {
            	Match("days"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__241"

    // $ANTLR start "T__242"
    public void mT__242() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__242;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:103:8: ( 'day' )
            // EsperEPL2Grammar.g:103:10: 'day'
            {
            	Match("day"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__242"

    // $ANTLR start "T__243"
    public void mT__243() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__243;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:104:8: ( 'hours' )
            // EsperEPL2Grammar.g:104:10: 'hours'
            {
            	Match("hours"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__243"

    // $ANTLR start "T__244"
    public void mT__244() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__244;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:105:8: ( 'hour' )
            // EsperEPL2Grammar.g:105:10: 'hour'
            {
            	Match("hour"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__244"

    // $ANTLR start "T__245"
    public void mT__245() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__245;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:106:8: ( 'minute' )
            // EsperEPL2Grammar.g:106:10: 'minute'
            {
            	Match("minute"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__245"

    // $ANTLR start "T__246"
    public void mT__246() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__246;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:107:8: ( 'second' )
            // EsperEPL2Grammar.g:107:10: 'second'
            {
            	Match("second"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__246"

    // $ANTLR start "T__247"
    public void mT__247() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__247;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:108:8: ( 'sec' )
            // EsperEPL2Grammar.g:108:10: 'sec'
            {
            	Match("sec"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__247"

    // $ANTLR start "T__248"
    public void mT__248() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__248;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:109:8: ( 'milliseconds' )
            // EsperEPL2Grammar.g:109:10: 'milliseconds'
            {
            	Match("milliseconds"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__248"

    // $ANTLR start "T__249"
    public void mT__249() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__249;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:110:8: ( 'millisecond' )
            // EsperEPL2Grammar.g:110:10: 'millisecond'
            {
            	Match("millisecond"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__249"

    // $ANTLR start "T__250"
    public void mT__250() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__250;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:111:8: ( 'msec' )
            // EsperEPL2Grammar.g:111:10: 'msec'
            {
            	Match("msec"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__250"

    // $ANTLR start "FOLLOWED_BY"
    public void mFOLLOWED_BY() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = FOLLOWED_BY;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1134:14: ( '->' )
            // EsperEPL2Grammar.g:1134:16: '->'
            {
            	Match("->"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "FOLLOWED_BY"

    // $ANTLR start "EQUALS"
    public void mEQUALS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = EQUALS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1135:10: ( '=' )
            // EsperEPL2Grammar.g:1135:12: '='
            {
            	Match('='); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "EQUALS"

    // $ANTLR start "SQL_NE"
    public void mSQL_NE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SQL_NE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1136:10: ( '<>' )
            // EsperEPL2Grammar.g:1136:12: '<>'
            {
            	Match("<>"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SQL_NE"

    // $ANTLR start "QUESTION"
    public void mQUESTION() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = QUESTION;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1137:11: ( '?' )
            // EsperEPL2Grammar.g:1137:13: '?'
            {
            	Match('?'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "QUESTION"

    // $ANTLR start "LPAREN"
    public void mLPAREN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LPAREN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1138:10: ( '(' )
            // EsperEPL2Grammar.g:1138:12: '('
            {
            	Match('('); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LPAREN"

    // $ANTLR start "RPAREN"
    public void mRPAREN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = RPAREN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1139:10: ( ')' )
            // EsperEPL2Grammar.g:1139:12: ')'
            {
            	Match(')'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "RPAREN"

    // $ANTLR start "LBRACK"
    public void mLBRACK() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LBRACK;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1140:10: ( '[' )
            // EsperEPL2Grammar.g:1140:12: '['
            {
            	Match('['); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LBRACK"

    // $ANTLR start "RBRACK"
    public void mRBRACK() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = RBRACK;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1141:10: ( ']' )
            // EsperEPL2Grammar.g:1141:12: ']'
            {
            	Match(']'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "RBRACK"

    // $ANTLR start "LCURLY"
    public void mLCURLY() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LCURLY;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1142:10: ( '{' )
            // EsperEPL2Grammar.g:1142:12: '{'
            {
            	Match('{'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LCURLY"

    // $ANTLR start "RCURLY"
    public void mRCURLY() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = RCURLY;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1143:10: ( '}' )
            // EsperEPL2Grammar.g:1143:12: '}'
            {
            	Match('}'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "RCURLY"

    // $ANTLR start "COLON"
    public void mCOLON() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = COLON;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1144:9: ( ':' )
            // EsperEPL2Grammar.g:1144:11: ':'
            {
            	Match(':'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "COLON"

    // $ANTLR start "COMMA"
    public void mCOMMA() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = COMMA;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1145:9: ( ',' )
            // EsperEPL2Grammar.g:1145:11: ','
            {
            	Match(','); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "COMMA"

    // $ANTLR start "EQUAL"
    public void mEQUAL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = EQUAL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1146:9: ( '==' )
            // EsperEPL2Grammar.g:1146:11: '=='
            {
            	Match("=="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "EQUAL"

    // $ANTLR start "LNOT"
    public void mLNOT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LNOT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1147:8: ( '!' )
            // EsperEPL2Grammar.g:1147:10: '!'
            {
            	Match('!'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LNOT"

    // $ANTLR start "BNOT"
    public void mBNOT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BNOT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1148:8: ( '~' )
            // EsperEPL2Grammar.g:1148:10: '~'
            {
            	Match('~'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BNOT"

    // $ANTLR start "NOT_EQUAL"
    public void mNOT_EQUAL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NOT_EQUAL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1149:12: ( '!=' )
            // EsperEPL2Grammar.g:1149:14: '!='
            {
            	Match("!="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NOT_EQUAL"

    // $ANTLR start "DIV"
    public void mDIV() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = DIV;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1150:7: ( '/' )
            // EsperEPL2Grammar.g:1150:9: '/'
            {
            	Match('/'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "DIV"

    // $ANTLR start "DIV_ASSIGN"
    public void mDIV_ASSIGN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = DIV_ASSIGN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1151:13: ( '/=' )
            // EsperEPL2Grammar.g:1151:15: '/='
            {
            	Match("/="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "DIV_ASSIGN"

    // $ANTLR start "PLUS"
    public void mPLUS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = PLUS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1152:8: ( '+' )
            // EsperEPL2Grammar.g:1152:10: '+'
            {
            	Match('+'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "PLUS"

    // $ANTLR start "PLUS_ASSIGN"
    public void mPLUS_ASSIGN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = PLUS_ASSIGN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1153:13: ( '+=' )
            // EsperEPL2Grammar.g:1153:15: '+='
            {
            	Match("+="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "PLUS_ASSIGN"

    // $ANTLR start "INC"
    public void mINC() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = INC;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1154:7: ( '++' )
            // EsperEPL2Grammar.g:1154:9: '++'
            {
            	Match("++"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "INC"

    // $ANTLR start "MINUS"
    public void mMINUS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MINUS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1155:9: ( '-' )
            // EsperEPL2Grammar.g:1155:11: '-'
            {
            	Match('-'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MINUS"

    // $ANTLR start "MINUS_ASSIGN"
    public void mMINUS_ASSIGN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MINUS_ASSIGN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1156:15: ( '-=' )
            // EsperEPL2Grammar.g:1156:17: '-='
            {
            	Match("-="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MINUS_ASSIGN"

    // $ANTLR start "DEC"
    public void mDEC() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = DEC;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1157:7: ( '--' )
            // EsperEPL2Grammar.g:1157:9: '--'
            {
            	Match("--"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "DEC"

    // $ANTLR start "STAR"
    public void mSTAR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = STAR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1158:8: ( '*' )
            // EsperEPL2Grammar.g:1158:10: '*'
            {
            	Match('*'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "STAR"

    // $ANTLR start "STAR_ASSIGN"
    public void mSTAR_ASSIGN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = STAR_ASSIGN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1159:14: ( '*=' )
            // EsperEPL2Grammar.g:1159:16: '*='
            {
            	Match("*="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "STAR_ASSIGN"

    // $ANTLR start "MOD"
    public void mMOD() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MOD;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1160:7: ( '%' )
            // EsperEPL2Grammar.g:1160:9: '%'
            {
            	Match('%'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MOD"

    // $ANTLR start "MOD_ASSIGN"
    public void mMOD_ASSIGN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MOD_ASSIGN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1161:13: ( '%=' )
            // EsperEPL2Grammar.g:1161:15: '%='
            {
            	Match("%="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MOD_ASSIGN"

    // $ANTLR start "SR"
    public void mSR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1162:6: ( '>>' )
            // EsperEPL2Grammar.g:1162:8: '>>'
            {
            	Match(">>"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SR"

    // $ANTLR start "SR_ASSIGN"
    public void mSR_ASSIGN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SR_ASSIGN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1163:12: ( '>>=' )
            // EsperEPL2Grammar.g:1163:14: '>>='
            {
            	Match(">>="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SR_ASSIGN"

    // $ANTLR start "BSR"
    public void mBSR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BSR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1164:7: ( '>>>' )
            // EsperEPL2Grammar.g:1164:9: '>>>'
            {
            	Match(">>>"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BSR"

    // $ANTLR start "BSR_ASSIGN"
    public void mBSR_ASSIGN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BSR_ASSIGN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1165:13: ( '>>>=' )
            // EsperEPL2Grammar.g:1165:15: '>>>='
            {
            	Match(">>>="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BSR_ASSIGN"

    // $ANTLR start "GE"
    public void mGE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = GE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1166:6: ( '>=' )
            // EsperEPL2Grammar.g:1166:8: '>='
            {
            	Match(">="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "GE"

    // $ANTLR start "GT"
    public void mGT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = GT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1167:6: ( '>' )
            // EsperEPL2Grammar.g:1167:8: '>'
            {
            	Match('>'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "GT"

    // $ANTLR start "SL"
    public void mSL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1168:6: ( '<<' )
            // EsperEPL2Grammar.g:1168:8: '<<'
            {
            	Match("<<"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SL"

    // $ANTLR start "SL_ASSIGN"
    public void mSL_ASSIGN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SL_ASSIGN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1169:12: ( '<<=' )
            // EsperEPL2Grammar.g:1169:14: '<<='
            {
            	Match("<<="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SL_ASSIGN"

    // $ANTLR start "LE"
    public void mLE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1170:6: ( '<=' )
            // EsperEPL2Grammar.g:1170:8: '<='
            {
            	Match("<="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LE"

    // $ANTLR start "LT"
    public void mLT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1171:6: ( '<' )
            // EsperEPL2Grammar.g:1171:8: '<'
            {
            	Match('<'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LT"

    // $ANTLR start "BXOR"
    public void mBXOR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BXOR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1172:8: ( '^' )
            // EsperEPL2Grammar.g:1172:10: '^'
            {
            	Match('^'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BXOR"

    // $ANTLR start "BXOR_ASSIGN"
    public void mBXOR_ASSIGN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BXOR_ASSIGN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1173:14: ( '^=' )
            // EsperEPL2Grammar.g:1173:16: '^='
            {
            	Match("^="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BXOR_ASSIGN"

    // $ANTLR start "BOR"
    public void mBOR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BOR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1174:6: ( '|' )
            // EsperEPL2Grammar.g:1174:8: '|'
            {
            	Match('|'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BOR"

    // $ANTLR start "BOR_ASSIGN"
    public void mBOR_ASSIGN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BOR_ASSIGN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1175:13: ( '|=' )
            // EsperEPL2Grammar.g:1175:15: '|='
            {
            	Match("|="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BOR_ASSIGN"

    // $ANTLR start "LOR"
    public void mLOR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LOR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1176:6: ( '||' )
            // EsperEPL2Grammar.g:1176:8: '||'
            {
            	Match("||"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LOR"

    // $ANTLR start "BAND"
    public void mBAND() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BAND;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1177:8: ( '&' )
            // EsperEPL2Grammar.g:1177:10: '&'
            {
            	Match('&'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BAND"

    // $ANTLR start "BAND_ASSIGN"
    public void mBAND_ASSIGN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = BAND_ASSIGN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1178:14: ( '&=' )
            // EsperEPL2Grammar.g:1178:16: '&='
            {
            	Match("&="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "BAND_ASSIGN"

    // $ANTLR start "LAND"
    public void mLAND() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = LAND;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1179:8: ( '&&' )
            // EsperEPL2Grammar.g:1179:10: '&&'
            {
            	Match("&&"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "LAND"

    // $ANTLR start "SEMI"
    public void mSEMI() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SEMI;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1180:8: ( ';' )
            // EsperEPL2Grammar.g:1180:10: ';'
            {
            	Match(';'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SEMI"

    // $ANTLR start "DOT"
    public void mDOT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = DOT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1181:7: ( '.' )
            // EsperEPL2Grammar.g:1181:9: '.'
            {
            	Match('.'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "DOT"

    // $ANTLR start "NUM_LONG"
    public void mNUM_LONG() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NUM_LONG;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1182:10: ( '\\u18FF' )
            // EsperEPL2Grammar.g:1182:12: '\\u18FF'
            {
            	Match('\u18FF'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NUM_LONG"

    // $ANTLR start "NUM_DOUBLE"
    public void mNUM_DOUBLE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NUM_DOUBLE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1183:12: ( '\\u18FE' )
            // EsperEPL2Grammar.g:1183:14: '\\u18FE'
            {
            	Match('\u18FE'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NUM_DOUBLE"

    // $ANTLR start "NUM_FLOAT"
    public void mNUM_FLOAT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NUM_FLOAT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1184:11: ( '\\u18FD' )
            // EsperEPL2Grammar.g:1184:13: '\\u18FD'
            {
            	Match('\u18FD'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NUM_FLOAT"

    // $ANTLR start "ESCAPECHAR"
    public void mESCAPECHAR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ESCAPECHAR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1185:12: ( '\\\\' )
            // EsperEPL2Grammar.g:1185:14: '\\\\'
            {
            	Match('\\'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ESCAPECHAR"

    // $ANTLR start "WS"
    public void mWS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1188:4: ( ( ' ' | '\\t' | '\\f' | ( '\\r' | '\\n' ) )+ )
            // EsperEPL2Grammar.g:1188:6: ( ' ' | '\\t' | '\\f' | ( '\\r' | '\\n' ) )+
            {
            	// EsperEPL2Grammar.g:1188:6: ( ' ' | '\\t' | '\\f' | ( '\\r' | '\\n' ) )+
            	int cnt1 = 0;
            	do 
            	{
            	    int alt1 = 2;
            	    int LA1_0 = input.LA(1);

            	    if ( ((LA1_0 >= '\t' && LA1_0 <= '\n') || (LA1_0 >= '\f' && LA1_0 <= '\r') || LA1_0 == ' ') )
            	    {
            	        alt1 = 1;
            	    }


            	    switch (alt1) 
            		{
            			case 1 :
            			    // EsperEPL2Grammar.g:
            			    {
            			    	if ( (input.LA(1) >= '\t' && input.LA(1) <= '\n') || (input.LA(1) >= '\f' && input.LA(1) <= '\r') || input.LA(1) == ' ' ) 
            			    	{
            			    	    input.Consume();
            			    	state.failed = false;
            			    	}
            			    	else 
            			    	{
            			    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            			    	    MismatchedSetException mse = new MismatchedSetException(null,input);
            			    	    Recover(mse);
            			    	    throw mse;}


            			    }
            			    break;

            			default:
            			    if ( cnt1 >= 1 ) goto loop1;
            			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            		            EarlyExitException eee =
            		                new EarlyExitException(1, input);
            		            throw eee;
            	    }
            	    cnt1++;
            	} while (true);

            	loop1:
            		;	// Stops C# compiler whinging that label 'loop1' has no statements

            	if ( state.backtracking == 0 ) 
            	{
            	   _channel=HIDDEN; 
            	}

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WS"

    // $ANTLR start "SL_COMMENT"
    public void mSL_COMMENT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SL_COMMENT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1202:2: ( '//' (~ ( '\\n' | '\\r' ) )* ( '\\n' | '\\r' ( '\\n' )? )? )
            // EsperEPL2Grammar.g:1202:4: '//' (~ ( '\\n' | '\\r' ) )* ( '\\n' | '\\r' ( '\\n' )? )?
            {
            	Match("//"); if (state.failed) return ;

            	// EsperEPL2Grammar.g:1203:3: (~ ( '\\n' | '\\r' ) )*
            	do 
            	{
            	    int alt2 = 2;
            	    int LA2_0 = input.LA(1);

            	    if ( ((LA2_0 >= '\u0000' && LA2_0 <= '\t') || (LA2_0 >= '\u000B' && LA2_0 <= '\f') || (LA2_0 >= '\u000E' && LA2_0 <= '\uFFFE')) )
            	    {
            	        alt2 = 1;
            	    }


            	    switch (alt2) 
            		{
            			case 1 :
            			    // EsperEPL2Grammar.g:1203:4: ~ ( '\\n' | '\\r' )
            			    {
            			    	if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '\uFFFE') ) 
            			    	{
            			    	    input.Consume();
            			    	state.failed = false;
            			    	}
            			    	else 
            			    	{
            			    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            			    	    MismatchedSetException mse = new MismatchedSetException(null,input);
            			    	    Recover(mse);
            			    	    throw mse;}


            			    }
            			    break;

            			default:
            			    goto loop2;
            	    }
            	} while (true);

            	loop2:
            		;	// Stops C# compiler whining that label 'loop2' has no statements

            	// EsperEPL2Grammar.g:1203:19: ( '\\n' | '\\r' ( '\\n' )? )?
            	int alt4 = 3;
            	int LA4_0 = input.LA(1);

            	if ( (LA4_0 == '\n') )
            	{
            	    alt4 = 1;
            	}
            	else if ( (LA4_0 == '\r') )
            	{
            	    alt4 = 2;
            	}
            	switch (alt4) 
            	{
            	    case 1 :
            	        // EsperEPL2Grammar.g:1203:20: '\\n'
            	        {
            	        	Match('\n'); if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // EsperEPL2Grammar.g:1203:25: '\\r' ( '\\n' )?
            	        {
            	        	Match('\r'); if (state.failed) return ;
            	        	// EsperEPL2Grammar.g:1203:29: ( '\\n' )?
            	        	int alt3 = 2;
            	        	int LA3_0 = input.LA(1);

            	        	if ( (LA3_0 == '\n') )
            	        	{
            	        	    alt3 = 1;
            	        	}
            	        	switch (alt3) 
            	        	{
            	        	    case 1 :
            	        	        // EsperEPL2Grammar.g:1203:30: '\\n'
            	        	        {
            	        	        	Match('\n'); if (state.failed) return ;

            	        	        }
            	        	        break;

            	        	}


            	        }
            	        break;

            	}

            	if ( state.backtracking == 0 ) 
            	{
            	  _channel=HIDDEN;
            	}

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SL_COMMENT"

    // $ANTLR start "ML_COMMENT"
    public void mML_COMMENT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = ML_COMMENT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1209:5: ( '/*' ( options {greedy=false; } : . )* '*/' )
            // EsperEPL2Grammar.g:1209:9: '/*' ( options {greedy=false; } : . )* '*/'
            {
            	Match("/*"); if (state.failed) return ;

            	// EsperEPL2Grammar.g:1209:14: ( options {greedy=false; } : . )*
            	do 
            	{
            	    int alt5 = 2;
            	    int LA5_0 = input.LA(1);

            	    if ( (LA5_0 == '*') )
            	    {
            	        int LA5_1 = input.LA(2);

            	        if ( (LA5_1 == '/') )
            	        {
            	            alt5 = 2;
            	        }
            	        else if ( ((LA5_1 >= '\u0000' && LA5_1 <= '.') || (LA5_1 >= '0' && LA5_1 <= '\uFFFE')) )
            	        {
            	            alt5 = 1;
            	        }


            	    }
            	    else if ( ((LA5_0 >= '\u0000' && LA5_0 <= ')') || (LA5_0 >= '+' && LA5_0 <= '\uFFFE')) )
            	    {
            	        alt5 = 1;
            	    }


            	    switch (alt5) 
            		{
            			case 1 :
            			    // EsperEPL2Grammar.g:1209:42: .
            			    {
            			    	MatchAny(); if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop5;
            	    }
            	} while (true);

            	loop5:
            		;	// Stops C# compiler whining that label 'loop5' has no statements

            	Match("*/"); if (state.failed) return ;

            	if ( state.backtracking == 0 ) 
            	{
            	  _channel=HIDDEN;
            	}

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "ML_COMMENT"

    // $ANTLR start "STRING_LITERAL"
    public void mSTRING_LITERAL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = STRING_LITERAL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1214:2: ( '\"' ( ESC | ~ ( '\"' | '\\\\' | '\\n' | '\\r' ) )* '\"' )
            // EsperEPL2Grammar.g:1214:4: '\"' ( ESC | ~ ( '\"' | '\\\\' | '\\n' | '\\r' ) )* '\"'
            {
            	Match('\"'); if (state.failed) return ;
            	// EsperEPL2Grammar.g:1214:8: ( ESC | ~ ( '\"' | '\\\\' | '\\n' | '\\r' ) )*
            	do 
            	{
            	    int alt6 = 3;
            	    int LA6_0 = input.LA(1);

            	    if ( (LA6_0 == '\\') )
            	    {
            	        alt6 = 1;
            	    }
            	    else if ( ((LA6_0 >= '\u0000' && LA6_0 <= '\t') || (LA6_0 >= '\u000B' && LA6_0 <= '\f') || (LA6_0 >= '\u000E' && LA6_0 <= '!') || (LA6_0 >= '#' && LA6_0 <= '[') || (LA6_0 >= ']' && LA6_0 <= '\uFFFE')) )
            	    {
            	        alt6 = 2;
            	    }


            	    switch (alt6) 
            		{
            			case 1 :
            			    // EsperEPL2Grammar.g:1214:9: ESC
            			    {
            			    	mESC(); if (state.failed) return ;

            			    }
            			    break;
            			case 2 :
            			    // EsperEPL2Grammar.g:1214:13: ~ ( '\"' | '\\\\' | '\\n' | '\\r' )
            			    {
            			    	if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '!') || (input.LA(1) >= '#' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\uFFFE') ) 
            			    	{
            			    	    input.Consume();
            			    	state.failed = false;
            			    	}
            			    	else 
            			    	{
            			    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            			    	    MismatchedSetException mse = new MismatchedSetException(null,input);
            			    	    Recover(mse);
            			    	    throw mse;}


            			    }
            			    break;

            			default:
            			    goto loop6;
            	    }
            	} while (true);

            	loop6:
            		;	// Stops C# compiler whining that label 'loop6' has no statements

            	Match('\"'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "STRING_LITERAL"

    // $ANTLR start "QUOTED_STRING_LITERAL"
    public void mQUOTED_STRING_LITERAL() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = QUOTED_STRING_LITERAL;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1218:2: ( '\\'' ( ESC | ~ ( '\\'' | '\\\\' | '\\n' | '\\r' ) )* '\\'' )
            // EsperEPL2Grammar.g:1218:4: '\\'' ( ESC | ~ ( '\\'' | '\\\\' | '\\n' | '\\r' ) )* '\\''
            {
            	Match('\''); if (state.failed) return ;
            	// EsperEPL2Grammar.g:1218:9: ( ESC | ~ ( '\\'' | '\\\\' | '\\n' | '\\r' ) )*
            	do 
            	{
            	    int alt7 = 3;
            	    int LA7_0 = input.LA(1);

            	    if ( (LA7_0 == '\\') )
            	    {
            	        alt7 = 1;
            	    }
            	    else if ( ((LA7_0 >= '\u0000' && LA7_0 <= '\t') || (LA7_0 >= '\u000B' && LA7_0 <= '\f') || (LA7_0 >= '\u000E' && LA7_0 <= '&') || (LA7_0 >= '(' && LA7_0 <= '[') || (LA7_0 >= ']' && LA7_0 <= '\uFFFE')) )
            	    {
            	        alt7 = 2;
            	    }


            	    switch (alt7) 
            		{
            			case 1 :
            			    // EsperEPL2Grammar.g:1218:10: ESC
            			    {
            			    	mESC(); if (state.failed) return ;

            			    }
            			    break;
            			case 2 :
            			    // EsperEPL2Grammar.g:1218:14: ~ ( '\\'' | '\\\\' | '\\n' | '\\r' )
            			    {
            			    	if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '&') || (input.LA(1) >= '(' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\uFFFE') ) 
            			    	{
            			    	    input.Consume();
            			    	state.failed = false;
            			    	}
            			    	else 
            			    	{
            			    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            			    	    MismatchedSetException mse = new MismatchedSetException(null,input);
            			    	    Recover(mse);
            			    	    throw mse;}


            			    }
            			    break;

            			default:
            			    goto loop7;
            	    }
            	} while (true);

            	loop7:
            		;	// Stops C# compiler whining that label 'loop7' has no statements

            	Match('\''); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "QUOTED_STRING_LITERAL"

    // $ANTLR start "ESC"
    public void mESC() // throws RecognitionException [2]
    {
    		try
    		{
            // EsperEPL2Grammar.g:1231:2: ( '\\\\' ( 'n' | 'r' | 't' | 'b' | 'f' | '\"' | '\\'' | '\\\\' | ( 'u' )+ HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT | '0' .. '3' ( '0' .. '7' ( '0' .. '7' )? )? | '4' .. '7' ( '0' .. '7' )? ) )
            // EsperEPL2Grammar.g:1231:4: '\\\\' ( 'n' | 'r' | 't' | 'b' | 'f' | '\"' | '\\'' | '\\\\' | ( 'u' )+ HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT | '0' .. '3' ( '0' .. '7' ( '0' .. '7' )? )? | '4' .. '7' ( '0' .. '7' )? )
            {
            	Match('\\'); if (state.failed) return ;
            	// EsperEPL2Grammar.g:1232:3: ( 'n' | 'r' | 't' | 'b' | 'f' | '\"' | '\\'' | '\\\\' | ( 'u' )+ HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT | '0' .. '3' ( '0' .. '7' ( '0' .. '7' )? )? | '4' .. '7' ( '0' .. '7' )? )
            	int alt12 = 11;
            	switch ( input.LA(1) ) 
            	{
            	case 'n':
            		{
            	    alt12 = 1;
            	    }
            	    break;
            	case 'r':
            		{
            	    alt12 = 2;
            	    }
            	    break;
            	case 't':
            		{
            	    alt12 = 3;
            	    }
            	    break;
            	case 'b':
            		{
            	    alt12 = 4;
            	    }
            	    break;
            	case 'f':
            		{
            	    alt12 = 5;
            	    }
            	    break;
            	case '\"':
            		{
            	    alt12 = 6;
            	    }
            	    break;
            	case '\'':
            		{
            	    alt12 = 7;
            	    }
            	    break;
            	case '\\':
            		{
            	    alt12 = 8;
            	    }
            	    break;
            	case 'u':
            		{
            	    alt12 = 9;
            	    }
            	    break;
            	case '0':
            	case '1':
            	case '2':
            	case '3':
            		{
            	    alt12 = 10;
            	    }
            	    break;
            	case '4':
            	case '5':
            	case '6':
            	case '7':
            		{
            	    alt12 = 11;
            	    }
            	    break;
            		default:
            		    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            		    NoViableAltException nvae_d12s0 =
            		        new NoViableAltException("", 12, 0, input);

            		    throw nvae_d12s0;
            	}

            	switch (alt12) 
            	{
            	    case 1 :
            	        // EsperEPL2Grammar.g:1232:5: 'n'
            	        {
            	        	Match('n'); if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // EsperEPL2Grammar.g:1233:5: 'r'
            	        {
            	        	Match('r'); if (state.failed) return ;

            	        }
            	        break;
            	    case 3 :
            	        // EsperEPL2Grammar.g:1234:5: 't'
            	        {
            	        	Match('t'); if (state.failed) return ;

            	        }
            	        break;
            	    case 4 :
            	        // EsperEPL2Grammar.g:1235:5: 'b'
            	        {
            	        	Match('b'); if (state.failed) return ;

            	        }
            	        break;
            	    case 5 :
            	        // EsperEPL2Grammar.g:1236:5: 'f'
            	        {
            	        	Match('f'); if (state.failed) return ;

            	        }
            	        break;
            	    case 6 :
            	        // EsperEPL2Grammar.g:1237:5: '\"'
            	        {
            	        	Match('\"'); if (state.failed) return ;

            	        }
            	        break;
            	    case 7 :
            	        // EsperEPL2Grammar.g:1238:5: '\\''
            	        {
            	        	Match('\''); if (state.failed) return ;

            	        }
            	        break;
            	    case 8 :
            	        // EsperEPL2Grammar.g:1239:5: '\\\\'
            	        {
            	        	Match('\\'); if (state.failed) return ;

            	        }
            	        break;
            	    case 9 :
            	        // EsperEPL2Grammar.g:1240:5: ( 'u' )+ HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
            	        {
            	        	// EsperEPL2Grammar.g:1240:5: ( 'u' )+
            	        	int cnt8 = 0;
            	        	do 
            	        	{
            	        	    int alt8 = 2;
            	        	    int LA8_0 = input.LA(1);

            	        	    if ( (LA8_0 == 'u') )
            	        	    {
            	        	        alt8 = 1;
            	        	    }


            	        	    switch (alt8) 
            	        		{
            	        			case 1 :
            	        			    // EsperEPL2Grammar.g:1240:6: 'u'
            	        			    {
            	        			    	Match('u'); if (state.failed) return ;

            	        			    }
            	        			    break;

            	        			default:
            	        			    if ( cnt8 >= 1 ) goto loop8;
            	        			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	        		            EarlyExitException eee =
            	        		                new EarlyExitException(8, input);
            	        		            throw eee;
            	        	    }
            	        	    cnt8++;
            	        	} while (true);

            	        	loop8:
            	        		;	// Stops C# compiler whinging that label 'loop8' has no statements

            	        	mHEX_DIGIT(); if (state.failed) return ;
            	        	mHEX_DIGIT(); if (state.failed) return ;
            	        	mHEX_DIGIT(); if (state.failed) return ;
            	        	mHEX_DIGIT(); if (state.failed) return ;

            	        }
            	        break;
            	    case 10 :
            	        // EsperEPL2Grammar.g:1241:5: '0' .. '3' ( '0' .. '7' ( '0' .. '7' )? )?
            	        {
            	        	MatchRange('0','3'); if (state.failed) return ;
            	        	// EsperEPL2Grammar.g:1242:4: ( '0' .. '7' ( '0' .. '7' )? )?
            	        	int alt10 = 2;
            	        	int LA10_0 = input.LA(1);

            	        	if ( ((LA10_0 >= '0' && LA10_0 <= '7')) )
            	        	{
            	        	    alt10 = 1;
            	        	}
            	        	switch (alt10) 
            	        	{
            	        	    case 1 :
            	        	        // EsperEPL2Grammar.g:1243:5: '0' .. '7' ( '0' .. '7' )?
            	        	        {
            	        	        	MatchRange('0','7'); if (state.failed) return ;
            	        	        	// EsperEPL2Grammar.g:1244:5: ( '0' .. '7' )?
            	        	        	int alt9 = 2;
            	        	        	int LA9_0 = input.LA(1);

            	        	        	if ( ((LA9_0 >= '0' && LA9_0 <= '7')) )
            	        	        	{
            	        	        	    alt9 = 1;
            	        	        	}
            	        	        	switch (alt9) 
            	        	        	{
            	        	        	    case 1 :
            	        	        	        // EsperEPL2Grammar.g:1245:6: '0' .. '7'
            	        	        	        {
            	        	        	        	MatchRange('0','7'); if (state.failed) return ;

            	        	        	        }
            	        	        	        break;

            	        	        	}


            	        	        }
            	        	        break;

            	        	}


            	        }
            	        break;
            	    case 11 :
            	        // EsperEPL2Grammar.g:1248:5: '4' .. '7' ( '0' .. '7' )?
            	        {
            	        	MatchRange('4','7'); if (state.failed) return ;
            	        	// EsperEPL2Grammar.g:1249:4: ( '0' .. '7' )?
            	        	int alt11 = 2;
            	        	int LA11_0 = input.LA(1);

            	        	if ( ((LA11_0 >= '0' && LA11_0 <= '7')) )
            	        	{
            	        	    alt11 = 1;
            	        	}
            	        	switch (alt11) 
            	        	{
            	        	    case 1 :
            	        	        // EsperEPL2Grammar.g:1250:5: '0' .. '7'
            	        	        {
            	        	        	MatchRange('0','7'); if (state.failed) return ;

            	        	        }
            	        	        break;

            	        	}


            	        }
            	        break;

            	}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "ESC"

    // $ANTLR start "HEX_DIGIT"
    public void mHEX_DIGIT() // throws RecognitionException [2]
    {
    		try
    		{
            // EsperEPL2Grammar.g:1259:2: ( ( '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' ) )
            // EsperEPL2Grammar.g:1259:4: ( '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' )
            {
            	if ( (input.LA(1) >= '0' && input.LA(1) <= '9') || (input.LA(1) >= 'A' && input.LA(1) <= 'F') || (input.LA(1) >= 'a' && input.LA(1) <= 'f') ) 
            	{
            	    input.Consume();
            	state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "HEX_DIGIT"

    // $ANTLR start "IDENT"
    public void mIDENT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = IDENT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // EsperEPL2Grammar.g:1267:2: ( ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '$' ) ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' | '$' )* )
            // EsperEPL2Grammar.g:1267:4: ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '$' ) ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' | '$' )*
            {
            	if ( input.LA(1) == '$' || (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
            	{
            	    input.Consume();
            	state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}

            	// EsperEPL2Grammar.g:1267:32: ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' | '$' )*
            	do 
            	{
            	    int alt13 = 2;
            	    int LA13_0 = input.LA(1);

            	    if ( (LA13_0 == '$' || (LA13_0 >= '0' && LA13_0 <= '9') || (LA13_0 >= 'A' && LA13_0 <= 'Z') || LA13_0 == '_' || (LA13_0 >= 'a' && LA13_0 <= 'z')) )
            	    {
            	        alt13 = 1;
            	    }


            	    switch (alt13) 
            		{
            			case 1 :
            			    // EsperEPL2Grammar.g:
            			    {
            			    	if ( input.LA(1) == '$' || (input.LA(1) >= '0' && input.LA(1) <= '9') || (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
            			    	{
            			    	    input.Consume();
            			    	state.failed = false;
            			    	}
            			    	else 
            			    	{
            			    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            			    	    MismatchedSetException mse = new MismatchedSetException(null,input);
            			    	    Recover(mse);
            			    	    throw mse;}


            			    }
            			    break;

            			default:
            			    goto loop13;
            	    }
            	} while (true);

            	loop13:
            		;	// Stops C# compiler whining that label 'loop13' has no statements


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "IDENT"

    // $ANTLR start "NUM_INT"
    public void mNUM_INT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NUM_INT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            IToken f1 = null;
            IToken f2 = null;
            IToken f3 = null;
            IToken f4 = null;

            bool isDecimal=false; IToken t=null;
            // EsperEPL2Grammar.g:1274:5: ( '.' ( ( '0' .. '9' )+ ( EXPONENT )? (f1= FLOAT_SUFFIX )? )? | ( '0' ( ( 'x' ) ( HEX_DIGIT )+ | ( ( '0' .. '9' )+ ( '.' | EXPONENT | FLOAT_SUFFIX ) )=> ( '0' .. '9' )+ | ( '0' .. '7' )+ )? | ( '1' .. '9' ) ( '0' .. '9' )* ) ( ( 'l' ) | {...}? ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX ) )? )
            int alt30 = 2;
            int LA30_0 = input.LA(1);

            if ( (LA30_0 == '.') )
            {
                alt30 = 1;
            }
            else if ( ((LA30_0 >= '0' && LA30_0 <= '9')) )
            {
                alt30 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d30s0 =
                    new NoViableAltException("", 30, 0, input);

                throw nvae_d30s0;
            }
            switch (alt30) 
            {
                case 1 :
                    // EsperEPL2Grammar.g:1274:9: '.' ( ( '0' .. '9' )+ ( EXPONENT )? (f1= FLOAT_SUFFIX )? )?
                    {
                    	Match('.'); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	  _type = DOT;
                    	}
                    	// EsperEPL2Grammar.g:1275:13: ( ( '0' .. '9' )+ ( EXPONENT )? (f1= FLOAT_SUFFIX )? )?
                    	int alt17 = 2;
                    	int LA17_0 = input.LA(1);

                    	if ( ((LA17_0 >= '0' && LA17_0 <= '9')) )
                    	{
                    	    alt17 = 1;
                    	}
                    	switch (alt17) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Grammar.g:1275:15: ( '0' .. '9' )+ ( EXPONENT )? (f1= FLOAT_SUFFIX )?
                    	        {
                    	        	// EsperEPL2Grammar.g:1275:15: ( '0' .. '9' )+
                    	        	int cnt14 = 0;
                    	        	do 
                    	        	{
                    	        	    int alt14 = 2;
                    	        	    int LA14_0 = input.LA(1);

                    	        	    if ( ((LA14_0 >= '0' && LA14_0 <= '9')) )
                    	        	    {
                    	        	        alt14 = 1;
                    	        	    }


                    	        	    switch (alt14) 
                    	        		{
                    	        			case 1 :
                    	        			    // EsperEPL2Grammar.g:1275:16: '0' .. '9'
                    	        			    {
                    	        			    	MatchRange('0','9'); if (state.failed) return ;

                    	        			    }
                    	        			    break;

                    	        			default:
                    	        			    if ( cnt14 >= 1 ) goto loop14;
                    	        			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        		            EarlyExitException eee =
                    	        		                new EarlyExitException(14, input);
                    	        		            throw eee;
                    	        	    }
                    	        	    cnt14++;
                    	        	} while (true);

                    	        	loop14:
                    	        		;	// Stops C# compiler whinging that label 'loop14' has no statements

                    	        	// EsperEPL2Grammar.g:1275:27: ( EXPONENT )?
                    	        	int alt15 = 2;
                    	        	int LA15_0 = input.LA(1);

                    	        	if ( (LA15_0 == 'e') )
                    	        	{
                    	        	    alt15 = 1;
                    	        	}
                    	        	switch (alt15) 
                    	        	{
                    	        	    case 1 :
                    	        	        // EsperEPL2Grammar.g:1275:28: EXPONENT
                    	        	        {
                    	        	        	mEXPONENT(); if (state.failed) return ;

                    	        	        }
                    	        	        break;

                    	        	}

                    	        	// EsperEPL2Grammar.g:1275:39: (f1= FLOAT_SUFFIX )?
                    	        	int alt16 = 2;
                    	        	int LA16_0 = input.LA(1);

                    	        	if ( (LA16_0 == 'd' || LA16_0 == 'f') )
                    	        	{
                    	        	    alt16 = 1;
                    	        	}
                    	        	switch (alt16) 
                    	        	{
                    	        	    case 1 :
                    	        	        // EsperEPL2Grammar.g:1275:40: f1= FLOAT_SUFFIX
                    	        	        {
                    	        	        	int f1Start1711 = CharIndex;
                    	        	        	mFLOAT_SUFFIX(); if (state.failed) return ;
                    	        	        	f1 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, f1Start1711, CharIndex-1);
                    	        	        	if ( state.backtracking == 0 ) 
                    	        	        	{
                    	        	        	  t=f1;
                    	        	        	}

                    	        	        }
                    	        	        break;

                    	        	}

                    	        	if ( state.backtracking == 0 ) 
                    	        	{

                    	        	  				if (t != null && t.Text.ToUpper().IndexOf('F')>=0) {
                    	        	                  	_type = NUM_FLOAT;
                    	        	  				}
                    	        	  				else {
                    	        	                  	_type = NUM_DOUBLE; // assume double
                    	        	  				}
                    	        	  				
                    	        	}

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // EsperEPL2Grammar.g:1286:4: ( '0' ( ( 'x' ) ( HEX_DIGIT )+ | ( ( '0' .. '9' )+ ( '.' | EXPONENT | FLOAT_SUFFIX ) )=> ( '0' .. '9' )+ | ( '0' .. '7' )+ )? | ( '1' .. '9' ) ( '0' .. '9' )* ) ( ( 'l' ) | {...}? ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX ) )?
                    {
                    	// EsperEPL2Grammar.g:1286:4: ( '0' ( ( 'x' ) ( HEX_DIGIT )+ | ( ( '0' .. '9' )+ ( '.' | EXPONENT | FLOAT_SUFFIX ) )=> ( '0' .. '9' )+ | ( '0' .. '7' )+ )? | ( '1' .. '9' ) ( '0' .. '9' )* )
                    	int alt23 = 2;
                    	int LA23_0 = input.LA(1);

                    	if ( (LA23_0 == '0') )
                    	{
                    	    alt23 = 1;
                    	}
                    	else if ( ((LA23_0 >= '1' && LA23_0 <= '9')) )
                    	{
                    	    alt23 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d23s0 =
                    	        new NoViableAltException("", 23, 0, input);

                    	    throw nvae_d23s0;
                    	}
                    	switch (alt23) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Grammar.g:1286:6: '0' ( ( 'x' ) ( HEX_DIGIT )+ | ( ( '0' .. '9' )+ ( '.' | EXPONENT | FLOAT_SUFFIX ) )=> ( '0' .. '9' )+ | ( '0' .. '7' )+ )?
                    	        {
                    	        	Match('0'); if (state.failed) return ;
                    	        	if ( state.backtracking == 0 ) 
                    	        	{
                    	        	  isDecimal = true;
                    	        	}
                    	        	// EsperEPL2Grammar.g:1287:4: ( ( 'x' ) ( HEX_DIGIT )+ | ( ( '0' .. '9' )+ ( '.' | EXPONENT | FLOAT_SUFFIX ) )=> ( '0' .. '9' )+ | ( '0' .. '7' )+ )?
                    	        	int alt21 = 4;
                    	        	int LA21_0 = input.LA(1);

                    	        	if ( (LA21_0 == 'x') )
                    	        	{
                    	        	    alt21 = 1;
                    	        	}
                    	        	else if ( ((LA21_0 >= '0' && LA21_0 <= '7')) )
                    	        	{
                    	        	    int LA21_2 = input.LA(2);

                    	        	    if ( (synpred1_EsperEPL2Grammar()) )
                    	        	    {
                    	        	        alt21 = 2;
                    	        	    }
                    	        	    else if ( (true) )
                    	        	    {
                    	        	        alt21 = 3;
                    	        	    }
                    	        	}
                    	        	else if ( ((LA21_0 >= '8' && LA21_0 <= '9')) && (synpred1_EsperEPL2Grammar()) )
                    	        	{
                    	        	    alt21 = 2;
                    	        	}
                    	        	switch (alt21) 
                    	        	{
                    	        	    case 1 :
                    	        	        // EsperEPL2Grammar.g:1287:6: ( 'x' ) ( HEX_DIGIT )+
                    	        	        {
                    	        	        	// EsperEPL2Grammar.g:1287:6: ( 'x' )
                    	        	        	// EsperEPL2Grammar.g:1287:7: 'x'
                    	        	        	{
                    	        	        		Match('x'); if (state.failed) return ;

                    	        	        	}

                    	        	        	// EsperEPL2Grammar.g:1288:5: ( HEX_DIGIT )+
                    	        	        	int cnt18 = 0;
                    	        	        	do 
                    	        	        	{
                    	        	        	    int alt18 = 2;
                    	        	        	    switch ( input.LA(1) ) 
                    	        	        	    {
                    	        	        	    case 'e':
                    	        	        	    	{
                    	        	        	        int LA18_2 = input.LA(2);

                    	        	        	        if ( ((LA18_2 >= '0' && LA18_2 <= '9')) )
                    	        	        	        {
                    	        	        	            int LA18_5 = input.LA(3);

                    	        	        	            if ( (!(((isDecimal)))) )
                    	        	        	            {
                    	        	        	                alt18 = 1;
                    	        	        	            }


                    	        	        	        }

                    	        	        	        else 
                    	        	        	        {
                    	        	        	            alt18 = 1;
                    	        	        	        }

                    	        	        	        }
                    	        	        	        break;
                    	        	        	    case 'd':
                    	        	        	    case 'f':
                    	        	        	    	{
                    	        	        	        int LA18_3 = input.LA(2);

                    	        	        	        if ( (!(((isDecimal)))) )
                    	        	        	        {
                    	        	        	            alt18 = 1;
                    	        	        	        }


                    	        	        	        }
                    	        	        	        break;
                    	        	        	    case '0':
                    	        	        	    case '1':
                    	        	        	    case '2':
                    	        	        	    case '3':
                    	        	        	    case '4':
                    	        	        	    case '5':
                    	        	        	    case '6':
                    	        	        	    case '7':
                    	        	        	    case '8':
                    	        	        	    case '9':
                    	        	        	    case 'A':
                    	        	        	    case 'B':
                    	        	        	    case 'C':
                    	        	        	    case 'D':
                    	        	        	    case 'E':
                    	        	        	    case 'F':
                    	        	        	    case 'a':
                    	        	        	    case 'b':
                    	        	        	    case 'c':
                    	        	        	    	{
                    	        	        	        alt18 = 1;
                    	        	        	        }
                    	        	        	        break;

                    	        	        	    }

                    	        	        	    switch (alt18) 
                    	        	        		{
                    	        	        			case 1 :
                    	        	        			    // EsperEPL2Grammar.g:1294:6: HEX_DIGIT
                    	        	        			    {
                    	        	        			    	mHEX_DIGIT(); if (state.failed) return ;

                    	        	        			    }
                    	        	        			    break;

                    	        	        			default:
                    	        	        			    if ( cnt18 >= 1 ) goto loop18;
                    	        	        			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        	        		            EarlyExitException eee =
                    	        	        		                new EarlyExitException(18, input);
                    	        	        		            throw eee;
                    	        	        	    }
                    	        	        	    cnt18++;
                    	        	        	} while (true);

                    	        	        	loop18:
                    	        	        		;	// Stops C# compiler whinging that label 'loop18' has no statements


                    	        	        }
                    	        	        break;
                    	        	    case 2 :
                    	        	        // EsperEPL2Grammar.g:1298:5: ( ( '0' .. '9' )+ ( '.' | EXPONENT | FLOAT_SUFFIX ) )=> ( '0' .. '9' )+
                    	        	        {
                    	        	        	// EsperEPL2Grammar.g:1298:50: ( '0' .. '9' )+
                    	        	        	int cnt19 = 0;
                    	        	        	do 
                    	        	        	{
                    	        	        	    int alt19 = 2;
                    	        	        	    int LA19_0 = input.LA(1);

                    	        	        	    if ( ((LA19_0 >= '0' && LA19_0 <= '9')) )
                    	        	        	    {
                    	        	        	        alt19 = 1;
                    	        	        	    }


                    	        	        	    switch (alt19) 
                    	        	        		{
                    	        	        			case 1 :
                    	        	        			    // EsperEPL2Grammar.g:1298:51: '0' .. '9'
                    	        	        			    {
                    	        	        			    	MatchRange('0','9'); if (state.failed) return ;

                    	        	        			    }
                    	        	        			    break;

                    	        	        			default:
                    	        	        			    if ( cnt19 >= 1 ) goto loop19;
                    	        	        			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        	        		            EarlyExitException eee =
                    	        	        		                new EarlyExitException(19, input);
                    	        	        		            throw eee;
                    	        	        	    }
                    	        	        	    cnt19++;
                    	        	        	} while (true);

                    	        	        	loop19:
                    	        	        		;	// Stops C# compiler whinging that label 'loop19' has no statements


                    	        	        }
                    	        	        break;
                    	        	    case 3 :
                    	        	        // EsperEPL2Grammar.g:1300:6: ( '0' .. '7' )+
                    	        	        {
                    	        	        	// EsperEPL2Grammar.g:1300:6: ( '0' .. '7' )+
                    	        	        	int cnt20 = 0;
                    	        	        	do 
                    	        	        	{
                    	        	        	    int alt20 = 2;
                    	        	        	    int LA20_0 = input.LA(1);

                    	        	        	    if ( ((LA20_0 >= '0' && LA20_0 <= '7')) )
                    	        	        	    {
                    	        	        	        alt20 = 1;
                    	        	        	    }


                    	        	        	    switch (alt20) 
                    	        	        		{
                    	        	        			case 1 :
                    	        	        			    // EsperEPL2Grammar.g:1300:7: '0' .. '7'
                    	        	        			    {
                    	        	        			    	MatchRange('0','7'); if (state.failed) return ;

                    	        	        			    }
                    	        	        			    break;

                    	        	        			default:
                    	        	        			    if ( cnt20 >= 1 ) goto loop20;
                    	        	        			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        	        		            EarlyExitException eee =
                    	        	        		                new EarlyExitException(20, input);
                    	        	        		            throw eee;
                    	        	        	    }
                    	        	        	    cnt20++;
                    	        	        	} while (true);

                    	        	        	loop20:
                    	        	        		;	// Stops C# compiler whinging that label 'loop20' has no statements


                    	        	        }
                    	        	        break;

                    	        	}


                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Grammar.g:1302:5: ( '1' .. '9' ) ( '0' .. '9' )*
                    	        {
                    	        	// EsperEPL2Grammar.g:1302:5: ( '1' .. '9' )
                    	        	// EsperEPL2Grammar.g:1302:6: '1' .. '9'
                    	        	{
                    	        		MatchRange('1','9'); if (state.failed) return ;

                    	        	}

                    	        	// EsperEPL2Grammar.g:1302:16: ( '0' .. '9' )*
                    	        	do 
                    	        	{
                    	        	    int alt22 = 2;
                    	        	    int LA22_0 = input.LA(1);

                    	        	    if ( ((LA22_0 >= '0' && LA22_0 <= '9')) )
                    	        	    {
                    	        	        alt22 = 1;
                    	        	    }


                    	        	    switch (alt22) 
                    	        		{
                    	        			case 1 :
                    	        			    // EsperEPL2Grammar.g:1302:17: '0' .. '9'
                    	        			    {
                    	        			    	MatchRange('0','9'); if (state.failed) return ;

                    	        			    }
                    	        			    break;

                    	        			default:
                    	        			    goto loop22;
                    	        	    }
                    	        	} while (true);

                    	        	loop22:
                    	        		;	// Stops C# compiler whining that label 'loop22' has no statements

                    	        	if ( state.backtracking == 0 ) 
                    	        	{
                    	        	  isDecimal=true;
                    	        	}

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Grammar.g:1304:3: ( ( 'l' ) | {...}? ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX ) )?
                    	int alt29 = 3;
                    	int LA29_0 = input.LA(1);

                    	if ( (LA29_0 == 'l') )
                    	{
                    	    alt29 = 1;
                    	}
                    	else if ( (LA29_0 == '.' || (LA29_0 >= 'd' && LA29_0 <= 'f')) )
                    	{
                    	    alt29 = 2;
                    	}
                    	switch (alt29) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Grammar.g:1304:5: ( 'l' )
                    	        {
                    	        	// EsperEPL2Grammar.g:1304:5: ( 'l' )
                    	        	// EsperEPL2Grammar.g:1304:6: 'l'
                    	        	{
                    	        		Match('l'); if (state.failed) return ;

                    	        	}

                    	        	if ( state.backtracking == 0 ) 
                    	        	{
                    	        	   _type = NUM_LONG; 
                    	        	}

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Grammar.g:1307:5: {...}? ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX )
                    	        {
                    	        	if ( !((isDecimal)) ) 
                    	        	{
                    	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        	    throw new FailedPredicateException(input, "NUM_INT", "isDecimal");
                    	        	}
                    	        	// EsperEPL2Grammar.g:1308:13: ( '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )? | EXPONENT (f3= FLOAT_SUFFIX )? | f4= FLOAT_SUFFIX )
                    	        	int alt28 = 3;
                    	        	switch ( input.LA(1) ) 
                    	        	{
                    	        	case '.':
                    	        		{
                    	        	    alt28 = 1;
                    	        	    }
                    	        	    break;
                    	        	case 'e':
                    	        		{
                    	        	    alt28 = 2;
                    	        	    }
                    	        	    break;
                    	        	case 'd':
                    	        	case 'f':
                    	        		{
                    	        	    alt28 = 3;
                    	        	    }
                    	        	    break;
                    	        		default:
                    	        		    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        		    NoViableAltException nvae_d28s0 =
                    	        		        new NoViableAltException("", 28, 0, input);

                    	        		    throw nvae_d28s0;
                    	        	}

                    	        	switch (alt28) 
                    	        	{
                    	        	    case 1 :
                    	        	        // EsperEPL2Grammar.g:1308:17: '.' ( '0' .. '9' )* ( EXPONENT )? (f2= FLOAT_SUFFIX )?
                    	        	        {
                    	        	        	Match('.'); if (state.failed) return ;
                    	        	        	// EsperEPL2Grammar.g:1308:21: ( '0' .. '9' )*
                    	        	        	do 
                    	        	        	{
                    	        	        	    int alt24 = 2;
                    	        	        	    int LA24_0 = input.LA(1);

                    	        	        	    if ( ((LA24_0 >= '0' && LA24_0 <= '9')) )
                    	        	        	    {
                    	        	        	        alt24 = 1;
                    	        	        	    }


                    	        	        	    switch (alt24) 
                    	        	        		{
                    	        	        			case 1 :
                    	        	        			    // EsperEPL2Grammar.g:1308:22: '0' .. '9'
                    	        	        			    {
                    	        	        			    	MatchRange('0','9'); if (state.failed) return ;

                    	        	        			    }
                    	        	        			    break;

                    	        	        			default:
                    	        	        			    goto loop24;
                    	        	        	    }
                    	        	        	} while (true);

                    	        	        	loop24:
                    	        	        		;	// Stops C# compiler whining that label 'loop24' has no statements

                    	        	        	// EsperEPL2Grammar.g:1308:33: ( EXPONENT )?
                    	        	        	int alt25 = 2;
                    	        	        	int LA25_0 = input.LA(1);

                    	        	        	if ( (LA25_0 == 'e') )
                    	        	        	{
                    	        	        	    alt25 = 1;
                    	        	        	}
                    	        	        	switch (alt25) 
                    	        	        	{
                    	        	        	    case 1 :
                    	        	        	        // EsperEPL2Grammar.g:1308:34: EXPONENT
                    	        	        	        {
                    	        	        	        	mEXPONENT(); if (state.failed) return ;

                    	        	        	        }
                    	        	        	        break;

                    	        	        	}

                    	        	        	// EsperEPL2Grammar.g:1308:45: (f2= FLOAT_SUFFIX )?
                    	        	        	int alt26 = 2;
                    	        	        	int LA26_0 = input.LA(1);

                    	        	        	if ( (LA26_0 == 'd' || LA26_0 == 'f') )
                    	        	        	{
                    	        	        	    alt26 = 1;
                    	        	        	}
                    	        	        	switch (alt26) 
                    	        	        	{
                    	        	        	    case 1 :
                    	        	        	        // EsperEPL2Grammar.g:1308:46: f2= FLOAT_SUFFIX
                    	        	        	        {
                    	        	        	        	int f2Start1975 = CharIndex;
                    	        	        	        	mFLOAT_SUFFIX(); if (state.failed) return ;
                    	        	        	        	f2 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, f2Start1975, CharIndex-1);
                    	        	        	        	if ( state.backtracking == 0 ) 
                    	        	        	        	{
                    	        	        	        	  t=f2;
                    	        	        	        	}

                    	        	        	        }
                    	        	        	        break;

                    	        	        	}


                    	        	        }
                    	        	        break;
                    	        	    case 2 :
                    	        	        // EsperEPL2Grammar.g:1309:17: EXPONENT (f3= FLOAT_SUFFIX )?
                    	        	        {
                    	        	        	mEXPONENT(); if (state.failed) return ;
                    	        	        	// EsperEPL2Grammar.g:1309:26: (f3= FLOAT_SUFFIX )?
                    	        	        	int alt27 = 2;
                    	        	        	int LA27_0 = input.LA(1);

                    	        	        	if ( (LA27_0 == 'd' || LA27_0 == 'f') )
                    	        	        	{
                    	        	        	    alt27 = 1;
                    	        	        	}
                    	        	        	switch (alt27) 
                    	        	        	{
                    	        	        	    case 1 :
                    	        	        	        // EsperEPL2Grammar.g:1309:27: f3= FLOAT_SUFFIX
                    	        	        	        {
                    	        	        	        	int f3Start2002 = CharIndex;
                    	        	        	        	mFLOAT_SUFFIX(); if (state.failed) return ;
                    	        	        	        	f3 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, f3Start2002, CharIndex-1);
                    	        	        	        	if ( state.backtracking == 0 ) 
                    	        	        	        	{
                    	        	        	        	  t=f3;
                    	        	        	        	}

                    	        	        	        }
                    	        	        	        break;

                    	        	        	}


                    	        	        }
                    	        	        break;
                    	        	    case 3 :
                    	        	        // EsperEPL2Grammar.g:1310:17: f4= FLOAT_SUFFIX
                    	        	        {
                    	        	        	int f4Start2026 = CharIndex;
                    	        	        	mFLOAT_SUFFIX(); if (state.failed) return ;
                    	        	        	f4 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, f4Start2026, CharIndex-1);
                    	        	        	if ( state.backtracking == 0 ) 
                    	        	        	{
                    	        	        	  t=f4;
                    	        	        	}

                    	        	        }
                    	        	        break;

                    	        	}

                    	        	if ( state.backtracking == 0 ) 
                    	        	{

                    	        	  			if (t != null && t.Text.ToUpper() .IndexOf('F') >= 0) {
                    	        	                  _type = NUM_FLOAT;
                    	        	  			}
                    	        	              else {
                    	        	  	           	_type = NUM_DOUBLE; // assume double
                    	        	  			}
                    	        	  			
                    	        	}

                    	        }
                    	        break;

                    	}


                    }
                    break;

            }
            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NUM_INT"

    // $ANTLR start "EXPONENT"
    public void mEXPONENT() // throws RecognitionException [2]
    {
    		try
    		{
            // EsperEPL2Grammar.g:1327:2: ( ( 'e' ) ( '+' | '-' )? ( '0' .. '9' )+ )
            // EsperEPL2Grammar.g:1327:4: ( 'e' ) ( '+' | '-' )? ( '0' .. '9' )+
            {
            	// EsperEPL2Grammar.g:1327:4: ( 'e' )
            	// EsperEPL2Grammar.g:1327:5: 'e'
            	{
            		Match('e'); if (state.failed) return ;

            	}

            	// EsperEPL2Grammar.g:1327:10: ( '+' | '-' )?
            	int alt31 = 2;
            	int LA31_0 = input.LA(1);

            	if ( (LA31_0 == '+' || LA31_0 == '-') )
            	{
            	    alt31 = 1;
            	}
            	switch (alt31) 
            	{
            	    case 1 :
            	        // EsperEPL2Grammar.g:
            	        {
            	        	if ( input.LA(1) == '+' || input.LA(1) == '-' ) 
            	        	{
            	        	    input.Consume();
            	        	state.failed = false;
            	        	}
            	        	else 
            	        	{
            	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	        	    Recover(mse);
            	        	    throw mse;}


            	        }
            	        break;

            	}

            	// EsperEPL2Grammar.g:1327:21: ( '0' .. '9' )+
            	int cnt32 = 0;
            	do 
            	{
            	    int alt32 = 2;
            	    int LA32_0 = input.LA(1);

            	    if ( ((LA32_0 >= '0' && LA32_0 <= '9')) )
            	    {
            	        alt32 = 1;
            	    }


            	    switch (alt32) 
            		{
            			case 1 :
            			    // EsperEPL2Grammar.g:1327:22: '0' .. '9'
            			    {
            			    	MatchRange('0','9'); if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    if ( cnt32 >= 1 ) goto loop32;
            			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            		            EarlyExitException eee =
            		                new EarlyExitException(32, input);
            		            throw eee;
            	    }
            	    cnt32++;
            	} while (true);

            	loop32:
            		;	// Stops C# compiler whinging that label 'loop32' has no statements


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "EXPONENT"

    // $ANTLR start "FLOAT_SUFFIX"
    public void mFLOAT_SUFFIX() // throws RecognitionException [2]
    {
    		try
    		{
            // EsperEPL2Grammar.g:1333:2: ( 'f' | 'd' )
            // EsperEPL2Grammar.g:
            {
            	if ( input.LA(1) == 'd' || input.LA(1) == 'f' ) 
            	{
            	    input.Consume();
            	state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "FLOAT_SUFFIX"

    override public void mTokens() // throws RecognitionException 
    {
        // EsperEPL2Grammar.g:1:8: ( CREATE | WINDOW | IN_SET | BETWEEN | LIKE | REGEXP | ESCAPE | OR_EXPR | AND_EXPR | NOT_EXPR | EVERY_EXPR | WHERE | AS | SUM | AVG | MAX | MIN | COALESCE | MEDIAN | STDDEV | AVEDEV | COUNT | SELECT | CASE | ELSE | WHEN | THEN | END | FROM | OUTER | JOIN | LEFT | RIGHT | FULL | ON | IS | BY | GROUP | HAVING | DISTINCT | ALL | OUTPUT | EVENTS | SECONDS | MINUTES | FIRST | LAST | INSERT | INTO | ORDER | ASC | DESC | RSTREAM | ISTREAM | IRSTREAM | UNIDIRECTIONAL | PATTERN | SQL | METADATASQL | PREVIOUS | PRIOR | EXISTS | WEEKDAY | LW | INSTANCEOF | CAST | CURRENT_TIMESTAMP | DELETE | SNAPSHOT | SET | VARIABLE | T__238 | T__239 | T__240 | T__241 | T__242 | T__243 | T__244 | T__245 | T__246 | T__247 | T__248 | T__249 | T__250 | FOLLOWED_BY | EQUALS | SQL_NE | QUESTION | LPAREN | RPAREN | LBRACK | RBRACK | LCURLY | RCURLY | COLON | COMMA | EQUAL | LNOT | BNOT | NOT_EQUAL | DIV | DIV_ASSIGN | PLUS | PLUS_ASSIGN | INC | MINUS | MINUS_ASSIGN | DEC | STAR | STAR_ASSIGN | MOD | MOD_ASSIGN | SR | SR_ASSIGN | BSR | BSR_ASSIGN | GE | GT | SL | SL_ASSIGN | LE | LT | BXOR | BXOR_ASSIGN | BOR | BOR_ASSIGN | LOR | BAND | BAND_ASSIGN | LAND | SEMI | DOT | NUM_LONG | NUM_DOUBLE | NUM_FLOAT | ESCAPECHAR | WS | SL_COMMENT | ML_COMMENT | STRING_LITERAL | QUOTED_STRING_LITERAL | IDENT | NUM_INT )
        int alt33 = 143;
        alt33 = dfa33.Predict(input);
        switch (alt33) 
        {
            case 1 :
                // EsperEPL2Grammar.g:1:10: CREATE
                {
                	mCREATE(); if (state.failed) return ;

                }
                break;
            case 2 :
                // EsperEPL2Grammar.g:1:17: WINDOW
                {
                	mWINDOW(); if (state.failed) return ;

                }
                break;
            case 3 :
                // EsperEPL2Grammar.g:1:24: IN_SET
                {
                	mIN_SET(); if (state.failed) return ;

                }
                break;
            case 4 :
                // EsperEPL2Grammar.g:1:31: BETWEEN
                {
                	mBETWEEN(); if (state.failed) return ;

                }
                break;
            case 5 :
                // EsperEPL2Grammar.g:1:39: LIKE
                {
                	mLIKE(); if (state.failed) return ;

                }
                break;
            case 6 :
                // EsperEPL2Grammar.g:1:44: REGEXP
                {
                	mREGEXP(); if (state.failed) return ;

                }
                break;
            case 7 :
                // EsperEPL2Grammar.g:1:51: ESCAPE
                {
                	mESCAPE(); if (state.failed) return ;

                }
                break;
            case 8 :
                // EsperEPL2Grammar.g:1:58: OR_EXPR
                {
                	mOR_EXPR(); if (state.failed) return ;

                }
                break;
            case 9 :
                // EsperEPL2Grammar.g:1:66: AND_EXPR
                {
                	mAND_EXPR(); if (state.failed) return ;

                }
                break;
            case 10 :
                // EsperEPL2Grammar.g:1:75: NOT_EXPR
                {
                	mNOT_EXPR(); if (state.failed) return ;

                }
                break;
            case 11 :
                // EsperEPL2Grammar.g:1:84: EVERY_EXPR
                {
                	mEVERY_EXPR(); if (state.failed) return ;

                }
                break;
            case 12 :
                // EsperEPL2Grammar.g:1:95: WHERE
                {
                	mWHERE(); if (state.failed) return ;

                }
                break;
            case 13 :
                // EsperEPL2Grammar.g:1:101: AS
                {
                	mAS(); if (state.failed) return ;

                }
                break;
            case 14 :
                // EsperEPL2Grammar.g:1:104: SUM
                {
                	mSUM(); if (state.failed) return ;

                }
                break;
            case 15 :
                // EsperEPL2Grammar.g:1:108: AVG
                {
                	mAVG(); if (state.failed) return ;

                }
                break;
            case 16 :
                // EsperEPL2Grammar.g:1:112: MAX
                {
                	mMAX(); if (state.failed) return ;

                }
                break;
            case 17 :
                // EsperEPL2Grammar.g:1:116: MIN
                {
                	mMIN(); if (state.failed) return ;

                }
                break;
            case 18 :
                // EsperEPL2Grammar.g:1:120: COALESCE
                {
                	mCOALESCE(); if (state.failed) return ;

                }
                break;
            case 19 :
                // EsperEPL2Grammar.g:1:129: MEDIAN
                {
                	mMEDIAN(); if (state.failed) return ;

                }
                break;
            case 20 :
                // EsperEPL2Grammar.g:1:136: STDDEV
                {
                	mSTDDEV(); if (state.failed) return ;

                }
                break;
            case 21 :
                // EsperEPL2Grammar.g:1:143: AVEDEV
                {
                	mAVEDEV(); if (state.failed) return ;

                }
                break;
            case 22 :
                // EsperEPL2Grammar.g:1:150: COUNT
                {
                	mCOUNT(); if (state.failed) return ;

                }
                break;
            case 23 :
                // EsperEPL2Grammar.g:1:156: SELECT
                {
                	mSELECT(); if (state.failed) return ;

                }
                break;
            case 24 :
                // EsperEPL2Grammar.g:1:163: CASE
                {
                	mCASE(); if (state.failed) return ;

                }
                break;
            case 25 :
                // EsperEPL2Grammar.g:1:168: ELSE
                {
                	mELSE(); if (state.failed) return ;

                }
                break;
            case 26 :
                // EsperEPL2Grammar.g:1:173: WHEN
                {
                	mWHEN(); if (state.failed) return ;

                }
                break;
            case 27 :
                // EsperEPL2Grammar.g:1:178: THEN
                {
                	mTHEN(); if (state.failed) return ;

                }
                break;
            case 28 :
                // EsperEPL2Grammar.g:1:183: END
                {
                	mEND(); if (state.failed) return ;

                }
                break;
            case 29 :
                // EsperEPL2Grammar.g:1:187: FROM
                {
                	mFROM(); if (state.failed) return ;

                }
                break;
            case 30 :
                // EsperEPL2Grammar.g:1:192: OUTER
                {
                	mOUTER(); if (state.failed) return ;

                }
                break;
            case 31 :
                // EsperEPL2Grammar.g:1:198: JOIN
                {
                	mJOIN(); if (state.failed) return ;

                }
                break;
            case 32 :
                // EsperEPL2Grammar.g:1:203: LEFT
                {
                	mLEFT(); if (state.failed) return ;

                }
                break;
            case 33 :
                // EsperEPL2Grammar.g:1:208: RIGHT
                {
                	mRIGHT(); if (state.failed) return ;

                }
                break;
            case 34 :
                // EsperEPL2Grammar.g:1:214: FULL
                {
                	mFULL(); if (state.failed) return ;

                }
                break;
            case 35 :
                // EsperEPL2Grammar.g:1:219: ON
                {
                	mON(); if (state.failed) return ;

                }
                break;
            case 36 :
                // EsperEPL2Grammar.g:1:222: IS
                {
                	mIS(); if (state.failed) return ;

                }
                break;
            case 37 :
                // EsperEPL2Grammar.g:1:225: BY
                {
                	mBY(); if (state.failed) return ;

                }
                break;
            case 38 :
                // EsperEPL2Grammar.g:1:228: GROUP
                {
                	mGROUP(); if (state.failed) return ;

                }
                break;
            case 39 :
                // EsperEPL2Grammar.g:1:234: HAVING
                {
                	mHAVING(); if (state.failed) return ;

                }
                break;
            case 40 :
                // EsperEPL2Grammar.g:1:241: DISTINCT
                {
                	mDISTINCT(); if (state.failed) return ;

                }
                break;
            case 41 :
                // EsperEPL2Grammar.g:1:250: ALL
                {
                	mALL(); if (state.failed) return ;

                }
                break;
            case 42 :
                // EsperEPL2Grammar.g:1:254: OUTPUT
                {
                	mOUTPUT(); if (state.failed) return ;

                }
                break;
            case 43 :
                // EsperEPL2Grammar.g:1:261: EVENTS
                {
                	mEVENTS(); if (state.failed) return ;

                }
                break;
            case 44 :
                // EsperEPL2Grammar.g:1:268: SECONDS
                {
                	mSECONDS(); if (state.failed) return ;

                }
                break;
            case 45 :
                // EsperEPL2Grammar.g:1:276: MINUTES
                {
                	mMINUTES(); if (state.failed) return ;

                }
                break;
            case 46 :
                // EsperEPL2Grammar.g:1:284: FIRST
                {
                	mFIRST(); if (state.failed) return ;

                }
                break;
            case 47 :
                // EsperEPL2Grammar.g:1:290: LAST
                {
                	mLAST(); if (state.failed) return ;

                }
                break;
            case 48 :
                // EsperEPL2Grammar.g:1:295: INSERT
                {
                	mINSERT(); if (state.failed) return ;

                }
                break;
            case 49 :
                // EsperEPL2Grammar.g:1:302: INTO
                {
                	mINTO(); if (state.failed) return ;

                }
                break;
            case 50 :
                // EsperEPL2Grammar.g:1:307: ORDER
                {
                	mORDER(); if (state.failed) return ;

                }
                break;
            case 51 :
                // EsperEPL2Grammar.g:1:313: ASC
                {
                	mASC(); if (state.failed) return ;

                }
                break;
            case 52 :
                // EsperEPL2Grammar.g:1:317: DESC
                {
                	mDESC(); if (state.failed) return ;

                }
                break;
            case 53 :
                // EsperEPL2Grammar.g:1:322: RSTREAM
                {
                	mRSTREAM(); if (state.failed) return ;

                }
                break;
            case 54 :
                // EsperEPL2Grammar.g:1:330: ISTREAM
                {
                	mISTREAM(); if (state.failed) return ;

                }
                break;
            case 55 :
                // EsperEPL2Grammar.g:1:338: IRSTREAM
                {
                	mIRSTREAM(); if (state.failed) return ;

                }
                break;
            case 56 :
                // EsperEPL2Grammar.g:1:347: UNIDIRECTIONAL
                {
                	mUNIDIRECTIONAL(); if (state.failed) return ;

                }
                break;
            case 57 :
                // EsperEPL2Grammar.g:1:362: PATTERN
                {
                	mPATTERN(); if (state.failed) return ;

                }
                break;
            case 58 :
                // EsperEPL2Grammar.g:1:370: SQL
                {
                	mSQL(); if (state.failed) return ;

                }
                break;
            case 59 :
                // EsperEPL2Grammar.g:1:374: METADATASQL
                {
                	mMETADATASQL(); if (state.failed) return ;

                }
                break;
            case 60 :
                // EsperEPL2Grammar.g:1:386: PREVIOUS
                {
                	mPREVIOUS(); if (state.failed) return ;

                }
                break;
            case 61 :
                // EsperEPL2Grammar.g:1:395: PRIOR
                {
                	mPRIOR(); if (state.failed) return ;

                }
                break;
            case 62 :
                // EsperEPL2Grammar.g:1:401: EXISTS
                {
                	mEXISTS(); if (state.failed) return ;

                }
                break;
            case 63 :
                // EsperEPL2Grammar.g:1:408: WEEKDAY
                {
                	mWEEKDAY(); if (state.failed) return ;

                }
                break;
            case 64 :
                // EsperEPL2Grammar.g:1:416: LW
                {
                	mLW(); if (state.failed) return ;

                }
                break;
            case 65 :
                // EsperEPL2Grammar.g:1:419: INSTANCEOF
                {
                	mINSTANCEOF(); if (state.failed) return ;

                }
                break;
            case 66 :
                // EsperEPL2Grammar.g:1:430: CAST
                {
                	mCAST(); if (state.failed) return ;

                }
                break;
            case 67 :
                // EsperEPL2Grammar.g:1:435: CURRENT_TIMESTAMP
                {
                	mCURRENT_TIMESTAMP(); if (state.failed) return ;

                }
                break;
            case 68 :
                // EsperEPL2Grammar.g:1:453: DELETE
                {
                	mDELETE(); if (state.failed) return ;

                }
                break;
            case 69 :
                // EsperEPL2Grammar.g:1:460: SNAPSHOT
                {
                	mSNAPSHOT(); if (state.failed) return ;

                }
                break;
            case 70 :
                // EsperEPL2Grammar.g:1:469: SET
                {
                	mSET(); if (state.failed) return ;

                }
                break;
            case 71 :
                // EsperEPL2Grammar.g:1:473: VARIABLE
                {
                	mVARIABLE(); if (state.failed) return ;

                }
                break;
            case 72 :
                // EsperEPL2Grammar.g:1:482: T__238
                {
                	mT__238(); if (state.failed) return ;

                }
                break;
            case 73 :
                // EsperEPL2Grammar.g:1:489: T__239
                {
                	mT__239(); if (state.failed) return ;

                }
                break;
            case 74 :
                // EsperEPL2Grammar.g:1:496: T__240
                {
                	mT__240(); if (state.failed) return ;

                }
                break;
            case 75 :
                // EsperEPL2Grammar.g:1:503: T__241
                {
                	mT__241(); if (state.failed) return ;

                }
                break;
            case 76 :
                // EsperEPL2Grammar.g:1:510: T__242
                {
                	mT__242(); if (state.failed) return ;

                }
                break;
            case 77 :
                // EsperEPL2Grammar.g:1:517: T__243
                {
                	mT__243(); if (state.failed) return ;

                }
                break;
            case 78 :
                // EsperEPL2Grammar.g:1:524: T__244
                {
                	mT__244(); if (state.failed) return ;

                }
                break;
            case 79 :
                // EsperEPL2Grammar.g:1:531: T__245
                {
                	mT__245(); if (state.failed) return ;

                }
                break;
            case 80 :
                // EsperEPL2Grammar.g:1:538: T__246
                {
                	mT__246(); if (state.failed) return ;

                }
                break;
            case 81 :
                // EsperEPL2Grammar.g:1:545: T__247
                {
                	mT__247(); if (state.failed) return ;

                }
                break;
            case 82 :
                // EsperEPL2Grammar.g:1:552: T__248
                {
                	mT__248(); if (state.failed) return ;

                }
                break;
            case 83 :
                // EsperEPL2Grammar.g:1:559: T__249
                {
                	mT__249(); if (state.failed) return ;

                }
                break;
            case 84 :
                // EsperEPL2Grammar.g:1:566: T__250
                {
                	mT__250(); if (state.failed) return ;

                }
                break;
            case 85 :
                // EsperEPL2Grammar.g:1:573: FOLLOWED_BY
                {
                	mFOLLOWED_BY(); if (state.failed) return ;

                }
                break;
            case 86 :
                // EsperEPL2Grammar.g:1:585: EQUALS
                {
                	mEQUALS(); if (state.failed) return ;

                }
                break;
            case 87 :
                // EsperEPL2Grammar.g:1:592: SQL_NE
                {
                	mSQL_NE(); if (state.failed) return ;

                }
                break;
            case 88 :
                // EsperEPL2Grammar.g:1:599: QUESTION
                {
                	mQUESTION(); if (state.failed) return ;

                }
                break;
            case 89 :
                // EsperEPL2Grammar.g:1:608: LPAREN
                {
                	mLPAREN(); if (state.failed) return ;

                }
                break;
            case 90 :
                // EsperEPL2Grammar.g:1:615: RPAREN
                {
                	mRPAREN(); if (state.failed) return ;

                }
                break;
            case 91 :
                // EsperEPL2Grammar.g:1:622: LBRACK
                {
                	mLBRACK(); if (state.failed) return ;

                }
                break;
            case 92 :
                // EsperEPL2Grammar.g:1:629: RBRACK
                {
                	mRBRACK(); if (state.failed) return ;

                }
                break;
            case 93 :
                // EsperEPL2Grammar.g:1:636: LCURLY
                {
                	mLCURLY(); if (state.failed) return ;

                }
                break;
            case 94 :
                // EsperEPL2Grammar.g:1:643: RCURLY
                {
                	mRCURLY(); if (state.failed) return ;

                }
                break;
            case 95 :
                // EsperEPL2Grammar.g:1:650: COLON
                {
                	mCOLON(); if (state.failed) return ;

                }
                break;
            case 96 :
                // EsperEPL2Grammar.g:1:656: COMMA
                {
                	mCOMMA(); if (state.failed) return ;

                }
                break;
            case 97 :
                // EsperEPL2Grammar.g:1:662: EQUAL
                {
                	mEQUAL(); if (state.failed) return ;

                }
                break;
            case 98 :
                // EsperEPL2Grammar.g:1:668: LNOT
                {
                	mLNOT(); if (state.failed) return ;

                }
                break;
            case 99 :
                // EsperEPL2Grammar.g:1:673: BNOT
                {
                	mBNOT(); if (state.failed) return ;

                }
                break;
            case 100 :
                // EsperEPL2Grammar.g:1:678: NOT_EQUAL
                {
                	mNOT_EQUAL(); if (state.failed) return ;

                }
                break;
            case 101 :
                // EsperEPL2Grammar.g:1:688: DIV
                {
                	mDIV(); if (state.failed) return ;

                }
                break;
            case 102 :
                // EsperEPL2Grammar.g:1:692: DIV_ASSIGN
                {
                	mDIV_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 103 :
                // EsperEPL2Grammar.g:1:703: PLUS
                {
                	mPLUS(); if (state.failed) return ;

                }
                break;
            case 104 :
                // EsperEPL2Grammar.g:1:708: PLUS_ASSIGN
                {
                	mPLUS_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 105 :
                // EsperEPL2Grammar.g:1:720: INC
                {
                	mINC(); if (state.failed) return ;

                }
                break;
            case 106 :
                // EsperEPL2Grammar.g:1:724: MINUS
                {
                	mMINUS(); if (state.failed) return ;

                }
                break;
            case 107 :
                // EsperEPL2Grammar.g:1:730: MINUS_ASSIGN
                {
                	mMINUS_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 108 :
                // EsperEPL2Grammar.g:1:743: DEC
                {
                	mDEC(); if (state.failed) return ;

                }
                break;
            case 109 :
                // EsperEPL2Grammar.g:1:747: STAR
                {
                	mSTAR(); if (state.failed) return ;

                }
                break;
            case 110 :
                // EsperEPL2Grammar.g:1:752: STAR_ASSIGN
                {
                	mSTAR_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 111 :
                // EsperEPL2Grammar.g:1:764: MOD
                {
                	mMOD(); if (state.failed) return ;

                }
                break;
            case 112 :
                // EsperEPL2Grammar.g:1:768: MOD_ASSIGN
                {
                	mMOD_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 113 :
                // EsperEPL2Grammar.g:1:779: SR
                {
                	mSR(); if (state.failed) return ;

                }
                break;
            case 114 :
                // EsperEPL2Grammar.g:1:782: SR_ASSIGN
                {
                	mSR_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 115 :
                // EsperEPL2Grammar.g:1:792: BSR
                {
                	mBSR(); if (state.failed) return ;

                }
                break;
            case 116 :
                // EsperEPL2Grammar.g:1:796: BSR_ASSIGN
                {
                	mBSR_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 117 :
                // EsperEPL2Grammar.g:1:807: GE
                {
                	mGE(); if (state.failed) return ;

                }
                break;
            case 118 :
                // EsperEPL2Grammar.g:1:810: GT
                {
                	mGT(); if (state.failed) return ;

                }
                break;
            case 119 :
                // EsperEPL2Grammar.g:1:813: SL
                {
                	mSL(); if (state.failed) return ;

                }
                break;
            case 120 :
                // EsperEPL2Grammar.g:1:816: SL_ASSIGN
                {
                	mSL_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 121 :
                // EsperEPL2Grammar.g:1:826: LE
                {
                	mLE(); if (state.failed) return ;

                }
                break;
            case 122 :
                // EsperEPL2Grammar.g:1:829: LT
                {
                	mLT(); if (state.failed) return ;

                }
                break;
            case 123 :
                // EsperEPL2Grammar.g:1:832: BXOR
                {
                	mBXOR(); if (state.failed) return ;

                }
                break;
            case 124 :
                // EsperEPL2Grammar.g:1:837: BXOR_ASSIGN
                {
                	mBXOR_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 125 :
                // EsperEPL2Grammar.g:1:849: BOR
                {
                	mBOR(); if (state.failed) return ;

                }
                break;
            case 126 :
                // EsperEPL2Grammar.g:1:853: BOR_ASSIGN
                {
                	mBOR_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 127 :
                // EsperEPL2Grammar.g:1:864: LOR
                {
                	mLOR(); if (state.failed) return ;

                }
                break;
            case 128 :
                // EsperEPL2Grammar.g:1:868: BAND
                {
                	mBAND(); if (state.failed) return ;

                }
                break;
            case 129 :
                // EsperEPL2Grammar.g:1:873: BAND_ASSIGN
                {
                	mBAND_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 130 :
                // EsperEPL2Grammar.g:1:885: LAND
                {
                	mLAND(); if (state.failed) return ;

                }
                break;
            case 131 :
                // EsperEPL2Grammar.g:1:890: SEMI
                {
                	mSEMI(); if (state.failed) return ;

                }
                break;
            case 132 :
                // EsperEPL2Grammar.g:1:895: DOT
                {
                	mDOT(); if (state.failed) return ;

                }
                break;
            case 133 :
                // EsperEPL2Grammar.g:1:899: NUM_LONG
                {
                	mNUM_LONG(); if (state.failed) return ;

                }
                break;
            case 134 :
                // EsperEPL2Grammar.g:1:908: NUM_DOUBLE
                {
                	mNUM_DOUBLE(); if (state.failed) return ;

                }
                break;
            case 135 :
                // EsperEPL2Grammar.g:1:919: NUM_FLOAT
                {
                	mNUM_FLOAT(); if (state.failed) return ;

                }
                break;
            case 136 :
                // EsperEPL2Grammar.g:1:929: ESCAPECHAR
                {
                	mESCAPECHAR(); if (state.failed) return ;

                }
                break;
            case 137 :
                // EsperEPL2Grammar.g:1:940: WS
                {
                	mWS(); if (state.failed) return ;

                }
                break;
            case 138 :
                // EsperEPL2Grammar.g:1:943: SL_COMMENT
                {
                	mSL_COMMENT(); if (state.failed) return ;

                }
                break;
            case 139 :
                // EsperEPL2Grammar.g:1:954: ML_COMMENT
                {
                	mML_COMMENT(); if (state.failed) return ;

                }
                break;
            case 140 :
                // EsperEPL2Grammar.g:1:965: STRING_LITERAL
                {
                	mSTRING_LITERAL(); if (state.failed) return ;

                }
                break;
            case 141 :
                // EsperEPL2Grammar.g:1:980: QUOTED_STRING_LITERAL
                {
                	mQUOTED_STRING_LITERAL(); if (state.failed) return ;

                }
                break;
            case 142 :
                // EsperEPL2Grammar.g:1:1002: IDENT
                {
                	mIDENT(); if (state.failed) return ;

                }
                break;
            case 143 :
                // EsperEPL2Grammar.g:1:1008: NUM_INT
                {
                	mNUM_INT(); if (state.failed) return ;

                }
                break;

        }

    }

    // $ANTLR start "synpred1_EsperEPL2Grammar"
    public void synpred1_EsperEPL2Grammar_fragment() //throws RecognitionException
    {   
        // EsperEPL2Grammar.g:1298:5: ( ( '0' .. '9' )+ ( '.' | EXPONENT | FLOAT_SUFFIX ) )
        // EsperEPL2Grammar.g:1298:6: ( '0' .. '9' )+ ( '.' | EXPONENT | FLOAT_SUFFIX )
        {
        	// EsperEPL2Grammar.g:1298:6: ( '0' .. '9' )+
        	int cnt34 = 0;
        	do 
        	{
        	    int alt34 = 2;
        	    int LA34_0 = input.LA(1);

        	    if ( ((LA34_0 >= '0' && LA34_0 <= '9')) )
        	    {
        	        alt34 = 1;
        	    }


        	    switch (alt34) 
        		{
        			case 1 :
        			    // EsperEPL2Grammar.g:1298:7: '0' .. '9'
        			    {
        			    	MatchRange('0','9'); if (state.failed) return ;

        			    }
        			    break;

        			default:
        			    if ( cnt34 >= 1 ) goto loop34;
        			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
        		            EarlyExitException eee =
        		                new EarlyExitException(34, input);
        		            throw eee;
        	    }
        	    cnt34++;
        	} while (true);

        	loop34:
        		;	// Stops C# compiler whinging that label 'loop34' has no statements

        	// EsperEPL2Grammar.g:1298:18: ( '.' | EXPONENT | FLOAT_SUFFIX )
        	int alt35 = 3;
        	switch ( input.LA(1) ) 
        	{
        	case '.':
        		{
        	    alt35 = 1;
        	    }
        	    break;
        	case 'e':
        		{
        	    alt35 = 2;
        	    }
        	    break;
        	case 'd':
        	case 'f':
        		{
        	    alt35 = 3;
        	    }
        	    break;
        		default:
        		    if ( state.backtracking > 0 ) {state.failed = true; return ;}
        		    NoViableAltException nvae_d35s0 =
        		        new NoViableAltException("", 35, 0, input);

        		    throw nvae_d35s0;
        	}

        	switch (alt35) 
        	{
        	    case 1 :
        	        // EsperEPL2Grammar.g:1298:19: '.'
        	        {
        	        	Match('.'); if (state.failed) return ;

        	        }
        	        break;
        	    case 2 :
        	        // EsperEPL2Grammar.g:1298:23: EXPONENT
        	        {
        	        	mEXPONENT(); if (state.failed) return ;

        	        }
        	        break;
        	    case 3 :
        	        // EsperEPL2Grammar.g:1298:32: FLOAT_SUFFIX
        	        {
        	        	mFLOAT_SUFFIX(); if (state.failed) return ;

        	        }
        	        break;

        	}


        }
    }
    // $ANTLR end "synpred1_EsperEPL2Grammar"

   	public bool synpred1_EsperEPL2Grammar() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred1_EsperEPL2Grammar_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}


    protected DFA33 dfa33;
	private void InitializeCyclicDFAs()
	{
	    this.dfa33 = new DFA33(this);
	}

    const string DFA33_eotS =
        "\x01\uffff\x15\x35\x01\x74\x01\x76\x01\x7a\x09\uffff\x01\x7c\x01"+
        "\uffff\x01\u0080\x01\u0083\x01\u0085\x01\u0087\x01\u008a\x01\u008c"+
        "\x01\u008f\x01\u0092\x01\uffff\x01\u0093\x09\uffff\x07\x35\x01\u009e"+
        "\x01\u00a0\x02\x35\x01\u00a3\x0b\x35\x01\u00b0\x01\x35\x01\u00b2"+
        "\x01\x35\x01\u00b5\x1e\x35\x07\uffff\x01\u00dc\x0f\uffff\x01\u00df"+
        "\x0b\uffff\x0a\x35\x01\uffff\x01\x35\x01\uffff\x02\x35\x01\uffff"+
        "\x09\x35\x01\u00fa\x02\x35\x01\uffff\x01\x35\x01\uffff\x01\u00ff"+
        "\x01\u0100\x01\uffff\x01\u0101\x01\x35\x01\u0103\x01\u0104\x01\x35"+
        "\x01\u0106\x02\x35\x01\u010a\x01\u010b\x01\u010c\x01\x35\x01\u010e"+
        "\x01\u0110\x11\x35\x01\u0123\x05\x35\x03\uffff\x01\u012a\x01\uffff"+
        "\x03\x35\x01\u012e\x01\u012f\x03\x35\x01\u0133\x03\x35\x01\u0137"+
        "\x03\x35\x01\u013b\x01\u013c\x01\u013e\x06\x35\x01\u0145\x01\uffff"+
        "\x04\x35\x03\uffff\x01\x35\x02\uffff\x01\u014b\x01\uffff\x03\x35"+
        "\x03\uffff\x01\x35\x01\uffff\x01\x35\x01\uffff\x03\x35\x01\u0154"+
        "\x01\u0155\x01\u0156\x01\u0157\x01\u0158\x02\x35\x01\u015b\x02\x35"+
        "\x01\u015f\x01\x35\x01\u0161\x01\x35\x01\u0163\x01\uffff\x02\x35"+
        "\x01\u0166\x02\x35\x02\uffff\x02\x35\x01\u016b\x02\uffff\x02\x35"+
        "\x01\u016e\x01\uffff\x03\x35\x01\uffff\x03\x35\x02\uffff\x01\x35"+
        "\x01\uffff\x01\x35\x01\u0177\x02\x35\x01\u017a\x01\x35\x01\uffff"+
        "\x01\x35\x01\u017d\x01\u017e\x02\x35\x01\uffff\x08\x35\x05\uffff"+
        "\x01\u0189\x01\u018a\x01\uffff\x01\u018b\x01\x35\x01\u018d\x01\uffff"+
        "\x01\x35\x01\uffff\x01\x35\x01\uffff\x02\x35\x01\uffff\x01\u0192"+
        "\x01\x35\x01\u0194\x01\x35\x01\uffff\x01\x35\x01\u0197\x01\uffff"+
        "\x01\x35\x01\u0199\x05\x35\x01\u019f\x01\uffff\x01\x35\x01\u01a1"+
        "\x01\uffff\x01\u01a2\x01\u01a3\x02\uffff\x01\u01a4\x01\u01a5\x01"+
        "\u01a6\x01\u01a7\x01\u01a9\x01\x35\x01\u01ac\x01\x35\x01\u01ae\x01"+
        "\x35\x03\uffff\x01\u01b0\x01\uffff\x01\x35\x01\u01b2\x02\x35\x01"+
        "\uffff\x01\x35\x01\uffff\x02\x35\x01\uffff\x01\u01b8\x01\uffff\x01"+
        "\x35\x01\u01ba\x01\x35\x01\u01bc\x01\x35\x01\uffff\x01\u01be\x07"+
        "\uffff\x01\u01bf\x01\uffff\x01\x35\x01\u01c1\x01\uffff\x01\x35\x01"+
        "\uffff\x01\x35\x01\uffff\x01\x35\x01\uffff\x01\x35\x01\u01c6\x01"+
        "\x35\x01\u01c8\x01\x35\x01\uffff\x01\x35\x01\uffff\x01\u01cb\x01"+
        "\uffff\x01\x35\x02\uffff\x01\u01cd\x01\uffff\x02\x35\x01\u01d0\x01"+
        "\x35\x01\uffff\x01\u01d2\x01\uffff\x02\x35\x01\uffff\x01\x35\x01"+
        "\uffff\x02\x35\x01\uffff\x01\x35\x01\uffff\x01\x35\x01\u01da\x05"+
        "\x35\x01\uffff\x01\u01e0\x01\u01e2\x01\u01e3\x02\x35\x01\uffff\x01"+
        "\u01e6\x02\uffff\x02\x35\x01\uffff\x02\x35\x01\u01eb\x01\x35\x01"+
        "\uffff\x01\x35\x01\u01ee\x01\uffff";
    const string DFA33_eofS =
        "\u01ef\uffff";
    const string DFA33_minS =
        "\x01\x09\x01\x61\x01\x65\x01\x6e\x01\x65\x01\x61\x01\x65\x01\x6c"+
        "\x01\x6e\x01\x6c\x01\x6f\x01\x65\x01\x61\x01\x68\x01\x61\x01\x6f"+
        "\x01\x72\x02\x61\x01\x6e\x02\x61\x01\x2d\x01\x3d\x01\x3c\x09\uffff"+
        "\x01\x3d\x01\uffff\x01\x2a\x01\x2b\x05\x3d\x01\x26\x01\uffff\x01"+
        "\x30\x09\uffff\x01\x65\x01\x61\x01\x73\x01\x72\x01\x6e\x02\x65\x02"+
        "\x24\x01\x73\x01\x74\x01\x24\x01\x6b\x01\x66\x01\x73\x02\x67\x01"+
        "\x74\x01\x63\x01\x65\x01\x73\x01\x64\x01\x69\x01\x24\x01\x74\x01"+
        "\x24\x01\x64\x01\x24\x01\x65\x01\x6c\x01\x74\x01\x6c\x01\x6d\x01"+
        "\x64\x01\x63\x01\x6c\x01\x61\x01\x78\x01\x6c\x01\x64\x02\x65\x01"+
        "\x75\x01\x6f\x01\x6c\x01\x72\x01\x6c\x01\x69\x01\x6f\x01\x76\x01"+
        "\x75\x01\x73\x01\x6c\x01\x79\x01\x69\x01\x74\x01\x65\x01\x72\x07"+
        "\uffff\x01\x3d\x0f\uffff\x01\x3d\x0b\uffff\x01\x61\x01\x6c\x01\x6e"+
        "\x01\x65\x01\x72\x01\x64\x01\x6e\x01\x6b\x01\x65\x01\x6f\x01\uffff"+
        "\x01\x72\x01\uffff\x01\x74\x01\x77\x01\uffff\x01\x65\x02\x74\x01"+
        "\x65\x01\x68\x01\x72\x01\x61\x01\x6e\x01\x65\x01\x24\x01\x73\x01"+
        "\x65\x01\uffff\x01\x65\x01\uffff\x02\x24\x01\uffff\x01\x24\x01\x64"+
        "\x02\x24\x01\x6c\x01\x24\x01\x64\x01\x65\x03\x24\x01\x70\x02\x24"+
        "\x01\x6c\x01\x69\x01\x61\x01\x63\x01\x6e\x01\x65\x01\x6d\x01\x6c"+
        "\x02\x73\x01\x6e\x01\x75\x01\x69\x01\x72\x01\x74\x01\x63\x01\x65"+
        "\x01\x24\x01\x64\x01\x74\x01\x76\x01\x6f\x01\x69\x03\uffff\x01\x3d"+
        "\x01\uffff\x01\x74\x01\x65\x01\x74\x02\x24\x01\x65\x01\x6f\x01\x65"+
        "\x01\x24\x01\x64\x01\x72\x01\x61\x01\x24\x01\x65\x01\x72\x01\x65"+
        "\x03\x24\x01\x78\x01\x74\x01\x65\x01\x70\x01\x79\x01\x74\x01\x24"+
        "\x01\uffff\x01\x74\x02\x72\x01\x75\x03\uffff\x01\x65\x02\uffff\x01"+
        "\x24\x01\uffff\x01\x65\x01\x63\x01\x6e\x03\uffff\x01\x73\x01\uffff"+
        "\x01\x74\x01\uffff\x01\x69\x01\x61\x01\x64\x05\x24\x01\x74\x01\x65"+
        "\x01\x24\x01\x70\x01\x6e\x01\x24\x01\x69\x01\x24\x01\x74\x01\x24"+
        "\x01\uffff\x01\x69\x01\x65\x01\x24\x01\x72\x01\x61\x02\uffff\x01"+
        "\x65\x01\x73\x01\x24\x02\uffff\x01\x6e\x01\x77\x01\x24\x01\uffff"+
        "\x01\x61\x01\x74\x01\x6e\x01\uffff\x01\x61\x02\x65\x02\uffff\x01"+
        "\x65\x01\uffff\x01\x70\x01\x24\x01\x61\x01\x65\x01\x24\x01\x73\x01"+
        "\uffff\x01\x73\x02\x24\x01\x74\x01\x76\x01\uffff\x01\x76\x01\x74"+
        "\x01\x64\x01\x68\x01\x65\x01\x73\x01\x6e\x01\x61\x05\uffff\x02\x24"+
        "\x01\uffff\x01\x24\x01\x67\x01\x24\x01\uffff\x01\x6e\x01\uffff\x01"+
        "\x65\x01\uffff\x02\x72\x01\uffff\x01\x24\x01\x62\x01\x24\x01\x63"+
        "\x01\uffff\x01\x74\x01\x24\x01\uffff\x01\x79\x01\x24\x01\x63\x01"+
        "\x6d\x01\x61\x01\x6e\x01\x65\x01\x24\x01\uffff\x01\x6d\x01\x24\x01"+
        "\uffff\x02\x24\x02\uffff\x05\x24\x01\x6f\x01\x24\x01\x65\x01\x24"+
        "\x01\x74\x03\uffff\x01\x24\x01\uffff\x01\x63\x01\x24\x01\x65\x01"+
        "\x6e\x01\uffff\x01\x6c\x01\uffff\x01\x65\x01\x5f\x01\uffff\x01\x24"+
        "\x01\uffff\x01\x65\x01\x24\x01\x6d\x01\x24\x01\x6b\x01\uffff\x01"+
        "\x24\x07\uffff\x01\x24\x01\uffff\x01\x74\x01\x24\x01\uffff\x01\x63"+
        "\x01\uffff\x01\x61\x01\uffff\x01\x74\x01\uffff\x01\x63\x01\x24\x01"+
        "\x65\x01\x24\x01\x74\x01\uffff\x01\x6f\x01\uffff\x01\x24\x01\uffff"+
        "\x01\x64\x02\uffff\x01\x24\x01\uffff\x01\x6f\x01\x73\x01\x24\x01"+
        "\x74\x01\uffff\x01\x24\x01\uffff\x01\x69\x01\x66\x01\uffff\x01\x61"+
        "\x01\uffff\x01\x6e\x01\x71\x01\uffff\x01\x69\x01\uffff\x01\x6d\x01"+
        "\x24\x01\x79\x01\x64\x01\x6c\x01\x6f\x01\x65\x01\uffff\x03\x24\x01"+
        "\x6e\x01\x73\x01\uffff\x01\x24\x02\uffff\x01\x61\x01\x74\x01\uffff"+
        "\x01\x6c\x01\x61\x01\x24\x01\x6d\x01\uffff\x01\x70\x01\x24\x01\uffff";
    const string DFA33_maxS =
        "\x01\u18ff\x01\x75\x01\x69\x01\x73\x01\x79\x01\x69\x01\x73\x01"+
        "\x78\x01\x75\x01\x76\x02\x75\x01\x73\x01\x72\x01\x75\x01\x6f\x01"+
        "\x72\x01\x6f\x01\x69\x01\x6e\x01\x72\x01\x61\x01\x3e\x01\x3d\x01"+
        "\x3e\x09\uffff\x01\x3d\x01\uffff\x04\x3d\x01\x3e\x01\x3d\x01\x7c"+
        "\x01\x3d\x01\uffff\x01\x39\x09\uffff\x01\x65\x01\x75\x01\x73\x01"+
        "\x72\x01\x6e\x02\x65\x02\x7a\x01\x73\x01\x74\x01\x7a\x01\x6b\x01"+
        "\x66\x01\x73\x02\x67\x01\x74\x01\x63\x01\x65\x01\x73\x01\x64\x01"+
        "\x69\x01\x7a\x01\x74\x01\x7a\x01\x64\x01\x7a\x01\x67\x01\x6c\x01"+
        "\x74\x01\x6c\x01\x6d\x01\x64\x01\x74\x01\x6c\x01\x61\x01\x78\x01"+
        "\x6e\x01\x74\x02\x65\x01\x75\x01\x6f\x01\x6c\x01\x72\x01\x6c\x01"+
        "\x69\x01\x6f\x01\x76\x01\x75\x02\x73\x01\x79\x01\x69\x01\x74\x01"+
        "\x69\x01\x72\x07\uffff\x01\x3d\x0f\uffff\x01\x3e\x0b\uffff\x01\x61"+
        "\x01\x6c\x01\x6e\x01\x74\x01\x72\x01\x64\x01\x72\x01\x6b\x01\x74"+
        "\x01\x6f\x01\uffff\x01\x72\x01\uffff\x01\x74\x01\x77\x01\uffff\x01"+
        "\x65\x02\x74\x01\x65\x01\x68\x01\x72\x01\x61\x01\x72\x01\x65\x01"+
        "\x7a\x01\x73\x01\x65\x01\uffff\x01\x70\x01\uffff\x02\x7a\x01\uffff"+
        "\x01\x7a\x01\x64\x02\x7a\x01\x6c\x01\x7a\x01\x64\x01\x65\x03\x7a"+
        "\x01\x70\x02\x7a\x01\x6c\x01\x69\x01\x61\x01\x63\x01\x6e\x01\x65"+
        "\x01\x6d\x01\x6c\x02\x73\x01\x6e\x01\x75\x01\x69\x01\x72\x01\x74"+
        "\x01\x63\x01\x65\x01\x7a\x01\x64\x01\x74\x01\x76\x01\x6f\x01\x69"+
        "\x03\uffff\x01\x3d\x01\uffff\x01\x74\x01\x65\x01\x74\x02\x7a\x01"+
        "\x65\x01\x6f\x01\x65\x01\x7a\x01\x64\x01\x72\x01\x61\x01\x7a\x01"+
        "\x65\x01\x72\x01\x65\x03\x7a\x01\x78\x01\x74\x01\x65\x01\x70\x01"+
        "\x79\x01\x74\x01\x7a\x01\uffff\x01\x74\x02\x72\x01\x75\x03\uffff"+
        "\x01\x65\x02\uffff\x01\x7a\x01\uffff\x01\x65\x01\x63\x01\x6e\x03"+
        "\uffff\x01\x73\x01\uffff\x01\x74\x01\uffff\x01\x69\x01\x61\x01\x64"+
        "\x05\x7a\x01\x74\x01\x65\x01\x7a\x01\x70\x01\x6e\x01\x7a\x01\x69"+
        "\x01\x7a\x01\x74\x01\x7a\x01\uffff\x01\x69\x01\x65\x01\x7a\x01\x72"+
        "\x01\x61\x02\uffff\x01\x65\x01\x73\x01\x7a\x02\uffff\x01\x6e\x01"+
        "\x77\x01\x7a\x01\uffff\x01\x61\x01\x74\x01\x6e\x01\uffff\x01\x61"+
        "\x02\x65\x02\uffff\x01\x65\x01\uffff\x01\x70\x01\x7a\x01\x61\x01"+
        "\x65\x01\x7a\x01\x73\x01\uffff\x01\x73\x02\x7a\x01\x74\x01\x76\x01"+
        "\uffff\x01\x76\x01\x74\x01\x64\x01\x68\x01\x65\x01\x73\x01\x6e\x01"+
        "\x61\x05\uffff\x02\x7a\x01\uffff\x01\x7a\x01\x67\x01\x7a\x01\uffff"+
        "\x01\x6e\x01\uffff\x01\x65\x01\uffff\x02\x72\x01\uffff\x01\x7a\x01"+
        "\x62\x01\x7a\x01\x63\x01\uffff\x01\x74\x01\x7a\x01\uffff\x01\x79"+
        "\x01\x7a\x01\x63\x01\x6d\x01\x61\x01\x6e\x01\x65\x01\x7a\x01\uffff"+
        "\x01\x6d\x01\x7a\x01\uffff\x02\x7a\x02\uffff\x05\x7a\x01\x6f\x01"+
        "\x7a\x01\x65\x01\x7a\x01\x74\x03\uffff\x01\x7a\x01\uffff\x01\x63"+
        "\x01\x7a\x01\x65\x01\x6e\x01\uffff\x01\x6c\x01\uffff\x01\x65\x01"+
        "\x5f\x01\uffff\x01\x7a\x01\uffff\x01\x65\x01\x7a\x01\x6d\x01\x7a"+
        "\x01\x6b\x01\uffff\x01\x7a\x07\uffff\x01\x7a\x01\uffff\x01\x74\x01"+
        "\x7a\x01\uffff\x01\x63\x01\uffff\x01\x61\x01\uffff\x01\x74\x01\uffff"+
        "\x01\x63\x01\x7a\x01\x65\x01\x7a\x01\x74\x01\uffff\x01\x6f\x01\uffff"+
        "\x01\x7a\x01\uffff\x01\x64\x02\uffff\x01\x7a\x01\uffff\x01\x6f\x01"+
        "\x73\x01\x7a\x01\x74\x01\uffff\x01\x7a\x01\uffff\x01\x69\x01\x66"+
        "\x01\uffff\x01\x61\x01\uffff\x01\x6e\x01\x71\x01\uffff\x01\x69\x01"+
        "\uffff\x01\x6d\x01\x7a\x01\x79\x01\x64\x01\x6c\x01\x6f\x01\x65\x01"+
        "\uffff\x03\x7a\x01\x6e\x01\x73\x01\uffff\x01\x7a\x02\uffff\x01\x61"+
        "\x01\x74\x01\uffff\x01\x6c\x01\x61\x01\x7a\x01\x6d\x01\uffff\x01"+
        "\x70\x01\x7a\x01\uffff";
    const string DFA33_acceptS =
        "\x19\uffff\x01\x58\x01\x59\x01\x5a\x01\x5b\x01\x5c\x01\x5d\x01"+
        "\x5e\x01\x5f\x01\x60\x01\uffff\x01\x63\x08\uffff\x01\u0083\x01\uffff"+
        "\x01\u0085\x01\u0086\x01\u0087\x01\u0088\x01\u0089\x01\u008c\x01"+
        "\u008d\x01\u008e\x01\u008f\x3a\uffff\x01\x55\x01\x6b\x01\x6c\x01"+
        "\x6a\x01\x61\x01\x56\x01\x57\x01\uffff\x01\x79\x01\x7a\x01\x64\x01"+
        "\x62\x01\x66\x01\u008a\x01\u008b\x01\x65\x01\x68\x01\x69\x01\x67"+
        "\x01\x6e\x01\x6d\x01\x70\x01\x6f\x01\uffff\x01\x75\x01\x76\x01\x7c"+
        "\x01\x7b\x01\x7e\x01\x7f\x01\x7d\x01\u0081\x01\u0082\x01\u0080\x01"+
        "\u0084\x0a\uffff\x01\x03\x01\uffff\x01\x24\x02\uffff\x01\x25\x0c"+
        "\uffff\x01\x08\x01\uffff\x01\x23\x02\uffff\x01\x0d\x25\uffff\x01"+
        "\x78\x01\x77\x01\x72\x01\uffff\x01\x71\x1a\uffff\x01\x1c\x04\uffff"+
        "\x01\x09\x01\x33\x01\x0f\x01\uffff\x01\x29\x01\x0a\x01\uffff\x01"+
        "\x0e\x03\uffff\x01\x51\x01\x46\x01\x3a\x01\uffff\x01\x10\x01\uffff"+
        "\x01\x11\x12\uffff\x01\x4c\x05\uffff\x01\x74\x01\x73\x03\uffff\x01"+
        "\x18\x01\x42\x03\uffff\x01\x1a\x03\uffff\x01\x31\x03\uffff\x01\x05"+
        "\x01\x20\x01\uffff\x01\x2f\x06\uffff\x01\x19\x05\uffff\x01\x4a\x08"+
        "\uffff\x01\x54\x01\x1b\x01\x48\x01\x1d\x01\x22\x02\uffff\x01\x1f"+
        "\x03\uffff\x01\x4e\x01\uffff\x01\x34\x01\uffff\x01\x4b\x02\uffff"+
        "\x01\x3c\x04\uffff\x01\x16\x02\uffff\x01\x0c\x08\uffff\x01\x21\x02"+
        "\uffff\x01\x0b\x02\uffff\x01\x32\x01\x1e\x0a\uffff\x01\x2e\x01\x49"+
        "\x01\x26\x01\uffff\x01\x4d\x04\uffff\x01\x3d\x01\uffff\x01\x01\x02"+
        "\uffff\x01\x02\x01\uffff\x01\x30\x05\uffff\x01\x06\x01\uffff\x01"+
        "\x07\x01\x2b\x01\x3e\x01\x2a\x01\x15\x01\x14\x01\x17\x01\uffff\x01"+
        "\x50\x02\uffff\x01\x4f\x01\uffff\x01\x13\x01\uffff\x01\x27\x01\uffff"+
        "\x01\x44\x05\uffff\x01\x3f\x01\uffff\x01\x36\x01\uffff\x01\x04\x01"+
        "\uffff\x01\x35\x01\x2c\x01\uffff\x01\x2d\x04\uffff\x01\x39\x01\uffff"+
        "\x01\x12\x02\uffff\x01\x37\x01\uffff\x01\x45\x02\uffff\x01\x28\x01"+
        "\uffff\x01\x47\x07\uffff\x01\x41\x05\uffff\x01\x40\x01\uffff\x01"+
        "\x53\x01\x3b\x02\uffff\x01\x52\x04\uffff\x01\x38\x02\uffff\x01\x43";
    const string DFA33_specialS =
        "\u01ef\uffff}>";
    static readonly string[] DFA33_transitionS = {
            "\x02\x32\x01\uffff\x02\x32\x12\uffff\x01\x32\x01\x22\x01\x33"+
            "\x01\uffff\x01\x35\x01\x27\x01\x2b\x01\x34\x01\x1a\x01\x1b\x01"+
            "\x26\x01\x25\x01\x21\x01\x16\x01\x2d\x01\x24\x0a\x36\x01\x20"+
            "\x01\x2c\x01\x18\x01\x17\x01\x28\x01\x19\x01\uffff\x1a\x35\x01"+
            "\x1c\x01\x31\x01\x1d\x01\x29\x01\x35\x01\uffff\x01\x09\x01\x04"+
            "\x01\x01\x01\x12\x01\x07\x01\x0e\x01\x10\x01\x11\x01\x03\x01"+
            "\x0f\x01\x35\x01\x05\x01\x0c\x01\x0a\x01\x08\x01\x14\x01\x35"+
            "\x01\x06\x01\x0b\x01\x0d\x01\x13\x01\x15\x01\x02\x03\x35\x01"+
            "\x1e\x01\x2a\x01\x1f\x01\x23\u187e\uffff\x01\x30\x01\x2f\x01"+
            "\x2e",
            "\x01\x39\x0d\uffff\x01\x38\x02\uffff\x01\x37\x02\uffff\x01"+
            "\x3a",
            "\x01\x3d\x02\uffff\x01\x3c\x01\x3b",
            "\x01\x3e\x03\uffff\x01\x40\x01\x3f",
            "\x01\x41\x13\uffff\x01\x42",
            "\x01\x45\x03\uffff\x01\x44\x03\uffff\x01\x43",
            "\x01\x46\x03\uffff\x01\x47\x09\uffff\x01\x48",
            "\x01\x4b\x01\uffff\x01\x4c\x04\uffff\x01\x49\x02\uffff\x01"+
            "\x4a\x01\uffff\x01\x4d",
            "\x01\x50\x03\uffff\x01\x4e\x02\uffff\x01\x4f",
            "\x01\x54\x01\uffff\x01\x51\x04\uffff\x01\x52\x02\uffff\x01"+
            "\x53",
            "\x01\x55\x05\uffff\x01\x56",
            "\x01\x59\x08\uffff\x01\x5b\x02\uffff\x01\x5a\x02\uffff\x01"+
            "\x58\x01\x57",
            "\x01\x5c\x03\uffff\x01\x5e\x03\uffff\x01\x5d\x09\uffff\x01"+
            "\x5f",
            "\x01\x60\x09\uffff\x01\x61",
            "\x01\x65\x07\uffff\x01\x64\x08\uffff\x01\x62\x02\uffff\x01"+
            "\x63",
            "\x01\x66",
            "\x01\x67",
            "\x01\x68\x0d\uffff\x01\x69",
            "\x01\x6c\x03\uffff\x01\x6b\x03\uffff\x01\x6a",
            "\x01\x6d",
            "\x01\x6e\x10\uffff\x01\x6f",
            "\x01\x70",
            "\x01\x73\x0f\uffff\x01\x72\x01\x71",
            "\x01\x75",
            "\x01\x78\x01\x79\x01\x77",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\x7b",
            "",
            "\x01\x7f\x04\uffff\x01\x7e\x0d\uffff\x01\x7d",
            "\x01\u0082\x11\uffff\x01\u0081",
            "\x01\u0084",
            "\x01\u0086",
            "\x01\u0089\x01\u0088",
            "\x01\u008b",
            "\x01\u008d\x3e\uffff\x01\u008e",
            "\x01\u0091\x16\uffff\x01\u0090",
            "",
            "\x0a\x36",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\u0094",
            "\x01\u0095\x13\uffff\x01\u0096",
            "\x01\u0097",
            "\x01\u0098",
            "\x01\u0099",
            "\x01\u009a",
            "\x01\u009b",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x12\x35\x01\u009c\x01\u009d\x06\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x13\x35\x01\u009f\x06\x35",
            "\x01\u00a1",
            "\x01\u00a2",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u00a4",
            "\x01\u00a5",
            "\x01\u00a6",
            "\x01\u00a7",
            "\x01\u00a8",
            "\x01\u00a9",
            "\x01\u00aa",
            "\x01\u00ab",
            "\x01\u00ac",
            "\x01\u00ad",
            "\x01\u00ae",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x03\x35\x01\u00af\x16\x35",
            "\x01\u00b1",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u00b3",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x02\x35\x01\u00b4\x17\x35",
            "\x01\u00b7\x01\uffff\x01\u00b6",
            "\x01\u00b8",
            "\x01\u00b9",
            "\x01\u00ba",
            "\x01\u00bb",
            "\x01\u00bc",
            "\x01\u00be\x08\uffff\x01\u00bd\x07\uffff\x01\u00bf",
            "\x01\u00c0",
            "\x01\u00c1",
            "\x01\u00c2",
            "\x01\u00c4\x01\uffff\x01\u00c3",
            "\x01\u00c5\x0f\uffff\x01\u00c6",
            "\x01\u00c7",
            "\x01\u00c8",
            "\x01\u00c9",
            "\x01\u00ca",
            "\x01\u00cb",
            "\x01\u00cc",
            "\x01\u00cd",
            "\x01\u00ce",
            "\x01\u00cf",
            "\x01\u00d0",
            "\x01\u00d1",
            "\x01\u00d2",
            "\x01\u00d4\x06\uffff\x01\u00d3",
            "\x01\u00d5",
            "\x01\u00d6",
            "\x01\u00d7",
            "\x01\u00d8\x03\uffff\x01\u00d9",
            "\x01\u00da",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\u00db",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\u00dd\x01\u00de",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\u00e0",
            "\x01\u00e1",
            "\x01\u00e2",
            "\x01\u00e3\x0e\uffff\x01\u00e4",
            "\x01\u00e5",
            "\x01\u00e6",
            "\x01\u00e8\x03\uffff\x01\u00e7",
            "\x01\u00e9",
            "\x01\u00ea\x0e\uffff\x01\u00eb",
            "\x01\u00ec",
            "",
            "\x01\u00ed",
            "",
            "\x01\u00ee",
            "\x01\u00ef",
            "",
            "\x01\u00f0",
            "\x01\u00f1",
            "\x01\u00f2",
            "\x01\u00f3",
            "\x01\u00f4",
            "\x01\u00f5",
            "\x01\u00f6",
            "\x01\u00f8\x03\uffff\x01\u00f7",
            "\x01\u00f9",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u00fb",
            "\x01\u00fc",
            "",
            "\x01\u00fd\x0a\uffff\x01\u00fe",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u0102",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u0105",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u0107",
            "\x01\u0108",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x0e\x35\x01\u0109\x0b\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u010d",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x14\x35\x01\u010f\x05\x35",
            "\x01\u0111",
            "\x01\u0112",
            "\x01\u0113",
            "\x01\u0114",
            "\x01\u0115",
            "\x01\u0116",
            "\x01\u0117",
            "\x01\u0118",
            "\x01\u0119",
            "\x01\u011a",
            "\x01\u011b",
            "\x01\u011c",
            "\x01\u011d",
            "\x01\u011e",
            "\x01\u011f",
            "\x01\u0120",
            "\x01\u0121",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x12\x35\x01\u0122\x07\x35",
            "\x01\u0124",
            "\x01\u0125",
            "\x01\u0126",
            "\x01\u0127",
            "\x01\u0128",
            "",
            "",
            "",
            "\x01\u0129",
            "",
            "\x01\u012b",
            "\x01\u012c",
            "\x01\u012d",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u0130",
            "\x01\u0131",
            "\x01\u0132",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u0134",
            "\x01\u0135",
            "\x01\u0136",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u0138",
            "\x01\u0139",
            "\x01\u013a",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x16\x35\x01\u013d\x03\x35",
            "\x01\u013f",
            "\x01\u0140",
            "\x01\u0141",
            "\x01\u0142",
            "\x01\u0143",
            "\x01\u0144",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u0146",
            "\x01\u0147",
            "\x01\u0148",
            "\x01\u0149",
            "",
            "",
            "",
            "\x01\u014a",
            "",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u014c",
            "\x01\u014d",
            "\x01\u014e",
            "",
            "",
            "",
            "\x01\u014f",
            "",
            "\x01\u0150",
            "",
            "\x01\u0151",
            "\x01\u0152",
            "\x01\u0153",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u0159",
            "\x01\u015a",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u015c",
            "\x01\u015d",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x12\x35\x01\u015e\x07\x35",
            "\x01\u0160",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u0162",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u0164",
            "\x01\u0165",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u0167",
            "\x01\u0168",
            "",
            "",
            "\x01\u0169",
            "\x01\u016a",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "",
            "\x01\u016c",
            "\x01\u016d",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u016f",
            "\x01\u0170",
            "\x01\u0171",
            "",
            "\x01\u0172",
            "\x01\u0173",
            "\x01\u0174",
            "",
            "",
            "\x01\u0175",
            "",
            "\x01\u0176",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u0178",
            "\x01\u0179",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u017b",
            "",
            "\x01\u017c",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u017f",
            "\x01\u0180",
            "",
            "\x01\u0181",
            "\x01\u0182",
            "\x01\u0183",
            "\x01\u0184",
            "\x01\u0185",
            "\x01\u0186",
            "\x01\u0187",
            "\x01\u0188",
            "",
            "",
            "",
            "",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u018c",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u018e",
            "",
            "\x01\u018f",
            "",
            "\x01\u0190",
            "\x01\u0191",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u0193",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u0195",
            "",
            "\x01\u0196",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u0198",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u019a",
            "\x01\u019b",
            "\x01\u019c",
            "\x01\u019d",
            "\x01\u019e",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u01a0",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x12\x35\x01\u01a8\x07\x35",
            "\x01\u01aa",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x12\x35\x01\u01ab\x07\x35",
            "\x01\u01ad",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u01af",
            "",
            "",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u01b1",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u01b3",
            "\x01\u01b4",
            "",
            "\x01\u01b5",
            "",
            "\x01\u01b6",
            "\x01\u01b7",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u01b9",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u01bb",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u01bd",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u01c0",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u01c2",
            "",
            "\x01\u01c3",
            "",
            "\x01\u01c4",
            "",
            "\x01\u01c5",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u01c7",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u01c9",
            "",
            "\x01\u01ca",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u01cc",
            "",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u01ce",
            "\x01\u01cf",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u01d1",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "\x01\u01d3",
            "\x01\u01d4",
            "",
            "\x01\u01d5",
            "",
            "\x01\u01d6",
            "\x01\u01d7",
            "",
            "\x01\u01d8",
            "",
            "\x01\u01d9",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u01db",
            "\x01\u01dc",
            "\x01\u01dd",
            "\x01\u01de",
            "\x01\u01df",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x12\x35\x01\u01e1\x07\x35",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u01e4",
            "\x01\u01e5",
            "",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "",
            "",
            "\x01\u01e7",
            "\x01\u01e8",
            "",
            "\x01\u01e9",
            "\x01\u01ea",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            "\x01\u01ec",
            "",
            "\x01\u01ed",
            "\x01\x35\x0b\uffff\x0a\x35\x07\uffff\x1a\x35\x04\uffff\x01"+
            "\x35\x01\uffff\x1a\x35",
            ""
    };

    static readonly short[] DFA33_eot = DFA.UnpackEncodedString(DFA33_eotS);
    static readonly short[] DFA33_eof = DFA.UnpackEncodedString(DFA33_eofS);
    static readonly char[] DFA33_min = DFA.UnpackEncodedStringToUnsignedChars(DFA33_minS);
    static readonly char[] DFA33_max = DFA.UnpackEncodedStringToUnsignedChars(DFA33_maxS);
    static readonly short[] DFA33_accept = DFA.UnpackEncodedString(DFA33_acceptS);
    static readonly short[] DFA33_special = DFA.UnpackEncodedString(DFA33_specialS);
    static readonly short[][] DFA33_transition = DFA.UnpackEncodedStringArray(DFA33_transitionS);

    protected class DFA33 : DFA
    {
        public DFA33(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 33;
            this.eot = DFA33_eot;
            this.eof = DFA33_eof;
            this.min = DFA33_min;
            this.max = DFA33_max;
            this.accept = DFA33_accept;
            this.special = DFA33_special;
            this.transition = DFA33_transition;

        }

        override public string Description
        {
            get { return "1:1: Tokens : ( CREATE | WINDOW | IN_SET | BETWEEN | LIKE | REGEXP | ESCAPE | OR_EXPR | AND_EXPR | NOT_EXPR | EVERY_EXPR | WHERE | AS | SUM | AVG | MAX | MIN | COALESCE | MEDIAN | STDDEV | AVEDEV | COUNT | SELECT | CASE | ELSE | WHEN | THEN | END | FROM | OUTER | JOIN | LEFT | RIGHT | FULL | ON | IS | BY | GROUP | HAVING | DISTINCT | ALL | OUTPUT | EVENTS | SECONDS | MINUTES | FIRST | LAST | INSERT | INTO | ORDER | ASC | DESC | RSTREAM | ISTREAM | IRSTREAM | UNIDIRECTIONAL | PATTERN | SQL | METADATASQL | PREVIOUS | PRIOR | EXISTS | WEEKDAY | LW | INSTANCEOF | CAST | CURRENT_TIMESTAMP | DELETE | SNAPSHOT | SET | VARIABLE | T__238 | T__239 | T__240 | T__241 | T__242 | T__243 | T__244 | T__245 | T__246 | T__247 | T__248 | T__249 | T__250 | FOLLOWED_BY | EQUALS | SQL_NE | QUESTION | LPAREN | RPAREN | LBRACK | RBRACK | LCURLY | RCURLY | COLON | COMMA | EQUAL | LNOT | BNOT | NOT_EQUAL | DIV | DIV_ASSIGN | PLUS | PLUS_ASSIGN | INC | MINUS | MINUS_ASSIGN | DEC | STAR | STAR_ASSIGN | MOD | MOD_ASSIGN | SR | SR_ASSIGN | BSR | BSR_ASSIGN | GE | GT | SL | SL_ASSIGN | LE | LT | BXOR | BXOR_ASSIGN | BOR | BOR_ASSIGN | LOR | BAND | BAND_ASSIGN | LAND | SEMI | DOT | NUM_LONG | NUM_DOUBLE | NUM_FLOAT | ESCAPECHAR | WS | SL_COMMENT | ML_COMMENT | STRING_LITERAL | QUOTED_STRING_LITERAL | IDENT | NUM_INT );"; }
        }

    }

 
    
}
}