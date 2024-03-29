using System;

using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.epl.join.table;

namespace com.espertech.esper.epl.join.exec
{
    /// <summary>
    /// Lookup on an unindexed table returning the full table as matching events.
    /// </summary>
    
    public class FullTableScanLookupStrategy : TableLookupStrategy
    {
        private readonly UnindexedEventTable eventIndex;

        /// <summary>Ctor.</summary>
        /// <param name="eventIndex">table to use</param>

        public FullTableScanLookupStrategy(UnindexedEventTable eventIndex)
        {
            this.eventIndex = eventIndex;
        }

        /// <summary>
        /// Lookups the specified ev.
        /// </summary>
        /// <param name="ev">The ev.</param>
        /// <returns></returns>
        public Set<EventBean> Lookup(EventBean ev)
        {
            Set<EventBean> result = eventIndex.EventSet;
            if (result.IsEmpty)
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Returns the associated table. 
        /// </summary>
        /// <returns>table for lookup</returns>

        public UnindexedEventTable EventIndex
        {
            get { return eventIndex; }
        }
    }
}
