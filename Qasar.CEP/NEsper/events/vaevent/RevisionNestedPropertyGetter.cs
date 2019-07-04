///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.events;

namespace com.espertech.esper.events.vaevent
{
    /// <summary>
    /// A getter that works on object events residing within a Map as an event property.
    /// </summary>
    public class RevisionNestedPropertyGetter : EventPropertyGetter
    {
        private readonly EventPropertyGetter revisionGetter;
        private readonly EventPropertyGetter nestedGetter;
        private readonly EventAdapterService eventAdapterService;
    
        /// <summary>Ctor. </summary>
        /// <param name="revisionGetter">getter for revision value</param>
        /// <param name="nestedGetter">getter to apply to revision value</param>
        /// <param name="eventAdapterService">for handling object types</param>
        public RevisionNestedPropertyGetter(EventPropertyGetter revisionGetter, EventPropertyGetter nestedGetter, EventAdapterService eventAdapterService) {
            this.revisionGetter = revisionGetter;
            this.eventAdapterService = eventAdapterService;
            this.nestedGetter = nestedGetter;
        }


        /// <summary> Return the value for the property in the event object specified when the instance was obtained.
        /// Useful for fast access to event properties. Throws a PropertyAccessException if the getter instance
        /// doesn't match the EventType it was obtained from, and to indicate other property access problems.
        /// </summary>
        /// <param name="eventBean">is the event to get the value of a property from
        /// </param>
        /// <returns> value of property in event
        /// </returns>
        /// <throws>  PropertyAccessException to indicate that property access failed </throws>
        public object GetValue(EventBean eventBean)
        {
            Object result = revisionGetter.GetValue(eventBean);
            if (result == null)
            {
                return result;
            }
    
            // Object within the map
            EventBean @event = eventAdapterService.AdapterForBean(result);
            return nestedGetter.GetValue(@event);
        }
    
        public bool IsExistsProperty(EventBean eventBean)
        {
            return true; // Property exists as the property is not dynamic (unchecked)
        }
    }
}
