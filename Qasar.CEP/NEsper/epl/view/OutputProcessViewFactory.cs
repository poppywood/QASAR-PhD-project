///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using com.espertech.esper.core;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;

namespace com.espertech.esper.epl.view
{
    /// <summary>Factory for output processing views. </summary>
    public class OutputProcessViewFactory
    {
        /// <summary>Creates an output processor view depending on the presence of output limiting requirements. </summary>
        /// <param name="resultSetProcessor">is the processing for select-clause and grouping</param>
        /// <param name="statementContext">is the statement-level services</param>
        /// <param name="internalEventRouter">service for routing events internally</param>
        /// <param name="statementSpec">the statement specification</param>
        /// <returns>output processing view</returns>
        /// <throws>ExprValidationException to indicate</throws>
        public static OutputProcessView MakeView(ResultSetProcessor resultSetProcessor,
                                                 StatementSpecCompiled statementSpec,
                                                 StatementContext statementContext,
                                                 InternalEventRouter internalEventRouter)
        {
            bool isRouted = false;
            if (statementSpec.InsertIntoDesc != null)
            {
                isRouted = true;
            }
    
            OutputStrategy outputStrategy;
            if ((statementSpec.InsertIntoDesc != null) || (statementSpec.SelectStreamSelectorEnum == SelectClauseStreamSelectorEnum.RSTREAM_ONLY))
            {
                bool isRouteRStream = false;
                if (statementSpec.InsertIntoDesc != null)
                {
                    isRouteRStream = !statementSpec.InsertIntoDesc.IsIStream;
                }
    
                outputStrategy = new OutputStrategyPostProcess(isRouted, isRouteRStream, statementSpec.SelectStreamSelectorEnum, internalEventRouter, statementContext.EpStatementHandle);
            }
            else
            {
                outputStrategy = new OutputStrategySimple();
            }
    
            // Do we need to enforce an output policy?
            int streamCount = statementSpec.StreamSpecs.Count;
            OutputLimitSpec outputLimitSpec = statementSpec.OutputLimitSpec;
    
            try
            {
                if (outputLimitSpec != null)
                {
                    if (outputLimitSpec.DisplayLimit == OutputLimitLimitType.SNAPSHOT)
                    {
                        return new OutputProcessViewSnapshot(resultSetProcessor, outputStrategy, isRouted, streamCount, outputLimitSpec, statementContext);
                    }
                    else
                    {
                        return new OutputProcessViewPolicy(resultSetProcessor, outputStrategy, isRouted, streamCount, outputLimitSpec, statementContext);
                    }
                }
                return new OutputProcessViewDirect(resultSetProcessor, outputStrategy, isRouted, statementContext.StatementResultService);
            }
            catch (Exception ex)
            {
                throw new ExprValidationException("Error in the output rate limiting clause: " + ex.Message, ex);
            }
        }
    }
}
