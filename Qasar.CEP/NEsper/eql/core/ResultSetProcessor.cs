using System;
using System.Collections.Generic;

using net.esper.events;
using net.esper.collection;
using net.esper.compat;
using net.esper.view;

namespace net.esper.eql.core
{
	/// <summary>
	/// Processor for result sets coming from 2 sources. First, out of a simple view (no join).
	/// And second, out of a join of event streams. The processor must apply the select-clause, grou-by-clause and having-clauses
	/// as supplied. It must state what the event type of the result rows is.
	/// </summary>
    public interface ResultSetProcessor
    {
        /// <summary>
        /// Returns the event type of processed results.
        /// </summary>
        /// <value>The type of the result event.</value>
        /// <returns> event type of the resulting events posted by the processor.
        /// </returns>
        EventType ResultEventType { get; }

        /// <summary>
        /// For use by views posting their result, process the event rows that are entered and removed (new and old events).
        /// Processes according to select-clauses, group-by clauses and having-clauses and returns new events and
        /// old events as specified.
        /// </summary>
        /// <param name="newData">new events posted by view</param>
        /// <param name="oldData">old events posted by view</param>
        /// <returns>pair of new events and old events</returns>

        Pair<EventBean[], EventBean[]> ProcessViewResult(EventBean[] newData, EventBean[] oldData);

        /// <summary>
        /// For use by joins posting their result, process the event rows that are entered and removed (new and old events).
        /// Processes according to select-clauses, group-by clauses and having-clauses and returns new events and
        /// old events as specified.
        /// </summary>
        /// <param name="newEvents">new events posted by join</param>
        /// <param name="oldEvents">old events posted by join</param>
        /// <returns>pair of new events and old events</returns>
        Pair<EventBean[], EventBean[]> ProcessJoinResult(Set<MultiKey<EventBean>> newEvents, Set<MultiKey<EventBean>> oldEvents);

        /// <summary>
        /// Returns the iterator implementing the group-by and aggregation and order-by logic
        /// specific to each case of use of these construct.
        /// </summary>
        /// <param name="parent">is the parent view iterator</param>
        /// <returns>event iterator</returns>
        IEnumerator<EventBean> GetEnumerator(Viewable parent);
    }
}
