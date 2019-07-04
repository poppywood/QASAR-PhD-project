///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;

using Antlr.Runtime;
using Antlr.Runtime.Tree;

using log4net;

namespace com.espertech.esper.antlr
{
    /// <summary>Utility class for AST node handling.</summary>
	public class ASTUtil
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    private const String PROPERTY_ENABLED_AST_DUMP = "ENABLE_AST_DUMP";

        /// <summary>
        /// Dump the AST node to system.out.
        /// </summary>
        /// <param name="ast">to dump</param>
	    public static void DumpAST(ITree ast)
	    {
	        if (Environment.GetEnvironmentVariable(PROPERTY_ENABLED_AST_DUMP) != null)
	        {
	            StringWriter writer = new StringWriter();

	            RenderNode(new char[0], ast, writer);
	            DumpAST(writer, ast, 2);

	            log.Info(".dumpAST ANTLR Tree dump follows...\n" + writer);
	        }
	    }

	    private static void DumpAST(TextWriter writer, ITree ast, int ident)
	    {
	        char[] identChars = new char[ident];
            for (int ii = 0; ii < identChars.Length ; ii++)
                identChars[ii] = ' ';

	        if (ast == null)
	        {
	            RenderNode(identChars, ast, writer);
	            return;
	        }
	        for (int i = 0; i < ast.ChildCount; i++)
	        {
	            ITree node = ast.GetChild(i);
	            if (node == null)
	            {
	                throw new NullReferenceException("Null AST node");
	            }
                RenderNode(identChars, node, writer);
                DumpAST(writer, node, ident + 2);
	        }
	    }

	    /// <summary>Print the token stream to the logger.</summary>
	    /// <param name="tokens">to print</param>
	    public static void PrintTokens(CommonTokenStream tokens)
	    {
	        if (log.IsDebugEnabled)
	        {
	            IList tokenList = tokens.GetTokens();

	            StringWriter writer = new StringWriter();
	            for (int i = 0; i < tokens.Size(); i++)
	            {
	                IToken t = (IToken) tokenList[i];
	                String text = t.Text;
	                if (text.Trim().Length == 0)
	                {
                        writer.Write("'" + text + "'");
	                }
	                else
	                {
                        writer.Write(text);
	                }
                    writer.Write('[');
                    writer.Write(t.GetType());
                    writer.Write(']');
                    writer.Write(" ");
	            }
	            writer.WriteLine();
	            log.Debug("Tokens: " + writer);
	        }
	    }

        private static void RenderNode(char[] ident, ITree node, TextWriter writer)
	    {
	        writer.Write(ident);
	        if (node == null)
	        {
	            writer.Write("NULL NODE");
	        }
	        else
	        {
                writer.Write(node.Text);
                writer.Write(" [");
                writer.Write(node.GetType());
                writer.Write("]");
	        }
            writer.WriteLine();
	    }

	}
} // End of namespace
