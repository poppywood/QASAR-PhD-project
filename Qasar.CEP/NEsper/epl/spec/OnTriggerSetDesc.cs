///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace com.espertech.esper.epl.spec
{
    /// <summary>Specification for the on-set statement.</summary>
	public class OnTriggerSetDesc : OnTriggerDesc
	{
	    private readonly IList<OnTriggerSetAssignment> assignments;

	    /// <summary>Ctor.</summary>
	    public OnTriggerSetDesc()
	        : base(OnTriggerType.ON_SET)
	    {
	        assignments = new List<OnTriggerSetAssignment>();
	    }

	    /// <summary>Adds a variable assignment.</summary>
	    /// <param name="assignment">to add</param>
	    public void AddAssignment(OnTriggerSetAssignment assignment)
	    {
	        assignments.Add(assignment);
	    }

	    /// <summary>Returns a list of all variables assignment by the on-set</summary>
	    /// <returns>list of assignments</returns>
	    public IList<OnTriggerSetAssignment> Assignments
	    {
	        get { return assignments; }
	    }
	}
} // End of namespace
