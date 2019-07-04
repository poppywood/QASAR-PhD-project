using System;
using System.Collections.Generic;

using com.espertech.esper.epl;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.type;

using log4net;

namespace com.espertech.esper.epl.join.plan
{
	/// <summary>
    /// Build a query plan based on filtering information.
    /// </summary>
    
    public class QueryPlanBuilder
    {
        /// <summary>
        /// Build query plan using the filter.
        /// </summary>
        /// <param name="typesPerStream">The types per stream.</param>
        /// <param name="outerJoinDescList">list of outer join criteria, or null if there are no outer joins</param>
        /// <param name="optionalFilterNode">filter tree</param>
        /// <param name="streamNames">names of streams</param>
        /// <returns>query plan</returns>

	    public static QueryPlan GetPlan(EventType[] typesPerStream,
	                                    IList<OuterJoinDesc> outerJoinDescList,
	                                    ExprNode optionalFilterNode,
	                                    String[] streamNames)
	    {
            String methodName = ".getPlan ";

			int numStreams = typesPerStream.Length;
            if (numStreams < 2)
            {
                throw new ArgumentException("Number of join stream types is less then 2");
            }
            if (outerJoinDescList.Count >= numStreams)
            {
                throw new ArgumentException("Too many outer join descriptors found");
            }

            QueryGraph queryGraph = new QueryGraph(numStreams);

            // For outer joins the query graph will just contain outer join relationships
            if (outerJoinDescList.Count != 0)
            {
                OuterJoinAnalyzer.Analyze(outerJoinDescList, queryGraph);
                if (log.IsDebugEnabled)
                {
                    log.Debug(methodName + " After outer join queryGraph=\n" + queryGraph);
                }
            }
            else
            {
                // Let the query graph reflect the where-clause
                if (optionalFilterNode != null)
                {
                    // Analyze relationships between streams using the optional filter expression.
                    // Relationships are properties in AND and EQUALS nodes of joins.
                    FilterExprAnalyzer.Analyze(optionalFilterNode, queryGraph);
                    if (log.IsDebugEnabled)
                    {
                        log.Debug(methodName + "After filter expression queryGraph=\n" + queryGraph);
                    }

                    // Add navigation entries based on key and index property equivalency (a=b, b=c follows a=c)
                    QueryGraph.FillEquivalentNav(queryGraph);
                    if (log.IsDebugEnabled)
                    {
                        log.Debug(methodName + "After fill equiv. nav. queryGraph=\n" + queryGraph);
                    }
                }
            }

            if (numStreams == 2)
            {
                OuterJoinType? outerJoinType = null;
                if (outerJoinDescList.Count != 0)
                {
                    outerJoinType = outerJoinDescList[0].OuterJoinType;
                }

				QueryPlan queryPlan = TwoStreamQueryPlanBuilder.Build(typesPerStream, queryGraph, outerJoinType);

                if (log.IsDebugEnabled)
                {
                    log.Debug(methodName + "2-Stream queryPlan=" + queryPlan);
                }
                return queryPlan;
            }

            if (outerJoinDescList.Count == 0)
            {
                QueryPlan queryPlan = NStreamQueryPlanBuilder.Build(queryGraph, typesPerStream);

                if (log.IsDebugEnabled)
                {
                    log.Debug(methodName + "N-Stream no-outer-join queryPlan=" + queryPlan);
                }

                return queryPlan;
            }

            return NStreamOuterQueryPlanBuilder.Build(queryGraph, outerJoinDescList, streamNames, typesPerStream);
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
