///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using com.espertech.esper.epl.agg;
using com.espertech.esper.epl.core;

namespace com.espertech.esper.epl.agg
{
    /// <summary>Sum for integer values.</summary>

    public class IntegerSumAggregator : AggregationMethod
    {
        private int sum;
        private long numDataPoints;

        public void Clear()
        {
            sum = 0;
            numDataPoints = 0;
        }

        /// <summary>
        /// Enters the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Enter(Object item)
        {
            if (item == null)
            {
                return;
            }
            numDataPoints++;
            sum += Convert.ToInt32(item);
        }

        /// <summary>
        /// Leaves the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Leave(Object item)
        {
            if (item == null)
            {
                return;
            }
            numDataPoints--;
            sum -= Convert.ToInt32(item);
        }

        /// <summary>
        /// Returns the current value held.
        /// </summary>
        /// <value></value>
        /// <returns>current value</returns>
        public Object Value
        {
            get
            {
                if (numDataPoints == 0)
                {
                    return null;
                }
                return sum;
            }
        }

        /// <summary>
        /// Returns the type of the current value.
        /// </summary>
        /// <value></value>
        /// <returns>type of values held</returns>
        public Type ValueType
        {
            get { return typeof(int?); }
        }

        /// <summary>
        /// Make a new, initalized aggregation state.
        /// </summary>
        /// <param name="methodResolutionService">for use in creating new aggregation method instances as a factory</param>
        /// <returns>initialized copy of the aggregator</returns>
        public AggregationMethod NewAggregator(MethodResolutionService methodResolutionService)
        {
            return methodResolutionService.MakeSumAggregator(typeof(int));
        }
    }
} // End of namespace