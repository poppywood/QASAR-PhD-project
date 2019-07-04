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
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.view;
using log4net;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Result set processor for the simplest case: no aggregation functions used in the select
    /// clause, and no group-by. <para/> The processor generates one row for each event entering
    /// (new event) and one row for each event leaving (old event).
    /// </summary>
    public abstract class ResultSetProcessorBaseSimple : ResultSetProcessor
    {
        /// <summary>Clear out current state.</summary>
        public virtual void Clear()
        {
            // No need to clear state, there is no state held
        }

        #region "Abstract Methods"
        /// <summary>
        /// Returns the event type of processed results.
        /// </summary>
        /// <value>The type of the result event.</value>
        /// <returns> event type of the resulting events posted by the processor.
        /// </returns>
        public abstract EventType ResultEventType { get; }

        /// <summary>
        /// For use by views posting their result, process the event rows that are entered and removed (new and old events).
        /// Processes according to select-clauses, group-by clauses and having-clauses and returns new events and
        /// old events as specified.
        /// </summary>
        /// <param name="newData">new events posted by view</param>
        /// <param name="oldData">old events posted by view</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>pair of new events and old events</returns>
        public abstract UniformPair<EventBean[]> ProcessViewResult(EventBean[] newData,
                                                                   EventBean[] oldData,
                                                                   bool isSynthesize);

        /// <summary>
        /// For use by joins posting their result, process the event rows that are entered and removed (new and old events).
        /// Processes according to select-clauses, group-by clauses and having-clauses and returns new events and
        /// old events as specified.
        /// </summary>
        /// <param name="newEvents">new events posted by join</param>
        /// <param name="oldEvents">old events posted by join</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>pair of new events and old events</returns>
        public abstract UniformPair<EventBean[]> ProcessJoinResult(Set<MultiKey<EventBean>> newEvents,
                                                                   Set<MultiKey<EventBean>> oldEvents,
                                                                   bool isSynthesize);

        /// <summary>
        /// Returns the iterator implementing the group-by and aggregation and order-by logic
        /// specific to each case of use of these construct.
        /// </summary>
        /// <param name="parent">is the parent view iterator</param>
        /// <returns>event iterator</returns>
        public abstract IEnumerator<EventBean> GetEnumerator(Viewable parent);

        /// <summary>Returns the iterator for iterating over a join-result.</summary>
        /// <param name="joinSet">is the join result set</param>
        /// <returns>iterator over join results</returns>
        public abstract IEnumerator<EventBean> GetEnumerator(Set<MultiKey<EventBean>> joinSet);

        #endregion

        /// <summary>Processes batched events in case of output-rate limiting.</summary>
        /// <param name="joinEventsSet">the join results</param>
        /// <param name="generateSynthetic">flag to indicate whether synthetic events must be generated</param>
        /// <param name="outputLimitLimitType">the type of output rate limiting</param>
        /// <returns>results for dispatch</returns>
        public virtual UniformPair<EventBean[]> ProcessOutputLimitedJoin(
            IList<UniformPair<Set<MultiKey<EventBean>>>> joinEventsSet,
            bool generateSynthetic,
            OutputLimitLimitType outputLimitLimitType)
        {
            if (outputLimitLimitType != OutputLimitLimitType.LAST)
            {
                UniformPair<Set<MultiKey<EventBean>>> flattened = EventBeanUtility.FlattenBatchJoin(joinEventsSet);
                return ProcessJoinResult(flattened.First, flattened.Second, generateSynthetic);
            }

            // Determine the last event of the insert and remove stream that matches having-criteria
            int index = joinEventsSet.Count - 1;
            EventBean lastNonEmptyNew = null;
            EventBean lastNonEmptyOld = null;
            while (index >= 0)
            {
                UniformPair<Set<MultiKey<EventBean>>> pair = joinEventsSet[index];
                if (((pair.First != null) && (!pair.First.IsEmpty) && (lastNonEmptyNew == null)) ||
                     ((pair.Second != null) && (!pair.Second.IsEmpty) && (lastNonEmptyOld == null)))
                {
                    UniformPair<EventBean[]> result = ProcessJoinResult(pair.First, pair.Second, generateSynthetic);

                    if ((lastNonEmptyNew == null) && (result != null) && (result.First != null) && (result.First.Length > 0))
                    {
                        lastNonEmptyNew = result.First[result.First.Length - 1];
                    }
                    if ((lastNonEmptyOld == null) && (result != null) && (result.Second != null) && (result.Second.Length > 0))
                    {
                        lastNonEmptyOld = result.Second[result.Second.Length - 1];
                    }
                }
                if ((lastNonEmptyNew != null) && (lastNonEmptyOld != null))
                {
                    break;
                }
                index--;
            }

            EventBean[] lastNew = null;
            if (lastNonEmptyNew != null)
            {
                lastNew = new EventBean[] { lastNonEmptyNew };
            }
            EventBean[] lastOld = null;
            if (lastNonEmptyOld != null)
            {
                lastOld = new EventBean[] { lastNonEmptyOld };
            }

            return new UniformPair<EventBean[]>(lastNew, lastOld);
        }

        /// <summary>Processes batched events in case of output-rate limiting.</summary>
        /// <param name="viewEventsList">the view results</param>
        /// <param name="generateSynthetic">flag to indicate whether synthetic events must be generated</param>
        /// <param name="outputLimitLimitType">the type of output rate limiting</param>
        /// <returns>results for dispatch</returns>
        public virtual UniformPair<EventBean[]> ProcessOutputLimitedView(
            IList<UniformPair<EventBean[]>> viewEventsList,
            bool generateSynthetic,
            OutputLimitLimitType outputLimitLimitType)
        {
            if (outputLimitLimitType != OutputLimitLimitType.LAST)
            {
                UniformPair<EventBean[]> pair = EventBeanUtility.FlattenBatchStream(viewEventsList);
                return ProcessViewResult(pair.First, pair.Second, generateSynthetic);
            }

            // Determine the last event of the insert and remove stream that matches having-criteria
            int index = viewEventsList.Count - 1;
            EventBean lastNonEmptyNew = null;
            EventBean lastNonEmptyOld = null;
            while (index >= 0)
            {
                UniformPair<EventBean[]> pair = viewEventsList[index];
                if (((pair.First != null) && (pair.First.Length != 0) && (lastNonEmptyNew == null)) ||
                     ((pair.Second != null) && (pair.Second.Length != 0) && (lastNonEmptyOld == null)))
                {
                    UniformPair<EventBean[]> result = ProcessViewResult(pair.First, pair.Second, generateSynthetic);

                    if ((lastNonEmptyNew == null) && (result != null) && (result.First != null) && (result.First.Length > 0))
                    {
                        lastNonEmptyNew = result.First[result.First.Length - 1];
                    }
                    if ((lastNonEmptyOld == null) && (result != null) && (result.Second != null) && (result.Second.Length > 0))
                    {
                        lastNonEmptyOld = result.Second[result.Second.Length - 1];
                    }
                }
                if ((lastNonEmptyNew != null) && (lastNonEmptyOld != null))
                {
                    break;
                }
                index--;
            }

            EventBean[] lastNew = null;
            if (lastNonEmptyNew != null)
            {
                lastNew = new EventBean[] { lastNonEmptyNew };
            }
            EventBean[] lastOld = null;
            if (lastNonEmptyOld != null)
            {
                lastOld = new EventBean[] { lastNonEmptyOld };
            }

            return new UniformPair<EventBean[]>(lastNew, lastOld);
        }
    }
}