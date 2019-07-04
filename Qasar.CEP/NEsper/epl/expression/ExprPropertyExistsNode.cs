///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;

using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;

namespace com.espertech.esper.epl.expression
{
	/// <summary>Represents the EXISTS(property) function in an expression tree.</summary>
	public class ExprPropertyExistsNode : ExprNode
	{
	    private ExprIdentNode identNode;

	    /// <summary>Ctor.</summary>
	    public ExprPropertyExistsNode()
	    {
	    }

        public override void Validate(StreamTypeService streamTypeService,
                                      MethodResolutionService methodResolutionService,
                                      ViewResourceDelegate viewResourceDelegate,
                                      TimeProvider timeProvider,
                                      VariableService variableService)
        {
	        if (this.ChildNodes.Count != 1)
	        {
	            throw new ExprValidationException("Exists function node must have exactly 1 child node");
	        }

	        ExprIdentNode node = this.ChildNodes[0] as ExprIdentNode;
	        if ( node == null )
	        {
	            throw new ExprValidationException("Exists function expects an property value expression as the child node");
	        }

	        identNode = node;
	    }

	    public override bool IsConstantResult
	    {
	    	get { return false; }
	    }

	    public override Type ReturnType
	    {
	    	get { return typeof(bool?); }
	    }

	    public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData)
	    {
	        return identNode.EvaluatePropertyExists(eventsPerStream, isNewData);
	    }

	    public override String ExpressionString
	    {
	    	get
	    	{
		        StringBuilder buffer = new StringBuilder();
		        buffer.Append("exists(");
		        buffer.Append(this.ChildNodes[0].ExpressionString);
		        buffer.Append(')');
		        return buffer.ToString();
	    	}
	    }

	    public override bool EqualsNode(ExprNode node)
	    {
	    	return (node is ExprPropertyExistsNode);
	    }
	}
} // End of namespace
