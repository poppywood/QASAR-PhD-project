// $ANTLR 3.1 EsperEPL2Ast.g 2008-08-20 22:37:57

  using System;
  using System.Collections.Generic;
  using com.espertech.esper.compat;  
  using log4net;

using Antlr.Runtime;
using Antlr.Runtime.Tree;using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;

using IDictionary	= System.Collections.IDictionary;
using Hashtable 	= System.Collections.Hashtable;

public partial class EsperEPL2Ast : TreeParser
{
    public static readonly string[] tokenNames = new string[] 
	{
        "<invalid>", 
		"<EOR>", 
		"<DOWN>", 
		"<UP>", 
		"CREATE", 
		"WINDOW", 
		"IN_SET", 
		"BETWEEN", 
		"LIKE", 
		"REGEXP", 
		"ESCAPE", 
		"OR_EXPR", 
		"AND_EXPR", 
		"NOT_EXPR", 
		"EVERY_EXPR", 
		"WHERE", 
		"AS", 
		"SUM", 
		"AVG", 
		"MAX", 
		"MIN", 
		"COALESCE", 
		"MEDIAN", 
		"STDDEV", 
		"AVEDEV", 
		"COUNT", 
		"SELECT", 
		"CASE", 
		"CASE2", 
		"ELSE", 
		"WHEN", 
		"THEN", 
		"END", 
		"FROM", 
		"OUTER", 
		"JOIN", 
		"LEFT", 
		"RIGHT", 
		"FULL", 
		"ON", 
		"IS", 
		"BY", 
		"GROUP", 
		"HAVING", 
		"DISTINCT", 
		"ALL", 
		"OUTPUT", 
		"EVENTS", 
		"SECONDS", 
		"MINUTES", 
		"FIRST", 
		"LAST", 
		"INSERT", 
		"INTO", 
		"ORDER", 
		"ASC", 
		"DESC", 
		"RSTREAM", 
		"ISTREAM", 
		"IRSTREAM", 
		"UNIDIRECTIONAL", 
		"PATTERN", 
		"SQL", 
		"METADATASQL", 
		"PREVIOUS", 
		"PRIOR", 
		"EXISTS", 
		"WEEKDAY", 
		"LW", 
		"INSTANCEOF", 
		"CAST", 
		"CURRENT_TIMESTAMP", 
		"DELETE", 
		"SNAPSHOT", 
		"SET", 
		"VARIABLE", 
		"NUMERIC_PARAM_RANGE", 
		"NUMERIC_PARAM_LIST", 
		"NUMERIC_PARAM_FREQUENCY", 
		"FOLLOWED_BY_EXPR", 
		"ARRAY_PARAM_LIST", 
		"EVENT_FILTER_EXPR", 
		"EVENT_FILTER_IDENT", 
		"EVENT_FILTER_PARAM", 
		"EVENT_FILTER_RANGE", 
		"EVENT_FILTER_NOT_RANGE", 
		"EVENT_FILTER_IN", 
		"EVENT_FILTER_NOT_IN", 
		"EVENT_FILTER_BETWEEN", 
		"EVENT_FILTER_NOT_BETWEEN", 
		"CLASS_IDENT", 
		"GUARD_EXPR", 
		"OBSERVER_EXPR", 
		"VIEW_EXPR", 
		"PATTERN_INCL_EXPR", 
		"DATABASE_JOIN_EXPR", 
		"WHERE_EXPR", 
		"HAVING_EXPR", 
		"EVAL_BITWISE_EXPR", 
		"EVAL_AND_EXPR", 
		"EVAL_OR_EXPR", 
		"EVAL_EQUALS_EXPR", 
		"EVAL_NOTEQUALS_EXPR", 
		"EVAL_IDENT", 
		"SELECTION_EXPR", 
		"SELECTION_ELEMENT_EXPR", 
		"SELECTION_STREAM", 
		"STREAM_EXPR", 
		"OUTERJOIN_EXPR", 
		"LEFT_OUTERJOIN_EXPR", 
		"RIGHT_OUTERJOIN_EXPR", 
		"FULL_OUTERJOIN_EXPR", 
		"GROUP_BY_EXPR", 
		"ORDER_BY_EXPR", 
		"ORDER_ELEMENT_EXPR", 
		"EVENT_PROP_EXPR", 
		"EVENT_PROP_SIMPLE", 
		"EVENT_PROP_MAPPED", 
		"EVENT_PROP_INDEXED", 
		"EVENT_PROP_DYNAMIC_SIMPLE", 
		"EVENT_PROP_DYNAMIC_INDEXED", 
		"EVENT_PROP_DYNAMIC_MAPPED", 
		"EVENT_LIMIT_EXPR", 
		"SEC_LIMIT_EXPR", 
		"MIN_LIMIT_EXPR", 
		"TIMEPERIOD_LIMIT_EXPR", 
		"INSERTINTO_EXPR", 
		"INSERTINTO_EXPRCOL", 
		"CONCAT", 
		"LIB_FUNCTION", 
		"UNARY_MINUS", 
		"TIME_PERIOD", 
		"ARRAY_EXPR", 
		"DAY_PART", 
		"HOUR_PART", 
		"MINUTE_PART", 
		"SECOND_PART", 
		"MILLISECOND_PART", 
		"NOT_IN_SET", 
		"NOT_BETWEEN", 
		"NOT_LIKE", 
		"NOT_REGEXP", 
		"DBSELECT_EXPR", 
		"DBFROM_CLAUSE", 
		"DBWHERE_CLAUSE", 
		"WILDCARD_SELECT", 
		"INSERTINTO_STREAM_NAME", 
		"IN_RANGE", 
		"NOT_IN_RANGE", 
		"SUBSELECT_EXPR", 
		"EXISTS_SUBSELECT_EXPR", 
		"IN_SUBSELECT_EXPR", 
		"NOT_IN_SUBSELECT_EXPR", 
		"IN_SUBSELECT_QUERY_EXPR", 
		"LAST_OPERATOR", 
		"WEEKDAY_OPERATOR", 
		"SUBSTITUTION", 
		"CAST_EXPR", 
		"CREATE_WINDOW_EXPR", 
		"CREATE_WINDOW_SELECT_EXPR", 
		"ON_EXPR", 
		"ON_DELETE_EXPR", 
		"ON_SELECT_EXPR", 
		"ON_EXPR_FROM", 
		"ON_SET_EXPR", 
		"CREATE_VARIABLE_EXPR", 
		"METHOD_JOIN_EXPR", 
		"INT_TYPE", 
		"LONG_TYPE", 
		"FLOAT_TYPE", 
		"DOUBLE_TYPE", 
		"STRING_TYPE", 
		"BOOL_TYPE", 
		"NULL_TYPE", 
		"NUM_DOUBLE", 
		"EPL_EXPR", 
		"NUM_INT", 
		"NUM_LONG", 
		"NUM_FLOAT", 
		"QUESTION", 
		"MINUS", 
		"PLUS", 
		"STRING_LITERAL", 
		"QUOTED_STRING_LITERAL", 
		"IDENT", 
		"COMMA", 
		"EQUALS", 
		"DOT", 
		"STAR", 
		"LPAREN", 
		"RPAREN", 
		"LBRACK", 
		"RBRACK", 
		"COLON", 
		"BAND", 
		"BOR", 
		"BXOR", 
		"SQL_NE", 
		"NOT_EQUAL", 
		"LT", 
		"GT", 
		"LE", 
		"GE", 
		"LOR", 
		"DIV", 
		"MOD", 
		"LCURLY", 
		"RCURLY", 
		"FOLLOWED_BY", 
		"ESCAPECHAR", 
		"EQUAL", 
		"LNOT", 
		"BNOT", 
		"DIV_ASSIGN", 
		"PLUS_ASSIGN", 
		"INC", 
		"MINUS_ASSIGN", 
		"DEC", 
		"STAR_ASSIGN", 
		"MOD_ASSIGN", 
		"SR", 
		"SR_ASSIGN", 
		"BSR", 
		"BSR_ASSIGN", 
		"SL", 
		"SL_ASSIGN", 
		"BXOR_ASSIGN", 
		"BOR_ASSIGN", 
		"BAND_ASSIGN", 
		"LAND", 
		"SEMI", 
		"WS", 
		"SL_COMMENT", 
		"ML_COMMENT", 
		"ESC", 
		"HEX_DIGIT", 
		"EXPONENT", 
		"FLOAT_SUFFIX", 
		"'true'", 
		"'false'", 
		"'null'", 
		"'days'", 
		"'day'", 
		"'hours'", 
		"'hour'", 
		"'minute'", 
		"'second'", 
		"'sec'", 
		"'milliseconds'", 
		"'millisecond'", 
		"'msec'", 
		"NUMERIC_PARAM_FREQUENCE"
    };

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
    public const int NUMERIC_PARAM_FREQUENCE = 251;
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

    // delegates
    // delegators



        public EsperEPL2Ast(ITreeNodeStream input)
    		: this(input, new RecognizerSharedState()) {
        }

        public EsperEPL2Ast(ITreeNodeStream input, RecognizerSharedState state)
    		: base(input, state) {
    		InitializeCyclicDFAs();
             
        }
        

    override public string[] TokenNames {
		get { return EsperEPL2Ast.tokenNames; }
    }

    override public string GrammarFileName {
		get { return "EsperEPL2Ast.g"; }
    }


      private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      // For pattern processing within EPL and for create pattern
      protected virtual void SetIsPatternWalk(bool isPatternWalk) {}
      protected virtual void EndPattern() {}

      protected virtual void PushStmtContext() {}
      protected virtual void LeaveNode(ITree node) {}
      protected virtual void End() {}

      protected virtual void Mismatch(IIntStream input, int ttype, BitSet follow) {
        throw new MismatchedTokenException(ttype, input);  
      }

      public virtual void RecoverFromMismatchedToken(IIntStream intStream, RecognitionException recognitionException, int i, BitSet bitSet) {
        throw recognitionException;
      }

      public virtual void RecoverFromMismatchedSet(IIntStream intStream, RecognitionException recognitionException, BitSet bitSet) {
        throw recognitionException;
      }

      protected virtual bool RecoverFromMismatchedElement(IIntStream intStream, RecognitionException recognitionException, BitSet bitSet) {
        throw new ApplicationException("Error recovering from mismatched element", recognitionException);
      }
      
      public virtual void Recover(IIntStream intStream, RecognitionException recognitionException) {
        throw new ApplicationException("Error recovering from recognition exception", recognitionException);
      }



    // $ANTLR start "startEPLExpressionRule"
    // EsperEPL2Ast.g:59:1: startEPLExpressionRule : ^( EPL_EXPR eplExpressionRule ) ;
    public void startEPLExpressionRule() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:60:2: ( ^( EPL_EXPR eplExpressionRule ) )
            // EsperEPL2Ast.g:60:4: ^( EPL_EXPR eplExpressionRule )
            {
            	Match(input,EPL_EXPR,FOLLOW_EPL_EXPR_in_startEPLExpressionRule96); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_eplExpressionRule_in_startEPLExpressionRule98);
            	eplExpressionRule();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	   End(); 
            	}

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "startEPLExpressionRule"


    // $ANTLR start "eplExpressionRule"
    // EsperEPL2Ast.g:63:1: eplExpressionRule : ( selectExpr | createWindowExpr | createVariableExpr | onExpr ) ;
    public void eplExpressionRule() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:64:2: ( ( selectExpr | createWindowExpr | createVariableExpr | onExpr ) )
            // EsperEPL2Ast.g:64:4: ( selectExpr | createWindowExpr | createVariableExpr | onExpr )
            {
            	// EsperEPL2Ast.g:64:4: ( selectExpr | createWindowExpr | createVariableExpr | onExpr )
            	int alt1 = 4;
            	switch ( input.LA(1) ) 
            	{
            	case SELECTION_EXPR:
            	case INSERTINTO_EXPR:
            		{
            	    alt1 = 1;
            	    }
            	    break;
            	case CREATE_WINDOW_EXPR:
            		{
            	    alt1 = 2;
            	    }
            	    break;
            	case CREATE_VARIABLE_EXPR:
            		{
            	    alt1 = 3;
            	    }
            	    break;
            	case ON_EXPR:
            		{
            	    alt1 = 4;
            	    }
            	    break;
            		default:
            		    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            		    NoViableAltException nvae_d1s0 =
            		        new NoViableAltException("", 1, 0, input);

            		    throw nvae_d1s0;
            	}

            	switch (alt1) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:64:5: selectExpr
            	        {
            	        	PushFollow(FOLLOW_selectExpr_in_eplExpressionRule115);
            	        	selectExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // EsperEPL2Ast.g:64:18: createWindowExpr
            	        {
            	        	PushFollow(FOLLOW_createWindowExpr_in_eplExpressionRule119);
            	        	createWindowExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 3 :
            	        // EsperEPL2Ast.g:64:37: createVariableExpr
            	        {
            	        	PushFollow(FOLLOW_createVariableExpr_in_eplExpressionRule123);
            	        	createVariableExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 4 :
            	        // EsperEPL2Ast.g:64:58: onExpr
            	        {
            	        	PushFollow(FOLLOW_onExpr_in_eplExpressionRule127);
            	        	onExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}


            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "eplExpressionRule"


    // $ANTLR start "onExpr"
    // EsperEPL2Ast.g:67:1: onExpr : ^(i= ON_EXPR ( eventFilterExpr | patternInclusionExpression ) ( IDENT )? ( onDeleteExpr | onSelectExpr | onSetExpr ) ) ;
    public void onExpr() // throws RecognitionException [1]
    {   
        CommonTree i = null;

        try 
    	{
            // EsperEPL2Ast.g:68:2: ( ^(i= ON_EXPR ( eventFilterExpr | patternInclusionExpression ) ( IDENT )? ( onDeleteExpr | onSelectExpr | onSetExpr ) ) )
            // EsperEPL2Ast.g:68:4: ^(i= ON_EXPR ( eventFilterExpr | patternInclusionExpression ) ( IDENT )? ( onDeleteExpr | onSelectExpr | onSetExpr ) )
            {
            	i=(CommonTree)Match(input,ON_EXPR,FOLLOW_ON_EXPR_in_onExpr146); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// EsperEPL2Ast.g:68:16: ( eventFilterExpr | patternInclusionExpression )
            	int alt2 = 2;
            	int LA2_0 = input.LA(1);

            	if ( (LA2_0 == EVENT_FILTER_EXPR) )
            	{
            	    alt2 = 1;
            	}
            	else if ( (LA2_0 == PATTERN_INCL_EXPR) )
            	{
            	    alt2 = 2;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d2s0 =
            	        new NoViableAltException("", 2, 0, input);

            	    throw nvae_d2s0;
            	}
            	switch (alt2) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:68:17: eventFilterExpr
            	        {
            	        	PushFollow(FOLLOW_eventFilterExpr_in_onExpr149);
            	        	eventFilterExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // EsperEPL2Ast.g:68:35: patternInclusionExpression
            	        {
            	        	PushFollow(FOLLOW_patternInclusionExpression_in_onExpr153);
            	        	patternInclusionExpression();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:68:63: ( IDENT )?
            	int alt3 = 2;
            	int LA3_0 = input.LA(1);

            	if ( (LA3_0 == IDENT) )
            	{
            	    alt3 = 1;
            	}
            	switch (alt3) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:68:63: IDENT
            	        {
            	        	Match(input,IDENT,FOLLOW_IDENT_in_onExpr156); if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:69:3: ( onDeleteExpr | onSelectExpr | onSetExpr )
            	int alt4 = 3;
            	switch ( input.LA(1) ) 
            	{
            	case ON_DELETE_EXPR:
            		{
            	    alt4 = 1;
            	    }
            	    break;
            	case ON_SELECT_EXPR:
            		{
            	    alt4 = 2;
            	    }
            	    break;
            	case ON_SET_EXPR:
            		{
            	    alt4 = 3;
            	    }
            	    break;
            		default:
            		    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            		    NoViableAltException nvae_d4s0 =
            		        new NoViableAltException("", 4, 0, input);

            		    throw nvae_d4s0;
            	}

            	switch (alt4) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:69:4: onDeleteExpr
            	        {
            	        	PushFollow(FOLLOW_onDeleteExpr_in_onExpr163);
            	        	onDeleteExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // EsperEPL2Ast.g:69:19: onSelectExpr
            	        {
            	        	PushFollow(FOLLOW_onSelectExpr_in_onExpr167);
            	        	onSelectExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 3 :
            	        // EsperEPL2Ast.g:69:34: onSetExpr
            	        {
            	        	PushFollow(FOLLOW_onSetExpr_in_onExpr171);
            	        	onSetExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(i); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "onExpr"


    // $ANTLR start "onDeleteExpr"
    // EsperEPL2Ast.g:73:1: onDeleteExpr : ^( ON_DELETE_EXPR onExprFrom ( whereClause )? ) ;
    public void onDeleteExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:74:2: ( ^( ON_DELETE_EXPR onExprFrom ( whereClause )? ) )
            // EsperEPL2Ast.g:74:4: ^( ON_DELETE_EXPR onExprFrom ( whereClause )? )
            {
            	Match(input,ON_DELETE_EXPR,FOLLOW_ON_DELETE_EXPR_in_onDeleteExpr191); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_onExprFrom_in_onDeleteExpr193);
            	onExprFrom();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:74:32: ( whereClause )?
            	int alt5 = 2;
            	int LA5_0 = input.LA(1);

            	if ( (LA5_0 == WHERE_EXPR) )
            	{
            	    alt5 = 1;
            	}
            	switch (alt5) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:74:33: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_onDeleteExpr196);
            	        	whereClause();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "onDeleteExpr"


    // $ANTLR start "onSelectExpr"
    // EsperEPL2Ast.g:77:1: onSelectExpr : ^( ON_SELECT_EXPR ( insertIntoExpr )? selectionList onExprFrom ( whereClause )? ( groupByClause )? ( havingClause )? ( orderByClause )? ) ;
    public void onSelectExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:78:2: ( ^( ON_SELECT_EXPR ( insertIntoExpr )? selectionList onExprFrom ( whereClause )? ( groupByClause )? ( havingClause )? ( orderByClause )? ) )
            // EsperEPL2Ast.g:78:4: ^( ON_SELECT_EXPR ( insertIntoExpr )? selectionList onExprFrom ( whereClause )? ( groupByClause )? ( havingClause )? ( orderByClause )? )
            {
            	Match(input,ON_SELECT_EXPR,FOLLOW_ON_SELECT_EXPR_in_onSelectExpr213); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// EsperEPL2Ast.g:78:21: ( insertIntoExpr )?
            	int alt6 = 2;
            	int LA6_0 = input.LA(1);

            	if ( (LA6_0 == INSERTINTO_EXPR) )
            	{
            	    alt6 = 1;
            	}
            	switch (alt6) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:78:22: insertIntoExpr
            	        {
            	        	PushFollow(FOLLOW_insertIntoExpr_in_onSelectExpr216);
            	        	insertIntoExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_selectionList_in_onSelectExpr220);
            	selectionList();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	PushFollow(FOLLOW_onExprFrom_in_onSelectExpr222);
            	onExprFrom();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:78:64: ( whereClause )?
            	int alt7 = 2;
            	int LA7_0 = input.LA(1);

            	if ( (LA7_0 == WHERE_EXPR) )
            	{
            	    alt7 = 1;
            	}
            	switch (alt7) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:78:65: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_onSelectExpr225);
            	        	whereClause();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:78:79: ( groupByClause )?
            	int alt8 = 2;
            	int LA8_0 = input.LA(1);

            	if ( (LA8_0 == GROUP_BY_EXPR) )
            	{
            	    alt8 = 1;
            	}
            	switch (alt8) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:78:80: groupByClause
            	        {
            	        	PushFollow(FOLLOW_groupByClause_in_onSelectExpr230);
            	        	groupByClause();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:78:96: ( havingClause )?
            	int alt9 = 2;
            	int LA9_0 = input.LA(1);

            	if ( (LA9_0 == HAVING_EXPR) )
            	{
            	    alt9 = 1;
            	}
            	switch (alt9) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:78:97: havingClause
            	        {
            	        	PushFollow(FOLLOW_havingClause_in_onSelectExpr235);
            	        	havingClause();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:78:112: ( orderByClause )?
            	int alt10 = 2;
            	int LA10_0 = input.LA(1);

            	if ( (LA10_0 == ORDER_BY_EXPR) )
            	{
            	    alt10 = 1;
            	}
            	switch (alt10) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:78:113: orderByClause
            	        {
            	        	PushFollow(FOLLOW_orderByClause_in_onSelectExpr240);
            	        	orderByClause();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "onSelectExpr"


    // $ANTLR start "onSetExpr"
    // EsperEPL2Ast.g:81:1: onSetExpr : ^( ON_SET_EXPR onSetAssignment ( onSetAssignment )* ) ;
    public void onSetExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:82:2: ( ^( ON_SET_EXPR onSetAssignment ( onSetAssignment )* ) )
            // EsperEPL2Ast.g:82:4: ^( ON_SET_EXPR onSetAssignment ( onSetAssignment )* )
            {
            	Match(input,ON_SET_EXPR,FOLLOW_ON_SET_EXPR_in_onSetExpr257); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_onSetAssignment_in_onSetExpr259);
            	onSetAssignment();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:82:34: ( onSetAssignment )*
            	do 
            	{
            	    int alt11 = 2;
            	    int LA11_0 = input.LA(1);

            	    if ( (LA11_0 == IDENT) )
            	    {
            	        alt11 = 1;
            	    }


            	    switch (alt11) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:82:35: onSetAssignment
            			    {
            			    	PushFollow(FOLLOW_onSetAssignment_in_onSetExpr262);
            			    	onSetAssignment();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop11;
            	    }
            	} while (true);

            	loop11:
            		;	// Stops C# compiler whining that label 'loop11' has no statements


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "onSetExpr"


    // $ANTLR start "onSetAssignment"
    // EsperEPL2Ast.g:85:1: onSetAssignment : IDENT valueExpr ;
    public void onSetAssignment() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:86:2: ( IDENT valueExpr )
            // EsperEPL2Ast.g:86:4: IDENT valueExpr
            {
            	Match(input,IDENT,FOLLOW_IDENT_in_onSetAssignment277); if (state.failed) return ;
            	PushFollow(FOLLOW_valueExpr_in_onSetAssignment279);
            	valueExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "onSetAssignment"


    // $ANTLR start "onExprFrom"
    // EsperEPL2Ast.g:89:1: onExprFrom : ^( ON_EXPR_FROM IDENT ( IDENT )? ) ;
    public void onExprFrom() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:90:2: ( ^( ON_EXPR_FROM IDENT ( IDENT )? ) )
            // EsperEPL2Ast.g:90:4: ^( ON_EXPR_FROM IDENT ( IDENT )? )
            {
            	Match(input,ON_EXPR_FROM,FOLLOW_ON_EXPR_FROM_in_onExprFrom291); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	Match(input,IDENT,FOLLOW_IDENT_in_onExprFrom293); if (state.failed) return ;
            	// EsperEPL2Ast.g:90:25: ( IDENT )?
            	int alt12 = 2;
            	int LA12_0 = input.LA(1);

            	if ( (LA12_0 == IDENT) )
            	{
            	    alt12 = 1;
            	}
            	switch (alt12) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:90:26: IDENT
            	        {
            	        	Match(input,IDENT,FOLLOW_IDENT_in_onExprFrom296); if (state.failed) return ;

            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "onExprFrom"


    // $ANTLR start "createWindowExpr"
    // EsperEPL2Ast.g:93:1: createWindowExpr : ^(i= CREATE_WINDOW_EXPR IDENT ( viewListExpr )? ( createSelectionList )? CLASS_IDENT ) ;
    public void createWindowExpr() // throws RecognitionException [1]
    {   
        CommonTree i = null;

        try 
    	{
            // EsperEPL2Ast.g:94:2: ( ^(i= CREATE_WINDOW_EXPR IDENT ( viewListExpr )? ( createSelectionList )? CLASS_IDENT ) )
            // EsperEPL2Ast.g:94:4: ^(i= CREATE_WINDOW_EXPR IDENT ( viewListExpr )? ( createSelectionList )? CLASS_IDENT )
            {
            	i=(CommonTree)Match(input,CREATE_WINDOW_EXPR,FOLLOW_CREATE_WINDOW_EXPR_in_createWindowExpr314); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	Match(input,IDENT,FOLLOW_IDENT_in_createWindowExpr316); if (state.failed) return ;
            	// EsperEPL2Ast.g:94:33: ( viewListExpr )?
            	int alt13 = 2;
            	int LA13_0 = input.LA(1);

            	if ( (LA13_0 == VIEW_EXPR) )
            	{
            	    alt13 = 1;
            	}
            	switch (alt13) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:94:34: viewListExpr
            	        {
            	        	PushFollow(FOLLOW_viewListExpr_in_createWindowExpr319);
            	        	viewListExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:94:49: ( createSelectionList )?
            	int alt14 = 2;
            	int LA14_0 = input.LA(1);

            	if ( (LA14_0 == CREATE_WINDOW_SELECT_EXPR) )
            	{
            	    alt14 = 1;
            	}
            	switch (alt14) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:94:50: createSelectionList
            	        {
            	        	PushFollow(FOLLOW_createSelectionList_in_createWindowExpr324);
            	        	createSelectionList();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	Match(input,CLASS_IDENT,FOLLOW_CLASS_IDENT_in_createWindowExpr328); if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(i); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "createWindowExpr"


    // $ANTLR start "createVariableExpr"
    // EsperEPL2Ast.g:97:1: createVariableExpr : ^(i= CREATE_VARIABLE_EXPR IDENT IDENT ( valueExpr )? ) ;
    public void createVariableExpr() // throws RecognitionException [1]
    {   
        CommonTree i = null;

        try 
    	{
            // EsperEPL2Ast.g:98:2: ( ^(i= CREATE_VARIABLE_EXPR IDENT IDENT ( valueExpr )? ) )
            // EsperEPL2Ast.g:98:4: ^(i= CREATE_VARIABLE_EXPR IDENT IDENT ( valueExpr )? )
            {
            	i=(CommonTree)Match(input,CREATE_VARIABLE_EXPR,FOLLOW_CREATE_VARIABLE_EXPR_in_createVariableExpr347); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	Match(input,IDENT,FOLLOW_IDENT_in_createVariableExpr349); if (state.failed) return ;
            	Match(input,IDENT,FOLLOW_IDENT_in_createVariableExpr351); if (state.failed) return ;
            	// EsperEPL2Ast.g:98:41: ( valueExpr )?
            	int alt15 = 2;
            	alt15 = dfa15.Predict(input);
            	switch (alt15) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:98:42: valueExpr
            	        {
            	        	PushFollow(FOLLOW_valueExpr_in_createVariableExpr354);
            	        	valueExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(i); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "createVariableExpr"


    // $ANTLR start "createSelectionList"
    // EsperEPL2Ast.g:101:1: createSelectionList : ^(s= CREATE_WINDOW_SELECT_EXPR createSelectionListElement ( createSelectionListElement )* ) ;
    public void createSelectionList() // throws RecognitionException [1]
    {   
        CommonTree s = null;

        try 
    	{
            // EsperEPL2Ast.g:102:2: ( ^(s= CREATE_WINDOW_SELECT_EXPR createSelectionListElement ( createSelectionListElement )* ) )
            // EsperEPL2Ast.g:102:4: ^(s= CREATE_WINDOW_SELECT_EXPR createSelectionListElement ( createSelectionListElement )* )
            {
            	s=(CommonTree)Match(input,CREATE_WINDOW_SELECT_EXPR,FOLLOW_CREATE_WINDOW_SELECT_EXPR_in_createSelectionList374); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_createSelectionListElement_in_createSelectionList376);
            	createSelectionListElement();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:102:61: ( createSelectionListElement )*
            	do 
            	{
            	    int alt16 = 2;
            	    int LA16_0 = input.LA(1);

            	    if ( (LA16_0 == SELECTION_ELEMENT_EXPR || LA16_0 == WILDCARD_SELECT) )
            	    {
            	        alt16 = 1;
            	    }


            	    switch (alt16) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:102:62: createSelectionListElement
            			    {
            			    	PushFollow(FOLLOW_createSelectionListElement_in_createSelectionList379);
            			    	createSelectionListElement();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop16;
            	    }
            	} while (true);

            	loop16:
            		;	// Stops C# compiler whining that label 'loop16' has no statements

            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(s); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "createSelectionList"


    // $ANTLR start "createSelectionListElement"
    // EsperEPL2Ast.g:105:1: createSelectionListElement : (w= WILDCARD_SELECT | ^(s= SELECTION_ELEMENT_EXPR eventPropertyExpr ( IDENT )? ) );
    public void createSelectionListElement() // throws RecognitionException [1]
    {   
        CommonTree w = null;
        CommonTree s = null;

        try 
    	{
            // EsperEPL2Ast.g:106:2: (w= WILDCARD_SELECT | ^(s= SELECTION_ELEMENT_EXPR eventPropertyExpr ( IDENT )? ) )
            int alt18 = 2;
            int LA18_0 = input.LA(1);

            if ( (LA18_0 == WILDCARD_SELECT) )
            {
                alt18 = 1;
            }
            else if ( (LA18_0 == SELECTION_ELEMENT_EXPR) )
            {
                alt18 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d18s0 =
                    new NoViableAltException("", 18, 0, input);

                throw nvae_d18s0;
            }
            switch (alt18) 
            {
                case 1 :
                    // EsperEPL2Ast.g:106:4: w= WILDCARD_SELECT
                    {
                    	w=(CommonTree)Match(input,WILDCARD_SELECT,FOLLOW_WILDCARD_SELECT_in_createSelectionListElement399); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(w); 
                    	}

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:107:4: ^(s= SELECTION_ELEMENT_EXPR eventPropertyExpr ( IDENT )? )
                    {
                    	s=(CommonTree)Match(input,SELECTION_ELEMENT_EXPR,FOLLOW_SELECTION_ELEMENT_EXPR_in_createSelectionListElement409); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_eventPropertyExpr_in_createSelectionListElement411);
                    	eventPropertyExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:107:49: ( IDENT )?
                    	int alt17 = 2;
                    	int LA17_0 = input.LA(1);

                    	if ( (LA17_0 == IDENT) )
                    	{
                    	    alt17 = 1;
                    	}
                    	switch (alt17) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:107:50: IDENT
                    	        {
                    	        	Match(input,IDENT,FOLLOW_IDENT_in_createSelectionListElement414); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(s); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "createSelectionListElement"


    // $ANTLR start "selectExpr"
    // EsperEPL2Ast.g:110:1: selectExpr : ( insertIntoExpr )? selectClause fromClause ( whereClause )? ( groupByClause )? ( havingClause )? ( outputLimitExpr )? ( orderByClause )? ;
    public void selectExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:111:2: ( ( insertIntoExpr )? selectClause fromClause ( whereClause )? ( groupByClause )? ( havingClause )? ( outputLimitExpr )? ( orderByClause )? )
            // EsperEPL2Ast.g:111:4: ( insertIntoExpr )? selectClause fromClause ( whereClause )? ( groupByClause )? ( havingClause )? ( outputLimitExpr )? ( orderByClause )?
            {
            	// EsperEPL2Ast.g:111:4: ( insertIntoExpr )?
            	int alt19 = 2;
            	int LA19_0 = input.LA(1);

            	if ( (LA19_0 == INSERTINTO_EXPR) )
            	{
            	    alt19 = 1;
            	}
            	switch (alt19) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:111:5: insertIntoExpr
            	        {
            	        	PushFollow(FOLLOW_insertIntoExpr_in_selectExpr432);
            	        	insertIntoExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_selectClause_in_selectExpr438);
            	selectClause();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	PushFollow(FOLLOW_fromClause_in_selectExpr443);
            	fromClause();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:114:3: ( whereClause )?
            	int alt20 = 2;
            	alt20 = dfa20.Predict(input);
            	switch (alt20) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:114:4: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_selectExpr448);
            	        	whereClause();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:115:3: ( groupByClause )?
            	int alt21 = 2;
            	int LA21_0 = input.LA(1);

            	if ( (LA21_0 == GROUP_BY_EXPR) )
            	{
            	    alt21 = 1;
            	}
            	switch (alt21) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:115:4: groupByClause
            	        {
            	        	PushFollow(FOLLOW_groupByClause_in_selectExpr455);
            	        	groupByClause();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:116:3: ( havingClause )?
            	int alt22 = 2;
            	int LA22_0 = input.LA(1);

            	if ( (LA22_0 == HAVING_EXPR) )
            	{
            	    alt22 = 1;
            	}
            	switch (alt22) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:116:4: havingClause
            	        {
            	        	PushFollow(FOLLOW_havingClause_in_selectExpr462);
            	        	havingClause();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:117:3: ( outputLimitExpr )?
            	int alt23 = 2;
            	int LA23_0 = input.LA(1);

            	if ( ((LA23_0 >= EVENT_LIMIT_EXPR && LA23_0 <= TIMEPERIOD_LIMIT_EXPR)) )
            	{
            	    alt23 = 1;
            	}
            	switch (alt23) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:117:4: outputLimitExpr
            	        {
            	        	PushFollow(FOLLOW_outputLimitExpr_in_selectExpr469);
            	        	outputLimitExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:118:3: ( orderByClause )?
            	int alt24 = 2;
            	int LA24_0 = input.LA(1);

            	if ( (LA24_0 == ORDER_BY_EXPR) )
            	{
            	    alt24 = 1;
            	}
            	switch (alt24) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:118:4: orderByClause
            	        {
            	        	PushFollow(FOLLOW_orderByClause_in_selectExpr476);
            	        	orderByClause();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}


            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "selectExpr"


    // $ANTLR start "insertIntoExpr"
    // EsperEPL2Ast.g:121:1: insertIntoExpr : ^(i= INSERTINTO_EXPR ( ISTREAM | RSTREAM )? IDENT ( insertIntoExprCol )? ) ;
    public void insertIntoExpr() // throws RecognitionException [1]
    {   
        CommonTree i = null;

        try 
    	{
            // EsperEPL2Ast.g:122:2: ( ^(i= INSERTINTO_EXPR ( ISTREAM | RSTREAM )? IDENT ( insertIntoExprCol )? ) )
            // EsperEPL2Ast.g:122:4: ^(i= INSERTINTO_EXPR ( ISTREAM | RSTREAM )? IDENT ( insertIntoExprCol )? )
            {
            	i=(CommonTree)Match(input,INSERTINTO_EXPR,FOLLOW_INSERTINTO_EXPR_in_insertIntoExpr493); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// EsperEPL2Ast.g:122:24: ( ISTREAM | RSTREAM )?
            	int alt25 = 2;
            	int LA25_0 = input.LA(1);

            	if ( ((LA25_0 >= RSTREAM && LA25_0 <= ISTREAM)) )
            	{
            	    alt25 = 1;
            	}
            	switch (alt25) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:
            	        {
            	        	if ( (input.LA(1) >= RSTREAM && input.LA(1) <= ISTREAM) ) 
            	        	{
            	        	    input.Consume();
            	        	    state.errorRecovery = false;state.failed = false;
            	        	}
            	        	else 
            	        	{
            	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	        	    throw mse;
            	        	}


            	        }
            	        break;

            	}

            	Match(input,IDENT,FOLLOW_IDENT_in_insertIntoExpr504); if (state.failed) return ;
            	// EsperEPL2Ast.g:122:51: ( insertIntoExprCol )?
            	int alt26 = 2;
            	int LA26_0 = input.LA(1);

            	if ( (LA26_0 == INSERTINTO_EXPRCOL) )
            	{
            	    alt26 = 1;
            	}
            	switch (alt26) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:122:52: insertIntoExprCol
            	        {
            	        	PushFollow(FOLLOW_insertIntoExprCol_in_insertIntoExpr507);
            	        	insertIntoExprCol();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(i); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "insertIntoExpr"


    // $ANTLR start "insertIntoExprCol"
    // EsperEPL2Ast.g:125:1: insertIntoExprCol : ^( INSERTINTO_EXPRCOL IDENT ( IDENT )* ) ;
    public void insertIntoExprCol() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:126:2: ( ^( INSERTINTO_EXPRCOL IDENT ( IDENT )* ) )
            // EsperEPL2Ast.g:126:4: ^( INSERTINTO_EXPRCOL IDENT ( IDENT )* )
            {
            	Match(input,INSERTINTO_EXPRCOL,FOLLOW_INSERTINTO_EXPRCOL_in_insertIntoExprCol526); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	Match(input,IDENT,FOLLOW_IDENT_in_insertIntoExprCol528); if (state.failed) return ;
            	// EsperEPL2Ast.g:126:31: ( IDENT )*
            	do 
            	{
            	    int alt27 = 2;
            	    int LA27_0 = input.LA(1);

            	    if ( (LA27_0 == IDENT) )
            	    {
            	        alt27 = 1;
            	    }


            	    switch (alt27) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:126:32: IDENT
            			    {
            			    	Match(input,IDENT,FOLLOW_IDENT_in_insertIntoExprCol531); if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop27;
            	    }
            	} while (true);

            	loop27:
            		;	// Stops C# compiler whining that label 'loop27' has no statements


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "insertIntoExprCol"


    // $ANTLR start "selectClause"
    // EsperEPL2Ast.g:129:1: selectClause : ^(s= SELECTION_EXPR ( RSTREAM | ISTREAM | IRSTREAM )? selectionList ) ;
    public void selectClause() // throws RecognitionException [1]
    {   
        CommonTree s = null;

        try 
    	{
            // EsperEPL2Ast.g:130:2: ( ^(s= SELECTION_EXPR ( RSTREAM | ISTREAM | IRSTREAM )? selectionList ) )
            // EsperEPL2Ast.g:130:4: ^(s= SELECTION_EXPR ( RSTREAM | ISTREAM | IRSTREAM )? selectionList )
            {
            	s=(CommonTree)Match(input,SELECTION_EXPR,FOLLOW_SELECTION_EXPR_in_selectClause549); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// EsperEPL2Ast.g:130:23: ( RSTREAM | ISTREAM | IRSTREAM )?
            	int alt28 = 2;
            	int LA28_0 = input.LA(1);

            	if ( ((LA28_0 >= RSTREAM && LA28_0 <= IRSTREAM)) )
            	{
            	    alt28 = 1;
            	}
            	switch (alt28) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:
            	        {
            	        	if ( (input.LA(1) >= RSTREAM && input.LA(1) <= IRSTREAM) ) 
            	        	{
            	        	    input.Consume();
            	        	    state.errorRecovery = false;state.failed = false;
            	        	}
            	        	else 
            	        	{
            	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	        	    throw mse;
            	        	}


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_selectionList_in_selectClause564);
            	selectionList();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(s); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "selectClause"


    // $ANTLR start "fromClause"
    // EsperEPL2Ast.g:133:1: fromClause : streamExpression ( streamExpression ( outerJoin )* )* ;
    public void fromClause() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:134:2: ( streamExpression ( streamExpression ( outerJoin )* )* )
            // EsperEPL2Ast.g:134:4: streamExpression ( streamExpression ( outerJoin )* )*
            {
            	PushFollow(FOLLOW_streamExpression_in_fromClause578);
            	streamExpression();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:134:21: ( streamExpression ( outerJoin )* )*
            	do 
            	{
            	    int alt30 = 2;
            	    alt30 = dfa30.Predict(input);
            	    switch (alt30) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:134:22: streamExpression ( outerJoin )*
            			    {
            			    	PushFollow(FOLLOW_streamExpression_in_fromClause581);
            			    	streamExpression();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;
            			    	// EsperEPL2Ast.g:134:39: ( outerJoin )*
            			    	do 
            			    	{
            			    	    int alt29 = 2;
            			    	    alt29 = dfa29.Predict(input);
            			    	    switch (alt29) 
            			    		{
            			    			case 1 :
            			    			    // EsperEPL2Ast.g:134:40: outerJoin
            			    			    {
            			    			    	PushFollow(FOLLOW_outerJoin_in_fromClause584);
            			    			    	outerJoin();
            			    			    	state.followingStackPointer--;
            			    			    	if (state.failed) return ;

            			    			    }
            			    			    break;

            			    			default:
            			    			    goto loop29;
            			    	    }
            			    	} while (true);

            			    	loop29:
            			    		;	// Stops C# compiler whining that label 'loop29' has no statements


            			    }
            			    break;

            			default:
            			    goto loop30;
            	    }
            	} while (true);

            	loop30:
            		;	// Stops C# compiler whining that label 'loop30' has no statements


            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "fromClause"


    // $ANTLR start "selectionList"
    // EsperEPL2Ast.g:137:1: selectionList : selectionListElement ( selectionListElement )* ;
    public void selectionList() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:138:2: ( selectionListElement ( selectionListElement )* )
            // EsperEPL2Ast.g:138:4: selectionListElement ( selectionListElement )*
            {
            	PushFollow(FOLLOW_selectionListElement_in_selectionList601);
            	selectionListElement();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:138:25: ( selectionListElement )*
            	do 
            	{
            	    int alt31 = 2;
            	    int LA31_0 = input.LA(1);

            	    if ( ((LA31_0 >= SELECTION_ELEMENT_EXPR && LA31_0 <= SELECTION_STREAM) || LA31_0 == WILDCARD_SELECT) )
            	    {
            	        alt31 = 1;
            	    }


            	    switch (alt31) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:138:26: selectionListElement
            			    {
            			    	PushFollow(FOLLOW_selectionListElement_in_selectionList604);
            			    	selectionListElement();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop31;
            	    }
            	} while (true);

            	loop31:
            		;	// Stops C# compiler whining that label 'loop31' has no statements


            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "selectionList"


    // $ANTLR start "selectionListElement"
    // EsperEPL2Ast.g:141:1: selectionListElement : (w= WILDCARD_SELECT | ^(e= SELECTION_ELEMENT_EXPR valueExpr ( IDENT )? ) | ^(s= SELECTION_STREAM IDENT ( IDENT )? ) );
    public void selectionListElement() // throws RecognitionException [1]
    {   
        CommonTree w = null;
        CommonTree e = null;
        CommonTree s = null;

        try 
    	{
            // EsperEPL2Ast.g:142:2: (w= WILDCARD_SELECT | ^(e= SELECTION_ELEMENT_EXPR valueExpr ( IDENT )? ) | ^(s= SELECTION_STREAM IDENT ( IDENT )? ) )
            int alt34 = 3;
            switch ( input.LA(1) ) 
            {
            case WILDCARD_SELECT:
            	{
                alt34 = 1;
                }
                break;
            case SELECTION_ELEMENT_EXPR:
            	{
                alt34 = 2;
                }
                break;
            case SELECTION_STREAM:
            	{
                alt34 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d34s0 =
            	        new NoViableAltException("", 34, 0, input);

            	    throw nvae_d34s0;
            }

            switch (alt34) 
            {
                case 1 :
                    // EsperEPL2Ast.g:142:4: w= WILDCARD_SELECT
                    {
                    	w=(CommonTree)Match(input,WILDCARD_SELECT,FOLLOW_WILDCARD_SELECT_in_selectionListElement620); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(w); 
                    	}

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:143:4: ^(e= SELECTION_ELEMENT_EXPR valueExpr ( IDENT )? )
                    {
                    	e=(CommonTree)Match(input,SELECTION_ELEMENT_EXPR,FOLLOW_SELECTION_ELEMENT_EXPR_in_selectionListElement630); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_selectionListElement632);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:143:41: ( IDENT )?
                    	int alt32 = 2;
                    	int LA32_0 = input.LA(1);

                    	if ( (LA32_0 == IDENT) )
                    	{
                    	    alt32 = 1;
                    	}
                    	switch (alt32) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:143:42: IDENT
                    	        {
                    	        	Match(input,IDENT,FOLLOW_IDENT_in_selectionListElement635); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(e); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:144:4: ^(s= SELECTION_STREAM IDENT ( IDENT )? )
                    {
                    	s=(CommonTree)Match(input,SELECTION_STREAM,FOLLOW_SELECTION_STREAM_in_selectionListElement649); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	Match(input,IDENT,FOLLOW_IDENT_in_selectionListElement651); if (state.failed) return ;
                    	// EsperEPL2Ast.g:144:31: ( IDENT )?
                    	int alt33 = 2;
                    	int LA33_0 = input.LA(1);

                    	if ( (LA33_0 == IDENT) )
                    	{
                    	    alt33 = 1;
                    	}
                    	switch (alt33) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:144:32: IDENT
                    	        {
                    	        	Match(input,IDENT,FOLLOW_IDENT_in_selectionListElement654); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(s); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "selectionListElement"


    // $ANTLR start "outerJoin"
    // EsperEPL2Ast.g:147:1: outerJoin : outerJoinIdent ;
    public void outerJoin() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:148:2: ( outerJoinIdent )
            // EsperEPL2Ast.g:148:4: outerJoinIdent
            {
            	PushFollow(FOLLOW_outerJoinIdent_in_outerJoin673);
            	outerJoinIdent();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "outerJoin"


    // $ANTLR start "outerJoinIdent"
    // EsperEPL2Ast.g:151:1: outerJoinIdent : ( ^(tl= LEFT_OUTERJOIN_EXPR eventPropertyExpr eventPropertyExpr ( eventPropertyExpr eventPropertyExpr )* ) | ^(tr= RIGHT_OUTERJOIN_EXPR eventPropertyExpr eventPropertyExpr ( eventPropertyExpr eventPropertyExpr )* ) | ^(tf= FULL_OUTERJOIN_EXPR eventPropertyExpr eventPropertyExpr ( eventPropertyExpr eventPropertyExpr )* ) );
    public void outerJoinIdent() // throws RecognitionException [1]
    {   
        CommonTree tl = null;
        CommonTree tr = null;
        CommonTree tf = null;

        try 
    	{
            // EsperEPL2Ast.g:152:2: ( ^(tl= LEFT_OUTERJOIN_EXPR eventPropertyExpr eventPropertyExpr ( eventPropertyExpr eventPropertyExpr )* ) | ^(tr= RIGHT_OUTERJOIN_EXPR eventPropertyExpr eventPropertyExpr ( eventPropertyExpr eventPropertyExpr )* ) | ^(tf= FULL_OUTERJOIN_EXPR eventPropertyExpr eventPropertyExpr ( eventPropertyExpr eventPropertyExpr )* ) )
            int alt38 = 3;
            switch ( input.LA(1) ) 
            {
            case LEFT_OUTERJOIN_EXPR:
            	{
                alt38 = 1;
                }
                break;
            case RIGHT_OUTERJOIN_EXPR:
            	{
                alt38 = 2;
                }
                break;
            case FULL_OUTERJOIN_EXPR:
            	{
                alt38 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d38s0 =
            	        new NoViableAltException("", 38, 0, input);

            	    throw nvae_d38s0;
            }

            switch (alt38) 
            {
                case 1 :
                    // EsperEPL2Ast.g:152:4: ^(tl= LEFT_OUTERJOIN_EXPR eventPropertyExpr eventPropertyExpr ( eventPropertyExpr eventPropertyExpr )* )
                    {
                    	tl=(CommonTree)Match(input,LEFT_OUTERJOIN_EXPR,FOLLOW_LEFT_OUTERJOIN_EXPR_in_outerJoinIdent687); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_eventPropertyExpr_in_outerJoinIdent689);
                    	eventPropertyExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_eventPropertyExpr_in_outerJoinIdent691);
                    	eventPropertyExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:152:65: ( eventPropertyExpr eventPropertyExpr )*
                    	do 
                    	{
                    	    int alt35 = 2;
                    	    int LA35_0 = input.LA(1);

                    	    if ( (LA35_0 == EVENT_PROP_EXPR) )
                    	    {
                    	        alt35 = 1;
                    	    }


                    	    switch (alt35) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:152:66: eventPropertyExpr eventPropertyExpr
                    			    {
                    			    	PushFollow(FOLLOW_eventPropertyExpr_in_outerJoinIdent694);
                    			    	eventPropertyExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;
                    			    	PushFollow(FOLLOW_eventPropertyExpr_in_outerJoinIdent696);
                    			    	eventPropertyExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop35;
                    	    }
                    	} while (true);

                    	loop35:
                    		;	// Stops C# compiler whining that label 'loop35' has no statements

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(tl); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:153:4: ^(tr= RIGHT_OUTERJOIN_EXPR eventPropertyExpr eventPropertyExpr ( eventPropertyExpr eventPropertyExpr )* )
                    {
                    	tr=(CommonTree)Match(input,RIGHT_OUTERJOIN_EXPR,FOLLOW_RIGHT_OUTERJOIN_EXPR_in_outerJoinIdent710); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_eventPropertyExpr_in_outerJoinIdent712);
                    	eventPropertyExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_eventPropertyExpr_in_outerJoinIdent714);
                    	eventPropertyExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:153:66: ( eventPropertyExpr eventPropertyExpr )*
                    	do 
                    	{
                    	    int alt36 = 2;
                    	    int LA36_0 = input.LA(1);

                    	    if ( (LA36_0 == EVENT_PROP_EXPR) )
                    	    {
                    	        alt36 = 1;
                    	    }


                    	    switch (alt36) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:153:67: eventPropertyExpr eventPropertyExpr
                    			    {
                    			    	PushFollow(FOLLOW_eventPropertyExpr_in_outerJoinIdent717);
                    			    	eventPropertyExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;
                    			    	PushFollow(FOLLOW_eventPropertyExpr_in_outerJoinIdent719);
                    			    	eventPropertyExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop36;
                    	    }
                    	} while (true);

                    	loop36:
                    		;	// Stops C# compiler whining that label 'loop36' has no statements

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(tr); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:154:4: ^(tf= FULL_OUTERJOIN_EXPR eventPropertyExpr eventPropertyExpr ( eventPropertyExpr eventPropertyExpr )* )
                    {
                    	tf=(CommonTree)Match(input,FULL_OUTERJOIN_EXPR,FOLLOW_FULL_OUTERJOIN_EXPR_in_outerJoinIdent733); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_eventPropertyExpr_in_outerJoinIdent735);
                    	eventPropertyExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_eventPropertyExpr_in_outerJoinIdent737);
                    	eventPropertyExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:154:65: ( eventPropertyExpr eventPropertyExpr )*
                    	do 
                    	{
                    	    int alt37 = 2;
                    	    int LA37_0 = input.LA(1);

                    	    if ( (LA37_0 == EVENT_PROP_EXPR) )
                    	    {
                    	        alt37 = 1;
                    	    }


                    	    switch (alt37) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:154:66: eventPropertyExpr eventPropertyExpr
                    			    {
                    			    	PushFollow(FOLLOW_eventPropertyExpr_in_outerJoinIdent740);
                    			    	eventPropertyExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;
                    			    	PushFollow(FOLLOW_eventPropertyExpr_in_outerJoinIdent742);
                    			    	eventPropertyExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop37;
                    	    }
                    	} while (true);

                    	loop37:
                    		;	// Stops C# compiler whining that label 'loop37' has no statements

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(tf); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "outerJoinIdent"


    // $ANTLR start "streamExpression"
    // EsperEPL2Ast.g:157:1: streamExpression : ^(v= STREAM_EXPR ( eventFilterExpr | patternInclusionExpression | databaseJoinExpression | methodJoinExpression ) ( viewListExpr )? ( IDENT )? ( UNIDIRECTIONAL )? ) ;
    public void streamExpression() // throws RecognitionException [1]
    {   
        CommonTree v = null;

        try 
    	{
            // EsperEPL2Ast.g:158:2: ( ^(v= STREAM_EXPR ( eventFilterExpr | patternInclusionExpression | databaseJoinExpression | methodJoinExpression ) ( viewListExpr )? ( IDENT )? ( UNIDIRECTIONAL )? ) )
            // EsperEPL2Ast.g:158:4: ^(v= STREAM_EXPR ( eventFilterExpr | patternInclusionExpression | databaseJoinExpression | methodJoinExpression ) ( viewListExpr )? ( IDENT )? ( UNIDIRECTIONAL )? )
            {
            	v=(CommonTree)Match(input,STREAM_EXPR,FOLLOW_STREAM_EXPR_in_streamExpression762); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// EsperEPL2Ast.g:158:20: ( eventFilterExpr | patternInclusionExpression | databaseJoinExpression | methodJoinExpression )
            	int alt39 = 4;
            	switch ( input.LA(1) ) 
            	{
            	case EVENT_FILTER_EXPR:
            		{
            	    alt39 = 1;
            	    }
            	    break;
            	case PATTERN_INCL_EXPR:
            		{
            	    alt39 = 2;
            	    }
            	    break;
            	case DATABASE_JOIN_EXPR:
            		{
            	    alt39 = 3;
            	    }
            	    break;
            	case METHOD_JOIN_EXPR:
            		{
            	    alt39 = 4;
            	    }
            	    break;
            		default:
            		    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            		    NoViableAltException nvae_d39s0 =
            		        new NoViableAltException("", 39, 0, input);

            		    throw nvae_d39s0;
            	}

            	switch (alt39) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:158:21: eventFilterExpr
            	        {
            	        	PushFollow(FOLLOW_eventFilterExpr_in_streamExpression765);
            	        	eventFilterExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 2 :
            	        // EsperEPL2Ast.g:158:39: patternInclusionExpression
            	        {
            	        	PushFollow(FOLLOW_patternInclusionExpression_in_streamExpression769);
            	        	patternInclusionExpression();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 3 :
            	        // EsperEPL2Ast.g:158:68: databaseJoinExpression
            	        {
            	        	PushFollow(FOLLOW_databaseJoinExpression_in_streamExpression773);
            	        	databaseJoinExpression();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;
            	    case 4 :
            	        // EsperEPL2Ast.g:158:93: methodJoinExpression
            	        {
            	        	PushFollow(FOLLOW_methodJoinExpression_in_streamExpression777);
            	        	methodJoinExpression();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:158:115: ( viewListExpr )?
            	int alt40 = 2;
            	int LA40_0 = input.LA(1);

            	if ( (LA40_0 == VIEW_EXPR) )
            	{
            	    alt40 = 1;
            	}
            	switch (alt40) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:158:116: viewListExpr
            	        {
            	        	PushFollow(FOLLOW_viewListExpr_in_streamExpression781);
            	        	viewListExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:158:131: ( IDENT )?
            	int alt41 = 2;
            	int LA41_0 = input.LA(1);

            	if ( (LA41_0 == IDENT) )
            	{
            	    alt41 = 1;
            	}
            	switch (alt41) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:158:132: IDENT
            	        {
            	        	Match(input,IDENT,FOLLOW_IDENT_in_streamExpression786); if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:158:140: ( UNIDIRECTIONAL )?
            	int alt42 = 2;
            	int LA42_0 = input.LA(1);

            	if ( (LA42_0 == UNIDIRECTIONAL) )
            	{
            	    alt42 = 1;
            	}
            	switch (alt42) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:158:141: UNIDIRECTIONAL
            	        {
            	        	Match(input,UNIDIRECTIONAL,FOLLOW_UNIDIRECTIONAL_in_streamExpression791); if (state.failed) return ;

            	        }
            	        break;

            	}

            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(v); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "streamExpression"


    // $ANTLR start "patternInclusionExpression"
    // EsperEPL2Ast.g:161:1: patternInclusionExpression : ^(p= PATTERN_INCL_EXPR exprChoice ) ;
    public void patternInclusionExpression() // throws RecognitionException [1]
    {   
        CommonTree p = null;

        try 
    	{
            // EsperEPL2Ast.g:162:2: ( ^(p= PATTERN_INCL_EXPR exprChoice ) )
            // EsperEPL2Ast.g:162:4: ^(p= PATTERN_INCL_EXPR exprChoice )
            {
            	p=(CommonTree)Match(input,PATTERN_INCL_EXPR,FOLLOW_PATTERN_INCL_EXPR_in_patternInclusionExpression811); if (state.failed) return ;

            	if ( state.backtracking == 0 ) 
            	{
            	   SetIsPatternWalk(true); 
            	}

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_exprChoice_in_patternInclusionExpression815);
            	exprChoice();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	   SetIsPatternWalk(false); LeaveNode(p); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "patternInclusionExpression"


    // $ANTLR start "databaseJoinExpression"
    // EsperEPL2Ast.g:165:1: databaseJoinExpression : ^( DATABASE_JOIN_EXPR IDENT ( STRING_LITERAL | QUOTED_STRING_LITERAL ) ( STRING_LITERAL | QUOTED_STRING_LITERAL )? ) ;
    public void databaseJoinExpression() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:166:2: ( ^( DATABASE_JOIN_EXPR IDENT ( STRING_LITERAL | QUOTED_STRING_LITERAL ) ( STRING_LITERAL | QUOTED_STRING_LITERAL )? ) )
            // EsperEPL2Ast.g:166:4: ^( DATABASE_JOIN_EXPR IDENT ( STRING_LITERAL | QUOTED_STRING_LITERAL ) ( STRING_LITERAL | QUOTED_STRING_LITERAL )? )
            {
            	Match(input,DATABASE_JOIN_EXPR,FOLLOW_DATABASE_JOIN_EXPR_in_databaseJoinExpression832); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	Match(input,IDENT,FOLLOW_IDENT_in_databaseJoinExpression834); if (state.failed) return ;
            	if ( (input.LA(1) >= STRING_LITERAL && input.LA(1) <= QUOTED_STRING_LITERAL) ) 
            	{
            	    input.Consume();
            	    state.errorRecovery = false;state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    throw mse;
            	}

            	// EsperEPL2Ast.g:166:72: ( STRING_LITERAL | QUOTED_STRING_LITERAL )?
            	int alt43 = 2;
            	int LA43_0 = input.LA(1);

            	if ( ((LA43_0 >= STRING_LITERAL && LA43_0 <= QUOTED_STRING_LITERAL)) )
            	{
            	    alt43 = 1;
            	}
            	switch (alt43) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:
            	        {
            	        	if ( (input.LA(1) >= STRING_LITERAL && input.LA(1) <= QUOTED_STRING_LITERAL) ) 
            	        	{
            	        	    input.Consume();
            	        	    state.errorRecovery = false;state.failed = false;
            	        	}
            	        	else 
            	        	{
            	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	        	    throw mse;
            	        	}


            	        }
            	        break;

            	}


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "databaseJoinExpression"


    // $ANTLR start "methodJoinExpression"
    // EsperEPL2Ast.g:169:1: methodJoinExpression : ^( METHOD_JOIN_EXPR IDENT CLASS_IDENT ( valueExpr )* ) ;
    public void methodJoinExpression() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:170:2: ( ^( METHOD_JOIN_EXPR IDENT CLASS_IDENT ( valueExpr )* ) )
            // EsperEPL2Ast.g:170:4: ^( METHOD_JOIN_EXPR IDENT CLASS_IDENT ( valueExpr )* )
            {
            	Match(input,METHOD_JOIN_EXPR,FOLLOW_METHOD_JOIN_EXPR_in_methodJoinExpression865); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	Match(input,IDENT,FOLLOW_IDENT_in_methodJoinExpression867); if (state.failed) return ;
            	Match(input,CLASS_IDENT,FOLLOW_CLASS_IDENT_in_methodJoinExpression869); if (state.failed) return ;
            	// EsperEPL2Ast.g:170:41: ( valueExpr )*
            	do 
            	{
            	    int alt44 = 2;
            	    alt44 = dfa44.Predict(input);
            	    switch (alt44) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:170:42: valueExpr
            			    {
            			    	PushFollow(FOLLOW_valueExpr_in_methodJoinExpression872);
            			    	valueExpr();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop44;
            	    }
            	} while (true);

            	loop44:
            		;	// Stops C# compiler whining that label 'loop44' has no statements


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "methodJoinExpression"


    // $ANTLR start "viewListExpr"
    // EsperEPL2Ast.g:173:1: viewListExpr : viewExpr ( viewExpr )* ;
    public void viewListExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:174:2: ( viewExpr ( viewExpr )* )
            // EsperEPL2Ast.g:174:4: viewExpr ( viewExpr )*
            {
            	PushFollow(FOLLOW_viewExpr_in_viewListExpr886);
            	viewExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:174:13: ( viewExpr )*
            	do 
            	{
            	    int alt45 = 2;
            	    int LA45_0 = input.LA(1);

            	    if ( (LA45_0 == VIEW_EXPR) )
            	    {
            	        alt45 = 1;
            	    }


            	    switch (alt45) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:174:14: viewExpr
            			    {
            			    	PushFollow(FOLLOW_viewExpr_in_viewListExpr889);
            			    	viewExpr();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop45;
            	    }
            	} while (true);

            	loop45:
            		;	// Stops C# compiler whining that label 'loop45' has no statements


            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "viewListExpr"


    // $ANTLR start "viewExpr"
    // EsperEPL2Ast.g:177:1: viewExpr : ^(n= VIEW_EXPR IDENT IDENT ( parameter )* ) ;
    public void viewExpr() // throws RecognitionException [1]
    {   
        CommonTree n = null;

        try 
    	{
            // EsperEPL2Ast.g:178:2: ( ^(n= VIEW_EXPR IDENT IDENT ( parameter )* ) )
            // EsperEPL2Ast.g:178:4: ^(n= VIEW_EXPR IDENT IDENT ( parameter )* )
            {
            	n=(CommonTree)Match(input,VIEW_EXPR,FOLLOW_VIEW_EXPR_in_viewExpr906); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	Match(input,IDENT,FOLLOW_IDENT_in_viewExpr908); if (state.failed) return ;
            	Match(input,IDENT,FOLLOW_IDENT_in_viewExpr910); if (state.failed) return ;
            	// EsperEPL2Ast.g:178:30: ( parameter )*
            	do 
            	{
            	    int alt46 = 2;
            	    alt46 = dfa46.Predict(input);
            	    switch (alt46) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:178:31: parameter
            			    {
            			    	PushFollow(FOLLOW_parameter_in_viewExpr913);
            			    	parameter();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop46;
            	    }
            	} while (true);

            	loop46:
            		;	// Stops C# compiler whining that label 'loop46' has no statements

            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(n); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "viewExpr"


    // $ANTLR start "whereClause"
    // EsperEPL2Ast.g:181:1: whereClause : ^(n= WHERE_EXPR valueExpr ) ;
    public void whereClause() // throws RecognitionException [1]
    {   
        CommonTree n = null;

        try 
    	{
            // EsperEPL2Ast.g:182:2: ( ^(n= WHERE_EXPR valueExpr ) )
            // EsperEPL2Ast.g:182:4: ^(n= WHERE_EXPR valueExpr )
            {
            	n=(CommonTree)Match(input,WHERE_EXPR,FOLLOW_WHERE_EXPR_in_whereClause934); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_valueExpr_in_whereClause936);
            	valueExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(n); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "whereClause"


    // $ANTLR start "groupByClause"
    // EsperEPL2Ast.g:185:1: groupByClause : ^(g= GROUP_BY_EXPR valueExpr ( valueExpr )* ) ;
    public void groupByClause() // throws RecognitionException [1]
    {   
        CommonTree g = null;

        try 
    	{
            // EsperEPL2Ast.g:186:2: ( ^(g= GROUP_BY_EXPR valueExpr ( valueExpr )* ) )
            // EsperEPL2Ast.g:186:4: ^(g= GROUP_BY_EXPR valueExpr ( valueExpr )* )
            {
            	g=(CommonTree)Match(input,GROUP_BY_EXPR,FOLLOW_GROUP_BY_EXPR_in_groupByClause954); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_valueExpr_in_groupByClause956);
            	valueExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:186:32: ( valueExpr )*
            	do 
            	{
            	    int alt47 = 2;
            	    alt47 = dfa47.Predict(input);
            	    switch (alt47) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:186:33: valueExpr
            			    {
            			    	PushFollow(FOLLOW_valueExpr_in_groupByClause959);
            			    	valueExpr();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop47;
            	    }
            	} while (true);

            	loop47:
            		;	// Stops C# compiler whining that label 'loop47' has no statements


            	Match(input, Token.UP, null); if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(g); 
            	}

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "groupByClause"


    // $ANTLR start "orderByClause"
    // EsperEPL2Ast.g:189:1: orderByClause : ^( ORDER_BY_EXPR orderByElement ( orderByElement )* ) ;
    public void orderByClause() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:190:2: ( ^( ORDER_BY_EXPR orderByElement ( orderByElement )* ) )
            // EsperEPL2Ast.g:190:4: ^( ORDER_BY_EXPR orderByElement ( orderByElement )* )
            {
            	Match(input,ORDER_BY_EXPR,FOLLOW_ORDER_BY_EXPR_in_orderByClause977); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_orderByElement_in_orderByClause979);
            	orderByElement();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:190:35: ( orderByElement )*
            	do 
            	{
            	    int alt48 = 2;
            	    int LA48_0 = input.LA(1);

            	    if ( (LA48_0 == ORDER_ELEMENT_EXPR) )
            	    {
            	        alt48 = 1;
            	    }


            	    switch (alt48) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:190:36: orderByElement
            			    {
            			    	PushFollow(FOLLOW_orderByElement_in_orderByClause982);
            			    	orderByElement();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop48;
            	    }
            	} while (true);

            	loop48:
            		;	// Stops C# compiler whining that label 'loop48' has no statements


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "orderByClause"


    // $ANTLR start "orderByElement"
    // EsperEPL2Ast.g:193:1: orderByElement : ^(e= ORDER_ELEMENT_EXPR valueExpr ( ASC | DESC )? ) ;
    public void orderByElement() // throws RecognitionException [1]
    {   
        CommonTree e = null;

        try 
    	{
            // EsperEPL2Ast.g:194:2: ( ^(e= ORDER_ELEMENT_EXPR valueExpr ( ASC | DESC )? ) )
            // EsperEPL2Ast.g:194:5: ^(e= ORDER_ELEMENT_EXPR valueExpr ( ASC | DESC )? )
            {
            	e=(CommonTree)Match(input,ORDER_ELEMENT_EXPR,FOLLOW_ORDER_ELEMENT_EXPR_in_orderByElement1002); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_valueExpr_in_orderByElement1004);
            	valueExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:194:38: ( ASC | DESC )?
            	int alt49 = 2;
            	int LA49_0 = input.LA(1);

            	if ( ((LA49_0 >= ASC && LA49_0 <= DESC)) )
            	{
            	    alt49 = 1;
            	}
            	switch (alt49) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:
            	        {
            	        	if ( (input.LA(1) >= ASC && input.LA(1) <= DESC) ) 
            	        	{
            	        	    input.Consume();
            	        	    state.errorRecovery = false;state.failed = false;
            	        	}
            	        	else 
            	        	{
            	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	        	    throw mse;
            	        	}


            	        }
            	        break;

            	}

            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(e); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "orderByElement"


    // $ANTLR start "havingClause"
    // EsperEPL2Ast.g:197:1: havingClause : ^(n= HAVING_EXPR valueExpr ) ;
    public void havingClause() // throws RecognitionException [1]
    {   
        CommonTree n = null;

        try 
    	{
            // EsperEPL2Ast.g:198:2: ( ^(n= HAVING_EXPR valueExpr ) )
            // EsperEPL2Ast.g:198:4: ^(n= HAVING_EXPR valueExpr )
            {
            	n=(CommonTree)Match(input,HAVING_EXPR,FOLLOW_HAVING_EXPR_in_havingClause1029); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_valueExpr_in_havingClause1031);
            	valueExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(n); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "havingClause"


    // $ANTLR start "outputLimitExpr"
    // EsperEPL2Ast.g:201:1: outputLimitExpr : ( ^(e= EVENT_LIMIT_EXPR ( ALL | FIRST | LAST | SNAPSHOT )? ( number | IDENT ) ) | ^(sec= SEC_LIMIT_EXPR ( ALL | FIRST | LAST | SNAPSHOT )? ( number | IDENT ) ) | ^(min= MIN_LIMIT_EXPR ( ALL | FIRST | LAST | SNAPSHOT )? ( number | IDENT ) ) | ^(tp= TIMEPERIOD_LIMIT_EXPR ( ALL | FIRST | LAST | SNAPSHOT )? time_period ) );
    public void outputLimitExpr() // throws RecognitionException [1]
    {   
        CommonTree e = null;
        CommonTree sec = null;
        CommonTree min = null;
        CommonTree tp = null;

        try 
    	{
            // EsperEPL2Ast.g:202:2: ( ^(e= EVENT_LIMIT_EXPR ( ALL | FIRST | LAST | SNAPSHOT )? ( number | IDENT ) ) | ^(sec= SEC_LIMIT_EXPR ( ALL | FIRST | LAST | SNAPSHOT )? ( number | IDENT ) ) | ^(min= MIN_LIMIT_EXPR ( ALL | FIRST | LAST | SNAPSHOT )? ( number | IDENT ) ) | ^(tp= TIMEPERIOD_LIMIT_EXPR ( ALL | FIRST | LAST | SNAPSHOT )? time_period ) )
            int alt57 = 4;
            switch ( input.LA(1) ) 
            {
            case EVENT_LIMIT_EXPR:
            	{
                alt57 = 1;
                }
                break;
            case SEC_LIMIT_EXPR:
            	{
                alt57 = 2;
                }
                break;
            case MIN_LIMIT_EXPR:
            	{
                alt57 = 3;
                }
                break;
            case TIMEPERIOD_LIMIT_EXPR:
            	{
                alt57 = 4;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d57s0 =
            	        new NoViableAltException("", 57, 0, input);

            	    throw nvae_d57s0;
            }

            switch (alt57) 
            {
                case 1 :
                    // EsperEPL2Ast.g:202:4: ^(e= EVENT_LIMIT_EXPR ( ALL | FIRST | LAST | SNAPSHOT )? ( number | IDENT ) )
                    {
                    	e=(CommonTree)Match(input,EVENT_LIMIT_EXPR,FOLLOW_EVENT_LIMIT_EXPR_in_outputLimitExpr1049); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// EsperEPL2Ast.g:202:25: ( ALL | FIRST | LAST | SNAPSHOT )?
                    	int alt50 = 2;
                    	int LA50_0 = input.LA(1);

                    	if ( (LA50_0 == ALL || (LA50_0 >= FIRST && LA50_0 <= LAST) || LA50_0 == SNAPSHOT) )
                    	{
                    	    alt50 = 1;
                    	}
                    	switch (alt50) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:
                    	        {
                    	        	if ( input.LA(1) == ALL || (input.LA(1) >= FIRST && input.LA(1) <= LAST) || input.LA(1) == SNAPSHOT ) 
                    	        	{
                    	        	    input.Consume();
                    	        	    state.errorRecovery = false;state.failed = false;
                    	        	}
                    	        	else 
                    	        	{
                    	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        	    throw mse;
                    	        	}


                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:202:52: ( number | IDENT )
                    	int alt51 = 2;
                    	int LA51_0 = input.LA(1);

                    	if ( ((LA51_0 >= INT_TYPE && LA51_0 <= DOUBLE_TYPE)) )
                    	{
                    	    alt51 = 1;
                    	}
                    	else if ( (LA51_0 == IDENT) )
                    	{
                    	    alt51 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d51s0 =
                    	        new NoViableAltException("", 51, 0, input);

                    	    throw nvae_d51s0;
                    	}
                    	switch (alt51) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:202:53: number
                    	        {
                    	        	PushFollow(FOLLOW_number_in_outputLimitExpr1063);
                    	        	number();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:202:60: IDENT
                    	        {
                    	        	Match(input,IDENT,FOLLOW_IDENT_in_outputLimitExpr1065); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(e); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:203:7: ^(sec= SEC_LIMIT_EXPR ( ALL | FIRST | LAST | SNAPSHOT )? ( number | IDENT ) )
                    {
                    	sec=(CommonTree)Match(input,SEC_LIMIT_EXPR,FOLLOW_SEC_LIMIT_EXPR_in_outputLimitExpr1082); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// EsperEPL2Ast.g:203:28: ( ALL | FIRST | LAST | SNAPSHOT )?
                    	int alt52 = 2;
                    	int LA52_0 = input.LA(1);

                    	if ( (LA52_0 == ALL || (LA52_0 >= FIRST && LA52_0 <= LAST) || LA52_0 == SNAPSHOT) )
                    	{
                    	    alt52 = 1;
                    	}
                    	switch (alt52) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:
                    	        {
                    	        	if ( input.LA(1) == ALL || (input.LA(1) >= FIRST && input.LA(1) <= LAST) || input.LA(1) == SNAPSHOT ) 
                    	        	{
                    	        	    input.Consume();
                    	        	    state.errorRecovery = false;state.failed = false;
                    	        	}
                    	        	else 
                    	        	{
                    	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        	    throw mse;
                    	        	}


                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:203:55: ( number | IDENT )
                    	int alt53 = 2;
                    	int LA53_0 = input.LA(1);

                    	if ( ((LA53_0 >= INT_TYPE && LA53_0 <= DOUBLE_TYPE)) )
                    	{
                    	    alt53 = 1;
                    	}
                    	else if ( (LA53_0 == IDENT) )
                    	{
                    	    alt53 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d53s0 =
                    	        new NoViableAltException("", 53, 0, input);

                    	    throw nvae_d53s0;
                    	}
                    	switch (alt53) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:203:56: number
                    	        {
                    	        	PushFollow(FOLLOW_number_in_outputLimitExpr1096);
                    	        	number();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:203:63: IDENT
                    	        {
                    	        	Match(input,IDENT,FOLLOW_IDENT_in_outputLimitExpr1098); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(sec); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:204:7: ^(min= MIN_LIMIT_EXPR ( ALL | FIRST | LAST | SNAPSHOT )? ( number | IDENT ) )
                    {
                    	min=(CommonTree)Match(input,MIN_LIMIT_EXPR,FOLLOW_MIN_LIMIT_EXPR_in_outputLimitExpr1114); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// EsperEPL2Ast.g:204:28: ( ALL | FIRST | LAST | SNAPSHOT )?
                    	int alt54 = 2;
                    	int LA54_0 = input.LA(1);

                    	if ( (LA54_0 == ALL || (LA54_0 >= FIRST && LA54_0 <= LAST) || LA54_0 == SNAPSHOT) )
                    	{
                    	    alt54 = 1;
                    	}
                    	switch (alt54) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:
                    	        {
                    	        	if ( input.LA(1) == ALL || (input.LA(1) >= FIRST && input.LA(1) <= LAST) || input.LA(1) == SNAPSHOT ) 
                    	        	{
                    	        	    input.Consume();
                    	        	    state.errorRecovery = false;state.failed = false;
                    	        	}
                    	        	else 
                    	        	{
                    	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        	    throw mse;
                    	        	}


                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:204:55: ( number | IDENT )
                    	int alt55 = 2;
                    	int LA55_0 = input.LA(1);

                    	if ( ((LA55_0 >= INT_TYPE && LA55_0 <= DOUBLE_TYPE)) )
                    	{
                    	    alt55 = 1;
                    	}
                    	else if ( (LA55_0 == IDENT) )
                    	{
                    	    alt55 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d55s0 =
                    	        new NoViableAltException("", 55, 0, input);

                    	    throw nvae_d55s0;
                    	}
                    	switch (alt55) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:204:56: number
                    	        {
                    	        	PushFollow(FOLLOW_number_in_outputLimitExpr1128);
                    	        	number();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:204:63: IDENT
                    	        {
                    	        	Match(input,IDENT,FOLLOW_IDENT_in_outputLimitExpr1130); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(min); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:205:7: ^(tp= TIMEPERIOD_LIMIT_EXPR ( ALL | FIRST | LAST | SNAPSHOT )? time_period )
                    {
                    	tp=(CommonTree)Match(input,TIMEPERIOD_LIMIT_EXPR,FOLLOW_TIMEPERIOD_LIMIT_EXPR_in_outputLimitExpr1146); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// EsperEPL2Ast.g:205:34: ( ALL | FIRST | LAST | SNAPSHOT )?
                    	int alt56 = 2;
                    	int LA56_0 = input.LA(1);

                    	if ( (LA56_0 == ALL || (LA56_0 >= FIRST && LA56_0 <= LAST) || LA56_0 == SNAPSHOT) )
                    	{
                    	    alt56 = 1;
                    	}
                    	switch (alt56) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:
                    	        {
                    	        	if ( input.LA(1) == ALL || (input.LA(1) >= FIRST && input.LA(1) <= LAST) || input.LA(1) == SNAPSHOT ) 
                    	        	{
                    	        	    input.Consume();
                    	        	    state.errorRecovery = false;state.failed = false;
                    	        	}
                    	        	else 
                    	        	{
                    	        	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        	    throw mse;
                    	        	}


                    	        }
                    	        break;

                    	}

                    	PushFollow(FOLLOW_time_period_in_outputLimitExpr1159);
                    	time_period();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(tp); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "outputLimitExpr"


    // $ANTLR start "relationalExpr"
    // EsperEPL2Ast.g:208:1: relationalExpr : ( ^(n= LT valueExpr valueExpr ) | ^(n= GT valueExpr valueExpr ) | ^(n= LE valueExpr valueExpr ) | ^(n= GE valueExpr valueExpr ) );
    public void relationalExpr() // throws RecognitionException [1]
    {   
        CommonTree n = null;

        try 
    	{
            // EsperEPL2Ast.g:209:2: ( ^(n= LT valueExpr valueExpr ) | ^(n= GT valueExpr valueExpr ) | ^(n= LE valueExpr valueExpr ) | ^(n= GE valueExpr valueExpr ) )
            int alt58 = 4;
            switch ( input.LA(1) ) 
            {
            case LT:
            	{
                alt58 = 1;
                }
                break;
            case GT:
            	{
                alt58 = 2;
                }
                break;
            case LE:
            	{
                alt58 = 3;
                }
                break;
            case GE:
            	{
                alt58 = 4;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d58s0 =
            	        new NoViableAltException("", 58, 0, input);

            	    throw nvae_d58s0;
            }

            switch (alt58) 
            {
                case 1 :
                    // EsperEPL2Ast.g:209:5: ^(n= LT valueExpr valueExpr )
                    {
                    	n=(CommonTree)Match(input,LT,FOLLOW_LT_in_relationalExpr1178); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_relationalExpr1180);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_relationalExpr1182);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(n); 
                    	}

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:210:5: ^(n= GT valueExpr valueExpr )
                    {
                    	n=(CommonTree)Match(input,GT,FOLLOW_GT_in_relationalExpr1194); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_relationalExpr1196);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_relationalExpr1198);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(n); 
                    	}

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:211:5: ^(n= LE valueExpr valueExpr )
                    {
                    	n=(CommonTree)Match(input,LE,FOLLOW_LE_in_relationalExpr1210); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_relationalExpr1212);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_relationalExpr1214);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(n); 
                    	}

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:212:4: ^(n= GE valueExpr valueExpr )
                    {
                    	n=(CommonTree)Match(input,GE,FOLLOW_GE_in_relationalExpr1225); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_relationalExpr1227);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_relationalExpr1229);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(n); 
                    	}

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "relationalExpr"


    // $ANTLR start "evalExprChoice"
    // EsperEPL2Ast.g:215:1: evalExprChoice : ( ^(jo= EVAL_OR_EXPR valueExpr valueExpr ( valueExpr )* ) | ^(ja= EVAL_AND_EXPR valueExpr valueExpr ( valueExpr )* ) | ^(je= EVAL_EQUALS_EXPR valueExpr valueExpr ) | ^(jne= EVAL_NOTEQUALS_EXPR valueExpr valueExpr ) | ^(n= NOT_EXPR valueExpr ) | r= relationalExpr );
    public void evalExprChoice() // throws RecognitionException [1]
    {   
        CommonTree jo = null;
        CommonTree ja = null;
        CommonTree je = null;
        CommonTree jne = null;
        CommonTree n = null;

        try 
    	{
            // EsperEPL2Ast.g:216:2: ( ^(jo= EVAL_OR_EXPR valueExpr valueExpr ( valueExpr )* ) | ^(ja= EVAL_AND_EXPR valueExpr valueExpr ( valueExpr )* ) | ^(je= EVAL_EQUALS_EXPR valueExpr valueExpr ) | ^(jne= EVAL_NOTEQUALS_EXPR valueExpr valueExpr ) | ^(n= NOT_EXPR valueExpr ) | r= relationalExpr )
            int alt61 = 6;
            alt61 = dfa61.Predict(input);
            switch (alt61) 
            {
                case 1 :
                    // EsperEPL2Ast.g:216:4: ^(jo= EVAL_OR_EXPR valueExpr valueExpr ( valueExpr )* )
                    {
                    	jo=(CommonTree)Match(input,EVAL_OR_EXPR,FOLLOW_EVAL_OR_EXPR_in_evalExprChoice1246); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_evalExprChoice1248);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_evalExprChoice1250);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:216:42: ( valueExpr )*
                    	do 
                    	{
                    	    int alt59 = 2;
                    	    alt59 = dfa59.Predict(input);
                    	    switch (alt59) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:216:43: valueExpr
                    			    {
                    			    	PushFollow(FOLLOW_valueExpr_in_evalExprChoice1253);
                    			    	valueExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop59;
                    	    }
                    	} while (true);

                    	loop59:
                    		;	// Stops C# compiler whining that label 'loop59' has no statements

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(jo); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:217:4: ^(ja= EVAL_AND_EXPR valueExpr valueExpr ( valueExpr )* )
                    {
                    	ja=(CommonTree)Match(input,EVAL_AND_EXPR,FOLLOW_EVAL_AND_EXPR_in_evalExprChoice1267); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_evalExprChoice1269);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_evalExprChoice1271);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:217:43: ( valueExpr )*
                    	do 
                    	{
                    	    int alt60 = 2;
                    	    alt60 = dfa60.Predict(input);
                    	    switch (alt60) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:217:44: valueExpr
                    			    {
                    			    	PushFollow(FOLLOW_valueExpr_in_evalExprChoice1274);
                    			    	valueExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop60;
                    	    }
                    	} while (true);

                    	loop60:
                    		;	// Stops C# compiler whining that label 'loop60' has no statements

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(ja); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:218:4: ^(je= EVAL_EQUALS_EXPR valueExpr valueExpr )
                    {
                    	je=(CommonTree)Match(input,EVAL_EQUALS_EXPR,FOLLOW_EVAL_EQUALS_EXPR_in_evalExprChoice1288); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_evalExprChoice1290);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_evalExprChoice1292);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(je); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:219:4: ^(jne= EVAL_NOTEQUALS_EXPR valueExpr valueExpr )
                    {
                    	jne=(CommonTree)Match(input,EVAL_NOTEQUALS_EXPR,FOLLOW_EVAL_NOTEQUALS_EXPR_in_evalExprChoice1304); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_evalExprChoice1306);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_evalExprChoice1308);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(jne); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // EsperEPL2Ast.g:220:4: ^(n= NOT_EXPR valueExpr )
                    {
                    	n=(CommonTree)Match(input,NOT_EXPR,FOLLOW_NOT_EXPR_in_evalExprChoice1320); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_evalExprChoice1322);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(n); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 6 :
                    // EsperEPL2Ast.g:221:4: r= relationalExpr
                    {
                    	PushFollow(FOLLOW_relationalExpr_in_evalExprChoice1333);
                    	relationalExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "evalExprChoice"


    // $ANTLR start "valueExpr"
    // EsperEPL2Ast.g:224:1: valueExpr : ( constant[true] | substitution | arithmeticExpr | eventPropertyExpr | evalExprChoice | builtinFunc | libFunc | caseExpr | inExpr | betweenExpr | likeExpr | regExpExpr | arrayExpr | subSelectInExpr | subSelectRowExpr | subSelectExistsExpr );
    public void valueExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:225:2: ( constant[true] | substitution | arithmeticExpr | eventPropertyExpr | evalExprChoice | builtinFunc | libFunc | caseExpr | inExpr | betweenExpr | likeExpr | regExpExpr | arrayExpr | subSelectInExpr | subSelectRowExpr | subSelectExistsExpr )
            int alt62 = 16;
            alt62 = dfa62.Predict(input);
            switch (alt62) 
            {
                case 1 :
                    // EsperEPL2Ast.g:225:5: constant[true]
                    {
                    	PushFollow(FOLLOW_constant_in_valueExpr1346);
                    	constant(true);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:226:4: substitution
                    {
                    	PushFollow(FOLLOW_substitution_in_valueExpr1352);
                    	substitution();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:227:5: arithmeticExpr
                    {
                    	PushFollow(FOLLOW_arithmeticExpr_in_valueExpr1358);
                    	arithmeticExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:228:5: eventPropertyExpr
                    {
                    	PushFollow(FOLLOW_eventPropertyExpr_in_valueExpr1365);
                    	eventPropertyExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // EsperEPL2Ast.g:229:7: evalExprChoice
                    {
                    	PushFollow(FOLLOW_evalExprChoice_in_valueExpr1373);
                    	evalExprChoice();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 6 :
                    // EsperEPL2Ast.g:230:4: builtinFunc
                    {
                    	PushFollow(FOLLOW_builtinFunc_in_valueExpr1378);
                    	builtinFunc();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 7 :
                    // EsperEPL2Ast.g:231:7: libFunc
                    {
                    	PushFollow(FOLLOW_libFunc_in_valueExpr1386);
                    	libFunc();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 8 :
                    // EsperEPL2Ast.g:232:4: caseExpr
                    {
                    	PushFollow(FOLLOW_caseExpr_in_valueExpr1391);
                    	caseExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 9 :
                    // EsperEPL2Ast.g:233:4: inExpr
                    {
                    	PushFollow(FOLLOW_inExpr_in_valueExpr1396);
                    	inExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 10 :
                    // EsperEPL2Ast.g:234:4: betweenExpr
                    {
                    	PushFollow(FOLLOW_betweenExpr_in_valueExpr1402);
                    	betweenExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 11 :
                    // EsperEPL2Ast.g:235:4: likeExpr
                    {
                    	PushFollow(FOLLOW_likeExpr_in_valueExpr1407);
                    	likeExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 12 :
                    // EsperEPL2Ast.g:236:4: regExpExpr
                    {
                    	PushFollow(FOLLOW_regExpExpr_in_valueExpr1412);
                    	regExpExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 13 :
                    // EsperEPL2Ast.g:237:4: arrayExpr
                    {
                    	PushFollow(FOLLOW_arrayExpr_in_valueExpr1417);
                    	arrayExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 14 :
                    // EsperEPL2Ast.g:238:4: subSelectInExpr
                    {
                    	PushFollow(FOLLOW_subSelectInExpr_in_valueExpr1422);
                    	subSelectInExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 15 :
                    // EsperEPL2Ast.g:239:5: subSelectRowExpr
                    {
                    	PushFollow(FOLLOW_subSelectRowExpr_in_valueExpr1428);
                    	subSelectRowExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 16 :
                    // EsperEPL2Ast.g:240:5: subSelectExistsExpr
                    {
                    	PushFollow(FOLLOW_subSelectExistsExpr_in_valueExpr1435);
                    	subSelectExistsExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "valueExpr"


    // $ANTLR start "subSelectRowExpr"
    // EsperEPL2Ast.g:243:1: subSelectRowExpr : ^(s= SUBSELECT_EXPR subQueryExpr ) ;
    public void subSelectRowExpr() // throws RecognitionException [1]
    {   
        CommonTree s = null;

        try 
    	{
            // EsperEPL2Ast.g:244:2: ( ^(s= SUBSELECT_EXPR subQueryExpr ) )
            // EsperEPL2Ast.g:244:4: ^(s= SUBSELECT_EXPR subQueryExpr )
            {
            	if ( state.backtracking == 0 ) 
            	{
            	  PushStmtContext();
            	}
            	s=(CommonTree)Match(input,SUBSELECT_EXPR,FOLLOW_SUBSELECT_EXPR_in_subSelectRowExpr1451); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_subQueryExpr_in_subSelectRowExpr1453);
            	subQueryExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	  LeaveNode(s);
            	}

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "subSelectRowExpr"


    // $ANTLR start "subSelectExistsExpr"
    // EsperEPL2Ast.g:247:1: subSelectExistsExpr : ^(e= EXISTS_SUBSELECT_EXPR subQueryExpr ) ;
    public void subSelectExistsExpr() // throws RecognitionException [1]
    {   
        CommonTree e = null;

        try 
    	{
            // EsperEPL2Ast.g:248:2: ( ^(e= EXISTS_SUBSELECT_EXPR subQueryExpr ) )
            // EsperEPL2Ast.g:248:4: ^(e= EXISTS_SUBSELECT_EXPR subQueryExpr )
            {
            	if ( state.backtracking == 0 ) 
            	{
            	  PushStmtContext();
            	}
            	e=(CommonTree)Match(input,EXISTS_SUBSELECT_EXPR,FOLLOW_EXISTS_SUBSELECT_EXPR_in_subSelectExistsExpr1472); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_subQueryExpr_in_subSelectExistsExpr1474);
            	subQueryExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	  LeaveNode(e);
            	}

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "subSelectExistsExpr"


    // $ANTLR start "subSelectInExpr"
    // EsperEPL2Ast.g:251:1: subSelectInExpr : ( ^(s= IN_SUBSELECT_EXPR valueExpr subSelectInQueryExpr ) | ^(s= NOT_IN_SUBSELECT_EXPR valueExpr subSelectInQueryExpr ) );
    public void subSelectInExpr() // throws RecognitionException [1]
    {   
        CommonTree s = null;

        try 
    	{
            // EsperEPL2Ast.g:252:2: ( ^(s= IN_SUBSELECT_EXPR valueExpr subSelectInQueryExpr ) | ^(s= NOT_IN_SUBSELECT_EXPR valueExpr subSelectInQueryExpr ) )
            int alt63 = 2;
            int LA63_0 = input.LA(1);

            if ( (LA63_0 == IN_SUBSELECT_EXPR) )
            {
                alt63 = 1;
            }
            else if ( (LA63_0 == NOT_IN_SUBSELECT_EXPR) )
            {
                alt63 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d63s0 =
                    new NoViableAltException("", 63, 0, input);

                throw nvae_d63s0;
            }
            switch (alt63) 
            {
                case 1 :
                    // EsperEPL2Ast.g:252:5: ^(s= IN_SUBSELECT_EXPR valueExpr subSelectInQueryExpr )
                    {
                    	s=(CommonTree)Match(input,IN_SUBSELECT_EXPR,FOLLOW_IN_SUBSELECT_EXPR_in_subSelectInExpr1493); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_subSelectInExpr1495);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_subSelectInQueryExpr_in_subSelectInExpr1497);
                    	subSelectInQueryExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(s); 
                    	}

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:253:5: ^(s= NOT_IN_SUBSELECT_EXPR valueExpr subSelectInQueryExpr )
                    {
                    	s=(CommonTree)Match(input,NOT_IN_SUBSELECT_EXPR,FOLLOW_NOT_IN_SUBSELECT_EXPR_in_subSelectInExpr1509); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_subSelectInExpr1511);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_subSelectInQueryExpr_in_subSelectInExpr1513);
                    	subSelectInQueryExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(s); 
                    	}

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "subSelectInExpr"


    // $ANTLR start "subSelectInQueryExpr"
    // EsperEPL2Ast.g:256:1: subSelectInQueryExpr : ^(i= IN_SUBSELECT_QUERY_EXPR subQueryExpr ) ;
    public void subSelectInQueryExpr() // throws RecognitionException [1]
    {   
        CommonTree i = null;

        try 
    	{
            // EsperEPL2Ast.g:257:2: ( ^(i= IN_SUBSELECT_QUERY_EXPR subQueryExpr ) )
            // EsperEPL2Ast.g:257:4: ^(i= IN_SUBSELECT_QUERY_EXPR subQueryExpr )
            {
            	if ( state.backtracking == 0 ) 
            	{
            	  PushStmtContext();
            	}
            	i=(CommonTree)Match(input,IN_SUBSELECT_QUERY_EXPR,FOLLOW_IN_SUBSELECT_QUERY_EXPR_in_subSelectInQueryExpr1532); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_subQueryExpr_in_subSelectInQueryExpr1534);
            	subQueryExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	  LeaveNode(i);
            	}

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "subSelectInQueryExpr"


    // $ANTLR start "subQueryExpr"
    // EsperEPL2Ast.g:260:1: subQueryExpr : selectionListElement subSelectFilterExpr ( viewExpr )* ( IDENT )? ( whereClause )? ;
    public void subQueryExpr() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:261:2: ( selectionListElement subSelectFilterExpr ( viewExpr )* ( IDENT )? ( whereClause )? )
            // EsperEPL2Ast.g:261:4: selectionListElement subSelectFilterExpr ( viewExpr )* ( IDENT )? ( whereClause )?
            {
            	PushFollow(FOLLOW_selectionListElement_in_subQueryExpr1550);
            	selectionListElement();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	PushFollow(FOLLOW_subSelectFilterExpr_in_subQueryExpr1552);
            	subSelectFilterExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:261:45: ( viewExpr )*
            	do 
            	{
            	    int alt64 = 2;
            	    int LA64_0 = input.LA(1);

            	    if ( (LA64_0 == VIEW_EXPR) )
            	    {
            	        alt64 = 1;
            	    }


            	    switch (alt64) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:261:46: viewExpr
            			    {
            			    	PushFollow(FOLLOW_viewExpr_in_subQueryExpr1555);
            			    	viewExpr();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop64;
            	    }
            	} while (true);

            	loop64:
            		;	// Stops C# compiler whining that label 'loop64' has no statements

            	// EsperEPL2Ast.g:261:57: ( IDENT )?
            	int alt65 = 2;
            	int LA65_0 = input.LA(1);

            	if ( (LA65_0 == IDENT) )
            	{
            	    alt65 = 1;
            	}
            	switch (alt65) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:261:58: IDENT
            	        {
            	        	Match(input,IDENT,FOLLOW_IDENT_in_subQueryExpr1560); if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:261:66: ( whereClause )?
            	int alt66 = 2;
            	int LA66_0 = input.LA(1);

            	if ( (LA66_0 == WHERE_EXPR) )
            	{
            	    alt66 = 1;
            	}
            	switch (alt66) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:261:67: whereClause
            	        {
            	        	PushFollow(FOLLOW_whereClause_in_subQueryExpr1565);
            	        	whereClause();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}


            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "subQueryExpr"


    // $ANTLR start "subSelectFilterExpr"
    // EsperEPL2Ast.g:264:1: subSelectFilterExpr : ^(v= STREAM_EXPR eventFilterExpr ( viewListExpr )? ( IDENT )? ) ;
    public void subSelectFilterExpr() // throws RecognitionException [1]
    {   
        CommonTree v = null;

        try 
    	{
            // EsperEPL2Ast.g:265:2: ( ^(v= STREAM_EXPR eventFilterExpr ( viewListExpr )? ( IDENT )? ) )
            // EsperEPL2Ast.g:265:4: ^(v= STREAM_EXPR eventFilterExpr ( viewListExpr )? ( IDENT )? )
            {
            	v=(CommonTree)Match(input,STREAM_EXPR,FOLLOW_STREAM_EXPR_in_subSelectFilterExpr1582); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_eventFilterExpr_in_subSelectFilterExpr1584);
            	eventFilterExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:265:36: ( viewListExpr )?
            	int alt67 = 2;
            	int LA67_0 = input.LA(1);

            	if ( (LA67_0 == VIEW_EXPR) )
            	{
            	    alt67 = 1;
            	}
            	switch (alt67) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:265:37: viewListExpr
            	        {
            	        	PushFollow(FOLLOW_viewListExpr_in_subSelectFilterExpr1587);
            	        	viewListExpr();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:265:52: ( IDENT )?
            	int alt68 = 2;
            	int LA68_0 = input.LA(1);

            	if ( (LA68_0 == IDENT) )
            	{
            	    alt68 = 1;
            	}
            	switch (alt68) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:265:53: IDENT
            	        {
            	        	Match(input,IDENT,FOLLOW_IDENT_in_subSelectFilterExpr1592); if (state.failed) return ;

            	        }
            	        break;

            	}

            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(v); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "subSelectFilterExpr"


    // $ANTLR start "caseExpr"
    // EsperEPL2Ast.g:268:1: caseExpr : ( ^(c= CASE ( valueExpr )* ) | ^(c= CASE2 ( valueExpr )* ) );
    public void caseExpr() // throws RecognitionException [1]
    {   
        CommonTree c = null;

        try 
    	{
            // EsperEPL2Ast.g:269:2: ( ^(c= CASE ( valueExpr )* ) | ^(c= CASE2 ( valueExpr )* ) )
            int alt71 = 2;
            int LA71_0 = input.LA(1);

            if ( (LA71_0 == CASE) )
            {
                alt71 = 1;
            }
            else if ( (LA71_0 == CASE2) )
            {
                alt71 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d71s0 =
                    new NoViableAltException("", 71, 0, input);

                throw nvae_d71s0;
            }
            switch (alt71) 
            {
                case 1 :
                    // EsperEPL2Ast.g:269:4: ^(c= CASE ( valueExpr )* )
                    {
                    	c=(CommonTree)Match(input,CASE,FOLLOW_CASE_in_caseExpr1613); if (state.failed) return ;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // EsperEPL2Ast.g:269:13: ( valueExpr )*
                    	    do 
                    	    {
                    	        int alt69 = 2;
                    	        alt69 = dfa69.Predict(input);
                    	        switch (alt69) 
                    	    	{
                    	    		case 1 :
                    	    		    // EsperEPL2Ast.g:269:14: valueExpr
                    	    		    {
                    	    		    	PushFollow(FOLLOW_valueExpr_in_caseExpr1616);
                    	    		    	valueExpr();
                    	    		    	state.followingStackPointer--;
                    	    		    	if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop69;
                    	        }
                    	    } while (true);

                    	    loop69:
                    	    	;	// Stops C# compiler whining that label 'loop69' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(c); 
                    	}

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:270:4: ^(c= CASE2 ( valueExpr )* )
                    {
                    	c=(CommonTree)Match(input,CASE2,FOLLOW_CASE2_in_caseExpr1629); if (state.failed) return ;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // EsperEPL2Ast.g:270:14: ( valueExpr )*
                    	    do 
                    	    {
                    	        int alt70 = 2;
                    	        alt70 = dfa70.Predict(input);
                    	        switch (alt70) 
                    	    	{
                    	    		case 1 :
                    	    		    // EsperEPL2Ast.g:270:15: valueExpr
                    	    		    {
                    	    		    	PushFollow(FOLLOW_valueExpr_in_caseExpr1632);
                    	    		    	valueExpr();
                    	    		    	state.followingStackPointer--;
                    	    		    	if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop70;
                    	        }
                    	    } while (true);

                    	    loop70:
                    	    	;	// Stops C# compiler whining that label 'loop70' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(c); 
                    	}

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "caseExpr"


    // $ANTLR start "inExpr"
    // EsperEPL2Ast.g:273:1: inExpr : ( ^(i= IN_SET valueExpr ( LPAREN | LBRACK ) valueExpr ( valueExpr )* ( RPAREN | RBRACK ) ) | ^(i= NOT_IN_SET valueExpr ( LPAREN | LBRACK ) valueExpr ( valueExpr )* ( RPAREN | RBRACK ) ) | ^(i= IN_RANGE valueExpr ( LPAREN | LBRACK ) valueExpr valueExpr ( RPAREN | RBRACK ) ) | ^(i= NOT_IN_RANGE valueExpr ( LPAREN | LBRACK ) valueExpr valueExpr ( RPAREN | RBRACK ) ) );
    public void inExpr() // throws RecognitionException [1]
    {   
        CommonTree i = null;

        try 
    	{
            // EsperEPL2Ast.g:274:2: ( ^(i= IN_SET valueExpr ( LPAREN | LBRACK ) valueExpr ( valueExpr )* ( RPAREN | RBRACK ) ) | ^(i= NOT_IN_SET valueExpr ( LPAREN | LBRACK ) valueExpr ( valueExpr )* ( RPAREN | RBRACK ) ) | ^(i= IN_RANGE valueExpr ( LPAREN | LBRACK ) valueExpr valueExpr ( RPAREN | RBRACK ) ) | ^(i= NOT_IN_RANGE valueExpr ( LPAREN | LBRACK ) valueExpr valueExpr ( RPAREN | RBRACK ) ) )
            int alt74 = 4;
            switch ( input.LA(1) ) 
            {
            case IN_SET:
            	{
                alt74 = 1;
                }
                break;
            case NOT_IN_SET:
            	{
                alt74 = 2;
                }
                break;
            case IN_RANGE:
            	{
                alt74 = 3;
                }
                break;
            case NOT_IN_RANGE:
            	{
                alt74 = 4;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d74s0 =
            	        new NoViableAltException("", 74, 0, input);

            	    throw nvae_d74s0;
            }

            switch (alt74) 
            {
                case 1 :
                    // EsperEPL2Ast.g:274:4: ^(i= IN_SET valueExpr ( LPAREN | LBRACK ) valueExpr ( valueExpr )* ( RPAREN | RBRACK ) )
                    {
                    	i=(CommonTree)Match(input,IN_SET,FOLLOW_IN_SET_in_inExpr1652); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_inExpr1654);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( input.LA(1) == LPAREN || input.LA(1) == LBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}

                    	PushFollow(FOLLOW_valueExpr_in_inExpr1662);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:274:51: ( valueExpr )*
                    	do 
                    	{
                    	    int alt72 = 2;
                    	    alt72 = dfa72.Predict(input);
                    	    switch (alt72) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:274:52: valueExpr
                    			    {
                    			    	PushFollow(FOLLOW_valueExpr_in_inExpr1665);
                    			    	valueExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop72;
                    	    }
                    	} while (true);

                    	loop72:
                    		;	// Stops C# compiler whining that label 'loop72' has no statements

                    	if ( input.LA(1) == RPAREN || input.LA(1) == RBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(i); 
                    	}

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:275:4: ^(i= NOT_IN_SET valueExpr ( LPAREN | LBRACK ) valueExpr ( valueExpr )* ( RPAREN | RBRACK ) )
                    {
                    	i=(CommonTree)Match(input,NOT_IN_SET,FOLLOW_NOT_IN_SET_in_inExpr1684); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_inExpr1686);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( input.LA(1) == LPAREN || input.LA(1) == LBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}

                    	PushFollow(FOLLOW_valueExpr_in_inExpr1694);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:275:55: ( valueExpr )*
                    	do 
                    	{
                    	    int alt73 = 2;
                    	    alt73 = dfa73.Predict(input);
                    	    switch (alt73) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:275:56: valueExpr
                    			    {
                    			    	PushFollow(FOLLOW_valueExpr_in_inExpr1697);
                    			    	valueExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop73;
                    	    }
                    	} while (true);

                    	loop73:
                    		;	// Stops C# compiler whining that label 'loop73' has no statements

                    	if ( input.LA(1) == RPAREN || input.LA(1) == RBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(i); 
                    	}

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:276:4: ^(i= IN_RANGE valueExpr ( LPAREN | LBRACK ) valueExpr valueExpr ( RPAREN | RBRACK ) )
                    {
                    	i=(CommonTree)Match(input,IN_RANGE,FOLLOW_IN_RANGE_in_inExpr1716); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_inExpr1718);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( input.LA(1) == LPAREN || input.LA(1) == LBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}

                    	PushFollow(FOLLOW_valueExpr_in_inExpr1726);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_inExpr1728);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( input.LA(1) == RPAREN || input.LA(1) == RBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(i); 
                    	}

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:277:4: ^(i= NOT_IN_RANGE valueExpr ( LPAREN | LBRACK ) valueExpr valueExpr ( RPAREN | RBRACK ) )
                    {
                    	i=(CommonTree)Match(input,NOT_IN_RANGE,FOLLOW_NOT_IN_RANGE_in_inExpr1745); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_inExpr1747);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( input.LA(1) == LPAREN || input.LA(1) == LBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}

                    	PushFollow(FOLLOW_valueExpr_in_inExpr1755);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_inExpr1757);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( input.LA(1) == RPAREN || input.LA(1) == RBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(i); 
                    	}

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "inExpr"


    // $ANTLR start "betweenExpr"
    // EsperEPL2Ast.g:280:1: betweenExpr : ( ^(b= BETWEEN valueExpr valueExpr valueExpr ) | ^(b= NOT_BETWEEN valueExpr valueExpr ( valueExpr )* ) );
    public void betweenExpr() // throws RecognitionException [1]
    {   
        CommonTree b = null;

        try 
    	{
            // EsperEPL2Ast.g:281:2: ( ^(b= BETWEEN valueExpr valueExpr valueExpr ) | ^(b= NOT_BETWEEN valueExpr valueExpr ( valueExpr )* ) )
            int alt76 = 2;
            int LA76_0 = input.LA(1);

            if ( (LA76_0 == BETWEEN) )
            {
                alt76 = 1;
            }
            else if ( (LA76_0 == NOT_BETWEEN) )
            {
                alt76 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d76s0 =
                    new NoViableAltException("", 76, 0, input);

                throw nvae_d76s0;
            }
            switch (alt76) 
            {
                case 1 :
                    // EsperEPL2Ast.g:281:4: ^(b= BETWEEN valueExpr valueExpr valueExpr )
                    {
                    	b=(CommonTree)Match(input,BETWEEN,FOLLOW_BETWEEN_in_betweenExpr1782); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_betweenExpr1784);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_betweenExpr1786);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_betweenExpr1788);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(b); 
                    	}

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:282:4: ^(b= NOT_BETWEEN valueExpr valueExpr ( valueExpr )* )
                    {
                    	b=(CommonTree)Match(input,NOT_BETWEEN,FOLLOW_NOT_BETWEEN_in_betweenExpr1799); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_betweenExpr1801);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_betweenExpr1803);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:282:40: ( valueExpr )*
                    	do 
                    	{
                    	    int alt75 = 2;
                    	    alt75 = dfa75.Predict(input);
                    	    switch (alt75) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:282:41: valueExpr
                    			    {
                    			    	PushFollow(FOLLOW_valueExpr_in_betweenExpr1806);
                    			    	valueExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop75;
                    	    }
                    	} while (true);

                    	loop75:
                    		;	// Stops C# compiler whining that label 'loop75' has no statements


                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(b); 
                    	}

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "betweenExpr"


    // $ANTLR start "likeExpr"
    // EsperEPL2Ast.g:285:1: likeExpr : ( ^(l= LIKE valueExpr valueExpr ( valueExpr )? ) | ^(l= NOT_LIKE valueExpr valueExpr ( valueExpr )? ) );
    public void likeExpr() // throws RecognitionException [1]
    {   
        CommonTree l = null;

        try 
    	{
            // EsperEPL2Ast.g:286:2: ( ^(l= LIKE valueExpr valueExpr ( valueExpr )? ) | ^(l= NOT_LIKE valueExpr valueExpr ( valueExpr )? ) )
            int alt79 = 2;
            int LA79_0 = input.LA(1);

            if ( (LA79_0 == LIKE) )
            {
                alt79 = 1;
            }
            else if ( (LA79_0 == NOT_LIKE) )
            {
                alt79 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d79s0 =
                    new NoViableAltException("", 79, 0, input);

                throw nvae_d79s0;
            }
            switch (alt79) 
            {
                case 1 :
                    // EsperEPL2Ast.g:286:4: ^(l= LIKE valueExpr valueExpr ( valueExpr )? )
                    {
                    	l=(CommonTree)Match(input,LIKE,FOLLOW_LIKE_in_likeExpr1826); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_likeExpr1828);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_likeExpr1830);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:286:33: ( valueExpr )?
                    	int alt77 = 2;
                    	alt77 = dfa77.Predict(input);
                    	switch (alt77) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:286:34: valueExpr
                    	        {
                    	        	PushFollow(FOLLOW_valueExpr_in_likeExpr1833);
                    	        	valueExpr();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(l); 
                    	}

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:287:4: ^(l= NOT_LIKE valueExpr valueExpr ( valueExpr )? )
                    {
                    	l=(CommonTree)Match(input,NOT_LIKE,FOLLOW_NOT_LIKE_in_likeExpr1846); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_likeExpr1848);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_likeExpr1850);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:287:37: ( valueExpr )?
                    	int alt78 = 2;
                    	alt78 = dfa78.Predict(input);
                    	switch (alt78) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:287:38: valueExpr
                    	        {
                    	        	PushFollow(FOLLOW_valueExpr_in_likeExpr1853);
                    	        	valueExpr();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(l); 
                    	}

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "likeExpr"


    // $ANTLR start "regExpExpr"
    // EsperEPL2Ast.g:290:1: regExpExpr : ( ^(r= REGEXP valueExpr valueExpr ) | ^(r= NOT_REGEXP valueExpr valueExpr ) );
    public void regExpExpr() // throws RecognitionException [1]
    {   
        CommonTree r = null;

        try 
    	{
            // EsperEPL2Ast.g:291:2: ( ^(r= REGEXP valueExpr valueExpr ) | ^(r= NOT_REGEXP valueExpr valueExpr ) )
            int alt80 = 2;
            int LA80_0 = input.LA(1);

            if ( (LA80_0 == REGEXP) )
            {
                alt80 = 1;
            }
            else if ( (LA80_0 == NOT_REGEXP) )
            {
                alt80 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d80s0 =
                    new NoViableAltException("", 80, 0, input);

                throw nvae_d80s0;
            }
            switch (alt80) 
            {
                case 1 :
                    // EsperEPL2Ast.g:291:4: ^(r= REGEXP valueExpr valueExpr )
                    {
                    	r=(CommonTree)Match(input,REGEXP,FOLLOW_REGEXP_in_regExpExpr1872); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_regExpExpr1874);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_regExpExpr1876);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(r); 
                    	}

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:292:4: ^(r= NOT_REGEXP valueExpr valueExpr )
                    {
                    	r=(CommonTree)Match(input,NOT_REGEXP,FOLLOW_NOT_REGEXP_in_regExpExpr1887); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_regExpExpr1889);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_regExpExpr1891);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(r); 
                    	}

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "regExpExpr"


    // $ANTLR start "builtinFunc"
    // EsperEPL2Ast.g:295:1: builtinFunc : ( ^(f= SUM ( DISTINCT )? valueExpr ) | ^(f= AVG ( DISTINCT )? valueExpr ) | ^(f= COUNT ( ( DISTINCT )? valueExpr )? ) | ^(f= MEDIAN ( DISTINCT )? valueExpr ) | ^(f= STDDEV ( DISTINCT )? valueExpr ) | ^(f= AVEDEV ( DISTINCT )? valueExpr ) | ^(f= COALESCE valueExpr valueExpr ( valueExpr )* ) | ^(f= PREVIOUS valueExpr eventPropertyExpr ) | ^(f= PRIOR c= NUM_INT eventPropertyExpr ) | ^(f= INSTANCEOF valueExpr CLASS_IDENT ( CLASS_IDENT )* ) | ^(f= CAST valueExpr CLASS_IDENT ) | ^(f= EXISTS eventPropertyExpr ) | ^(f= CURRENT_TIMESTAMP ) );
    public void builtinFunc() // throws RecognitionException [1]
    {   
        CommonTree f = null;
        CommonTree c = null;

        try 
    	{
            // EsperEPL2Ast.g:296:2: ( ^(f= SUM ( DISTINCT )? valueExpr ) | ^(f= AVG ( DISTINCT )? valueExpr ) | ^(f= COUNT ( ( DISTINCT )? valueExpr )? ) | ^(f= MEDIAN ( DISTINCT )? valueExpr ) | ^(f= STDDEV ( DISTINCT )? valueExpr ) | ^(f= AVEDEV ( DISTINCT )? valueExpr ) | ^(f= COALESCE valueExpr valueExpr ( valueExpr )* ) | ^(f= PREVIOUS valueExpr eventPropertyExpr ) | ^(f= PRIOR c= NUM_INT eventPropertyExpr ) | ^(f= INSTANCEOF valueExpr CLASS_IDENT ( CLASS_IDENT )* ) | ^(f= CAST valueExpr CLASS_IDENT ) | ^(f= EXISTS eventPropertyExpr ) | ^(f= CURRENT_TIMESTAMP ) )
            int alt90 = 13;
            alt90 = dfa90.Predict(input);
            switch (alt90) 
            {
                case 1 :
                    // EsperEPL2Ast.g:296:5: ^(f= SUM ( DISTINCT )? valueExpr )
                    {
                    	f=(CommonTree)Match(input,SUM,FOLLOW_SUM_in_builtinFunc1910); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// EsperEPL2Ast.g:296:13: ( DISTINCT )?
                    	int alt81 = 2;
                    	alt81 = dfa81.Predict(input);
                    	switch (alt81) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:296:14: DISTINCT
                    	        {
                    	        	Match(input,DISTINCT,FOLLOW_DISTINCT_in_builtinFunc1913); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	PushFollow(FOLLOW_valueExpr_in_builtinFunc1917);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:297:4: ^(f= AVG ( DISTINCT )? valueExpr )
                    {
                    	f=(CommonTree)Match(input,AVG,FOLLOW_AVG_in_builtinFunc1928); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// EsperEPL2Ast.g:297:12: ( DISTINCT )?
                    	int alt82 = 2;
                    	alt82 = dfa82.Predict(input);
                    	switch (alt82) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:297:13: DISTINCT
                    	        {
                    	        	Match(input,DISTINCT,FOLLOW_DISTINCT_in_builtinFunc1931); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	PushFollow(FOLLOW_valueExpr_in_builtinFunc1935);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:298:4: ^(f= COUNT ( ( DISTINCT )? valueExpr )? )
                    {
                    	f=(CommonTree)Match(input,COUNT,FOLLOW_COUNT_in_builtinFunc1946); if (state.failed) return ;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // EsperEPL2Ast.g:298:14: ( ( DISTINCT )? valueExpr )?
                    	    int alt84 = 2;
                    	    alt84 = dfa84.Predict(input);
                    	    switch (alt84) 
                    	    {
                    	        case 1 :
                    	            // EsperEPL2Ast.g:298:15: ( DISTINCT )? valueExpr
                    	            {
                    	            	// EsperEPL2Ast.g:298:15: ( DISTINCT )?
                    	            	int alt83 = 2;
                    	            	alt83 = dfa83.Predict(input);
                    	            	switch (alt83) 
                    	            	{
                    	            	    case 1 :
                    	            	        // EsperEPL2Ast.g:298:16: DISTINCT
                    	            	        {
                    	            	        	Match(input,DISTINCT,FOLLOW_DISTINCT_in_builtinFunc1950); if (state.failed) return ;

                    	            	        }
                    	            	        break;

                    	            	}

                    	            	PushFollow(FOLLOW_valueExpr_in_builtinFunc1954);
                    	            	valueExpr();
                    	            	state.followingStackPointer--;
                    	            	if (state.failed) return ;

                    	            }
                    	            break;

                    	    }


                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:299:4: ^(f= MEDIAN ( DISTINCT )? valueExpr )
                    {
                    	f=(CommonTree)Match(input,MEDIAN,FOLLOW_MEDIAN_in_builtinFunc1968); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// EsperEPL2Ast.g:299:15: ( DISTINCT )?
                    	int alt85 = 2;
                    	alt85 = dfa85.Predict(input);
                    	switch (alt85) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:299:16: DISTINCT
                    	        {
                    	        	Match(input,DISTINCT,FOLLOW_DISTINCT_in_builtinFunc1971); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	PushFollow(FOLLOW_valueExpr_in_builtinFunc1975);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    }
                    break;
                case 5 :
                    // EsperEPL2Ast.g:300:4: ^(f= STDDEV ( DISTINCT )? valueExpr )
                    {
                    	f=(CommonTree)Match(input,STDDEV,FOLLOW_STDDEV_in_builtinFunc1986); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// EsperEPL2Ast.g:300:15: ( DISTINCT )?
                    	int alt86 = 2;
                    	alt86 = dfa86.Predict(input);
                    	switch (alt86) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:300:16: DISTINCT
                    	        {
                    	        	Match(input,DISTINCT,FOLLOW_DISTINCT_in_builtinFunc1989); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	PushFollow(FOLLOW_valueExpr_in_builtinFunc1993);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    }
                    break;
                case 6 :
                    // EsperEPL2Ast.g:301:4: ^(f= AVEDEV ( DISTINCT )? valueExpr )
                    {
                    	f=(CommonTree)Match(input,AVEDEV,FOLLOW_AVEDEV_in_builtinFunc2004); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// EsperEPL2Ast.g:301:15: ( DISTINCT )?
                    	int alt87 = 2;
                    	alt87 = dfa87.Predict(input);
                    	switch (alt87) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:301:16: DISTINCT
                    	        {
                    	        	Match(input,DISTINCT,FOLLOW_DISTINCT_in_builtinFunc2007); if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	PushFollow(FOLLOW_valueExpr_in_builtinFunc2011);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    }
                    break;
                case 7 :
                    // EsperEPL2Ast.g:302:5: ^(f= COALESCE valueExpr valueExpr ( valueExpr )* )
                    {
                    	f=(CommonTree)Match(input,COALESCE,FOLLOW_COALESCE_in_builtinFunc2023); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_builtinFunc2025);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_builtinFunc2027);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:302:38: ( valueExpr )*
                    	do 
                    	{
                    	    int alt88 = 2;
                    	    alt88 = dfa88.Predict(input);
                    	    switch (alt88) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:302:39: valueExpr
                    			    {
                    			    	PushFollow(FOLLOW_valueExpr_in_builtinFunc2030);
                    			    	valueExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop88;
                    	    }
                    	} while (true);

                    	loop88:
                    		;	// Stops C# compiler whining that label 'loop88' has no statements


                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    }
                    break;
                case 8 :
                    // EsperEPL2Ast.g:303:5: ^(f= PREVIOUS valueExpr eventPropertyExpr )
                    {
                    	f=(CommonTree)Match(input,PREVIOUS,FOLLOW_PREVIOUS_in_builtinFunc2045); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_builtinFunc2047);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_eventPropertyExpr_in_builtinFunc2049);
                    	eventPropertyExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    }
                    break;
                case 9 :
                    // EsperEPL2Ast.g:304:5: ^(f= PRIOR c= NUM_INT eventPropertyExpr )
                    {
                    	f=(CommonTree)Match(input,PRIOR,FOLLOW_PRIOR_in_builtinFunc2061); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	c=(CommonTree)Match(input,NUM_INT,FOLLOW_NUM_INT_in_builtinFunc2065); if (state.failed) return ;
                    	PushFollow(FOLLOW_eventPropertyExpr_in_builtinFunc2067);
                    	eventPropertyExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	  LeaveNode(c); LeaveNode(f);
                    	}

                    }
                    break;
                case 10 :
                    // EsperEPL2Ast.g:305:5: ^(f= INSTANCEOF valueExpr CLASS_IDENT ( CLASS_IDENT )* )
                    {
                    	f=(CommonTree)Match(input,INSTANCEOF,FOLLOW_INSTANCEOF_in_builtinFunc2079); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_builtinFunc2081);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	Match(input,CLASS_IDENT,FOLLOW_CLASS_IDENT_in_builtinFunc2083); if (state.failed) return ;
                    	// EsperEPL2Ast.g:305:42: ( CLASS_IDENT )*
                    	do 
                    	{
                    	    int alt89 = 2;
                    	    int LA89_0 = input.LA(1);

                    	    if ( (LA89_0 == CLASS_IDENT) )
                    	    {
                    	        alt89 = 1;
                    	    }


                    	    switch (alt89) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:305:43: CLASS_IDENT
                    			    {
                    			    	Match(input,CLASS_IDENT,FOLLOW_CLASS_IDENT_in_builtinFunc2086); if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop89;
                    	    }
                    	} while (true);

                    	loop89:
                    		;	// Stops C# compiler whining that label 'loop89' has no statements


                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    }
                    break;
                case 11 :
                    // EsperEPL2Ast.g:306:5: ^(f= CAST valueExpr CLASS_IDENT )
                    {
                    	f=(CommonTree)Match(input,CAST,FOLLOW_CAST_in_builtinFunc2100); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_builtinFunc2102);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	Match(input,CLASS_IDENT,FOLLOW_CLASS_IDENT_in_builtinFunc2104); if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    }
                    break;
                case 12 :
                    // EsperEPL2Ast.g:307:5: ^(f= EXISTS eventPropertyExpr )
                    {
                    	f=(CommonTree)Match(input,EXISTS,FOLLOW_EXISTS_in_builtinFunc2116); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_eventPropertyExpr_in_builtinFunc2118);
                    	eventPropertyExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    }
                    break;
                case 13 :
                    // EsperEPL2Ast.g:308:4: ^(f= CURRENT_TIMESTAMP )
                    {
                    	f=(CommonTree)Match(input,CURRENT_TIMESTAMP,FOLLOW_CURRENT_TIMESTAMP_in_builtinFunc2129); if (state.failed) return ;

                    	if ( state.backtracking == 0 ) 
                    	{
                    	}

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "builtinFunc"


    // $ANTLR start "arrayExpr"
    // EsperEPL2Ast.g:311:1: arrayExpr : ^(a= ARRAY_EXPR ( valueExpr )* ) ;
    public void arrayExpr() // throws RecognitionException [1]
    {   
        CommonTree a = null;

        try 
    	{
            // EsperEPL2Ast.g:312:2: ( ^(a= ARRAY_EXPR ( valueExpr )* ) )
            // EsperEPL2Ast.g:312:4: ^(a= ARRAY_EXPR ( valueExpr )* )
            {
            	a=(CommonTree)Match(input,ARRAY_EXPR,FOLLOW_ARRAY_EXPR_in_arrayExpr2149); if (state.failed) return ;

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); if (state.failed) return ;
            	    // EsperEPL2Ast.g:312:19: ( valueExpr )*
            	    do 
            	    {
            	        int alt91 = 2;
            	        alt91 = dfa91.Predict(input);
            	        switch (alt91) 
            	    	{
            	    		case 1 :
            	    		    // EsperEPL2Ast.g:312:20: valueExpr
            	    		    {
            	    		    	PushFollow(FOLLOW_valueExpr_in_arrayExpr2152);
            	    		    	valueExpr();
            	    		    	state.followingStackPointer--;
            	    		    	if (state.failed) return ;

            	    		    }
            	    		    break;

            	    		default:
            	    		    goto loop91;
            	        }
            	    } while (true);

            	    loop91:
            	    	;	// Stops C# compiler whining that label 'loop91' has no statements


            	    Match(input, Token.UP, null); if (state.failed) return ;
            	}
            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(a); 
            	}

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "arrayExpr"


    // $ANTLR start "arithmeticExpr"
    // EsperEPL2Ast.g:315:1: arithmeticExpr : ( ^(a= PLUS valueExpr valueExpr ) | ^(a= MINUS valueExpr valueExpr ) | ^(a= DIV valueExpr valueExpr ) | ^(a= STAR valueExpr valueExpr ) | ^(a= MOD valueExpr valueExpr ) | ^(a= BAND valueExpr valueExpr ) | ^(a= BOR valueExpr valueExpr ) | ^(a= BXOR valueExpr valueExpr ) | ^(a= CONCAT valueExpr valueExpr ( valueExpr )* ) );
    public void arithmeticExpr() // throws RecognitionException [1]
    {   
        CommonTree a = null;

        try 
    	{
            // EsperEPL2Ast.g:316:2: ( ^(a= PLUS valueExpr valueExpr ) | ^(a= MINUS valueExpr valueExpr ) | ^(a= DIV valueExpr valueExpr ) | ^(a= STAR valueExpr valueExpr ) | ^(a= MOD valueExpr valueExpr ) | ^(a= BAND valueExpr valueExpr ) | ^(a= BOR valueExpr valueExpr ) | ^(a= BXOR valueExpr valueExpr ) | ^(a= CONCAT valueExpr valueExpr ( valueExpr )* ) )
            int alt93 = 9;
            alt93 = dfa93.Predict(input);
            switch (alt93) 
            {
                case 1 :
                    // EsperEPL2Ast.g:316:5: ^(a= PLUS valueExpr valueExpr )
                    {
                    	a=(CommonTree)Match(input,PLUS,FOLLOW_PLUS_in_arithmeticExpr2173); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2175);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2177);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(a); 
                    	}

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:317:5: ^(a= MINUS valueExpr valueExpr )
                    {
                    	a=(CommonTree)Match(input,MINUS,FOLLOW_MINUS_in_arithmeticExpr2189); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2191);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2193);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(a); 
                    	}

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:318:5: ^(a= DIV valueExpr valueExpr )
                    {
                    	a=(CommonTree)Match(input,DIV,FOLLOW_DIV_in_arithmeticExpr2205); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2207);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2209);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(a); 
                    	}

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:319:4: ^(a= STAR valueExpr valueExpr )
                    {
                    	a=(CommonTree)Match(input,STAR,FOLLOW_STAR_in_arithmeticExpr2220); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2222);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2224);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(a); 
                    	}

                    }
                    break;
                case 5 :
                    // EsperEPL2Ast.g:320:5: ^(a= MOD valueExpr valueExpr )
                    {
                    	a=(CommonTree)Match(input,MOD,FOLLOW_MOD_in_arithmeticExpr2236); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2238);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2240);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(a); 
                    	}

                    }
                    break;
                case 6 :
                    // EsperEPL2Ast.g:321:4: ^(a= BAND valueExpr valueExpr )
                    {
                    	a=(CommonTree)Match(input,BAND,FOLLOW_BAND_in_arithmeticExpr2251); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2253);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2255);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(a); 
                    	}

                    }
                    break;
                case 7 :
                    // EsperEPL2Ast.g:322:4: ^(a= BOR valueExpr valueExpr )
                    {
                    	a=(CommonTree)Match(input,BOR,FOLLOW_BOR_in_arithmeticExpr2266); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2268);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2270);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(a); 
                    	}

                    }
                    break;
                case 8 :
                    // EsperEPL2Ast.g:323:4: ^(a= BXOR valueExpr valueExpr )
                    {
                    	a=(CommonTree)Match(input,BXOR,FOLLOW_BXOR_in_arithmeticExpr2281); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2283);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2285);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(a); 
                    	}

                    }
                    break;
                case 9 :
                    // EsperEPL2Ast.g:324:5: ^(a= CONCAT valueExpr valueExpr ( valueExpr )* )
                    {
                    	a=(CommonTree)Match(input,CONCAT,FOLLOW_CONCAT_in_arithmeticExpr2297); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2299);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2301);
                    	valueExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:324:36: ( valueExpr )*
                    	do 
                    	{
                    	    int alt92 = 2;
                    	    alt92 = dfa92.Predict(input);
                    	    switch (alt92) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:324:37: valueExpr
                    			    {
                    			    	PushFollow(FOLLOW_valueExpr_in_arithmeticExpr2304);
                    			    	valueExpr();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop92;
                    	    }
                    	} while (true);

                    	loop92:
                    		;	// Stops C# compiler whining that label 'loop92' has no statements


                    	Match(input, Token.UP, null); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(a); 
                    	}

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "arithmeticExpr"


    // $ANTLR start "libFunc"
    // EsperEPL2Ast.g:327:1: libFunc : ^(l= LIB_FUNCTION ( CLASS_IDENT )? IDENT ( DISTINCT )? ( valueExpr )* ) ;
    public void libFunc() // throws RecognitionException [1]
    {   
        CommonTree l = null;

        try 
    	{
            // EsperEPL2Ast.g:328:2: ( ^(l= LIB_FUNCTION ( CLASS_IDENT )? IDENT ( DISTINCT )? ( valueExpr )* ) )
            // EsperEPL2Ast.g:328:5: ^(l= LIB_FUNCTION ( CLASS_IDENT )? IDENT ( DISTINCT )? ( valueExpr )* )
            {
            	l=(CommonTree)Match(input,LIB_FUNCTION,FOLLOW_LIB_FUNCTION_in_libFunc2325); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// EsperEPL2Ast.g:328:22: ( CLASS_IDENT )?
            	int alt94 = 2;
            	int LA94_0 = input.LA(1);

            	if ( (LA94_0 == CLASS_IDENT) )
            	{
            	    alt94 = 1;
            	}
            	switch (alt94) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:328:23: CLASS_IDENT
            	        {
            	        	Match(input,CLASS_IDENT,FOLLOW_CLASS_IDENT_in_libFunc2328); if (state.failed) return ;

            	        }
            	        break;

            	}

            	Match(input,IDENT,FOLLOW_IDENT_in_libFunc2332); if (state.failed) return ;
            	// EsperEPL2Ast.g:328:43: ( DISTINCT )?
            	int alt95 = 2;
            	alt95 = dfa95.Predict(input);
            	switch (alt95) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:328:44: DISTINCT
            	        {
            	        	Match(input,DISTINCT,FOLLOW_DISTINCT_in_libFunc2335); if (state.failed) return ;

            	        }
            	        break;

            	}

            	// EsperEPL2Ast.g:328:55: ( valueExpr )*
            	do 
            	{
            	    int alt96 = 2;
            	    alt96 = dfa96.Predict(input);
            	    switch (alt96) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:328:56: valueExpr
            			    {
            			    	PushFollow(FOLLOW_valueExpr_in_libFunc2340);
            			    	valueExpr();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop96;
            	    }
            	} while (true);

            	loop96:
            		;	// Stops C# compiler whining that label 'loop96' has no statements


            	Match(input, Token.UP, null); if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(l); 
            	}

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "libFunc"


    // $ANTLR start "startPatternExpressionRule"
    // EsperEPL2Ast.g:334:1: startPatternExpressionRule : exprChoice ;
    public void startPatternExpressionRule() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:335:2: ( exprChoice )
            // EsperEPL2Ast.g:335:4: exprChoice
            {
            	if ( state.backtracking == 0 ) 
            	{
            	  SetIsPatternWalk(true);
            	}
            	PushFollow(FOLLOW_exprChoice_in_startPatternExpressionRule2362);
            	exprChoice();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	   EndPattern(); End(); 
            	}

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "startPatternExpressionRule"


    // $ANTLR start "exprChoice"
    // EsperEPL2Ast.g:338:1: exprChoice : ( atomicExpr | patternOp | ^(a= EVERY_EXPR exprChoice ) | ^(n= NOT_EXPR exprChoice ) | ^(g= GUARD_EXPR exprChoice IDENT IDENT ( parameter )* ) );
    public void exprChoice() // throws RecognitionException [1]
    {   
        CommonTree a = null;
        CommonTree n = null;
        CommonTree g = null;

        try 
    	{
            // EsperEPL2Ast.g:339:2: ( atomicExpr | patternOp | ^(a= EVERY_EXPR exprChoice ) | ^(n= NOT_EXPR exprChoice ) | ^(g= GUARD_EXPR exprChoice IDENT IDENT ( parameter )* ) )
            int alt98 = 5;
            switch ( input.LA(1) ) 
            {
            case EVENT_FILTER_EXPR:
            case OBSERVER_EXPR:
            	{
                alt98 = 1;
                }
                break;
            case OR_EXPR:
            case AND_EXPR:
            case FOLLOWED_BY_EXPR:
            	{
                alt98 = 2;
                }
                break;
            case EVERY_EXPR:
            	{
                alt98 = 3;
                }
                break;
            case NOT_EXPR:
            	{
                alt98 = 4;
                }
                break;
            case GUARD_EXPR:
            	{
                alt98 = 5;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d98s0 =
            	        new NoViableAltException("", 98, 0, input);

            	    throw nvae_d98s0;
            }

            switch (alt98) 
            {
                case 1 :
                    // EsperEPL2Ast.g:339:5: atomicExpr
                    {
                    	PushFollow(FOLLOW_atomicExpr_in_exprChoice2376);
                    	atomicExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:340:4: patternOp
                    {
                    	PushFollow(FOLLOW_patternOp_in_exprChoice2381);
                    	patternOp();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:341:5: ^(a= EVERY_EXPR exprChoice )
                    {
                    	a=(CommonTree)Match(input,EVERY_EXPR,FOLLOW_EVERY_EXPR_in_exprChoice2391); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_exprChoice_in_exprChoice2393);
                    	exprChoice();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(a); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:342:5: ^(n= NOT_EXPR exprChoice )
                    {
                    	n=(CommonTree)Match(input,NOT_EXPR,FOLLOW_NOT_EXPR_in_exprChoice2407); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_exprChoice_in_exprChoice2409);
                    	exprChoice();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(n); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // EsperEPL2Ast.g:343:5: ^(g= GUARD_EXPR exprChoice IDENT IDENT ( parameter )* )
                    {
                    	g=(CommonTree)Match(input,GUARD_EXPR,FOLLOW_GUARD_EXPR_in_exprChoice2423); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_exprChoice_in_exprChoice2425);
                    	exprChoice();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	Match(input,IDENT,FOLLOW_IDENT_in_exprChoice2427); if (state.failed) return ;
                    	Match(input,IDENT,FOLLOW_IDENT_in_exprChoice2429); if (state.failed) return ;
                    	// EsperEPL2Ast.g:343:44: ( parameter )*
                    	do 
                    	{
                    	    int alt97 = 2;
                    	    alt97 = dfa97.Predict(input);
                    	    switch (alt97) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:343:44: parameter
                    			    {
                    			    	PushFollow(FOLLOW_parameter_in_exprChoice2431);
                    			    	parameter();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop97;
                    	    }
                    	} while (true);

                    	loop97:
                    		;	// Stops C# compiler whining that label 'loop97' has no statements

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(g); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "exprChoice"


    // $ANTLR start "patternOp"
    // EsperEPL2Ast.g:346:1: patternOp : ( ^(f= FOLLOWED_BY_EXPR exprChoice exprChoice ( exprChoice )* ) | ^(o= OR_EXPR exprChoice exprChoice ( exprChoice )* ) | ^(a= AND_EXPR exprChoice exprChoice ( exprChoice )* ) );
    public void patternOp() // throws RecognitionException [1]
    {   
        CommonTree f = null;
        CommonTree o = null;
        CommonTree a = null;

        try 
    	{
            // EsperEPL2Ast.g:347:2: ( ^(f= FOLLOWED_BY_EXPR exprChoice exprChoice ( exprChoice )* ) | ^(o= OR_EXPR exprChoice exprChoice ( exprChoice )* ) | ^(a= AND_EXPR exprChoice exprChoice ( exprChoice )* ) )
            int alt102 = 3;
            switch ( input.LA(1) ) 
            {
            case FOLLOWED_BY_EXPR:
            	{
                alt102 = 1;
                }
                break;
            case OR_EXPR:
            	{
                alt102 = 2;
                }
                break;
            case AND_EXPR:
            	{
                alt102 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d102s0 =
            	        new NoViableAltException("", 102, 0, input);

            	    throw nvae_d102s0;
            }

            switch (alt102) 
            {
                case 1 :
                    // EsperEPL2Ast.g:347:4: ^(f= FOLLOWED_BY_EXPR exprChoice exprChoice ( exprChoice )* )
                    {
                    	f=(CommonTree)Match(input,FOLLOWED_BY_EXPR,FOLLOW_FOLLOWED_BY_EXPR_in_patternOp2452); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_exprChoice_in_patternOp2454);
                    	exprChoice();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_exprChoice_in_patternOp2456);
                    	exprChoice();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:347:48: ( exprChoice )*
                    	do 
                    	{
                    	    int alt99 = 2;
                    	    alt99 = dfa99.Predict(input);
                    	    switch (alt99) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:347:49: exprChoice
                    			    {
                    			    	PushFollow(FOLLOW_exprChoice_in_patternOp2459);
                    			    	exprChoice();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop99;
                    	    }
                    	} while (true);

                    	loop99:
                    		;	// Stops C# compiler whining that label 'loop99' has no statements

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(f); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:348:5: ^(o= OR_EXPR exprChoice exprChoice ( exprChoice )* )
                    {
                    	o=(CommonTree)Match(input,OR_EXPR,FOLLOW_OR_EXPR_in_patternOp2475); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_exprChoice_in_patternOp2477);
                    	exprChoice();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_exprChoice_in_patternOp2479);
                    	exprChoice();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:348:40: ( exprChoice )*
                    	do 
                    	{
                    	    int alt100 = 2;
                    	    alt100 = dfa100.Predict(input);
                    	    switch (alt100) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:348:41: exprChoice
                    			    {
                    			    	PushFollow(FOLLOW_exprChoice_in_patternOp2482);
                    			    	exprChoice();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop100;
                    	    }
                    	} while (true);

                    	loop100:
                    		;	// Stops C# compiler whining that label 'loop100' has no statements

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(o); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:349:5: ^(a= AND_EXPR exprChoice exprChoice ( exprChoice )* )
                    {
                    	a=(CommonTree)Match(input,AND_EXPR,FOLLOW_AND_EXPR_in_patternOp2498); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_exprChoice_in_patternOp2500);
                    	exprChoice();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	PushFollow(FOLLOW_exprChoice_in_patternOp2502);
                    	exprChoice();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:349:41: ( exprChoice )*
                    	do 
                    	{
                    	    int alt101 = 2;
                    	    alt101 = dfa101.Predict(input);
                    	    switch (alt101) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:349:42: exprChoice
                    			    {
                    			    	PushFollow(FOLLOW_exprChoice_in_patternOp2505);
                    			    	exprChoice();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop101;
                    	    }
                    	} while (true);

                    	loop101:
                    		;	// Stops C# compiler whining that label 'loop101' has no statements

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(a); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "patternOp"


    // $ANTLR start "atomicExpr"
    // EsperEPL2Ast.g:352:1: atomicExpr : ( eventFilterExpr | ^(ac= OBSERVER_EXPR IDENT IDENT ( parameter )* ) );
    public void atomicExpr() // throws RecognitionException [1]
    {   
        CommonTree ac = null;

        try 
    	{
            // EsperEPL2Ast.g:353:2: ( eventFilterExpr | ^(ac= OBSERVER_EXPR IDENT IDENT ( parameter )* ) )
            int alt104 = 2;
            int LA104_0 = input.LA(1);

            if ( (LA104_0 == EVENT_FILTER_EXPR) )
            {
                alt104 = 1;
            }
            else if ( (LA104_0 == OBSERVER_EXPR) )
            {
                alt104 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d104s0 =
                    new NoViableAltException("", 104, 0, input);

                throw nvae_d104s0;
            }
            switch (alt104) 
            {
                case 1 :
                    // EsperEPL2Ast.g:353:4: eventFilterExpr
                    {
                    	PushFollow(FOLLOW_eventFilterExpr_in_atomicExpr2524);
                    	eventFilterExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:354:7: ^(ac= OBSERVER_EXPR IDENT IDENT ( parameter )* )
                    {
                    	ac=(CommonTree)Match(input,OBSERVER_EXPR,FOLLOW_OBSERVER_EXPR_in_atomicExpr2536); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	Match(input,IDENT,FOLLOW_IDENT_in_atomicExpr2538); if (state.failed) return ;
                    	Match(input,IDENT,FOLLOW_IDENT_in_atomicExpr2540); if (state.failed) return ;
                    	// EsperEPL2Ast.g:354:39: ( parameter )*
                    	do 
                    	{
                    	    int alt103 = 2;
                    	    alt103 = dfa103.Predict(input);
                    	    switch (alt103) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:354:39: parameter
                    			    {
                    			    	PushFollow(FOLLOW_parameter_in_atomicExpr2542);
                    			    	parameter();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop103;
                    	    }
                    	} while (true);

                    	loop103:
                    		;	// Stops C# compiler whining that label 'loop103' has no statements

                    	if ( state.backtracking == 0 ) 
                    	{
                    	   LeaveNode(ac); 
                    	}

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "atomicExpr"


    // $ANTLR start "eventFilterExpr"
    // EsperEPL2Ast.g:357:1: eventFilterExpr : ^(f= EVENT_FILTER_EXPR ( IDENT )? CLASS_IDENT ( valueExpr )* ) ;
    public void eventFilterExpr() // throws RecognitionException [1]
    {   
        CommonTree f = null;

        try 
    	{
            // EsperEPL2Ast.g:358:2: ( ^(f= EVENT_FILTER_EXPR ( IDENT )? CLASS_IDENT ( valueExpr )* ) )
            // EsperEPL2Ast.g:358:4: ^(f= EVENT_FILTER_EXPR ( IDENT )? CLASS_IDENT ( valueExpr )* )
            {
            	f=(CommonTree)Match(input,EVENT_FILTER_EXPR,FOLLOW_EVENT_FILTER_EXPR_in_eventFilterExpr2562); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	// EsperEPL2Ast.g:358:27: ( IDENT )?
            	int alt105 = 2;
            	int LA105_0 = input.LA(1);

            	if ( (LA105_0 == IDENT) )
            	{
            	    alt105 = 1;
            	}
            	switch (alt105) 
            	{
            	    case 1 :
            	        // EsperEPL2Ast.g:358:27: IDENT
            	        {
            	        	Match(input,IDENT,FOLLOW_IDENT_in_eventFilterExpr2564); if (state.failed) return ;

            	        }
            	        break;

            	}

            	Match(input,CLASS_IDENT,FOLLOW_CLASS_IDENT_in_eventFilterExpr2567); if (state.failed) return ;
            	// EsperEPL2Ast.g:358:46: ( valueExpr )*
            	do 
            	{
            	    int alt106 = 2;
            	    alt106 = dfa106.Predict(input);
            	    switch (alt106) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:358:47: valueExpr
            			    {
            			    	PushFollow(FOLLOW_valueExpr_in_eventFilterExpr2570);
            			    	valueExpr();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop106;
            	    }
            	} while (true);

            	loop106:
            		;	// Stops C# compiler whining that label 'loop106' has no statements

            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(f); 
            	}

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "eventFilterExpr"


    // $ANTLR start "filterParam"
    // EsperEPL2Ast.g:361:1: filterParam : ^( EVENT_FILTER_PARAM valueExpr ( valueExpr )* ) ;
    public void filterParam() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:362:2: ( ^( EVENT_FILTER_PARAM valueExpr ( valueExpr )* ) )
            // EsperEPL2Ast.g:362:4: ^( EVENT_FILTER_PARAM valueExpr ( valueExpr )* )
            {
            	Match(input,EVENT_FILTER_PARAM,FOLLOW_EVENT_FILTER_PARAM_in_filterParam2589); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_valueExpr_in_filterParam2591);
            	valueExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:362:35: ( valueExpr )*
            	do 
            	{
            	    int alt107 = 2;
            	    alt107 = dfa107.Predict(input);
            	    switch (alt107) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:362:36: valueExpr
            			    {
            			    	PushFollow(FOLLOW_valueExpr_in_filterParam2594);
            			    	valueExpr();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop107;
            	    }
            	} while (true);

            	loop107:
            		;	// Stops C# compiler whining that label 'loop107' has no statements


            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "filterParam"


    // $ANTLR start "filterParamComparator"
    // EsperEPL2Ast.g:365:1: filterParamComparator : ( ^( EQUALS filterAtom ) | ^( NOT_EQUAL filterAtom ) | ^( LT filterAtom ) | ^( LE filterAtom ) | ^( GT filterAtom ) | ^( GE filterAtom ) | ^( EVENT_FILTER_RANGE ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ( RPAREN | RBRACK ) ) | ^( EVENT_FILTER_NOT_RANGE ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ( RPAREN | RBRACK ) ) | ^( EVENT_FILTER_IN ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier )* ( RPAREN | RBRACK ) ) | ^( EVENT_FILTER_NOT_IN ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier )* ( RPAREN | RBRACK ) ) | ^( EVENT_FILTER_BETWEEN ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ) | ^( EVENT_FILTER_NOT_BETWEEN ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ) );
    public void filterParamComparator() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:366:2: ( ^( EQUALS filterAtom ) | ^( NOT_EQUAL filterAtom ) | ^( LT filterAtom ) | ^( LE filterAtom ) | ^( GT filterAtom ) | ^( GE filterAtom ) | ^( EVENT_FILTER_RANGE ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ( RPAREN | RBRACK ) ) | ^( EVENT_FILTER_NOT_RANGE ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ( RPAREN | RBRACK ) ) | ^( EVENT_FILTER_IN ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier )* ( RPAREN | RBRACK ) ) | ^( EVENT_FILTER_NOT_IN ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier )* ( RPAREN | RBRACK ) ) | ^( EVENT_FILTER_BETWEEN ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ) | ^( EVENT_FILTER_NOT_BETWEEN ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ) )
            int alt120 = 12;
            alt120 = dfa120.Predict(input);
            switch (alt120) 
            {
                case 1 :
                    // EsperEPL2Ast.g:366:4: ^( EQUALS filterAtom )
                    {
                    	Match(input,EQUALS,FOLLOW_EQUALS_in_filterParamComparator2610); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_filterAtom_in_filterParamComparator2612);
                    	filterAtom();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:367:4: ^( NOT_EQUAL filterAtom )
                    {
                    	Match(input,NOT_EQUAL,FOLLOW_NOT_EQUAL_in_filterParamComparator2619); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_filterAtom_in_filterParamComparator2621);
                    	filterAtom();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:368:4: ^( LT filterAtom )
                    {
                    	Match(input,LT,FOLLOW_LT_in_filterParamComparator2628); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_filterAtom_in_filterParamComparator2630);
                    	filterAtom();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:369:4: ^( LE filterAtom )
                    {
                    	Match(input,LE,FOLLOW_LE_in_filterParamComparator2637); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_filterAtom_in_filterParamComparator2639);
                    	filterAtom();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // EsperEPL2Ast.g:370:4: ^( GT filterAtom )
                    {
                    	Match(input,GT,FOLLOW_GT_in_filterParamComparator2646); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_filterAtom_in_filterParamComparator2648);
                    	filterAtom();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 6 :
                    // EsperEPL2Ast.g:371:4: ^( GE filterAtom )
                    {
                    	Match(input,GE,FOLLOW_GE_in_filterParamComparator2655); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	PushFollow(FOLLOW_filterAtom_in_filterParamComparator2657);
                    	filterAtom();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 7 :
                    // EsperEPL2Ast.g:372:4: ^( EVENT_FILTER_RANGE ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ( RPAREN | RBRACK ) )
                    {
                    	Match(input,EVENT_FILTER_RANGE,FOLLOW_EVENT_FILTER_RANGE_in_filterParamComparator2664); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	if ( input.LA(1) == LPAREN || input.LA(1) == LBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}

                    	// EsperEPL2Ast.g:372:41: ( constant[false] | filterIdentifier )
                    	int alt108 = 2;
                    	int LA108_0 = input.LA(1);

                    	if ( ((LA108_0 >= INT_TYPE && LA108_0 <= NULL_TYPE)) )
                    	{
                    	    alt108 = 1;
                    	}
                    	else if ( (LA108_0 == EVENT_FILTER_IDENT) )
                    	{
                    	    alt108 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d108s0 =
                    	        new NoViableAltException("", 108, 0, input);

                    	    throw nvae_d108s0;
                    	}
                    	switch (alt108) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:372:42: constant[false]
                    	        {
                    	        	PushFollow(FOLLOW_constant_in_filterParamComparator2673);
                    	        	constant(false);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:372:58: filterIdentifier
                    	        {
                    	        	PushFollow(FOLLOW_filterIdentifier_in_filterParamComparator2676);
                    	        	filterIdentifier();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:372:76: ( constant[false] | filterIdentifier )
                    	int alt109 = 2;
                    	int LA109_0 = input.LA(1);

                    	if ( ((LA109_0 >= INT_TYPE && LA109_0 <= NULL_TYPE)) )
                    	{
                    	    alt109 = 1;
                    	}
                    	else if ( (LA109_0 == EVENT_FILTER_IDENT) )
                    	{
                    	    alt109 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d109s0 =
                    	        new NoViableAltException("", 109, 0, input);

                    	    throw nvae_d109s0;
                    	}
                    	switch (alt109) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:372:77: constant[false]
                    	        {
                    	        	PushFollow(FOLLOW_constant_in_filterParamComparator2680);
                    	        	constant(false);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:372:93: filterIdentifier
                    	        {
                    	        	PushFollow(FOLLOW_filterIdentifier_in_filterParamComparator2683);
                    	        	filterIdentifier();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	if ( input.LA(1) == RPAREN || input.LA(1) == RBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 8 :
                    // EsperEPL2Ast.g:373:4: ^( EVENT_FILTER_NOT_RANGE ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ( RPAREN | RBRACK ) )
                    {
                    	Match(input,EVENT_FILTER_NOT_RANGE,FOLLOW_EVENT_FILTER_NOT_RANGE_in_filterParamComparator2697); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	if ( input.LA(1) == LPAREN || input.LA(1) == LBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}

                    	// EsperEPL2Ast.g:373:45: ( constant[false] | filterIdentifier )
                    	int alt110 = 2;
                    	int LA110_0 = input.LA(1);

                    	if ( ((LA110_0 >= INT_TYPE && LA110_0 <= NULL_TYPE)) )
                    	{
                    	    alt110 = 1;
                    	}
                    	else if ( (LA110_0 == EVENT_FILTER_IDENT) )
                    	{
                    	    alt110 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d110s0 =
                    	        new NoViableAltException("", 110, 0, input);

                    	    throw nvae_d110s0;
                    	}
                    	switch (alt110) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:373:46: constant[false]
                    	        {
                    	        	PushFollow(FOLLOW_constant_in_filterParamComparator2706);
                    	        	constant(false);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:373:62: filterIdentifier
                    	        {
                    	        	PushFollow(FOLLOW_filterIdentifier_in_filterParamComparator2709);
                    	        	filterIdentifier();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:373:80: ( constant[false] | filterIdentifier )
                    	int alt111 = 2;
                    	int LA111_0 = input.LA(1);

                    	if ( ((LA111_0 >= INT_TYPE && LA111_0 <= NULL_TYPE)) )
                    	{
                    	    alt111 = 1;
                    	}
                    	else if ( (LA111_0 == EVENT_FILTER_IDENT) )
                    	{
                    	    alt111 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d111s0 =
                    	        new NoViableAltException("", 111, 0, input);

                    	    throw nvae_d111s0;
                    	}
                    	switch (alt111) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:373:81: constant[false]
                    	        {
                    	        	PushFollow(FOLLOW_constant_in_filterParamComparator2713);
                    	        	constant(false);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:373:97: filterIdentifier
                    	        {
                    	        	PushFollow(FOLLOW_filterIdentifier_in_filterParamComparator2716);
                    	        	filterIdentifier();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	if ( input.LA(1) == RPAREN || input.LA(1) == RBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 9 :
                    // EsperEPL2Ast.g:374:4: ^( EVENT_FILTER_IN ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier )* ( RPAREN | RBRACK ) )
                    {
                    	Match(input,EVENT_FILTER_IN,FOLLOW_EVENT_FILTER_IN_in_filterParamComparator2730); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	if ( input.LA(1) == LPAREN || input.LA(1) == LBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}

                    	// EsperEPL2Ast.g:374:38: ( constant[false] | filterIdentifier )
                    	int alt112 = 2;
                    	int LA112_0 = input.LA(1);

                    	if ( ((LA112_0 >= INT_TYPE && LA112_0 <= NULL_TYPE)) )
                    	{
                    	    alt112 = 1;
                    	}
                    	else if ( (LA112_0 == EVENT_FILTER_IDENT) )
                    	{
                    	    alt112 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d112s0 =
                    	        new NoViableAltException("", 112, 0, input);

                    	    throw nvae_d112s0;
                    	}
                    	switch (alt112) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:374:39: constant[false]
                    	        {
                    	        	PushFollow(FOLLOW_constant_in_filterParamComparator2739);
                    	        	constant(false);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:374:55: filterIdentifier
                    	        {
                    	        	PushFollow(FOLLOW_filterIdentifier_in_filterParamComparator2742);
                    	        	filterIdentifier();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:374:73: ( constant[false] | filterIdentifier )*
                    	do 
                    	{
                    	    int alt113 = 3;
                    	    alt113 = dfa113.Predict(input);
                    	    switch (alt113) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:374:74: constant[false]
                    			    {
                    			    	PushFollow(FOLLOW_constant_in_filterParamComparator2746);
                    			    	constant(false);
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;
                    			case 2 :
                    			    // EsperEPL2Ast.g:374:90: filterIdentifier
                    			    {
                    			    	PushFollow(FOLLOW_filterIdentifier_in_filterParamComparator2749);
                    			    	filterIdentifier();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop113;
                    	    }
                    	} while (true);

                    	loop113:
                    		;	// Stops C# compiler whining that label 'loop113' has no statements

                    	if ( input.LA(1) == RPAREN || input.LA(1) == RBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 10 :
                    // EsperEPL2Ast.g:375:4: ^( EVENT_FILTER_NOT_IN ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier )* ( RPAREN | RBRACK ) )
                    {
                    	Match(input,EVENT_FILTER_NOT_IN,FOLLOW_EVENT_FILTER_NOT_IN_in_filterParamComparator2764); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	if ( input.LA(1) == LPAREN || input.LA(1) == LBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}

                    	// EsperEPL2Ast.g:375:42: ( constant[false] | filterIdentifier )
                    	int alt114 = 2;
                    	int LA114_0 = input.LA(1);

                    	if ( ((LA114_0 >= INT_TYPE && LA114_0 <= NULL_TYPE)) )
                    	{
                    	    alt114 = 1;
                    	}
                    	else if ( (LA114_0 == EVENT_FILTER_IDENT) )
                    	{
                    	    alt114 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d114s0 =
                    	        new NoViableAltException("", 114, 0, input);

                    	    throw nvae_d114s0;
                    	}
                    	switch (alt114) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:375:43: constant[false]
                    	        {
                    	        	PushFollow(FOLLOW_constant_in_filterParamComparator2773);
                    	        	constant(false);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:375:59: filterIdentifier
                    	        {
                    	        	PushFollow(FOLLOW_filterIdentifier_in_filterParamComparator2776);
                    	        	filterIdentifier();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:375:77: ( constant[false] | filterIdentifier )*
                    	do 
                    	{
                    	    int alt115 = 3;
                    	    alt115 = dfa115.Predict(input);
                    	    switch (alt115) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:375:78: constant[false]
                    			    {
                    			    	PushFollow(FOLLOW_constant_in_filterParamComparator2780);
                    			    	constant(false);
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;
                    			case 2 :
                    			    // EsperEPL2Ast.g:375:94: filterIdentifier
                    			    {
                    			    	PushFollow(FOLLOW_filterIdentifier_in_filterParamComparator2783);
                    			    	filterIdentifier();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    goto loop115;
                    	    }
                    	} while (true);

                    	loop115:
                    		;	// Stops C# compiler whining that label 'loop115' has no statements

                    	if ( input.LA(1) == RPAREN || input.LA(1) == RBRACK ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 11 :
                    // EsperEPL2Ast.g:376:4: ^( EVENT_FILTER_BETWEEN ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) )
                    {
                    	Match(input,EVENT_FILTER_BETWEEN,FOLLOW_EVENT_FILTER_BETWEEN_in_filterParamComparator2798); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// EsperEPL2Ast.g:376:27: ( constant[false] | filterIdentifier )
                    	int alt116 = 2;
                    	int LA116_0 = input.LA(1);

                    	if ( ((LA116_0 >= INT_TYPE && LA116_0 <= NULL_TYPE)) )
                    	{
                    	    alt116 = 1;
                    	}
                    	else if ( (LA116_0 == EVENT_FILTER_IDENT) )
                    	{
                    	    alt116 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d116s0 =
                    	        new NoViableAltException("", 116, 0, input);

                    	    throw nvae_d116s0;
                    	}
                    	switch (alt116) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:376:28: constant[false]
                    	        {
                    	        	PushFollow(FOLLOW_constant_in_filterParamComparator2801);
                    	        	constant(false);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:376:44: filterIdentifier
                    	        {
                    	        	PushFollow(FOLLOW_filterIdentifier_in_filterParamComparator2804);
                    	        	filterIdentifier();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:376:62: ( constant[false] | filterIdentifier )
                    	int alt117 = 2;
                    	int LA117_0 = input.LA(1);

                    	if ( ((LA117_0 >= INT_TYPE && LA117_0 <= NULL_TYPE)) )
                    	{
                    	    alt117 = 1;
                    	}
                    	else if ( (LA117_0 == EVENT_FILTER_IDENT) )
                    	{
                    	    alt117 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d117s0 =
                    	        new NoViableAltException("", 117, 0, input);

                    	    throw nvae_d117s0;
                    	}
                    	switch (alt117) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:376:63: constant[false]
                    	        {
                    	        	PushFollow(FOLLOW_constant_in_filterParamComparator2808);
                    	        	constant(false);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:376:79: filterIdentifier
                    	        {
                    	        	PushFollow(FOLLOW_filterIdentifier_in_filterParamComparator2811);
                    	        	filterIdentifier();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 12 :
                    // EsperEPL2Ast.g:377:4: ^( EVENT_FILTER_NOT_BETWEEN ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) )
                    {
                    	Match(input,EVENT_FILTER_NOT_BETWEEN,FOLLOW_EVENT_FILTER_NOT_BETWEEN_in_filterParamComparator2819); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// EsperEPL2Ast.g:377:31: ( constant[false] | filterIdentifier )
                    	int alt118 = 2;
                    	int LA118_0 = input.LA(1);

                    	if ( ((LA118_0 >= INT_TYPE && LA118_0 <= NULL_TYPE)) )
                    	{
                    	    alt118 = 1;
                    	}
                    	else if ( (LA118_0 == EVENT_FILTER_IDENT) )
                    	{
                    	    alt118 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d118s0 =
                    	        new NoViableAltException("", 118, 0, input);

                    	    throw nvae_d118s0;
                    	}
                    	switch (alt118) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:377:32: constant[false]
                    	        {
                    	        	PushFollow(FOLLOW_constant_in_filterParamComparator2822);
                    	        	constant(false);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:377:48: filterIdentifier
                    	        {
                    	        	PushFollow(FOLLOW_filterIdentifier_in_filterParamComparator2825);
                    	        	filterIdentifier();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:377:66: ( constant[false] | filterIdentifier )
                    	int alt119 = 2;
                    	int LA119_0 = input.LA(1);

                    	if ( ((LA119_0 >= INT_TYPE && LA119_0 <= NULL_TYPE)) )
                    	{
                    	    alt119 = 1;
                    	}
                    	else if ( (LA119_0 == EVENT_FILTER_IDENT) )
                    	{
                    	    alt119 = 2;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    NoViableAltException nvae_d119s0 =
                    	        new NoViableAltException("", 119, 0, input);

                    	    throw nvae_d119s0;
                    	}
                    	switch (alt119) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:377:67: constant[false]
                    	        {
                    	        	PushFollow(FOLLOW_constant_in_filterParamComparator2829);
                    	        	constant(false);
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;
                    	    case 2 :
                    	        // EsperEPL2Ast.g:377:83: filterIdentifier
                    	        {
                    	        	PushFollow(FOLLOW_filterIdentifier_in_filterParamComparator2832);
                    	        	filterIdentifier();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "filterParamComparator"


    // $ANTLR start "filterAtom"
    // EsperEPL2Ast.g:380:1: filterAtom : ( constant[false] | filterIdentifier );
    public void filterAtom() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:381:2: ( constant[false] | filterIdentifier )
            int alt121 = 2;
            int LA121_0 = input.LA(1);

            if ( ((LA121_0 >= INT_TYPE && LA121_0 <= NULL_TYPE)) )
            {
                alt121 = 1;
            }
            else if ( (LA121_0 == EVENT_FILTER_IDENT) )
            {
                alt121 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d121s0 =
                    new NoViableAltException("", 121, 0, input);

                throw nvae_d121s0;
            }
            switch (alt121) 
            {
                case 1 :
                    // EsperEPL2Ast.g:381:4: constant[false]
                    {
                    	PushFollow(FOLLOW_constant_in_filterAtom2846);
                    	constant(false);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:382:4: filterIdentifier
                    {
                    	PushFollow(FOLLOW_filterIdentifier_in_filterAtom2852);
                    	filterIdentifier();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "filterAtom"


    // $ANTLR start "filterIdentifier"
    // EsperEPL2Ast.g:384:1: filterIdentifier : ^( EVENT_FILTER_IDENT IDENT eventPropertyExpr ) ;
    public void filterIdentifier() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:385:2: ( ^( EVENT_FILTER_IDENT IDENT eventPropertyExpr ) )
            // EsperEPL2Ast.g:385:4: ^( EVENT_FILTER_IDENT IDENT eventPropertyExpr )
            {
            	Match(input,EVENT_FILTER_IDENT,FOLLOW_EVENT_FILTER_IDENT_in_filterIdentifier2863); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	Match(input,IDENT,FOLLOW_IDENT_in_filterIdentifier2865); if (state.failed) return ;
            	PushFollow(FOLLOW_eventPropertyExpr_in_filterIdentifier2867);
            	eventPropertyExpr();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "filterIdentifier"


    // $ANTLR start "eventPropertyExpr"
    // EsperEPL2Ast.g:388:1: eventPropertyExpr : ^(p= EVENT_PROP_EXPR eventPropertyAtomic ( eventPropertyAtomic )* ) ;
    public void eventPropertyExpr() // throws RecognitionException [1]
    {   
        CommonTree p = null;

        try 
    	{
            // EsperEPL2Ast.g:389:2: ( ^(p= EVENT_PROP_EXPR eventPropertyAtomic ( eventPropertyAtomic )* ) )
            // EsperEPL2Ast.g:389:4: ^(p= EVENT_PROP_EXPR eventPropertyAtomic ( eventPropertyAtomic )* )
            {
            	p=(CommonTree)Match(input,EVENT_PROP_EXPR,FOLLOW_EVENT_PROP_EXPR_in_eventPropertyExpr2884); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_eventPropertyAtomic_in_eventPropertyExpr2886);
            	eventPropertyAtomic();
            	state.followingStackPointer--;
            	if (state.failed) return ;
            	// EsperEPL2Ast.g:389:44: ( eventPropertyAtomic )*
            	do 
            	{
            	    int alt122 = 2;
            	    int LA122_0 = input.LA(1);

            	    if ( ((LA122_0 >= EVENT_PROP_SIMPLE && LA122_0 <= EVENT_PROP_DYNAMIC_MAPPED)) )
            	    {
            	        alt122 = 1;
            	    }


            	    switch (alt122) 
            		{
            			case 1 :
            			    // EsperEPL2Ast.g:389:45: eventPropertyAtomic
            			    {
            			    	PushFollow(FOLLOW_eventPropertyAtomic_in_eventPropertyExpr2889);
            			    	eventPropertyAtomic();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop122;
            	    }
            	} while (true);

            	loop122:
            		;	// Stops C# compiler whining that label 'loop122' has no statements


            	Match(input, Token.UP, null); if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(p); 
            	}

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "eventPropertyExpr"


    // $ANTLR start "eventPropertyAtomic"
    // EsperEPL2Ast.g:392:1: eventPropertyAtomic : ( ^( EVENT_PROP_SIMPLE IDENT ) | ^( EVENT_PROP_INDEXED IDENT NUM_INT ) | ^( EVENT_PROP_MAPPED IDENT ( STRING_LITERAL | QUOTED_STRING_LITERAL ) ) | ^( EVENT_PROP_DYNAMIC_SIMPLE IDENT ) | ^( EVENT_PROP_DYNAMIC_INDEXED IDENT NUM_INT ) | ^( EVENT_PROP_DYNAMIC_MAPPED IDENT ( STRING_LITERAL | QUOTED_STRING_LITERAL ) ) );
    public void eventPropertyAtomic() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:393:2: ( ^( EVENT_PROP_SIMPLE IDENT ) | ^( EVENT_PROP_INDEXED IDENT NUM_INT ) | ^( EVENT_PROP_MAPPED IDENT ( STRING_LITERAL | QUOTED_STRING_LITERAL ) ) | ^( EVENT_PROP_DYNAMIC_SIMPLE IDENT ) | ^( EVENT_PROP_DYNAMIC_INDEXED IDENT NUM_INT ) | ^( EVENT_PROP_DYNAMIC_MAPPED IDENT ( STRING_LITERAL | QUOTED_STRING_LITERAL ) ) )
            int alt123 = 6;
            switch ( input.LA(1) ) 
            {
            case EVENT_PROP_SIMPLE:
            	{
                alt123 = 1;
                }
                break;
            case EVENT_PROP_INDEXED:
            	{
                alt123 = 2;
                }
                break;
            case EVENT_PROP_MAPPED:
            	{
                alt123 = 3;
                }
                break;
            case EVENT_PROP_DYNAMIC_SIMPLE:
            	{
                alt123 = 4;
                }
                break;
            case EVENT_PROP_DYNAMIC_INDEXED:
            	{
                alt123 = 5;
                }
                break;
            case EVENT_PROP_DYNAMIC_MAPPED:
            	{
                alt123 = 6;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d123s0 =
            	        new NoViableAltException("", 123, 0, input);

            	    throw nvae_d123s0;
            }

            switch (alt123) 
            {
                case 1 :
                    // EsperEPL2Ast.g:393:4: ^( EVENT_PROP_SIMPLE IDENT )
                    {
                    	Match(input,EVENT_PROP_SIMPLE,FOLLOW_EVENT_PROP_SIMPLE_in_eventPropertyAtomic2908); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	Match(input,IDENT,FOLLOW_IDENT_in_eventPropertyAtomic2910); if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:394:4: ^( EVENT_PROP_INDEXED IDENT NUM_INT )
                    {
                    	Match(input,EVENT_PROP_INDEXED,FOLLOW_EVENT_PROP_INDEXED_in_eventPropertyAtomic2917); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	Match(input,IDENT,FOLLOW_IDENT_in_eventPropertyAtomic2919); if (state.failed) return ;
                    	Match(input,NUM_INT,FOLLOW_NUM_INT_in_eventPropertyAtomic2921); if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:395:4: ^( EVENT_PROP_MAPPED IDENT ( STRING_LITERAL | QUOTED_STRING_LITERAL ) )
                    {
                    	Match(input,EVENT_PROP_MAPPED,FOLLOW_EVENT_PROP_MAPPED_in_eventPropertyAtomic2928); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	Match(input,IDENT,FOLLOW_IDENT_in_eventPropertyAtomic2930); if (state.failed) return ;
                    	if ( (input.LA(1) >= STRING_LITERAL && input.LA(1) <= QUOTED_STRING_LITERAL) ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:396:4: ^( EVENT_PROP_DYNAMIC_SIMPLE IDENT )
                    {
                    	Match(input,EVENT_PROP_DYNAMIC_SIMPLE,FOLLOW_EVENT_PROP_DYNAMIC_SIMPLE_in_eventPropertyAtomic2945); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	Match(input,IDENT,FOLLOW_IDENT_in_eventPropertyAtomic2947); if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // EsperEPL2Ast.g:397:4: ^( EVENT_PROP_DYNAMIC_INDEXED IDENT NUM_INT )
                    {
                    	Match(input,EVENT_PROP_DYNAMIC_INDEXED,FOLLOW_EVENT_PROP_DYNAMIC_INDEXED_in_eventPropertyAtomic2954); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	Match(input,IDENT,FOLLOW_IDENT_in_eventPropertyAtomic2956); if (state.failed) return ;
                    	Match(input,NUM_INT,FOLLOW_NUM_INT_in_eventPropertyAtomic2958); if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 6 :
                    // EsperEPL2Ast.g:398:4: ^( EVENT_PROP_DYNAMIC_MAPPED IDENT ( STRING_LITERAL | QUOTED_STRING_LITERAL ) )
                    {
                    	Match(input,EVENT_PROP_DYNAMIC_MAPPED,FOLLOW_EVENT_PROP_DYNAMIC_MAPPED_in_eventPropertyAtomic2965); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	Match(input,IDENT,FOLLOW_IDENT_in_eventPropertyAtomic2967); if (state.failed) return ;
                    	if ( (input.LA(1) >= STRING_LITERAL && input.LA(1) <= QUOTED_STRING_LITERAL) ) 
                    	{
                    	    input.Consume();
                    	    state.errorRecovery = false;state.failed = false;
                    	}
                    	else 
                    	{
                    	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	    throw mse;
                    	}


                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "eventPropertyAtomic"


    // $ANTLR start "parameter"
    // EsperEPL2Ast.g:404:1: parameter : ( ( singleParameter )=> singleParameter | ^( NUMERIC_PARAM_LIST ( numericParameterList )+ ) | ^( ARRAY_PARAM_LIST ( constant[false] )* ) | eventPropertyExpr );
    public void parameter() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:405:2: ( ( singleParameter )=> singleParameter | ^( NUMERIC_PARAM_LIST ( numericParameterList )+ ) | ^( ARRAY_PARAM_LIST ( constant[false] )* ) | eventPropertyExpr )
            int alt126 = 4;
            alt126 = dfa126.Predict(input);
            switch (alt126) 
            {
                case 1 :
                    // EsperEPL2Ast.g:405:5: ( singleParameter )=> singleParameter
                    {
                    	PushFollow(FOLLOW_singleParameter_in_parameter2999);
                    	singleParameter();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:406:5: ^( NUMERIC_PARAM_LIST ( numericParameterList )+ )
                    {
                    	Match(input,NUMERIC_PARAM_LIST,FOLLOW_NUMERIC_PARAM_LIST_in_parameter3007); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	// EsperEPL2Ast.g:406:27: ( numericParameterList )+
                    	int cnt124 = 0;
                    	do 
                    	{
                    	    int alt124 = 2;
                    	    int LA124_0 = input.LA(1);

                    	    if ( (LA124_0 == NUMERIC_PARAM_RANGE || LA124_0 == NUM_INT || LA124_0 == NUMERIC_PARAM_FREQUENCE) )
                    	    {
                    	        alt124 = 1;
                    	    }


                    	    switch (alt124) 
                    		{
                    			case 1 :
                    			    // EsperEPL2Ast.g:406:28: numericParameterList
                    			    {
                    			    	PushFollow(FOLLOW_numericParameterList_in_parameter3010);
                    			    	numericParameterList();
                    			    	state.followingStackPointer--;
                    			    	if (state.failed) return ;

                    			    }
                    			    break;

                    			default:
                    			    if ( cnt124 >= 1 ) goto loop124;
                    			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    		            EarlyExitException eee =
                    		                new EarlyExitException(124, input);
                    		            throw eee;
                    	    }
                    	    cnt124++;
                    	} while (true);

                    	loop124:
                    		;	// Stops C# compiler whinging that label 'loop124' has no statements


                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:407:4: ^( ARRAY_PARAM_LIST ( constant[false] )* )
                    {
                    	Match(input,ARRAY_PARAM_LIST,FOLLOW_ARRAY_PARAM_LIST_in_parameter3021); if (state.failed) return ;

                    	if ( input.LA(1) == Token.DOWN )
                    	{
                    	    Match(input, Token.DOWN, null); if (state.failed) return ;
                    	    // EsperEPL2Ast.g:407:24: ( constant[false] )*
                    	    do 
                    	    {
                    	        int alt125 = 2;
                    	        int LA125_0 = input.LA(1);

                    	        if ( ((LA125_0 >= INT_TYPE && LA125_0 <= NULL_TYPE)) )
                    	        {
                    	            alt125 = 1;
                    	        }


                    	        switch (alt125) 
                    	    	{
                    	    		case 1 :
                    	    		    // EsperEPL2Ast.g:407:25: constant[false]
                    	    		    {
                    	    		    	PushFollow(FOLLOW_constant_in_parameter3024);
                    	    		    	constant(false);
                    	    		    	state.followingStackPointer--;
                    	    		    	if (state.failed) return ;

                    	    		    }
                    	    		    break;

                    	    		default:
                    	    		    goto loop125;
                    	        }
                    	    } while (true);

                    	    loop125:
                    	    	;	// Stops C# compiler whining that label 'loop125' has no statements


                    	    Match(input, Token.UP, null); if (state.failed) return ;
                    	}

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:408:4: eventPropertyExpr
                    {
                    	PushFollow(FOLLOW_eventPropertyExpr_in_parameter3033);
                    	eventPropertyExpr();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "parameter"


    // $ANTLR start "singleParameter"
    // EsperEPL2Ast.g:411:1: singleParameter : ( STAR | LAST | LW | lastOperator | weekDayOperator | constant[false] | ^( NUMERIC_PARAM_RANGE NUM_INT NUM_INT ) | ^( NUMERIC_PARAM_FREQUENCY NUM_INT ) | time_period );
    public void singleParameter() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:412:2: ( STAR | LAST | LW | lastOperator | weekDayOperator | constant[false] | ^( NUMERIC_PARAM_RANGE NUM_INT NUM_INT ) | ^( NUMERIC_PARAM_FREQUENCY NUM_INT ) | time_period )
            int alt127 = 9;
            alt127 = dfa127.Predict(input);
            switch (alt127) 
            {
                case 1 :
                    // EsperEPL2Ast.g:412:5: STAR
                    {
                    	Match(input,STAR,FOLLOW_STAR_in_singleParameter3045); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:413:4: LAST
                    {
                    	Match(input,LAST,FOLLOW_LAST_in_singleParameter3050); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:414:4: LW
                    {
                    	Match(input,LW,FOLLOW_LW_in_singleParameter3055); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:415:4: lastOperator
                    {
                    	PushFollow(FOLLOW_lastOperator_in_singleParameter3060);
                    	lastOperator();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 5 :
                    // EsperEPL2Ast.g:416:4: weekDayOperator
                    {
                    	PushFollow(FOLLOW_weekDayOperator_in_singleParameter3065);
                    	weekDayOperator();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 6 :
                    // EsperEPL2Ast.g:417:5: constant[false]
                    {
                    	PushFollow(FOLLOW_constant_in_singleParameter3071);
                    	constant(false);
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;
                case 7 :
                    // EsperEPL2Ast.g:418:5: ^( NUMERIC_PARAM_RANGE NUM_INT NUM_INT )
                    {
                    	Match(input,NUMERIC_PARAM_RANGE,FOLLOW_NUMERIC_PARAM_RANGE_in_singleParameter3080); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	Match(input,NUM_INT,FOLLOW_NUM_INT_in_singleParameter3082); if (state.failed) return ;
                    	Match(input,NUM_INT,FOLLOW_NUM_INT_in_singleParameter3084); if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 8 :
                    // EsperEPL2Ast.g:419:5: ^( NUMERIC_PARAM_FREQUENCY NUM_INT )
                    {
                    	Match(input,NUMERIC_PARAM_FREQUENCY,FOLLOW_NUMERIC_PARAM_FREQUENCY_in_singleParameter3093); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	Match(input,NUM_INT,FOLLOW_NUM_INT_in_singleParameter3095); if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 9 :
                    // EsperEPL2Ast.g:420:5: time_period
                    {
                    	PushFollow(FOLLOW_time_period_in_singleParameter3102);
                    	time_period();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "singleParameter"


    // $ANTLR start "numericParameterList"
    // EsperEPL2Ast.g:423:1: numericParameterList : ( NUM_INT | ^( NUMERIC_PARAM_RANGE NUM_INT NUM_INT ) | ^( NUMERIC_PARAM_FREQUENCE NUM_INT ) );
    public void numericParameterList() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:424:2: ( NUM_INT | ^( NUMERIC_PARAM_RANGE NUM_INT NUM_INT ) | ^( NUMERIC_PARAM_FREQUENCE NUM_INT ) )
            int alt128 = 3;
            switch ( input.LA(1) ) 
            {
            case NUM_INT:
            	{
                alt128 = 1;
                }
                break;
            case NUMERIC_PARAM_RANGE:
            	{
                alt128 = 2;
                }
                break;
            case NUMERIC_PARAM_FREQUENCE:
            	{
                alt128 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d128s0 =
            	        new NoViableAltException("", 128, 0, input);

            	    throw nvae_d128s0;
            }

            switch (alt128) 
            {
                case 1 :
                    // EsperEPL2Ast.g:424:5: NUM_INT
                    {
                    	Match(input,NUM_INT,FOLLOW_NUM_INT_in_numericParameterList3114); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:425:5: ^( NUMERIC_PARAM_RANGE NUM_INT NUM_INT )
                    {
                    	Match(input,NUMERIC_PARAM_RANGE,FOLLOW_NUMERIC_PARAM_RANGE_in_numericParameterList3122); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	Match(input,NUM_INT,FOLLOW_NUM_INT_in_numericParameterList3124); if (state.failed) return ;
                    	Match(input,NUM_INT,FOLLOW_NUM_INT_in_numericParameterList3126); if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:426:5: ^( NUMERIC_PARAM_FREQUENCE NUM_INT )
                    {
                    	Match(input,NUMERIC_PARAM_FREQUENCE,FOLLOW_NUMERIC_PARAM_FREQUENCE_in_numericParameterList3135); if (state.failed) return ;

                    	Match(input, Token.DOWN, null); if (state.failed) return ;
                    	Match(input,NUM_INT,FOLLOW_NUM_INT_in_numericParameterList3137); if (state.failed) return ;

                    	Match(input, Token.UP, null); if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "numericParameterList"


    // $ANTLR start "lastOperator"
    // EsperEPL2Ast.g:429:1: lastOperator : ^( LAST_OPERATOR NUM_INT ) ;
    public void lastOperator() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:430:2: ( ^( LAST_OPERATOR NUM_INT ) )
            // EsperEPL2Ast.g:430:4: ^( LAST_OPERATOR NUM_INT )
            {
            	Match(input,LAST_OPERATOR,FOLLOW_LAST_OPERATOR_in_lastOperator3151); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	Match(input,NUM_INT,FOLLOW_NUM_INT_in_lastOperator3153); if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "lastOperator"


    // $ANTLR start "weekDayOperator"
    // EsperEPL2Ast.g:433:1: weekDayOperator : ^( WEEKDAY_OPERATOR NUM_INT ) ;
    public void weekDayOperator() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:434:2: ( ^( WEEKDAY_OPERATOR NUM_INT ) )
            // EsperEPL2Ast.g:434:4: ^( WEEKDAY_OPERATOR NUM_INT )
            {
            	Match(input,WEEKDAY_OPERATOR,FOLLOW_WEEKDAY_OPERATOR_in_weekDayOperator3168); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	Match(input,NUM_INT,FOLLOW_NUM_INT_in_weekDayOperator3170); if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "weekDayOperator"


    // $ANTLR start "time_period"
    // EsperEPL2Ast.g:437:1: time_period : ^( TIME_PERIOD timePeriodDef ) ;
    public void time_period() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:438:2: ( ^( TIME_PERIOD timePeriodDef ) )
            // EsperEPL2Ast.g:438:5: ^( TIME_PERIOD timePeriodDef )
            {
            	Match(input,TIME_PERIOD,FOLLOW_TIME_PERIOD_in_time_period3186); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_timePeriodDef_in_time_period3188);
            	timePeriodDef();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "time_period"


    // $ANTLR start "timePeriodDef"
    // EsperEPL2Ast.g:441:1: timePeriodDef : ( dayPart ( hourPart )? ( minutePart )? ( secondPart )? ( millisecondPart )? | hourPart ( minutePart )? ( secondPart )? ( millisecondPart )? | minutePart ( secondPart )? ( millisecondPart )? | secondPart ( millisecondPart )? | millisecondPart );
    public void timePeriodDef() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:442:2: ( dayPart ( hourPart )? ( minutePart )? ( secondPart )? ( millisecondPart )? | hourPart ( minutePart )? ( secondPart )? ( millisecondPart )? | minutePart ( secondPart )? ( millisecondPart )? | secondPart ( millisecondPart )? | millisecondPart )
            int alt139 = 5;
            switch ( input.LA(1) ) 
            {
            case DAY_PART:
            	{
                alt139 = 1;
                }
                break;
            case HOUR_PART:
            	{
                alt139 = 2;
                }
                break;
            case MINUTE_PART:
            	{
                alt139 = 3;
                }
                break;
            case SECOND_PART:
            	{
                alt139 = 4;
                }
                break;
            case MILLISECOND_PART:
            	{
                alt139 = 5;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d139s0 =
            	        new NoViableAltException("", 139, 0, input);

            	    throw nvae_d139s0;
            }

            switch (alt139) 
            {
                case 1 :
                    // EsperEPL2Ast.g:442:5: dayPart ( hourPart )? ( minutePart )? ( secondPart )? ( millisecondPart )?
                    {
                    	PushFollow(FOLLOW_dayPart_in_timePeriodDef3203);
                    	dayPart();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:442:13: ( hourPart )?
                    	int alt129 = 2;
                    	int LA129_0 = input.LA(1);

                    	if ( (LA129_0 == HOUR_PART) )
                    	{
                    	    alt129 = 1;
                    	}
                    	switch (alt129) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:442:14: hourPart
                    	        {
                    	        	PushFollow(FOLLOW_hourPart_in_timePeriodDef3206);
                    	        	hourPart();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:442:25: ( minutePart )?
                    	int alt130 = 2;
                    	int LA130_0 = input.LA(1);

                    	if ( (LA130_0 == MINUTE_PART) )
                    	{
                    	    alt130 = 1;
                    	}
                    	switch (alt130) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:442:26: minutePart
                    	        {
                    	        	PushFollow(FOLLOW_minutePart_in_timePeriodDef3211);
                    	        	minutePart();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:442:39: ( secondPart )?
                    	int alt131 = 2;
                    	int LA131_0 = input.LA(1);

                    	if ( (LA131_0 == SECOND_PART) )
                    	{
                    	    alt131 = 1;
                    	}
                    	switch (alt131) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:442:40: secondPart
                    	        {
                    	        	PushFollow(FOLLOW_secondPart_in_timePeriodDef3216);
                    	        	secondPart();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:442:53: ( millisecondPart )?
                    	int alt132 = 2;
                    	int LA132_0 = input.LA(1);

                    	if ( (LA132_0 == MILLISECOND_PART) )
                    	{
                    	    alt132 = 1;
                    	}
                    	switch (alt132) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:442:54: millisecondPart
                    	        {
                    	        	PushFollow(FOLLOW_millisecondPart_in_timePeriodDef3221);
                    	        	millisecondPart();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:443:4: hourPart ( minutePart )? ( secondPart )? ( millisecondPart )?
                    {
                    	PushFollow(FOLLOW_hourPart_in_timePeriodDef3228);
                    	hourPart();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:443:13: ( minutePart )?
                    	int alt133 = 2;
                    	int LA133_0 = input.LA(1);

                    	if ( (LA133_0 == MINUTE_PART) )
                    	{
                    	    alt133 = 1;
                    	}
                    	switch (alt133) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:443:14: minutePart
                    	        {
                    	        	PushFollow(FOLLOW_minutePart_in_timePeriodDef3231);
                    	        	minutePart();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:443:27: ( secondPart )?
                    	int alt134 = 2;
                    	int LA134_0 = input.LA(1);

                    	if ( (LA134_0 == SECOND_PART) )
                    	{
                    	    alt134 = 1;
                    	}
                    	switch (alt134) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:443:28: secondPart
                    	        {
                    	        	PushFollow(FOLLOW_secondPart_in_timePeriodDef3236);
                    	        	secondPart();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:443:41: ( millisecondPart )?
                    	int alt135 = 2;
                    	int LA135_0 = input.LA(1);

                    	if ( (LA135_0 == MILLISECOND_PART) )
                    	{
                    	    alt135 = 1;
                    	}
                    	switch (alt135) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:443:42: millisecondPart
                    	        {
                    	        	PushFollow(FOLLOW_millisecondPart_in_timePeriodDef3241);
                    	        	millisecondPart();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:444:4: minutePart ( secondPart )? ( millisecondPart )?
                    {
                    	PushFollow(FOLLOW_minutePart_in_timePeriodDef3248);
                    	minutePart();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:444:15: ( secondPart )?
                    	int alt136 = 2;
                    	int LA136_0 = input.LA(1);

                    	if ( (LA136_0 == SECOND_PART) )
                    	{
                    	    alt136 = 1;
                    	}
                    	switch (alt136) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:444:16: secondPart
                    	        {
                    	        	PushFollow(FOLLOW_secondPart_in_timePeriodDef3251);
                    	        	secondPart();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}

                    	// EsperEPL2Ast.g:444:29: ( millisecondPart )?
                    	int alt137 = 2;
                    	int LA137_0 = input.LA(1);

                    	if ( (LA137_0 == MILLISECOND_PART) )
                    	{
                    	    alt137 = 1;
                    	}
                    	switch (alt137) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:444:30: millisecondPart
                    	        {
                    	        	PushFollow(FOLLOW_millisecondPart_in_timePeriodDef3256);
                    	        	millisecondPart();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:445:4: secondPart ( millisecondPart )?
                    {
                    	PushFollow(FOLLOW_secondPart_in_timePeriodDef3263);
                    	secondPart();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;
                    	// EsperEPL2Ast.g:445:15: ( millisecondPart )?
                    	int alt138 = 2;
                    	int LA138_0 = input.LA(1);

                    	if ( (LA138_0 == MILLISECOND_PART) )
                    	{
                    	    alt138 = 1;
                    	}
                    	switch (alt138) 
                    	{
                    	    case 1 :
                    	        // EsperEPL2Ast.g:445:16: millisecondPart
                    	        {
                    	        	PushFollow(FOLLOW_millisecondPart_in_timePeriodDef3266);
                    	        	millisecondPart();
                    	        	state.followingStackPointer--;
                    	        	if (state.failed) return ;

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 5 :
                    // EsperEPL2Ast.g:446:4: millisecondPart
                    {
                    	PushFollow(FOLLOW_millisecondPart_in_timePeriodDef3273);
                    	millisecondPart();
                    	state.followingStackPointer--;
                    	if (state.failed) return ;

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "timePeriodDef"


    // $ANTLR start "dayPart"
    // EsperEPL2Ast.g:449:1: dayPart : ^( DAY_PART number ) ;
    public void dayPart() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:450:2: ( ^( DAY_PART number ) )
            // EsperEPL2Ast.g:450:4: ^( DAY_PART number )
            {
            	Match(input,DAY_PART,FOLLOW_DAY_PART_in_dayPart3287); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_number_in_dayPart3289);
            	number();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "dayPart"


    // $ANTLR start "hourPart"
    // EsperEPL2Ast.g:453:1: hourPart : ^( HOUR_PART number ) ;
    public void hourPart() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:454:2: ( ^( HOUR_PART number ) )
            // EsperEPL2Ast.g:454:4: ^( HOUR_PART number )
            {
            	Match(input,HOUR_PART,FOLLOW_HOUR_PART_in_hourPart3303); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_number_in_hourPart3305);
            	number();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "hourPart"


    // $ANTLR start "minutePart"
    // EsperEPL2Ast.g:457:1: minutePart : ^( MINUTE_PART number ) ;
    public void minutePart() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:458:2: ( ^( MINUTE_PART number ) )
            // EsperEPL2Ast.g:458:4: ^( MINUTE_PART number )
            {
            	Match(input,MINUTE_PART,FOLLOW_MINUTE_PART_in_minutePart3319); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_number_in_minutePart3321);
            	number();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "minutePart"


    // $ANTLR start "secondPart"
    // EsperEPL2Ast.g:461:1: secondPart : ^( SECOND_PART number ) ;
    public void secondPart() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:462:2: ( ^( SECOND_PART number ) )
            // EsperEPL2Ast.g:462:4: ^( SECOND_PART number )
            {
            	Match(input,SECOND_PART,FOLLOW_SECOND_PART_in_secondPart3335); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_number_in_secondPart3337);
            	number();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "secondPart"


    // $ANTLR start "millisecondPart"
    // EsperEPL2Ast.g:465:1: millisecondPart : ^( MILLISECOND_PART number ) ;
    public void millisecondPart() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:466:2: ( ^( MILLISECOND_PART number ) )
            // EsperEPL2Ast.g:466:4: ^( MILLISECOND_PART number )
            {
            	Match(input,MILLISECOND_PART,FOLLOW_MILLISECOND_PART_in_millisecondPart3351); if (state.failed) return ;

            	Match(input, Token.DOWN, null); if (state.failed) return ;
            	PushFollow(FOLLOW_number_in_millisecondPart3353);
            	number();
            	state.followingStackPointer--;
            	if (state.failed) return ;

            	Match(input, Token.UP, null); if (state.failed) return ;

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "millisecondPart"


    // $ANTLR start "substitution"
    // EsperEPL2Ast.g:469:1: substitution : s= SUBSTITUTION ;
    public void substitution() // throws RecognitionException [1]
    {   
        CommonTree s = null;

        try 
    	{
            // EsperEPL2Ast.g:470:2: (s= SUBSTITUTION )
            // EsperEPL2Ast.g:470:4: s= SUBSTITUTION
            {
            	s=(CommonTree)Match(input,SUBSTITUTION,FOLLOW_SUBSTITUTION_in_substitution3367); if (state.failed) return ;
            	if ( state.backtracking == 0 ) 
            	{
            	   LeaveNode(s); 
            	}

            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "substitution"


    // $ANTLR start "constant"
    // EsperEPL2Ast.g:473:1: constant[bool isLeaveNode] : (c= INT_TYPE | c= LONG_TYPE | c= FLOAT_TYPE | c= DOUBLE_TYPE | c= STRING_TYPE | c= BOOL_TYPE | c= NULL_TYPE );
    public void constant(bool isLeaveNode) // throws RecognitionException [1]
    {   
        CommonTree c = null;

        try 
    	{
            // EsperEPL2Ast.g:474:2: (c= INT_TYPE | c= LONG_TYPE | c= FLOAT_TYPE | c= DOUBLE_TYPE | c= STRING_TYPE | c= BOOL_TYPE | c= NULL_TYPE )
            int alt140 = 7;
            switch ( input.LA(1) ) 
            {
            case INT_TYPE:
            	{
                alt140 = 1;
                }
                break;
            case LONG_TYPE:
            	{
                alt140 = 2;
                }
                break;
            case FLOAT_TYPE:
            	{
                alt140 = 3;
                }
                break;
            case DOUBLE_TYPE:
            	{
                alt140 = 4;
                }
                break;
            case STRING_TYPE:
            	{
                alt140 = 5;
                }
                break;
            case BOOL_TYPE:
            	{
                alt140 = 6;
                }
                break;
            case NULL_TYPE:
            	{
                alt140 = 7;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    NoViableAltException nvae_d140s0 =
            	        new NoViableAltException("", 140, 0, input);

            	    throw nvae_d140s0;
            }

            switch (alt140) 
            {
                case 1 :
                    // EsperEPL2Ast.g:474:4: c= INT_TYPE
                    {
                    	c=(CommonTree)Match(input,INT_TYPE,FOLLOW_INT_TYPE_in_constant3383); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   if (isLeaveNode) LeaveNode(c); 
                    	}

                    }
                    break;
                case 2 :
                    // EsperEPL2Ast.g:475:4: c= LONG_TYPE
                    {
                    	c=(CommonTree)Match(input,LONG_TYPE,FOLLOW_LONG_TYPE_in_constant3392); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   if (isLeaveNode) LeaveNode(c); 
                    	}

                    }
                    break;
                case 3 :
                    // EsperEPL2Ast.g:476:4: c= FLOAT_TYPE
                    {
                    	c=(CommonTree)Match(input,FLOAT_TYPE,FOLLOW_FLOAT_TYPE_in_constant3401); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   if (isLeaveNode) LeaveNode(c); 
                    	}

                    }
                    break;
                case 4 :
                    // EsperEPL2Ast.g:477:4: c= DOUBLE_TYPE
                    {
                    	c=(CommonTree)Match(input,DOUBLE_TYPE,FOLLOW_DOUBLE_TYPE_in_constant3410); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   if (isLeaveNode) LeaveNode(c); 
                    	}

                    }
                    break;
                case 5 :
                    // EsperEPL2Ast.g:478:11: c= STRING_TYPE
                    {
                    	c=(CommonTree)Match(input,STRING_TYPE,FOLLOW_STRING_TYPE_in_constant3426); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   if (isLeaveNode) LeaveNode(c); 
                    	}

                    }
                    break;
                case 6 :
                    // EsperEPL2Ast.g:479:11: c= BOOL_TYPE
                    {
                    	c=(CommonTree)Match(input,BOOL_TYPE,FOLLOW_BOOL_TYPE_in_constant3442); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   if (isLeaveNode) LeaveNode(c); 
                    	}

                    }
                    break;
                case 7 :
                    // EsperEPL2Ast.g:480:8: c= NULL_TYPE
                    {
                    	c=(CommonTree)Match(input,NULL_TYPE,FOLLOW_NULL_TYPE_in_constant3455); if (state.failed) return ;
                    	if ( state.backtracking == 0 ) 
                    	{
                    	   if (isLeaveNode) LeaveNode(c); 
                    	}

                    }
                    break;

            }
        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "constant"


    // $ANTLR start "number"
    // EsperEPL2Ast.g:483:1: number : ( INT_TYPE | LONG_TYPE | FLOAT_TYPE | DOUBLE_TYPE );
    public void number() // throws RecognitionException [1]
    {   
        try 
    	{
            // EsperEPL2Ast.g:484:2: ( INT_TYPE | LONG_TYPE | FLOAT_TYPE | DOUBLE_TYPE )
            // EsperEPL2Ast.g:
            {
            	if ( (input.LA(1) >= INT_TYPE && input.LA(1) <= DOUBLE_TYPE) ) 
            	{
            	    input.Consume();
            	    state.errorRecovery = false;state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    throw mse;
            	}


            }

        }

          catch (RecognitionException rex) {
            throw rex;
          }
        finally 
    	{
        }
        return ;
    }
    // $ANTLR end "number"

    // $ANTLR start "synpred1_EsperEPL2Ast"
    public void synpred1_EsperEPL2Ast_fragment() //throws RecognitionException
    {   
        // EsperEPL2Ast.g:405:5: ( singleParameter )
        // EsperEPL2Ast.g:405:6: singleParameter
        {
        	PushFollow(FOLLOW_singleParameter_in_synpred1_EsperEPL2Ast2994);
        	singleParameter();
        	state.followingStackPointer--;
        	if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred1_EsperEPL2Ast"

    // Delegated rules

   	public bool synpred1_EsperEPL2Ast() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred1_EsperEPL2Ast_fragment(); // can never throw exception
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


   	protected DFA15 dfa15;
   	protected DFA20 dfa20;
   	protected DFA30 dfa30;
   	protected DFA29 dfa29;
   	protected DFA44 dfa44;
   	protected DFA46 dfa46;
   	protected DFA47 dfa47;
   	protected DFA61 dfa61;
   	protected DFA59 dfa59;
   	protected DFA60 dfa60;
   	protected DFA62 dfa62;
   	protected DFA69 dfa69;
   	protected DFA70 dfa70;
   	protected DFA72 dfa72;
   	protected DFA73 dfa73;
   	protected DFA75 dfa75;
   	protected DFA77 dfa77;
   	protected DFA78 dfa78;
   	protected DFA90 dfa90;
   	protected DFA81 dfa81;
   	protected DFA82 dfa82;
   	protected DFA84 dfa84;
   	protected DFA83 dfa83;
   	protected DFA85 dfa85;
   	protected DFA86 dfa86;
   	protected DFA87 dfa87;
   	protected DFA88 dfa88;
   	protected DFA91 dfa91;
   	protected DFA93 dfa93;
   	protected DFA92 dfa92;
   	protected DFA95 dfa95;
   	protected DFA96 dfa96;
   	protected DFA97 dfa97;
   	protected DFA99 dfa99;
   	protected DFA100 dfa100;
   	protected DFA101 dfa101;
   	protected DFA103 dfa103;
   	protected DFA106 dfa106;
   	protected DFA107 dfa107;
   	protected DFA120 dfa120;
   	protected DFA113 dfa113;
   	protected DFA115 dfa115;
   	protected DFA126 dfa126;
   	protected DFA127 dfa127;
	private void InitializeCyclicDFAs()
	{
    	this.dfa15 = new DFA15(this);
    	this.dfa20 = new DFA20(this);
    	this.dfa30 = new DFA30(this);
    	this.dfa29 = new DFA29(this);
    	this.dfa44 = new DFA44(this);
    	this.dfa46 = new DFA46(this);
    	this.dfa47 = new DFA47(this);
    	this.dfa61 = new DFA61(this);
    	this.dfa59 = new DFA59(this);
    	this.dfa60 = new DFA60(this);
    	this.dfa62 = new DFA62(this);
    	this.dfa69 = new DFA69(this);
    	this.dfa70 = new DFA70(this);
    	this.dfa72 = new DFA72(this);
    	this.dfa73 = new DFA73(this);
    	this.dfa75 = new DFA75(this);
    	this.dfa77 = new DFA77(this);
    	this.dfa78 = new DFA78(this);
    	this.dfa90 = new DFA90(this);
    	this.dfa81 = new DFA81(this);
    	this.dfa82 = new DFA82(this);
    	this.dfa84 = new DFA84(this);
    	this.dfa83 = new DFA83(this);
    	this.dfa85 = new DFA85(this);
    	this.dfa86 = new DFA86(this);
    	this.dfa87 = new DFA87(this);
    	this.dfa88 = new DFA88(this);
    	this.dfa91 = new DFA91(this);
    	this.dfa93 = new DFA93(this);
    	this.dfa92 = new DFA92(this);
    	this.dfa95 = new DFA95(this);
    	this.dfa96 = new DFA96(this);
    	this.dfa97 = new DFA97(this);
    	this.dfa99 = new DFA99(this);
    	this.dfa100 = new DFA100(this);
    	this.dfa101 = new DFA101(this);
    	this.dfa103 = new DFA103(this);
    	this.dfa106 = new DFA106(this);
    	this.dfa107 = new DFA107(this);
    	this.dfa120 = new DFA120(this);
    	this.dfa113 = new DFA113(this);
    	this.dfa115 = new DFA115(this);
    	this.dfa126 = new DFA126(this);
    	this.dfa127 = new DFA127(this);










































	    this.dfa126.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA126_SpecialStateTransition);

	}

    const string DFA15_eotS =
        "\x3c\uffff";
    const string DFA15_eofS =
        "\x3c\uffff";
    const string DFA15_minS =
        "\x01\x03\x3b\uffff";
    const string DFA15_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA15_acceptS =
        "\x01\uffff\x01\x01\x39\uffff\x01\x02";
    const string DFA15_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA15_transitionS = {
            "\x01\x3b\x02\uffff\x04\x01\x03\uffff\x01\x01\x03\uffff\x02"+
            "\x01\x02\uffff\x05\x01\x01\uffff\x02\x01\x23\uffff\x03\x01\x02"+
            "\uffff\x03\x01\x1b\uffff\x04\x01\x0c\uffff\x01\x01\x0c\uffff"+
            "\x02\x01\x02\uffff\x01\x01\x05\uffff\x04\x01\x05\uffff\x06\x01"+
            "\x03\uffff\x01\x01\x0a\uffff\x07\x01\x06\uffff\x02\x01\x06\uffff"+
            "\x01\x01\x05\uffff\x03\x01\x02\uffff\x04\x01\x01\uffff\x02\x01",
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
            ""
    };

    static readonly short[] DFA15_eot = DFA.UnpackEncodedString(DFA15_eotS);
    static readonly short[] DFA15_eof = DFA.UnpackEncodedString(DFA15_eofS);
    static readonly char[] DFA15_min = DFA.UnpackEncodedStringToUnsignedChars(DFA15_minS);
    static readonly char[] DFA15_max = DFA.UnpackEncodedStringToUnsignedChars(DFA15_maxS);
    static readonly short[] DFA15_accept = DFA.UnpackEncodedString(DFA15_acceptS);
    static readonly short[] DFA15_special = DFA.UnpackEncodedString(DFA15_specialS);
    static readonly short[][] DFA15_transition = DFA.UnpackEncodedStringArray(DFA15_transitionS);

    protected class DFA15 : DFA
    {
        public DFA15(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 15;
            this.eot = DFA15_eot;
            this.eof = DFA15_eof;
            this.min = DFA15_min;
            this.max = DFA15_max;
            this.accept = DFA15_accept;
            this.special = DFA15_special;
            this.transition = DFA15_transition;

        }

        override public string Description
        {
            get { return "98:41: ( valueExpr )?"; }
        }

    }

    const string DFA20_eotS =
        "\x0a\uffff";
    const string DFA20_eofS =
        "\x0a\uffff";
    const string DFA20_minS =
        "\x01\x03\x09\uffff";
    const string DFA20_maxS =
        "\x01\x7d\x09\uffff";
    const string DFA20_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x07\uffff";
    const string DFA20_specialS =
        "\x0a\uffff}>";
    static readonly string[] DFA20_transitionS = {
            "\x01\x02\x5c\uffff\x01\x01\x01\x02\x0e\uffff\x02\x02\x08\uffff"+
            "\x04\x02",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA20_eot = DFA.UnpackEncodedString(DFA20_eotS);
    static readonly short[] DFA20_eof = DFA.UnpackEncodedString(DFA20_eofS);
    static readonly char[] DFA20_min = DFA.UnpackEncodedStringToUnsignedChars(DFA20_minS);
    static readonly char[] DFA20_max = DFA.UnpackEncodedStringToUnsignedChars(DFA20_maxS);
    static readonly short[] DFA20_accept = DFA.UnpackEncodedString(DFA20_acceptS);
    static readonly short[] DFA20_special = DFA.UnpackEncodedString(DFA20_specialS);
    static readonly short[][] DFA20_transition = DFA.UnpackEncodedStringArray(DFA20_transitionS);

    protected class DFA20 : DFA
    {
        public DFA20(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 20;
            this.eot = DFA20_eot;
            this.eof = DFA20_eof;
            this.min = DFA20_min;
            this.max = DFA20_max;
            this.accept = DFA20_accept;
            this.special = DFA20_special;
            this.transition = DFA20_transition;

        }

        override public string Description
        {
            get { return "114:3: ( whereClause )?"; }
        }

    }

    const string DFA30_eotS =
        "\x0b\uffff";
    const string DFA30_eofS =
        "\x0b\uffff";
    const string DFA30_minS =
        "\x01\x03\x0a\uffff";
    const string DFA30_maxS =
        "\x01\x7d\x0a\uffff";
    const string DFA30_acceptS =
        "\x01\uffff\x01\x02\x08\uffff\x01\x01";
    const string DFA30_specialS =
        "\x0b\uffff}>";
    static readonly string[] DFA30_transitionS = {
            "\x01\x01\x5c\uffff\x02\x01\x09\uffff\x01\x0a\x04\uffff\x02"+
            "\x01\x08\uffff\x04\x01",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA30_eot = DFA.UnpackEncodedString(DFA30_eotS);
    static readonly short[] DFA30_eof = DFA.UnpackEncodedString(DFA30_eofS);
    static readonly char[] DFA30_min = DFA.UnpackEncodedStringToUnsignedChars(DFA30_minS);
    static readonly char[] DFA30_max = DFA.UnpackEncodedStringToUnsignedChars(DFA30_maxS);
    static readonly short[] DFA30_accept = DFA.UnpackEncodedString(DFA30_acceptS);
    static readonly short[] DFA30_special = DFA.UnpackEncodedString(DFA30_specialS);
    static readonly short[][] DFA30_transition = DFA.UnpackEncodedStringArray(DFA30_transitionS);

    protected class DFA30 : DFA
    {
        public DFA30(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 30;
            this.eot = DFA30_eot;
            this.eof = DFA30_eof;
            this.min = DFA30_min;
            this.max = DFA30_max;
            this.accept = DFA30_accept;
            this.special = DFA30_special;
            this.transition = DFA30_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 134:21: ( streamExpression ( outerJoin )* )*"; }
        }

    }

    const string DFA29_eotS =
        "\x0e\uffff";
    const string DFA29_eofS =
        "\x0e\uffff";
    const string DFA29_minS =
        "\x01\x03\x0d\uffff";
    const string DFA29_maxS =
        "\x01\x7d\x0d\uffff";
    const string DFA29_acceptS =
        "\x01\uffff\x01\x02\x09\uffff\x01\x01\x02\uffff";
    const string DFA29_specialS =
        "\x0e\uffff}>";
    static readonly string[] DFA29_transitionS = {
            "\x01\x01\x5c\uffff\x02\x01\x09\uffff\x01\x01\x01\uffff\x03"+
            "\x0b\x02\x01\x08\uffff\x04\x01",
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
            ""
    };

    static readonly short[] DFA29_eot = DFA.UnpackEncodedString(DFA29_eotS);
    static readonly short[] DFA29_eof = DFA.UnpackEncodedString(DFA29_eofS);
    static readonly char[] DFA29_min = DFA.UnpackEncodedStringToUnsignedChars(DFA29_minS);
    static readonly char[] DFA29_max = DFA.UnpackEncodedStringToUnsignedChars(DFA29_maxS);
    static readonly short[] DFA29_accept = DFA.UnpackEncodedString(DFA29_acceptS);
    static readonly short[] DFA29_special = DFA.UnpackEncodedString(DFA29_specialS);
    static readonly short[][] DFA29_transition = DFA.UnpackEncodedStringArray(DFA29_transitionS);

    protected class DFA29 : DFA
    {
        public DFA29(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 29;
            this.eot = DFA29_eot;
            this.eof = DFA29_eof;
            this.min = DFA29_min;
            this.max = DFA29_max;
            this.accept = DFA29_accept;
            this.special = DFA29_special;
            this.transition = DFA29_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 134:39: ( outerJoin )*"; }
        }

    }

    const string DFA44_eotS =
        "\x3c\uffff";
    const string DFA44_eofS =
        "\x3c\uffff";
    const string DFA44_minS =
        "\x01\x03\x3b\uffff";
    const string DFA44_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA44_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA44_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA44_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA44_eot = DFA.UnpackEncodedString(DFA44_eotS);
    static readonly short[] DFA44_eof = DFA.UnpackEncodedString(DFA44_eofS);
    static readonly char[] DFA44_min = DFA.UnpackEncodedStringToUnsignedChars(DFA44_minS);
    static readonly char[] DFA44_max = DFA.UnpackEncodedStringToUnsignedChars(DFA44_maxS);
    static readonly short[] DFA44_accept = DFA.UnpackEncodedString(DFA44_acceptS);
    static readonly short[] DFA44_special = DFA.UnpackEncodedString(DFA44_specialS);
    static readonly short[][] DFA44_transition = DFA.UnpackEncodedStringArray(DFA44_transitionS);

    protected class DFA44 : DFA
    {
        public DFA44(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 44;
            this.eot = DFA44_eot;
            this.eof = DFA44_eof;
            this.min = DFA44_min;
            this.max = DFA44_max;
            this.accept = DFA44_accept;
            this.special = DFA44_special;
            this.transition = DFA44_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 170:41: ( valueExpr )*"; }
        }

    }

    const string DFA46_eotS =
        "\x14\uffff";
    const string DFA46_eofS =
        "\x14\uffff";
    const string DFA46_minS =
        "\x01\x03\x13\uffff";
    const string DFA46_maxS =
        "\x01\u00bc\x13\uffff";
    const string DFA46_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x11\uffff";
    const string DFA46_specialS =
        "\x14\uffff}>";
    static readonly string[] DFA46_transitionS = {
            "\x01\x01\x2f\uffff\x01\x02\x10\uffff\x01\x02\x07\uffff\x03"+
            "\x02\x01\uffff\x01\x02\x22\uffff\x01\x02\x0f\uffff\x01\x02\x16"+
            "\uffff\x02\x02\x0b\uffff\x07\x02\x0e\uffff\x01\x02",
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
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA46_eot = DFA.UnpackEncodedString(DFA46_eotS);
    static readonly short[] DFA46_eof = DFA.UnpackEncodedString(DFA46_eofS);
    static readonly char[] DFA46_min = DFA.UnpackEncodedStringToUnsignedChars(DFA46_minS);
    static readonly char[] DFA46_max = DFA.UnpackEncodedStringToUnsignedChars(DFA46_maxS);
    static readonly short[] DFA46_accept = DFA.UnpackEncodedString(DFA46_acceptS);
    static readonly short[] DFA46_special = DFA.UnpackEncodedString(DFA46_specialS);
    static readonly short[][] DFA46_transition = DFA.UnpackEncodedStringArray(DFA46_transitionS);

    protected class DFA46 : DFA
    {
        public DFA46(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 46;
            this.eot = DFA46_eot;
            this.eof = DFA46_eof;
            this.min = DFA46_min;
            this.max = DFA46_max;
            this.accept = DFA46_accept;
            this.special = DFA46_special;
            this.transition = DFA46_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 178:30: ( parameter )*"; }
        }

    }

    const string DFA47_eotS =
        "\x3c\uffff";
    const string DFA47_eofS =
        "\x3c\uffff";
    const string DFA47_minS =
        "\x01\x03\x3b\uffff";
    const string DFA47_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA47_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA47_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA47_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA47_eot = DFA.UnpackEncodedString(DFA47_eotS);
    static readonly short[] DFA47_eof = DFA.UnpackEncodedString(DFA47_eofS);
    static readonly char[] DFA47_min = DFA.UnpackEncodedStringToUnsignedChars(DFA47_minS);
    static readonly char[] DFA47_max = DFA.UnpackEncodedStringToUnsignedChars(DFA47_maxS);
    static readonly short[] DFA47_accept = DFA.UnpackEncodedString(DFA47_acceptS);
    static readonly short[] DFA47_special = DFA.UnpackEncodedString(DFA47_specialS);
    static readonly short[][] DFA47_transition = DFA.UnpackEncodedStringArray(DFA47_transitionS);

    protected class DFA47 : DFA
    {
        public DFA47(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 47;
            this.eot = DFA47_eot;
            this.eof = DFA47_eof;
            this.min = DFA47_min;
            this.max = DFA47_max;
            this.accept = DFA47_accept;
            this.special = DFA47_special;
            this.transition = DFA47_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 186:32: ( valueExpr )*"; }
        }

    }

    const string DFA61_eotS =
        "\x0a\uffff";
    const string DFA61_eofS =
        "\x0a\uffff";
    const string DFA61_minS =
        "\x01\x0d\x09\uffff";
    const string DFA61_maxS =
        "\x01\u00ca\x09\uffff";
    const string DFA61_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x01\x06\x03"+
        "\uffff";
    const string DFA61_specialS =
        "\x0a\uffff}>";
    static readonly string[] DFA61_transitionS = {
            "\x01\x05\x55\uffff\x01\x02\x01\x01\x01\x03\x01\x04\x60\uffff"+
            "\x04\x06",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA61_eot = DFA.UnpackEncodedString(DFA61_eotS);
    static readonly short[] DFA61_eof = DFA.UnpackEncodedString(DFA61_eofS);
    static readonly char[] DFA61_min = DFA.UnpackEncodedStringToUnsignedChars(DFA61_minS);
    static readonly char[] DFA61_max = DFA.UnpackEncodedStringToUnsignedChars(DFA61_maxS);
    static readonly short[] DFA61_accept = DFA.UnpackEncodedString(DFA61_acceptS);
    static readonly short[] DFA61_special = DFA.UnpackEncodedString(DFA61_specialS);
    static readonly short[][] DFA61_transition = DFA.UnpackEncodedStringArray(DFA61_transitionS);

    protected class DFA61 : DFA
    {
        public DFA61(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 61;
            this.eot = DFA61_eot;
            this.eof = DFA61_eof;
            this.min = DFA61_min;
            this.max = DFA61_max;
            this.accept = DFA61_accept;
            this.special = DFA61_special;
            this.transition = DFA61_transition;

        }

        override public string Description
        {
            get { return "215:1: evalExprChoice : ( ^(jo= EVAL_OR_EXPR valueExpr valueExpr ( valueExpr )* ) | ^(ja= EVAL_AND_EXPR valueExpr valueExpr ( valueExpr )* ) | ^(je= EVAL_EQUALS_EXPR valueExpr valueExpr ) | ^(jne= EVAL_NOTEQUALS_EXPR valueExpr valueExpr ) | ^(n= NOT_EXPR valueExpr ) | r= relationalExpr );"; }
        }

    }

    const string DFA59_eotS =
        "\x3c\uffff";
    const string DFA59_eofS =
        "\x3c\uffff";
    const string DFA59_minS =
        "\x01\x03\x3b\uffff";
    const string DFA59_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA59_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA59_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA59_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA59_eot = DFA.UnpackEncodedString(DFA59_eotS);
    static readonly short[] DFA59_eof = DFA.UnpackEncodedString(DFA59_eofS);
    static readonly char[] DFA59_min = DFA.UnpackEncodedStringToUnsignedChars(DFA59_minS);
    static readonly char[] DFA59_max = DFA.UnpackEncodedStringToUnsignedChars(DFA59_maxS);
    static readonly short[] DFA59_accept = DFA.UnpackEncodedString(DFA59_acceptS);
    static readonly short[] DFA59_special = DFA.UnpackEncodedString(DFA59_specialS);
    static readonly short[][] DFA59_transition = DFA.UnpackEncodedStringArray(DFA59_transitionS);

    protected class DFA59 : DFA
    {
        public DFA59(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 59;
            this.eot = DFA59_eot;
            this.eof = DFA59_eof;
            this.min = DFA59_min;
            this.max = DFA59_max;
            this.accept = DFA59_accept;
            this.special = DFA59_special;
            this.transition = DFA59_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 216:42: ( valueExpr )*"; }
        }

    }

    const string DFA60_eotS =
        "\x3c\uffff";
    const string DFA60_eofS =
        "\x3c\uffff";
    const string DFA60_minS =
        "\x01\x03\x3b\uffff";
    const string DFA60_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA60_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA60_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA60_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA60_eot = DFA.UnpackEncodedString(DFA60_eotS);
    static readonly short[] DFA60_eof = DFA.UnpackEncodedString(DFA60_eofS);
    static readonly char[] DFA60_min = DFA.UnpackEncodedStringToUnsignedChars(DFA60_minS);
    static readonly char[] DFA60_max = DFA.UnpackEncodedStringToUnsignedChars(DFA60_maxS);
    static readonly short[] DFA60_accept = DFA.UnpackEncodedString(DFA60_acceptS);
    static readonly short[] DFA60_special = DFA.UnpackEncodedString(DFA60_specialS);
    static readonly short[][] DFA60_transition = DFA.UnpackEncodedStringArray(DFA60_transitionS);

    protected class DFA60 : DFA
    {
        public DFA60(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 60;
            this.eot = DFA60_eot;
            this.eof = DFA60_eof;
            this.min = DFA60_min;
            this.max = DFA60_max;
            this.accept = DFA60_accept;
            this.special = DFA60_special;
            this.transition = DFA60_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 217:43: ( valueExpr )*"; }
        }

    }

    const string DFA62_eotS =
        "\x3b\uffff";
    const string DFA62_eofS =
        "\x3b\uffff";
    const string DFA62_minS =
        "\x01\x06\x3a\uffff";
    const string DFA62_maxS =
        "\x01\u00cd\x3a\uffff";
    const string DFA62_acceptS =
        "\x01\uffff\x01\x01\x06\uffff\x01\x02\x01\x03\x08\uffff\x01\x04"+
        "\x01\x05\x08\uffff\x01\x06\x0c\uffff\x01\x07\x01\x08\x01\uffff\x01"+
        "\x09\x03\uffff\x01\x0a\x01\uffff\x01\x0b\x01\uffff\x01\x0c\x01\uffff"+
        "\x01\x0d\x01\x0e\x01\uffff\x01\x0f\x01\x10";
    const string DFA62_specialS =
        "\x3b\uffff}>";
    static readonly string[] DFA62_transitionS = {
            "\x01\x2c\x01\x30\x01\x32\x01\x34\x03\uffff\x01\x13\x03\uffff"+
            "\x02\x1c\x02\uffff\x05\x1c\x01\uffff\x02\x2a\x23\uffff\x03\x1c"+
            "\x02\uffff\x03\x1c\x1b\uffff\x04\x13\x0c\uffff\x01\x12\x0c\uffff"+
            "\x01\x09\x01\x29\x02\uffff\x01\x36\x05\uffff\x01\x2c\x01\x30"+
            "\x01\x32\x01\x34\x05\uffff\x02\x2c\x01\x39\x01\x3a\x02\x37\x03"+
            "\uffff\x01\x08\x0a\uffff\x07\x01\x06\uffff\x02\x09\x06\uffff"+
            "\x01\x09\x05\uffff\x03\x09\x02\uffff\x04\x13\x01\uffff\x02\x09",
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
            ""
    };

    static readonly short[] DFA62_eot = DFA.UnpackEncodedString(DFA62_eotS);
    static readonly short[] DFA62_eof = DFA.UnpackEncodedString(DFA62_eofS);
    static readonly char[] DFA62_min = DFA.UnpackEncodedStringToUnsignedChars(DFA62_minS);
    static readonly char[] DFA62_max = DFA.UnpackEncodedStringToUnsignedChars(DFA62_maxS);
    static readonly short[] DFA62_accept = DFA.UnpackEncodedString(DFA62_acceptS);
    static readonly short[] DFA62_special = DFA.UnpackEncodedString(DFA62_specialS);
    static readonly short[][] DFA62_transition = DFA.UnpackEncodedStringArray(DFA62_transitionS);

    protected class DFA62 : DFA
    {
        public DFA62(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 62;
            this.eot = DFA62_eot;
            this.eof = DFA62_eof;
            this.min = DFA62_min;
            this.max = DFA62_max;
            this.accept = DFA62_accept;
            this.special = DFA62_special;
            this.transition = DFA62_transition;

        }

        override public string Description
        {
            get { return "224:1: valueExpr : ( constant[true] | substitution | arithmeticExpr | eventPropertyExpr | evalExprChoice | builtinFunc | libFunc | caseExpr | inExpr | betweenExpr | likeExpr | regExpExpr | arrayExpr | subSelectInExpr | subSelectRowExpr | subSelectExistsExpr );"; }
        }

    }

    const string DFA69_eotS =
        "\x3c\uffff";
    const string DFA69_eofS =
        "\x3c\uffff";
    const string DFA69_minS =
        "\x01\x03\x3b\uffff";
    const string DFA69_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA69_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA69_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA69_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA69_eot = DFA.UnpackEncodedString(DFA69_eotS);
    static readonly short[] DFA69_eof = DFA.UnpackEncodedString(DFA69_eofS);
    static readonly char[] DFA69_min = DFA.UnpackEncodedStringToUnsignedChars(DFA69_minS);
    static readonly char[] DFA69_max = DFA.UnpackEncodedStringToUnsignedChars(DFA69_maxS);
    static readonly short[] DFA69_accept = DFA.UnpackEncodedString(DFA69_acceptS);
    static readonly short[] DFA69_special = DFA.UnpackEncodedString(DFA69_specialS);
    static readonly short[][] DFA69_transition = DFA.UnpackEncodedStringArray(DFA69_transitionS);

    protected class DFA69 : DFA
    {
        public DFA69(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 69;
            this.eot = DFA69_eot;
            this.eof = DFA69_eof;
            this.min = DFA69_min;
            this.max = DFA69_max;
            this.accept = DFA69_accept;
            this.special = DFA69_special;
            this.transition = DFA69_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 269:13: ( valueExpr )*"; }
        }

    }

    const string DFA70_eotS =
        "\x3c\uffff";
    const string DFA70_eofS =
        "\x3c\uffff";
    const string DFA70_minS =
        "\x01\x03\x3b\uffff";
    const string DFA70_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA70_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA70_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA70_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA70_eot = DFA.UnpackEncodedString(DFA70_eotS);
    static readonly short[] DFA70_eof = DFA.UnpackEncodedString(DFA70_eofS);
    static readonly char[] DFA70_min = DFA.UnpackEncodedStringToUnsignedChars(DFA70_minS);
    static readonly char[] DFA70_max = DFA.UnpackEncodedStringToUnsignedChars(DFA70_maxS);
    static readonly short[] DFA70_accept = DFA.UnpackEncodedString(DFA70_acceptS);
    static readonly short[] DFA70_special = DFA.UnpackEncodedString(DFA70_specialS);
    static readonly short[][] DFA70_transition = DFA.UnpackEncodedStringArray(DFA70_transitionS);

    protected class DFA70 : DFA
    {
        public DFA70(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 70;
            this.eot = DFA70_eot;
            this.eof = DFA70_eof;
            this.min = DFA70_min;
            this.max = DFA70_max;
            this.accept = DFA70_accept;
            this.special = DFA70_special;
            this.transition = DFA70_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 270:14: ( valueExpr )*"; }
        }

    }

    const string DFA72_eotS =
        "\x3c\uffff";
    const string DFA72_eofS =
        "\x3c\uffff";
    const string DFA72_minS =
        "\x01\x06\x3b\uffff";
    const string DFA72_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA72_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA72_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA72_transitionS = {
            "\x04\x02\x03\uffff\x01\x02\x03\uffff\x02\x02\x02\uffff\x05"+
            "\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02\uffff\x03\x02\x1b"+
            "\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff\x02\x02\x02\uffff"+
            "\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02\x03\uffff\x01\x02"+
            "\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff\x01\x02\x01\uffff"+
            "\x01\x01\x01\uffff\x01\x01\x01\uffff\x03\x02\x02\uffff\x04\x02"+
            "\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA72_eot = DFA.UnpackEncodedString(DFA72_eotS);
    static readonly short[] DFA72_eof = DFA.UnpackEncodedString(DFA72_eofS);
    static readonly char[] DFA72_min = DFA.UnpackEncodedStringToUnsignedChars(DFA72_minS);
    static readonly char[] DFA72_max = DFA.UnpackEncodedStringToUnsignedChars(DFA72_maxS);
    static readonly short[] DFA72_accept = DFA.UnpackEncodedString(DFA72_acceptS);
    static readonly short[] DFA72_special = DFA.UnpackEncodedString(DFA72_specialS);
    static readonly short[][] DFA72_transition = DFA.UnpackEncodedStringArray(DFA72_transitionS);

    protected class DFA72 : DFA
    {
        public DFA72(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 72;
            this.eot = DFA72_eot;
            this.eof = DFA72_eof;
            this.min = DFA72_min;
            this.max = DFA72_max;
            this.accept = DFA72_accept;
            this.special = DFA72_special;
            this.transition = DFA72_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 274:51: ( valueExpr )*"; }
        }

    }

    const string DFA73_eotS =
        "\x3c\uffff";
    const string DFA73_eofS =
        "\x3c\uffff";
    const string DFA73_minS =
        "\x01\x06\x3b\uffff";
    const string DFA73_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA73_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA73_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA73_transitionS = {
            "\x04\x02\x03\uffff\x01\x02\x03\uffff\x02\x02\x02\uffff\x05"+
            "\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02\uffff\x03\x02\x1b"+
            "\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff\x02\x02\x02\uffff"+
            "\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02\x03\uffff\x01\x02"+
            "\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff\x01\x02\x01\uffff"+
            "\x01\x01\x01\uffff\x01\x01\x01\uffff\x03\x02\x02\uffff\x04\x02"+
            "\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA73_eot = DFA.UnpackEncodedString(DFA73_eotS);
    static readonly short[] DFA73_eof = DFA.UnpackEncodedString(DFA73_eofS);
    static readonly char[] DFA73_min = DFA.UnpackEncodedStringToUnsignedChars(DFA73_minS);
    static readonly char[] DFA73_max = DFA.UnpackEncodedStringToUnsignedChars(DFA73_maxS);
    static readonly short[] DFA73_accept = DFA.UnpackEncodedString(DFA73_acceptS);
    static readonly short[] DFA73_special = DFA.UnpackEncodedString(DFA73_specialS);
    static readonly short[][] DFA73_transition = DFA.UnpackEncodedStringArray(DFA73_transitionS);

    protected class DFA73 : DFA
    {
        public DFA73(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 73;
            this.eot = DFA73_eot;
            this.eof = DFA73_eof;
            this.min = DFA73_min;
            this.max = DFA73_max;
            this.accept = DFA73_accept;
            this.special = DFA73_special;
            this.transition = DFA73_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 275:55: ( valueExpr )*"; }
        }

    }

    const string DFA75_eotS =
        "\x3c\uffff";
    const string DFA75_eofS =
        "\x3c\uffff";
    const string DFA75_minS =
        "\x01\x03\x3b\uffff";
    const string DFA75_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA75_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA75_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA75_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA75_eot = DFA.UnpackEncodedString(DFA75_eotS);
    static readonly short[] DFA75_eof = DFA.UnpackEncodedString(DFA75_eofS);
    static readonly char[] DFA75_min = DFA.UnpackEncodedStringToUnsignedChars(DFA75_minS);
    static readonly char[] DFA75_max = DFA.UnpackEncodedStringToUnsignedChars(DFA75_maxS);
    static readonly short[] DFA75_accept = DFA.UnpackEncodedString(DFA75_acceptS);
    static readonly short[] DFA75_special = DFA.UnpackEncodedString(DFA75_specialS);
    static readonly short[][] DFA75_transition = DFA.UnpackEncodedStringArray(DFA75_transitionS);

    protected class DFA75 : DFA
    {
        public DFA75(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 75;
            this.eot = DFA75_eot;
            this.eof = DFA75_eof;
            this.min = DFA75_min;
            this.max = DFA75_max;
            this.accept = DFA75_accept;
            this.special = DFA75_special;
            this.transition = DFA75_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 282:40: ( valueExpr )*"; }
        }

    }

    const string DFA77_eotS =
        "\x3c\uffff";
    const string DFA77_eofS =
        "\x3c\uffff";
    const string DFA77_minS =
        "\x01\x03\x3b\uffff";
    const string DFA77_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA77_acceptS =
        "\x01\uffff\x01\x01\x39\uffff\x01\x02";
    const string DFA77_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA77_transitionS = {
            "\x01\x3b\x02\uffff\x04\x01\x03\uffff\x01\x01\x03\uffff\x02"+
            "\x01\x02\uffff\x05\x01\x01\uffff\x02\x01\x23\uffff\x03\x01\x02"+
            "\uffff\x03\x01\x1b\uffff\x04\x01\x0c\uffff\x01\x01\x0c\uffff"+
            "\x02\x01\x02\uffff\x01\x01\x05\uffff\x04\x01\x05\uffff\x06\x01"+
            "\x03\uffff\x01\x01\x0a\uffff\x07\x01\x06\uffff\x02\x01\x06\uffff"+
            "\x01\x01\x05\uffff\x03\x01\x02\uffff\x04\x01\x01\uffff\x02\x01",
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
            ""
    };

    static readonly short[] DFA77_eot = DFA.UnpackEncodedString(DFA77_eotS);
    static readonly short[] DFA77_eof = DFA.UnpackEncodedString(DFA77_eofS);
    static readonly char[] DFA77_min = DFA.UnpackEncodedStringToUnsignedChars(DFA77_minS);
    static readonly char[] DFA77_max = DFA.UnpackEncodedStringToUnsignedChars(DFA77_maxS);
    static readonly short[] DFA77_accept = DFA.UnpackEncodedString(DFA77_acceptS);
    static readonly short[] DFA77_special = DFA.UnpackEncodedString(DFA77_specialS);
    static readonly short[][] DFA77_transition = DFA.UnpackEncodedStringArray(DFA77_transitionS);

    protected class DFA77 : DFA
    {
        public DFA77(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 77;
            this.eot = DFA77_eot;
            this.eof = DFA77_eof;
            this.min = DFA77_min;
            this.max = DFA77_max;
            this.accept = DFA77_accept;
            this.special = DFA77_special;
            this.transition = DFA77_transition;

        }

        override public string Description
        {
            get { return "286:33: ( valueExpr )?"; }
        }

    }

    const string DFA78_eotS =
        "\x3c\uffff";
    const string DFA78_eofS =
        "\x3c\uffff";
    const string DFA78_minS =
        "\x01\x03\x3b\uffff";
    const string DFA78_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA78_acceptS =
        "\x01\uffff\x01\x01\x39\uffff\x01\x02";
    const string DFA78_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA78_transitionS = {
            "\x01\x3b\x02\uffff\x04\x01\x03\uffff\x01\x01\x03\uffff\x02"+
            "\x01\x02\uffff\x05\x01\x01\uffff\x02\x01\x23\uffff\x03\x01\x02"+
            "\uffff\x03\x01\x1b\uffff\x04\x01\x0c\uffff\x01\x01\x0c\uffff"+
            "\x02\x01\x02\uffff\x01\x01\x05\uffff\x04\x01\x05\uffff\x06\x01"+
            "\x03\uffff\x01\x01\x0a\uffff\x07\x01\x06\uffff\x02\x01\x06\uffff"+
            "\x01\x01\x05\uffff\x03\x01\x02\uffff\x04\x01\x01\uffff\x02\x01",
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
            ""
    };

    static readonly short[] DFA78_eot = DFA.UnpackEncodedString(DFA78_eotS);
    static readonly short[] DFA78_eof = DFA.UnpackEncodedString(DFA78_eofS);
    static readonly char[] DFA78_min = DFA.UnpackEncodedStringToUnsignedChars(DFA78_minS);
    static readonly char[] DFA78_max = DFA.UnpackEncodedStringToUnsignedChars(DFA78_maxS);
    static readonly short[] DFA78_accept = DFA.UnpackEncodedString(DFA78_acceptS);
    static readonly short[] DFA78_special = DFA.UnpackEncodedString(DFA78_specialS);
    static readonly short[][] DFA78_transition = DFA.UnpackEncodedStringArray(DFA78_transitionS);

    protected class DFA78 : DFA
    {
        public DFA78(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 78;
            this.eot = DFA78_eot;
            this.eof = DFA78_eof;
            this.min = DFA78_min;
            this.max = DFA78_max;
            this.accept = DFA78_accept;
            this.special = DFA78_special;
            this.transition = DFA78_transition;

        }

        override public string Description
        {
            get { return "287:37: ( valueExpr )?"; }
        }

    }

    const string DFA90_eotS =
        "\x0e\uffff";
    const string DFA90_eofS =
        "\x0e\uffff";
    const string DFA90_minS =
        "\x01\x11\x0d\uffff";
    const string DFA90_maxS =
        "\x01\x47\x0d\uffff";
    const string DFA90_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x01\x06\x01"+
        "\x07\x01\x08\x01\x09\x01\x0a\x01\x0b\x01\x0c\x01\x0d";
    const string DFA90_specialS =
        "\x0e\uffff}>";
    static readonly string[] DFA90_transitionS = {
            "\x01\x01\x01\x02\x02\uffff\x01\x07\x01\x04\x01\x05\x01\x06"+
            "\x01\x03\x26\uffff\x01\x08\x01\x09\x01\x0c\x02\uffff\x01\x0a"+
            "\x01\x0b\x01\x0d",
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
            ""
    };

    static readonly short[] DFA90_eot = DFA.UnpackEncodedString(DFA90_eotS);
    static readonly short[] DFA90_eof = DFA.UnpackEncodedString(DFA90_eofS);
    static readonly char[] DFA90_min = DFA.UnpackEncodedStringToUnsignedChars(DFA90_minS);
    static readonly char[] DFA90_max = DFA.UnpackEncodedStringToUnsignedChars(DFA90_maxS);
    static readonly short[] DFA90_accept = DFA.UnpackEncodedString(DFA90_acceptS);
    static readonly short[] DFA90_special = DFA.UnpackEncodedString(DFA90_specialS);
    static readonly short[][] DFA90_transition = DFA.UnpackEncodedStringArray(DFA90_transitionS);

    protected class DFA90 : DFA
    {
        public DFA90(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 90;
            this.eot = DFA90_eot;
            this.eof = DFA90_eof;
            this.min = DFA90_min;
            this.max = DFA90_max;
            this.accept = DFA90_accept;
            this.special = DFA90_special;
            this.transition = DFA90_transition;

        }

        override public string Description
        {
            get { return "295:1: builtinFunc : ( ^(f= SUM ( DISTINCT )? valueExpr ) | ^(f= AVG ( DISTINCT )? valueExpr ) | ^(f= COUNT ( ( DISTINCT )? valueExpr )? ) | ^(f= MEDIAN ( DISTINCT )? valueExpr ) | ^(f= STDDEV ( DISTINCT )? valueExpr ) | ^(f= AVEDEV ( DISTINCT )? valueExpr ) | ^(f= COALESCE valueExpr valueExpr ( valueExpr )* ) | ^(f= PREVIOUS valueExpr eventPropertyExpr ) | ^(f= PRIOR c= NUM_INT eventPropertyExpr ) | ^(f= INSTANCEOF valueExpr CLASS_IDENT ( CLASS_IDENT )* ) | ^(f= CAST valueExpr CLASS_IDENT ) | ^(f= EXISTS eventPropertyExpr ) | ^(f= CURRENT_TIMESTAMP ) );"; }
        }

    }

    const string DFA81_eotS =
        "\x3c\uffff";
    const string DFA81_eofS =
        "\x3c\uffff";
    const string DFA81_minS =
        "\x01\x06\x3b\uffff";
    const string DFA81_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA81_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x39\uffff";
    const string DFA81_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA81_transitionS = {
            "\x04\x02\x03\uffff\x01\x02\x03\uffff\x02\x02\x02\uffff\x05"+
            "\x02\x01\uffff\x02\x02\x0f\uffff\x01\x01\x13\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA81_eot = DFA.UnpackEncodedString(DFA81_eotS);
    static readonly short[] DFA81_eof = DFA.UnpackEncodedString(DFA81_eofS);
    static readonly char[] DFA81_min = DFA.UnpackEncodedStringToUnsignedChars(DFA81_minS);
    static readonly char[] DFA81_max = DFA.UnpackEncodedStringToUnsignedChars(DFA81_maxS);
    static readonly short[] DFA81_accept = DFA.UnpackEncodedString(DFA81_acceptS);
    static readonly short[] DFA81_special = DFA.UnpackEncodedString(DFA81_specialS);
    static readonly short[][] DFA81_transition = DFA.UnpackEncodedStringArray(DFA81_transitionS);

    protected class DFA81 : DFA
    {
        public DFA81(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 81;
            this.eot = DFA81_eot;
            this.eof = DFA81_eof;
            this.min = DFA81_min;
            this.max = DFA81_max;
            this.accept = DFA81_accept;
            this.special = DFA81_special;
            this.transition = DFA81_transition;

        }

        override public string Description
        {
            get { return "296:13: ( DISTINCT )?"; }
        }

    }

    const string DFA82_eotS =
        "\x3c\uffff";
    const string DFA82_eofS =
        "\x3c\uffff";
    const string DFA82_minS =
        "\x01\x06\x3b\uffff";
    const string DFA82_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA82_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x39\uffff";
    const string DFA82_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA82_transitionS = {
            "\x04\x02\x03\uffff\x01\x02\x03\uffff\x02\x02\x02\uffff\x05"+
            "\x02\x01\uffff\x02\x02\x0f\uffff\x01\x01\x13\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA82_eot = DFA.UnpackEncodedString(DFA82_eotS);
    static readonly short[] DFA82_eof = DFA.UnpackEncodedString(DFA82_eofS);
    static readonly char[] DFA82_min = DFA.UnpackEncodedStringToUnsignedChars(DFA82_minS);
    static readonly char[] DFA82_max = DFA.UnpackEncodedStringToUnsignedChars(DFA82_maxS);
    static readonly short[] DFA82_accept = DFA.UnpackEncodedString(DFA82_acceptS);
    static readonly short[] DFA82_special = DFA.UnpackEncodedString(DFA82_specialS);
    static readonly short[][] DFA82_transition = DFA.UnpackEncodedStringArray(DFA82_transitionS);

    protected class DFA82 : DFA
    {
        public DFA82(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 82;
            this.eot = DFA82_eot;
            this.eof = DFA82_eof;
            this.min = DFA82_min;
            this.max = DFA82_max;
            this.accept = DFA82_accept;
            this.special = DFA82_special;
            this.transition = DFA82_transition;

        }

        override public string Description
        {
            get { return "297:12: ( DISTINCT )?"; }
        }

    }

    const string DFA84_eotS =
        "\x3d\uffff";
    const string DFA84_eofS =
        "\x3d\uffff";
    const string DFA84_minS =
        "\x01\x03\x3c\uffff";
    const string DFA84_maxS =
        "\x01\u00cd\x3c\uffff";
    const string DFA84_acceptS =
        "\x01\uffff\x01\x01\x3a\uffff\x01\x02";
    const string DFA84_specialS =
        "\x3d\uffff}>";
    static readonly string[] DFA84_transitionS = {
            "\x01\x3c\x02\uffff\x04\x01\x03\uffff\x01\x01\x03\uffff\x02"+
            "\x01\x02\uffff\x05\x01\x01\uffff\x02\x01\x0f\uffff\x01\x01\x13"+
            "\uffff\x03\x01\x02\uffff\x03\x01\x1b\uffff\x04\x01\x0c\uffff"+
            "\x01\x01\x0c\uffff\x02\x01\x02\uffff\x01\x01\x05\uffff\x04\x01"+
            "\x05\uffff\x06\x01\x03\uffff\x01\x01\x0a\uffff\x07\x01\x06\uffff"+
            "\x02\x01\x06\uffff\x01\x01\x05\uffff\x03\x01\x02\uffff\x04\x01"+
            "\x01\uffff\x02\x01",
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
            ""
    };

    static readonly short[] DFA84_eot = DFA.UnpackEncodedString(DFA84_eotS);
    static readonly short[] DFA84_eof = DFA.UnpackEncodedString(DFA84_eofS);
    static readonly char[] DFA84_min = DFA.UnpackEncodedStringToUnsignedChars(DFA84_minS);
    static readonly char[] DFA84_max = DFA.UnpackEncodedStringToUnsignedChars(DFA84_maxS);
    static readonly short[] DFA84_accept = DFA.UnpackEncodedString(DFA84_acceptS);
    static readonly short[] DFA84_special = DFA.UnpackEncodedString(DFA84_specialS);
    static readonly short[][] DFA84_transition = DFA.UnpackEncodedStringArray(DFA84_transitionS);

    protected class DFA84 : DFA
    {
        public DFA84(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 84;
            this.eot = DFA84_eot;
            this.eof = DFA84_eof;
            this.min = DFA84_min;
            this.max = DFA84_max;
            this.accept = DFA84_accept;
            this.special = DFA84_special;
            this.transition = DFA84_transition;

        }

        override public string Description
        {
            get { return "298:14: ( ( DISTINCT )? valueExpr )?"; }
        }

    }

    const string DFA83_eotS =
        "\x3c\uffff";
    const string DFA83_eofS =
        "\x3c\uffff";
    const string DFA83_minS =
        "\x01\x06\x3b\uffff";
    const string DFA83_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA83_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x39\uffff";
    const string DFA83_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA83_transitionS = {
            "\x04\x02\x03\uffff\x01\x02\x03\uffff\x02\x02\x02\uffff\x05"+
            "\x02\x01\uffff\x02\x02\x0f\uffff\x01\x01\x13\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA83_eot = DFA.UnpackEncodedString(DFA83_eotS);
    static readonly short[] DFA83_eof = DFA.UnpackEncodedString(DFA83_eofS);
    static readonly char[] DFA83_min = DFA.UnpackEncodedStringToUnsignedChars(DFA83_minS);
    static readonly char[] DFA83_max = DFA.UnpackEncodedStringToUnsignedChars(DFA83_maxS);
    static readonly short[] DFA83_accept = DFA.UnpackEncodedString(DFA83_acceptS);
    static readonly short[] DFA83_special = DFA.UnpackEncodedString(DFA83_specialS);
    static readonly short[][] DFA83_transition = DFA.UnpackEncodedStringArray(DFA83_transitionS);

    protected class DFA83 : DFA
    {
        public DFA83(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 83;
            this.eot = DFA83_eot;
            this.eof = DFA83_eof;
            this.min = DFA83_min;
            this.max = DFA83_max;
            this.accept = DFA83_accept;
            this.special = DFA83_special;
            this.transition = DFA83_transition;

        }

        override public string Description
        {
            get { return "298:15: ( DISTINCT )?"; }
        }

    }

    const string DFA85_eotS =
        "\x3c\uffff";
    const string DFA85_eofS =
        "\x3c\uffff";
    const string DFA85_minS =
        "\x01\x06\x3b\uffff";
    const string DFA85_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA85_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x39\uffff";
    const string DFA85_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA85_transitionS = {
            "\x04\x02\x03\uffff\x01\x02\x03\uffff\x02\x02\x02\uffff\x05"+
            "\x02\x01\uffff\x02\x02\x0f\uffff\x01\x01\x13\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA85_eot = DFA.UnpackEncodedString(DFA85_eotS);
    static readonly short[] DFA85_eof = DFA.UnpackEncodedString(DFA85_eofS);
    static readonly char[] DFA85_min = DFA.UnpackEncodedStringToUnsignedChars(DFA85_minS);
    static readonly char[] DFA85_max = DFA.UnpackEncodedStringToUnsignedChars(DFA85_maxS);
    static readonly short[] DFA85_accept = DFA.UnpackEncodedString(DFA85_acceptS);
    static readonly short[] DFA85_special = DFA.UnpackEncodedString(DFA85_specialS);
    static readonly short[][] DFA85_transition = DFA.UnpackEncodedStringArray(DFA85_transitionS);

    protected class DFA85 : DFA
    {
        public DFA85(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 85;
            this.eot = DFA85_eot;
            this.eof = DFA85_eof;
            this.min = DFA85_min;
            this.max = DFA85_max;
            this.accept = DFA85_accept;
            this.special = DFA85_special;
            this.transition = DFA85_transition;

        }

        override public string Description
        {
            get { return "299:15: ( DISTINCT )?"; }
        }

    }

    const string DFA86_eotS =
        "\x3c\uffff";
    const string DFA86_eofS =
        "\x3c\uffff";
    const string DFA86_minS =
        "\x01\x06\x3b\uffff";
    const string DFA86_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA86_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x39\uffff";
    const string DFA86_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA86_transitionS = {
            "\x04\x02\x03\uffff\x01\x02\x03\uffff\x02\x02\x02\uffff\x05"+
            "\x02\x01\uffff\x02\x02\x0f\uffff\x01\x01\x13\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA86_eot = DFA.UnpackEncodedString(DFA86_eotS);
    static readonly short[] DFA86_eof = DFA.UnpackEncodedString(DFA86_eofS);
    static readonly char[] DFA86_min = DFA.UnpackEncodedStringToUnsignedChars(DFA86_minS);
    static readonly char[] DFA86_max = DFA.UnpackEncodedStringToUnsignedChars(DFA86_maxS);
    static readonly short[] DFA86_accept = DFA.UnpackEncodedString(DFA86_acceptS);
    static readonly short[] DFA86_special = DFA.UnpackEncodedString(DFA86_specialS);
    static readonly short[][] DFA86_transition = DFA.UnpackEncodedStringArray(DFA86_transitionS);

    protected class DFA86 : DFA
    {
        public DFA86(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 86;
            this.eot = DFA86_eot;
            this.eof = DFA86_eof;
            this.min = DFA86_min;
            this.max = DFA86_max;
            this.accept = DFA86_accept;
            this.special = DFA86_special;
            this.transition = DFA86_transition;

        }

        override public string Description
        {
            get { return "300:15: ( DISTINCT )?"; }
        }

    }

    const string DFA87_eotS =
        "\x3c\uffff";
    const string DFA87_eofS =
        "\x3c\uffff";
    const string DFA87_minS =
        "\x01\x06\x3b\uffff";
    const string DFA87_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA87_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x39\uffff";
    const string DFA87_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA87_transitionS = {
            "\x04\x02\x03\uffff\x01\x02\x03\uffff\x02\x02\x02\uffff\x05"+
            "\x02\x01\uffff\x02\x02\x0f\uffff\x01\x01\x13\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA87_eot = DFA.UnpackEncodedString(DFA87_eotS);
    static readonly short[] DFA87_eof = DFA.UnpackEncodedString(DFA87_eofS);
    static readonly char[] DFA87_min = DFA.UnpackEncodedStringToUnsignedChars(DFA87_minS);
    static readonly char[] DFA87_max = DFA.UnpackEncodedStringToUnsignedChars(DFA87_maxS);
    static readonly short[] DFA87_accept = DFA.UnpackEncodedString(DFA87_acceptS);
    static readonly short[] DFA87_special = DFA.UnpackEncodedString(DFA87_specialS);
    static readonly short[][] DFA87_transition = DFA.UnpackEncodedStringArray(DFA87_transitionS);

    protected class DFA87 : DFA
    {
        public DFA87(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 87;
            this.eot = DFA87_eot;
            this.eof = DFA87_eof;
            this.min = DFA87_min;
            this.max = DFA87_max;
            this.accept = DFA87_accept;
            this.special = DFA87_special;
            this.transition = DFA87_transition;

        }

        override public string Description
        {
            get { return "301:15: ( DISTINCT )?"; }
        }

    }

    const string DFA88_eotS =
        "\x3c\uffff";
    const string DFA88_eofS =
        "\x3c\uffff";
    const string DFA88_minS =
        "\x01\x03\x3b\uffff";
    const string DFA88_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA88_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA88_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA88_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA88_eot = DFA.UnpackEncodedString(DFA88_eotS);
    static readonly short[] DFA88_eof = DFA.UnpackEncodedString(DFA88_eofS);
    static readonly char[] DFA88_min = DFA.UnpackEncodedStringToUnsignedChars(DFA88_minS);
    static readonly char[] DFA88_max = DFA.UnpackEncodedStringToUnsignedChars(DFA88_maxS);
    static readonly short[] DFA88_accept = DFA.UnpackEncodedString(DFA88_acceptS);
    static readonly short[] DFA88_special = DFA.UnpackEncodedString(DFA88_specialS);
    static readonly short[][] DFA88_transition = DFA.UnpackEncodedStringArray(DFA88_transitionS);

    protected class DFA88 : DFA
    {
        public DFA88(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 88;
            this.eot = DFA88_eot;
            this.eof = DFA88_eof;
            this.min = DFA88_min;
            this.max = DFA88_max;
            this.accept = DFA88_accept;
            this.special = DFA88_special;
            this.transition = DFA88_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 302:38: ( valueExpr )*"; }
        }

    }

    const string DFA91_eotS =
        "\x3c\uffff";
    const string DFA91_eofS =
        "\x3c\uffff";
    const string DFA91_minS =
        "\x01\x03\x3b\uffff";
    const string DFA91_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA91_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA91_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA91_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA91_eot = DFA.UnpackEncodedString(DFA91_eotS);
    static readonly short[] DFA91_eof = DFA.UnpackEncodedString(DFA91_eofS);
    static readonly char[] DFA91_min = DFA.UnpackEncodedStringToUnsignedChars(DFA91_minS);
    static readonly char[] DFA91_max = DFA.UnpackEncodedStringToUnsignedChars(DFA91_maxS);
    static readonly short[] DFA91_accept = DFA.UnpackEncodedString(DFA91_acceptS);
    static readonly short[] DFA91_special = DFA.UnpackEncodedString(DFA91_specialS);
    static readonly short[][] DFA91_transition = DFA.UnpackEncodedStringArray(DFA91_transitionS);

    protected class DFA91 : DFA
    {
        public DFA91(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 91;
            this.eot = DFA91_eot;
            this.eof = DFA91_eof;
            this.min = DFA91_min;
            this.max = DFA91_max;
            this.accept = DFA91_accept;
            this.special = DFA91_special;
            this.transition = DFA91_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 312:19: ( valueExpr )*"; }
        }

    }

    const string DFA93_eotS =
        "\x0a\uffff";
    const string DFA93_eofS =
        "\x0a\uffff";
    const string DFA93_minS =
        "\x01\u0080\x09\uffff";
    const string DFA93_maxS =
        "\x01\u00cd\x09\uffff";
    const string DFA93_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x01\x06\x01"+
        "\x07\x01\x08\x01\x09";
    const string DFA93_specialS =
        "\x0a\uffff}>";
    static readonly string[] DFA93_transitionS = {
            "\x01\x09\x33\uffff\x01\x02\x01\x01\x06\uffff\x01\x04\x05\uffff"+
            "\x01\x06\x01\x07\x01\x08\x07\uffff\x01\x03\x01\x05",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA93_eot = DFA.UnpackEncodedString(DFA93_eotS);
    static readonly short[] DFA93_eof = DFA.UnpackEncodedString(DFA93_eofS);
    static readonly char[] DFA93_min = DFA.UnpackEncodedStringToUnsignedChars(DFA93_minS);
    static readonly char[] DFA93_max = DFA.UnpackEncodedStringToUnsignedChars(DFA93_maxS);
    static readonly short[] DFA93_accept = DFA.UnpackEncodedString(DFA93_acceptS);
    static readonly short[] DFA93_special = DFA.UnpackEncodedString(DFA93_specialS);
    static readonly short[][] DFA93_transition = DFA.UnpackEncodedStringArray(DFA93_transitionS);

    protected class DFA93 : DFA
    {
        public DFA93(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 93;
            this.eot = DFA93_eot;
            this.eof = DFA93_eof;
            this.min = DFA93_min;
            this.max = DFA93_max;
            this.accept = DFA93_accept;
            this.special = DFA93_special;
            this.transition = DFA93_transition;

        }

        override public string Description
        {
            get { return "315:1: arithmeticExpr : ( ^(a= PLUS valueExpr valueExpr ) | ^(a= MINUS valueExpr valueExpr ) | ^(a= DIV valueExpr valueExpr ) | ^(a= STAR valueExpr valueExpr ) | ^(a= MOD valueExpr valueExpr ) | ^(a= BAND valueExpr valueExpr ) | ^(a= BOR valueExpr valueExpr ) | ^(a= BXOR valueExpr valueExpr ) | ^(a= CONCAT valueExpr valueExpr ( valueExpr )* ) );"; }
        }

    }

    const string DFA92_eotS =
        "\x3c\uffff";
    const string DFA92_eofS =
        "\x3c\uffff";
    const string DFA92_minS =
        "\x01\x03\x3b\uffff";
    const string DFA92_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA92_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA92_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA92_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA92_eot = DFA.UnpackEncodedString(DFA92_eotS);
    static readonly short[] DFA92_eof = DFA.UnpackEncodedString(DFA92_eofS);
    static readonly char[] DFA92_min = DFA.UnpackEncodedStringToUnsignedChars(DFA92_minS);
    static readonly char[] DFA92_max = DFA.UnpackEncodedStringToUnsignedChars(DFA92_maxS);
    static readonly short[] DFA92_accept = DFA.UnpackEncodedString(DFA92_acceptS);
    static readonly short[] DFA92_special = DFA.UnpackEncodedString(DFA92_specialS);
    static readonly short[][] DFA92_transition = DFA.UnpackEncodedStringArray(DFA92_transitionS);

    protected class DFA92 : DFA
    {
        public DFA92(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 92;
            this.eot = DFA92_eot;
            this.eof = DFA92_eof;
            this.min = DFA92_min;
            this.max = DFA92_max;
            this.accept = DFA92_accept;
            this.special = DFA92_special;
            this.transition = DFA92_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 324:36: ( valueExpr )*"; }
        }

    }

    const string DFA95_eotS =
        "\x3d\uffff";
    const string DFA95_eofS =
        "\x3d\uffff";
    const string DFA95_minS =
        "\x01\x03\x3c\uffff";
    const string DFA95_maxS =
        "\x01\u00cd\x3c\uffff";
    const string DFA95_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x3a\uffff";
    const string DFA95_specialS =
        "\x3d\uffff}>";
    static readonly string[] DFA95_transitionS = {
            "\x01\x02\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x0f\uffff\x01\x01\x13"+
            "\uffff\x03\x02\x02\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff"+
            "\x01\x02\x0c\uffff\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02"+
            "\x05\uffff\x06\x02\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff"+
            "\x02\x02\x06\uffff\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02"+
            "\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA95_eot = DFA.UnpackEncodedString(DFA95_eotS);
    static readonly short[] DFA95_eof = DFA.UnpackEncodedString(DFA95_eofS);
    static readonly char[] DFA95_min = DFA.UnpackEncodedStringToUnsignedChars(DFA95_minS);
    static readonly char[] DFA95_max = DFA.UnpackEncodedStringToUnsignedChars(DFA95_maxS);
    static readonly short[] DFA95_accept = DFA.UnpackEncodedString(DFA95_acceptS);
    static readonly short[] DFA95_special = DFA.UnpackEncodedString(DFA95_specialS);
    static readonly short[][] DFA95_transition = DFA.UnpackEncodedStringArray(DFA95_transitionS);

    protected class DFA95 : DFA
    {
        public DFA95(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 95;
            this.eot = DFA95_eot;
            this.eof = DFA95_eof;
            this.min = DFA95_min;
            this.max = DFA95_max;
            this.accept = DFA95_accept;
            this.special = DFA95_special;
            this.transition = DFA95_transition;

        }

        override public string Description
        {
            get { return "328:43: ( DISTINCT )?"; }
        }

    }

    const string DFA96_eotS =
        "\x3c\uffff";
    const string DFA96_eofS =
        "\x3c\uffff";
    const string DFA96_minS =
        "\x01\x03\x3b\uffff";
    const string DFA96_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA96_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA96_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA96_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA96_eot = DFA.UnpackEncodedString(DFA96_eotS);
    static readonly short[] DFA96_eof = DFA.UnpackEncodedString(DFA96_eofS);
    static readonly char[] DFA96_min = DFA.UnpackEncodedStringToUnsignedChars(DFA96_minS);
    static readonly char[] DFA96_max = DFA.UnpackEncodedStringToUnsignedChars(DFA96_maxS);
    static readonly short[] DFA96_accept = DFA.UnpackEncodedString(DFA96_acceptS);
    static readonly short[] DFA96_special = DFA.UnpackEncodedString(DFA96_specialS);
    static readonly short[][] DFA96_transition = DFA.UnpackEncodedStringArray(DFA96_transitionS);

    protected class DFA96 : DFA
    {
        public DFA96(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 96;
            this.eot = DFA96_eot;
            this.eof = DFA96_eof;
            this.min = DFA96_min;
            this.max = DFA96_max;
            this.accept = DFA96_accept;
            this.special = DFA96_special;
            this.transition = DFA96_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 328:55: ( valueExpr )*"; }
        }

    }

    const string DFA97_eotS =
        "\x14\uffff";
    const string DFA97_eofS =
        "\x14\uffff";
    const string DFA97_minS =
        "\x01\x03\x13\uffff";
    const string DFA97_maxS =
        "\x01\u00bc\x13\uffff";
    const string DFA97_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x11\uffff";
    const string DFA97_specialS =
        "\x14\uffff}>";
    static readonly string[] DFA97_transitionS = {
            "\x01\x01\x2f\uffff\x01\x02\x10\uffff\x01\x02\x07\uffff\x03"+
            "\x02\x01\uffff\x01\x02\x22\uffff\x01\x02\x0f\uffff\x01\x02\x16"+
            "\uffff\x02\x02\x0b\uffff\x07\x02\x0e\uffff\x01\x02",
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
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA97_eot = DFA.UnpackEncodedString(DFA97_eotS);
    static readonly short[] DFA97_eof = DFA.UnpackEncodedString(DFA97_eofS);
    static readonly char[] DFA97_min = DFA.UnpackEncodedStringToUnsignedChars(DFA97_minS);
    static readonly char[] DFA97_max = DFA.UnpackEncodedStringToUnsignedChars(DFA97_maxS);
    static readonly short[] DFA97_accept = DFA.UnpackEncodedString(DFA97_acceptS);
    static readonly short[] DFA97_special = DFA.UnpackEncodedString(DFA97_specialS);
    static readonly short[][] DFA97_transition = DFA.UnpackEncodedStringArray(DFA97_transitionS);

    protected class DFA97 : DFA
    {
        public DFA97(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 97;
            this.eot = DFA97_eot;
            this.eof = DFA97_eof;
            this.min = DFA97_min;
            this.max = DFA97_max;
            this.accept = DFA97_accept;
            this.special = DFA97_special;
            this.transition = DFA97_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 343:44: ( parameter )*"; }
        }

    }

    const string DFA99_eotS =
        "\x0a\uffff";
    const string DFA99_eofS =
        "\x0a\uffff";
    const string DFA99_minS =
        "\x01\x03\x09\uffff";
    const string DFA99_maxS =
        "\x01\x5c\x09\uffff";
    const string DFA99_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x07\uffff";
    const string DFA99_specialS =
        "\x0a\uffff}>";
    static readonly string[] DFA99_transitionS = {
            "\x01\x01\x07\uffff\x04\x02\x40\uffff\x01\x02\x01\uffff\x01"+
            "\x02\x09\uffff\x02\x02",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA99_eot = DFA.UnpackEncodedString(DFA99_eotS);
    static readonly short[] DFA99_eof = DFA.UnpackEncodedString(DFA99_eofS);
    static readonly char[] DFA99_min = DFA.UnpackEncodedStringToUnsignedChars(DFA99_minS);
    static readonly char[] DFA99_max = DFA.UnpackEncodedStringToUnsignedChars(DFA99_maxS);
    static readonly short[] DFA99_accept = DFA.UnpackEncodedString(DFA99_acceptS);
    static readonly short[] DFA99_special = DFA.UnpackEncodedString(DFA99_specialS);
    static readonly short[][] DFA99_transition = DFA.UnpackEncodedStringArray(DFA99_transitionS);

    protected class DFA99 : DFA
    {
        public DFA99(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 99;
            this.eot = DFA99_eot;
            this.eof = DFA99_eof;
            this.min = DFA99_min;
            this.max = DFA99_max;
            this.accept = DFA99_accept;
            this.special = DFA99_special;
            this.transition = DFA99_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 347:48: ( exprChoice )*"; }
        }

    }

    const string DFA100_eotS =
        "\x0a\uffff";
    const string DFA100_eofS =
        "\x0a\uffff";
    const string DFA100_minS =
        "\x01\x03\x09\uffff";
    const string DFA100_maxS =
        "\x01\x5c\x09\uffff";
    const string DFA100_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x07\uffff";
    const string DFA100_specialS =
        "\x0a\uffff}>";
    static readonly string[] DFA100_transitionS = {
            "\x01\x01\x07\uffff\x04\x02\x40\uffff\x01\x02\x01\uffff\x01"+
            "\x02\x09\uffff\x02\x02",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA100_eot = DFA.UnpackEncodedString(DFA100_eotS);
    static readonly short[] DFA100_eof = DFA.UnpackEncodedString(DFA100_eofS);
    static readonly char[] DFA100_min = DFA.UnpackEncodedStringToUnsignedChars(DFA100_minS);
    static readonly char[] DFA100_max = DFA.UnpackEncodedStringToUnsignedChars(DFA100_maxS);
    static readonly short[] DFA100_accept = DFA.UnpackEncodedString(DFA100_acceptS);
    static readonly short[] DFA100_special = DFA.UnpackEncodedString(DFA100_specialS);
    static readonly short[][] DFA100_transition = DFA.UnpackEncodedStringArray(DFA100_transitionS);

    protected class DFA100 : DFA
    {
        public DFA100(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 100;
            this.eot = DFA100_eot;
            this.eof = DFA100_eof;
            this.min = DFA100_min;
            this.max = DFA100_max;
            this.accept = DFA100_accept;
            this.special = DFA100_special;
            this.transition = DFA100_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 348:40: ( exprChoice )*"; }
        }

    }

    const string DFA101_eotS =
        "\x0a\uffff";
    const string DFA101_eofS =
        "\x0a\uffff";
    const string DFA101_minS =
        "\x01\x03\x09\uffff";
    const string DFA101_maxS =
        "\x01\x5c\x09\uffff";
    const string DFA101_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x07\uffff";
    const string DFA101_specialS =
        "\x0a\uffff}>";
    static readonly string[] DFA101_transitionS = {
            "\x01\x01\x07\uffff\x04\x02\x40\uffff\x01\x02\x01\uffff\x01"+
            "\x02\x09\uffff\x02\x02",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA101_eot = DFA.UnpackEncodedString(DFA101_eotS);
    static readonly short[] DFA101_eof = DFA.UnpackEncodedString(DFA101_eofS);
    static readonly char[] DFA101_min = DFA.UnpackEncodedStringToUnsignedChars(DFA101_minS);
    static readonly char[] DFA101_max = DFA.UnpackEncodedStringToUnsignedChars(DFA101_maxS);
    static readonly short[] DFA101_accept = DFA.UnpackEncodedString(DFA101_acceptS);
    static readonly short[] DFA101_special = DFA.UnpackEncodedString(DFA101_specialS);
    static readonly short[][] DFA101_transition = DFA.UnpackEncodedStringArray(DFA101_transitionS);

    protected class DFA101 : DFA
    {
        public DFA101(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 101;
            this.eot = DFA101_eot;
            this.eof = DFA101_eof;
            this.min = DFA101_min;
            this.max = DFA101_max;
            this.accept = DFA101_accept;
            this.special = DFA101_special;
            this.transition = DFA101_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 349:41: ( exprChoice )*"; }
        }

    }

    const string DFA103_eotS =
        "\x14\uffff";
    const string DFA103_eofS =
        "\x14\uffff";
    const string DFA103_minS =
        "\x01\x03\x13\uffff";
    const string DFA103_maxS =
        "\x01\u00bc\x13\uffff";
    const string DFA103_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x11\uffff";
    const string DFA103_specialS =
        "\x14\uffff}>";
    static readonly string[] DFA103_transitionS = {
            "\x01\x01\x2f\uffff\x01\x02\x10\uffff\x01\x02\x07\uffff\x03"+
            "\x02\x01\uffff\x01\x02\x22\uffff\x01\x02\x0f\uffff\x01\x02\x16"+
            "\uffff\x02\x02\x0b\uffff\x07\x02\x0e\uffff\x01\x02",
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
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA103_eot = DFA.UnpackEncodedString(DFA103_eotS);
    static readonly short[] DFA103_eof = DFA.UnpackEncodedString(DFA103_eofS);
    static readonly char[] DFA103_min = DFA.UnpackEncodedStringToUnsignedChars(DFA103_minS);
    static readonly char[] DFA103_max = DFA.UnpackEncodedStringToUnsignedChars(DFA103_maxS);
    static readonly short[] DFA103_accept = DFA.UnpackEncodedString(DFA103_acceptS);
    static readonly short[] DFA103_special = DFA.UnpackEncodedString(DFA103_specialS);
    static readonly short[][] DFA103_transition = DFA.UnpackEncodedStringArray(DFA103_transitionS);

    protected class DFA103 : DFA
    {
        public DFA103(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 103;
            this.eot = DFA103_eot;
            this.eof = DFA103_eof;
            this.min = DFA103_min;
            this.max = DFA103_max;
            this.accept = DFA103_accept;
            this.special = DFA103_special;
            this.transition = DFA103_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 354:39: ( parameter )*"; }
        }

    }

    const string DFA106_eotS =
        "\x3c\uffff";
    const string DFA106_eofS =
        "\x3c\uffff";
    const string DFA106_minS =
        "\x01\x03\x3b\uffff";
    const string DFA106_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA106_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA106_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA106_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA106_eot = DFA.UnpackEncodedString(DFA106_eotS);
    static readonly short[] DFA106_eof = DFA.UnpackEncodedString(DFA106_eofS);
    static readonly char[] DFA106_min = DFA.UnpackEncodedStringToUnsignedChars(DFA106_minS);
    static readonly char[] DFA106_max = DFA.UnpackEncodedStringToUnsignedChars(DFA106_maxS);
    static readonly short[] DFA106_accept = DFA.UnpackEncodedString(DFA106_acceptS);
    static readonly short[] DFA106_special = DFA.UnpackEncodedString(DFA106_specialS);
    static readonly short[][] DFA106_transition = DFA.UnpackEncodedStringArray(DFA106_transitionS);

    protected class DFA106 : DFA
    {
        public DFA106(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 106;
            this.eot = DFA106_eot;
            this.eof = DFA106_eof;
            this.min = DFA106_min;
            this.max = DFA106_max;
            this.accept = DFA106_accept;
            this.special = DFA106_special;
            this.transition = DFA106_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 358:46: ( valueExpr )*"; }
        }

    }

    const string DFA107_eotS =
        "\x3c\uffff";
    const string DFA107_eofS =
        "\x3c\uffff";
    const string DFA107_minS =
        "\x01\x03\x3b\uffff";
    const string DFA107_maxS =
        "\x01\u00cd\x3b\uffff";
    const string DFA107_acceptS =
        "\x01\uffff\x01\x02\x01\x01\x39\uffff";
    const string DFA107_specialS =
        "\x3c\uffff}>";
    static readonly string[] DFA107_transitionS = {
            "\x01\x01\x02\uffff\x04\x02\x03\uffff\x01\x02\x03\uffff\x02"+
            "\x02\x02\uffff\x05\x02\x01\uffff\x02\x02\x23\uffff\x03\x02\x02"+
            "\uffff\x03\x02\x1b\uffff\x04\x02\x0c\uffff\x01\x02\x0c\uffff"+
            "\x02\x02\x02\uffff\x01\x02\x05\uffff\x04\x02\x05\uffff\x06\x02"+
            "\x03\uffff\x01\x02\x0a\uffff\x07\x02\x06\uffff\x02\x02\x06\uffff"+
            "\x01\x02\x05\uffff\x03\x02\x02\uffff\x04\x02\x01\uffff\x02\x02",
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
            ""
    };

    static readonly short[] DFA107_eot = DFA.UnpackEncodedString(DFA107_eotS);
    static readonly short[] DFA107_eof = DFA.UnpackEncodedString(DFA107_eofS);
    static readonly char[] DFA107_min = DFA.UnpackEncodedStringToUnsignedChars(DFA107_minS);
    static readonly char[] DFA107_max = DFA.UnpackEncodedStringToUnsignedChars(DFA107_maxS);
    static readonly short[] DFA107_accept = DFA.UnpackEncodedString(DFA107_acceptS);
    static readonly short[] DFA107_special = DFA.UnpackEncodedString(DFA107_specialS);
    static readonly short[][] DFA107_transition = DFA.UnpackEncodedStringArray(DFA107_transitionS);

    protected class DFA107 : DFA
    {
        public DFA107(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 107;
            this.eot = DFA107_eot;
            this.eof = DFA107_eof;
            this.min = DFA107_min;
            this.max = DFA107_max;
            this.accept = DFA107_accept;
            this.special = DFA107_special;
            this.transition = DFA107_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 362:35: ( valueExpr )*"; }
        }

    }

    const string DFA120_eotS =
        "\x0d\uffff";
    const string DFA120_eofS =
        "\x0d\uffff";
    const string DFA120_minS =
        "\x01\x54\x0c\uffff";
    const string DFA120_maxS =
        "\x01\u00ca\x0c\uffff";
    const string DFA120_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x01\x06\x01"+
        "\x07\x01\x08\x01\x09\x01\x0a\x01\x0b\x01\x0c";
    const string DFA120_specialS =
        "\x0d\uffff}>";
    static readonly string[] DFA120_transitionS = {
            "\x01\x07\x01\x08\x01\x09\x01\x0a\x01\x0b\x01\x0c\x60\uffff"+
            "\x01\x01\x0b\uffff\x01\x02\x01\x03\x01\x05\x01\x04\x01\x06",
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
            ""
    };

    static readonly short[] DFA120_eot = DFA.UnpackEncodedString(DFA120_eotS);
    static readonly short[] DFA120_eof = DFA.UnpackEncodedString(DFA120_eofS);
    static readonly char[] DFA120_min = DFA.UnpackEncodedStringToUnsignedChars(DFA120_minS);
    static readonly char[] DFA120_max = DFA.UnpackEncodedStringToUnsignedChars(DFA120_maxS);
    static readonly short[] DFA120_accept = DFA.UnpackEncodedString(DFA120_acceptS);
    static readonly short[] DFA120_special = DFA.UnpackEncodedString(DFA120_specialS);
    static readonly short[][] DFA120_transition = DFA.UnpackEncodedStringArray(DFA120_transitionS);

    protected class DFA120 : DFA
    {
        public DFA120(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 120;
            this.eot = DFA120_eot;
            this.eof = DFA120_eof;
            this.min = DFA120_min;
            this.max = DFA120_max;
            this.accept = DFA120_accept;
            this.special = DFA120_special;
            this.transition = DFA120_transition;

        }

        override public string Description
        {
            get { return "365:1: filterParamComparator : ( ^( EQUALS filterAtom ) | ^( NOT_EQUAL filterAtom ) | ^( LT filterAtom ) | ^( LE filterAtom ) | ^( GT filterAtom ) | ^( GE filterAtom ) | ^( EVENT_FILTER_RANGE ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ( RPAREN | RBRACK ) ) | ^( EVENT_FILTER_NOT_RANGE ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ( RPAREN | RBRACK ) ) | ^( EVENT_FILTER_IN ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier )* ( RPAREN | RBRACK ) ) | ^( EVENT_FILTER_NOT_IN ( LPAREN | LBRACK ) ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier )* ( RPAREN | RBRACK ) ) | ^( EVENT_FILTER_BETWEEN ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ) | ^( EVENT_FILTER_NOT_BETWEEN ( constant[false] | filterIdentifier ) ( constant[false] | filterIdentifier ) ) );"; }
        }

    }

    const string DFA113_eotS =
        "\x0a\uffff";
    const string DFA113_eofS =
        "\x0a\uffff";
    const string DFA113_minS =
        "\x01\x52\x09\uffff";
    const string DFA113_maxS =
        "\x01\u00c0\x09\uffff";
    const string DFA113_acceptS =
        "\x01\uffff\x01\x03\x01\x01\x06\uffff\x01\x02";
    const string DFA113_specialS =
        "\x0a\uffff}>";
    static readonly string[] DFA113_transitionS = {
            "\x01\x09\x54\uffff\x07\x02\x10\uffff\x01\x01\x01\uffff\x01"+
            "\x01",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA113_eot = DFA.UnpackEncodedString(DFA113_eotS);
    static readonly short[] DFA113_eof = DFA.UnpackEncodedString(DFA113_eofS);
    static readonly char[] DFA113_min = DFA.UnpackEncodedStringToUnsignedChars(DFA113_minS);
    static readonly char[] DFA113_max = DFA.UnpackEncodedStringToUnsignedChars(DFA113_maxS);
    static readonly short[] DFA113_accept = DFA.UnpackEncodedString(DFA113_acceptS);
    static readonly short[] DFA113_special = DFA.UnpackEncodedString(DFA113_specialS);
    static readonly short[][] DFA113_transition = DFA.UnpackEncodedStringArray(DFA113_transitionS);

    protected class DFA113 : DFA
    {
        public DFA113(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 113;
            this.eot = DFA113_eot;
            this.eof = DFA113_eof;
            this.min = DFA113_min;
            this.max = DFA113_max;
            this.accept = DFA113_accept;
            this.special = DFA113_special;
            this.transition = DFA113_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 374:73: ( constant[false] | filterIdentifier )*"; }
        }

    }

    const string DFA115_eotS =
        "\x0a\uffff";
    const string DFA115_eofS =
        "\x0a\uffff";
    const string DFA115_minS =
        "\x01\x52\x09\uffff";
    const string DFA115_maxS =
        "\x01\u00c0\x09\uffff";
    const string DFA115_acceptS =
        "\x01\uffff\x01\x03\x01\x01\x06\uffff\x01\x02";
    const string DFA115_specialS =
        "\x0a\uffff}>";
    static readonly string[] DFA115_transitionS = {
            "\x01\x09\x54\uffff\x07\x02\x10\uffff\x01\x01\x01\uffff\x01"+
            "\x01",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA115_eot = DFA.UnpackEncodedString(DFA115_eotS);
    static readonly short[] DFA115_eof = DFA.UnpackEncodedString(DFA115_eofS);
    static readonly char[] DFA115_min = DFA.UnpackEncodedStringToUnsignedChars(DFA115_minS);
    static readonly char[] DFA115_max = DFA.UnpackEncodedStringToUnsignedChars(DFA115_maxS);
    static readonly short[] DFA115_accept = DFA.UnpackEncodedString(DFA115_acceptS);
    static readonly short[] DFA115_special = DFA.UnpackEncodedString(DFA115_specialS);
    static readonly short[][] DFA115_transition = DFA.UnpackEncodedStringArray(DFA115_transitionS);

    protected class DFA115 : DFA
    {
        public DFA115(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 115;
            this.eot = DFA115_eot;
            this.eof = DFA115_eof;
            this.min = DFA115_min;
            this.max = DFA115_max;
            this.accept = DFA115_accept;
            this.special = DFA115_special;
            this.transition = DFA115_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 375:77: ( constant[false] | filterIdentifier )*"; }
        }

    }

    const string DFA126_eotS =
        "\x13\uffff";
    const string DFA126_eofS =
        "\x13\uffff";
    const string DFA126_minS =
        "\x01\x33\x12\uffff";
    const string DFA126_maxS =
        "\x01\u00bc\x12\uffff";
    const string DFA126_acceptS =
        "\x01\uffff\x0f\x01\x01\x02\x01\x03\x01\x04";
    const string DFA126_specialS =
        "\x01\x00\x12\uffff}>";
    static readonly string[] DFA126_transitionS = {
            "\x01\x02\x10\uffff\x01\x03\x07\uffff\x01\x0d\x01\x10\x01\x0e"+
            "\x01\uffff\x01\x11\x22\uffff\x01\x12\x0f\uffff\x01\x0f\x16\uffff"+
            "\x01\x04\x01\x05\x0b\uffff\x01\x06\x01\x07\x01\x08\x01\x09\x01"+
            "\x0a\x01\x0b\x01\x0c\x0e\uffff\x01\x01",
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
            "",
            "",
            ""
    };

    static readonly short[] DFA126_eot = DFA.UnpackEncodedString(DFA126_eotS);
    static readonly short[] DFA126_eof = DFA.UnpackEncodedString(DFA126_eofS);
    static readonly char[] DFA126_min = DFA.UnpackEncodedStringToUnsignedChars(DFA126_minS);
    static readonly char[] DFA126_max = DFA.UnpackEncodedStringToUnsignedChars(DFA126_maxS);
    static readonly short[] DFA126_accept = DFA.UnpackEncodedString(DFA126_acceptS);
    static readonly short[] DFA126_special = DFA.UnpackEncodedString(DFA126_specialS);
    static readonly short[][] DFA126_transition = DFA.UnpackEncodedStringArray(DFA126_transitionS);

    protected class DFA126 : DFA
    {
        public DFA126(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 126;
            this.eot = DFA126_eot;
            this.eof = DFA126_eof;
            this.min = DFA126_min;
            this.max = DFA126_max;
            this.accept = DFA126_accept;
            this.special = DFA126_special;
            this.transition = DFA126_transition;

        }

        override public string Description
        {
            get { return "404:1: parameter : ( ( singleParameter )=> singleParameter | ^( NUMERIC_PARAM_LIST ( numericParameterList )+ ) | ^( ARRAY_PARAM_LIST ( constant[false] )* ) | eventPropertyExpr );"; }
        }

    }


    protected internal int DFA126_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            ITreeNodeStream input = (ITreeNodeStream)_input;
    	int _s = s;
        switch ( s )
        {
               	case 0 : 
                   	int LA126_0 = input.LA(1);

                   	 
                   	int index126_0 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (LA126_0 == STAR) && (synpred1_EsperEPL2Ast()) ) { s = 1; }

                   	else if ( (LA126_0 == LAST) && (synpred1_EsperEPL2Ast()) ) { s = 2; }

                   	else if ( (LA126_0 == LW) && (synpred1_EsperEPL2Ast()) ) { s = 3; }

                   	else if ( (LA126_0 == LAST_OPERATOR) && (synpred1_EsperEPL2Ast()) ) { s = 4; }

                   	else if ( (LA126_0 == WEEKDAY_OPERATOR) && (synpred1_EsperEPL2Ast()) ) { s = 5; }

                   	else if ( (LA126_0 == INT_TYPE) && (synpred1_EsperEPL2Ast()) ) { s = 6; }

                   	else if ( (LA126_0 == LONG_TYPE) && (synpred1_EsperEPL2Ast()) ) { s = 7; }

                   	else if ( (LA126_0 == FLOAT_TYPE) && (synpred1_EsperEPL2Ast()) ) { s = 8; }

                   	else if ( (LA126_0 == DOUBLE_TYPE) && (synpred1_EsperEPL2Ast()) ) { s = 9; }

                   	else if ( (LA126_0 == STRING_TYPE) && (synpred1_EsperEPL2Ast()) ) { s = 10; }

                   	else if ( (LA126_0 == BOOL_TYPE) && (synpred1_EsperEPL2Ast()) ) { s = 11; }

                   	else if ( (LA126_0 == NULL_TYPE) && (synpred1_EsperEPL2Ast()) ) { s = 12; }

                   	else if ( (LA126_0 == NUMERIC_PARAM_RANGE) && (synpred1_EsperEPL2Ast()) ) { s = 13; }

                   	else if ( (LA126_0 == NUMERIC_PARAM_FREQUENCY) && (synpred1_EsperEPL2Ast()) ) { s = 14; }

                   	else if ( (LA126_0 == TIME_PERIOD) && (synpred1_EsperEPL2Ast()) ) { s = 15; }

                   	else if ( (LA126_0 == NUMERIC_PARAM_LIST) ) { s = 16; }

                   	else if ( (LA126_0 == ARRAY_PARAM_LIST) ) { s = 17; }

                   	else if ( (LA126_0 == EVENT_PROP_EXPR) ) { s = 18; }

                   	 
                   	input.Seek(index126_0);
                   	if ( s >= 0 ) return s;
                   	break;
        }
        if (state.backtracking > 0) {state.failed = true; return -1;}
        NoViableAltException nvae =
            new NoViableAltException(dfa.Description, 126, _s, input);
        dfa.Error(nvae);
        throw nvae;
    }
    const string DFA127_eotS =
        "\x10\uffff";
    const string DFA127_eofS =
        "\x10\uffff";
    const string DFA127_minS =
        "\x01\x33\x0f\uffff";
    const string DFA127_maxS =
        "\x01\u00bc\x0f\uffff";
    const string DFA127_acceptS =
        "\x01\uffff\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x01\x06\x06"+
        "\uffff\x01\x07\x01\x08\x01\x09";
    const string DFA127_specialS =
        "\x10\uffff}>";
    static readonly string[] DFA127_transitionS = {
            "\x01\x02\x10\uffff\x01\x03\x07\uffff\x01\x0d\x01\uffff\x01"+
            "\x0e\x34\uffff\x01\x0f\x16\uffff\x01\x04\x01\x05\x0b\uffff\x07"+
            "\x06\x0e\uffff\x01\x01",
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
            ""
    };

    static readonly short[] DFA127_eot = DFA.UnpackEncodedString(DFA127_eotS);
    static readonly short[] DFA127_eof = DFA.UnpackEncodedString(DFA127_eofS);
    static readonly char[] DFA127_min = DFA.UnpackEncodedStringToUnsignedChars(DFA127_minS);
    static readonly char[] DFA127_max = DFA.UnpackEncodedStringToUnsignedChars(DFA127_maxS);
    static readonly short[] DFA127_accept = DFA.UnpackEncodedString(DFA127_acceptS);
    static readonly short[] DFA127_special = DFA.UnpackEncodedString(DFA127_specialS);
    static readonly short[][] DFA127_transition = DFA.UnpackEncodedStringArray(DFA127_transitionS);

    protected class DFA127 : DFA
    {
        public DFA127(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 127;
            this.eot = DFA127_eot;
            this.eof = DFA127_eof;
            this.min = DFA127_min;
            this.max = DFA127_max;
            this.accept = DFA127_accept;
            this.special = DFA127_special;
            this.transition = DFA127_transition;

        }

        override public string Description
        {
            get { return "411:1: singleParameter : ( STAR | LAST | LW | lastOperator | weekDayOperator | constant[false] | ^( NUMERIC_PARAM_RANGE NUM_INT NUM_INT ) | ^( NUMERIC_PARAM_FREQUENCY NUM_INT ) | time_period );"; }
        }

    }

 

    public static readonly BitSet FOLLOW_EPL_EXPR_in_startEPLExpressionRule96 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_eplExpressionRule_in_startEPLExpressionRule98 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_selectExpr_in_eplExpressionRule115 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_createWindowExpr_in_eplExpressionRule119 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_createVariableExpr_in_eplExpressionRule123 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_onExpr_in_eplExpressionRule127 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ON_EXPR_in_onExpr146 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_eventFilterExpr_in_onExpr149 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100001600000000UL});
    public static readonly BitSet FOLLOW_patternInclusionExpression_in_onExpr153 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100001600000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_onExpr156 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100001600000000UL});
    public static readonly BitSet FOLLOW_onDeleteExpr_in_onExpr163 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_onSelectExpr_in_onExpr167 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_onSetExpr_in_onExpr171 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ON_DELETE_EXPR_in_onDeleteExpr191 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_onExprFrom_in_onDeleteExpr193 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000100000000UL});
    public static readonly BitSet FOLLOW_whereClause_in_onDeleteExpr196 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ON_SELECT_EXPR_in_onSelectExpr213 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_insertIntoExpr_in_onSelectExpr216 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000060000000000UL,0x0000000000020000UL});
    public static readonly BitSet FOLLOW_selectionList_in_onSelectExpr220 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0000000800000000UL});
    public static readonly BitSet FOLLOW_onExprFrom_in_onSelectExpr222 = new BitSet(new ulong[]{0x0000000000000008UL,0x0003000300000000UL});
    public static readonly BitSet FOLLOW_whereClause_in_onSelectExpr225 = new BitSet(new ulong[]{0x0000000000000008UL,0x0003000200000000UL});
    public static readonly BitSet FOLLOW_groupByClause_in_onSelectExpr230 = new BitSet(new ulong[]{0x0000000000000008UL,0x0002000200000000UL});
    public static readonly BitSet FOLLOW_havingClause_in_onSelectExpr235 = new BitSet(new ulong[]{0x0000000000000008UL,0x0002000000000000UL});
    public static readonly BitSet FOLLOW_orderByClause_in_onSelectExpr240 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_ON_SET_EXPR_in_onSetExpr257 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_onSetAssignment_in_onSetExpr259 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_onSetAssignment_in_onSetExpr262 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_onSetAssignment277 = new BitSet(new ulong[]{0x000000001BE623C0UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_onSetAssignment279 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_ON_EXPR_FROM_in_onExprFrom291 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_onExprFrom293 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_onExprFrom296 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CREATE_WINDOW_EXPR_in_createWindowExpr314 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_createWindowExpr316 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000024000000UL,0x0000000080000000UL});
    public static readonly BitSet FOLLOW_viewListExpr_in_createWindowExpr319 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000004000000UL,0x0000000080000000UL});
    public static readonly BitSet FOLLOW_createSelectionList_in_createWindowExpr324 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000004000000UL});
    public static readonly BitSet FOLLOW_CLASS_IDENT_in_createWindowExpr328 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CREATE_VARIABLE_EXPR_in_createVariableExpr347 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_createVariableExpr349 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_createVariableExpr351 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_createVariableExpr354 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CREATE_WINDOW_SELECT_EXPR_in_createSelectionList374 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_createSelectionListElement_in_createSelectionList376 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000020000000000UL,0x0000000000020000UL});
    public static readonly BitSet FOLLOW_createSelectionListElement_in_createSelectionList379 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000020000000000UL,0x0000000000020000UL});
    public static readonly BitSet FOLLOW_WILDCARD_SELECT_in_createSelectionListElement399 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SELECTION_ELEMENT_EXPR_in_createSelectionListElement409 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_createSelectionListElement411 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_createSelectionListElement414 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_insertIntoExpr_in_selectExpr432 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000010000000000UL});
    public static readonly BitSet FOLLOW_selectClause_in_selectExpr438 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000080000000000UL});
    public static readonly BitSet FOLLOW_fromClause_in_selectExpr443 = new BitSet(new ulong[]{0x0000000000000002UL,0x3C03000300000000UL});
    public static readonly BitSet FOLLOW_whereClause_in_selectExpr448 = new BitSet(new ulong[]{0x0000000000000002UL,0x3C03000200000000UL});
    public static readonly BitSet FOLLOW_groupByClause_in_selectExpr455 = new BitSet(new ulong[]{0x0000000000000002UL,0x3C02000200000000UL});
    public static readonly BitSet FOLLOW_havingClause_in_selectExpr462 = new BitSet(new ulong[]{0x0000000000000002UL,0x3C02000000000000UL});
    public static readonly BitSet FOLLOW_outputLimitExpr_in_selectExpr469 = new BitSet(new ulong[]{0x0000000000000002UL,0x0002000000000000UL});
    public static readonly BitSet FOLLOW_orderByClause_in_selectExpr476 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INSERTINTO_EXPR_in_insertIntoExpr493 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_insertIntoExpr495 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_insertIntoExpr504 = new BitSet(new ulong[]{0x0000000000000008UL,0x8000000000000000UL});
    public static readonly BitSet FOLLOW_insertIntoExprCol_in_insertIntoExpr507 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_INSERTINTO_EXPRCOL_in_insertIntoExprCol526 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_insertIntoExprCol528 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_insertIntoExprCol531 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_SELECTION_EXPR_in_selectClause549 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_selectClause551 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000060000000000UL,0x0000000000020000UL});
    public static readonly BitSet FOLLOW_selectionList_in_selectClause564 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_streamExpression_in_fromClause578 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000080000000000UL});
    public static readonly BitSet FOLLOW_streamExpression_in_fromClause581 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000E80000000000UL});
    public static readonly BitSet FOLLOW_outerJoin_in_fromClause584 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000E80000000000UL});
    public static readonly BitSet FOLLOW_selectionListElement_in_selectionList601 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000060000000000UL,0x0000000000020000UL});
    public static readonly BitSet FOLLOW_selectionListElement_in_selectionList604 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000060000000000UL,0x0000000000020000UL});
    public static readonly BitSet FOLLOW_WILDCARD_SELECT_in_selectionListElement620 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SELECTION_ELEMENT_EXPR_in_selectionListElement630 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_selectionListElement632 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_selectionListElement635 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SELECTION_STREAM_in_selectionListElement649 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_selectionListElement651 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_selectionListElement654 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_outerJoinIdent_in_outerJoin673 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_LEFT_OUTERJOIN_EXPR_in_outerJoinIdent687 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_outerJoinIdent689 = new BitSet(new ulong[]{0x0000000000000000UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_outerJoinIdent691 = new BitSet(new ulong[]{0x0000000000000008UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_outerJoinIdent694 = new BitSet(new ulong[]{0x0000000000000000UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_outerJoinIdent696 = new BitSet(new ulong[]{0x0000000000000008UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_RIGHT_OUTERJOIN_EXPR_in_outerJoinIdent710 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_outerJoinIdent712 = new BitSet(new ulong[]{0x0000000000000008UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_outerJoinIdent714 = new BitSet(new ulong[]{0x0000000000000008UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_outerJoinIdent717 = new BitSet(new ulong[]{0x0000000000000008UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_outerJoinIdent719 = new BitSet(new ulong[]{0x0000000000000008UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_FULL_OUTERJOIN_EXPR_in_outerJoinIdent733 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_outerJoinIdent735 = new BitSet(new ulong[]{0x0000000000000008UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_outerJoinIdent737 = new BitSet(new ulong[]{0x0000000000000008UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_outerJoinIdent740 = new BitSet(new ulong[]{0x0000000000000008UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_outerJoinIdent742 = new BitSet(new ulong[]{0x0000000000000008UL,0x0008000000000000UL});
    public static readonly BitSet FOLLOW_STREAM_EXPR_in_streamExpression762 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_eventFilterExpr_in_streamExpression765 = new BitSet(new ulong[]{0x1000000000000008UL,0x0000000020000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_patternInclusionExpression_in_streamExpression769 = new BitSet(new ulong[]{0x1000000000000008UL,0x0000000020000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_databaseJoinExpression_in_streamExpression773 = new BitSet(new ulong[]{0x1000000000000008UL,0x0000000020000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_methodJoinExpression_in_streamExpression777 = new BitSet(new ulong[]{0x1000000000000008UL,0x0000000020000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_viewListExpr_in_streamExpression781 = new BitSet(new ulong[]{0x1000000000000008UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_streamExpression786 = new BitSet(new ulong[]{0x1000000000000008UL});
    public static readonly BitSet FOLLOW_UNIDIRECTIONAL_in_streamExpression791 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_PATTERN_INCL_EXPR_in_patternInclusionExpression811 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprChoice_in_patternInclusionExpression815 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DATABASE_JOIN_EXPR_in_databaseJoinExpression832 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_databaseJoinExpression834 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x00C0000000000000UL});
    public static readonly BitSet FOLLOW_set_in_databaseJoinExpression836 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x00C0000000000000UL});
    public static readonly BitSet FOLLOW_set_in_databaseJoinExpression844 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_METHOD_JOIN_EXPR_in_methodJoinExpression865 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_methodJoinExpression867 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000004000000UL});
    public static readonly BitSet FOLLOW_CLASS_IDENT_in_methodJoinExpression869 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_methodJoinExpression872 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_viewExpr_in_viewListExpr886 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000020000000UL});
    public static readonly BitSet FOLLOW_viewExpr_in_viewListExpr889 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000020000000UL});
    public static readonly BitSet FOLLOW_VIEW_EXPR_in_viewExpr906 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_viewExpr908 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_viewExpr910 = new BitSet(new ulong[]{0x0008000000000008UL,0x0008000000017010UL,0x10003F800C000008UL});
    public static readonly BitSet FOLLOW_parameter_in_viewExpr913 = new BitSet(new ulong[]{0x0008000000000008UL,0x0008000000017010UL,0x10003F800C000008UL});
    public static readonly BitSet FOLLOW_WHERE_EXPR_in_whereClause934 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_whereClause936 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GROUP_BY_EXPR_in_groupByClause954 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_groupByClause956 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_groupByClause959 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_ORDER_BY_EXPR_in_orderByClause977 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_orderByElement_in_orderByClause979 = new BitSet(new ulong[]{0x0000000000000008UL,0x0004000000000000UL});
    public static readonly BitSet FOLLOW_orderByElement_in_orderByClause982 = new BitSet(new ulong[]{0x0000000000000008UL,0x0004000000000000UL});
    public static readonly BitSet FOLLOW_ORDER_ELEMENT_EXPR_in_orderByElement1002 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_orderByElement1004 = new BitSet(new ulong[]{0x0180000000000008UL});
    public static readonly BitSet FOLLOW_set_in_orderByElement1006 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_HAVING_EXPR_in_havingClause1029 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_havingClause1031 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_LIMIT_EXPR_in_outputLimitExpr1049 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_outputLimitExpr1051 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100078000000000UL});
    public static readonly BitSet FOLLOW_number_in_outputLimitExpr1063 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IDENT_in_outputLimitExpr1065 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SEC_LIMIT_EXPR_in_outputLimitExpr1082 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_outputLimitExpr1084 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100078000000000UL});
    public static readonly BitSet FOLLOW_number_in_outputLimitExpr1096 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IDENT_in_outputLimitExpr1098 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_MIN_LIMIT_EXPR_in_outputLimitExpr1114 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_outputLimitExpr1116 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100078000000000UL});
    public static readonly BitSet FOLLOW_number_in_outputLimitExpr1128 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IDENT_in_outputLimitExpr1130 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_TIMEPERIOD_LIMIT_EXPR_in_outputLimitExpr1146 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_outputLimitExpr1148 = new BitSet(new ulong[]{0x0008000000000000UL,0x0000000000005010UL,0x10003F800C000008UL});
    public static readonly BitSet FOLLOW_time_period_in_outputLimitExpr1159 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LT_in_relationalExpr1178 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_relationalExpr1180 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_relationalExpr1182 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GT_in_relationalExpr1194 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_relationalExpr1196 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_relationalExpr1198 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LE_in_relationalExpr1210 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_relationalExpr1212 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_relationalExpr1214 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GE_in_relationalExpr1225 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_relationalExpr1227 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_relationalExpr1229 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVAL_OR_EXPR_in_evalExprChoice1246 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_evalExprChoice1248 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_evalExprChoice1250 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_evalExprChoice1253 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_EVAL_AND_EXPR_in_evalExprChoice1267 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_evalExprChoice1269 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_evalExprChoice1271 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_evalExprChoice1274 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_EVAL_EQUALS_EXPR_in_evalExprChoice1288 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_evalExprChoice1290 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_evalExprChoice1292 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVAL_NOTEQUALS_EXPR_in_evalExprChoice1304 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_evalExprChoice1306 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_evalExprChoice1308 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_EXPR_in_evalExprChoice1320 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_evalExprChoice1322 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_relationalExpr_in_evalExprChoice1333 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_constant_in_valueExpr1346 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_substitution_in_valueExpr1352 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arithmeticExpr_in_valueExpr1358 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_valueExpr1365 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_evalExprChoice_in_valueExpr1373 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_builtinFunc_in_valueExpr1378 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_libFunc_in_valueExpr1386 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_caseExpr_in_valueExpr1391 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_inExpr_in_valueExpr1396 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_betweenExpr_in_valueExpr1402 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_likeExpr_in_valueExpr1407 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_regExpExpr_in_valueExpr1412 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_arrayExpr_in_valueExpr1417 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_subSelectInExpr_in_valueExpr1422 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_subSelectRowExpr_in_valueExpr1428 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_subSelectExistsExpr_in_valueExpr1435 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SUBSELECT_EXPR_in_subSelectRowExpr1451 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_subQueryExpr_in_subSelectRowExpr1453 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EXISTS_SUBSELECT_EXPR_in_subSelectExistsExpr1472 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_subQueryExpr_in_subSelectExistsExpr1474 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_SUBSELECT_EXPR_in_subSelectInExpr1493 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_subSelectInExpr1495 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0000000002000000UL});
    public static readonly BitSet FOLLOW_subSelectInQueryExpr_in_subSelectInExpr1497 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_IN_SUBSELECT_EXPR_in_subSelectInExpr1509 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_subSelectInExpr1511 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0000000002000000UL});
    public static readonly BitSet FOLLOW_subSelectInQueryExpr_in_subSelectInExpr1513 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_SUBSELECT_QUERY_EXPR_in_subSelectInQueryExpr1532 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_subQueryExpr_in_subSelectInQueryExpr1534 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_selectionListElement_in_subQueryExpr1550 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000080000000000UL});
    public static readonly BitSet FOLLOW_subSelectFilterExpr_in_subQueryExpr1552 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000120000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_viewExpr_in_subQueryExpr1555 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000120000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_subQueryExpr1560 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000100000000UL});
    public static readonly BitSet FOLLOW_whereClause_in_subQueryExpr1565 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_STREAM_EXPR_in_subSelectFilterExpr1582 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_eventFilterExpr_in_subSelectFilterExpr1584 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000020000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_viewListExpr_in_subSelectFilterExpr1587 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_subSelectFilterExpr1592 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CASE_in_caseExpr1613 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_caseExpr1616 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_CASE2_in_caseExpr1629 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_caseExpr1632 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_IN_SET_in_inExpr1652 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_inExpr1654 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0xA000000000000000UL});
    public static readonly BitSet FOLLOW_set_in_inExpr1656 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x10303F8011F83C13UL,0x000000000000379CUL});
    public static readonly BitSet FOLLOW_valueExpr_in_inExpr1662 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_inExpr1665 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_set_in_inExpr1669 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_IN_SET_in_inExpr1684 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_inExpr1686 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0xA000000000000000UL});
    public static readonly BitSet FOLLOW_set_in_inExpr1688 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_inExpr1694 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_inExpr1697 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_set_in_inExpr1701 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_IN_RANGE_in_inExpr1716 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_inExpr1718 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0xA000000000000000UL});
    public static readonly BitSet FOLLOW_set_in_inExpr1720 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_inExpr1726 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_inExpr1728 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x4000000000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_set_in_inExpr1730 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_IN_RANGE_in_inExpr1745 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_inExpr1747 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0xA000000000000000UL});
    public static readonly BitSet FOLLOW_set_in_inExpr1749 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_inExpr1755 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_inExpr1757 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x4000000000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_set_in_inExpr1759 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BETWEEN_in_betweenExpr1782 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_betweenExpr1784 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_betweenExpr1786 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_betweenExpr1788 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_BETWEEN_in_betweenExpr1799 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_betweenExpr1801 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_betweenExpr1803 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_betweenExpr1806 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_LIKE_in_likeExpr1826 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_likeExpr1828 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_likeExpr1830 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_likeExpr1833 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_LIKE_in_likeExpr1846 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_likeExpr1848 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_likeExpr1850 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_likeExpr1853 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_REGEXP_in_regExpExpr1872 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_regExpExpr1874 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_regExpExpr1876 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_REGEXP_in_regExpExpr1887 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_regExpExpr1889 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_regExpExpr1891 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SUM_in_builtinFunc1910 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_builtinFunc1913 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_builtinFunc1917 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_AVG_in_builtinFunc1928 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_builtinFunc1931 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_builtinFunc1935 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_COUNT_in_builtinFunc1946 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_builtinFunc1950 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_builtinFunc1954 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_MEDIAN_in_builtinFunc1968 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_builtinFunc1971 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_builtinFunc1975 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_STDDEV_in_builtinFunc1986 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_builtinFunc1989 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_builtinFunc1993 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_AVEDEV_in_builtinFunc2004 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_DISTINCT_in_builtinFunc2007 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_builtinFunc2011 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_COALESCE_in_builtinFunc2023 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_builtinFunc2025 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_builtinFunc2027 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_builtinFunc2030 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_PREVIOUS_in_builtinFunc2045 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_builtinFunc2047 = new BitSet(new ulong[]{0x0008000000000008UL,0x0008000000017010UL,0x10003F800C000008UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_builtinFunc2049 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_PRIOR_in_builtinFunc2061 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_builtinFunc2065 = new BitSet(new ulong[]{0x0008000000000008UL,0x0008000000017010UL,0x10003F800C000008UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_builtinFunc2067 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_INSTANCEOF_in_builtinFunc2079 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_builtinFunc2081 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000004000000UL});
    public static readonly BitSet FOLLOW_CLASS_IDENT_in_builtinFunc2083 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000004000000UL});
    public static readonly BitSet FOLLOW_CLASS_IDENT_in_builtinFunc2086 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000004000000UL});
    public static readonly BitSet FOLLOW_CAST_in_builtinFunc2100 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_builtinFunc2102 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000004000000UL});
    public static readonly BitSet FOLLOW_CLASS_IDENT_in_builtinFunc2104 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EXISTS_in_builtinFunc2116 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_builtinFunc2118 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CURRENT_TIMESTAMP_in_builtinFunc2129 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_ARRAY_EXPR_in_arrayExpr2149 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_arrayExpr2152 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_PLUS_in_arithmeticExpr2173 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2175 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2177 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_MINUS_in_arithmeticExpr2189 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2191 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2193 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_DIV_in_arithmeticExpr2205 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2207 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2209 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_STAR_in_arithmeticExpr2220 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2222 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2224 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_MOD_in_arithmeticExpr2236 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2238 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2240 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BAND_in_arithmeticExpr2251 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2253 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2255 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BOR_in_arithmeticExpr2266 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2268 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2270 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BXOR_in_arithmeticExpr2281 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2283 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2285 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_CONCAT_in_arithmeticExpr2297 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2299 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2301 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_arithmeticExpr2304 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_LIB_FUNCTION_in_libFunc2325 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_CLASS_IDENT_in_libFunc2328 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_libFunc2332 = new BitSet(new ulong[]{0x000010001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_DISTINCT_in_libFunc2335 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_libFunc2340 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_exprChoice_in_startPatternExpressionRule2362 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_atomicExpr_in_exprChoice2376 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_patternOp_in_exprChoice2381 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_EVERY_EXPR_in_exprChoice2391 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprChoice_in_exprChoice2393 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_EXPR_in_exprChoice2407 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprChoice_in_exprChoice2409 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GUARD_EXPR_in_exprChoice2423 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprChoice_in_exprChoice2425 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_exprChoice2427 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_exprChoice2429 = new BitSet(new ulong[]{0x0008000000000008UL,0x0008000000017010UL,0x10003F800C000008UL});
    public static readonly BitSet FOLLOW_parameter_in_exprChoice2431 = new BitSet(new ulong[]{0x0008000000000008UL,0x0008000000017010UL,0x10003F800C000008UL});
    public static readonly BitSet FOLLOW_FOLLOWED_BY_EXPR_in_patternOp2452 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprChoice_in_patternOp2454 = new BitSet(new ulong[]{0x0000000000007800UL,0x0000000018028000UL});
    public static readonly BitSet FOLLOW_exprChoice_in_patternOp2456 = new BitSet(new ulong[]{0x0000000000007808UL,0x0000000018028000UL});
    public static readonly BitSet FOLLOW_exprChoice_in_patternOp2459 = new BitSet(new ulong[]{0x0000000000007808UL,0x0000000018028000UL});
    public static readonly BitSet FOLLOW_OR_EXPR_in_patternOp2475 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprChoice_in_patternOp2477 = new BitSet(new ulong[]{0x0000000000007808UL,0x0000000018028000UL});
    public static readonly BitSet FOLLOW_exprChoice_in_patternOp2479 = new BitSet(new ulong[]{0x0000000000007808UL,0x0000000018028000UL});
    public static readonly BitSet FOLLOW_exprChoice_in_patternOp2482 = new BitSet(new ulong[]{0x0000000000007808UL,0x0000000018028000UL});
    public static readonly BitSet FOLLOW_AND_EXPR_in_patternOp2498 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exprChoice_in_patternOp2500 = new BitSet(new ulong[]{0x0000000000007808UL,0x0000000018028000UL});
    public static readonly BitSet FOLLOW_exprChoice_in_patternOp2502 = new BitSet(new ulong[]{0x0000000000007808UL,0x0000000018028000UL});
    public static readonly BitSet FOLLOW_exprChoice_in_patternOp2505 = new BitSet(new ulong[]{0x0000000000007808UL,0x0000000018028000UL});
    public static readonly BitSet FOLLOW_eventFilterExpr_in_atomicExpr2524 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_OBSERVER_EXPR_in_atomicExpr2536 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_atomicExpr2538 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0100000000000000UL});
    public static readonly BitSet FOLLOW_IDENT_in_atomicExpr2540 = new BitSet(new ulong[]{0x0008000000000008UL,0x0008000000017010UL,0x10003F800C000008UL});
    public static readonly BitSet FOLLOW_parameter_in_atomicExpr2542 = new BitSet(new ulong[]{0x0008000000000008UL,0x0008000000017010UL,0x10003F800C000008UL});
    public static readonly BitSet FOLLOW_EVENT_FILTER_EXPR_in_eventFilterExpr2562 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_eventFilterExpr2564 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000004000000UL});
    public static readonly BitSet FOLLOW_CLASS_IDENT_in_eventFilterExpr2567 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_eventFilterExpr2570 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_EVENT_FILTER_PARAM_in_filterParam2589 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_valueExpr_in_filterParam2591 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_valueExpr_in_filterParam2594 = new BitSet(new ulong[]{0x000000001BE623C8UL,0x00080078000000E7UL,0x50303F8011F83C13UL,0x000000000000379DUL});
    public static readonly BitSet FOLLOW_EQUALS_in_filterParamComparator2610 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_filterAtom_in_filterParamComparator2612 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NOT_EQUAL_in_filterParamComparator2619 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_filterAtom_in_filterParamComparator2621 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LT_in_filterParamComparator2628 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_filterAtom_in_filterParamComparator2630 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LE_in_filterParamComparator2637 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_filterAtom_in_filterParamComparator2639 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GT_in_filterParamComparator2646 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_filterAtom_in_filterParamComparator2648 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_GE_in_filterParamComparator2655 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_filterAtom_in_filterParamComparator2657 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_FILTER_RANGE_in_filterParamComparator2664 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_filterParamComparator2666 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x00003F8000000000UL});
    public static readonly BitSet FOLLOW_constant_in_filterParamComparator2673 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x00003F8000000000UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterParamComparator2676 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x00003F8000000000UL});
    public static readonly BitSet FOLLOW_constant_in_filterParamComparator2680 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x4000000000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterParamComparator2683 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x4000000000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_set_in_filterParamComparator2686 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_FILTER_NOT_RANGE_in_filterParamComparator2697 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_filterParamComparator2699 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x00003F8000000000UL});
    public static readonly BitSet FOLLOW_constant_in_filterParamComparator2706 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x00003F8000000000UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterParamComparator2709 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x00003F8000000000UL});
    public static readonly BitSet FOLLOW_constant_in_filterParamComparator2713 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x4000000000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterParamComparator2716 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x4000000000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_set_in_filterParamComparator2719 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_FILTER_IN_in_filterParamComparator2730 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_filterParamComparator2732 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x00003F8000000000UL});
    public static readonly BitSet FOLLOW_constant_in_filterParamComparator2739 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterParamComparator2742 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_constant_in_filterParamComparator2746 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterParamComparator2749 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_set_in_filterParamComparator2753 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_FILTER_NOT_IN_in_filterParamComparator2764 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_set_in_filterParamComparator2766 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_constant_in_filterParamComparator2773 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterParamComparator2776 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_constant_in_filterParamComparator2780 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterParamComparator2783 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_set_in_filterParamComparator2787 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_FILTER_BETWEEN_in_filterParamComparator2798 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_constant_in_filterParamComparator2801 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterParamComparator2804 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_constant_in_filterParamComparator2808 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterParamComparator2811 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_FILTER_NOT_BETWEEN_in_filterParamComparator2819 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_constant_in_filterParamComparator2822 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterParamComparator2825 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000040000UL,0x40003F8000000000UL,0x0000000000000001UL});
    public static readonly BitSet FOLLOW_constant_in_filterParamComparator2829 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterParamComparator2832 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_constant_in_filterAtom2846 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_filterIdentifier_in_filterAtom2852 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_EVENT_FILTER_IDENT_in_filterIdentifier2863 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_filterIdentifier2865 = new BitSet(new ulong[]{0x0008000000000008UL,0x0008000000017010UL,0x10003F800C000008UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_filterIdentifier2867 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_PROP_EXPR_in_eventPropertyExpr2884 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_eventPropertyAtomic_in_eventPropertyExpr2886 = new BitSet(new ulong[]{0x0000000000000008UL,0x03F0000000000000UL});
    public static readonly BitSet FOLLOW_eventPropertyAtomic_in_eventPropertyExpr2889 = new BitSet(new ulong[]{0x0000000000000008UL,0x03F0000000000000UL});
    public static readonly BitSet FOLLOW_EVENT_PROP_SIMPLE_in_eventPropertyAtomic2908 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_eventPropertyAtomic2910 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_PROP_INDEXED_in_eventPropertyAtomic2917 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_eventPropertyAtomic2919 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0001000000000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_eventPropertyAtomic2921 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_PROP_MAPPED_in_eventPropertyAtomic2928 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_eventPropertyAtomic2930 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x00C0000000000000UL});
    public static readonly BitSet FOLLOW_set_in_eventPropertyAtomic2932 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_PROP_DYNAMIC_SIMPLE_in_eventPropertyAtomic2945 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_eventPropertyAtomic2947 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_PROP_DYNAMIC_INDEXED_in_eventPropertyAtomic2954 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_eventPropertyAtomic2956 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0001000000000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_eventPropertyAtomic2958 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EVENT_PROP_DYNAMIC_MAPPED_in_eventPropertyAtomic2965 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_IDENT_in_eventPropertyAtomic2967 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x00C0000000000000UL});
    public static readonly BitSet FOLLOW_set_in_eventPropertyAtomic2969 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_singleParameter_in_parameter2999 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NUMERIC_PARAM_LIST_in_parameter3007 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_numericParameterList_in_parameter3010 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000001000UL,0x0001000000000000UL,0x0800000000000000UL});
    public static readonly BitSet FOLLOW_ARRAY_PARAM_LIST_in_parameter3021 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_constant_in_parameter3024 = new BitSet(new ulong[]{0x0000000000000008UL,0x0000000000000000UL,0x00003F8000000000UL});
    public static readonly BitSet FOLLOW_eventPropertyExpr_in_parameter3033 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_STAR_in_singleParameter3045 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_LAST_in_singleParameter3050 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_LW_in_singleParameter3055 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_lastOperator_in_singleParameter3060 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_weekDayOperator_in_singleParameter3065 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_constant_in_singleParameter3071 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NUMERIC_PARAM_RANGE_in_singleParameter3080 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_singleParameter3082 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0001000000000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_singleParameter3084 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NUMERIC_PARAM_FREQUENCY_in_singleParameter3093 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_singleParameter3095 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_time_period_in_singleParameter3102 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_numericParameterList3114 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NUMERIC_PARAM_RANGE_in_numericParameterList3122 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_numericParameterList3124 = new BitSet(new ulong[]{0x0000000000000000UL,0x0000000000000000UL,0x0001000000000000UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_numericParameterList3126 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_NUMERIC_PARAM_FREQUENCE_in_numericParameterList3135 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_numericParameterList3137 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LAST_OPERATOR_in_lastOperator3151 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_lastOperator3153 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_WEEKDAY_OPERATOR_in_weekDayOperator3168 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_NUM_INT_in_weekDayOperator3170 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_TIME_PERIOD_in_time_period3186 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_timePeriodDef_in_time_period3188 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_dayPart_in_timePeriodDef3203 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x00000000000003C0UL});
    public static readonly BitSet FOLLOW_hourPart_in_timePeriodDef3206 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000000380UL});
    public static readonly BitSet FOLLOW_minutePart_in_timePeriodDef3211 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000000300UL});
    public static readonly BitSet FOLLOW_secondPart_in_timePeriodDef3216 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000000200UL});
    public static readonly BitSet FOLLOW_millisecondPart_in_timePeriodDef3221 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_hourPart_in_timePeriodDef3228 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000000380UL});
    public static readonly BitSet FOLLOW_minutePart_in_timePeriodDef3231 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000000300UL});
    public static readonly BitSet FOLLOW_secondPart_in_timePeriodDef3236 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000000200UL});
    public static readonly BitSet FOLLOW_millisecondPart_in_timePeriodDef3241 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_minutePart_in_timePeriodDef3248 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000000300UL});
    public static readonly BitSet FOLLOW_secondPart_in_timePeriodDef3251 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000000200UL});
    public static readonly BitSet FOLLOW_millisecondPart_in_timePeriodDef3256 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_secondPart_in_timePeriodDef3263 = new BitSet(new ulong[]{0x0000000000000002UL,0x0000000000000000UL,0x0000000000000200UL});
    public static readonly BitSet FOLLOW_millisecondPart_in_timePeriodDef3266 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_millisecondPart_in_timePeriodDef3273 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DAY_PART_in_dayPart3287 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_number_in_dayPart3289 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_HOUR_PART_in_hourPart3303 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_number_in_hourPart3305 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_MINUTE_PART_in_minutePart3319 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_number_in_minutePart3321 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SECOND_PART_in_secondPart3335 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_number_in_secondPart3337 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_MILLISECOND_PART_in_millisecondPart3351 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_number_in_millisecondPart3353 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SUBSTITUTION_in_substitution3367 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_INT_TYPE_in_constant3383 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_LONG_TYPE_in_constant3392 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_FLOAT_TYPE_in_constant3401 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_DOUBLE_TYPE_in_constant3410 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_STRING_TYPE_in_constant3426 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_BOOL_TYPE_in_constant3442 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NULL_TYPE_in_constant3455 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_number0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_singleParameter_in_synpred1_EsperEPL2Ast2994 = new BitSet(new ulong[]{0x0000000000000002UL});

}
