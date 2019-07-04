///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.events
{
    /// <summary>
    /// A getter that works on EventBean events residing within a Map as an event property.
    /// </summary>
    public class MapEventBeanEntryPropertyGetter : EventPropertyGetter
    {
    
        private readonly String propertyMap;
        private readonly EventPropertyGetter eventBeanEntryGetter;
    
        /// <summary>Ctor. </summary>
        /// <param name="propertyMap">the property to look at</param>
        /// <param name="eventBeanEntryGetter">the getter for the map entry</param>
        public MapEventBeanEntryPropertyGetter(String propertyMap, EventPropertyGetter eventBeanEntryGetter) {
            this.propertyMap = propertyMap;
            this.eventBeanEntryGetter = eventBeanEntryGetter;
        }
    
        public Object GetValue(EventBean obj)
        {
            Object underlying = obj.Underlying;
    
            // The underlying is expected to be a map
            DataMap map = underlying as DataMap;
            if ( map == null )
            {
                throw new PropertyAccessException(
                    "Mismatched property getter to event bean type, " +
                    "the underlying data object is not of type DataMap");
            }
    
            // If the map does not contain the key, this is allowed and represented as null
            Object value = map.Get(propertyMap);
    
            if (value == null)
            {
                return null;
            }
    
            // Object within the map
            EventBean _event = (EventBean) value;
            return eventBeanEntryGetter.GetValue(_event);
        }
    
        public bool IsExistsProperty(EventBean eventBean)
        {
            return true; // Property exists as the property is not dynamic (unchecked)
        }
    }
}
