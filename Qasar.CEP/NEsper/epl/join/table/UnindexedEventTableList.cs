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
using com.espertech.esper.events;

namespace com.espertech.esper.epl.join.table
{
    /// <summary>
    /// Simple table of events without an index, based on a List implementation rather then a set
    /// since we know there cannot be duplicates (such as a poll returning individual rows).
    /// </summary>
	public class UnindexedEventTableList : EventTable
	{
	    private readonly static NullEnumerator<EventBean> emptyIterator = new NullEnumerator<EventBean>();
	    private readonly IList<EventBean> eventSet;

	    /// <summary>Ctor.</summary>
	    /// <param name="eventSet">is a list initializing the table</param>
	    public UnindexedEventTableList(IList<EventBean> eventSet)
	    {
	        this.eventSet = eventSet;
	    }

        /// <summary>
        /// Adds the specified add events.
        /// </summary>
        /// <param name="addEvents">The add events.</param>
	    public void Add(IEnumerable<EventBean> addEvents)
	    {
	        if (addEvents == null)
	        {
	            return;
	        }

	        foreach (EventBean addEvent in addEvents)
	        {
	            eventSet.Add(addEvent);
	        }
	    }

        /// <summary>
        /// Removes the specified remove events.
        /// </summary>
        /// <param name="removeEvents">The remove events.</param>
        public void Remove(IEnumerable<EventBean> removeEvents)
	    {
	        if (removeEvents == null)
	        {
	            return;
	        }

	        foreach (EventBean removeEvent in removeEvents)
	        {
	            eventSet.Remove(removeEvent);
	        }
	    }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
	    public IEnumerator<EventBean> GetEnumerator()
	    {
	        if (eventSet == null)
	        {
	            return emptyIterator;
	        }
	        return eventSet.GetEnumerator();
	    }

        /// <summary>
        /// Returns true if the index is empty, or false if not
        /// </summary>
        /// <value></value>
        /// <returns>true for empty index</returns>
	    public bool IsEmpty
	    {
            get { return eventSet.Count == 0; }
	    }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
	    public override String ToString()
	    {
	        return "UnindexedEventTableList";
	    }

        /// <summary>
        /// Clear out index.
        /// </summary>
	    public void Clear()
	    {
	        eventSet.Clear();
	    }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
} // End of namespace
