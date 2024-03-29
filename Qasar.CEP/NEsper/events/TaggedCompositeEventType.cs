///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.collection;
using com.espertech.esper.compat;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Interface for composite event type in which each property is itself an event.
    /// <para>
    /// For use with patterns in which pattern tags are properties in a result event and property values
    /// are the event itself that is matching in a pattern.
    /// </para>
    /// </summary>
	public interface TaggedCompositeEventType
	{
	    /// <summary>Returns the event types for each composing event.</summary>
	    /// <returns>map of tag name and event type</returns>
        Map<String, Pair<EventType, String>> TaggedEventTypes { get;}
	}
} // End of namespace
