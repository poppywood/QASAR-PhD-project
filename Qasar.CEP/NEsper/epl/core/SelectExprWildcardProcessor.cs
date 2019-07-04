///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.epl.expression;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.core
{
    /// <summary>Processor for select-clause expressions that handles wildcards for single streams with no insert-into. </summary>
    public class SelectExprWildcardProcessor : SelectExprProcessor
    {
        private readonly EventType eventType;
    
        /// <summary>Ctor. </summary>
        /// <param name="eventType">is the type of event this processor produces</param>
        /// <throws>com.espertech.esper.epl.expression.ExprValidationException if the expression validation failed</throws>
        public SelectExprWildcardProcessor(EventType eventType)
        {
            this.eventType = eventType;
        }
    
        public EventBean Process(EventBean[] eventsPerStream, bool isNewData, bool isSynthesize)
        {
            return eventsPerStream[0];
        }
    
        public EventType ResultEventType
        {
            get { return eventType; }
        }
    }
}
