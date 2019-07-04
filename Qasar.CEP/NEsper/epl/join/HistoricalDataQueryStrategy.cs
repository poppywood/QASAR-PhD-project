using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.events;
using com.espertech.esper.view;

namespace com.espertech.esper.epl.join
{
    /// <summary> Query strategy for use with <see cref="HistoricalEventViewable"/>
    /// to perform lookup for a given stream using the poll method on a viewable.
    /// </summary>

    public class HistoricalDataQueryStrategy : QueryStrategy
    {
        private readonly int myStreamNumber;
        private readonly int historicalStreamNumber;
        private readonly HistoricalEventViewable historicalEventViewable;
        private readonly EventBean[][] lookupRows1Event;
        private readonly bool isOuterJoin;
        private readonly ExprEqualsNode outerJoinCompareNode;
        private readonly HistoricalIndexLookupStrategy indexLookupStrategy;
        private readonly PollResultIndexingStrategy pollResultIndexingStrategy;

        /// <summary>Ctor.</summary>
        /// <param name="myStreamNumber">is the strategy's stream number</param>
        /// <param name="historicalStreamNumber">is the stream number of the view to be polled</param>
        /// <param name="historicalEventViewable">is the view to be polled from</param>
        /// <param name="isOuterJoin">is this is an outer join</param>
        /// <param name="outerJoinCompareNode">is the node to perform the on-comparison for outer joins</param>
        /// <param name="indexLookupStrategy">the strategy to use for limiting the cache result setto only those rows that match filter criteria</param>
        /// <param name="pollResultIndexingStrategy">the strategy for indexing poll-results such that astrategy can use the index instead of a full table scan to resolve rows</param>
        public HistoricalDataQueryStrategy(int myStreamNumber,
                                           int historicalStreamNumber,
                                           HistoricalEventViewable historicalEventViewable,
                                           bool isOuterJoin,
                                           ExprEqualsNode outerJoinCompareNode,
                                           HistoricalIndexLookupStrategy indexLookupStrategy,
                                           PollResultIndexingStrategy pollResultIndexingStrategy)
        {
            this.myStreamNumber = myStreamNumber;
            this.historicalStreamNumber = historicalStreamNumber;
            this.historicalEventViewable = historicalEventViewable;
            this.isOuterJoin = isOuterJoin;
            this.outerJoinCompareNode = outerJoinCompareNode;

            lookupRows1Event = new EventBean[1][];
            lookupRows1Event[0] = new EventBean[2];

            this.indexLookupStrategy = indexLookupStrategy;
            this.pollResultIndexingStrategy = pollResultIndexingStrategy;
        }

        /// <summary>
        /// Look up events returning tuples of joined events.
        /// </summary>
        /// <param name="lookupEvents">events to use to perform the join</param>
        /// <param name="joinSet">result join tuples of events</param>
        public void Lookup(EventBean[] lookupEvents, Set<MultiKey<EventBean>> joinSet)
        {
            EventBean[][] lookupRows;

            // If looking up a single event, reuse the buffered array
            if (lookupEvents.Length == 1)
            {
                lookupRows = lookupRows1Event;
                lookupRows[0][myStreamNumber] = lookupEvents[0];
            }
            else
            {
                // Prepare rows with each row N events where N is the number of streams
                lookupRows = new EventBean[lookupEvents.Length][];
                for (int i = 0; i < lookupEvents.Length; i++)
                {
                    lookupRows[i] = new EventBean[2];
                    lookupRows[i][myStreamNumber] = lookupEvents[i];
                }
            }

            EventTable[] indexPerLookupRow = historicalEventViewable.Poll(lookupRows, pollResultIndexingStrategy);

            int count = 0;
            foreach (EventTable index in indexPerLookupRow)
            {
                // Using the index, determine a subset of the whole indexed table to process, unless
                // the strategy is a full table scan
                IEnumerator<EventBean> subsetIter =
                    indexLookupStrategy.Lookup(lookupEvents[count], index);
                // Ensure that the subset enumerator is advanced; assuming that there
                // was an iterator at all.
                bool subsetIterAdvanced =
                    (subsetIter != null) &&
                    (subsetIter.MoveNext());

                // In an outer join
                if (isOuterJoin && !subsetIterAdvanced)
                {
                    EventBean[] resultRow = new EventBean[2];
                    resultRow[myStreamNumber] = lookupEvents[count];
                    joinSet.Add(new MultiKey<EventBean>(resultRow));
                }
                else
                {
                    if (subsetIterAdvanced)
                    {
                        // Add each row to the join result or, for outer joins, run through
                        // the outer join filter

                        do
                        {
                            EventBean[] resultRow = new EventBean[2];
                            resultRow[myStreamNumber] = lookupEvents[count];
                            resultRow[historicalStreamNumber] = subsetIter.Current;

                            // In an outer join compare the on-fields
                            if (isOuterJoin)
                            {
                                bool? compareResult = (bool?) outerJoinCompareNode.Evaluate(resultRow, true);
                                if (compareResult ?? false)
                                {
                                    joinSet.Add(new MultiKey<EventBean>(resultRow));
                                }
                            }
                            else
                            {
                                joinSet.Add(new MultiKey<EventBean>(resultRow));
                            }
                        } while (subsetIter.MoveNext());
                    }
                }
                count++;
            }
        }
    }
}
