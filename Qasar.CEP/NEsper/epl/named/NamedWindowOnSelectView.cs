///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.epl.core;
using com.espertech.esper.events;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.epl.named
{
    /// <summary>View for the on-select statement that handles selecting events from a named window. </summary>
    public class NamedWindowOnSelectView : NamedWindowOnExprBaseView
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
        private readonly InternalEventRouter internalEventRouter;
        private readonly ResultSetProcessor resultSetProcessor;
        private readonly EPStatementHandle statementHandle;
        private readonly StatementResultService statementResultService;
        private EventBean[] lastResult;
        private readonly Set<MultiKey<EventBean>> oldEvents = new HashSet<MultiKey<EventBean>>();
    
        /// <summary>Ctor. </summary>
        /// <param name="statementStopService">for indicating a statement was stopped or destroyed for cleanup</param>
        /// <param name="lookupStrategy">for handling trigger events to determine deleted events</param>
        /// <param name="rootView">the named window root view</param>
        /// <param name="internalEventRouter">for insert-into behavior</param>
        /// <param name="resultSetProcessor">for processing aggregation, having and ordering</param>
        /// <param name="statementHandle">required for routing events</param>
        /// <param name="statementResultService">for coordinating on whether insert and remove stream events should be posted</param>
        public NamedWindowOnSelectView(StatementStopService statementStopService,
                                       LookupStrategy lookupStrategy,
                                       NamedWindowRootView rootView,
                                       InternalEventRouter internalEventRouter,
                                       ResultSetProcessor resultSetProcessor,
                                       EPStatementHandle statementHandle,
                                       StatementResultService statementResultService)
            : base(statementStopService, lookupStrategy, rootView)
        {
            this.internalEventRouter = internalEventRouter;
            this.resultSetProcessor = resultSetProcessor;
            this.statementHandle = statementHandle;
            this.statementResultService = statementResultService;
        }
    
        public override void HandleMatching(EventBean[] triggerEvents, EventBean[] matchingEvents)
        {
            EventBean[] newData;
    
            // clear state from prior results
            resultSetProcessor.Clear();
    
            // build join result
            Set<MultiKey<EventBean>> newEvents = new HashSet<MultiKey<EventBean>>();
            for (int i = 0; i < triggerEvents.Length; i++)
            {
                EventBean triggerEvent = triggerEvents[0];
                if (matchingEvents != null)
                {
                    for (int j = 0; j < matchingEvents.Length; j++)
                    {
                        EventBean[] eventsPerStream = new EventBean[2];
                        eventsPerStream[0] = matchingEvents[j];
                        eventsPerStream[1] = triggerEvent;
                        newEvents.Add(new MultiKey<EventBean>(eventsPerStream));
                    }
                }
            }
    
            // process matches
            UniformPair<EventBean[]> pair = resultSetProcessor.ProcessJoinResult(newEvents, oldEvents, false);
            newData = pair.First;
    
            if (internalEventRouter != null)
            {
                for (int i = 0; i < newData.Length; i++)
                {
                    internalEventRouter.Route(newData[i], statementHandle);
                }
            }
    
            // The on-select listeners receive the events selected
            if ((newData != null) && (newData.Length > 0))
            {
                // And post only if we have listeners/subscribers that need the data 
                if (statementResultService.IsMakeNatural || statementResultService.IsMakeSynthetic)
                {
                    UpdateChildren(newData, null);
                }
            }
            lastResult = newData;
        }
    
        public override EventType EventType
        {
            get
            {
                if (resultSetProcessor != null) {
                    return resultSetProcessor.ResultEventType;
                } else {
                    return namedWindowEventType;
                }
            }
        }
    
        public override IEnumerator<EventBean> GetEnumerator()
        {
            if (lastResult == null) return null;
            return ((IEnumerable<EventBean>) lastResult).GetEnumerator();
        }
    }
}
