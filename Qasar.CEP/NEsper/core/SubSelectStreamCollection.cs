///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.compat;
using com.espertech.esper.epl.expression;
using com.espertech.esper.view;

namespace com.espertech.esper.core
{
	/// <summary>Holds stream information for subqueries.</summary>
	public class SubSelectStreamCollection
	{
	    private readonly Map<ExprSubselectNode, SubSelectHolder> subqueries;

	    /// <summary>Ctor.</summary>
	    public SubSelectStreamCollection()
	    {
	        subqueries = new HashMap<ExprSubselectNode, SubSelectHolder>();
	    }

	    /// <summary>Add subquery.</summary>
	    /// <param name="subselectNode">is the subselect expression node</param>
        /// <param name="streamNumber">is the lookup stream number</param>
        /// <param name="viewable">is the lookup viewable</param>
	    /// <param name="viewFactoryChain">is the chain of view factories</param>
	    public void Add(ExprSubselectNode subselectNode, int streamNumber, Viewable viewable, ViewFactoryChain viewFactoryChain)
	    {
	        subqueries[subselectNode] = new SubSelectHolder(streamNumber, viewable, viewFactoryChain);
	    }

	    /// <summary>Returns stream number.</summary>
        /// <param name="subqueryNode">is the lookup node's stream number</param>
	    /// <returns>number of stream</returns>
	    public int GetStreamNumber(ExprSubselectNode subqueryNode)
	    {
	        return subqueries[subqueryNode].StreamNumber;
	    }

        /// <summary>Returns the lookup viewable, child-most view.</summary>
	    /// <param name="subqueryNode">is the expression node to get this for</param>
	    /// <returns>child viewable</returns>
	    public Viewable GetRootViewable(ExprSubselectNode subqueryNode)
	    {
	        return subqueries[subqueryNode].Viewable;
	    }

        /// <summary>Returns the lookup's view factory chain.</summary>
	    /// <param name="subqueryNode">is the node to look for</param>
	    /// <returns>view factory chain</returns>
	    public ViewFactoryChain GetViewFactoryChain(ExprSubselectNode subqueryNode)
	    {
	        return subqueries[subqueryNode].ViewFactoryChain;
	    }
	}
} // End of namespace
