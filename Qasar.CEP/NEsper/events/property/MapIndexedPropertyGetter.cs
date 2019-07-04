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
    /// Getter for a dynamic indexed property for maps.
    /// </summary>
	public class MapIndexedPropertyGetter : EventPropertyGetter
	{
	    private readonly int index;
	    private readonly String fieldName;

	    /// <summary>Ctor.</summary>
	    /// <param name="fieldName">property name</param>
	    /// <param name="index">index to get the element at</param>
	    public MapIndexedPropertyGetter(String fieldName, int index)
	    {
	        this.index = index;
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

            Array array = value as Array;
            if ( array == null )
            {
                return null;
            }

	        if (index >= array.Length)
	        {
	            return null;
	        }
	        return array.GetValue(index);
	    }

	    public bool IsExistsProperty(EventBean eventBean)
	    {
            Object underlying = eventBean.Underlying;

            DataMap map = underlying as DataMap;
            if (map == null)
            {
                return false;
            }
            
	        Object value = map.Get(fieldName);

            Array array = value as Array;
            if (array == null)
            {
                return false;
            }

	        if (index >= array.Length)
	        {
	            return false;
	        }

	        return true;
	    }
	}
} // End of namespace
