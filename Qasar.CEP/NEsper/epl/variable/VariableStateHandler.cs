///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.collection;
using com.espertech.esper.core;

namespace com.espertech.esper.epl.variable
{
    /// <summary>
    /// Interface for a plug-in to <see cref="VariableService"/> to handle variable persistent state.
    /// </summary>
	public interface VariableStateHandler
	{
	    /// <summary>
	    /// Returns the current variable state plus Boolean.TRUE if there is a current state since the variable
	    /// may have the value of null; returns Boolean.FALSE and null if there is no current state
	    /// </summary>
	    /// <param name="variableName">variable name</param>
	    /// <param name="variableNumber">number of the variable</param>
	    /// <param name="type">type of the variable</param>
	    /// <param name="statementExtContext">for caches etc.</param>
	    /// <returns>
	    /// indicator whether the variable is known and it's state, or whether it doesn't have state (false)
	    /// </returns>
	    Pair<Boolean, Object> GetHasState(String variableName, int variableNumber, Type type, StatementExtensionSvcContext statementExtContext);

	    /// <summary>Sets the new variable value</summary>
	    /// <param name="variableName">name of the variable</param>
	    /// <param name="variableNumber">number of the variable</param>
	    /// <param name="newValue">new variable value, null values allowed</param>
	    void SetState(String variableName, int variableNumber, Object newValue);
	}
} // End of namespace
