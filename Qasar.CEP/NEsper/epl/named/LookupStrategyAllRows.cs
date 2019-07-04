///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.events;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// Deletes from a named window all events simply using the named window's data window iterator.
    /// </summary>
	public class LookupStrategyAllRows : LookupStrategy
	{
	    private readonly IEnumerable<EventBean> source;

	    /// <summary>Ctor.</summary>
	    /// <param name="source">iterator of the data window under the named window</param>
        public LookupStrategyAllRows(IEnumerable<EventBean> source)
	    {
	        this.source = source;
	    }

	    public EventBean[] Lookup(EventBean[] newData)
	    {
	        List<EventBean> events = new List<EventBean>();
	        events.AddRange(source);
	        return events.ToArray();
	    }
	}
} // End of namespace
