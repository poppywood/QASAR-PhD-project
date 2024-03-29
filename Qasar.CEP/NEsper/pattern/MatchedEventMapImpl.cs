///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using net.esper.compat;
using net.esper.events;

namespace net.esper.pattern
{
	/// <summary>
	/// Collection for internal use similar to the MatchedEventMap class in the client package
	/// that holds the one or more events that could match any defined event expressions.
	/// The optional tag value supplied when an event expression is created is used as a key for placing
	/// matching event objects into this collection.
	/// </summary>
	public sealed class MatchedEventMapImpl : MatchedEventMap
	{
	    private EDictionary<String, EventBean> events = new HashDictionary<String, EventBean>();

	    /// <summary>
        /// Constructor creates an empty collection of events.
        /// </summary>
	    public MatchedEventMapImpl()
	    {
	    }

	    private MatchedEventMapImpl(EDictionary<String, EventBean> events)
	    {
	        this.events = events;
	    }

        /// <summary>
        /// Add an event to the collection identified by the given tag.
        /// </summary>
        /// <param name="tag">is an identifier to retrieve the event from</param>
        /// <param name="_event">is the event object to be added</param>
	    public void Add(String tag, EventBean _event)
	    {
	    	events[tag] = _event;
	    }

	    /// <summary>
	    /// Returns a map containing the events where the key is the event tag string and the value is the event
	    /// instance.
	    /// </summary>
	    /// <returns>Hashtable containing event instances</returns>
	    public EDictionary<String, EventBean> MatchingEvents
	    {
	        get { return events; }
	    }

	    /// <summary>
	    /// Returns a single event instance given the tag identifier, or null if the tag could not be located.
	    /// </summary>
	    /// <param name="tag">is the identifier to look for</param>
	    /// <returns>event instances for the tag</returns>
	    public EventBean GetMatchingEvent(String tag)
	    {
	        return events.Get(tag);
	    }

	    public override bool Equals(Object otherObject)
	    {
	        if (otherObject == this)
	        {
	            return true;
	        }

	        if (otherObject == null)
	        {
	            return false;
	        }

	        if (GetType() != otherObject.GetType())
	        {
	            return false;
	        }

	        MatchedEventMapImpl other = (MatchedEventMapImpl) otherObject;

	        if (events.Count != other.events.Count)
	        {
	            return false;
	        }

	        // Compare entry by entry
	        foreach (KeyValuePair<String, EventBean> entry in events)
	        {
	            String tag = entry.Key;
	            Object _event = entry.Value;

	            if (other.GetMatchingEvent(tag) != _event)
	            {
	                return false;
	            }
	        }

	        return true;
	    }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
	    public override String ToString()
	    {
	        StringBuilder buffer = new StringBuilder();
	        int count = 0;

	        foreach (KeyValuePair<String, EventBean> entry in events)
	        {
	            buffer.Append(" (" + (count++) + ") ");
	            buffer.Append("tag=" + entry.Key);
	            buffer.Append("  event=" + entry.Value);
	        }

	        return buffer.ToString();
	    }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
	    public override int GetHashCode()
	    {
	        return events.GetHashCode();
	    }

	    /// <summary>Make a shallow copy of this collection.</summary>
	    /// <returns>shallow copy</returns>
	    public MatchedEventMap ShallowCopy()
	    {
	        EDictionary<String, EventBean> copy = new HashDictionary<String, EventBean>();
	        copy.PutAll(events);
	        return new MatchedEventMapImpl(copy);
	    }

	    /// <summary>
	    /// Merge the state of an other match event structure into this one by adding all entries
	    /// within the MatchedEventMap to this match event.
	    /// </summary>
	    /// <param name="other">is the other instance to merge in.</param>
	    public void Merge(MatchedEventMap other)
	    {
	        if (!(other is MatchedEventMapImpl))
	        {
	            throw new UnsupportedOperationException("Merge requires same types");
	        }
	        MatchedEventMapImpl otherImpl = (MatchedEventMapImpl) other;
	        events.PutAll(otherImpl.events);
	    }
	}
} // End of namespace
