///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.compat;
using com.espertech.esper.collection;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.join
{
	/// <summary> Join execution strategy based on a 3-step GetSelectListEvents of composing a join set, filtering the join set and
	/// indicating.
	/// </summary>
	public class JoinExecutionStrategyImpl : JoinExecutionStrategy
	{
		private readonly JoinSetComposer composer;
		private readonly JoinSetProcessor filter;
		private readonly JoinSetProcessor indicator;
		
		/// <summary> Ctor.</summary>
		/// <param name="composer">determines join tuple set
		/// </param>
		/// <param name="filter">for filtering among tuples
		/// </param>
		/// <param name="indicator">for presenting the info to a view
		/// </param>
		
		public JoinExecutionStrategyImpl(JoinSetComposer composer, JoinSetProcessor filter, JoinSetProcessor indicator)
		{
			this.composer = composer;
			this.filter = filter;
			this.indicator = indicator;
		}

        /// <summary>
        /// Execute join. The first dimension in the 2-dim arrays is the stream that generated the events,
        /// and the second dimension is the actual events generated.
        /// </summary>
        /// <param name="newDataPerStream">new events for each stream</param>
        /// <param name="oldDataPerStream">old events for each stream</param>
		public virtual void Join(EventBean[][] newDataPerStream, EventBean[][] oldDataPerStream)
		{
			UniformPair<Set<MultiKey<EventBean>>> joinSet = composer.Join( newDataPerStream, oldDataPerStream );
			
			filter.Process(joinSet.First, joinSet.Second);
			
			if ((!joinSet.First.IsEmpty) || (!joinSet.Second.IsEmpty))
			{
				indicator.Process(joinSet.First, joinSet.Second);
			}
		}

        public Set<MultiKey<EventBean>> StaticJoin()
        {
            Set<MultiKey<EventBean>> joinSet = composer.StaticJoin();
            filter.Process(joinSet, null);
            return joinSet;
        }
    }
}
