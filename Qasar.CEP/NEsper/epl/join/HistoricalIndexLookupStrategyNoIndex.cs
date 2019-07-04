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
    /// Full table scan strategy for a poll-based cache result.
    /// </summary>
	public class HistoricalIndexLookupStrategyNoIndex : HistoricalIndexLookupStrategy
	{
	    public IEnumerator<EventBean> Lookup(EventBean lookupEvent, EventTable index)
	    {
	        return index.GetEnumerator();
	    }
	}
} // End of namespace
