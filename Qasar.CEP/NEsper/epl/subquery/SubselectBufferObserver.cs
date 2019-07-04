///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.collection;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.view.internals;

namespace com.espertech.esper.epl.subquery
{
    /// <summary>
    /// Observer to a buffer that is filled by a subselect view when it posts events,
    /// to be added and removed from indexes.
    /// </summary>
	public class SubselectBufferObserver : BufferObserver
	{
	    private readonly EventTable eventIndex;

	    /// <summary>Ctor.</summary>
	    /// <param name="eventIndex">index to update</param>
	    public SubselectBufferObserver(EventTable eventIndex) {
	        this.eventIndex = eventIndex;
	    }

	    public void NewData(int streamId, FlushedEventBuffer newEventBuffer, FlushedEventBuffer oldEventBuffer)
	    {
	        eventIndex.Add(newEventBuffer.GetAndFlush());
	        eventIndex.Remove(oldEventBuffer.GetAndFlush());
	    }
	}
} // End of namespace
