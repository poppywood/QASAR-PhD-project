///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using net.esper.client;
using net.esper.dispatch;
using net.esper.events;
using net.esper.view;

using org.apache.commons.logging;

namespace net.esper.core
{
/// <summary>
/// Convenience view for dispatching view updates received from a parent view to update listeners
/// via the dispatch service.
/// </summary>
    public class UpdateDispatchViewBlocking : UpdateDispatchViewBase
    {
        private readonly bool isDebugEnabled;
        private DispatchFuture currentFuture;
        private long msecTimeout;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="epServiceProvider">engine instance to supply to statement-aware listeners</param>
        /// <param name="statement">the statement instance to supply to statement-aware listeners</param>
        /// <param name="updateListeners">listeners to update</param>
        /// <param name="dispatchService">for performing the dispatch</param>
        /// <param name="msecTimeout">timeout for preserving dispatch order through blocking</param>
        public UpdateDispatchViewBlocking(EPServiceProvider epServiceProvider, EPStatement statement, EPStatementListenerSet updateListeners, DispatchService dispatchService, long msecTimeout)
            : base(epServiceProvider, statement, updateListeners, dispatchService)
        {
            this.isDebugEnabled = log.IsDebugEnabled;
            this.currentFuture = new DispatchFuture(); // use a completed future as a start
            this.msecTimeout = msecTimeout;
        }

        public override void Update(EventBean[] newData, EventBean[] oldData)
        {
            if (isDebugEnabled)
            {
                ViewSupport.DumpUpdateParams(".update for view " + this, newData, oldData);
            }

            ThreadLocalData threadLocal = base.LocalData;

            if (newData != null)
            {
                lastIterableEvent = newData[0];
                threadLocal.lastNewEvents.Add(newData);
            }
            if (oldData != null)
            {
                threadLocal.lastOldEvents.Add(oldData);
            }
            if (!threadLocal.isDispatchWaiting)
            {
                DispatchFuture nextFuture;
                lock (this)
                {
                    nextFuture = new DispatchFuture(this, currentFuture, (int)msecTimeout);
                    currentFuture.SetLater(nextFuture);
                    currentFuture = nextFuture;
                }
                dispatchService.AddExternal(nextFuture);
                threadLocal.isDispatchWaiting = true;
            }
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
} // End of namespace
