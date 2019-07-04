///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.events;
using com.espertech.esper.collection;
using com.espertech.esper.compat;

namespace com.espertech.esper.epl.join
{
	/// <summary> Interface for populating a join tuple result set from new data and old data for each stream.</summary>
	public interface JoinSetComposer
	{
        /// <summary>
        /// Provides initialization events per stream to composer to populate join indexes, if required
        /// </summary>
        /// <param name="eventsPerStream">is an array of events for each stream, with null elements to indicate no events for a stream</param>
        void Init(EventBean[][] eventsPerStream);

		/// <summary> Return join tuple result set from new data and old data for each stream.</summary>
		/// <param name="newDataPerStream">for each stream the event array (can be null).
		/// </param>
		/// <param name="oldDataPerStream">for each stream the event array (can be null).
		/// </param>
		/// <returns> join tuples
		/// </returns>
		UniformPair<Set<MultiKey<EventBean>>> Join( EventBean[][] newDataPerStream, EventBean[][] oldDataPerStream );

        /// <summary>For use in iteration over join statements, this must build a join tuple result set fromall events in indexes, executing query strategies for each.</summary>
        /// <returns>static join result</returns>
        Set<MultiKey<EventBean>> StaticJoin();

        /// <summary>Destroy stateful index tables, if any.</summary>
        void Destroy();
	}
}
