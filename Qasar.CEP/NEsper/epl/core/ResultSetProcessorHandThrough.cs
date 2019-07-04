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
using com.espertech.esper.events;
using com.espertech.esper.view;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Result set processor for the hand-through case: no aggregation functions used in
    /// the select clause, and no group-by, no having and ordering.
    /// </summary>
    public class ResultSetProcessorHandThrough : ResultSetProcessorBaseSimple
    {
        private readonly bool isSelectRStream;
        private readonly SelectExprProcessor selectExprProcessor;
        private readonly Set<MultiKey<EventBean>> emptyRowSet = new HashSet<MultiKey<EventBean>>();
    
        /// <summary>Ctor. </summary>
        /// <param name="selectExprProcessor">for processing the select expression and generting the readonly output rowsa row per group even if groups didn't change </param>
        /// <param name="isSelectRStream">true if remove stream events should be generated</param>
        public ResultSetProcessorHandThrough(SelectExprProcessor selectExprProcessor, bool isSelectRStream)
        {
            this.selectExprProcessor = selectExprProcessor;
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
    
            if (isSelectRStream)
            {
                selectOldEvents = GetSelectEventsNoHaving(selectExprProcessor, oldEvents, false, isSynthesize);
            }
            selectNewEvents = GetSelectEventsNoHaving(selectExprProcessor, newEvents, true, isSynthesize);
    
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
        public override UniformPair<EventBean[]> ProcessViewResult(EventBean[] newData,
                                                                   EventBean[] oldData,
                                                                   bool isSynthesize)
        {
            EventBean[] selectOldEvents = null;
            
            if (isSelectRStream)
            {
                selectOldEvents = GetSelectEventsNoHaving(selectExprProcessor, oldData, false, isSynthesize);
            }
            EventBean[] selectNewEvents = GetSelectEventsNoHaving(selectExprProcessor, newData, true, isSynthesize);
    
            return new UniformPair<EventBean[]>(selectNewEvents, selectOldEvents);
        }

        /// <summary>Applies the select-clause to the given events returning the selected events. The number of events stays the same, i.e. this method does not filter it just transforms the result set. </summary>
        /// <param name="exprProcessor">processes each input event and returns output event</param>
        /// <param name="events">input events</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>output events, one for each input event</returns>
        protected static EventBean[] GetSelectEventsNoHaving(SelectExprProcessor exprProcessor,
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
    
                // Wildcard select case
                if(exprProcessor == null)
                {
                    result[i] = events[i];
                }
                else
                {
                    result[i] = exprProcessor.Process(eventsPerStream, isNewData, isSynthesize);
                }
            }
    
            return result;
        }

        /// <summary>Applies the select-clause to the given events returning the selected events. The number of events stays the same, i.e. this method does not filter it just transforms the result set. </summary>
        /// <param name="exprProcessor">processes each input event and returns output event</param>
        /// <param name="events">input events</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>output events, one for each input event</returns>
        protected static EventBean[] GetSelectEventsNoHaving(SelectExprProcessor exprProcessor,
                                                             Set<MultiKey<EventBean>> events,
                                                             bool isNewData,
                                                             bool isSynthesize)
        {
            int length = events.Count;
            if (length == 0)
            {
                return null;
            }
    
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

        /// <summary>Clear out current state.</summary>
        public override void Clear()
        {
            // No need to clear state, there is no state held
        }
    
        public override IEnumerator<EventBean> GetEnumerator(Viewable parent)
        {
            // Return an iterator that gives row-by-row a result
            ResultSetProcessorSimpleTransform t = new ResultSetProcessorSimpleTransform(this);
            return TransformEventUtil.TransformEnumerator(parent.GetEnumerator(), t.Transform);
        }
    
        public override IEnumerator<EventBean> GetEnumerator(Set<MultiKey<EventBean>> joinSet)
        {
            // Process join results set as a regular join, includes sorting and having-clause filter
            UniformPair<EventBean[]> result = ProcessJoinResult(joinSet, emptyRowSet, true);
            if (result == null) return null;
            ICollection<EventBean> eventBeanCollection = (ICollection<EventBean>) result.First;
            if (eventBeanCollection == null) return null;
            return eventBeanCollection.GetEnumerator();
        }    
    }
}
