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
    /// A getter for use with Map-based events simply returns the value for the key.
    /// </summary>
    public class MapEventPropertyGetter : EventPropertyGetter 
    {
        private readonly String propertyName;
    
        /// <summary>Ctor. </summary>
        /// <param name="propertyName">property to get</param>
        public MapEventPropertyGetter(String propertyName) {
            this.propertyName = propertyName;
        }
    
        public Object GetValue(EventBean obj)
        {
            DataMap map = obj.Underlying as DataMap;

            // The underlying is expected to be a map
            if ( map == null )
            {
                throw new PropertyAccessException(
                    "Mismatched property getter to event bean type, " +
                    "the underlying data object is not of type DataMap");
            }
    
            // If the map does not contain the key, this is allowed and represented as null
            return map.Get(propertyName);
        }
    
        public bool IsExistsProperty(EventBean eventBean)
        {
            return true; // Property exists as the property is not dynamic (unchecked)
        }
    }
}
