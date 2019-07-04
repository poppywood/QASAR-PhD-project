///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

using com.espertech.esper.client.soda;
using com.espertech.esper.core;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// Substitution parameter that represents a node in an expression tree for which to supply a parameter value
	/// before statement creation time.
	/// </summary>
    [Serializable]
    public class SubstitutionParameterExpression : ExpressionBase
	{
	    private readonly int index;
	    private Object constant;
	    private bool isSatisfied;

	    /// <summary>Ctor.</summary>
	    /// <param name="index">is the index of the substitution parameter</param>
	    public SubstitutionParameterExpression(int index)
	    {
	        this.index = index;
	    }

	    public override void ToEPL(StringWriter writer)
	    {
	        if (!isSatisfied)
	        {
	            writer.Write("?");
	        }
	        else
	        {
	            EPStatementObjectModelHelper.RenderEQL(writer, constant);
	        }
	    }

	    /// <summary>Gets or sets the constant value that the expression represents.</summary>
	    /// <returns>value of constant</returns>
	    public Object Constant
	    {
	    	get { return constant; }
			set
			{
		        this.constant = value;
		        this.isSatisfied = true;
		    }
	    }

	    /// <summary>Returns true if the parameter is satisfied, or false if not.</summary>
	    /// <returns>true if the actual value is supplied, false if not</returns>
	    public bool IsSatisfied
	    {
	    	get { return isSatisfied; }
	    }

	    /// <summary>Returns the index of the parameter.</summary>
	    /// <returns>parameter index.</returns>
	    public int Index
	    {
	    	get { return index; }
	    }
	}
} // End of namespace
