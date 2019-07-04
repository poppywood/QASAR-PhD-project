///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using Antlr.Runtime;

using com.espertech.esper.client;
using com.espertech.esper.epl.generated;

namespace com.espertech.esper.epl.parse
{
    /// <summary>This exception is thrown to indicate a problem in statement creation. </summary>
    [Serializable]
    public class EPStatementSyntaxException : EPStatementException
    {
        /// <summary>Ctor. </summary>
        /// <param name="message">error message</param>
        /// <param name="expression">expression text</param>
        public EPStatementSyntaxException(String message, String expression)
            : base(message, expression)
        {
        }
    
        /// <summary>Converts from a syntax error to a nice statement exception. </summary>
        /// <param name="e">is the syntax error</param>
        /// <param name="expression">is the expression text</param>
        /// <param name="parser">the parser that parsed the expression</param>
        /// <returns>syntax exception</returns>
        public static EPStatementSyntaxException Convert(RecognitionException e, String expression, EsperEPL2GrammarParser parser)
        {
            string message;
            if (expression.Trim().Length == 0) {
                message = "Unexpected end of input";
                return new EPStatementSyntaxException(message, expression);
            }

            IToken t;
            if (e.Index < parser.TokenStream.Size())
            {
                t = parser.TokenStream.Get(e.Index);
            }
            else
            {
                t = parser.TokenStream.Get(parser.TokenStream.Size() - 1);
            }                
            String positionInfo = GetPositionInfo(t);
            String token = "'" + t.Text + "'";

            Stack<string> stack = parser.getParaphrases();
            String check = "";
            if (stack.Count > 0)
            {
                String delimiter = "";
                StringBuilder checkList = new StringBuilder();
                checkList.Append(", please check the ");
                while(stack.Count != 0)
                {
                    checkList.Append(delimiter);
                    checkList.Append(stack.Pop());
                    delimiter = " within the ";
                }
                check = checkList.ToString();
            }
    
            message = "Incorrect syntax near " + token + positionInfo + check;
            if (e is NoViableAltException)
            {
                NoViableAltException nvae = (NoViableAltException) e;
                if (nvae.Token.Type == -1)
                {
                    message = "Unexpected end of input near " + token + positionInfo + check;
                }
                else
                {
                    if (parser.getParserTokenParaphrases().Get(nvae.Token.Type) != null)
                    {
                        message = "Incorrect syntax near " + token + " (a reserved keyword)" + positionInfo + check;
                    }
                }
            }
    
            if (e is MismatchedTokenException)
            {
                MismatchedTokenException mismatched = (MismatchedTokenException) e;
    
                String expected = "end of input";
                if ((mismatched.Expecting >= 0) && (mismatched.Expecting < parser.TokenNames.Length))
                {
                    expected = parser.TokenNames[mismatched.Expecting];
                }
                if (parser.getLexerTokenParaphrases().Get(mismatched.Expecting) != null)
                {
                    expected = parser.getLexerTokenParaphrases().Get(mismatched.Expecting);
                }
                if (parser.getParserTokenParaphrases().Get(mismatched.Expecting) != null)
                {
                    expected = parser.getParserTokenParaphrases().Get(mismatched.Expecting);
                }
    
                String unexpected;
                if ((mismatched.UnexpectedType < 0) || (mismatched.UnexpectedType >= parser.TokenNames.Length))
                {
                    unexpected = "end of input";
                }
                else
                {
                    unexpected = parser.TokenNames[mismatched.UnexpectedType];
                }
                if (parser.getLexerTokenParaphrases().Get(mismatched.UnexpectedType) != null)
                {
                    unexpected = parser.getLexerTokenParaphrases().Get(mismatched.UnexpectedType);
                }
                if (parser.getParserTokenParaphrases().Get(mismatched.UnexpectedType) != null)
                {
                    unexpected = parser.getParserTokenParaphrases().Get(mismatched.UnexpectedType);
                }
    
                String expecting = " expecting " + expected.Trim() + " but found " + unexpected.Trim();
                message = "Incorrect syntax near " + token + expecting + positionInfo + check;
            }
    
            return new EPStatementSyntaxException(message, expression);
        }
    
        /// <summary>Converts from a syntax error to a nice statement exception. </summary>
        /// <param name="e">is the syntax error</param>
        /// <param name="expression">is the expression text</param>
        /// <param name="treeWalker">the tree walker that walked the tree</param>
        /// <returns>syntax exception</returns>
        public static EPStatementSyntaxException Convert(RecognitionException e, String expression, EsperEPL2Ast treeWalker)
        {
            String positionInfo = GetPositionInfo(e.Token);
            String tokenName = "end of input";
            if ((e.Token != null) && (e.Token.Type >= 0) && (e.Token.Type < treeWalker.TokenNames.Length))
            {
                tokenName = treeWalker.TokenNames[e.Token.Type];
            }
    
            String message = "Unexpected error processing statement near token " + tokenName + positionInfo;
    
            if (e is MismatchedTokenException)
            {
                MismatchedTokenException mismatched = (MismatchedTokenException) e;
    
                String expected = "end of input";
                if ((mismatched.Expecting >= 0) && (mismatched.Expecting < treeWalker.TokenNames.Length))
                {
                    expected = treeWalker.TokenNames[mismatched.Expecting];
                }
    
                String unexpected;
                if ((mismatched.UnexpectedType < 0) || (mismatched.UnexpectedType >= treeWalker.TokenNames.Length))
                {
                    unexpected = "end of input";
                }
                else
                {
                    unexpected = treeWalker.TokenNames[mismatched.UnexpectedType];
                }
    
                String expecting = " expecting " + expected.Trim() + " but found " + unexpected.Trim();
                message = "Unexpected error processing statement near token " + tokenName + expecting + positionInfo;
            }
    
            return new EPStatementSyntaxException(message, expression);
        }
    
        /// <summary>Returns the position information string for a parser exception. </summary>
        /// <param name="t">the token to return the information for</param>
        /// <returns>is a string with line and column information</returns>
        private static String GetPositionInfo(IToken t)
        {
            return t.Line > 0 && t.CharPositionInLine > 0
                    ? " at line " + t.Line + " column " + t.CharPositionInLine
                    : "";
        }
    }
    
    
    
}
