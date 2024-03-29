///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.collection;
using com.espertech.esper.compat;

using com.espertech.esper.events;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.join.table
{
	/// <summary>
	/// Index that organizes events by the event property values into hash buckets. Based on a HashMap
	/// with <see cref="com.espertech.esper.collection.MultiKeyUntyped"/> keys that store the property values.
	/// <p>
	/// Performs coercion of the index keys before storing the keys, and coercion of the lookup keys before lookup.
	/// </p>
	/// <p>
	/// Takes a list of property names as parameter. Doesn't care which event type the events have as long as the properties
	/// exist. If the same event is added twice, the class throws an exception on add.
    /// </p>
    /// </summary>
	public class PropertyIndTableCoerceAll : PropertyIndTableCoerceAdd
	{
	    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	    
	    private readonly Type[] coercionTypes;

	    /// <summary>Ctor.</summary>
	    /// <param name="streamNum">is the stream number of the indexed stream</param>
	    /// <param name="eventType">is the event type of the indexed stream</param>
	    /// <param name="propertyNames">are the property names to get property values</param>
	    /// <param name="coercionTypes">are the classes to coerce indexed values to</param>
	    public PropertyIndTableCoerceAll(int streamNum, EventType eventType, String[] propertyNames, Type[] coercionTypes)
	        : base(streamNum, eventType, propertyNames, coercionTypes)
	    {
	        this.coercionTypes = coercionTypes;
	    }

	    /// <summary>
	    /// Returns the set of events that have the same property value as the given event.
	    /// </summary>
	    /// <param name="keys">to compare against</param>
	    /// <returns>
	    /// set of events with property value, or null if none found (never returns zero-sized set)
	    /// </returns>
	    public override Set<EventBean> Lookup(Object[] keys)
	    {
	        for (int i = 0; i < keys.Length; i++)
	        {
	            Type coercionType = coercionTypes[i];
	            Object key = keys[i];
	            if ((key != null) && (key.GetType() != coercionType))
	            {
	                if (TypeHelper.IsNumericValue(key))
	                {
	                    key = TypeHelper.CoerceBoxed(key, coercionTypes[i]);
	                    keys[i] = key;
	                }
	            }
	        }
	        
	        MultiKeyUntyped _key = new MultiKeyUntyped(keys);
	        return propertyIndex.Get(_key);
	    }
	}
} // End of namespace
