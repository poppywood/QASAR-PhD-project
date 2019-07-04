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
    /// <summary>
    /// An output strategy that outputs if there are results of if the force-update flag is set.
    ///  </summary>
    public class OutputStrategySimple : OutputStrategy
    {
        public void Output(bool forceUpdate, UniformPair<EventBean[]> result, UpdateDispatchView finalView)
        {
            EventBean[] newEvents = result != null ? result.First : null;
            EventBean[] oldEvents = result != null ? result.Second : null;
            if(newEvents != null || oldEvents != null)
            {
                finalView.NewResult(result);
            }
            else if(forceUpdate)
            {
                finalView.NewResult(result);
            }
        }
    }
}
