///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.events;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// A getter that interrogates a given property in a map which may itself contain
    /// nested maps or indexed entries.
    /// </summary>
	public class MapPropertyGetter : EventPropertyGetter
	{
	    private readonly String propertyMap;
	    private readonly EventPropertyGetter getter;

	    /// <summary>Ctor.</summary>
	    /// <param name="propertyMap">is the property returning the map to interrogate</param>
	    /// <param name="getter">
	    /// is the getter to use to interrogate the property in the map
	    /// </param>
	    public MapPropertyGetter(String propertyMap, EventPropertyGetter getter)
	    {
	        if (getter == null)
	        {
	            throw new ArgumentException("Getter is a required parameter");
	        }
	        this.propertyMap = propertyMap;
	        this.getter = getter;
	    }

	    public Object GetValue(EventBean eventBean)
	    {
	        // The map contains a map-type property, that we are querying, named valueTop.
	        // (A map could also contain an object-type property handled as an object).
	        Object result = eventBean.Underlying;
	        DataMap map = result as DataMap;
            if ( map == null )
	        {
	            return null;
	        }

	        Object valueTopObj = map.Get(propertyMap);
	        DataMap valueTop = valueTopObj as DataMap;
            if ( valueTop == null )
	        {
	            return null;
	        }

	        // Obtains for the inner map the property value
	        EventBean @event = new MapEventBean(valueTop, null);
	        return getter.GetValue(@event);
	    }

	    public bool IsExistsProperty(EventBean eventBean)
	    {
	        Object result = eventBean.Underlying;
            DataMap map = result as DataMap;
            if ( map == null )
            {
                return false;
            }

	        Object valueTopObj = map.Get(propertyMap);
            DataMap valueTop = valueTopObj as DataMap;
            if ( valueTop == null )
	        {
	            return false;
	        }

	        // Obtains for the inner map the property value
	        EventBean @event = new MapEventBean(valueTop, null);
	        return getter.IsExistsProperty(@event);
	    }
	}
} // End of namespace
