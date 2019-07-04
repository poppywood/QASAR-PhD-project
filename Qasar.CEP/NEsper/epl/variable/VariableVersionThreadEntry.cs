///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.compat;

namespace com.espertech.esper.epl.variable
{
    /// <summary>
    /// Thread-specific state in regards to variable versions.
    /// </summary>
	public class VariableVersionThreadEntry
	{
	    private int version;
	    private Map<int, Object> uncommitted;

	    /// <summary>Ctor.</summary>
	    /// <param name="version">
	    /// current version number of the variables visible to thread
	    /// </param>
	    /// <param name="uncommitted">
	    /// the uncommitted values of variables for the thread, if any
	    /// </param>
        public VariableVersionThreadEntry(int version, Map<int, Object> uncommitted)
	    {
	        this.version = version;
	        this.uncommitted = uncommitted;
	    }

	    /// <summary>Gets or sets the version visible for a thread.</summary>
	    /// <returns>version number</returns>
	    public int Version
	    {
            get { return version; }
            set { version = value; }
	    }

	    /// <summary>
	    /// Gets or sets a map of variable number and uncommitted value, or empty
	    /// map or null if none exist
	    /// </summary>
	    /// <returns>uncommitted values</returns>
	    public Map<int, Object> Uncommitted
	    {
	        get { return uncommitted; }
            set { uncommitted = value; }
	    }
	}
} // End of namespace
