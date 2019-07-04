///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.epl.expression;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
    /// <summary>Descriptor for an on-set assignment.</summary>
	public class OnTriggerSetAssignment : MetaDefItem
	{
	    private readonly String variableName;
	    private ExprNode expression;

	    /// <summary>Ctor.</summary>
	    /// <param name="variableName">variable name</param>
	    /// <param name="expression">expression providing new variable value</param>
	    public OnTriggerSetAssignment(String variableName, ExprNode expression)
	    {
	        this.variableName = variableName;
	        this.expression = expression;
	    }

	    /// <summary>Returns the variable name</summary>
	    /// <returns>variable name</returns>
	    public String VariableName
	    {
            get { return variableName; }
	    }

	    /// <summary>
	    /// Gets or sets the expression providing the new variable value, or null if none
	    /// </summary>
	    /// <returns>assignment expression</returns>
        public ExprNode Expression
        {
            get { return expression; }
            set { expression = value; }
        }
	}
} // End of namespace
