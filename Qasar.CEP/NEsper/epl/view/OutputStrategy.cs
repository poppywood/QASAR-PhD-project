///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.collection;
using com.espertech.esper.core;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.view
{
    /// <summary>Strategy for performing an output via dispatch view. </summary>
    public interface OutputStrategy
    {
        /// <summary>Outputs the result to the output view and following update policy. </summary>
        /// <param name="forceUpdate">indicates whether output can be skipped, such as when no results collected</param>
        /// <param name="result">the output to indicate</param>
        /// <param name="outputView">the view to output to</param>
        void Output(bool forceUpdate, UniformPair<EventBean[]> result, UpdateDispatchView outputView);
    }
}
