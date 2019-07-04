using System;
using FullTableScanLookupStrategy = com.espertech.esper.epl.join.exec.FullTableScanLookupStrategy;
using TableLookupStrategy = com.espertech.esper.epl.join.exec.TableLookupStrategy;
using EventTable = com.espertech.esper.epl.join.table.EventTable;
using UnindexedEventTable = com.espertech.esper.epl.join.table.UnindexedEventTable;
using EventType = com.espertech.esper.events.EventType;
namespace com.espertech.esper.epl.join.plan
{
	
	/// <summary> Plan for a full table scan.</summary>
	public class FullTableScanLookupPlan:TableLookupPlan
	{
		/// <summary> Ctor.</summary>
		/// <param name="lookupStream">stream that generates event to look up for
		/// </param>
		/// <param name="indexedStream">stream to full table scan
		/// </param>
		/// <param name="indexNum">index number for the table containing the full unindexed contents
		/// </param>
		public FullTableScanLookupPlan(int lookupStream, int indexedStream, int indexNum):base(lookupStream, indexedStream, indexNum)
		{
		}

        /// <summary>
        /// Instantiates the lookup plan into a execution strategy for the lookup.
        /// </summary>
        /// <param name="indexesPerStream">tables for each stream</param>
        /// <param name="eventTypes">types of events in stream</param>
        /// <returns>lookup strategy instance</returns>
		public override TableLookupStrategy MakeStrategy(EventTable[][] indexesPerStream, EventType[] eventTypes)
		{
			UnindexedEventTable index = (UnindexedEventTable) indexesPerStream[this.IndexedStream][this.IndexNum];
			return new FullTableScanLookupStrategy(index);
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
		public override String ToString()
		{
			return "FullTableScanLookupPlan " + base.ToString();
		}
	}
}