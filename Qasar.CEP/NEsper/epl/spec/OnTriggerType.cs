///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.epl.spec
{
    /// <summary>Enum for the type of on-trigger statement.</summary>
	public enum OnTriggerType
	{
	    /// <summary>
	    /// For on-delete triggers that delete from a named window when a triggering event arrives.
	    /// </summary>
	    ON_DELETE,

	    /// <summary>
	    /// For on-select triggers that selected from a named window when a triggering event arrives.
	    /// </summary>
	    ON_SELECT,

	    /// <summary>
	    /// For on-set triggers that set variable values when a triggering event arrives.
	    /// </summary>
	    ON_SET
	}
} // End of namespace
