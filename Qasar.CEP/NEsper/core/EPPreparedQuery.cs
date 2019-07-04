///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.events;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Interface for a prepared query that can be executed multiple times.
    /// </summary>
    public interface EPPreparedQuery
    {
        /// <summary>
        /// Execute the prepared query returning query results.
        /// </summary>
        /// <returns>query result</returns>
        EPQueryResult Execute();

        /// <summary>
        /// Returns the event type, representing the columns of the select-clause
        /// </summary>
        /// <value>The type of the event.</value>
        /// <returns>event type</returns>
        EventType EventType { get; }
    }
}
