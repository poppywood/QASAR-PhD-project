// ************************************************************************************
// Copyright (C) 2006 Thomas Bernhardt. All rights reserved.                          *
// http://esper.codehaus.org                                                          *
// ---------------------------------------------------------------------------------- *
// The software in this package is published under the terms of the GPL license       *
// a copy of which has been included with this distribution in the license.txt file.  *
// ************************************************************************************

using System;

using net.esper.events;

namespace net.esper.client
{
    /// <summary>
    /// Defines an interface to notify of new and old events.
    /// <para>
    /// Also see <seealso cref="StatementAwareUpdateListener"/> for update listeners that require
    /// the statement and service provider instance to be passed to the listener in addition
    /// to events.
    /// </para>
    /// </summary>

    public interface UpdateListener
    {
        /// <summary>
        /// Notify that new events are available or old events are removed.
        /// If the call to update contains new (inserted) events, then the first argument will be a non-empty list and
        /// the second will be empty. Similarly, if the call is a notification of deleted events, then the first argument
        /// will be empty and the second will be non-empty.
        /// <para>
        /// Either the newEvents or oldEvents will be non-null. This method won't be called with both arguments being null,
        /// but either one could be null. The same is true for zero-length arrays.
        /// </para>
        /// 	<para>
        /// Either newEvents or oldEvents will be non-empty. If both are non-empty, then the update is a modification
        /// notification.
        /// </para>
        /// </summary>
        /// <param name="newEvents">is any new events. This will be null or empty if the update is for old events only.</param>
        /// <param name="oldEvents">is any old events. This will be null or empty if the update is for new events only.</param>
        void Update(EventBean[] newEvents, EventBean[] oldEvents);
    }

    /// <summary>
    /// Defines a delegate that is notified of new and old events.
    /// </summary>
    /// <param name="newEvents"></param>
    /// <param name="oldEvents"></param>

    public delegate void UpdateEventHandler(EventBean[] newEvents, EventBean[] oldEvents);

    public sealed class ProxyUpdateListener : UpdateListener
    {
        private readonly UpdateEventHandler proxyObj;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyUpdateListener"/> class.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public ProxyUpdateListener( UpdateEventHandler handler )
        {
            this.proxyObj = handler;
        }

        /// <summary>
        /// Notify that new events are available or old events are removed.
        /// If the call to update contains new (inserted) events, then the first argument will be a non-empty list and
        /// the second will be empty. Similarly, if the call is a notification of deleted events, then the first argument
        /// will be empty and the second will be non-empty.
        /// <para>
        /// Either the newEvents or oldEvents will be non-null. This method won't be called with both arguments being null,
        /// but either one could be null. The same is true for zero-length arrays.
        /// </para>
        /// 	<para>
        /// Either newEvents or oldEvents will be non-empty. If both are non-empty, then the update is a modification
        /// notification.
        /// </para>
        /// </summary>
        /// <param name="newEvents">is any new events. This will be null or empty if the update is for old events only.</param>
        /// <param name="oldEvents">is any old events. This will be null or empty if the update is for new events only.</param>
        public void Update(EventBean[] newEvents, EventBean[] oldEvents)
        {
            proxyObj(newEvents, oldEvents);
        }
    }
}