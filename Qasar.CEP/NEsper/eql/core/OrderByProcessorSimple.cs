///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using net.esper.compat;
using net.esper.collection;
using net.esper.eql.spec;
using net.esper.events;
using net.esper.util;

using net.esper.eql.agg;
using net.esper.eql.core;
using net.esper.eql.expression;

using Log = org.apache.commons.logging.Log;
using LogFactory = org.apache.commons.logging.LogFactory;

namespace net.esper.eql.core
{
    /// <summary>
    /// An order-by processor that sorts events according to the expressions
    /// in the order_by clause.
    /// </summary>

    public class OrderByProcessorSimple : OrderByProcessor
    {
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IList<OrderByItem> orderByList;
        private readonly IList<ExprNode> groupByNodes;
        private readonly Boolean needsGroupByKeys;
        private readonly AggregationService aggregationService;

        private readonly IComparer<MultiKeyUntyped> comparator;

        /// <summary>Ctor.</summary>
        /// <param name="orderByList">the nodes that generate the keys to sort events on</param>
        /// <param name="groupByNodes">generate the keys for determining aggregation groups</param>
        /// <param name="needsGroupByKeys">indicates whether this processor needs to have individualgroup by keys to evaluate the sort condition successfully</param>
        /// <param name="aggregationService">used to evaluate aggregate functions in the group-by andsort-by clauses</param>

        public OrderByProcessorSimple(IList<OrderByItem> orderByList,
                                      IList<ExprNode> groupByNodes,
                                      Boolean needsGroupByKeys,
                                      AggregationService aggregationService)
        {
            this.orderByList = orderByList;
            this.groupByNodes = groupByNodes;
            this.needsGroupByKeys = needsGroupByKeys;
            this.aggregationService = aggregationService;

			this.comparator = new MultiKeyComparator(IsDescendingValues);
        }

        public MultiKeyUntyped GetSortKey(EventBean[] eventsPerStream, bool isNewData)
        {
            Object[] values = new Object[orderByList.Count];
            int count = 0;
            foreach (OrderByItem sortPair in orderByList)
            {
                ExprNode sortNode = sortPair.ExprNode;
                values[count++] = sortNode.Evaluate(eventsPerStream, isNewData);
            }

            return new MultiKeyUntyped(values);
        }

        public MultiKeyUntyped[] GetSortKeyPerRow(EventBean[] generatingEvents, bool isNewData)
        {
            if (generatingEvents == null)
            {
                return null;
            }

            MultiKeyUntyped[] sortProperties = new MultiKeyUntyped[generatingEvents.Length];

            int count = 0;
            EventBean[] evalEventsPerStream = new EventBean[1];
            foreach (EventBean _event in generatingEvents)
            {
                Object[] values = new Object[orderByList.Count];
                int countTwo = 0;
                evalEventsPerStream[0] = _event;
                foreach (OrderByItem sortPair in orderByList)
                {
                    ExprNode sortNode = sortPair.ExprNode;
                    values[countTwo++] = sortNode.Evaluate(evalEventsPerStream, isNewData);
                }

                sortProperties[count] = new MultiKeyUntyped(values);
                count++;
            }

            return sortProperties;
        }

        /// <summary>
        /// Sorts the specified outgoing events.
        /// </summary>
        /// <param name="outgoingEvents">The outgoing events.</param>
        /// <param name="generatingEvents">The generating events.</param>
		/// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <returns></returns>
        
        public EventBean[] Sort(EventBean[] outgoingEvents,
								EventBean[][] generatingEvents,
								bool isNewData)
        {
            if (outgoingEvents == null || outgoingEvents.Length < 2)
            {
                return outgoingEvents;
            }

            // Get the group by keys if needed
            MultiKeyUntyped[] groupByKeys = null;
            if (needsGroupByKeys)
            {
                groupByKeys = GenerateGroupKeys(generatingEvents, isNewData);
            }

            return Sort(outgoingEvents, generatingEvents, groupByKeys, isNewData);
        }

        /// <summary>
        /// Sorts the specified outgoing events.
        /// </summary>
        /// <param name="outgoingEvents">The outgoing events.</param>
        /// <param name="generatingEvents">The generating events.</param>
        /// <param name="groupByKeys">The group by keys.</param>
		/// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <returns></returns>
        
        public EventBean[] Sort(EventBean[] outgoingEvents,
								EventBean[][] generatingEvents,
								MultiKeyUntyped[] groupByKeys,
								bool isNewData)
        {
            if (ExecutionPathDebugLog.IsEnabled && log.IsDebugEnabled)
            {
                log.Debug(".sort");
            }

            if (outgoingEvents == null || outgoingEvents.Length < 2)
            {
                return outgoingEvents;
            }

            // Create the multikeys of sort values
            MultiKeyUntyped[] sortValuesMultiKeys = CreateSortProperties(generatingEvents, groupByKeys, isNewData);

            // Map the sort values to the corresponding outgoing events
            IDictionary<MultiKeyUntyped, IList<EventBean>> sortToOutgoing = new Dictionary<MultiKeyUntyped, IList<EventBean>>();
            int countOne = 0;
            foreach (MultiKeyUntyped sortValues in sortValuesMultiKeys)
            {
                IList<EventBean> list;
                if ( ! sortToOutgoing.TryGetValue( sortValues, out list ) )
                {
                    list = new List<EventBean>();
                }
                list.Add(outgoingEvents[countOne++]);
                sortToOutgoing[sortValues] = list;
            }

            // Sort the sort values
			Array.Sort( sortValuesMultiKeys, comparator );

            // Sort the outgoing events in the same order
            Set<MultiKeyUntyped> sortSet = new LinkedHashSet<MultiKeyUntyped>() ;
            sortSet.AddAll( sortValuesMultiKeys );
            
            EventBean[] result = new EventBean[outgoingEvents.Length];
            int countTwo = 0;
            foreach (MultiKeyUntyped sortValues in sortSet)
            {
                IList<EventBean> output;
                if ( sortToOutgoing.TryGetValue( sortValues, out output ) )
                {
	                foreach (EventBean ev in output)
	                {
	                    result[countTwo++] = ev;
	                }
                }
            }

            return result;
        }

        /// <summary>Compares values for sorting.</summary>
        /// <param name="valueOne">first value to compare, can be null</param>
        /// <param name="valueTwo">second value to compare, can be null</param>
        /// <param name="descending">true if ascending, false if descending</param>
        /// <returns>0 if equal, -1 if smaller, +1 if larger</returns>

        protected static int CompareValues(Object valueOne, Object valueTwo, Boolean descending)
        {
            if (descending)
            {
                Object temp = valueOne;
                valueOne = valueTwo;
                valueTwo = temp;
            }

            if (valueOne == null || valueTwo == null)
            {
                // A null value is considered equal to another null
                // value and smaller than any nonnull value
                if (valueOne == null && valueTwo == null)
                {
                    return 0;
                }
                if (valueOne == null)
                {
                    return -1;
                }
                return 1;
            }

            IComparable comparable1 = valueOne as IComparable;
            if ( comparable1 == null )
            {
            	throw new InvalidCastException("Sort by clause cannot sort objects of type " + valueOne.GetType());
            }

            return comparable1.CompareTo( valueTwo ) ;
        }

        private MultiKeyUntyped[] CreateSortProperties(
            EventBean[][] generatingEvents,
            MultiKeyUntyped[] groupByKeys,
			bool isNewData)
        {
            MultiKeyUntyped[] sortProperties = new MultiKeyUntyped[generatingEvents.Length];

            int count = 0;
            foreach (EventBean[] eventsPerStream in generatingEvents)
            {
                // Make a new multikey that contains the sort-by values.
                if (needsGroupByKeys)
                {
                    aggregationService.SetCurrentRow(groupByKeys[count]);
                }

                Object[] values = new Object[orderByList.Count];
                int countTwo = 0;
                foreach (OrderByItem sortPair in orderByList)
                {
                    ExprNode sortNode = sortPair.ExprNode;
                    values[countTwo++] = sortNode.Evaluate(eventsPerStream, isNewData);
                }

                sortProperties[count] = new MultiKeyUntyped(values);
                count++;
            }

            return sortProperties;
        }

        private MultiKeyUntyped GenerateGroupKey(EventBean[] eventsPerStream, bool isNewData)
        {
            Object[] keys = new Object[groupByNodes.Count];

            int count = 0;
            foreach (ExprNode exprNode in groupByNodes)
            {
                keys[count] = exprNode.Evaluate(eventsPerStream, isNewData);
                count++;
            }

            return new MultiKeyUntyped(keys);
        }

        /// <summary>
        /// Sort a given array of outgoing events using the sort keys returning a sorted outgoing event array.
        /// </summary>
        /// <param name="outgoingEvents">is the events to sort</param>
        /// <param name="orderKeys">is the keys to sort by</param>
        /// <returns>sorted events</returns>
        public EventBean[] Sort(EventBean[] outgoingEvents, MultiKeyUntyped[] orderKeys)
        {
            TreeDictionary<MultiKeyUntyped, Object> sort = new TreeDictionary<MultiKeyUntyped, Object>(comparator);

            if (outgoingEvents == null || outgoingEvents.Length < 2)
            {
                return outgoingEvents;
            }

            for (int i = 0; i < outgoingEvents.Length; i++)
            {
                Object entry = sort.Get(orderKeys[i]);
                if (entry == null)
                {
                    sort.Put(orderKeys[i], outgoingEvents[i]);
                }
                else if (entry is EventBean)
                {
                    List<EventBean> list = new List<EventBean>();
                    list.Add((EventBean) entry);
                    list.Add(outgoingEvents[i]);
                    sort.Put(orderKeys[i], list);
                }
                else
                {
                    List<EventBean> list = (List<EventBean>) entry;
                    list.Add(outgoingEvents[i]);
                }
            }

            EventBean[] result = new EventBean[outgoingEvents.Length];
            int count = 0;
            foreach (Object entry in sort.Values)
            {
                if (entry is List<EventBean>)
                {
                    List<EventBean> output = (List<EventBean>) entry;
                    foreach (EventBean _event in output)
                    {
                        result[count++] = _event;
                    }
                }
                else
                {
                    result[count++] = (EventBean) entry;
                }
            }
            return result;
        }

        private MultiKeyUntyped[] GenerateGroupKeys(EventBean[][] generatingEvents, bool isNewData)
        {
            MultiKeyUntyped[] keys = new MultiKeyUntyped[generatingEvents.Length];

            int count = 0;
            foreach (EventBean[] eventsPerStream in generatingEvents)
            {
                keys[count++] = GenerateGroupKey(eventsPerStream, isNewData);
            }

            return keys;
        }

        private bool[] IsDescendingValues
        {
            get
            {
                bool[] isDescendingValues = new bool[orderByList.Count];
                int count = 0;
                foreach (OrderByItem pair in orderByList)
                {
                    isDescendingValues[count++] = pair.IsDescending;
                }
                return isDescendingValues;
            }
        }
    }
}
