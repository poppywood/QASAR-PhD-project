using System;

using net.esper.events;
using net.esper.collection;
using net.esper.compat;

namespace net.esper.eql.join
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
	}
}
