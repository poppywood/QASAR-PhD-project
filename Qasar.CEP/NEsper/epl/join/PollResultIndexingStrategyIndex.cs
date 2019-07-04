///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.epl.join.table;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.join
{
    /// <summary>
    /// Strategy for building an index out of poll-results knowing the properties to base the index on.
    /// </summary>
	public class PollResultIndexingStrategyIndex : PollResultIndexingStrategy
	{
	    private readonly int streamNum;
	    private readonly EventType eventType;
        private readonly String[] propertyNames;

	    /// <summary>Ctor.</summary>
	    /// <param name="streamNum">is the stream number of the indexed stream</param>
	    /// <param name="eventType">is the event type of the indexed stream</param>
	    /// <param name="propertyNames">is the property names to be indexed</param>
	    public PollResultIndexingStrategyIndex(int streamNum, EventType eventType, String[] propertyNames)
	    {
	        this.streamNum = streamNum;
	        this.eventType = eventType;
	        this.propertyNames = propertyNames;
	    }

	    public EventTable Index(IList<EventBean> pollResult, bool isActiveCache)
	    {
	        if (!isActiveCache)
	        {
	            return new UnindexedEventTableList(pollResult);
	        }
	        PropertyIndexedEventTable table = new PropertyIndexedEventTable(streamNum, eventType, propertyNames);
	        table.Add(pollResult);
	        return table;
	    }
	}
} // End of namespace
