///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;
using com.espertech.esper.compat;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Provides update listeners for use by statement instances, and the management methods around these.
    /// <para>
    /// The collection of update listeners is based on copy-on-write:
    /// When the engine dispatches events to a set of listeners, then while iterating through the set there
    /// may be listeners added or removed (the listener may remove itself).
    /// </para>
    /// <para>
    /// Additionally, events may be dispatched by multiple threads to the same listener.
    /// </para>
    /// </summary>
    public class EPStatementListenerSet
    {
        private Object subscriber;
        private CopyOnWriteArraySet<UpdateListener> listeners;
        private CopyOnWriteArraySet<StatementAwareUpdateListener> stmtAwareListeners;

        /// <summary>Ctor.</summary>
        public EPStatementListenerSet()
        {
            listeners = new CopyOnWriteArraySet<UpdateListener>();
            stmtAwareListeners = new CopyOnWriteArraySet<StatementAwareUpdateListener>();
        }

        /// <summary>Ctor.</summary>
        /// <param name="listeners">is a set of update listener</param>
        /// <param name="stmtAwareListeners">is a set of statement-aware update listener</param>
        public EPStatementListenerSet(
            CopyOnWriteArraySet<UpdateListener> listeners,
            CopyOnWriteArraySet<StatementAwareUpdateListener> stmtAwareListeners)
        {
            this.listeners = listeners;
            this.stmtAwareListeners = stmtAwareListeners;
        }

        /// <summary>Copy the update listener set to from another.</summary>
        /// <param name="listenerSet">a collection of update listeners</param>
        public void Copy(EPStatementListenerSet listenerSet)
        {
            this.listeners = listenerSet.Listeners;
            this.stmtAwareListeners = listenerSet.StmtAwareListeners;
        }

        /// <summary>Returns the set of listeners to the statement.</summary>
        /// <returns>statement listeners</returns>
        public CopyOnWriteArraySet<UpdateListener> Listeners
        {
            get { return listeners; }
        }

        /// <summary>Returns the set of statement-aware listeners.</summary>
        /// <returns>statement-aware listeners</returns>
        public CopyOnWriteArraySet<StatementAwareUpdateListener> StmtAwareListeners
        {
            get { return stmtAwareListeners; }
        }

        /// <summary>Add a listener to the statement.</summary>
        /// <param name="listener">to add</param>
        public void AddListener(UpdateListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentException("Null listener reference supplied");
            }

            listeners.Add(listener);
        }

        /// <summary>Remove a listeners to a statement.</summary>
        /// <param name="listener">to remove</param>
        public void RemoveListener(UpdateListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentException("Null listener reference supplied");
            }

            listeners.Remove(listener);
        }

        /// <summary>Remove all listeners to a statement.</summary>
        public void RemoveAllListeners()
        {
            listeners.Clear();
            stmtAwareListeners.Clear();
        }

        /// <summary>Add a listener to the statement.</summary>
        /// <param name="listener">to add</param>
        public void AddListener(StatementAwareUpdateListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentException("Null listener reference supplied");
            }

            stmtAwareListeners.Add(listener);
        }

        /// <summary>Remove a listeners to a statement.</summary>
        /// <param name="listener">to remove</param>
        public void RemoveListener(StatementAwareUpdateListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentException("Null listener reference supplied");
            }

            stmtAwareListeners.Remove(listener);
        }

        /// <summary>
        /// Gets or sets the subscriber instance.
        /// </summary>

        public Object Subscriber
        {
            get { return subscriber; }
            set { subscriber = value; }
        }
    }
}
