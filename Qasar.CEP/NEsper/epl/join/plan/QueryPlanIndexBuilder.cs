using System;
using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.collection;

namespace com.espertech.esper.epl.join.plan
{
    /// <summary>
    /// Build query index plans.
    /// </summary>
    
    public class QueryPlanIndexBuilder
    {
        /// <summary>
        /// Build index specification from navigability info.
        /// <para>
        /// Looks at each stream and determines which properties in the stream must be indexed
        /// in order for other streams to look up into the stream. Determines the unique set of properties
        /// to avoid building duplicate indexes on the same set of properties.
        /// </para>
        /// </summary>
        /// <param name="queryGraph">navigability info</param>
        /// <returns>query index specs for each stream</returns>
        public static QueryPlanIndex[] BuildIndexSpec(QueryGraph queryGraph)
        {
            int numStreams = queryGraph.NumStreams;
            QueryPlanIndex[] indexSpecs = new QueryPlanIndex[numStreams];

            // For each stream compile a list of index property sets.
            for (int streamIndexed = 0; streamIndexed < numStreams; streamIndexed++)
            {
                Set<MultiKey<String>> indexesSetSortedProps = new HashSet<MultiKey<String>>();
                List<String[]> indexesList = new List<String[]>();

                // Look at the index from the viewpoint of the stream looking up in the index
                for (int streamLookup = 0; streamLookup < numStreams; streamLookup++)
                {
                    if (streamIndexed == streamLookup)
                    {
                        continue;
                    }

                    // Sort index properties, but use the sorted properties only to eliminate duplicates
                    String[] indexProps = queryGraph.GetIndexProperties(streamLookup, streamIndexed);
                    if (indexProps != null)
                    {
                        String[] indexPropsSorted = new String[indexProps.Length];
                        Array.Copy(indexProps, 0, indexPropsSorted, 0, indexProps.Length);
                        Array.Sort(indexPropsSorted);

                        // Eliminate duplicates by sorting and using a set
                        MultiKey<String> indexPropsUniqueKey = new MultiKey<String>(indexPropsSorted);
                        if (!indexesSetSortedProps.Contains(indexPropsUniqueKey))
                        {
                            indexesSetSortedProps.Add(indexPropsUniqueKey);
                            indexesList.Add(indexProps);
                        }
                    }
                }

                // Copy the index properties for the stream to a QueryPlanIndex instance
                {
                    String[][] indexProps = null;
                    if (indexesList.Count != 0)
                    {
                        indexProps = new String[indexesList.Count][];
                        int count = 0;
                        foreach (String[] entry in indexesList)
                        {
                            indexProps[count] = entry;
                            count++;
                        }
                    }
                    else
                    {
                        // There are no indexes, create a default table for the event set
                        indexProps = new String[1][];
                        indexProps[0] = new String[0];
                    }
                    indexSpecs[streamIndexed] = new QueryPlanIndex(indexProps, new Type[indexProps.Length][]);
                }
            }

            return indexSpecs;
        }
    }
}
