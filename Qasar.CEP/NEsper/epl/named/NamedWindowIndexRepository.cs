///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.epl.lookup;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// A repository of index tables for use with a single named window and all it's deleting statements that
    /// may use the indexes to correlate triggering events with indexed events of the named window.
    ///
    /// Maintains index tables and keeps a reference count for user. Allows reuse of indexes for multiple
    /// deleting statements.
    /// </summary>
	public class NamedWindowIndexRepository
	{
	    private readonly IList<EventTable> tables;
	    private readonly Map<MultiKey<JoinedPropDesc>, Pair<PropertyIndexedEventTable, int>> tableIndexes;

	    /// <summary>Ctor.</summary>
	    public NamedWindowIndexRepository()
	    {
	        tables = new List<EventTable>();
	        tableIndexes = new HashMap<MultiKey<JoinedPropDesc>, Pair<PropertyIndexedEventTable, int>>();
	    }

	    /// <summary>
	    /// Create a new index table or use an existing index table, by matching the
	    /// join descriptor properties to an existing table.
	    /// </summary>
	    /// <param name="joinedPropDesc">
	    /// must be in sorted natural order and define the properties joined
	    /// </param>
	    /// <param name="prefilledEvents">
	    /// is the events to enter into a new table, if a new table is created
	    /// </param>
	    /// <param name="indexedType">is the type of event to hold in the index</param>
	    /// <param name="mustCoerce">is an indicator whether coercion is required or not.</param>
	    /// <returns>new or existing index table</returns>
	    public PropertyIndexedEventTable AddTable(JoinedPropDesc[] joinedPropDesc,
	                               IEnumerable<EventBean> prefilledEvents,
	                               EventType indexedType,
	                               bool mustCoerce)
	    {
	        MultiKey<JoinedPropDesc> indexPropKey = new MultiKey<JoinedPropDesc>(joinedPropDesc);

	        // Get an existing table, if any
	        Pair<PropertyIndexedEventTable, int> refTablePair = tableIndexes.Get(indexPropKey);
	        if (refTablePair != null)
	        {
	            refTablePair.Second++;
	            return refTablePair.First;
	        }

	        String[] indexProps = JoinedPropDesc.GetIndexProperties(joinedPropDesc);
	        Type[] coercionTypes = JoinedPropDesc.GetCoercionTypes(joinedPropDesc);
	        PropertyIndexedEventTable table;
	        if (!mustCoerce)
	        {
	            table = new PropertyIndexedEventTable(0, indexedType, indexProps);
	        }
	        else
	        {
	            table = new PropertyIndTableCoerceAdd(0, indexedType, indexProps, coercionTypes);
	        }

	        // fill table since its new
	        EventBean[] events = new EventBean[1];
	        foreach (EventBean prefilledEvent in prefilledEvents)
	        {
	            events[0] = prefilledEvent;
	            table.Add(events);
	        }

	        // add table
	        tables.Add(table);

	        // add index, reference counted
	        tableIndexes.Put(indexPropKey, new Pair<PropertyIndexedEventTable, int>(table, 1));

	        return table;
	    }

	    /// <summary>
	    /// Remove a reference to an index table, decreasing its reference count.
	    /// If the table is no longer used, discard it and no longer update events into the index.
	    /// </summary>
	    /// <param name="table">to remove a reference to</param>
	    public void RemoveTableReference(EventTable table)
	    {
	        foreach (KeyValuePair<MultiKey<JoinedPropDesc>, Pair<PropertyIndexedEventTable, int>> entry in tableIndexes)
	        {
	            if (entry.Value.First == table)
	            {
	                int current = entry.Value.Second;
	                if (current > 1)
	                {
	                    current--;
	                    entry.Value.Second = current;
	                    break;
	                }

	                tables.Remove(table);
	                tableIndexes.Remove(entry.Key);
	                break;
	            }
	        }
	    }

	    /// <summary>Returns a list of current index tables in the repository.</summary>
	    /// <returns>index tables</returns>
	    public IList<EventTable> Tables
	    {
	        get { return tables; }
	    }

	    /// <summary>Destroy indexes.</summary>
	    public void Destroy()
	    {
	        tables.Clear();
	        tableIndexes.Clear();
	    }
	}
} // End of namespace
