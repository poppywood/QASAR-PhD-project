///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.epl.variable
{
    /// <summary>
    /// A callback interface for indicating a change in variable value.
    /// </summary>
	public interface VariableChangeCallback
	{
	    /// <summary>Indicate a change in variable value.</summary>
	    /// <param name="newValue">new value</param>
	    /// <param name="oldValue">old value</param>
	    void Update(Object newValue, Object oldValue);
	}
} // End of namespace
