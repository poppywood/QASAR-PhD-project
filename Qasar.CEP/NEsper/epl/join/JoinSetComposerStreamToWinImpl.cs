///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.join
{
    /// <summary>Implements the function to determine a join result for a unidirectional stream-to-window joins, in which a single stream's events are every only evaluated using a query strategy. </summary>
    public class JoinSetComposerStreamToWinImpl : JoinSetComposer
    {
        private readonly EventTable[][] repositories;
        private readonly int streamNumber;
        private readonly QueryStrategy queryStrategy;
    
        private readonly Set<MultiKey<EventBean>> emptyResults = new LinkedHashSet<MultiKey<EventBean>>();
        private readonly Set<MultiKey<EventBean>> newResults = new LinkedHashSet<MultiKey<EventBean>>();
    
        /// <summary>Ctor. </summary>
        /// <param name="repositories">for each stream an array of (indexed/unindexed) tables for lookup.</param>
        /// <param name="streamNumber">is the undirectional stream</param>
        /// <param name="queryStrategy">is the lookup query strategy for the stream</param>
        public JoinSetComposerStreamToWinImpl(EventTable[][] repositories, int streamNumber, QueryStrategy queryStrategy)
        {
            this.repositories = repositories;
            this.streamNumber = streamNumber;
            this.queryStrategy = queryStrategy;
        }
    
        public void Init(EventBean[][] eventsPerStream)
        {
            for (int i = 0; i < eventsPerStream.Length; i++)
            {
                if ((eventsPerStream[i] != null) && (i != streamNumber))
                {
                    for (int j = 0; j < repositories[i].Length; j++)
                    {
                        repositories[i][j].Add((eventsPerStream[i]));
                    }
                }
            }
        }
    
        public void Destroy()
        {
            for (int i = 0; i < repositories.Length; i++)
            {
                foreach (EventTable table in repositories[i])
                {
                    table.Clear();
                }
            }
        }
    
        public UniformPair<Set<MultiKey<EventBean>>> Join(EventBean[][] newDataPerStream, EventBean[][] oldDataPerStream)
        {
            newResults.Clear();
    
            // add new data to indexes
            for (int i = 0; i < newDataPerStream.Length; i++)
            {
                if ((newDataPerStream[i] != null) && (i != streamNumber))
                {
                    for (int j = 0; j < repositories[i].Length; j++)
                    {
                        repositories[i][j].Add((newDataPerStream[i]));
                    }
                }
            }
    
            // remove old data from indexes
            // adding first and then removing as the events added may be remove right away
            for (int i = 0; i < oldDataPerStream.Length; i++)
            {
                if ((oldDataPerStream[i] != null) && (i != streamNumber))
                {
                    for (int j = 0; j < repositories[i].Length; j++)
                    {
                        repositories[i][j].Remove(oldDataPerStream[i]);
                    }
                }
            }
    
            // join new data
            if (newDataPerStream[streamNumber] != null)
            {
                queryStrategy.Lookup(newDataPerStream[streamNumber], newResults);
            }
    
            return new UniformPair<Set<MultiKey<EventBean>>>(newResults, emptyResults);
        }
    
        public Set<MultiKey<EventBean>> StaticJoin()
        {
            throw new UnsupportedOperationException("Iteration over a unidirectional join is not supported");
        }
    }
}
