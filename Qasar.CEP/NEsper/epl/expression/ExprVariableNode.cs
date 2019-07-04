///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;

using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;

namespace com.espertech.esper.epl.expression
{
    /// <summary>Represents a variable in an expression tree.</summary>
	public class ExprVariableNode : ExprNode
	{
	    private readonly String variableName;
	    private Type variableType;
	    private VariableReader reader;

	    /// <summary>Ctor.</summary>
	    /// <param name="variableName">is the name of the variable</param>
	    public ExprVariableNode(String variableName)
	    {
	        if (variableName == null)
	        {
	            throw new ArgumentException("Variables name is null");
	        }
	        this.variableName = variableName;
	    }

	    /// <summary>Returns the name of the variable.</summary>
	    /// <returns>variable name</returns>
	    public String VariableName
	    {
            get { return variableName; }
	    }

	    public override void Validate(StreamTypeService streamTypeService, MethodResolutionService methodResolutionService, ViewResourceDelegate viewResourceDelegate, TimeProvider timeProvider, VariableService variableService)
	    {
	        reader = variableService.GetReader(variableName);
	        if (reader == null)
	        {
	            throw new ExprValidationException("A variable by name '" + variableName + " has not been declared");
	        }

	        // the variable name should not overlap with a property name
	        try
	        {
	            streamTypeService.ResolveByPropertyName(variableName);
	            throw new ExprValidationException("The variable by name '" + variableName + " is ambigous to a property of the same name");
	        }
	        catch (DuplicatePropertyException)
	        {
	            throw new ExprValidationException("The variable by name '" + variableName + " is ambigous to a property of the same name");
	        }
	        catch (PropertyNotFoundException)
	        {
	        }

	        variableType = reader.VariableType;
	    }

	    public override Type ReturnType
	    {
            get
	        {
	            if (variableType == null)
	            {
	                throw new IllegalStateException("Variables node has not been validated");
	            }
	            return variableType;
	        }
	    }

	    public override bool IsConstantResult
	    {
            get { return false; }
	    }

	    public override String ToString()
	    {
	        return "variableName=" + variableName;
	    }

	    public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData)
	    {
	        return reader.GetValue();
	    }

	    public override String ExpressionString
	    {
            get
            {
                StringBuilder buffer = new StringBuilder();
                buffer.Append(variableName);
                return buffer.ToString();
            }
	    }

	    public override bool EqualsNode(ExprNode node)
	    {
	        ExprVariableNode other = node as ExprVariableNode;
            if ( other == null )
            {
                return false;
            }

	        return other.variableName == this.variableName;
	    }
	}
} // End of namespace
