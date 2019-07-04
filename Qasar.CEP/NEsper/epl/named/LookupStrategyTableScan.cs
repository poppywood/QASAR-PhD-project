///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.epl.expression;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// Determine events to be deleted from a named window using the where-clause and full table scan.
    /// </summary>
	public class LookupStrategyTableScan : LookupStrategy
	{
	    private readonly ExprNode joinExpr;
	    private readonly EventBean[] eventsPerStream;
	    private readonly IEnumerable<EventBean> iterableNamedWindow;

	    /// <summary>Ctor.</summary>
	    /// <param name="joinExpr">is the where clause</param>
	    /// <param name="iterable">is the named window's data window iterator</param>
        public LookupStrategyTableScan(ExprNode joinExpr, IEnumerable<EventBean> iterable)
	    {
	        this.joinExpr = joinExpr;
	        this.eventsPerStream = new EventBean[2];
	        this.iterableNamedWindow = iterable;
	    }

	    public EventBean[] Lookup(EventBean[] newData)
	    {
	        Set<EventBean> removeEvents = null;

            foreach( EventBean eventBean in iterableNamedWindow )
	        {
	            eventsPerStream[0] = eventBean;   // next named window event

                foreach (EventBean aNewData in newData)
                {
                    eventsPerStream[1] = aNewData; // Stream 1 events are the originating events (on-delete events)

                    bool? result = (bool?) joinExpr.Evaluate(eventsPerStream, true);
                    if (result ?? false)
                    {
                        if (removeEvents == null)
                        {
                            removeEvents = new LinkedHashSet<EventBean>();
                        }
                        removeEvents.Add(eventsPerStream[0]);
                    }
                }
	        }

	        if (removeEvents == null)
	        {
	            return null;
	        }

	        return removeEvents.ToArray();
	    }
	}
} // End of namespace
