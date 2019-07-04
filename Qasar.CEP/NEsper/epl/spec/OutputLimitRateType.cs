///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.epl.spec
{
    /// <summary>
    /// Enum for the type of rate for output-rate limiting.
    /// </summary>
	public enum OutputLimitRateType
	{
	    /// <summary>Output by events.</summary>
	    EVENTS,

	    /// <summary>Output by seconds.</summary>
	    TIME_SEC,

	    /// <summary>Output by minutes.</summary>
	    TIME_MIN
	}
} // End of namespace
