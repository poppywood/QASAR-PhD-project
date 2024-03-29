using System;
using System.Collections.Generic;

using net.esper.collection;
using net.esper.compat;
using net.esper.eql.agg;
using net.esper.eql.expression;
using net.esper.events;
using net.esper.view;

namespace net.esper.eql.core
{
    /// <summary> Result set processor for the fully-grouped case:
    /// there is a group-by and all non-aggregation event properties in the select clause are listed in the group by,
    /// and there are aggregation functions.
    /// <para>
    /// Produces one row for each group that changed (and not one row per event). Computes MultiKey group-by keys for
    /// each event and uses a set of the group-by keys to generate the result rows, using the first (old or new, anyone) event
    /// for each distinct group-by key.
    /// </para>
    /// </summary>

    public class ResultSetProcessorRowPerGroup : ResultSetProcessor
    {
        private readonly SelectExprProcessor selectExprProcessor;
        private readonly OrderByProcessor orderByProcessor;
        private readonly AggregationService aggregationService;
        private readonly IList<ExprNode> groupKeyNodes;
        private readonly ExprNode optionalHavingNode;
        private readonly bool isOutputLimiting;
        private readonly bool isOutputLimitLastOnly;
        private readonly bool isSorting;

        // For output rate limiting, keep a representative event for each group for
        // representing each group in an output limit clause
        private readonly IDictionary<MultiKeyUntyped, EventBean> newEventGroupReps = new LinkedDictionary<MultiKeyUntyped, EventBean>();
        private readonly IDictionary<MultiKeyUntyped, EventBean> oldEventGroupReps = new LinkedDictionary<MultiKeyUntyped, EventBean>();

        // For sorting, keep the generating events for each outgoing event
        private readonly IDictionary<MultiKeyUntyped, EventBean[]> newGenerators = new Dictionary<MultiKeyUntyped, EventBean[]>();
        private readonly IDictionary<MultiKeyUntyped, EventBean[]> oldGenerators = new Dictionary<MultiKeyUntyped, EventBean[]>();

        /// <summary>
        /// Returns the event type of processed results.
        /// </summary>
        /// <value></value>
        /// <returns> event type of the resulting events posted by the processor.
        /// </returns>
        virtual public EventType ResultEventType
        {
            get { return selectExprProcessor.ResultEventType; }
        }

        /// <summary> Ctor.</summary>
        /// <param name="selectExprProcessor">for processing the select expression and generting the final output rows
        /// </param>
        /// <param name="orderByProcessor">for sorting outgoing events according to the order-by clause
        /// </param>
        /// <param name="aggregationService">handles aggregation
        /// </param>
        /// <param name="groupKeyNodes">list of group-by expression nodes needed for building the group-by keys
        /// </param>
        /// <param name="optionalHavingNode">expression node representing validated HAVING clause, or null if none given.
        /// Aggregation functions in the having node must have been pointed to the AggregationService for evaluation.
        /// </param>
        /// <param name="isOutputLimiting">true to indicate we are output limiting and must keep producing
        /// a row per group even if groups didn't change
        /// </param>
        /// <param name="isOutputLimitLastOnly">true if output limiting and interested in last event only
        /// </param>
        public ResultSetProcessorRowPerGroup(
            SelectExprProcessor selectExprProcessor,
            OrderByProcessor orderByProcessor,
            AggregationService aggregationService,
            IList<ExprNode> groupKeyNodes,
            ExprNode optionalHavingNode,
            bool isOutputLimiting,
            bool isOutputLimitLastOnly)
        {
            this.selectExprProcessor = selectExprProcessor;
            this.orderByProcessor = orderByProcessor;
            this.aggregationService = aggregationService;
            this.groupKeyNodes = groupKeyNodes;
            this.optionalHavingNode = optionalHavingNode;
            this.isOutputLimiting = isOutputLimiting;
            this.isOutputLimitLastOnly = isOutputLimitLastOnly;
            this.isSorting = orderByProcessor != null;
        }

        /// <summary>
        /// For use by joins posting their result, process the event rows that are entered and removed (new and old events).
        /// Processes according to select-clauses, group-by clauses and having-clauses and returns new events and
        /// old events as specified.
        /// </summary>
        /// <param name="newEvents">new events posted by join</param>
        /// <param name="oldEvents">old events posted by join</param>
        /// <returns>pair of new events and old events</returns>
        public Pair<EventBean[], EventBean[]> ProcessJoinResult(
            Set<MultiKey<EventBean>> newEvents,
            Set<MultiKey<EventBean>> oldEvents)
        {
            // Generate group-by keys for all events, collect all keys in a set for later event generation
            IDictionary<MultiKeyUntyped, EventBean[]> keysAndEvents = new Dictionary<MultiKeyUntyped, EventBean[]>();
            MultiKeyUntyped[] newDataMultiKey = GenerateGroupKeys(newEvents, keysAndEvents, true);
            MultiKeyUntyped[] oldDataMultiKey = GenerateGroupKeys(oldEvents, keysAndEvents, false);

            // generate old events
            EventBean[] selectOldEvents = GenerateOutputEventsJoin(keysAndEvents, optionalHavingNode, oldEventGroupReps, oldGenerators, false);

            // update aggregates
            if (!newEvents.IsEmpty)
            {
                // apply old data to aggregates
                int count = 0;
                foreach (MultiKey<EventBean> eventsPerStream in newEvents)
                {
                    aggregationService.ApplyEnter(eventsPerStream.Array, newDataMultiKey[count]);
                    count++;
                }
            }
            if (!oldEvents.IsEmpty)
            {
                // apply old data to aggregates
                int count = 0;
                foreach (MultiKey<EventBean> eventsPerStream in oldEvents)
                {
                    aggregationService.ApplyLeave(eventsPerStream.Array, oldDataMultiKey[count]);
                    count++;
                }
            }

            // generate new events using select expressions
            EventBean[] selectNewEvents = GenerateOutputEventsJoin(keysAndEvents, optionalHavingNode, newEventGroupReps, newGenerators, true);

            if ((selectNewEvents != null) || (selectOldEvents != null))
            {
                return new Pair<EventBean[], EventBean[]>(selectNewEvents, selectOldEvents);
            }
            return null;
        }

        /// <summary>
        /// For use by views posting their result, process the event rows that are entered and removed (new and old events).
        /// Processes according to select-clauses, group-by clauses and having-clauses and returns new events and
        /// old events as specified.
        /// </summary>
        /// <param name="newData">new events posted by view</param>
        /// <param name="oldData">old events posted by view</param>
        /// <returns>pair of new events and old events</returns>
        public Pair<EventBean[], EventBean[]> ProcessViewResult(EventBean[] newData, EventBean[] oldData)
        {
            // Generate group-by keys for all events, collect all keys in a set for later event generation
            IDictionary<MultiKeyUntyped, EventBean> keysAndEvents = new Dictionary<MultiKeyUntyped, EventBean>();
            MultiKeyUntyped[] newDataMultiKey = GenerateGroupKeys(newData, keysAndEvents, true);
            MultiKeyUntyped[] oldDataMultiKey = GenerateGroupKeys(oldData, keysAndEvents, false);

            // generate old events
            EventBean[] selectOldEvents = GenerateOutputEventsView(keysAndEvents, optionalHavingNode, oldEventGroupReps, oldGenerators, false);

            // update aggregates
            EventBean[] eventsPerStream = new EventBean[1];
            if (newData != null)
            {
                // apply new data to aggregates
                for (int i = 0; i < newData.Length; i++)
                {
                    eventsPerStream[0] = newData[i];
                    aggregationService.ApplyEnter(eventsPerStream, newDataMultiKey[i]);
                }
            }
            if (oldData != null)
            {
                // apply old data to aggregates
                for (int i = 0; i < oldData.Length; i++)
                {
                    eventsPerStream[0] = oldData[i];
                    aggregationService.ApplyLeave(eventsPerStream, oldDataMultiKey[i]);
                }
            }

            // generate new events using select expressions
            EventBean[] selectNewEvents = GenerateOutputEventsView(keysAndEvents, optionalHavingNode, newEventGroupReps, newGenerators, true);

            if ((selectNewEvents != null) || (selectOldEvents != null))
            {
                return new Pair<EventBean[], EventBean[]>(selectNewEvents, selectOldEvents);
            }
            return null;
        }

        /// <summary>
        /// Generates the output events view.
        /// </summary>
        /// <param name="keysAndEvents">The keys and events.</param>
        /// <param name="optionalHavingNode">The optional having expr.</param>
        /// <param name="groupReps">The group reps.</param>
        /// <param name="generators">The generators.</param>
        /// <param name="isNewData">if set to <c>true</c> [is new data].</param>
        /// <returns></returns>

        private EventBean[] GenerateOutputEventsView(
            IDictionary<MultiKeyUntyped, EventBean> keysAndEvents, ExprNode optionalHavingNode,
            IDictionary<MultiKeyUntyped, EventBean> groupReps,
            IDictionary<MultiKeyUntyped, EventBean[]> generators,
            bool isNewData)
        {
            EventBean[] eventsPerStream = new EventBean[1];
            EventBean[] events = new EventBean[keysAndEvents.Count];
            MultiKeyUntyped[] keys = new MultiKeyUntyped[keysAndEvents.Count];
            EventBean[][] currentGenerators = null;
            if (isSorting)
            {
                currentGenerators = new EventBean[keysAndEvents.Count][];
            }

            int count = 0;
            foreach (KeyValuePair<MultiKeyUntyped, EventBean> entry in keysAndEvents)
            {
                // Set the current row of aggregation states
                aggregationService.SetCurrentRow(entry.Key);

                eventsPerStream[0] = entry.Value;

                // Filter the having clause
                if (optionalHavingNode != null)
                {
                    bool result = (bool) optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                    if (!result)
                    {
                        continue;
                    }
                }

                events[count] = selectExprProcessor.Process(eventsPerStream, isNewData);
                keys[count] = entry.Key;
                if (isSorting)
                {
                    EventBean[] currentEventsPerStream = new EventBean[] {entry.Value};
                    generators[keys[count]] = currentEventsPerStream;
                    currentGenerators[count] = currentEventsPerStream;
                }

                count++;
            }

            // Resize if some rows were filtered out
            if (count != events.Length)
            {
                if (count == 0)
                {
                    return null;
                }
                EventBean[] outList = new EventBean[count];
                Array.Copy(events, 0, outList, 0, count);
                events = outList;

                if (isSorting || (isOutputLimiting && !isOutputLimitLastOnly))
                {
                    MultiKeyUntyped[] outKeys = new MultiKeyUntyped[count];
                    Array.Copy(keys, 0, outKeys, 0, count);
                    keys = outKeys;
                }

                if (isSorting)
                {
                    EventBean[][] outGens = new EventBean[count][];
                    Array.Copy(currentGenerators, 0, outGens, 0, count);
                    currentGenerators = outGens;
                }
            }

            return ApplyOutputLimitAndOrderBy(events, currentGenerators, keys, groupReps, generators, isNewData);
        }

        private EventBean[] GenerateOutputEventsJoin(
            IDictionary<MultiKeyUntyped, EventBean[]> keysAndEvents,
            ExprNode optionalHavingNode,
            IDictionary<MultiKeyUntyped, EventBean> groupReps,
            IDictionary<MultiKeyUntyped, EventBean[]> generators,
			bool isNewData
			)
        {
            EventBean[] events = new EventBean[keysAndEvents.Count];
            MultiKeyUntyped[] keys = new MultiKeyUntyped[keysAndEvents.Count];
            EventBean[][] currentGenerators = null;
            if (isSorting)
            {
                currentGenerators = new EventBean[keysAndEvents.Count][];
            }

            int count = 0;
            foreach (KeyValuePair<MultiKeyUntyped, EventBean[]> entry in keysAndEvents)
            {
                aggregationService.SetCurrentRow(entry.Key);
                EventBean[] eventsPerStream = entry.Value;

                // Filter the having clause
                if (optionalHavingNode != null)
                {
                    bool result = (bool)optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                    if (!result)
                    {
                        continue;
                    }
                }

                events[count] = selectExprProcessor.Process(eventsPerStream, isNewData);
                keys[count] = entry.Key;
                if (isSorting)
                {
                    generators[keys[count]] = eventsPerStream;
                    currentGenerators[count] = eventsPerStream;
                }

                count++;
            }

            // Resize if some rows were filtered out
            if (count != events.Length)
            {
                if (count == 0)
                {
                    return null;
                }
                EventBean[] _out = new EventBean[count];
                Array.Copy(events, 0, _out, 0, count);
                events = _out;

                if (isSorting || (isOutputLimiting && !isOutputLimitLastOnly))
                {
                    MultiKeyUntyped[] outKeys = new MultiKeyUntyped[count];
                    Array.Copy(keys, 0, outKeys, 0, count);
                    keys = outKeys;
                }

                if (isSorting)
                {
                    EventBean[][] outGens = new EventBean[count][];
                    Array.Copy(currentGenerators, 0, outGens, 0, count);
                    currentGenerators = outGens;
                }
            }

            return ApplyOutputLimitAndOrderBy(events, currentGenerators, keys, groupReps, generators, isNewData);
        }

        private EventBean[] ApplyOutputLimitAndOrderBy(
            EventBean[] events,
            EventBean[][] currentGenerators,
            MultiKeyUntyped[] keys,
            IDictionary<MultiKeyUntyped, EventBean> groupReps,
            IDictionary<MultiKeyUntyped, EventBean[]> generators,
			bool isNewData
			)
        {
            if (isOutputLimiting && !isOutputLimitLastOnly)
            {
                // Update the group representatives
                int count = 0;
                foreach (MultiKeyUntyped key in keys)
                {
                    groupReps[key] = events[count++];
                }

                // Update the outgoing events
                events = CollectionHelper.ToArray(groupReps.Values);

                // Update the generating events and group-by keys if needed
                if (isSorting)
                {
                    currentGenerators = CollectionHelper.ToArray(generators.Values);
                    keys = CollectionHelper.ToArray( groupReps.Keys ) ;
                }
            }

            if (isSorting)
            {
                events = orderByProcessor.Sort(events, currentGenerators, keys, isNewData);
            }

            return events;
        }

        private MultiKeyUntyped[] GenerateGroupKeys(
			EventBean[] events, 
			IDictionary<MultiKeyUntyped, EventBean> eventPerKey,
			bool isNewData)
        {
            if (events == null)
            {
                return null;
            }

            EventBean[] eventsPerStream = new EventBean[1];
            MultiKeyUntyped[] keys = new MultiKeyUntyped[events.Length];

            for (int i = 0; i < events.Length; i++)
            {
                eventsPerStream[0] = events[i];
                
                MultiKeyUntyped tempKey = keys[i] = GenerateGroupKey(eventsPerStream, isNewData);

                if (!eventPerKey.ContainsKey(tempKey))
                {
                    eventPerKey[tempKey] = events[i];
                }
            }

            return keys;
        }

        private MultiKeyUntyped[] GenerateGroupKeys(
			Set<MultiKey<EventBean>> resultSet,
			IDictionary<MultiKeyUntyped, EventBean[]> eventPerKey,
			bool isNewData)
        {
            if (resultSet.IsEmpty)
            {
                return null;
            }

            MultiKeyUntyped[] keys = new MultiKeyUntyped[resultSet.Count];

            int count = 0;
            foreach (MultiKey<EventBean> eventsPerStream in resultSet)
            {
                MultiKeyUntyped tempKey = keys[count] = GenerateGroupKey(eventsPerStream.Array, isNewData);

                if (!eventPerKey.ContainsKey(tempKey))
                {
                    eventPerKey[tempKey] = eventsPerStream.Array;
                }

                count++;
            }

            return keys;
        }

        /// <summary>
        /// Generates the group-by key for the row
        /// </summary>
        /// <param name="eventsPerStream">the row of events</param>
        /// <param name="isNewData">true for new data</param>
        /// <returns></returns>
        private MultiKeyUntyped GenerateGroupKey(EventBean[] eventsPerStream, bool isNewData)
        {
            Object[] keys = new Object[groupKeyNodes.Count];

            int count = 0;
            foreach (ExprNode exprNode in groupKeyNodes)
            {
                keys[count] = exprNode.Evaluate(eventsPerStream, isNewData);
                count++;
            }

            return new MultiKeyUntyped(keys);
        }

        /// <summary>
        /// Gets the optional having expression.
        /// </summary>
        /// <value>The optional having expression.</value>
        public ExprNode OptionalHavingNode
        {
            get { return optionalHavingNode; }
        }

        /// <summary>
        /// Gets the select expression processor.
        /// </summary>
        /// <value>The select expression processor.</value>
        public SelectExprProcessor SelectExprProcessor
        {
            get {return selectExprProcessor;}
        }

        private IEnumerator<EventBean> GetRowPerGroupEnumerator(Viewable parent)
        {
            // Cache member variables locally
            AggregationService lAggregationService = this.aggregationService;
            SelectExprProcessor lSelectExprProcessor = this.selectExprProcessor;
            ExprNode lOptionalHavingNode = OptionalHavingNode;
            // Create items needed for evaluation
            EventBean[] eventsPerStream = new EventBean[1];
            Set<MultiKeyUntyped> priorSeenGroups = new HashSet<MultiKeyUntyped>();
            
            // Iterate over the candidates in the parent
            foreach( EventBean candidate in parent )
            {
                eventsPerStream[0] = candidate;

                MultiKeyUntyped groupKey = GenerateGroupKey(eventsPerStream, true);
                lAggregationService.SetCurrentRow(groupKey);

                bool pass = true;
                if (lOptionalHavingNode != null)
                {
                    pass = (bool)lOptionalHavingNode.Evaluate(eventsPerStream, true);
                }
                if (!pass)
                {
                    continue;
                }
                if (priorSeenGroups.Contains(groupKey))
                {
                    continue;
                }
                priorSeenGroups.Add(groupKey);

                if (lSelectExprProcessor == null)
                {
                    yield return candidate;
                }
                else
                {
                    yield return lSelectExprProcessor.Process(eventsPerStream, true);
                }
            }
        }

        public IEnumerator<EventBean> GetEnumerator(Viewable parent)
        {
            if (orderByProcessor == null)
            {
                return GetRowPerGroupEnumerator(parent);
            }

            // Pull all parent events, generate order keys
            EventBean[] eventsPerStream = new EventBean[1];
            List<EventBean> outgoingEvents = new List<EventBean>();
            List<MultiKeyUntyped> orderKeys = new List<MultiKeyUntyped>();
            Set<MultiKeyUntyped> priorSeenGroups = new HashSet<MultiKeyUntyped>();

            foreach( EventBean candidate in parent )
            {
                eventsPerStream[0] = candidate;

                MultiKeyUntyped groupKey = GenerateGroupKey(eventsPerStream, true);
                aggregationService.SetCurrentRow(groupKey);

                Boolean pass = true;
                if (optionalHavingNode != null)
                {
                    pass = (bool) optionalHavingNode.Evaluate(eventsPerStream, true);
                }
                if (!pass)
                {
                    continue;
                }
                if (priorSeenGroups.Contains(groupKey))
                {
                    continue;
                }
                priorSeenGroups.Add(groupKey);

                if (selectExprProcessor == null)
                {
                    outgoingEvents.Add(candidate);
                }
                else
                {
                    outgoingEvents.Add(selectExprProcessor.Process(eventsPerStream, true));
                }

                MultiKeyUntyped orderKey = orderByProcessor.GetSortKey(eventsPerStream, true);
                orderKeys.Add(orderKey);
            }

            // sort
            EventBean[] outgoingEventsArr = outgoingEvents.ToArray();
            MultiKeyUntyped[] orderKeysArr = orderKeys.ToArray();
            IList<EventBean> orderedEvents = orderByProcessor.Sort(outgoingEventsArr, orderKeysArr);

            return orderedEvents.GetEnumerator();
        }
    }
}
