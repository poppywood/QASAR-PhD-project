///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

using com.espertech.esper.antlr;
using com.espertech.esper.client;
using com.espertech.esper.epl.generated;

using log4net;

namespace com.espertech.esper.epl.parse
{
    /// <summary>Helper class for parsing an expression and walking a parse tree. </summary>
    public class ParseHelper
    {
        /// <summary>Walk parse tree starting at the rule the walkRuleSelector supplies. </summary>
        /// <param name="ast">ast to walk</param>
        /// <param name="walker">walker instance</param>
        /// <param name="walkRuleSelector">walk rule</param>
        /// <param name="expression">the expression we are walking in string form</param>
        public static void Walk(ITree ast, EPLTreeWalker walker, WalkRuleSelector walkRuleSelector, String expression)
        {
            // Walk tree
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(".walk Walking AST using walker " + walker.GetType().FullName);
                }
    
                walkRuleSelector.Invoke(walker);
    
                if (log.IsDebugEnabled)
                {
                    log.Debug(".walk AST tree after walking");
                    ASTUtil.DumpAST(ast);
                }
            }
            catch (RecognitionException e)
            {
                log.Info("Error walking statement [" + expression + "]", e);
                throw EPStatementSyntaxException.Convert(e, expression, walker);
            }
            catch (Exception e)
            {
                log.Info("Error walking statement [" + expression + "]", e);
                if (e.InnerException is RecognitionException)
                {
                    throw EPStatementSyntaxException.Convert((RecognitionException)e.InnerException, expression, walker);
                }
                else
                {
                    throw;
                }
            }
        }
    
        /// <summary>Parse expression using the rule the ParseRuleSelector instance supplies. </summary>
        /// <param name="expression">text to parse</param>
        /// <param name="parseRuleSelector">parse rule to select</param>
        /// <returns>AST - syntax tree</returns>
        /// <throws>EPException when the AST could not be parsed</throws>
        public static ITree Parse(String expression, ParseRuleSelector parseRuleSelector)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(".parse Parsing expr=" + expression);
            }
    
            ICharStream input;
            try
            {
                input = new NoCaseSensitiveStream(expression);
            }
            catch (IOException ex)
            {
                throw new EPException("IOException parsing expression '" + expression + '\'', ex);
            }
    
            EsperEPL2GrammarLexer lex = new EsperEPL2GrammarLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lex);
            EsperEPL2GrammarParser parser = new EsperEPL2GrammarParser(tokens);
            EsperEPL2GrammarParser.startEventPropertyRule_return r;
    
            ITree tree;
            try
            {
                tree = parseRuleSelector.Invoke(parser);
            }
            catch (RecognitionException ex)
            {
                log.Debug("Error parsing statement [" + expression + "]", ex);
                throw EPStatementSyntaxException.Convert(ex, expression, parser);
            }
            catch (Exception e)
            {
                log.Debug("Error parsing statement [" + expression + "]", e);
                if (e.InnerException is RecognitionException)
                {
                    throw EPStatementSyntaxException.Convert((RecognitionException)e.InnerException, expression, parser);
                }
                else
                {
                    throw;
                }
            }
    
            if (log.IsDebugEnabled)
            {
                log.Debug(".parse Dumping AST...");
                ASTUtil.DumpAST(tree);
            }
    
            return tree;
        }
    
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
