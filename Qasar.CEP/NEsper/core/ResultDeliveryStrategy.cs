///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;

using com.espertech.esper.collection;
using com.espertech.esper.events;

namespace com.espertech.esper.core
{
    /// <summary>Strategy for use with <see cref="StatementResultService"/> to dispatch to a statement's subscriber via method invocations. </summary>
    public interface ResultDeliveryStrategy
    {
        /// <summary>Execute the dispatch. </summary>
        /// <param name="result">is the insert and remove stream to indicate</param>
        void Execute(UniformPair<EventBean[]> result);
    }
}
