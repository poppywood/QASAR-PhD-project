///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.join
{
    /// <summary>
    /// Index lookup strategy into a poll-based cache result.
    /// </summary>
	public class HistoricalIndexLookupStrategyIndex : HistoricalIndexLookupStrategy
	{
	    private readonly EventPropertyGetter[] propertyGetters;

	    /// <summary>Ctor.</summary>
	    /// <param name="eventType">event type to expect for lookup</param>
	    /// <param name="properties">key properties</param>
	    public HistoricalIndexLookupStrategyIndex(EventType eventType, String[] properties)
	    {
            propertyGetters = new EventPropertyGetter[properties.Length];
	        for (int i = 0; i < properties.Length; i++)
	        {
	            propertyGetters[i] = eventType.GetGetter(properties[i]);

	            if (propertyGetters[i] == null)
	            {
	                throw new ArgumentException("Property named '" + properties[i] + "' is invalid for type " + eventType);
	            }
	        }
	    }

	    public IEnumerator<EventBean> Lookup(EventBean lookupEvent, EventTable indexTable)
	    {
	        // The table may not be indexed as the cache may not actively cache, in which case indexing doesn't makes sense
	        if (indexTable is PropertyIndexedEventTable)
	        {
	            PropertyIndexedEventTable index = (PropertyIndexedEventTable) indexTable;
	            Object[] keys = GetKeys(lookupEvent);

	            Set<EventBean> events = index.Lookup(keys);
	            if (events != null)
	            {
	                return events.GetEnumerator();
	            }
	            return null;
	        }

            return indexTable.GetEnumerator();
	    }

	    private Object[] GetKeys(EventBean @event)
	    {
	        return EventBeanUtility.GetPropertyArray(@event, propertyGetters);
	    }
	}
} // End of namespace
