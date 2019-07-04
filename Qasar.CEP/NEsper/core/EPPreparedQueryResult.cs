///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.events;

namespace com.espertech.esper.core
{
    /// <summary>
    /// The result of executing a prepared query.
    /// </summary>
    public class EPPreparedQueryResult
    {
        private readonly EventType eventType;
        private readonly EventBean[] result;
    
        /// <summary>Ctor. </summary>
        /// <param name="eventType">is the type of event produced by the query</param>
        /// <param name="result">the result rows</param>
        public EPPreparedQueryResult(EventType eventType, EventBean[] result)
        {
            this.eventType = eventType;
            this.result = result;
        }

        /// <summary>Returs the event type representing the selected columns. </summary>
        /// <returns>metadata</returns>
        public EventType EventType
        {
            get { return eventType; }
        }

        /// <summary>Returns the query result. </summary>
        /// <returns>result rows</returns>
        public EventBean[] Result
        {
            get { return result; }
        }
    }
}
