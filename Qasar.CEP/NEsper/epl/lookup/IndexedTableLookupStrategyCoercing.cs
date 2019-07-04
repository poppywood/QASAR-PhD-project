///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.epl.join.table;
using com.espertech.esper.events;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.lookup
{
    /// <summary>
    /// Index lookup strategy that coerces the key values before performing a lookup.
    /// </summary>
	public class IndexedTableLookupStrategyCoercing : IndexedTableLookupStrategy
	{
	    private readonly Type[] coercionTypes;

	    /// <summary>Ctor.</summary>
	    /// <param name="eventTypes">is the event type per stream</param>
	    /// <param name="streamNumbers">is the stream numbers to get keys from</param>
	    /// <param name="properties">is the property names</param>
	    /// <param name="index">is the table to look into</param>
	    /// <param name="coercionTypes">is the types to coerce to before lookup</param>
	    public IndexedTableLookupStrategyCoercing(EventType[] eventTypes, int[] streamNumbers, String[] properties, PropertyIndexedEventTable index, Type[] coercionTypes)
            : base(eventTypes, streamNumbers, properties, index)
	    {
	        this.coercionTypes = coercionTypes;
	    }

	    protected override Object[] GetKeys(EventBean[] eventsPerStream)
	    {
	        Object[] keyValues = new Object[propertyGetters.Length];
	        for (int i = 0; i < propertyGetters.Length; i++)
	        {
	            int streamNum = streamNumbers[i];
	            EventBean @event = eventsPerStream[streamNum];
	            Object value = propertyGetters[i].GetValue(@event);

	            Type coercionType = coercionTypes[i];
	            if ((value != null) && (value.GetType() != coercionType))
	            {
                    if (TypeHelper.IsNumericValue(value))
                    {
                        value = TypeHelper.CoerceBoxed(value, coercionTypes[i]);
                    }
	            }

	            keyValues[i] = value;
	        }
	        return keyValues;
	    }
	}
} // End of namespace