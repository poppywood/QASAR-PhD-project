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

namespace com.espertech.esper.epl.join
{
    /// <summary>
    /// Processes join tuple set by filtering out tuples.
    /// </summary>

    public class JoinSetFilter : JoinSetProcessor
    {
        private readonly ExprNode filterExprNode;

        /// <summary> Ctor.</summary>
        /// <param name="filterExprNode">filter tree
        /// </param>

        public JoinSetFilter(ExprNode filterExprNode)
        {
            this.filterExprNode = filterExprNode;
        }

        /// <summary>
        /// Process join result set.
        /// </summary>
        /// <param name="newEvents">set of event tuples representing new data</param>
        /// <param name="oldEvents">set of event tuples representing old data</param>
        public void Process(Set<MultiKey<EventBean>> newEvents, Set<MultiKey<EventBean>> oldEvents)
        {
            // Filter
            if (filterExprNode != null)
            {
                Filter(filterExprNode, newEvents, true);
                if (oldEvents != null)
                {
                    Filter(filterExprNode, oldEvents, false);
                }
            }
        }

        /// <summary> Filter event by applying the filter nodes evaluation method.</summary>
        /// <param name="filterExprNode">top node of the filter expression tree.</param>
        /// <param name="events">set of tuples of events</param>
        /// <param name="isNewData">true to indicate filter new data (istream) and not old data (rstream)</param>

        public static void Filter(ExprNode filterExprNode, Set<MultiKey<EventBean>> events, bool isNewData)
        {
            List<MultiKey<EventBean>> purgeList = null;
            
            foreach( MultiKey<EventBean> key in events )
            {
                EventBean[] eventArr = key.Array;

                bool? matched = (bool?)filterExprNode.Evaluate(eventArr, isNewData);

                if (!(matched ?? false))
                {
                    if (purgeList == null)
                    {
                        purgeList = new List<MultiKey<EventBean>>();
                    }

                    purgeList.Add(key);
                }
            }

            if (purgeList != null)
            {
                foreach (MultiKey<EventBean> key in purgeList)
                {
                    events.Remove(key);
                }
            }
        }
    }
}
