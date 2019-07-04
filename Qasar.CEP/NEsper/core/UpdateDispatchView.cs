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
using com.espertech.esper.view;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Update dispatch view to indicate statement results to listeners.
    /// </summary>
    public interface UpdateDispatchView : View
    {
        /// <summary>
        /// Convenience method that accepts a pair of new and old data as this is the most treated unit.
        /// </summary>
        /// <param name="result">is new data (insert stream) and old data (remove stream)</param>
        void NewResult(UniformPair<EventBean[]> result);
    }
}
