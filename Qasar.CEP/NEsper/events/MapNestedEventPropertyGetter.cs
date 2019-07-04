///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.events
{
    /// <summary>A getter for querying Map-within-Map event properties. </summary>
    public class MapNestedEventPropertyGetter : EventPropertyGetter
    {
        private readonly Stack<String> accessPath;
    
        /// <summary>Ctor. </summary>
        /// <param name="accessPath">is the properties to follow down into nested maps.</param>
        public MapNestedEventPropertyGetter(Stack<String> accessPath) {
            this.accessPath = new Stack<String>();
            foreach( string item in accessPath ) {
                this.accessPath.Push(item);
            }
        }
    
        /// <summary>Ctor. </summary>
        /// <param name="accessPath">is the properties to follow down into nested maps.</param>
        /// <param name="leaf">the last property we are looking for in the readonly map</param>
        public MapNestedEventPropertyGetter(Stack<String> accessPath, String leaf) {
            this.accessPath = new Stack<String>();
            foreach( string item in accessPath ) {
                this.accessPath.Push(item);
            }
            this.accessPath.Push(leaf);
        }
    
        public Object GetValue(EventBean obj)
        {
            // The underlying is expected to be a map
            if (!(obj.Underlying is DataMap))
            {
                throw new PropertyAccessException(
                    "Mismatched property getter to event bean type, " +
                    "the underlying data object is not of type DataMap");
            }

            DataMap map = (DataMap)obj.Underlying;
            Object value = null;
    
            // We are dealing with a nested map structure
            foreach (String next in accessPath)
            {
                value = map.Get(next);
                if (value == null)
                {
                    return null;
                }

                if (!(value is DataMap))
                {
                    return value;
                }

                map = (DataMap)value;
            }
    
            // If the map does not contain the key, this is allowed and represented as null
            return value;
        }
    
        public bool IsExistsProperty(EventBean eventBean)
        {
            return true; // Property exists as the property is not dynamic (unchecked)
        }
    }
}
