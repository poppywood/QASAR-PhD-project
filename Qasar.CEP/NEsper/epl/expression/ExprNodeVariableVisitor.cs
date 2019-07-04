///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.epl.expression
{
    /// <summary>
    /// Visitor for expression node trees that determines if the expressions within contain a variable.
    /// </summary>
	public class ExprNodeVariableVisitor : ExprNodeVisitor
	{
	    private bool hasVariables;

	    public bool IsVisit(ExprNode exprNode)
	    {
	        return true;
	    }

	    /// <summary>Returns true if the visitor finds a variable value.</summary>
	    /// <returns>true for variable present in expression</returns>
	    public bool HasVariables
	    {
	        get { return hasVariables; }
	    }

	    public void Visit(ExprNode exprNode)
	    {
	        if (!(exprNode is ExprVariableNode))
	        {
	            return;
	        }
	        hasVariables = true;
	    }
	}
} // End of namespace
