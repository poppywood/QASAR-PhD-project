///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;

using com.espertech.esper.client;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;

namespace com.espertech.esper.epl.expression
{
	/// <summary>
	/// Represents a substitution value to be substituted in an expression tree, not valid for any purpose of use
	/// as an expression, however can take a place in an expression tree.
	/// </summary>
	public class ExprSubstitutionNode : ExprNode
	{
	    private const String ERROR_MSG = "Invalid use of substitution parameters marked by '?' in statement, use the prepare method to prepare statements with substitution parameters";
	    private readonly int index;

	    /// <summary>Ctor.</summary>
	    /// <param name="index">is the index of the substitution parameter</param>
	    public ExprSubstitutionNode(int index)
	    {
	        this.index = index;
	    }

        public override void Validate(StreamTypeService streamTypeService,
                                      MethodResolutionService methodResolutionService,
                                      ViewResourceDelegate viewResourceDelegate,
                                      TimeProvider timeProvider,
                                      VariableService variableService)
        {
	        throw new ExprValidationException(ERROR_MSG);
	    }

	    /// <summary>Returns the substitution parameter index.</summary>
	    /// <returns>index</returns>
	    public int Index
	    {
	    	get { return index; }
	    }

	    public override bool IsConstantResult
	    {
	    	get { return false; }
	    }

	    public override Type ReturnType
	    {
	    	get { throw new ExprValidationException(ERROR_MSG); }
	    }

	    public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData)
	    {
	        throw new EPException(ERROR_MSG);
	    }

	    public override String ExpressionString
	    {
	    	get { throw new EPException(ERROR_MSG); }
	    }

	    public override bool EqualsNode(ExprNode node)
	    {
	    	return node is ExprSubstitutionNode;
	    }
	}
} // End of namespace
