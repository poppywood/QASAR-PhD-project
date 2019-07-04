///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

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
    /// Result set processor for the case: aggregation functions used in the select clause,
    /// and no group-by, and not all of the properties in the select clause are under an
    /// aggregation function.
    /// <para/>
    /// This processor does not perform grouping, every event entering and leaving is in the
    /// same group. The processor generates one row for each event entering (new event) and
    /// one row for each event leaving (old event). Aggregation state is simply one row
    /// holding all the state.
    /// </summary>
    public class ResultSetProcessorAggregateAll : ResultSetProcessor
    {
        private readonly SelectExprProcessor selectExprProcessor;
        private readonly OrderByProcessor orderByProcessor;
        private readonly AggregationService aggregationService;
        private readonly ExprNode optionalHavingNode;
        private readonly bool isSelectRStream;
    
        /// <summary>Ctor. </summary>
        /// <param name="selectExprProcessor">for processing the select expression and generting the readonly output rows</param>
        /// <param name="orderByProcessor">for sorting the outgoing events according to the order-by clause</param>
        /// <param name="aggregationService">handles aggregation</param>
        /// <param name="optionalHavingNode">having clause expression node</param>
        /// <param name="isSelectRStream">true if remove stream events should be generated</param>
        public ResultSetProcessorAggregateAll(SelectExprProcessor selectExprProcessor,
                                              OrderByProcessor orderByProcessor,
                                              AggregationService aggregationService,
                                              ExprNode optionalHavingNode,
                                              bool isSelectRStream)
        {
            this.selectExprProcessor = selectExprProcessor;
            this.orderByProcessor = orderByProcessor;
            this.aggregationService = aggregationService;
            this.optionalHavingNode = optionalHavingNode;
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
            EventBean[] selectOldEvents = null;
            EventBean[] selectNewEvents;
    
            if (CollectionHelper.IsNotEmpty(newEvents))
            {
                // apply new data to aggregates
                foreach (MultiKey<EventBean> events in newEvents)
                {
                    aggregationService.ApplyEnter(events.Array, null);
                }
            }
    
            if (CollectionHelper.IsNotEmpty(oldEvents))
            {
                // apply old data to aggregates
                foreach (MultiKey<EventBean> events in oldEvents)
                {
                    aggregationService.ApplyLeave(events.Array, null);
                }
            }
    
            if (optionalHavingNode == null)
            {
                if (isSelectRStream)
                {
                    selectOldEvents = ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, oldEvents, false, isSynthesize);
                }
                selectNewEvents = ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, newEvents, true, isSynthesize);
            }
            else
            {
                if (isSelectRStream)
                {
                    selectOldEvents = ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, orderByProcessor, oldEvents, optionalHavingNode, false, isSynthesize);
                }
                selectNewEvents = ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, orderByProcessor, newEvents, optionalHavingNode, true, isSynthesize);
            }
    
            if ((selectNewEvents == null) && (selectOldEvents == null))
            {
                return null;
            }
            return new UniformPair<EventBean[]>(selectNewEvents, selectOldEvents);
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
            EventBean[] selectOldEvents = null;
            EventBean[] selectNewEvents;
    
            EventBean[] eventsPerStream = new EventBean[1];
            if (newData != null)
            {
                // apply new data to aggregates
                foreach (EventBean aNewData in newData)
                {
                    eventsPerStream[0] = aNewData;
                    aggregationService.ApplyEnter(eventsPerStream, null);
                }
            }
            if (oldData != null)
            {
                // apply old data to aggregates
                foreach (EventBean anOldData in oldData)
                {
                    eventsPerStream[0] = anOldData;
                    aggregationService.ApplyLeave(eventsPerStream, null);
                }
            }
    
            // generate new events using select expressions
            if (optionalHavingNode == null)
            {
                if (isSelectRStream)
                {
                    selectOldEvents = ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, oldData, false, isSynthesize);
                }
                selectNewEvents = ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, newData, true, isSynthesize);
            }
            else
            {
                if (isSelectRStream)
                {
                    selectOldEvents = ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, orderByProcessor, oldData, optionalHavingNode, false, isSynthesize);
                }
                selectNewEvents = ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, orderByProcessor, newData, optionalHavingNode, true, isSynthesize);
            }
    
            if ((selectNewEvents == null) && (selectOldEvents == null))
            {
                return null;
            }
    
            return new UniformPair<EventBean[]>(selectNewEvents, selectOldEvents);
        }

        /// <summary>
        /// Returns the iterator implementing the group-by and aggregation and order-by logic
        /// specific to each case of use of these construct.
        /// </summary>
        /// <param name="parent">is the parent view iterator</param>
        /// <returns>event iterator</returns>
        public IEnumerator<EventBean> GetEnumerator(Viewable parent)
        {
            if (orderByProcessor == null)
            {
                return EnumerateResultSet(parent);
            }
    
            // Pull all parent events, generate order keys
            EventBean[] eventsPerStream = new EventBean[1];
            List<EventBean> outgoingEvents = new List<EventBean>();
            List<MultiKeyUntyped> orderKeys = new List<MultiKeyUntyped>();
    
            foreach (EventBean candidate in parent)
            {
                eventsPerStream[0] = candidate;
    
                bool? pass = true;
                if (optionalHavingNode != null)
                {
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
    
            return ((IEnumerable<EventBean>) orderedEvents).GetEnumerator();
        }

        private IEnumerator<EventBean> EnumerateResultSet(IEnumerable<EventBean> baseEnum)
        {
            EventBean[] eventArray = new EventBean[1];
            
            BeanEvaluator baseEval = delegate { return selectExprProcessor.Process(eventArray, true, true); };
            BeanEvaluator beanEval = baseEval;
            if (optionalHavingNode != null) {
                beanEval = delegate(EventBean eventBean) {
                               bool? result = (bool?) optionalHavingNode.Evaluate(eventArray, true);
                               return (result ?? false) ? baseEval.Invoke(eventBean) : null;
                           };
            }

            foreach (EventBean eventBean in baseEnum) {
                eventArray[0] = eventBean;
                EventBean rEventBean = beanEval.Invoke(eventBean);
                if (rEventBean != null) {
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

        /// <summary>Returns the optional having expression. </summary>
        /// <returns>having expression node</returns>
        public ExprNode OptionalHavingNode
        {
            get { return optionalHavingNode; }
        }

        /// <summary>Returns the iterator for iterating over a join-result.</summary>
        /// <param name="joinSet">is the join result set</param>
        /// <returns>iterator over join results</returns>
        public IEnumerator<EventBean> GetEnumerator(Set<MultiKey<EventBean>> joinSet)
        {
            EventBean[] result;
            if (optionalHavingNode == null)
            {
                result = ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, joinSet, true, true);
            }
            else
            {
                result = ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, orderByProcessor, joinSet, optionalHavingNode, true, true);
            }
            return (result == null)
                       ? (EnumerationHelper<EventBean>.CreateEmptyEnumerator())
                       : ((IEnumerable<EventBean>) result).GetEnumerator();
        }

        /// <summary>Clear out current state.</summary>
        public void Clear()
        {
            aggregationService.ClearResults();
        }

        /// <summary>Processes batched events in case of output-rate limiting.</summary>
        /// <param name="joinEventsSet">the join results</param>
        /// <param name="generateSynthetic">flag to indicate whether synthetic events must be generated</param>
        /// <param name="outputLimitLimitType">the type of output rate limiting</param>
        /// <returns>results for dispatch</returns>
        public UniformPair<EventBean[]> ProcessOutputLimitedJoin(
            IList<UniformPair<Set<MultiKey<EventBean>>>> joinEventsSet,
            bool generateSynthetic,
            OutputLimitLimitType outputLimitLimitType)
        {
            if (outputLimitLimitType == OutputLimitLimitType.LAST)
            {
                EventBean lastOldEvent = null;
                EventBean lastNewEvent = null;
    
                foreach (UniformPair<Set<MultiKey<EventBean>>> pair in joinEventsSet)
                {
                    Set<MultiKey<EventBean>> newData = pair.First;
                    Set<MultiKey<EventBean>> oldData = pair.Second;
    
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        foreach (MultiKey<EventBean> eventsPerStream in newData)
                        {
                            aggregationService.ApplyEnter(eventsPerStream.Array, null);
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        foreach (MultiKey<EventBean> eventsPerStream in oldData)
                        {
                            aggregationService.ApplyLeave(eventsPerStream.Array, null);
                        }
                    }
    
                    EventBean[] selectOldEvents;
                    if (isSelectRStream)
                    {
                        if (optionalHavingNode == null)
                        {
                            selectOldEvents = ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, oldData, false, generateSynthetic);
                        }
                        else
                        {
                            selectOldEvents = ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, oldData, optionalHavingNode, false, generateSynthetic);
                        }
                        if ((selectOldEvents != null) && (selectOldEvents.Length > 0))
                        {
                            lastOldEvent = selectOldEvents[selectOldEvents.Length - 1];
                        }
                    }
    
                    // generate new events using select expressions
                    EventBean[] selectNewEvents;
                    if (optionalHavingNode == null)
                    {
                        selectNewEvents = ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, newData, true, generateSynthetic);
                    }
                    else
                    {
                        selectNewEvents = ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, newData, optionalHavingNode, true, generateSynthetic);
                    }
                    if ((selectNewEvents != null) && (selectNewEvents.Length > 0))
                    {
                        lastNewEvent = selectNewEvents[selectNewEvents.Length - 1];
                    }
                }
    
                EventBean[] lastNew = (lastNewEvent != null) ? new EventBean[] {lastNewEvent} : null;
                EventBean[] lastOld = (lastOldEvent != null) ? new EventBean[] {lastOldEvent} : null;
    
                if ((lastNew == null) && (lastOld == null))
                {
                    return null;
                }
                return new UniformPair<EventBean[]>(lastNew, lastOld);
            }
            else
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
    
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        foreach (MultiKey<EventBean> row in newData)
                        {
                            aggregationService.ApplyEnter(row.Array, null);
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        foreach (MultiKey<EventBean> row in oldData)
                        {
                            aggregationService.ApplyLeave(row.Array, null);
                        }
                    }
    
                    // generate old events using select expressions
                    if (isSelectRStream)
                    {
                        if (optionalHavingNode == null)
                        {
                            ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, oldData, false, generateSynthetic, oldEvents, oldEventsSortKey);
                        }
                        // generate old events using having then select
                        else
                        {
                            ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, orderByProcessor, oldData, optionalHavingNode, false, generateSynthetic, oldEvents, oldEventsSortKey);
                        }
                    }
    
                    // generate new events using select expressions
                    if (optionalHavingNode == null)
                    {
                        ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, newData, true, generateSynthetic, newEvents, newEventsSortKey);
                    }
                    else
                    {
                        ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, orderByProcessor, newData, optionalHavingNode, true, generateSynthetic, newEvents, newEventsSortKey);
                    }
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
        }

        /// <summary>Processes batched events in case of output-rate limiting.</summary>
        /// <param name="viewEventsList">the view results</param>
        /// <param name="generateSynthetic">flag to indicate whether synthetic events must be generated</param>
        /// <param name="outputLimitLimitType">the type of output rate limiting</param>
        /// <returns>results for dispatch</returns>
        public UniformPair<EventBean[]> ProcessOutputLimitedView(IList<UniformPair<EventBean[]>> viewEventsList,
                                                                 bool generateSynthetic,
                                                                 OutputLimitLimitType outputLimitLimitType)
        {
            if (outputLimitLimitType == OutputLimitLimitType.LAST)
            {
                EventBean lastOldEvent = null;
                EventBean lastNewEvent = null;
                EventBean[] eventsPerStream = new EventBean[1];
    
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
                            aggregationService.ApplyEnter(eventsPerStream, null);
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        foreach (EventBean anOldData in oldData)
                        {
                            eventsPerStream[0] = anOldData;
                            aggregationService.ApplyLeave(eventsPerStream, null);
                        }
                    }
    
                    EventBean[] selectOldEvents;
                    if (isSelectRStream)
                    {
                        if (optionalHavingNode == null)
                        {
                            selectOldEvents = ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, oldData, false, generateSynthetic);
                        }
                        else
                        {
                            selectOldEvents = ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, oldData, optionalHavingNode, false, generateSynthetic);
                        }
                        if ((selectOldEvents != null) && (selectOldEvents.Length > 0))
                        {
                            lastOldEvent = selectOldEvents[selectOldEvents.Length - 1];
                        }
                    }
    
                    // generate new events using select expressions
                    EventBean[] selectNewEvents;
                    if (optionalHavingNode == null)
                    {
                        selectNewEvents = ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, newData, true, generateSynthetic);
                    }
                    else
                    {
                        selectNewEvents = ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, newData, optionalHavingNode, true, generateSynthetic);
                    }
                    if ((selectNewEvents != null) && (selectNewEvents.Length > 0))
                    {
                        lastNewEvent = selectNewEvents[selectNewEvents.Length - 1];
                    }
                }
    
                EventBean[] lastNew = (lastNewEvent != null) ? new EventBean[] {lastNewEvent} : null;
                EventBean[] lastOld = (lastOldEvent != null) ? new EventBean[] {lastOldEvent} : null;
    
                if ((lastNew == null) && (lastOld == null))
                {
                    return null;
                }
                return new UniformPair<EventBean[]>(lastNew, lastOld);
            }
            else
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
    
                    EventBean[] eventsPerStream = new EventBean[1];
                    if (newData != null)
                    {
                        // apply new data to aggregates
                        foreach (EventBean aNewData in newData)
                        {
                            eventsPerStream[0] = aNewData;
                            aggregationService.ApplyEnter(eventsPerStream, null);
                        }
                    }
                    if (oldData != null)
                    {
                        // apply old data to aggregates
                        foreach (EventBean anOldData in oldData)
                        {
                            eventsPerStream[0] = anOldData;
                            aggregationService.ApplyLeave(eventsPerStream, null);
                        }
                    }
    
                    // generate old events using select expressions
                    if (isSelectRStream)
                    {
                        if (optionalHavingNode == null)
                        {
                            ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, oldData, false, generateSynthetic, oldEvents, oldEventsSortKey);
                        }
                        // generate old events using having then select
                        else
                        {
                            ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, orderByProcessor, oldData, optionalHavingNode, false, generateSynthetic, oldEvents, oldEventsSortKey);
                        }
                    }
    
                    // generate new events using select expressions
                    if (optionalHavingNode == null)
                    {
                        ResultSetProcessorSimple.GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, newData, true, generateSynthetic, newEvents, newEventsSortKey);
                    }
                    else
                    {
                        ResultSetProcessorSimple.GetSelectEventsHaving(selectExprProcessor, orderByProcessor, newData, optionalHavingNode, true, generateSynthetic, newEvents, newEventsSortKey);
                    }
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
        }
    }
}
