///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace com.espertech.esper.epl.expression
{
	/// <summary>Visitor that collects <see cref="ExprSubselectNode"/> instances.</summary>
	public class ExprNodeSubselectVisitor : ExprNodeVisitor
	{
	    private readonly List<ExprSubselectNode> subselects;

	    /// <summary>Ctor.</summary>
	    public ExprNodeSubselectVisitor()
	    {
	        subselects = new List<ExprSubselectNode>();
	    }

        /// <summary>Returns a list of lookup expression nodes.</summary>
        /// <returns>lookup nodes</returns>
	    public IList<ExprSubselectNode> Subselects
        {
	        get { return subselects; }
	    }

	    public bool IsVisit(ExprNode exprNode)
	    {
	        return true;
	    }

	    public void Visit(ExprNode exprNode)
	    {
	        if (!(exprNode is ExprSubselectNode))
	        {
	            return;
	        }

	        ExprSubselectNode subselectNode = (ExprSubselectNode) exprNode;
	        subselects.Add(subselectNode);
	    }
	}
} // End of namespace
