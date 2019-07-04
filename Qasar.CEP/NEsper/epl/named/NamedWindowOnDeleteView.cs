///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.events;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// View for the on-delete statement that handles removing events from a named window.
    /// </summary>
	public class NamedWindowOnDeleteView : NamedWindowOnExprBaseView
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    private EventBean[] lastResult;
        private readonly StatementResultService statementResultService;

	    /// <summary>Ctor.</summary>
	    /// <param name="statementStopService">for indicating a statement was stopped or destroyed for cleanup</param>
	    /// <param name="lookupStrategy">/// for handling trigger events to determine deleted events</param>
	    /// <param name="removeStreamView">to indicate which events to delete</param>
        /// <param name="statementResultService">for coordinating on whether insert and remove stream events should be posted</param>
	    public NamedWindowOnDeleteView(StatementStopService statementStopService,
	                                 LookupStrategy lookupStrategy,
                                     NamedWindowRootView removeStreamView,
                                     StatementResultService statementResultService)
            : base(statementStopService, lookupStrategy, removeStreamView)
	    {
            this.statementResultService = statementResultService;
	    }

	    public override void HandleMatching(EventBean[] triggerEvents, EventBean[] matchingEvents)
	    {
	        if ((matchingEvents != null) && (matchingEvents.Length > 0))
	        {
	            // Events to delete are indicated via old data
	            rootView.Update(null, matchingEvents);

                // The on-delete listeners receive the events deleted, but only if there is interest
                if (statementResultService.IsMakeNatural || statementResultService.IsMakeSynthetic)
                {
                    UpdateChildren(matchingEvents, null);
                }
	        }

	        // Keep the last delete records
	        lastResult = matchingEvents;
	    }

	    public override EventType EventType
	    {
            get { return namedWindowEventType; }
	    }

	    public override IEnumerator<EventBean> GetEnumerator()
	    {
	        IEnumerable<EventBean> eventEnum = ((IEnumerable<EventBean>) lastResult);
	        return
	            eventEnum != null
	                ? eventEnum.GetEnumerator()
	                : EnumerationHelper<EventBean>.CreateEmptyEnumerator();
	    }
	}
} // End of namespace
