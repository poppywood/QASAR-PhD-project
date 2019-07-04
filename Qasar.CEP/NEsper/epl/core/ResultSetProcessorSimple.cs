///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.expression;
using com.espertech.esper.events;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Result set processor for the simplest case: no aggregation functions used in the select clause, and no group-by.
    /// <para/>
    /// The processor generates one row for each event entering (new event) and one row for each event leaving (old event).
    /// </summary>
	public class ResultSetProcessorSimple : ResultSetProcessorBaseSimple
	{
	    private readonly bool isSelectRStream;
	    private readonly SelectExprProcessor selectExprProcessor;
	    private readonly OrderByProcessor orderByProcessor;
	    private readonly ExprNode optionalHavingExpr;
	    private readonly Set<MultiKey<EventBean>> emptyRowSet = new HashSet<MultiKey<EventBean>>();

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="selectExprProcessor">for processing the select expression and generting the final output rows</param>
        /// <param name="orderByProcessor">for sorting the outgoing events according to the order-by clause</param>
        /// <param name="optionalHavingNode">having clause expression node</param>
        /// <param name="isSelectRStream">true if remove stream events should be generated</param>
	    public ResultSetProcessorSimple(SelectExprProcessor selectExprProcessor,
	                                    OrderByProcessor orderByProcessor,
	                                    ExprNode optionalHavingNode,
	                                    bool isSelectRStream)
	    {
	        this.selectExprProcessor = selectExprProcessor;
	        this.orderByProcessor = orderByProcessor;
	        this.optionalHavingExpr = optionalHavingNode;
	        this.isSelectRStream = isSelectRStream;
	    }

        /// <summary>
        /// Returns the event type of processed results.
        /// </summary>
        /// <value>The type of the result event.</value>
        /// <returns> event type of the resulting events posted by the processor.
        /// </returns>
        public override EventType ResultEventType
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
        public override UniformPair<EventBean[]> ProcessJoinResult(Set<MultiKey<EventBean>> newEvents,
                                                          Set<MultiKey<EventBean>> oldEvents,
                                                          bool isSynthesize)
	    {
	        EventBean[] selectOldEvents = null;
	        EventBean[] selectNewEvents;

	        if (optionalHavingExpr == null)
	        {
	            if (isSelectRStream)
	            {
	                selectOldEvents = GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, oldEvents, false, isSynthesize);
	            }
	            selectNewEvents = GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, newEvents, true, isSynthesize);
	        }
	        else
	        {
	            if (isSelectRStream)
	            {
	                selectOldEvents = GetSelectEventsHaving(selectExprProcessor, orderByProcessor, oldEvents, optionalHavingExpr, false, isSynthesize);
	            }
	            selectNewEvents = GetSelectEventsHaving(selectExprProcessor, orderByProcessor, newEvents, optionalHavingExpr, true, isSynthesize);
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
        public override UniformPair<EventBean[]> ProcessViewResult(EventBean[] newData, EventBean[] oldData, bool isSynthesize)
	    {
	        EventBean[] selectOldEvents = null;
	        EventBean[] selectNewEvents;
	        if (optionalHavingExpr == null)
	        {
	            if (isSelectRStream)
	            {
	                selectOldEvents = GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, oldData, false, isSynthesize);
	            }
	            selectNewEvents = GetSelectEventsNoHaving(selectExprProcessor, orderByProcessor, newData, true, isSynthesize);
	        }
	        else
	        {
	            if (isSelectRStream)
	            {
	                selectOldEvents = GetSelectEventsHaving(selectExprProcessor, orderByProcessor, oldData, optionalHavingExpr, false, isSynthesize);
	            }
	            selectNewEvents = GetSelectEventsHaving(selectExprProcessor, orderByProcessor, newData, optionalHavingExpr, true, isSynthesize);
	        }

	        return new UniformPair<EventBean[]>(selectNewEvents, selectOldEvents);
	    }

        /// <summary>
        /// Applies the select-clause to the given events returning the selected events. The number of events stays the
        /// same, i.e. this method does not filter it just transforms the result set.
        /// </summary>
        /// <param name="exprProcessor">processes each input event and returns output event</param>
        /// <param name="orderByProcessor">orders the outgoing events according to the order-by clause</param>
        /// <param name="events">input events</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>output events, one for each input event</returns>
        public static EventBean[] GetSelectEventsNoHaving(SelectExprProcessor exprProcessor,
                                                            OrderByProcessor orderByProcessor,
                                                            EventBean[] events,
                                                            bool isNewData,
                                                            bool isSynthesize)
	    {
	        if (events == null)
	        {
	            return null;
	        }

	        EventBean[] result = new EventBean[events.Length];
	        EventBean[][] eventGenerators = null;
	        if(orderByProcessor != null)
	        {
	            eventGenerators = new EventBean[events.Length][];
	        }

	        EventBean[] eventsPerStream = new EventBean[1];
	        for (int i = 0; i < events.Length; i++)
	        {
	            eventsPerStream[0] = events[i];

	            // Wildcard select case
	            if(exprProcessor == null)
	            {
	                result[i] = events[i];
	            }
	            else
	            {
	                result[i] = exprProcessor.Process(eventsPerStream, isNewData, isSynthesize);
	            }

	            if(orderByProcessor != null)
	            {
	                  eventGenerators[i] = new EventBean[] {events[i]};
	            }
	        }

	        if(orderByProcessor != null)
	        {
	            return orderByProcessor.Sort(result, eventGenerators, isNewData);
	        }
	        else
	        {
	            return result;
	        }
	    }

        /// <summary>
        /// Applies the select-clause to the given events returning the selected events. The number of events stays the
        /// same, i.e. this method does not filter it just transforms the result set.
        /// </summary>
        /// <param name="exprProcessor">processes each input event and returns output event</param>
        /// <param name="orderByProcessor">for sorting output events according to the order-by clause</param>
        /// <param name="events">input events</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>output events, one for each input event</returns>
        public static EventBean[] GetSelectEventsNoHaving(SelectExprProcessor exprProcessor,
                                                            OrderByProcessor orderByProcessor,
                                                            Set<MultiKey<EventBean>> events,
                                                            bool isNewData,
                                                            bool isSynthesize)
	    {
	        if ((events == null) || (events.Count == 0))
	        {
	            return null;
	        }
	        int length = events.Count;

	        EventBean[] result = new EventBean[length];
	        EventBean[][] eventGenerators = null;
	        if(orderByProcessor != null)
	        {
	            eventGenerators = new EventBean[length][];
	        }

	        int count = 0;
	        foreach (MultiKey<EventBean> key in events)
	        {
	            EventBean[] eventsPerStream = key.Array;
	            result[count] = exprProcessor.Process(eventsPerStream, isNewData, isSynthesize);
	            if(orderByProcessor != null)
	            {
	                eventGenerators[count] = eventsPerStream;
	            }
	            count++;
	        }

	        if(orderByProcessor != null)
	        {
	            return orderByProcessor.Sort(result, eventGenerators, isNewData);
	        }
	        else
	        {
	            return result;
	        }
	    }

        /// <summary>
        /// Applies the select-clause to the given events returning the selected events. The number of events stays the
        /// same, i.e. this method does not filter it just transforms the result set.
        /// <para/>
        /// Also applies a having clause.
        /// </summary>
        /// <param name="exprProcessor">processes each input event and returns output event</param>
        /// <param name="orderByProcessor">for sorting output events according to the order-by clause</param>
        /// <param name="events">input events</param>
        /// <param name="optionalHavingNode">supplies the having-clause expression</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>output events, one for each input event</returns>
        public static EventBean[] GetSelectEventsHaving(SelectExprProcessor exprProcessor,
                                                          OrderByProcessor orderByProcessor,
                                                          EventBean[] events,
                                                          ExprNode optionalHavingNode,
                                                          bool isNewData,
                                                          bool isSynthesize)
	    {
	        if (events == null)
	        {
	            return null;
	        }

	        List<EventBean> result = new List<EventBean>();
	        List<EventBean[]> eventGenerators = null;
	        if(orderByProcessor != null)
	        {
	            eventGenerators = new List<EventBean[]>();
	        }

	        EventBean[] eventsPerStream = new EventBean[1];
	        foreach (EventBean @event in events)
	        {
	            eventsPerStream[0] = @event;

	            bool? passesHaving = (bool?) optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                if ((passesHaving == null) || (!passesHaving.Value))
	            {
	                continue;
	            }

	            result.Add(exprProcessor.Process(eventsPerStream, isNewData, isSynthesize));
	            if (orderByProcessor != null)
	            {
	                eventGenerators.Add(new EventBean[]{@event});
	            }
	        }

	        if (result.Count != 0)
	        {
	            if(orderByProcessor != null)
	            {
	                return orderByProcessor.Sort(result.ToArray(), eventGenerators.ToArray(), isNewData);
	            }
	            else
	            {
	                return result.ToArray();
	            }
	        }
	        else
	        {
	            return null;
	        }
	    }

        /// <summary>
        /// Applies the select-clause to the given events returning the selected events. The number of events stays the
        /// same, i.e. this method does not filter it just transforms the result set.
        /// <para/>
        /// Also applies a having clause.
        /// </summary>
        /// <param name="exprProcessor">processes each input event and returns output event</param>
        /// <param name="orderByProcessor">for sorting output events according to the order-by clause</param>
        /// <param name="events">input events</param>
        /// <param name="optionalHavingNode">supplies the having-clause expression</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>output events, one for each input event</returns>
        public static EventBean[] GetSelectEventsHaving(SelectExprProcessor exprProcessor,
                                                          OrderByProcessor orderByProcessor,
                                                          Set<MultiKey<EventBean>> events,
                                                          ExprNode optionalHavingNode,
                                                          bool isNewData,
                                                          bool isSynthesize)
	    {
	        if ((events == null) || (events.Count == 0))
	        {
	            return null;
	        }

	        List<EventBean> result = new List<EventBean>();
	        List<EventBean[]> eventGenerators = null;
	        if(orderByProcessor != null)
	        {
	            eventGenerators = new List<EventBean[]>();
	        }

	        foreach (MultiKey<EventBean> key in events)
	        {
	            EventBean[] eventsPerStream = key.Array;

                bool? passesHaving = (bool?)optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                if ((passesHaving == null) || (!passesHaving.Value))
	            {
	                continue;
	            }

	            EventBean resultEvent = exprProcessor.Process(eventsPerStream, isNewData, isSynthesize);
	            result.Add(resultEvent);
	            if(orderByProcessor != null)
	            {
	                eventGenerators.Add(eventsPerStream);
	            }
	        }

	        if (result.Count != 0)
	        {
	            if(orderByProcessor != null)
	            {
	                return orderByProcessor.Sort(result.ToArray(), eventGenerators.ToArray(), isNewData);
	            }
	            else
	            {
	                return result.ToArray();
	            }
	        }
	        else
	        {
	            return null;
	        }
	    }

        /// <summary>
        /// Returns the iterator implementing the group-by and aggregation and order-by logic
        /// specific to each case of use of these construct.
        /// </summary>
        /// <param name="parent">is the parent view iterator</param>
        /// <returns>event iterator</returns>
        public override IEnumerator<EventBean> GetEnumerator(Viewable parent)
	    {
	        if (orderByProcessor != null)
	        {
	            // Pull all events, generate order keys
	            EventBean[] eventsPerStream = new EventBean[1];
	            List<EventBean> events = new List<EventBean>();
	            List<MultiKeyUntyped> orderKeys = new List<MultiKeyUntyped>();
	            foreach (EventBean aParent in parent)
	            {
	                eventsPerStream[0] = aParent;
	                MultiKeyUntyped orderKey = orderByProcessor.GetSortKey(eventsPerStream, true);
	                UniformPair<EventBean[]> pair = ProcessViewResult(eventsPerStream, null, true);
	                events.Add(pair.First[0]);
	                orderKeys.Add(orderKey);
	            }

	            // sort
	            EventBean[] outgoingEvents = events.ToArray();
	            MultiKeyUntyped[] orderKeysArr = orderKeys.ToArray();
	            EventBean[] orderedEvents = orderByProcessor.Sort(outgoingEvents, orderKeysArr);

	            return ((IEnumerable<EventBean>) orderedEvents).GetEnumerator();
	        }
	        // Return an iterator that gives row-by-row a result
            ResultSetProcessorSimpleTransform transform = new ResultSetProcessorSimpleTransform(this);
            return TransformEventUtil.TransformEnumerator(parent.GetEnumerator(), transform.Transform);
	    }

        /// <summary>Returns the iterator for iterating over a join-result.</summary>
        /// <param name="joinSet">is the join result set</param>
        /// <returns>iterator over join results</returns>
        public override IEnumerator<EventBean> GetEnumerator(Set<MultiKey<EventBean>> joinSet)
	    {
	        // Process join results set as a regular join, includes sorting and having-clause filter
	        UniformPair<EventBean[]> result = ProcessJoinResult(joinSet, emptyRowSet, true);
	        return ((IEnumerable<EventBean>) result.First).GetEnumerator();
	    }

        /// <summary>Clear out current state.</summary>
        public override void Clear()
	    {
	        // No need to clear state, there is no state held
	    }

        /// <summary>
        /// Applies the select-clause to the given events returning the selected events. The number of events stays the
        /// same, i.e. this method does not filter it just transforms the result set.
        /// </summary>
        /// <param name="exprProcessor">processes each input event and returns output event</param>
        /// <param name="events">input events</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>output events, one for each input event</returns>
        public static EventBean[] GetSelectEventsNoHaving(SelectExprProcessor exprProcessor,
                                                            EventBean[] events,
                                                            bool isNewData,
                                                            bool isSynthesize)
	    {
	        if (events == null)
	        {
	            return null;
	        }

	        EventBean[] result = new EventBean[events.Length];
	        EventBean[] eventsPerStream = new EventBean[1];
	        for (int i = 0; i < events.Length; i++)
	        {
	            eventsPerStream[0] = events[i];
	            result[i] = exprProcessor.Process(eventsPerStream, isNewData, isSynthesize);
	        }

	        return result;
	    }

        /// <summary>
        /// Applies the select-clause to the given events returning the selected events. The number of events stays the
        /// same, i.e. this method does not filter it just transforms the result set.
        /// </summary>
        /// <param name="exprProcessor">processes each input event and returns output event</param>
        /// <param name="events">input events</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>output events, one for each input event</returns>
        public static EventBean[] GetSelectEventsNoHaving(SelectExprProcessor exprProcessor,
                                                            Set<MultiKey<EventBean>> events,
                                                            bool isNewData,
                                                            bool isSynthesize)
	    {
	        if ((events == null) || (events.Count == 0))
	        {
	            return null;
	        }
	        int length = events.Count;

	        EventBean[] result = new EventBean[length];

	        int count = 0;
	        foreach (MultiKey<EventBean> key in events)
	        {
	            EventBean[] eventsPerStream = key.Array;
	            result[count] = exprProcessor.Process(eventsPerStream, isNewData, isSynthesize);
	            count++;
	        }

	        return result;
	    }

        /// <summary>
        /// Applies the select-clause to the given events returning the selected events. The number of events stays the
        /// same, i.e. this method does not filter it just transforms the result set.
        /// <para/>
        /// Also applies a having clause.
        /// </summary>
        /// <param name="exprProcessor">processes each input event and returns output event</param>
        /// <param name="events">input events</param>
        /// <param name="optionalHavingNode">supplies the having-clause expression</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>output events, one for each input event</returns>
        public static EventBean[] GetSelectEventsHaving(SelectExprProcessor exprProcessor,
                                                        EventBean[] events,
                                                        ExprNode optionalHavingNode,
                                                        bool isNewData,
                                                        bool isSynthesize)
	    {
	        if (events == null)
	        {
	            return null;
	        }

	        List<EventBean> result = new List<EventBean>();

	        EventBean[] eventsPerStream = new EventBean[1];
	        foreach (EventBean @event in events)
	        {
	            eventsPerStream[0] = @event;

                bool? passesHaving = (bool?) optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                if ((passesHaving == null) || (!passesHaving.Value))
                {
	                continue;
	            }

	            result.Add(exprProcessor.Process(eventsPerStream, isNewData, isSynthesize));
	        }

            if (result.Count != 0)
            {
                return result.ToArray();
	        }
	        else
	        {
	            return null;
	        }
	    }

        /// <summary>
        /// Applies the select-clause to the given events returning the selected events. The number of events stays the
        /// same, i.e. this method does not filter it just transforms the result set.
        /// <para/>
        /// Also applies a having clause.
        /// </summary>
        /// <param name="exprProcessor">processes each input event and returns output event</param>
        /// <param name="events">input events</param>
        /// <param name="optionalHavingNode">supplies the having-clause expression</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>output events, one for each input event</returns>
	    public static EventBean[] GetSelectEventsHaving(SelectExprProcessor exprProcessor, Set<MultiKey<EventBean>> events, ExprNode optionalHavingNode, bool isNewData, bool isSynthesize)
	    {
	        List<EventBean> result = new List<EventBean>();

	        foreach (MultiKey<EventBean> key in events)
	        {
	            EventBean[] eventsPerStream = key.Array;

                bool? passesHaving = (bool?)optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                if ((passesHaving == null) || (!passesHaving.Value))
	            {
	                continue;
	            }

	            EventBean resultEvent = exprProcessor.Process(eventsPerStream, isNewData, isSynthesize);
	            result.Add(resultEvent);
	        }

            if (result.Count != 0)
            {
                return result.ToArray();
	        }
	        else
	        {
	            return null;
	        }
	    }

        /// <summary>
        /// Applies the select-clause to the given events returning the selected events. The number of events stays the
        /// same, i.e. this method does not filter it just transforms the result set.
        /// </summary>
        /// <param name="exprProcessor">
        /// processes each input event and returns output event
        /// </param>
        /// <param name="orderByProcessor">
        /// orders the outgoing events according to the order-by clause
        /// </param>
        /// <param name="events">input events</param>
        /// <param name="isNewData">
        /// indicates whether we are dealing with new data (istream) or old data (rstream)
        /// </param>
        /// <param name="isSynthesize">
        /// set to true to indicate that synthetic events are required for an iterator result set
        /// </param>
        /// <param name="result">is the result event list to populate</param>
        /// <param name="optSortKeys">
        /// is the result sort key list to populate, for sorting
        /// </param>
        public static void GetSelectEventsNoHaving(SelectExprProcessor exprProcessor,
                                                      OrderByProcessor orderByProcessor,
                                                      EventBean[] events,
                                                      bool isNewData,
                                                      bool isSynthesize,
                                                      List<EventBean> result,
                                                      List<MultiKeyUntyped> optSortKeys)
	    {
	        if (events == null)
	        {
	            return;
	        }

	        EventBean[] eventsPerStream = new EventBean[1];
	        foreach (EventBean @event in events)
	        {
	            eventsPerStream[0] = @event;

	            result.Add(exprProcessor.Process(eventsPerStream, isNewData, isSynthesize));
	            if (orderByProcessor != null)
	            {
	                optSortKeys.Add(orderByProcessor.GetSortKey(eventsPerStream, isNewData));
	            }
	        }
	    }

        /// <summary>
        /// Applies the select-clause to the given events returning the selected events. The number of events stays the
        /// same, i.e. this method does not filter it just transforms the result set.
        /// </summary>
        /// <param name="exprProcessor">
        /// processes each input event and returns output event
        /// </param>
        /// <param name="orderByProcessor">
        /// for sorting output events according to the order-by clause
        /// </param>
        /// <param name="events">input events</param>
        /// <param name="isNewData">
        /// indicates whether we are dealing with new data (istream) or old data (rstream)
        /// </param>
        /// <param name="isSynthesize">
        /// set to true to indicate that synthetic events are required for an iterator result set
        /// </param>
        /// <param name="result">is the result event list to populate</param>
        /// <param name="optSortKeys">
        /// is the result sort key list to populate, for sorting
        /// </param>
        public static void GetSelectEventsNoHaving(SelectExprProcessor exprProcessor,
                                                      OrderByProcessor orderByProcessor,
                                                      Set<MultiKey<EventBean>> events,
                                                      bool isNewData,
                                                      bool isSynthesize,
                                                      List<EventBean> result,
                                                      List<MultiKeyUntyped> optSortKeys)
	    {
	        int length = events.Count;
	        if (length == 0)
	        {
	            return;
	        }

	        foreach (MultiKey<EventBean> key in events)
	        {
	            EventBean[] eventsPerStream = key.Array;
	            result.Add(exprProcessor.Process(eventsPerStream, isNewData, isSynthesize));
	            if(orderByProcessor != null)
	            {
	                optSortKeys.Add(orderByProcessor.GetSortKey(eventsPerStream, isNewData));
	            }
	        }
	    }

        /// <summary>
        /// Applies the select-clause to the given events returning the selected events. The number of events stays the
        /// same, i.e. this method does not filter it just transforms the result set.
        /// <para/>
        /// Also applies a having clause.
        /// </summary>
        /// <param name="exprProcessor">processes each input event and returns output event</param>
        /// <param name="orderByProcessor">for sorting output events according to the order-by clause</param>
        /// <param name="events">input events</param>
        /// <param name="optionalHavingNode">supplies the having-clause expression</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <param name="result">is the result event list to populate</param>
        /// <param name="optSortKeys">is the result sort key list to populate, for sorting</param>
        public static void GetSelectEventsHaving(SelectExprProcessor exprProcessor,
                                                    OrderByProcessor orderByProcessor,
                                                    EventBean[] events,
                                                    ExprNode optionalHavingNode,
                                                    bool isNewData,
                                                    bool isSynthesize,
                                                    List<EventBean> result,
                                                    List<MultiKeyUntyped> optSortKeys)
	    {
	        if (events == null)
	        {
	            return;
	        }

	        EventBean[] eventsPerStream = new EventBean[1];
	        foreach (EventBean @event in events)
	        {
	            eventsPerStream[0] = @event;

                bool? passesHaving = (bool?)optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                if ((passesHaving == null) || (!passesHaving.Value))
	            {
	                continue;
	            }

	            result.Add(exprProcessor.Process(eventsPerStream, isNewData, isSynthesize));
	            if (orderByProcessor != null)
	            {
	                optSortKeys.Add(orderByProcessor.GetSortKey(eventsPerStream, isNewData));
	            }
	        }
	    }

        /// <summary>
        /// Applies the select-clause to the given events returning the selected events. The number of events stays the
        /// same, i.e. this method does not filter it just transforms the result set.
        /// <para/>
        /// Also applies a having clause.
        /// </summary>
        /// <param name="exprProcessor">processes each input event and returns output event</param>
        /// <param name="orderByProcessor">for sorting output events according to the order-by clause</param>
        /// <param name="events">input events</param>
        /// <param name="optionalHavingNode">supplies the having-clause expression</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <param name="result">is the result event list to populate</param>
        /// <param name="optSortKeys">is the result sort key list to populate, for sorting</param>
        public static void GetSelectEventsHaving(SelectExprProcessor exprProcessor,
                                                    OrderByProcessor orderByProcessor,
                                                    Set<MultiKey<EventBean>> events,
                                                    ExprNode optionalHavingNode,
                                                    bool isNewData,
                                                    bool isSynthesize,
                                                    List<EventBean> result,
                                                    List<MultiKeyUntyped> optSortKeys)
	    {
	        foreach (MultiKey<EventBean> key in events)
	        {
	            EventBean[] eventsPerStream = key.Array;

	            bool? passesHaving = (bool?) optionalHavingNode.Evaluate(eventsPerStream, isNewData);
                if ((passesHaving == null) || (!passesHaving.Value))
                {
	                continue;
	            }

	            EventBean resultEvent = exprProcessor.Process(eventsPerStream, isNewData, isSynthesize);
	            result.Add(resultEvent);
	            if(orderByProcessor != null)
	            {
	                optSortKeys.Add(orderByProcessor.GetSortKey(eventsPerStream, isNewData));
	            }
	        }
	    }
	}
} // End of namespace
