using System.Collections.Generic;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.view;

namespace com.espertech.esper.epl.core
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
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>pair of new events and old events</returns>
        UniformPair<EventBean[]> ProcessViewResult(EventBean[] newData, EventBean[] oldData, bool isSynthesize);

        /// <summary>
        /// For use by joins posting their result, process the event rows that are entered and removed (new and old events).
        /// Processes according to select-clauses, group-by clauses and having-clauses and returns new events and
        /// old events as specified.
        /// </summary>
        /// <param name="newEvents">new events posted by join</param>
        /// <param name="oldEvents">old events posted by join</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>pair of new events and old events</returns>
        UniformPair<EventBean[]> ProcessJoinResult(Set<MultiKey<EventBean>> newEvents, Set<MultiKey<EventBean>> oldEvents, bool isSynthesize);

        /// <summary>
        /// Returns the iterator implementing the group-by and aggregation and order-by logic
        /// specific to each case of use of these construct.
        /// </summary>
        /// <param name="parent">is the parent view iterator</param>
        /// <returns>event iterator</returns>
        IEnumerator<EventBean> GetEnumerator(Viewable parent);

        /// <summary>Returns the iterator for iterating over a join-result.</summary>
        /// <param name="joinSet">is the join result set</param>
        /// <returns>iterator over join results</returns>
        IEnumerator<EventBean> GetEnumerator(Set<MultiKey<EventBean>> joinSet);

        /// <summary>Clear out current state.</summary>
        void Clear();

        /// <summary>Processes batched events in case of output-rate limiting.</summary>
        /// <param name="joinEventsSet">the join results</param>
        /// <param name="generateSynthetic">flag to indicate whether synthetic events must be generated</param>
        /// <param name="outputLimitLimitType">the type of output rate limiting</param>
        /// <returns>results for dispatch</returns>
        UniformPair<EventBean[]> ProcessOutputLimitedJoin(IList<UniformPair<Set<MultiKey<EventBean>>>> joinEventsSet, bool generateSynthetic, OutputLimitLimitType outputLimitLimitType);

        /// <summary>Processes batched events in case of output-rate limiting.</summary>
        /// <param name="viewEventsList">the view results</param>
        /// <param name="generateSynthetic">flag to indicate whether synthetic events must be generated</param>
        /// <param name="outputLimitLimitType">the type of output rate limiting</param>
        /// <returns>results for dispatch</returns>
        UniformPair<EventBean[]> ProcessOutputLimitedView(IList<UniformPair<EventBean[]>> viewEventsList, bool generateSynthetic, OutputLimitLimitType outputLimitLimitType);
    }
}
