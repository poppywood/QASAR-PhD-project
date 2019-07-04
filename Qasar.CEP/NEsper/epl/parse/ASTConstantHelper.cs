///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using Antlr.Runtime.Tree;

using com.espertech.esper.type;
using com.espertech.esper.epl.generated;

namespace com.espertech.esper.epl.parse
{
	/// <summary>
	/// Parses constant strings and returns the constant Object.
	/// </summary>
	public class ASTConstantHelper
	{
		/// <summary> Parse the AST constant node and return Object value.</summary>
		/// <param name="node">parse node for which to parse the string value
		/// </param>
		/// <returns> value matching AST node type
		/// </returns>
		public static Object Parse(ITree node)
		{
			switch (node.Type)
			{
				case EsperEPL2GrammarParser.NUM_INT:
                    return IntValue.ParseString(node.Text);
                case EsperEPL2GrammarParser.INT_TYPE:
                    return IntValue.ParseString(node.Text);
                case EsperEPL2GrammarParser.LONG_TYPE:
                    return LongValue.ParseString(node.Text);
                case EsperEPL2GrammarParser.BOOL_TYPE:
                    return BoolValue.ParseString(node.Text);
                case EsperEPL2GrammarParser.FLOAT_TYPE:
                    return FloatValue.ParseString(node.Text);
                case EsperEPL2GrammarParser.DOUBLE_TYPE:
                    return DoubleValue.ParseString(node.Text);
                case EsperEPL2GrammarParser.STRING_TYPE:
                    return StringValue.ParseString(node.Text);
                case EsperEPL2GrammarParser.NULL_TYPE:
                    return null;
				default: 
					throw new ArgumentException("Unexpected constant of type " + node.Type + " encountered, this class supports only primitgive types");
			}
		}
		
	    private static Object ParseIntLong(String arg)
	    {
	        // try to parse as an int first, else try to parse as a long
	        try
	        {
	            return Int32.Parse(arg);
	        }
	        catch (FormatException)
	        {
                return Int64.Parse(arg);
	        }
	    }
	}
}