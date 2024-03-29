///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using com.espertech.esper.util;

namespace com.espertech.esper.core
{
	/// <summary>
	/// Factory for the managed lock that provides statement resource protection.
	/// </summary>
	public interface StatementLockFactory
	{
	    /// <summary>Create lock for statement</summary>
	    /// <param name="statementName">is the statement name</param>
	    /// <param name="expressionText">is the statement expression text</param>
	    /// <returns>lock</returns>
	    ManagedLock GetStatementLock(String statementName, String expressionText);
	}
} // End of namespace
