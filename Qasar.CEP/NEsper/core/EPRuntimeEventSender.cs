///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.events;

namespace com.espertech.esper.core
{
    /// <summary>
    /// For use by <see cref="com.espertech.esper.client.EventSender"/> for direct feed of wrapped events for
    /// processing.
    /// </summary>
    public interface EPRuntimeEventSender
    {
        /// <summary>
        /// Equivalent to the sendEvent method of EPRuntime, for use to process an known event.
        /// </summary>
        /// <param name="eventBean">the event object wrapped by an event bean providing the event metadata</param>
        void ProcessWrappedEvent(EventBean eventBean);   
    }
}
