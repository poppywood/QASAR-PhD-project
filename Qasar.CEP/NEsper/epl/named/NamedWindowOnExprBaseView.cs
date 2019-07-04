///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.events;
using com.espertech.esper.util;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// View for the on-delete statement that handles removing events from a named window.
    /// </summary>
	public abstract class NamedWindowOnExprBaseView : ViewSupport, StatementStopCallback
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    /// <summary>The event type of the events hosted in the named window.</summary>
	    protected readonly EventType namedWindowEventType;
	    private readonly LookupStrategy lookupStrategy;

	    /// <summary>The root view accepting removals (old data).</summary>
        protected readonly NamedWindowRootView rootView;

	    /// <summary>Ctor.</summary>
	    /// <param name="statementStopService">
	    /// for indicating a statement was stopped or destroyed for cleanup
	    /// </param>
	    /// <param name="lookupStrategy">
	    /// for handling trigger events to determine deleted events
	    /// </param>
	    /// <param name="rootView">to indicate which events to delete</param>
	    public NamedWindowOnExprBaseView(StatementStopService statementStopService,
	                                 LookupStrategy lookupStrategy,
	                                 NamedWindowRootView rootView)
	    {
	        this.lookupStrategy = lookupStrategy;
	        this.rootView = rootView;
	        statementStopService.AddSubscriber(this);
	        namedWindowEventType = rootView.EventType;
	    }

	    /// <summary>
	    /// Implemented by on-trigger views to action on the combination of trigger and matching events in the named window.
	    /// </summary>
	    /// <param name="triggerEvents">is the trigger events (usually 1)</param>
	    /// <param name="matchingEvents">
	    /// is the matching events retrieved via lookup strategy
	    /// </param>
	    public abstract void HandleMatching(EventBean[] triggerEvents, EventBean[] matchingEvents);

	    public void StatementStopped()
	    {
	        log.Debug(".statementStopped");
	        rootView.RemoveOnExpr(lookupStrategy);
	    }

	    public override void Update(EventBean[] newData, EventBean[] oldData)
	    {
	        if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
	        {
	            log.Debug(".update Received update, " +
	                    "  newData.length==" + ((newData == null) ? 0 : newData.Length) +
	                    "  oldData.length==" + ((oldData == null) ? 0 : oldData.Length));
	        }

	        if (newData == null)
	        {
	            return;
	        }

	        // Determine via the lookup strategy a subset of events to process
	        EventBean[] eventsFound = lookupStrategy.Lookup(newData);

	        // Let the implementation handle the delete or
	        HandleMatching(newData, eventsFound);
	    }
	}
} // End of namespace
