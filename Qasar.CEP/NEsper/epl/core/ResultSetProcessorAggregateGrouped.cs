///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.agg;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.util;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Result-set processor for the aggregate-grouped case: there is a group-by and one or more
    /// non-aggregation event properties in the select clause are not listed in the group by, and 
    /// there are aggregation functions.
    /// <para/> This processor does perform grouping by computing 
    /// MultiKey group-by keys for each row. The processor generates one row for each event entering 
    /// (new event) and one row for each event leaving (old event).
    /// <para/>
    /// Aggregation state is a table of rows held by <see cref="AggregationService"/> where the row key
    /// is the group-by MultiKey.
    /// </summary>
    public class ResultSetProcessorAggregateGrouped : ResultSetProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
        private readonly SelectExprProcessor selectExprProcessor;
        private readonly OrderByProcessor orderByProcessor;
        private readonly AggregationService aggregationService;
        private readonly ICollection<ExprNode> groupKeyNodes;
        private readonly ExprNode optionalHavingNode;
        private readonly bool isSorting;
        private readonly bool isSelectRStream;
    
        // For output limiting, keep a representative of each group-by group
        private readonly Map<MultiKeyUntyped, EventBean[]> eventGroupReps = new HashMap<MultiKeyUntyped, EventBean[]>();
        private readonly Map<MultiKeyUntyped, EventBean[]> workCollection = new LinkedHashMap<MultiKeyUntyped, EventBean[]>();
        private readonly Map<MultiKeyUntyped, EventBean[]> workCollectionTwo = new LinkedHashMap<MultiKeyUntyped, EventBean[]>();
    
        // For sorting, keep the generating events for each outgoing event
        private readonly Map<MultiKeyUntyped, EventBean[]> newGenerators = new HashMap<MultiKeyUntyped, EventBean[]>();
    	private readonly Map<MultiKeyUntyped, EventBean[]> oldGenerators = new HashMap<MultiKeyUntyped, EventBean[]>();
    
    
        /// <summary>Ctor. </summary>
        /// <param name="selectExprProcessor">for processing the select expression and generting the readonly output rows</param>
        /// <param name="orderByProcessor">for sorting outgoing events according to the order-by clause</param>
        /// <param name="aggregationService">handles aggregation</param>
        /// <param name="groupKeyNodes">list of group-by expression nodes needed for building the group-by keys</param>
        /// <param name="optionalHavingNode">expression node representing validated HAVING clause, or null if none given.Aggregation functions in the having node must have been pointed to the AggregationService for evaluation. </param>
        /// <param name="isSelectRStream">true if remove stream events should be generated</param>
        public ResultSetProcessorAggregateGrouped(SelectExprProcessor selectExprProcessor,
                                          		  OrderByProcessor orderByProcessor,
                                          		  AggregationService aggregationService,
                                          		  ICollection<ExprNode> groupKeyNodes,
                                          		  ExprNode optionalHavingNode,
                                                  bool isSelectRStream)
        {
            this.selectExprProcessor = selectExprProcessor;
            this.orderByProcessor = orderByProcessor;
            this.aggregationService = aggregationService;
            this.groupKeyNodes = groupKeyNodes;
            this.optionalHavingNode = optionalHavingNode;
            this.isSorting = orderByProcessor != null;
            this.isSelectRStream = isSelectRStream;
        }

        /// <summary>
        /// Returns the event type of processed results.
        /// </summary>
        /// <value>The type of the result event.</value>
        /// <returns> event type of the resulting events posted by the processor.
        /// </returns>
        public EventType ResultEventType
        {
            get { return selectExprProcessor.ResultEventType; }
        }

        /// <summary>
        /// For use by joins posting their result, process the event rows that are entered and removed (new and old events).
        /// Processes according to select-clauses, group-by clauses and having-clauses and returns new events and
        /// old events as specified.
        /// </summary>
        /// <param name="newEvents">new events posted by join</param>
        /// <param name="oldEvents">old events posted by join</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>pair of new events and old events</returns>
        public UniformPair<EventBean[]> ProcessJoinResult(Set<MultiKey<EventBean>> newEvents, Set<MultiKey<EventBean>> oldEvents, bool isSynthesize)
        {
            // Generate group-by keys for all events
            MultiKeyUntyped[] newDataGroupByKeys = GenerateGroupKeys(newEvents, true);
            MultiKeyUntyped[] oldDataGroupByKeys = GenerateGroupKeys(oldEvents, false);
    
            // generate old events
            if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
            {
                log.Debug(".processJoinResults creating old output events");
            }
    
            // update aggregates
            if (!newEvents.IsEmpty)
            {
                // apply old data to aggregates
                int count = 0;
                foreach (MultiKey<EventBean> eventsPerStream in newEvents)
                {
                    aggregationService.ApplyEnter(eventsPerStream.Array, newDataGroupByKeys[count]);
                    count++;
                }
            }
            if (!oldEvents.IsEmpty)
            {
                // apply old data to aggregates
                int count = 0;
                foreach (MultiKey<EventBean> eventsPerStream in oldEvents)
                {
                    aggregationService.ApplyLeave(eventsPerStream.Array, oldDataGroupByKeys[count]);
                    count++;
                }
            }
    
            EventBean[] selectOldEvents = null;
            if (isSelectRStream)
            {
                selectOldEvents = GenerateOutputEventsJoin(oldEvents, oldDataGroupByKeys, oldGenerators, false, isSynthesize);
            }
            EventBean[] selectNewEvents = GenerateOutputEventsJoin(newEvents, newDataGroupByKeys, newGenerators, true, isSynthesize);
    
            if ((selectNewEvents != null) || (selectOldEvents != null))
            {
                return new UniformPair<EventBean[]>(selectNewEvents, selectOldEvents);
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
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>pair of new events and old events</returns>
        public UniformPair<EventBean[]> ProcessViewResult(EventBean[] newData, EventBean[] oldData, bool isSynthesize)
        {
            // Generate group-by keys for all events
            MultiKeyUntyped[] newDataGroupByKeys = GenerateGroupKeys(newData, true);
            MultiKeyUntyped[] oldDataGroupByKeys = GenerateGroupKeys(oldData, false);
    
            // generate old events
            if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
            {
                log.Debug(".processViewResults creating old output events");
            }
    
            // update aggregates
            EventBean[] eventsPerStream = new EventBean[1];
            if (newData != null)
            {
                // apply new data to aggregates
                for (int i = 0; i < newData.Length; i++)
                {
                    eventsPerStream[0] = newData[i];
                    aggregationService.ApplyEnter(eventsPerStream, newDataGroupByKeys[i]);
                }
            }
            if (oldData != null)
            {
                // apply old data to aggregates
                for (int i = 0; i < oldData.Length; i++)
                {
                    eventsPerStream[0] = oldData[i];
                    aggregationService.ApplyLeave(eventsPerStream, oldDataGroupByKeys[i]);
                }
            }
    
            EventBean[] selectOldEvents = null;
            if (isSelectRStream)
            {
                selectOldEvents = GenerateOutputEventsView(oldData, oldDataGroupByKeys, oldGenerators, false, isSynthesize);
            }
            EventBean[] selectNewEvents = GenerateOutputEventsView(newData, newDataGroupByKeys, newGenerators, true, isSynthesize);
    
            if ((selectNewEvents != null) || (selectOldEvents != null))
            {
                return new UniformPair<EventBean[]>(selectNewEvents, selectOldEvents);
            }
            return null;
        }
    
    	private EventBean[] GenerateOutputEventsView(EventBean[] outputEvents, MultiKeyUntyped[] groupByKeys, Map<MultiKeyUntyped, EventBean[]> generators, bool isNewData, bool isSynthesize)
        {
            if (outputEvents == null)
            {
                return null;
            }
    
            EventBean[] eventsPerStream = new EventBean[1];
            EventBean[] events = new EventBean[outputEvents.Length];
            MultiKeyUntyped[] keys = new MultiKeyUntyped[outputEvents.Length];
            EventBean[][] currentGenerators = null;
            if(isSorting)
            {
            	currentGenerators = new EventBean[outputEvents.Length][];
            }
    
            int count = 0;
            for (int i = 0; i < outputEvents.Length; i++)
            {
                aggregationService.SetCurrentRow(groupByKeys[count]);
                eventsPerStream[0] = outputEvents[count];
    
                // Filter the having clause
                if (optionalHavingNode != null)
                {
                    bool? result = (bool?) optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                    if (!result ?? true)
                    {
                        continue;
                    }
                }
    
                events[count] = selectExprProcessor.Process(eventsPerStream, isNewData, isSynthesize);
                keys[count] = groupByKeys[count];
                if(isSorting)
                {
                	EventBean[] currentEventsPerStream = new EventBean[] { outputEvents[count] };
                	generators.Put(keys[count], currentEventsPerStream);
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
                EventBean[] @out = new EventBean[count];
                Array.Copy(events, 0, @out, 0, count);
                events = @out;
    
                if(isSorting)
                {
                	MultiKeyUntyped[] outKeys = new MultiKeyUntyped[count];
                	Array.Copy(keys, 0, outKeys, 0, count);
                	keys = outKeys;
    
                	EventBean[][] outGens = new EventBean[count][];
                	Array.Copy(currentGenerators, 0, outGens, 0, count);
                	currentGenerators = outGens;
                }
            }
    
            if(isSorting)
            {
                events = orderByProcessor.Sort(events, currentGenerators, keys, isNewData);
            }
    
            return events;
        }
    
        private MultiKeyUntyped[] GenerateGroupKeys(ICollection<MultiKey<EventBean>> resultSet, bool isNewData)
        {
            if (CollectionHelper.IsEmpty(resultSet))
            {
                return null;
            }
    
            MultiKeyUntyped[] keys = new MultiKeyUntyped[resultSet.Count];
    
            int count = 0;
            foreach (MultiKey<EventBean> eventsPerStream in resultSet)
            {
                keys[count] = GenerateGroupKey(eventsPerStream.Array, isNewData);
                count++;
            }
    
            return keys;
        }
    
        private MultiKeyUntyped[] GenerateGroupKeys(EventBean[] events, bool isNewData)
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
                keys[i] = GenerateGroupKey(eventsPerStream, isNewData);
            }
    
            return keys;
        }
    
        /// <summary>Generates the group-by key for the row </summary>
        /// <param name="eventsPerStream">is the row of events</param>
        /// <param name="isNewData">is true for new data</param>
        /// <returns>grouping keys</returns>
        protected MultiKeyUntyped GenerateGroupKey(EventBean[] eventsPerStream, bool isNewData)
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
    
        private EventBean[] GenerateOutputEventsJoin(ICollection<MultiKey<EventBean>> resultSet, MultiKeyUntyped[] groupByKeys, Map<MultiKeyUntyped, EventBean[]> generators, bool isNewData, bool isSynthesize)
        {
            if (CollectionHelper.IsEmpty(resultSet))
            {
                return null;
            }
    
            EventBean[] events = new EventBean[resultSet.Count];
            MultiKeyUntyped[] keys = new MultiKeyUntyped[resultSet.Count];
            EventBean[][] currentGenerators = null;
            if(isSorting)
            {
            	currentGenerators = new EventBean[resultSet.Count][];
            }
    
            int count = 0;
            foreach (MultiKey<EventBean> row in resultSet)
            {
                EventBean[] eventsPerStream = row.Array;
    
                aggregationService.SetCurrentRow(groupByKeys[count]);
    
                // Filter the having clause
                if (optionalHavingNode != null)
                {
                    bool? result = (bool?) optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                    if (!result ?? true)
                    {
                        continue;
                    }
                }
    
                events[count] = selectExprProcessor.Process(eventsPerStream, isNewData, isSynthesize);
                keys[count] = groupByKeys[count];
                if(isSorting)
                {
                	generators.Put(keys[count], eventsPerStream);
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
                EventBean[] @out = new EventBean[count];
                Array.Copy(events, 0, @out, 0, count);
                events = @out;
    
                if(isSorting)
                {
                	MultiKeyUntyped[] outKeys = new MultiKeyUntyped[count];
                	Array.Copy(keys, 0, outKeys, 0, count);
                	keys = outKeys;
    
                	EventBean[][] outGens = new EventBean[count][];
                	Array.Copy(currentGenerators, 0, outGens, 0, count);
                	currentGenerators = outGens;
                }
            }
    
            if(isSorting)
            {
                events = orderByProcessor.Sort(events, currentGenerators, keys, isNewData);
            }
            return events;
        }
    
        public IEnumerator<EventBean> GetEnumerator(Viewable parent)
        {
            if (orderByProcessor == null)
            {
                return EnumerateResultSet(parent);
                //return new ResultSetAggregateGroupedIterator(parent.GetEnumerator(), this, aggregationService);
            }
    
            // Pull all parent events, generate order keys
            EventBean[] eventsPerStream = new EventBean[1];
            List<EventBean> outgoingEvents = new List<EventBean>();
            List<MultiKeyUntyped> orderKeys = new List<MultiKeyUntyped>();
    
            foreach (EventBean candidate in parent) {
                eventsPerStream[0] = candidate;
    
                MultiKeyUntyped groupKey = GenerateGroupKey(eventsPerStream, true);
                aggregationService.SetCurrentRow(groupKey);
    
                bool? pass = true;
                if (optionalHavingNode != null) {
                    pass = (bool?) optionalHavingNode.Evaluate(eventsPerStream, true);
                }
                if (!pass ?? true)
                {
                    continue;
                }
    
                outgoingEvents.Add(selectExprProcessor.Process(eventsPerStream, true, true));
    
                MultiKeyUntyped orderKey = orderByProcessor.GetSortKey(eventsPerStream, true);
                orderKeys.Add(orderKey);
            }
    
            // sort
            EventBean[] outgoingEventsArr = outgoingEvents.ToArray();
            MultiKeyUntyped[] orderKeysArr = orderKeys.ToArray();
            EventBean[] orderedEvents = orderByProcessor.Sort(outgoingEventsArr, orderKeysArr);

            return ((IList<EventBean>) orderedEvents).GetEnumerator();
        }

        private IEnumerator<EventBean> EnumerateResultSet(IEnumerable<EventBean> baseEnum)
        {
            EventBean[] eventArray = new EventBean[1];

            BeanEvaluator baseEval = delegate { return selectExprProcessor.Process(eventArray, true, true); };
            BeanEvaluator beanEval = baseEval;
            if (optionalHavingNode != null)
            {
                beanEval = delegate(EventBean eventBean)
                {
                    bool? result = (bool?)optionalHavingNode.Evaluate(eventArray, true);
                    return (result ?? false) ? baseEval.Invoke(eventBean) : null;
                };
            }

            foreach (EventBean eventBean in baseEnum)
            {
                eventArray[0] = eventBean;
                // Set the groupKey on the aggregation service
                MultiKeyUntyped groupKey = GenerateGroupKey(eventArray, true);
                aggregationService.SetCurrentRow(groupKey);
                // Check the eventBean
                EventBean rEventBean = beanEval.Invoke(eventBean);
                if (rEventBean != null)
                {
                    yield return rEventBean;
                }
            }
        }

        /// <summary>Returns the select expression processor </summary>
        /// <returns>select processor.</returns>
        public SelectExprProcessor SelectExprProcessor
        {
            get { return selectExprProcessor; }
        }

        /// <summary>Returns the having node. </summary>
        /// <returns>having expression</returns>
        public ExprNode OptionalHavingNode
        {
            get { return optionalHavingNode; }
        }

        public IEnumerator<EventBean> GetEnumerator(Set<MultiKey<EventBean>> joinSet)
        {
            // Generate group-by keys for all events
            MultiKeyUntyped[] groupByKeys = GenerateGroupKeys(joinSet, true);
            EventBean[] result = GenerateOutputEventsJoin(joinSet, groupByKeys, newGenerators, true, true);
            if (result == null) return EnumerationHelper<EventBean>.CreateEmptyEnumerator();
            return ((IList<EventBean>) result).GetEnumerator();
        }
    
        public void Clear()
        {
            aggregationService.ClearResults();
        }

        /// <summary>Processes batched events in case of output-rate limiting.</summary>
        /// <param name="joinEventsSet">the join results</param>
        /// <param name="generateSynthetic">flag to indicate whether synthetic events must be generated</param>
        /// <param name="outputLimitLimitType">the type of output rate limiting</param>
        /// <returns>results for dispatch</returns>
        public UniformPair<EventBean[]> ProcessOutputLimitedJoin(IList<UniformPair<Set<MultiKey<EventBean>>>> joinEventsSet, bool generateSynthetic, OutputLimitLimitType outputLimitLimitType)
        {
            if (outputLimitLimitType == OutputLimitLimitType.DEFAULT)
            {
                List<EventBean> newEvents = new List<EventBean>();
                List<EventBean> oldEvents = null;
                if (isSelectRStream)
                {
                     oldEvents = new List<EventBean>();
                }
    
                List<MultiKeyUntyped> newEventsSortKey = null;
                List<MultiKeyUntyped> oldEventsSortKey = null;
                if (orderByProcessor != null)
                {
                    newEventsSortKey = new List<MultiKeyUntyped>();
                    if (isSelectRStream)
                    {
                        oldEventsSortKey = new List<MultiKeyUntyped>();
                    }
                }
    
                foreach (UniformPair<Set<MultiKey<EventBean>>> pair in joinEventsSet)
                {
                    Set<MultiKey<EventBean>> newData = pair.First;
                    Set<MultiKey<EventBean>> oldData = pair.Second;
    
                    MultiKeyUntyped[] newDataMultiKey = GenerateGroupKeys(newData, true);
                    MultiKeyUntyped[] oldDataMultiKey = GenerateGroupKeys(oldData, false);
    
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        int count = 0;
                        foreach (MultiKey<EventBean> aNewData in newData)
                        {
                            aggregationService.ApplyEnter(aNewData.Array, newDataMultiKey[count]);
                            count++;
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        int count = 0;
                        foreach (MultiKey<EventBean> anOldData in oldData)
                        {
                            aggregationService.ApplyLeave(anOldData.Array, oldDataMultiKey[count]);
                            count++;
                        }
                    }
    
                    if (isSelectRStream)
                    {
                        GenerateOutputBatchedJoin(oldData, oldDataMultiKey, false, generateSynthetic, oldEvents, oldEventsSortKey);
                    }
                    GenerateOutputBatchedJoin(newData, newDataMultiKey, true, generateSynthetic, newEvents, newEventsSortKey);
                }
    
                EventBean[] newEventsArr = CollectionHelper.IsEmpty(newEvents) ? null : newEvents.ToArray();
                EventBean[] oldEventsArr = null;
                if (isSelectRStream)
                {
                    oldEventsArr = CollectionHelper.IsEmpty(oldEvents) ? null : oldEvents.ToArray();
                }
    
                if (orderByProcessor != null)
                {
                    MultiKeyUntyped[] sortKeysNew = CollectionHelper.IsEmpty(newEventsSortKey) ? null : newEventsSortKey.ToArray();
                    newEventsArr = orderByProcessor.Sort(newEventsArr, sortKeysNew);
                    if (isSelectRStream)
                    {
                        MultiKeyUntyped[] sortKeysOld = CollectionHelper.IsEmpty(oldEventsSortKey) ? null : oldEventsSortKey.ToArray();
                        oldEventsArr = orderByProcessor.Sort(oldEventsArr, sortKeysOld);
                    }
                }
    
                if ((newEventsArr == null) && (oldEventsArr == null))
                {
                    return null;
                }
                return new UniformPair<EventBean[]>(newEventsArr, oldEventsArr);
            }
            else if (outputLimitLimitType == OutputLimitLimitType.ALL)
            {
                List<EventBean> newEvents = new List<EventBean>();
                List<EventBean> oldEvents = null;
                if (isSelectRStream)
                {
                    oldEvents = new List<EventBean>();
                }
    
                List<MultiKeyUntyped> newEventsSortKey = null;
                List<MultiKeyUntyped> oldEventsSortKey = null;
                if (orderByProcessor != null)
                {
                    newEventsSortKey = new List<MultiKeyUntyped>();
                    if (isSelectRStream)
                    {
                        oldEventsSortKey = new List<MultiKeyUntyped>();
                    }
                }
    
                workCollection.Clear();
    
                foreach (UniformPair<Set<MultiKey<EventBean>>> pair in joinEventsSet)
                {
                    Set<MultiKey<EventBean>> newData = pair.First;
                    Set<MultiKey<EventBean>> oldData = pair.Second;
    
                    MultiKeyUntyped[] newDataMultiKey = GenerateGroupKeys(newData, true);
                    MultiKeyUntyped[] oldDataMultiKey = GenerateGroupKeys(oldData, false);
    
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        int count = 0;
                        foreach (MultiKey<EventBean> aNewData in newData)
                        {
                            MultiKeyUntyped mk = newDataMultiKey[count];
                            aggregationService.ApplyEnter(aNewData.Array, mk);
                            count++;
    
                            // keep the new event as a representative for the group
                            workCollection.Put(mk, aNewData.Array);
                            eventGroupReps.Put(mk, aNewData.Array);
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        int count = 0;
                        foreach (MultiKey<EventBean> anOldData in oldData)
                        {
                            aggregationService.ApplyLeave(anOldData.Array, oldDataMultiKey[count]);
                            count++;
                        }
                    }
    
                    if (isSelectRStream)
                    {
                        GenerateOutputBatchedJoin(oldData, oldDataMultiKey, false, generateSynthetic, oldEvents, oldEventsSortKey);
                    }
                    GenerateOutputBatchedJoin(newData, newDataMultiKey, true, generateSynthetic, newEvents, newEventsSortKey);
                }
    
                // For any group representatives not in the work collection, generate a row
                foreach (KeyValuePair<MultiKeyUntyped, EventBean[]> entry in eventGroupReps)
                {
                    if (!workCollection.ContainsKey(entry.Key))
                    {
                        workCollectionTwo.Put(entry.Key, entry.Value);
                        GenerateOutputBatchedArr(workCollectionTwo, true, generateSynthetic, newEvents, newEventsSortKey);
                        workCollectionTwo.Clear();
                    }
                }
    
                EventBean[] newEventsArr = CollectionHelper.IsEmpty(newEvents) ? null : newEvents.ToArray();
                EventBean[] oldEventsArr = null;
                if (isSelectRStream)
                {
                    oldEventsArr = CollectionHelper.IsEmpty(oldEvents) ? null : oldEvents.ToArray();
                }
    
                if (orderByProcessor != null)
                {
                    MultiKeyUntyped[] sortKeysNew = CollectionHelper.IsEmpty(newEventsSortKey) ? null : newEventsSortKey.ToArray();
                    newEventsArr = orderByProcessor.Sort(newEventsArr, sortKeysNew);
                    if (isSelectRStream)
                    {
                        MultiKeyUntyped[] sortKeysOld = CollectionHelper.IsEmpty(oldEventsSortKey) ? null : oldEventsSortKey.ToArray();
                        oldEventsArr = orderByProcessor.Sort(oldEventsArr, sortKeysOld);
                    }
                }
    
                if ((newEventsArr == null) && (oldEventsArr == null))
                {
                    return null;
                }
                return new UniformPair<EventBean[]>(newEventsArr, oldEventsArr);
            }
            else // (outputLimitLimitType == OutputLimitLimitType.LAST) Compute last per group
            {
                Map<MultiKeyUntyped, EventBean> lastPerGroupNew = new LinkedHashMap<MultiKeyUntyped, EventBean>();
                Map<MultiKeyUntyped, EventBean> lastPerGroupOld = null;
                if (isSelectRStream)
                {
                    lastPerGroupOld = new LinkedHashMap<MultiKeyUntyped, EventBean>();
                }
    
                Map<MultiKeyUntyped, MultiKeyUntyped> newEventsSortKey = null; // group key to sort key
                Map<MultiKeyUntyped, MultiKeyUntyped> oldEventsSortKey = null;
                if (orderByProcessor != null)
                {
                    newEventsSortKey = new LinkedHashMap<MultiKeyUntyped, MultiKeyUntyped>();
                    if (isSelectRStream)
                    {
                        oldEventsSortKey = new LinkedHashMap<MultiKeyUntyped, MultiKeyUntyped>();
                    }
                }
    
                foreach (UniformPair<Set<MultiKey<EventBean>>> pair in joinEventsSet)
                {
                    Set<MultiKey<EventBean>> newData = pair.First;
                    Set<MultiKey<EventBean>> oldData = pair.Second;
    
                    MultiKeyUntyped[] newDataMultiKey = GenerateGroupKeys(newData, true);
                    MultiKeyUntyped[] oldDataMultiKey = GenerateGroupKeys(oldData, false);
    
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        int count = 0;
                        foreach (MultiKey<EventBean> aNewData in newData)
                        {
                            MultiKeyUntyped mk = newDataMultiKey[count];
                            aggregationService.ApplyEnter(aNewData.Array, mk);
                            count++;
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        int count = 0;
                        foreach (MultiKey<EventBean> anOldData in oldData)
                        {
                            workCollection.Put(oldDataMultiKey[count], anOldData.Array);
                            aggregationService.ApplyLeave(anOldData.Array, oldDataMultiKey[count]);
                            count++;
                        }
                    }
    
                    if (isSelectRStream)
                    {
                        GenerateOutputBatchedJoin(oldData, oldDataMultiKey, false, generateSynthetic, lastPerGroupOld, oldEventsSortKey);
                    }
                    GenerateOutputBatchedJoin(newData, newDataMultiKey, false, generateSynthetic, lastPerGroupNew, newEventsSortKey);
                }

                EventBean[] newEventsArr = CollectionHelper.IsEmpty(lastPerGroupNew)
                                               ? null
                                               : CollectionHelper.ToArray(lastPerGroupNew.Values);
                EventBean[] oldEventsArr = null;
                if (isSelectRStream)
                {
                    oldEventsArr = CollectionHelper.IsEmpty(lastPerGroupOld)
                                       ? null
                                       : CollectionHelper.ToArray(lastPerGroupOld.Values);
                }
    
                if (orderByProcessor != null)
                {
                    MultiKeyUntyped[] sortKeysNew = CollectionHelper.IsEmpty(newEventsSortKey)
                                                        ? null
                                                        : CollectionHelper.ToArray(newEventsSortKey.Values);
                    newEventsArr = orderByProcessor.Sort(newEventsArr, sortKeysNew);
                    if (isSelectRStream)
                    {
                        MultiKeyUntyped[] sortKeysOld = CollectionHelper.IsEmpty(oldEventsSortKey)
                                                            ? null
                                                            : CollectionHelper.ToArray(oldEventsSortKey.Values);
                        oldEventsArr = orderByProcessor.Sort(oldEventsArr, sortKeysOld);
                    }
                }
    
                if ((newEventsArr == null) && (oldEventsArr == null))
                {
                    return null;
                }
                return new UniformPair<EventBean[]>(newEventsArr, oldEventsArr);
            }
        }

        /// <summary>Processes batched events in case of output-rate limiting.</summary>
        /// <param name="viewEventsList">the view results</param>
        /// <param name="generateSynthetic">flag to indicate whether synthetic events must be generated</param>
        /// <param name="outputLimitLimitType">the type of output rate limiting</param>
        /// <returns>results for dispatch</returns>
        public UniformPair<EventBean[]> ProcessOutputLimitedView(IList<UniformPair<EventBean[]>> viewEventsList, bool generateSynthetic, OutputLimitLimitType outputLimitLimitType)
        {
            EventBean[] eventsPerStream = new EventBean[1];
    
            if (outputLimitLimitType == OutputLimitLimitType.DEFAULT)
            {
                List<EventBean> newEvents = new List<EventBean>();
                List<EventBean> oldEvents = null;
                if (isSelectRStream)
                {
                    oldEvents = new List<EventBean>();
                }
    
                List<MultiKeyUntyped> newEventsSortKey = null;
                List<MultiKeyUntyped> oldEventsSortKey = null;
                if (orderByProcessor != null)
                {
                    newEventsSortKey = new List<MultiKeyUntyped>();
                    if (isSelectRStream)
                    {
                        oldEventsSortKey = new List<MultiKeyUntyped>();
                    }
                }
    
                foreach (UniformPair<EventBean[]> pair in viewEventsList)
                {
                    EventBean[] newData = pair.First;
                    EventBean[] oldData = pair.Second;
    
                    MultiKeyUntyped[] newDataMultiKey = GenerateGroupKeys(newData, true);
                    MultiKeyUntyped[] oldDataMultiKey = GenerateGroupKeys(oldData, false);
    
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        int count = 0;
                        foreach (EventBean aNewData in newData)
                        {
                            eventsPerStream[0] = aNewData;
                            aggregationService.ApplyEnter(eventsPerStream, newDataMultiKey[count]);
                            count++;
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        int count = 0;
                        foreach (EventBean anOldData in oldData)
                        {
                            eventsPerStream[0] = anOldData;
                            aggregationService.ApplyLeave(eventsPerStream, oldDataMultiKey[count]);
                            count++;
                        }
                    }
    
                    if (isSelectRStream)
                    {
                        GenerateOutputBatchedView(oldData, oldDataMultiKey, false, generateSynthetic, oldEvents, oldEventsSortKey);
                    }
                    GenerateOutputBatchedView(newData, newDataMultiKey, true, generateSynthetic, newEvents, newEventsSortKey);
                }
    
                EventBean[] newEventsArr = CollectionHelper.IsEmpty(newEvents) ? null : newEvents.ToArray();
                EventBean[] oldEventsArr = null;
                if (isSelectRStream)
                {
                    oldEventsArr = CollectionHelper.IsEmpty(oldEvents) ? null : oldEvents.ToArray();
                }
    
                if (orderByProcessor != null)
                {
                    MultiKeyUntyped[] sortKeysNew = CollectionHelper.IsEmpty(newEventsSortKey) ? null : newEventsSortKey.ToArray();
                    newEventsArr = orderByProcessor.Sort(newEventsArr, sortKeysNew);
                    if (isSelectRStream)
                    {
                        MultiKeyUntyped[] sortKeysOld = CollectionHelper.IsEmpty(oldEventsSortKey) ? null : oldEventsSortKey.ToArray();
                        oldEventsArr = orderByProcessor.Sort(oldEventsArr, sortKeysOld);
                    }
                }
    
                if ((newEventsArr == null) && (oldEventsArr == null))
                {
                    return null;
                }
                return new UniformPair<EventBean[]>(newEventsArr, oldEventsArr);
            }
            else if (outputLimitLimitType == OutputLimitLimitType.ALL)
            {
                List<EventBean> newEvents = new List<EventBean>();
                List<EventBean> oldEvents = null;
                if (isSelectRStream)
                {
                    oldEvents = new List<EventBean>();
                }
    
                List<MultiKeyUntyped> newEventsSortKey = null;
                List<MultiKeyUntyped> oldEventsSortKey = null;
                if (orderByProcessor != null)
                {
                    newEventsSortKey = new List<MultiKeyUntyped>();
                    if (isSelectRStream)
                    {
                        oldEventsSortKey = new List<MultiKeyUntyped>();
                    }
                }
    
                workCollection.Clear();
    
                foreach (UniformPair<EventBean[]> pair in viewEventsList)
                {
                    EventBean[] newData = pair.First;
                    EventBean[] oldData = pair.Second;
    
                    MultiKeyUntyped[] newDataMultiKey = GenerateGroupKeys(newData, true);
                    MultiKeyUntyped[] oldDataMultiKey = GenerateGroupKeys(oldData, false);
    
                    eventsPerStream = new EventBean[1];
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        int count = 0;
                        foreach (EventBean aNewData in newData)
                        {
                            MultiKeyUntyped mk = newDataMultiKey[count];
                            eventsPerStream[0] = aNewData;
                            aggregationService.ApplyEnter(eventsPerStream, mk);
                            count++;
    
                            // keep the new event as a representative for the group
                            workCollection.Put(mk, eventsPerStream);
                            eventGroupReps.Put(mk, new EventBean[] {aNewData});
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        int count = 0;
                        foreach (EventBean anOldData in oldData)
                        {
                            eventsPerStream[0] = anOldData;
                            aggregationService.ApplyLeave(eventsPerStream, oldDataMultiKey[count]);
                            count++;
                        }
                    }
    
                    if (isSelectRStream)
                    {
                        GenerateOutputBatchedView(oldData, oldDataMultiKey, false, generateSynthetic, oldEvents, oldEventsSortKey);
                    }
                    GenerateOutputBatchedView(newData, newDataMultiKey, true, generateSynthetic, newEvents, newEventsSortKey);
                }
    
                // For any group representatives not in the work collection, generate a row
                foreach (KeyValuePair<MultiKeyUntyped, EventBean[]> entry in eventGroupReps)
                {
                    if (!workCollection.ContainsKey(entry.Key))
                    {
                        workCollectionTwo.Put(entry.Key, entry.Value);
                        GenerateOutputBatchedArr(workCollectionTwo, true, generateSynthetic, newEvents, newEventsSortKey);
                        workCollectionTwo.Clear();
                    }
                }
    
                EventBean[] newEventsArr = CollectionHelper.IsEmpty(newEvents) ? null : newEvents.ToArray();
                EventBean[] oldEventsArr = null;
                if (isSelectRStream)
                {
                    oldEventsArr = CollectionHelper.IsEmpty(oldEvents) ? null : oldEvents.ToArray();
                }
    
                if (orderByProcessor != null)
                {
                    MultiKeyUntyped[] sortKeysNew = CollectionHelper.IsEmpty(newEventsSortKey) ? null : newEventsSortKey.ToArray();
                    newEventsArr = orderByProcessor.Sort(newEventsArr, sortKeysNew);
                    if (isSelectRStream)
                    {
                        MultiKeyUntyped[] sortKeysOld = CollectionHelper.IsEmpty(oldEventsSortKey) ? null : oldEventsSortKey.ToArray();
                        oldEventsArr = orderByProcessor.Sort(oldEventsArr, sortKeysOld);
                    }
                }
    
                if ((newEventsArr == null) && (oldEventsArr == null))
                {
                    return null;
                }
                return new UniformPair<EventBean[]>(newEventsArr, oldEventsArr);
            }
            else // (outputLimitLimitType == OutputLimitLimitType.LAST) Compute last per group
            {
                Map<MultiKeyUntyped, EventBean> lastPerGroupNew = new LinkedHashMap<MultiKeyUntyped, EventBean>();
                Map<MultiKeyUntyped, EventBean> lastPerGroupOld = null;
                if (isSelectRStream)
                {
                    lastPerGroupOld = new LinkedHashMap<MultiKeyUntyped, EventBean>();
                }
    
                Map<MultiKeyUntyped, MultiKeyUntyped> newEventsSortKey = null; // group key to sort key
                Map<MultiKeyUntyped, MultiKeyUntyped> oldEventsSortKey = null;
                if (orderByProcessor != null)
                {
                    newEventsSortKey = new LinkedHashMap<MultiKeyUntyped, MultiKeyUntyped>();
                    if (isSelectRStream)
                    {
                        oldEventsSortKey = new LinkedHashMap<MultiKeyUntyped, MultiKeyUntyped>();
                    }
                }
    
                foreach (UniformPair<EventBean[]> pair in viewEventsList)
                {
                    EventBean[] newData = pair.First;
                    EventBean[] oldData = pair.Second;
    
                    MultiKeyUntyped[] newDataMultiKey = GenerateGroupKeys(newData, true);
                    MultiKeyUntyped[] oldDataMultiKey = GenerateGroupKeys(oldData, false);
    
                    eventsPerStream = new EventBean[1];
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        int count = 0;
                        foreach (EventBean aNewData in newData)
                        {
                            MultiKeyUntyped mk = newDataMultiKey[count];
                            eventsPerStream[0] = aNewData;
                            aggregationService.ApplyEnter(eventsPerStream, mk);
                            count++;
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        int count = 0;
                        foreach (EventBean anOldData in oldData)
                        {
                            workCollection.Put(oldDataMultiKey[count], eventsPerStream);
                            eventsPerStream[0] = anOldData;
                            aggregationService.ApplyLeave(eventsPerStream, oldDataMultiKey[count]);
                            count++;
                        }
                    }
    
                    if (isSelectRStream)
                    {
                        GenerateOutputBatchedView(oldData, oldDataMultiKey, false, generateSynthetic, lastPerGroupOld, oldEventsSortKey);
                    }
                    GenerateOutputBatchedView(newData, newDataMultiKey, false, generateSynthetic, lastPerGroupNew, newEventsSortKey);
                }

                EventBean[] newEventsArr = CollectionHelper.IsEmpty(lastPerGroupNew)
                                               ? null
                                               : CollectionHelper.ToArray(lastPerGroupNew.Values);
                EventBean[] oldEventsArr = null;
                if (isSelectRStream)
                {
                    oldEventsArr = CollectionHelper.IsEmpty(lastPerGroupOld)
                                       ? null
                                       : CollectionHelper.ToArray(lastPerGroupOld.Values);
                }
    
                if (orderByProcessor != null)
                {
                    MultiKeyUntyped[] sortKeysNew = CollectionHelper.IsEmpty(newEventsSortKey)
                                                        ? null
                                                        : CollectionHelper.ToArray(newEventsSortKey.Values);
                    newEventsArr = orderByProcessor.Sort(newEventsArr, sortKeysNew);
                    if (isSelectRStream)
                    {
                        MultiKeyUntyped[] sortKeysOld = CollectionHelper.IsEmpty(oldEventsSortKey)
                                                            ? null
                                                            : CollectionHelper.ToArray(oldEventsSortKey.Values);
                        oldEventsArr = orderByProcessor.Sort(oldEventsArr, sortKeysOld);
                    }
                }
    
                if ((newEventsArr == null) && (oldEventsArr == null))
                {
                    return null;
                }
                return new UniformPair<EventBean[]>(newEventsArr, oldEventsArr);
            }
        }
    
        private void GenerateOutputBatchedArr(IEnumerable<KeyValuePair<MultiKeyUntyped, EventBean[]>> keysAndEvents, bool isNewData, bool isSynthesize, ICollection<EventBean> resultEvents, ICollection<MultiKeyUntyped> optSortKeys)
        {
            foreach (KeyValuePair<MultiKeyUntyped, EventBean[]> entry in keysAndEvents)
            {
                EventBean[] eventsPerStream = entry.Value;
    
                // Set the current row of aggregation states
                aggregationService.SetCurrentRow(entry.Key);
    
                // Filter the having clause
                if (optionalHavingNode != null)
                {
                    bool? result = (bool?) optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                    if (!result ?? true)
                    {
                        continue;
                    }
                }
    
                resultEvents.Add(selectExprProcessor.Process(eventsPerStream, isNewData, isSynthesize));
    
                if(isSorting)
                {
                    optSortKeys.Add(orderByProcessor.GetSortKey(eventsPerStream, isNewData));
                }
            }
        }
    
        private void GenerateOutputBatchedView(EventBean[] outputEvents, MultiKeyUntyped[] groupByKeys, bool isNewData, bool isSynthesize, ICollection<EventBean> resultEvents, ICollection<MultiKeyUntyped> optSortKeys)
        {
            if (outputEvents == null)
            {
                return;
            }
    
            EventBean[] eventsPerStream = new EventBean[1];
    
            int count = 0;
            for (int i = 0; i < outputEvents.Length; i++)
            {
                aggregationService.SetCurrentRow(groupByKeys[count]);
                eventsPerStream[0] = outputEvents[count];
    
                // Filter the having clause
                if (optionalHavingNode != null)
                {
                    bool? result = (bool?) optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                    if (!result ?? true)
                    {
                        continue;
                    }
                }
    
                resultEvents.Add(selectExprProcessor.Process(eventsPerStream, isNewData, isSynthesize));
                if(isSorting)
                {
                    optSortKeys.Add(orderByProcessor.GetSortKey(eventsPerStream, isNewData));
                }
    
                count++;
            }
        }
    
        private void GenerateOutputBatchedJoin(IEnumerable<MultiKey<EventBean>> outputEvents, MultiKeyUntyped[] groupByKeys, bool isNewData, bool isSynthesize, ICollection<EventBean> resultEvents, ICollection<MultiKeyUntyped> optSortKeys)
        {
            if (outputEvents == null)
            {
                return;
            }

            int count = 0;
            foreach (MultiKey<EventBean> row in outputEvents)
            {
                aggregationService.SetCurrentRow(groupByKeys[count]);
                EventBean[] eventsPerStream = row.Array;
    
                // Filter the having clause
                if (optionalHavingNode != null)
                {
                    bool? result = (bool?) optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                    if (!result ?? true)
                    {
                        continue;
                    }
                }
    
                resultEvents.Add(selectExprProcessor.Process(eventsPerStream, isNewData, isSynthesize));
                if(isSorting)
                {
                    optSortKeys.Add(orderByProcessor.GetSortKey(eventsPerStream, isNewData));
                }
    
                count++;
            }
        }
    
        private void GenerateOutputBatchedView(EventBean[] outputEvents, MultiKeyUntyped[] groupByKeys, bool isNewData, bool isSynthesize, Map<MultiKeyUntyped, EventBean> resultEvents, Map<MultiKeyUntyped, MultiKeyUntyped> optSortKeys)
        {
            if (outputEvents == null)
            {
                return;
            }
    
            EventBean[] eventsPerStream = new EventBean[1];
    
            int count = 0;
            for (int i = 0; i < outputEvents.Length; i++)
            {
                MultiKeyUntyped groupKey = groupByKeys[count];
                aggregationService.SetCurrentRow(groupKey);
                eventsPerStream[0] = outputEvents[count];
    
                // Filter the having clause
                if (optionalHavingNode != null)
                {
                    bool? result = (bool?)optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                    if (!result ?? true)
                    {
                        continue;
                    }
                }
    
                resultEvents.Put(groupKey, selectExprProcessor.Process(eventsPerStream, isNewData, isSynthesize));
                if(isSorting)
                {
                    optSortKeys.Put(groupKey, orderByProcessor.GetSortKey(eventsPerStream, isNewData));
                }
    
                count++;
            }
        }
    
        private void GenerateOutputBatchedJoin(IEnumerable<MultiKey<EventBean>> outputEvents, MultiKeyUntyped[] groupByKeys, bool isNewData, bool isSynthesize, Map<MultiKeyUntyped, EventBean> resultEvents, Map<MultiKeyUntyped, MultiKeyUntyped> optSortKeys)
        {
            if (outputEvents == null)
            {
                return;
            }
    
            int count = 0;
            foreach (MultiKey<EventBean> row in outputEvents)
            {
                MultiKeyUntyped groupKey = groupByKeys[count];
                aggregationService.SetCurrentRow(groupKey);
    
                // Filter the having clause
                if (optionalHavingNode != null)
                {
                    bool? result = (bool?) optionalHavingNode.Evaluate(row.Array, isNewData);
                    if (!result ?? true)
                    {
                        continue;
                    }
                }
    
                resultEvents.Put(groupKey, selectExprProcessor.Process(row.Array, isNewData, isSynthesize));
                if(isSorting)
                {
                    optSortKeys.Put(groupKey, orderByProcessor.GetSortKey(row.Array, isNewData));
                }
    
                count++;
            }
        }
    }
}
