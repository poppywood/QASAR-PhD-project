///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.epl.join.table;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.join
{
    /// <summary>
    /// Strategy of indexing that simply builds an unindexed table of poll results.
    /// 
    /// For use when caching is disabled or when no proper index could be build because no where-clause or on-clause exists or
    /// these clauses don't yield indexable columns on analysis.
    /// </summary>
	public class PollResultIndexingStrategyNoIndex : PollResultIndexingStrategy
	{
	    public EventTable Index(IList<EventBean> pollResult, bool isActiveCache)
	    {
	        return new UnindexedEventTableList(pollResult);
	    }
	}
} // End of namespace
