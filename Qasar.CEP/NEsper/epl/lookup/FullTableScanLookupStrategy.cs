///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.compat;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.lookup
{
    /// <summary>
    /// Lookup on an unindexed table returning the full table as matching events.
    /// </summary>
	public class FullTableScanLookupStrategy : TableLookupStrategy
	{
	    private readonly UnindexedEventTable eventIndex;

	    /// <summary>Ctor.</summary>
	    /// <param name="eventIndex">table to use</param>
	    public FullTableScanLookupStrategy(UnindexedEventTable eventIndex)
	    {
	        this.eventIndex = eventIndex;
	    }

	    public Set<EventBean> Lookup(EventBean[] eventPerStream)
	    {
	        Set<EventBean> result = eventIndex.EventSet;
	        if (result.IsEmpty)
	        {
	            return null;
	        }
	        return result;
	    }

	    /// <summary>Returns the associated table.</summary>
	    /// <returns>table for lookup.</returns>
	    public UnindexedEventTable EventIndex
	    {
            get { return eventIndex; }
	    }
	}
} // End of namespace
