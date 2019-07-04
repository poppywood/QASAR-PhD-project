using System;
using EventBean = com.espertech.esper.events.EventBean;
using EventType = com.espertech.esper.events.EventType;

namespace com.espertech.esper.epl.core
{
	/// <summary>
	/// Interface for processors of select-clause items, implementors are computing results based on matching events.
	/// </summary>
	public interface SelectExprProcessor
	{
		/// <summary> Returns the event type that represents the select-clause items.</summary>
		/// <returns> event type representing select-clause items
		/// </returns>
		EventType ResultEventType
		{
			get;
		}

        /// <summary>Computes the select-clause results and returns an event of the result event type that contains, in it'sproperties, the selected items.</summary>
        /// <param name="eventsPerStream">is per stream the event</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>event with properties containing selected items</returns>
        EventBean Process(EventBean[] eventsPerStream, bool isNewData, bool isSynthesize);
    }
}
