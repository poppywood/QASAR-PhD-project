///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.epl.join.table;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.join
{
    /// <summary>
    /// A strategy for converting a poll-result into a potentially indexed table.
    /// 
    /// Some implementations may decide to not index the poll result and simply hold
    /// a reference to the result. Other implementations may use predetermined index
    /// properties to index the poll result for faster lookup.
    /// </summary>
	public interface PollResultIndexingStrategy
	{
	    /// <summary>Build and index of a poll result.</summary>
	    /// <param name="pollResult">result of a poll operation</param>
	    /// <param name="isActiveCache">
	    /// true to indicate that caching is active and therefore index building makes sense as
	    /// the index structure is not a throw-away.
	    /// </param>
	    /// <returns>indexed collection of poll results</returns>
	    EventTable Index(IList<EventBean> pollResult, bool isActiveCache);
	}

    /// <summary>
    /// A delegate that mimics the behavior of the PollResultIndexStrategy.
    /// </summary>

    public delegate EventTable PollResultIndexer(IList<EventBean> pollResult, bool isActiveCache);

    /// <summary>
    /// An implementation of the PatternMatchCallback that proxies the
    /// interface through a delegate.
    /// </summary>
    /// 
    public class ProxyPollResultIndexingStrategy : PollResultIndexingStrategy
    {
        private readonly PollResultIndexer m_delegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyPollResultIndexingStrategy"/> class.
        /// </summary>
        /// <param name="_delegate">The _delegate.</param>
        public ProxyPollResultIndexingStrategy(PollResultIndexer _delegate)
    	{
    		m_delegate = _delegate;
    	}

        #region PollResultIndexingStrategy Members

        /// <summary>
        /// Build and index of a poll result.
        /// </summary>
        /// <param name="pollResult">result of a poll operation</param>
        /// <param name="isActiveCache">true to indicate that caching is active and therefore index building makes sense as
        /// the index structure is not a throw-away.</param>
        /// <returns>indexed collection of poll results</returns>
        public EventTable Index(IList<EventBean> pollResult, bool isActiveCache)
        {
            return m_delegate.Invoke(pollResult, isActiveCache);
        }

        #endregion
    }
} // End of namespace
