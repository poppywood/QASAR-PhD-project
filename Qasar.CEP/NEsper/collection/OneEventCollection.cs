///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
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
    /// Simple collection that exposes a limited add-and-get interface and
    /// that is optimized towards holding a single event, but can hold multiple
    /// events. If more then one event is added, the class allocates a linked
    /// list for additional events.
    /// </summary>
    public class OneEventCollection
    {
        private EventBean firstEvent;
        private LinkedList<EventBean> additionalEvents;
    
        /// <summary>Add an event to the collection. </summary>
        /// <param name="event">is the event to add</param>
        public void Add(EventBean @event)
        {
            if (@event == null)
            {
                throw new ArgumentException("Null event not allowed");
            }
            
            if (firstEvent == null)
            {
                firstEvent = @event;
                return;
            }
            
            if (additionalEvents == null)
            {
                additionalEvents = new LinkedList<EventBean>();
            }
            additionalEvents.AddLast(@event);
        }
    
        /// <summary>Returns true if the collection is empty. </summary>
        /// <returns>true if empty, false if not</returns>
        public bool IsEmpty
        {
            get { return firstEvent == null; }
        }
    
        /// <summary>Returns an array holding the collected events. </summary>
        /// <returns>event array</returns>
        public EventBean[] ToArray()
        {
            if (firstEvent == null)
            {
                return new EventBean[0];
            }
    
            if (additionalEvents == null)
            {
                return new EventBean[] {firstEvent};
            }
    
            EventBean[] events = new EventBean[1 + additionalEvents.Count];
            events[0] = firstEvent;
    
            int count = 1;
            foreach (EventBean @event in additionalEvents)
            {
                events[count] = @event;
                count++;
            }
    
            return events;
        }
    }
}
