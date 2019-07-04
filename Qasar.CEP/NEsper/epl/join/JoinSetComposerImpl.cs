///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.join
{
    /// <summary> Implements the function to determine a join result set using tables/indexes and query strategy
    /// instances for each stream.
    /// </summary>

    public class JoinSetComposerImpl : JoinSetComposer
    {
        /// <summary> Returns tables.</summary>
        /// <returns> tables for stream.
        /// </returns>

        virtual public EventTable[][] Tables
        {
            get { return repositories; }
        }

        /// <summary> Returns query strategies.</summary>
        /// <returns> query strategies
        /// </returns>

        virtual public QueryStrategy[] QueryStrategies
        {
            get { return queryStrategies; }
        }

        private readonly EventTable[][] repositories;
        private readonly QueryStrategy[] queryStrategies;
        private readonly SelectClauseStreamSelectorEnum selectStreamSelectorEnum;

        // Set semantic eliminates duplicates in result set, use Linked set to preserve order
        private readonly Set<MultiKey<EventBean>> oldResults = new LinkedHashSet<MultiKey<EventBean>>();
        private readonly Set<MultiKey<EventBean>> newResults = new LinkedHashSet<MultiKey<EventBean>>();

        /// <summary> Ctor.</summary>
        /// <param name="repositories">for each stream an array of (indexed/unindexed) tables for lookup.
        /// </param>
        /// <param name="queryStrategies">for each stream a strategy to execute the join
        /// </param>
        /// <param name="selectStreamSelectorEnum">indicator for rstream or istream-only, for optimization
        /// </param>

        public JoinSetComposerImpl(EventTable[][] repositories, QueryStrategy[] queryStrategies, SelectClauseStreamSelectorEnum selectStreamSelectorEnum)
        {
            this.repositories = repositories;
            this.queryStrategies = queryStrategies;
            this.selectStreamSelectorEnum = selectStreamSelectorEnum;
        }

        /// <summary>
        /// Provides initialization events per stream to composer to populate join indexes, if required
        /// </summary>
        /// <param name="eventsPerStream">is an array of events for each stream, with null elements to indicate no events for a stream</param>
        public void Init(EventBean[][] eventsPerStream)
        {
            for (int i = 0; i < eventsPerStream.Length; i++)
            {
                if (eventsPerStream[i] != null)
                {
                    for (int j = 0; j < repositories[i].Length; j++)
                    {
                        repositories[i][j].Add((eventsPerStream[i]));
                    }
                }
            }
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
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

        /// <summary>
        /// Return join tuple result set from new data and old data for each stream.
        /// </summary>
        /// <param name="newDataPerStream">for each stream the event array (can be null).</param>
        /// <param name="oldDataPerStream">for each stream the event array (can be null).</param>
        /// <returns>join tuples</returns>
        public UniformPair<Set<MultiKey<EventBean>>> Join(EventBean[][] newDataPerStream, EventBean[][] oldDataPerStream)
        {
            oldResults.Clear();
            newResults.Clear();

            // join old data
            for (int i = 0; i < oldDataPerStream.Length; i++)
            {
                if (oldDataPerStream[i] != null)
                {
                    queryStrategies[i].Lookup(oldDataPerStream[i], oldResults);
                }
            }

            // add new data to indexes
            for (int i = 0; i < newDataPerStream.Length; i++)
            {
                if (newDataPerStream[i] != null)
                {
                    for (int j = 0; j < repositories[i].Length; j++)
                    {
                        repositories[i][j].Add(newDataPerStream[i]);
                    }
                }
            }

            // remove old data from indexes
            for (int i = 0; i < oldDataPerStream.Length; i++)
            {
                if (oldDataPerStream[i] != null)
                {
                    for (int j = 0; j < repositories[i].Length; j++)
                    {
                        repositories[i][j].Remove(oldDataPerStream[i]);
                    }
                }
            }

            // join new data
            for (int i = 0; i < newDataPerStream.Length; i++)
            {
                if (newDataPerStream[i] != null)
                {
                    queryStrategies[i].Lookup(newDataPerStream[i], newResults);
                }
            }

            return new UniformPair<Set<MultiKey<EventBean>>>(newResults, oldResults);
        }

        public Set<MultiKey<EventBean>> StaticJoin()
        {
            Set<MultiKey<EventBean>> result = new LinkedHashSet<MultiKey<EventBean>>();
            EventBean[] lookupEvents = new EventBean[1];

            // for each stream, perform query strategy
            for (int stream = 0; stream < queryStrategies.Length; stream++)
            {
                foreach( EventBean eventBean in repositories[stream][0] )
                {
                    lookupEvents[0] = eventBean;
                    queryStrategies[stream].Lookup(lookupEvents, result);
                }
            }

            return result;
        }
    }
}
