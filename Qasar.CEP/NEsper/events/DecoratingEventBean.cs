///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Interface for event types that provide decorating event properties as a name-value map.
    /// </summary>
	public interface DecoratingEventBean
	{
	    /// <summary>Returns decorating properties.</summary>
	    /// <returns>property name and values</returns>
	    Map<String, Object> DecoratingProperties { get; }
	}
} // End of namespace
