using System;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.join
{
	/// <summary>
    /// Processes a join result set constisting of sets of tuples of events.
    /// </summary>
	
    public interface JoinSetProcessor
	{
		/// <summary> Process join result set.</summary>
		/// <param name="newEvents">set of event tuples representing new data
		/// </param>
		/// <param name="oldEvents">set of event tuples representing old data
		/// </param>
		void  Process( Set<MultiKey<EventBean>> newEvents, Set<MultiKey<EventBean>> oldEvents);
	}
}
