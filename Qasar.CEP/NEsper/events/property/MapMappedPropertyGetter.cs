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
    /// Getter for a dynamic mappeds property for maps.
    /// </summary>
	public class MapMappedPropertyGetter : EventPropertyGetter
	{
	    private readonly String key;
	    private readonly String fieldName;

	    /// <summary>Ctor.</summary>
	    /// <param name="fieldName">property name</param>
	    /// <param name="key">get the element at</param>
	    public MapMappedPropertyGetter(String fieldName, String key)
	    {
	        this.key = key;
	        this.fieldName = fieldName;
	    }

	    public Object GetValue(EventBean eventBean)
	    {
	        Object underlying = eventBean.Underlying;

	        DataMap map = underlying as DataMap;
            if ( map == null )
            {
                return null;
            }

            Object value = map.Get(fieldName);
	        if (value == null)
	        {
	            return null;
	        }

	        DataMap innerMap = value as DataMap;
            if ( innerMap == null )
            {
                return null;
            }

	        return innerMap.Get(key);
	    }

	    public bool IsExistsProperty(EventBean eventBean)
	    {
	        Object underlying = eventBean.Underlying;

	        DataMap map = underlying as DataMap;
            if ( map == null )
            {
                return false;
            }

	        Object value = map.Get(fieldName);
	        if (value == null)
	        {
	            return false;
	        }

            DataMap innerMap = value as DataMap;
            if ( innerMap == null )
            {
                return false;
            }

	        return innerMap.ContainsKey(key);
	    }
	}
} // End of namespace
