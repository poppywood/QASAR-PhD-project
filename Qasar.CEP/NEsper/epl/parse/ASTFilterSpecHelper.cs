///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Text;

using Antlr.Runtime.Tree;
using com.espertech.esper.compat;
using com.espertech.esper.epl.generated;

using log4net;

namespace com.espertech.esper.epl.parse
{
    /// <summary>Builds a filter specification from filter AST nodes. </summary>
    public class ASTFilterSpecHelper
    {
        /// <summary>Return the generated property name that is defined by the AST child node and it's siblings. </summary>
        /// <param name="parentNode">the AST node to consider as the parent for the child nodes to look at</param>
        /// <param name="startIndex">the index of the child node to start looking at</param>
        /// <returns>property name, ie. indexed[1] or Mapped('key') or nested.nested or a combination or just 'simple'.</returns>
        public static String GetPropertyName(ITree parentNode, int startIndex)
        {
            StringBuilder buffer = new StringBuilder();
            String delimiter = "";

            int childIndex = startIndex;
            while (childIndex < parentNode.ChildCount)
            {
                ITree child = parentNode.GetChild(childIndex);
                buffer.Append(delimiter);

                switch (child.Type) {
                    case EsperEPL2GrammarParser.EVENT_PROP_SIMPLE:
                        buffer.Append(EscapeDot(child.GetChild(0).Text));
                        break;
                    case EsperEPL2GrammarParser.EVENT_PROP_MAPPED:
                        buffer.Append(EscapeDot(child.GetChild(0).Text));
                        buffer.Append('(');
                        buffer.Append(child.GetChild(1).Text);
                        buffer.Append(')');
                        break;
                    case EsperEPL2GrammarParser.EVENT_PROP_INDEXED:
                        buffer.Append(EscapeDot(child.GetChild(0).Text));
                        buffer.Append('[');
                        buffer.Append(child.GetChild(1).Text);
                        buffer.Append(']');
                        break;
                    case EsperEPL2GrammarParser.EVENT_PROP_DYNAMIC_SIMPLE:
                        buffer.Append(EscapeDot(child.GetChild(0).Text));
                        buffer.Append('?');
                        break;
                    case EsperEPL2GrammarParser.EVENT_PROP_DYNAMIC_MAPPED:
                        buffer.Append(EscapeDot(child.GetChild(0).Text));
                        buffer.Append('(');
                        buffer.Append(child.GetChild(1).Text);
                        buffer.Append(')');
                        buffer.Append('?');
                        break;
                    case EsperEPL2GrammarParser.EVENT_PROP_DYNAMIC_INDEXED:
                        buffer.Append(EscapeDot(child.GetChild(0).Text));
                        buffer.Append('[');
                        buffer.Append(child.GetChild(1).Text);
                        buffer.Append(']');
                        buffer.Append('?');
                        break;
                    default:
                        throw new IllegalStateException("Event property AST node not recognized, type=" + child.Type);
                }

                delimiter = ".";
                childIndex++;
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Escape all unescape dot characters in the text (identifier only) passed in.
        /// </summary>
        /// <param name="identifierToEscape">text to escape</param>
        /// <returns>text where dots are escaped</returns>
        public static String EscapeDot(String identifierToEscape)
        {
            int indexof = identifierToEscape.IndexOf(".");
            if (indexof == -1)
            {
                return identifierToEscape;
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < identifierToEscape.Length; i++)
            {
                char c = identifierToEscape[i];
                if (c != '.')
                {
                    builder.Append(c);
                    continue;
                }

                if (i > 0)
                {
                    if (identifierToEscape[i - 1] == '\\')
                    {
                        builder.Append('.');
                        continue;
                    }
                }

                builder.Append('\\');
                builder.Append('.');
            }

            return builder.ToString();
        }

        /// <summary>Find the index of an unescaped dot (.) character, or return -1 if none found.</summary>
        /// <param name="identifier">text to find an un-escaped dot character</param>
        /// <returns>index of first unescaped dot</returns>
        public static int UnescapedIndexOfDot(String identifier)
        {
            int indexof = identifier.IndexOf(".");
            if (indexof == -1)
            {
                return -1;
            }

            for (int i = 0; i < identifier.Length; i++)
            {
                char c = identifier[i];
                if (c != '.')
                {
                    continue;
                }

                if (i > 0)
                {
                    if (identifier[i - 1] == '\\')
                    {
                        continue;
                    }
                }

                return i;
            }

            return -1;
        }

        /// <summary>Un-Escape all escaped dot characters in the text (identifier only) passed in.</summary>
        /// <param name="identifierToUnescape">text to un-escape</param>
        /// <returns>string</returns>
        public static String UnescapeDot(String identifierToUnescape)
        {
            int indexof = identifierToUnescape.IndexOf(".");
            if (indexof == -1)
            {
                return identifierToUnescape;
            }
            indexof = identifierToUnescape.IndexOf("\\");
            if (indexof == -1)
            {
                return identifierToUnescape;
            }

            StringBuilder builder = new StringBuilder();
            int index = -1;
            int max = identifierToUnescape.Length - 1;
            do
            {
                index++;
                char c = identifierToUnescape[index];
                if (c != '\\')
                {
                    builder.Append(c);
                    continue;
                }
                if (index < identifierToUnescape.Length - 1)
                {
                    if (identifierToUnescape[index + 1] == '.')
                    {
                        builder.Append('.');
                        index++;
                    }
                }
            }
            while (index < max);

            return builder.ToString();
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
