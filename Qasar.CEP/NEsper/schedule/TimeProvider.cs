///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.schedule
{
    /// <summary>
    /// Provider of internal system time.
    /// <para>
    /// Internal system time is controlled either by a timer function or by external time events.
    /// </para>
    /// </summary>
	public interface TimeProvider
	{
	    /// <summary>Returns the current engine time.</summary>
	    /// <returns>time that has last been set</returns>
        long Time { get; set; }
	}
} // End of namespace
