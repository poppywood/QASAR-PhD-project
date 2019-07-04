///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using com.espertech.esper.collection;
using com.espertech.esper.epl.agg;
using com.espertech.esper.epl.core;

namespace com.espertech.esper.epl.agg
{
	/// <summary>
	/// AggregationMethod for use on top of another aggregator that handles unique value aggregation (versus all-value aggregation)
	/// for the underlying aggregator.
	/// </summary>
	public class DistinctValueAggregator : AggregationMethod
	{
	    private readonly AggregationMethod inner;
        private readonly Type childType;
	    private readonly RefCountedSet<Object> valueSet;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="inner">is the aggregator function computing aggregation values</param>
        /// <param name="childType">the return type of the inner expression to aggregate, if any</param>
	    public DistinctValueAggregator(AggregationMethod inner, Type childType)
	    {
	        this.inner = inner;
            this.childType = childType;
	        this.valueSet = new RefCountedSet<Object>();
	    }

        /// <summary>
        /// Clear out the collection.
        /// </summary>
        public void Clear()
        {
            valueSet.Clear();
        }

	    public void Enter(Object value)
	    {
	        // if value not already encountered, enter into aggregate
	        if (valueSet.Add(value))
	        {
	            inner.Enter(value);
	        }
	    }

	    public void Leave(Object value)
	    {
	        // if last reference to the value is removed, remove from aggregate
	        if (valueSet.Remove(value))
	        {
	            inner.Leave(value);
	        }
	    }

	    public Object Value
	    {
            get { return inner.Value; }
	    }

	    public Type ValueType
	    {
            get { return inner.ValueType; }
	    }

	    public AggregationMethod NewAggregator(MethodResolutionService methodResolutionService)
	    {
	        AggregationMethod innerCopy = inner.NewAggregator(methodResolutionService);
            return methodResolutionService.MakeDistinctAggregator(innerCopy, childType);
        }
	}
} // End of namespace
