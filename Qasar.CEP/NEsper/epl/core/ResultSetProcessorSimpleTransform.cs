///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;

using com.espertech.esper.collection;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.core
{
    /// <summary>Method to transform an event based on the select expression. </summary>
    public class ResultSetProcessorSimpleTransform
    {
        private readonly ResultSetProcessorBaseSimple resultSetProcessor;
        private readonly EventBean[] newData;
    
        /// <summary>Ctor. </summary>
        /// <param name="resultSetProcessor">is applying the select expressions to the events for the transformation</param>
        public ResultSetProcessorSimpleTransform(ResultSetProcessorBaseSimple resultSetProcessor) {
            this.resultSetProcessor = resultSetProcessor;
            newData = new EventBean[1];
        }
    
        public EventBean Transform(EventBean @event)
        {
            newData[0] = @event;
            UniformPair<EventBean[]> pair = resultSetProcessor.ProcessViewResult(newData, null, true);
            return pair.First[0];
        }
    }
}
