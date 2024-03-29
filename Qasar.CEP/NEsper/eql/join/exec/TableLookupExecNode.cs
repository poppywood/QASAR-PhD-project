using System;
using System.Collections.Generic;

using net.esper.compat;
using net.esper.events;
using net.esper.util;

namespace net.esper.eql.join.exec
{
    /// <summary> Execution node for lookup in a table.</summary>
    public class TableLookupExecNode : ExecNode
    {
        private int indexedStream;
        private TableLookupStrategy lookupStrategy;

        /// <summary> Returns strategy for lookup.</summary>
        /// <returns> lookup strategy
        /// </returns>
        virtual public TableLookupStrategy LookupStrategy
        {
            get { return lookupStrategy; }
        }
        /// <summary> Returns target stream for lookup.</summary>
        /// <returns> indexed stream
        /// </returns>
        virtual public int IndexedStream
        {
            get { return indexedStream; }
        }

        /// <summary> Ctor.</summary>
        /// <param name="indexedStream">stream indexed for lookup
        /// </param>
        /// <param name="lookupStrategy">strategy to use for lookup (full table/indexed)
        /// </param>
        public TableLookupExecNode(int indexedStream, TableLookupStrategy lookupStrategy)
        {
            this.indexedStream = indexedStream;
            this.lookupStrategy = lookupStrategy;
        }

        /// <summary>
        /// Process single event using the prefill events to compile lookup results.
        /// </summary>
        /// <param name="lookupEvent">event to look up for or query for</param>
        /// <param name="prefillPath">set of events currently in the example tuple to serve
        /// as a prototype for result rows.</param>
        /// <param name="result">is the list of tuples to add a result row to</param>
        public override void Process(EventBean lookupEvent, EventBean[] prefillPath, IList<EventBean[]> result)
        {
            // Lookup events
            Set<EventBean> joinedEvents = lookupStrategy.Lookup(lookupEvent);

            if (joinedEvents == null)
            {
                return;
            }

            // Create result row for each found event
            foreach (EventBean joinedEvent in joinedEvents)
            {
                EventBean[] events = new EventBean[prefillPath.Length];
				Array.Copy(prefillPath, 0, events, 0, events.Length);
                events[indexedStream] = joinedEvent;
                result.Add(events);
            }
        }

        /// <summary>
        /// Output the execution strategy.
        /// </summary>
        /// <param name="writer">to output to</param>
        public override void Print(IndentWriter writer)
        {
            writer.WriteLine("TableLookupExecNode indexedStream=" + indexedStream + " lookup=" + lookupStrategy);
        }
    }
}
