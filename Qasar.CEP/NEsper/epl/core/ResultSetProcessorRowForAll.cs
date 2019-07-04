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
    /// and no group-by, and all properties in the select clause are under an aggregation function.
    /// <para/>
    /// This processor does not perform grouping, every event entering and leaving is in the same
    /// group. Produces one old event and one new event row every time either at least one old or
    /// new event is received. Aggregation state is simply one row holding all the state.
    /// </summary>
    public class ResultSetProcessorRowForAll : ResultSetProcessor
    {
        private readonly bool isSelectRStream;
        private readonly SelectExprProcessor selectExprProcessor;
        private readonly AggregationService aggregationService;
        private readonly OrderByProcessor orderByProcessor;
        private readonly ExprNode optionalHavingNode;
    
        /// <summary>Ctor. </summary>
        /// <param name="selectExprProcessor">for processing the select expression and generting the readonly output rows</param>
        /// <param name="aggregationService">handles aggregation</param>
        /// <param name="optionalHavingNode">having clause expression node</param>
        /// <param name="isSelectRStream">true if remove stream events should be generated</param>
        /// <param name="orderByProcessor">for ordering output events</param>
        public ResultSetProcessorRowForAll(SelectExprProcessor selectExprProcessor,
                                           AggregationService aggregationService,
                                           OrderByProcessor orderByProcessor,
                                           ExprNode optionalHavingNode,
                                           bool isSelectRStream)
        {
            this.selectExprProcessor = selectExprProcessor;
            this.aggregationService = aggregationService;
            this.optionalHavingNode = optionalHavingNode;
            this.orderByProcessor = orderByProcessor;
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
    
            if (isSelectRStream)
            {
                selectOldEvents = GetSelectListEvents(false, isSynthesize);
            }
    
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
    
            selectNewEvents = GetSelectListEvents(true, isSynthesize);
    
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
    
            if (isSelectRStream)
            {
                selectOldEvents = GetSelectListEvents(false, isSynthesize);
            }
    
            EventBean[] eventsPerStream = new EventBean[1];
            if (newData != null)
            {
                // apply new data to aggregates
                for (int i = 0; i < newData.Length; i++)
                {
                    eventsPerStream[0] = newData[i];
                    aggregationService.ApplyEnter(eventsPerStream, null);
                }
            }
            if (oldData != null)
            {
                // apply old data to aggregates
                for (int i = 0; i < oldData.Length; i++)
                {
                    eventsPerStream[0] = oldData[i];
                    aggregationService.ApplyLeave(eventsPerStream, null);
                }
            }
    
            // generate new events using select expressions
            selectNewEvents = GetSelectListEvents(true, isSynthesize);
    
            if ((selectNewEvents == null) && (selectOldEvents == null))
            {
                return null;
            }
    
            return new UniformPair<EventBean[]>(selectNewEvents, selectOldEvents);
        }
    
        private EventBean[] GetSelectListEvents(bool isNewData, bool isSynthesize)
        {
            // Since we are dealing with strictly aggregation nodes, there are no events required for evaluating
            EventBean @event = selectExprProcessor.Process(null, isNewData, isSynthesize);
    
            if (optionalHavingNode != null)
            {
                bool? result = (bool?) optionalHavingNode.Evaluate(null, isNewData);
                if (!result ?? true)
                {
                    return null;
                }
            }
    
            // The result is always a single row
            return new EventBean[] {@event};
        }
    
        private EventBean GetSelectListEvent(bool isNewData, bool isSynthesize)
        {
            // Since we are dealing with strictly aggregation nodes, there are no events required for evaluating
            EventBean @event = selectExprProcessor.Process(null, isNewData, isSynthesize);
    
            if (optionalHavingNode != null)
            {
                bool? result = (bool?) optionalHavingNode.Evaluate(null, isNewData);
                if (!result ?? true)
                {
                    return null;
                }
            }
    
            // The result is always a single row
            return @event;
        }

        /// <summary>
        /// Returns the iterator implementing the group-by and aggregation and order-by logic
        /// specific to each case of use of these construct.
        /// </summary>
        /// <param name="parent">is the parent view iterator</param>
        /// <returns>event iterator</returns>
        public IEnumerator<EventBean> GetEnumerator(Viewable parent)
        {
            EventBean[] selectNewEvents = GetSelectListEvents(true, true);
            if (selectNewEvents == null)
            {
                return EnumerationHelper<EventBean>.CreateEmptyEnumerator();
            }
            return new SingleEventIterator(selectNewEvents[0]);
        }

        /// <summary>Returns the iterator for iterating over a join-result.</summary>
        /// <param name="joinSet">is the join result set</param>
        /// <returns>iterator over join results</returns>
        public IEnumerator<EventBean> GetEnumerator(Set<MultiKey<EventBean>> joinSet)
        {
            EventBean[] result = GetSelectListEvents(true, true);
            return ((IEnumerable<EventBean>) result).GetEnumerator();
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
    
                // if empty (nothing to post)
                if (CollectionHelper.IsEmpty(joinEventsSet))
                {
                    if (isSelectRStream)
                    {
                        lastOldEvent = GetSelectListEvent(false, generateSynthetic);
                        lastNewEvent = lastOldEvent;
                    }
                    else
                    {
                        lastNewEvent = GetSelectListEvent(false, generateSynthetic);
                    }
                }
    
                foreach (UniformPair<Set<MultiKey<EventBean>>> pair in joinEventsSet)
                {
                    Set<MultiKey<EventBean>> newData = pair.First;
                    Set<MultiKey<EventBean>> oldData = pair.Second;
    
                    if ((lastOldEvent == null) && (isSelectRStream))
                    {
                        lastOldEvent = GetSelectListEvent(false, generateSynthetic);
                    }
    
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
    
                    lastNewEvent = GetSelectListEvent(true, generateSynthetic);
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
    
                    if (isSelectRStream)
                    {
                        GetSelectListEvent(false, generateSynthetic, oldEvents);
                    }
    
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
    
                    GetSelectListEvent(false, generateSynthetic, newEvents);
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
    
                if (CollectionHelper.IsEmpty(joinEventsSet))
                {
                    if (isSelectRStream)
                    {
                        oldEventsArr = GetSelectListEvents(false, generateSynthetic);
                    }
                    newEventsArr = GetSelectListEvents(true, generateSynthetic);
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
                // For last, if there are no events:
                //   As insert stream, return the current value, if matching the having clause
                //   As remove stream, return the current value, if matching the having clause
                // For last, if there are events in the batch:
                //   As insert stream, return the newest value that is matching the having clause
                //   As remove stream, return the oldest value that is matching the having clause
    
                EventBean lastOldEvent = null;
                EventBean lastNewEvent = null;
                EventBean[] eventsPerStream = new EventBean[1];
    
                // if empty (nothing to post)
                if (CollectionHelper.IsEmpty(viewEventsList))
                {
                    if (isSelectRStream)
                    {
                        lastOldEvent = GetSelectListEvent(false, generateSynthetic);
                        lastNewEvent = lastOldEvent;
                    }
                    else
                    {
                        lastNewEvent = GetSelectListEvent(false, generateSynthetic);
                    }
                }
    
                foreach (UniformPair<EventBean[]> pair in viewEventsList)
                {
                    EventBean[] newData = pair.First;
                    EventBean[] oldData = pair.Second;
    
                    if ((lastOldEvent == null) && (isSelectRStream))
                    {
                        lastOldEvent = GetSelectListEvent(false, generateSynthetic);
                    }
    
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
    
                    lastNewEvent = GetSelectListEvent(false, generateSynthetic);
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
    
                    if (isSelectRStream)
                    {
                        GetSelectListEvent(false, generateSynthetic, oldEvents);
                    }
    
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
    
                    GetSelectListEvent(true, generateSynthetic, newEvents);
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
    
                if (CollectionHelper.IsEmpty(viewEventsList))
                {
                    if (isSelectRStream)
                    {
                        oldEventsArr = GetSelectListEvents(false, generateSynthetic);
                    }
                    newEventsArr = GetSelectListEvents(true, generateSynthetic);
                }
    
                if ((newEventsArr == null) && (oldEventsArr == null))
                {
                    return null;
                }
                return new UniformPair<EventBean[]>(newEventsArr, oldEventsArr);
            }
        }
    
        private void GetSelectListEvent(bool isNewData, bool isSynthesize, ICollection<EventBean> resultEvents)
        {
            // Since we are dealing with strictly aggregation nodes, there are no events required for evaluating
            EventBean @event = selectExprProcessor.Process(null, isNewData, isSynthesize);
    
            if (optionalHavingNode != null)
            {
                bool? result = (bool?) optionalHavingNode.Evaluate(null, isNewData);
                if (!result ?? true)
                {
                    return;
                }
            }
    
            resultEvents.Add(@event);
        }
    }
}
