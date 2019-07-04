///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.events;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// A deletion strategy is for use with named window in on-delete statements and encapsulates
    /// the strategy for resolving one or more events arriving in the on-clause of an on-delete statement
    /// to one or more events to be deleted from the named window.
    /// </summary>
	public interface LookupStrategy
	{
	    /// <summary>Determines the events to be deleted from a named window.</summary>
	    /// <param name="newData">is the correlation events</param>
	    /// <returns>the events to delete from the named window</returns>
	    EventBean[] Lookup(EventBean[] newData);
	}
} // End of namespace
