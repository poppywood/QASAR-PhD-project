///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;

using com.espertech.esper.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.core
{
    /// <summary>A select expression processor that check what type of result (synthetic and natural) event is expected and produces. </summary>
    public class SelectExprResultProcessor : SelectExprProcessor
    {
        private readonly StatementResultService statementResultService;
        private readonly SelectExprProcessor syntheticProcessor;
        private readonly BindProcessor bindProcessor;
    
        /// <summary>Ctor. </summary>
        /// <param name="statementResultService">for awareness of listeners and subscribers handles output results</param>
        /// <param name="syntheticProcessor">is the processor generating synthetic events according to the select clause</param>
        /// <param name="bindProcessor">for generating natural object column results</param>
        /// <throws>ExprValidationException if the validation failed</throws>
        public SelectExprResultProcessor(StatementResultService statementResultService,
                                         SelectExprProcessor syntheticProcessor,
                                         BindProcessor bindProcessor)
        {
            this.statementResultService = statementResultService;
            this.syntheticProcessor = syntheticProcessor;
            this.bindProcessor = bindProcessor;
        }
    
        public EventType ResultEventType
        {
            get { return syntheticProcessor.ResultEventType; }
        }
    
        public EventBean Process(EventBean[] eventsPerStream, bool isNewData, bool isSynthesize)
        {
            if ((isSynthesize) && (!statementResultService.IsMakeNatural))
            {
                return syntheticProcessor.Process(eventsPerStream, isNewData, isSynthesize);
            }
    
            EventBean syntheticEvent = null;
            EventType syntheticEventType = null;
            if (statementResultService.IsMakeSynthetic || isSynthesize)
            {
                syntheticEvent = syntheticProcessor.Process(eventsPerStream, isNewData, isSynthesize);
    
                if (!statementResultService.IsMakeNatural)
                {
                    return syntheticEvent;
                }
    
                syntheticEventType = syntheticProcessor.ResultEventType;
            }
    
            if (!statementResultService.IsMakeNatural)
            {
                return null; // neither synthetic nor natural required, be cheap and generate no output event
            }
    
            Object[] parameters = bindProcessor.Process(eventsPerStream, isNewData);
            return new NaturalEventBean(syntheticEventType, parameters, syntheticEvent);
        }
    }
}
