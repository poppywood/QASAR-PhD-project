using System;
using System.Collections.Generic;

using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;

namespace com.espertech.esper.epl.join.plan
{
	/// <summary>
	/// Analyzes an outer join descriptor list and builds a query graph model from it.
	/// The 'on' expression identifiers are extracted and placed in the query graph
	/// model as navigable relationships (by key and index properties) between streams.
	/// </summary>

	public class OuterJoinAnalyzer
	{
		/// <summary> Analyzes the outer join descriptor list to build a query graph model.</summary>
		/// <param name="outerJoinDescList">list of outer join descriptors
		/// </param>
		/// <param name="queryGraph">model containing relationships between streams that is written into
		/// </param>
		/// <returns> queryGraph object
		/// </returns>
		public static QueryGraph Analyze( IList<OuterJoinDesc> outerJoinDescList, QueryGraph queryGraph )
		{
			foreach ( OuterJoinDesc outerJoinDesc in outerJoinDescList )
			{
                ExprIdentNode identNodeLeft = outerJoinDesc.LeftNode;
                ExprIdentNode identNodeRight = outerJoinDesc.RightNode;

                Add(queryGraph, identNodeLeft, identNodeRight);

                if (outerJoinDesc.AdditionalLeftNodes != null)
                {
                    for (int i = 0; i < outerJoinDesc.AdditionalLeftNodes.Length; i++)
                    {
                        Add(queryGraph,
                            outerJoinDesc.AdditionalLeftNodes[i],
                            outerJoinDesc.AdditionalRightNodes[i]);
                    }
                }
			}

			return queryGraph;
		}

        private static void Add(QueryGraph queryGraph, ExprIdentNode identNodeLeft, ExprIdentNode identNodeRight)
        {
            queryGraph.Add(
                identNodeLeft.StreamId,
                identNodeLeft.ResolvedPropertyName,
                identNodeRight.StreamId,
                identNodeRight.ResolvedPropertyName);
        }
	}
}
