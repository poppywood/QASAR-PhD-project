///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.events;

namespace com.espertech.esper.filter
{
	/// <summary>
	/// Interface for a callback method to be called when an event matches a filter specification. Provided
	/// as a convenience for use as a filter handle for registering with the <see cref="FilterService"/>.
	/// </summary>

	public interface FilterHandleCallback : FilterHandle
	{
	    /// <summary>
	    /// Indicate that an event was evaluated by the <see cref="com.espertech.esper.filter.FilterService"/>
	    /// which matches the filter specification <see cref="com.espertech.esper.filter.FilterSpecCompiled"/> associated with this callback.
	    /// </summary>
	    /// <param name="_event">the event received that matches the filter specification</param>
	    void MatchFound(EventBean _event);
	}

    /// <summary>
    /// Indicate that an event was evaluated by the <see cref="com.espertech.esper.filter.FilterService"/>
    /// which matches the filter specification <see cref="com.espertech.esper.filter.FilterSpecCompiled"/> associated with this callback.
    /// </summary>
    /// <param name="_event">the event received that matches the filter specification</param>
    public delegate void FilterHandleCallbackDelegate(EventBean _event);

    public class FilterHandleCallbackImpl : FilterHandleCallback
    {
        private FilterHandleCallbackDelegate m_delegate;

        public FilterHandleCallbackImpl( FilterHandleCallbackDelegate dg )
        {
            m_delegate = dg;
        }

        public void MatchFound(EventBean _event)
        {
            m_delegate(_event);
        }
    }
} // End of namespace
