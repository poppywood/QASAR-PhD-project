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
	/// <summary>
	/// Represents the CURRENT_TIMESTAMP() function or reserved keyword in an expression tree.
	/// </summary>
	public class ExprTimestampNode : ExprNode
	{
	    private TimeProvider timeProvider;

	    /// <summary>Ctor.</summary>
	    public ExprTimestampNode()
	    {
	    }

        public override void Validate(StreamTypeService streamTypeService,
                                      MethodResolutionService methodResolutionService,
                                      ViewResourceDelegate viewResourceDelegate,
                                      TimeProvider timeProvider,
                                      VariableService variableService)
	    {
	        if (this.ChildNodes.Count != 0)
	        {
	            throw new ExprValidationException("current_timestamp function node must have exactly 1 child node");
	        }
	        this.timeProvider = timeProvider;
	    }

	    public override bool IsConstantResult
	    {
	    	get { return false; }
	    }

	    public override Type ReturnType
	    {
	    	get { return typeof(long?); }
	    }

	    public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData)
	    {
	        return timeProvider.Time;
	    }

	    public override String ExpressionString
	    {
	    	get
	    	{
		        StringBuilder buffer = new StringBuilder();
		        buffer.Append("current_timestamp()");
		        return buffer.ToString();
	    	}
	    }

	    public override bool EqualsNode(ExprNode node)
	    {
	    	return node is ExprTimestampNode;
	    }
	}
} // End of namespace
