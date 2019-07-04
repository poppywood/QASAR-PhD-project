///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;

namespace com.espertech.esper.core
{
    /// <summary>SPI interface of the runtime exposes fire-and-forget, non-continuous query functionality. </summary>
    public interface EPRuntimeSPI : EPRuntime
    {
        /// <summary>Execute a free-form EPL dynamically, non-continuously, in a fire-and-forget fashion, against named windows. </summary>
        /// <param name="epl">is the EPL to execute</param>
        /// <returns>query result</returns>
        EPQueryResult ExecuteQuery(String epl);
    
        /// <summary>Prepare a non-continuous, fire-and-forget query for repeated execution. </summary>
        /// <param name="epl">to prepare</param>
        /// <returns>proxy to execute upon, that also provides the event type of the returned results</returns>
        EPPreparedQuery PrepareQuery(String epl);
    }
}
