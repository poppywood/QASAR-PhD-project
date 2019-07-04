///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.collection;
using com.espertech.esper.core;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.view
{
    /// <summary>An output strategy that handles routing (insert-into) and stream selection. </summary>
    public class OutputStrategyPostProcess : OutputStrategy
    {
        private readonly bool isRoute;
        private readonly bool isRouteRStream;
        private readonly SelectClauseStreamSelectorEnum selectStreamDirEnum;
        private readonly InternalEventRouter internalEventRouter;
        private readonly EPStatementHandle epStatementHandle;
    
        /// <summary>Ctor. </summary>
        /// <param name="route">true if this is insert-into</param>
        /// <param name="routeRStream">true if routing the remove stream events, false if routing insert stream events</param>
        /// <param name="selectStreamDirEnum">enumerator selecting what Stream(s) are selected</param>
        /// <param name="internalEventRouter">for performing the route operation</param>
        /// <param name="epStatementHandle">for use in routing to determine which statement routed</param>
        public OutputStrategyPostProcess(bool route, bool routeRStream, SelectClauseStreamSelectorEnum selectStreamDirEnum, InternalEventRouter internalEventRouter, EPStatementHandle epStatementHandle)
        {
            isRoute = route;
            isRouteRStream = routeRStream;
            this.selectStreamDirEnum = selectStreamDirEnum;
            this.internalEventRouter = internalEventRouter;
            this.epStatementHandle = epStatementHandle;
        }
    
        public void Output(bool forceUpdate, UniformPair<EventBean[]> result, UpdateDispatchView finalView)
        {
            EventBean[] newEvents;
            EventBean[] oldEvents;

            if ( result != null )
            {
                newEvents = result.First;
                oldEvents = result.Second;
            }
            else
            {
                newEvents = oldEvents = null;
            }
    
            // route first
            if (isRoute)
            {
                if ((newEvents != null) && (!isRouteRStream))
                {
                    Route(newEvents);
                }
    
                if ((oldEvents != null) && (isRouteRStream))
                {
                    Route(oldEvents);
                }
            }
    
            // discard one side of results
            if (selectStreamDirEnum == SelectClauseStreamSelectorEnum.RSTREAM_ONLY)
            {
                newEvents = oldEvents;
                oldEvents = null;
            }
            else if (selectStreamDirEnum == SelectClauseStreamSelectorEnum.ISTREAM_ONLY)
            {
                oldEvents = null;   // since the insert-into may require rstream
            }
    
            // dispatch
            if(newEvents != null || oldEvents != null)
            {
                finalView.NewResult(new UniformPair<EventBean[]>(newEvents, oldEvents));
            }
            else if(forceUpdate)
            {
                finalView.NewResult(new UniformPair<EventBean[]>(null, null));
            }
        }
    
        private void Route(EventBean[] events)
        {
            foreach (EventBean routed in events) {
                if (routed is NaturalEventBean) {
                    NaturalEventBean natural = (NaturalEventBean) routed;
                    internalEventRouter.Route(natural.OptionalSynthetic, epStatementHandle);
                } else {
                    internalEventRouter.Route(routed, epStatementHandle);
                }
            }
        }
    }
}
