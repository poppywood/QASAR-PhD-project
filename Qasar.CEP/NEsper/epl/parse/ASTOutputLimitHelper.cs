///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using Antlr.Runtime.Tree;
using com.espertech.esper.epl.generated;
using com.espertech.esper.epl.spec;
using com.espertech.esper.type;

namespace com.espertech.esper.epl.parse
{
    /// <summary>Builds an output limit spec from an output limit AST node. </summary>
    public class ASTOutputLimitHelper
    {
        /// <summary>Build an output limit spec from the AST node supplied.  </summary>
        /// <param name="node">parse node</param>
        /// <returns>output limit spec</returns>
        public static OutputLimitSpec BuildOutputLimitSpec(ITree node)
        {
            int count = 0;
            ITree child = node.GetChild(count);
    
            // parse type
            OutputLimitLimitType displayLimit = OutputLimitLimitType.DEFAULT;
            if (child.Type == EsperEPL2GrammarParser.FIRST)
            {
                displayLimit = OutputLimitLimitType.FIRST;
                child = node.GetChild(++count);
            }
            else if (child.Type == EsperEPL2GrammarParser.LAST)
            {
                displayLimit = OutputLimitLimitType.LAST;
                child = node.GetChild(++count);
            }
            else if (child.Type == EsperEPL2GrammarParser.SNAPSHOT)
            {
                displayLimit = OutputLimitLimitType.SNAPSHOT;
                child = node.GetChild(++count);
            }
            else if (child.Type == EsperEPL2GrammarParser.ALL)
            {
                displayLimit = OutputLimitLimitType.ALL;
                child = node.GetChild(++count);
            }

            // next is a variable, or time period, or number
            String variableName = null;
            double rate = -1;
            if (child.Type == EsperEPL2GrammarParser.IDENT)
            {
                variableName = child.Text;
            }
            else if (child.Type == EsperEPL2GrammarParser.TIME_PERIOD)
            {
                TimePeriodParameter param = ASTParameterHelper.MakeTimePeriod(child, 0L);
                rate = param.NumSeconds;
            }
            else
            {
                rate = Double.Parse(child.Text);
            }
    
            switch (node.Type)
            {
                case EsperEPL2GrammarParser.EVENT_LIMIT_EXPR:
                    return new OutputLimitSpec(rate, variableName, OutputLimitRateType.EVENTS, displayLimit);
                case EsperEPL2GrammarParser.SEC_LIMIT_EXPR:
                case EsperEPL2GrammarParser.TIMEPERIOD_LIMIT_EXPR:
                    return new OutputLimitSpec(rate, variableName, OutputLimitRateType.TIME_SEC, displayLimit);
                case EsperEPL2GrammarParser.MIN_LIMIT_EXPR:
                    return new OutputLimitSpec(rate, variableName, OutputLimitRateType.TIME_MIN, displayLimit);
                default:
                    throw new ArgumentException("Node type " + node.Type + " not a recognized output limit type");
    		 }
    	 }
    
    }
}
