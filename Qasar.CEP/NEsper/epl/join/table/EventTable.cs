///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.events;

namespace com.espertech.esper.epl.join.table
{
	/// <summary> Table of events allowing add and remove. Lookup in table is coordinated
	/// through the underlying implementation.
	/// </summary>
    public interface EventTable : IEnumerable<EventBean>
    {
        /// <summary> Add events to table.</summary>
        /// <param name="events">to add
        /// </param>
        void Add(IEnumerable<EventBean> events);

        /// <summary> Remove events from table.</summary>
        /// <param name="events">to remove
        /// </param>
        void Remove(IEnumerable<EventBean> events);

        /// <summary>Returns true if the index is empty, or false if not</summary>
        /// <returns>true for empty index</returns>
        bool IsEmpty { get; }

        /// <summary>Clear out index.</summary>
        void Clear();
    }
}
