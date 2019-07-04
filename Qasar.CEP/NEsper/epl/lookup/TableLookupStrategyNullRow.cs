///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.compat;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.lookup
{
    /// <summary>
    /// Implementation for a table lookup strategy that returns exactly one row but leaves
    /// that row as an undefined value.
    /// </summary>
    public class TableLookupStrategyNullRow : TableLookupStrategy
    {
        private static readonly Set<EventBean> singleNullRowEventSet = new HashSet<EventBean>();
    
        static TableLookupStrategyNullRow()
        {
            singleNullRowEventSet.Add(null);
        }
    
        public Set<EventBean> Lookup(EventBean[] events) {
            return singleNullRowEventSet;        
        }
    }
}
