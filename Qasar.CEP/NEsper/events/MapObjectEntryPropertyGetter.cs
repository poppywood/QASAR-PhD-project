///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using DataMap = com.espertech.esper.compat.Map<string,object>;

namespace com.espertech.esper.events
{
    /// <summary>
    /// A getter that works on ordinary events residing within a Map as an event property.
    /// </summary>
    public class MapObjectEntryPropertyGetter : EventPropertyGetter
    {
        private readonly string propertyMap;
        private readonly EventPropertyGetter mapEntryGetter;
        private readonly EventAdapterService eventAdapterService;
    
        /// <summary>Ctor. </summary>
        /// <param name="propertyMap">the property to look at</param>
        /// <param name="mapEntryGetter">the getter for the map entry</param>
        /// <param name="eventAdapterService">for producing wrappers to objects</param>
        public MapObjectEntryPropertyGetter(string propertyMap, EventPropertyGetter mapEntryGetter, EventAdapterService eventAdapterService) {
            this.propertyMap = propertyMap;
            this.mapEntryGetter = mapEntryGetter;
            this.eventAdapterService = eventAdapterService;
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
            object underlying = eventBean.Underlying;
    
            // The underlying is expected to be a map
            if (!(underlying is DataMap))
            {
                throw new PropertyAccessException(
                    "Mismatched property getter to event bean type, " +
                    "the underlying data object is not of type DataMap");
            }
    
            DataMap map = (DataMap) underlying;
    
            // If the map does not contain the key, this is allowed and represented as null
            object value = map.Get(propertyMap);
    
            if (value == null)
            {
                return null;
            }
    
            // Object within the map
            EventBean @event = eventAdapterService.AdapterForBean(value);
            return mapEntryGetter.GetValue(@event);
        }
    
        public bool IsExistsProperty(EventBean eventBean)
        {
            return true; // Property exists as the property is not dynamic (unchecked)
        }
    }
    
    
}
