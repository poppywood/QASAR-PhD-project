///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.events;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Results of a fire-and-forget, non-continuous query.
    /// </summary>
    public interface EPQueryResult : IEnumerable<EventBean>
    {
        /// <summary>Returns an array representing query result rows. </summary>
        /// <returns>result array</returns>
        EventBean[] GetArray();

        /// <summary>Returns the event type of the result. </summary>
        /// <returns>type</returns>
        EventType EventType { get; }
    }
}
