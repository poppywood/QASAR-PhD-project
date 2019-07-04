///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.core;
using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// Service to manage named window dispatches, locks and processors on an engine level.
    /// </summary>
	public interface NamedWindowService
	{
	    /// <summary>Returns true to indicate that the name is a named window.</summary>
	    /// <param name="name">is the window name</param>
	    /// <returns>true if a named window, false if not a named window</returns>
	    bool IsNamedWindow(String name);

        /// <summary>Returns the names of all named windows known.</summary>
        /// <returns>named window names</returns>
        ICollection<String> NamedWindows { get; }

        /// <summary>Create a new named window.</summary>
        /// <param name="name">window name</param>
        /// <param name="eventType">the event type of the window</param>
        /// <param name="createWindowStmtHandle">is the handle and lock of the create-named-window statement</param>
        /// <param name="statementResultService">for coordinating on whether insert and remove stream events should be posted</param>
        /// <param name="revisionProcessor">handles update events</param>
        /// <returns>processor for the named window</returns>
        /// <throws>ViewProcessingException if the named window already exists</throws>

        NamedWindowProcessor AddProcessor(String name,
                                          EventType eventType,
                                          EPStatementHandle createWindowStmtHandle,
                                          StatementResultService statementResultService,
                                          ValueAddEventProcessor revisionProcessor);

	    /// <summary>Returns the processing instance for a given named window.</summary>
	    /// <param name="name">window name</param>
	    /// <returns>processor for the named window</returns>
	    NamedWindowProcessor GetProcessor(String name);

	    /// <summary>
	    /// Upon destroy of the named window creation statement, the named window processor must be removed.
	    /// </summary>
	    /// <param name="name">is the named window name</param>
	    void RemoveProcessor(String name);

	    /// <summary>
	    /// Dispatch events of the insert and remove stream of named windows to consumers, as part of the
	    /// main event processing or dispatch loop.
	    /// </summary>
	    /// <returns>send events to consuming statements</returns>
	    bool Dispatch();

	    /// <summary>
	    /// For use to add a result of a named window that must be dispatched to consuming views.
	    /// </summary>
	    /// <param name="delta">is the result to dispatch</param>
	    /// <param name="consumers">
	    /// is the destination of the dispatch, a map of statements to one or more consuming views
	    /// </param>
	    void AddDispatch(NamedWindowDeltaData delta, Map<EPStatementHandle, IList<NamedWindowConsumerView>> consumers);

	    /// <summary>
	    /// Returns the statement lock for the named window, to be shared with on-delete statements for the same named window.
	    /// </summary>
	    /// <param name="windowName">is the window name</param>
	    /// <returns>
	    /// the lock for the named window, or null if the window dos not yet exists
	    /// </returns>
	    ManagedLock GetNamedWindowLock(String windowName);

	    /// <summary>Sets the lock to use for a named window.</summary>
	    /// <param name="windowName">is the named window name</param>
	    /// <param name="statementResourceLock">
	    /// is the statement lock for the create window statement
	    /// </param>
	    void AddNamedWindowLock(String windowName, ManagedLock statementResourceLock);

	    /// <summary>Clear out the service.</summary>
	    void Destroy();
	}

    public class NamedWindowServiceConstants
    {
        /// <summary>Error message for data windows required.</summary>
        public const String ERROR_MSG_DATAWINDOWS = "Named windows require one or more child views that are data window views";

        /// <summary>Error message for no data window allowed.</summary>
        public const String ERROR_MSG_NO_DATAWINDOW_ALLOWED = "Consuming statements to a named window cannot declare a data window view onto the named window";
    }
} // End of namespace
