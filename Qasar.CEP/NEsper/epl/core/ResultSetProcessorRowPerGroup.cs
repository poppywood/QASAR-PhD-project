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
using com.espertech.esper.view;


namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Result set processor for the fully-grouped case: there is a group-by and all non-aggregation
    /// event properties in the select clause are listed in the group by, and there are aggregation
    /// functions.
    /// <para/>
    /// Produces one row for each group that changed (and not one row per event). Computes MultiKey
    /// group-by keys for each event and uses a set of the group-by keys to generate the result rows,
    /// using the first (old or new, anyone) event for each distinct group-by key.
    /// </summary>
    public class ResultSetProcessorRowPerGroup : ResultSetProcessor
    {
        private readonly SelectExprProcessor selectExprProcessor;
        private readonly OrderByProcessor orderByProcessor;
        private readonly AggregationService aggregationService;
        private readonly IList<ExprNode> groupKeyNodes;
        private readonly ExprNode optionalHavingNode;
        private readonly bool isSorting;
        private readonly bool isSelectRStream;
    
        // For output rate limiting, keep a representative event for each group for
        // representing each group in an output limit clause
        private readonly Map<MultiKeyUntyped, EventBean[]> groupRepsView = new LinkedHashMap<MultiKeyUntyped, EventBean[]>();
        private readonly Map<MultiKeyUntyped, EventBean[]> workCollection = new LinkedHashMap<MultiKeyUntyped, EventBean[]>();
    
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
        public ResultSetProcessorRowPerGroup(SelectExprProcessor selectExprProcessor,
                                             OrderByProcessor orderByProcessor,
                                             AggregationService aggregationService,
                                             IList<ExprNode> groupKeyNodes,
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
    
        public EventType ResultEventType
        {
            get { return selectExprProcessor.ResultEventType; }
        }

        public UniformPair<EventBean[]> ProcessJoinResult(Set<MultiKey<EventBean>> newEvents,
                                                          Set<MultiKey<EventBean>> oldEvents,
                                                          bool isSynthesize)
        {
            // Generate group-by keys for all events, collect all keys in a set for later event generation
            Map<MultiKeyUntyped, EventBean[]> keysAndEvents = new HashMap<MultiKeyUntyped, EventBean[]>();
            MultiKeyUntyped[] newDataMultiKey = GenerateGroupKeys(newEvents, keysAndEvents, true);
            MultiKeyUntyped[] oldDataMultiKey = GenerateGroupKeys(oldEvents, keysAndEvents, false);
    
            // generate old events
            EventBean[] selectOldEvents = null;
            if (isSelectRStream)
            {
                selectOldEvents = GenerateOutputEventsJoin(keysAndEvents, oldGenerators, false, isSynthesize);
            }
    
            // update aggregates
            if (CollectionHelper.IsNotEmpty(newEvents))
            {
                // apply old data to aggregates
                int count = 0;
                foreach (MultiKey<EventBean> eventsPerStream in newEvents)
                {
                    aggregationService.ApplyEnter(eventsPerStream.Array, newDataMultiKey[count]);
                    count++;
                }
            }
            if (CollectionHelper.IsNotEmpty(oldEvents))
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
            EventBean[] selectNewEvents = GenerateOutputEventsJoin(keysAndEvents, newGenerators, true, isSynthesize);
    
            if ((selectNewEvents != null) || (selectOldEvents != null))
            {
                return new UniformPair<EventBean[]>(selectNewEvents, selectOldEvents);
            }
            return null;
        }

        public UniformPair<EventBean[]> ProcessViewResult(EventBean[] newData, EventBean[] oldData, bool isSynthesize)
        {
            // Generate group-by keys for all events, collect all keys in a set for later event generation
            Map<MultiKeyUntyped, EventBean> keysAndEvents = new HashMap<MultiKeyUntyped, EventBean>();
            MultiKeyUntyped[] newDataMultiKey = GenerateGroupKeys(newData, keysAndEvents, true);
            MultiKeyUntyped[] oldDataMultiKey = GenerateGroupKeys(oldData, keysAndEvents, false);
    
            EventBean[] selectOldEvents = null;
            if (isSelectRStream)
            {
                selectOldEvents = GenerateOutputEventsView(keysAndEvents, oldGenerators, false, isSynthesize);
            }
    
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
            EventBean[] selectNewEvents = GenerateOutputEventsView(keysAndEvents, newGenerators, true, isSynthesize);
    
            if ((selectNewEvents != null) || (selectOldEvents != null))
            {
                return new UniformPair<EventBean[]>(selectNewEvents, selectOldEvents);
            }
            return null;
        }
    
        private EventBean[] GenerateOutputEventsView(ICollection<KeyValuePair<MultiKeyUntyped, EventBean>> keysAndEvents, Map<MultiKeyUntyped, EventBean[]> generators, bool isNewData, bool isSynthesize)
        {
            EventBean[] eventsPerStream = new EventBean[1];
            EventBean[] events = new EventBean[keysAndEvents.Count];
            MultiKeyUntyped[] keys = new MultiKeyUntyped[keysAndEvents.Count];
            EventBean[][] currentGenerators = null;
            if(isSorting)
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
                    bool? result = (bool?) optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                    if (!result ?? true)
                    {
                        continue;
                    }
                }
    
                events[count] = selectExprProcessor.Process(eventsPerStream, isNewData, isSynthesize);
                keys[count] = entry.Key;
                if(isSorting)
                {
                    EventBean[] currentEventsPerStream = new EventBean[] { entry.Value };
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
                events =  orderByProcessor.Sort(events, currentGenerators, keys, isNewData);
            }
    
            return events;
        }
    
        private void GenerateOutputBatched(IEnumerable<KeyValuePair<MultiKeyUntyped, EventBean>> keysAndEvents, bool isNewData, bool isSynthesize, ICollection<EventBean> resultEvents, ICollection<MultiKeyUntyped> optSortKeys)
        {
            EventBean[] eventsPerStream = new EventBean[1];
    
            foreach (KeyValuePair<MultiKeyUntyped, EventBean> entry in keysAndEvents)
            {
                // Set the current row of aggregation states
                aggregationService.SetCurrentRow(entry.Key);
    
                eventsPerStream[0] = entry.Value;
    
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
    
        private EventBean[] GenerateOutputEventsJoin(ICollection<KeyValuePair<MultiKeyUntyped, EventBean[]>> keysAndEvents, Map<MultiKeyUntyped, EventBean[]> generators, bool isNewData, bool isSynthesize)
        {
            EventBean[] events = new EventBean[keysAndEvents.Count];
            MultiKeyUntyped[] keys = new MultiKeyUntyped[keysAndEvents.Count];
            EventBean[][] currentGenerators = null;
            if(isSorting)
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
                    bool? result = (bool?) optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                    if (!result ?? true)
                    {
                        continue;
                    }
                }
    
                events[count] = selectExprProcessor.Process(eventsPerStream, isNewData, isSynthesize);
                keys[count] = entry.Key;
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
                events =  orderByProcessor.Sort(events, currentGenerators, keys, isNewData);
            }
    
            return events;
        }
    
        private MultiKeyUntyped[] GenerateGroupKeys(EventBean[] events, Map<MultiKeyUntyped, EventBean> eventPerKey, bool isNewData)
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
                eventPerKey.Put(keys[i], events[i]);
            }
    
            return keys;
        }
    
        private MultiKeyUntyped[] GenerateGroupKeys(ICollection<MultiKey<EventBean>> resultSet, Map<MultiKeyUntyped, EventBean[]> eventPerKey, bool isNewData)
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
                eventPerKey.Put(keys[count], eventsPerStream.Array);
    
                count++;
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
    
        /// <summary>Returns the optional having expression. </summary>
        /// <returns>having expression node</returns>
        public ExprNode GetOptionalHavingNode()
        {
            return optionalHavingNode;
        }
    
        /// <summary>Returns the select expression processor </summary>
        /// <returns>select processor.</returns>
        public SelectExprProcessor GetSelectExprProcessor()
        {
            return selectExprProcessor;
        }
    
        public IEnumerator<EventBean> GetEnumerator(Viewable parent)
        {
            if (orderByProcessor == null)
            {
                return EnumerateResultSet(parent);
                //return new ResultSetRowPerGroupIterator(parent.GetEnumerator(), this, aggregationService);
            }
    
            // Pull all parent events, generate order keys
            EventBean[] eventsPerStream = new EventBean[1];
            List<EventBean> outgoingEvents = new List<EventBean>();
            List<MultiKeyUntyped> orderKeys = new List<MultiKeyUntyped>();
            Set<MultiKeyUntyped> priorSeenGroups = new HashSet<MultiKeyUntyped>();
    
            foreach (EventBean candidate in parent)
            {
                eventsPerStream[0] = candidate;
    
                MultiKeyUntyped groupKey = GenerateGroupKey(eventsPerStream, true);
                aggregationService.SetCurrentRow(groupKey);
    
                bool? pass = true;
                if (optionalHavingNode != null)
                {
                    pass = (bool?) optionalHavingNode.Evaluate(eventsPerStream, true);
                }
                if (!pass ?? true)
                {
                    continue;
                }
                if (priorSeenGroups.Contains(groupKey))
                {
                    continue;
                }
                priorSeenGroups.Add(groupKey);
    
                outgoingEvents.Add(selectExprProcessor.Process(eventsPerStream, true, true));
    
                MultiKeyUntyped orderKey = orderByProcessor.GetSortKey(eventsPerStream, true);
                orderKeys.Add(orderKey);
            }
    
            // sort
            EventBean[] outgoingEventsArr = outgoingEvents.ToArray();
            MultiKeyUntyped[] orderKeysArr = orderKeys.ToArray();
            EventBean[] orderedEvents = orderByProcessor.Sort(outgoingEventsArr, orderKeysArr);
    
            return ((IEnumerable<EventBean>) orderedEvents).GetEnumerator();
        }

        private IEnumerator<EventBean> EnumerateResultSet(IEnumerable<EventBean> baseEnum)
        {
            Set<MultiKeyUntyped> priorSeenGroups = new HashSet<MultiKeyUntyped>();

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
                    if (!priorSeenGroups.Contains(groupKey)) {
                        priorSeenGroups.Add(groupKey);
                        yield return rEventBean;
                    }
                }
            }
        }

        public IEnumerator<EventBean> GetEnumerator(Set<MultiKey<EventBean>> joinSet)
        {
            Map<MultiKeyUntyped, EventBean[]> keysAndEvents = new HashMap<MultiKeyUntyped, EventBean[]>();
            GenerateGroupKeys(joinSet, keysAndEvents, true);
            EventBean[] selectNewEvents = GenerateOutputEventsJoin(keysAndEvents, newGenerators, true, true);
            return ((IEnumerable<EventBean>) selectNewEvents).GetEnumerator();
        }
    
        public void Clear()
        {
            aggregationService.ClearResults();
        }
    
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
    
                Map<MultiKeyUntyped, EventBean[]> keysAndEvents = new HashMap<MultiKeyUntyped, EventBean[]>();
    
                foreach (UniformPair<Set<MultiKey<EventBean>>> pair in joinEventsSet)
                {
                    Set<MultiKey<EventBean>> newData = pair.First;
                    Set<MultiKey<EventBean>> oldData = pair.Second;
    
                    MultiKeyUntyped[] newDataMultiKey = GenerateGroupKeys(newData, keysAndEvents, true);
                    MultiKeyUntyped[] oldDataMultiKey = GenerateGroupKeys(oldData, keysAndEvents, false);
    
                    if (isSelectRStream)
                    {
                        GenerateOutputBatchedArr(keysAndEvents, false, generateSynthetic, oldEvents, oldEventsSortKey);
                    }
    
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
    
                    GenerateOutputBatchedArr(keysAndEvents, true, generateSynthetic, newEvents, newEventsSortKey);
    
                    keysAndEvents.Clear();
                }
    
                EventBean[] newEventsArr = (CollectionHelper.IsEmpty(newEvents)) ? null : newEvents.ToArray();
                EventBean[] oldEventsArr = null;
                if (isSelectRStream)
                {
                    oldEventsArr = (CollectionHelper.IsEmpty(oldEvents)) ? null : oldEvents.ToArray();
                }
    
                if (orderByProcessor != null)
                {
                    MultiKeyUntyped[] sortKeysNew = (CollectionHelper.IsEmpty(newEventsSortKey)) ? null : newEventsSortKey.ToArray();
                    newEventsArr = orderByProcessor.Sort(newEventsArr, sortKeysNew);
                    if (isSelectRStream)
                    {
                        MultiKeyUntyped[] sortKeysOld = (CollectionHelper.IsEmpty(oldEventsSortKey)) ? null : oldEventsSortKey.ToArray();
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
    
                if (isSelectRStream)
                {
                    GenerateOutputBatchedArr(groupRepsView, false, generateSynthetic, oldEvents, oldEventsSortKey);
                }
    
                foreach (UniformPair<Set<MultiKey<EventBean>>> pair in joinEventsSet)
                {
                    Set<MultiKey<EventBean>> newData = pair.First;
                    Set<MultiKey<EventBean>> oldData = pair.Second;
    
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        foreach (MultiKey<EventBean> aNewData in newData)
                        {
                            MultiKeyUntyped mk = GenerateGroupKey(aNewData.Array, true);
    
                            // if this is a newly encountered group, generate the remove stream event
                            if (groupRepsView.Push(mk, aNewData.Array) == null)
                            {
                                workCollection.Clear();
                                workCollection.Put(mk, aNewData.Array);
                                if (isSelectRStream)
                                {
                                    GenerateOutputBatchedArr(workCollection, false, generateSynthetic, oldEvents, oldEventsSortKey);
                                }
                            }
                            aggregationService.ApplyEnter(aNewData.Array, mk);
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        foreach (MultiKey<EventBean> anOldData in oldData)
                        {
                            MultiKeyUntyped mk = GenerateGroupKey(anOldData.Array, true);

                            if (groupRepsView.Push(mk, anOldData.Array) == null)
                            {
                                workCollection.Clear();
                                workCollection.Put(mk, anOldData.Array);
                                if (isSelectRStream)
                                {
                                    GenerateOutputBatchedArr(workCollection, false, generateSynthetic, oldEvents, oldEventsSortKey);
                                }
                            }
    
                            aggregationService.ApplyLeave(anOldData.Array, mk);
                        }
                    }
                }
    
                GenerateOutputBatchedArr(groupRepsView, true, generateSynthetic, newEvents, newEventsSortKey);
    
                EventBean[] newEventsArr = (CollectionHelper.IsEmpty(newEvents)) ? null : newEvents.ToArray();
                EventBean[] oldEventsArr = null;
                if (isSelectRStream)
                {
                    oldEventsArr = (CollectionHelper.IsEmpty(oldEvents)) ? null : oldEvents.ToArray();
                }
    
                if (orderByProcessor != null)
                {
                    MultiKeyUntyped[] sortKeysNew = (CollectionHelper.IsEmpty(newEventsSortKey)) ? null : newEventsSortKey.ToArray();
                    newEventsArr = orderByProcessor.Sort(newEventsArr, sortKeysNew);
                    if (isSelectRStream)
                    {
                        MultiKeyUntyped[] sortKeysOld = (CollectionHelper.IsEmpty(oldEventsSortKey)) ? null : oldEventsSortKey.ToArray();
                        oldEventsArr = orderByProcessor.Sort(oldEventsArr, sortKeysOld);
                    }
                }
    
                if ((newEventsArr == null) && (oldEventsArr == null))
                {
                    return null;
                }
                return new UniformPair<EventBean[]>(newEventsArr, oldEventsArr);
            }
            else // (outputLimitLimitType == OutputLimitLimitType.LAST)
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
    
                groupRepsView.Clear();
                foreach (UniformPair<Set<MultiKey<EventBean>>> pair in joinEventsSet)
                {
                    Set<MultiKey<EventBean>> newData = pair.First;
                    Set<MultiKey<EventBean>> oldData = pair.Second;
    
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        foreach (MultiKey<EventBean> aNewData in newData)
                        {
                            MultiKeyUntyped mk = GenerateGroupKey(aNewData.Array, true);
    
                            // if this is a newly encountered group, generate the remove stream event
                            if (groupRepsView.Push(mk, aNewData.Array) == null)
                            {
                                workCollection.Clear();
                                workCollection.Put(mk, aNewData.Array);
                                if (isSelectRStream)
                                {
                                    GenerateOutputBatchedArr(workCollection, false, generateSynthetic, oldEvents, oldEventsSortKey);
                                }
                            }
                            aggregationService.ApplyEnter(aNewData.Array, mk);
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        foreach (MultiKey<EventBean> anOldData in oldData)
                        {
                            MultiKeyUntyped mk = GenerateGroupKey(anOldData.Array, true);

                            if (groupRepsView.Push(mk, anOldData.Array) == null)
                            {
                                workCollection.Clear();
                                workCollection.Put(mk, anOldData.Array);
                                if (isSelectRStream)
                                {
                                    GenerateOutputBatchedArr(workCollection, false, generateSynthetic, oldEvents, oldEventsSortKey);
                                }
                            }
    
                            aggregationService.ApplyLeave(anOldData.Array, mk);
                        }
                    }
                }
    
                GenerateOutputBatchedArr(groupRepsView, true, generateSynthetic, newEvents, newEventsSortKey);
    
                EventBean[] newEventsArr = (CollectionHelper.IsEmpty(newEvents)) ? null : newEvents.ToArray();
                EventBean[] oldEventsArr = null;
                if (isSelectRStream)
                {
                    oldEventsArr = (CollectionHelper.IsEmpty(oldEvents)) ? null : oldEvents.ToArray();
                }
    
                if (orderByProcessor != null)
                {
                    MultiKeyUntyped[] sortKeysNew = (CollectionHelper.IsEmpty(newEventsSortKey)) ? null : newEventsSortKey.ToArray();
                    newEventsArr = orderByProcessor.Sort(newEventsArr, sortKeysNew);
    
                    if (isSelectRStream)
                    {
                        MultiKeyUntyped[] sortKeysOld = (CollectionHelper.IsEmpty(oldEventsSortKey)) ? null : oldEventsSortKey.ToArray();
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
    
        public UniformPair<EventBean[]> ProcessOutputLimitedView(IList<UniformPair<EventBean[]>> viewEventsList, bool generateSynthetic, OutputLimitLimitType outputLimitLimitType)
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
    
                Map<MultiKeyUntyped, EventBean> keysAndEvents = new HashMap<MultiKeyUntyped, EventBean>();
    
                foreach (UniformPair<EventBean[]> pair in viewEventsList)
                {
                    EventBean[] newData = pair.First;
                    EventBean[] oldData = pair.Second;
    
                    MultiKeyUntyped[] newDataMultiKey = GenerateGroupKeys(newData, keysAndEvents, true);
                    MultiKeyUntyped[] oldDataMultiKey = GenerateGroupKeys(oldData, keysAndEvents, false);
    
                    if (isSelectRStream)
                    {
                        GenerateOutputBatched(keysAndEvents, false, generateSynthetic, oldEvents, oldEventsSortKey);
                    }
    
                    EventBean[] eventsPerStream = new EventBean[1];
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
    
                    GenerateOutputBatched(keysAndEvents, true, generateSynthetic, newEvents, newEventsSortKey);
    
                    keysAndEvents.Clear();
                }
    
                EventBean[] newEventsArr = (CollectionHelper.IsEmpty(newEvents)) ? null : newEvents.ToArray();
                EventBean[] oldEventsArr = null;
                if (isSelectRStream)
                {
                    oldEventsArr = (CollectionHelper.IsEmpty(oldEvents)) ? null : oldEvents.ToArray();
                }
    
                if (orderByProcessor != null)
                {
                    MultiKeyUntyped[] sortKeysNew = (CollectionHelper.IsEmpty(newEventsSortKey)) ? null : newEventsSortKey.ToArray();
                    newEventsArr = orderByProcessor.Sort(newEventsArr, sortKeysNew);
                    if (isSelectRStream)
                    {
                        MultiKeyUntyped[] sortKeysOld = (CollectionHelper.IsEmpty(oldEventsSortKey)) ? null : oldEventsSortKey.ToArray();
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
                EventBean[] eventsPerStream = new EventBean[1];
    
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
    
                if (isSelectRStream)
                {
                    GenerateOutputBatchedArr(groupRepsView, false, generateSynthetic, oldEvents, oldEventsSortKey);
                }
    
                foreach (UniformPair<EventBean[]> pair in viewEventsList)
                {
                    EventBean[] newData = pair.First;
                    EventBean[] oldData = pair.Second;
    
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        foreach (EventBean aNewData in newData)
                        {
                            eventsPerStream[0] = aNewData;
                            MultiKeyUntyped mk = GenerateGroupKey(eventsPerStream, true);
    
                            // if this is a newly encountered group, generate the remove stream event
                            if (groupRepsView.Push(mk, new EventBean[] { aNewData }) == null)
                            {
                                workCollection.Clear();
                                workCollection.Put(mk, eventsPerStream);
                                if (isSelectRStream)
                                {
                                    GenerateOutputBatchedArr(workCollection, false, generateSynthetic, oldEvents, oldEventsSortKey);
                                }
                            }
                            aggregationService.ApplyEnter(eventsPerStream, mk);
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        foreach (EventBean anOldData in oldData)
                        {
                            eventsPerStream[0] = anOldData;
                            MultiKeyUntyped mk = GenerateGroupKey(eventsPerStream, true);

                            if (groupRepsView.Push(mk, new EventBean[] { anOldData }) == null)
                            {
                                workCollection.Clear();
                                workCollection.Put(mk, eventsPerStream);
                                if (isSelectRStream)
                                {
                                    GenerateOutputBatchedArr(workCollection, false, generateSynthetic, oldEvents, oldEventsSortKey);
                                }
                            }
    
                            aggregationService.ApplyLeave(eventsPerStream, mk);
                        }
                    }
                }
    
                GenerateOutputBatchedArr(groupRepsView, true, generateSynthetic, newEvents, newEventsSortKey);
    
                EventBean[] newEventsArr = (CollectionHelper.IsEmpty(newEvents)) ? null : newEvents.ToArray();
                EventBean[] oldEventsArr = null;
                if (isSelectRStream)
                {
                    oldEventsArr = (CollectionHelper.IsEmpty(oldEvents)) ? null : oldEvents.ToArray();
                }
    
                if (orderByProcessor != null)
                {
                    MultiKeyUntyped[] sortKeysNew = (CollectionHelper.IsEmpty(newEventsSortKey)) ? null : newEventsSortKey.ToArray();
                    newEventsArr = orderByProcessor.Sort(newEventsArr, sortKeysNew);
                    if (isSelectRStream)
                    {
                        MultiKeyUntyped[] sortKeysOld = (CollectionHelper.IsEmpty(oldEventsSortKey)) ? null : oldEventsSortKey.ToArray();
                        oldEventsArr = orderByProcessor.Sort(oldEventsArr, sortKeysOld);
                    }
                }
    
                if ((newEventsArr == null) && (oldEventsArr == null))
                {
                    return null;
                }
                return new UniformPair<EventBean[]>(newEventsArr, oldEventsArr);
            }
            else // (outputLimitLimitType == OutputLimitLimitType.LAST)
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
    
                groupRepsView.Clear();
                foreach (UniformPair<EventBean[]> pair in viewEventsList)
                {
                    EventBean[] newData = pair.First;
                    EventBean[] oldData = pair.Second;
    
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        foreach (EventBean aNewData in newData)
                        {
                            EventBean[] eventsPerStream = new EventBean[] {aNewData};
                            MultiKeyUntyped mk = GenerateGroupKey(eventsPerStream, true);
    
                            // if this is a newly encountered group, generate the remove stream event
                            if (groupRepsView.Push(mk, eventsPerStream) == null)
                            {
                                workCollection.Clear();
                                workCollection.Put(mk, eventsPerStream);
                                if (isSelectRStream)
                                {
                                    GenerateOutputBatchedArr(workCollection, false, generateSynthetic, oldEvents, oldEventsSortKey);
                                }
                            }
                            aggregationService.ApplyEnter(eventsPerStream, mk);
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        foreach (EventBean anOldData in oldData)
                        {
                            EventBean[] eventsPerStream = new EventBean[] {anOldData};
                            MultiKeyUntyped mk = GenerateGroupKey(eventsPerStream, true);

                            if (groupRepsView.Push(mk, eventsPerStream) == null)
                            {
                                workCollection.Clear();
                                workCollection.Put(mk, eventsPerStream);
                                if (isSelectRStream)
                                {
                                    GenerateOutputBatchedArr(workCollection, false, generateSynthetic, oldEvents, oldEventsSortKey);
                                }
                            }
    
                            aggregationService.ApplyLeave(eventsPerStream, mk);
                        }
                    }
                }
    
                GenerateOutputBatchedArr(groupRepsView, true, generateSynthetic, newEvents, newEventsSortKey);
    
                EventBean[] newEventsArr = (CollectionHelper.IsEmpty(newEvents)) ? null : newEvents.ToArray();
                EventBean[] oldEventsArr = null;
                if (isSelectRStream)
                {
                    oldEventsArr = (CollectionHelper.IsEmpty(oldEvents)) ? null : oldEvents.ToArray();
                }
    
                if (orderByProcessor != null)
                {
                    MultiKeyUntyped[] sortKeysNew = (CollectionHelper.IsEmpty(newEventsSortKey)) ? null : newEventsSortKey.ToArray();
                    newEventsArr = orderByProcessor.Sort(newEventsArr, sortKeysNew);
                    if (isSelectRStream)
                    {
                        MultiKeyUntyped[] sortKeysOld = (CollectionHelper.IsEmpty(oldEventsSortKey)) ? null : oldEventsSortKey.ToArray();
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
    }
}
