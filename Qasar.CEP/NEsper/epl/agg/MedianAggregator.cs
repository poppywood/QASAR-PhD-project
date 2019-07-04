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
    /// <summary>Median aggregation.</summary>
	public class MedianAggregator : AggregationMethod
	{
	    private SortedDoubleVector vector;

        public void Clear()
        {
            vector.Clear();
        }

	    /// <summary>Ctor.</summary>
	    public MedianAggregator()
	    {
	        this.vector = new SortedDoubleVector();
	    }

	    public void Enter(Object item)
	    {
            if (item == null)
	        {
	            return;
	        }
	        double value = Convert.ToDouble(item);
	        vector.Add(value);
	    }

	    public void Leave(Object item)
	    {
	        if (item == null)
	        {
	            return;
	        }
	        double value = Convert.ToDouble(item);
	        vector.Remove(value);
	    }

	    public Object Value
	    {
	    	get
	    	{
		        if (vector.Count == 0)
		        {
		            return null;
		        }
		        if (vector.Count == 1)
		        {
		        	return vector[0];
		        }
	
		        int middle = vector.Count >> 1;
		        if (vector.Count % 2 == 0)
		        {
		        	return (vector[middle - 1] + vector[middle]) / 2;
		        }
		        else
		        {
		        	return vector[middle];
		        }
	    	}
	    }

	    public Type ValueType
	    {
	        get { return typeof(double?); }
	    }

	    public AggregationMethod NewAggregator(MethodResolutionService methodResolutionService)
	    {
	        return methodResolutionService.MakeMedianAggregator();
	    }
	}
} // End of namespace
