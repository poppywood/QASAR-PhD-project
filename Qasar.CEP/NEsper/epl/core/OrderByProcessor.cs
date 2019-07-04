using System;

using com.espertech.esper.collection;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// A processor for ordering output events according to the order specified in the order-by clause.
    /// </summary>
    public interface OrderByProcessor
    {
        /// <summary>
        /// Sort the output events. If the order-by processor needs group-by
        /// keys to evaluate the expressions in the order-by clause, these will
        /// be computed from the generating events.
        /// </summary>
        /// <param name="outgoingEvents">the events to be sorted</param>
        /// <param name="generatingEvents">the events that generated the output events (each event has a corresponding array of generating events per different event streams)</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <returns>
        /// an array containing the output events in sorted order
        /// </returns>
        EventBean[] Sort(EventBean[] outgoingEvents, EventBean[][] generatingEvents, bool isNewData);

        /// <summary>
        /// Sort the output events, using the provided group-by keys for
        /// evaluating grouped aggregation functions, and avoiding the cost of
        /// recomputing the keys.
        /// </summary>
        /// <param name="outgoingEvents">the events to sort</param>
        /// <param name="generatingEvents">the events that generated the output events (each event has a corresponding array of generating events per different event streams)</param>
        /// <param name="groupByKeys">the keys to use for determining the group-by group of output events</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <returns>
        /// an array containing the output events in sorted order
        /// </returns>
        EventBean[] Sort(EventBean[] outgoingEvents, EventBean[][] generatingEvents, MultiKeyUntyped[] groupByKeys, bool isNewData);

        /// <summary>
        /// Returns the sort key for a given row.
        /// </summary>
        /// <param name="eventsPerStream">is the row consisting of one event per stream</param>
        /// <param name="isNewData">is true for new data</param>
        /// <returns>sort key</returns>
        MultiKeyUntyped GetSortKey(EventBean[] eventsPerStream, bool isNewData);

        /// <summary>
        /// Returns the sort key for a each row where a row is a single event (no join, single stream).
        /// </summary>
        /// <param name="generatingEvents">is the rows consisting of one event per row</param>
        /// <param name="isNewData">is true for new data</param>
        /// <returns>sort key for each row</returns>
        MultiKeyUntyped[] GetSortKeyPerRow(EventBean[] generatingEvents, bool isNewData);

        /// <summary>
        /// Sort a given array of outgoing events using the sort keys returning a sorted outgoing event array.
        /// </summary>
        /// <param name="outgoingEvents">is the events to sort</param>
        /// <param name="orderKeys">is the keys to sort by</param>
        /// <returns>sorted events</returns>
        EventBean[] Sort(EventBean[] outgoingEvents, MultiKeyUntyped[] orderKeys);
    }
}
