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

namespace com.espertech.esper.collection
{
	/// <summary>
	/// Buffer for events - accumulates events until flushed.
	/// </summary>

    public class FlushedEventBuffer
	{
	    private List<EventBean[]> remainEvents = new List<EventBean[]>();

	    /// <summary>Add an event array to buffer.</summary>
	    /// <param name="events">to add</param>
	    public void Add(EventBean[] events)
	    {
	        if (events != null)
	        {
	            remainEvents.Add(events);
	        }
	    }

	    /// <summary>
	    /// Get the events currently buffered. Returns null if the buffer is empty. Flushes the buffer.
	    /// </summary>
	    /// <returns>array of events in buffer or null if empty</returns>
	    public EventBean[] GetAndFlush()
	    {
	        EventBean[] flattened = EventBeanUtility.Flatten(remainEvents);
	        remainEvents.Clear();
	        return flattened;
	    }

	    /// <summary>Empty buffer.</summary>
	    public void Flush()
	    {
	        remainEvents.Clear();
	    }
	}
} // End of namespace
