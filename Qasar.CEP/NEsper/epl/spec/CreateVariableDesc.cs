///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.epl.expression;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
    /// <summary>Descriptor for create-variable statements.</summary>
	public class CreateVariableDesc : MetaDefItem
	{
	    private readonly String variableType;
        private readonly String variableName;
        private readonly ExprNode assignment;

	    /// <summary>Ctor.</summary>
	    /// <param name="variableType">type of the variable</param>
	    /// <param name="variableName">name of the variable</param>
	    /// <param name="assignment">
	    /// expression assigning the initial value, or null if none
	    /// </param>
	    public CreateVariableDesc(String variableType, String variableName, ExprNode assignment)
	    {
	        this.variableType = variableType;
	        this.variableName = variableName;
	        this.assignment = assignment;
	    }

	    /// <summary>Returns the variable type.</summary>
	    /// <returns>type of variable</returns>
	    public String VariableType
	    {
            get { return variableType; }
	    }

	    /// <summary>Returns the variable name</summary>
	    /// <returns>name</returns>
	    public String VariableName
	    {
            get { return variableName; }
	    }

	    /// <summary>Returns the assignment expression, or null if none</summary>
	    /// <returns>expression or null</returns>
	    public ExprNode Assignment
	    {
            get { return assignment; }
	    }
	}
} // End of namespace
