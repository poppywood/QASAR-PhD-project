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
using com.espertech.esper.epl.lookup;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// Uses an index to determine event to be deleted or selected from a named window.
    /// </summary>
	public class LookupStrategyIndexed : LookupStrategy
	{
	    private readonly ExprNode joinExpr;
	    private readonly EventBean[] eventsPerStream;
        private readonly TableLookupStrategy tableLookupStrategy;

	    /// <summary>Ctor.</summary>
	    /// <param name="joinExpr">the validated where clause of the on-delete</param>
	    /// <param name="tableLookupStrategy">
	    /// the strategy for looking up in an index the matching events using correlation
	    /// </param>
	    public LookupStrategyIndexed(ExprNode joinExpr, TableLookupStrategy tableLookupStrategy)
	    {
	        this.joinExpr = joinExpr;
	        this.eventsPerStream = new EventBean[2];
	        this.tableLookupStrategy = tableLookupStrategy;
	    }

	    public EventBean[] Lookup(EventBean[] newData)
	    {
	        Set<EventBean> removeEvents = null;

	        // For every new event (usually 1)
	        foreach (EventBean newEvent in newData)
	        {
	            eventsPerStream[1] = newEvent;

	            // use index to find match
	            Set<EventBean> matches = tableLookupStrategy.Lookup(eventsPerStream);
	            if ((matches == null) || (matches.IsEmpty))
	            {
	                continue;
	            }

	            // evaluate expression
                foreach( EventBean eventBean in matches )
	            {
	                eventsPerStream[0] = eventBean;   // next named window event

	                foreach (EventBean aNewData in newData)
	                {
	                    eventsPerStream[1] = aNewData;    // Stream 1 events are the originating events (on-delete events)

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
	        }

	        if (removeEvents == null)
	        {
	            return null;
	        }

	        return removeEvents.ToArray();
	    }
	}
} // End of namespace
