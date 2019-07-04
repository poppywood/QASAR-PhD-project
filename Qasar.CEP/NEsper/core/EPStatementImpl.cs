///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Threading;
using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.dispatch;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.timer;
using com.espertech.esper.view;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Statement implementation for EPL statements.
    /// </summary>
    public class EPStatementImpl : EPStatementSPI
    {
        private readonly EPStatementListenerSet statementListenerSet;

        private readonly String statementId;
        private readonly String statementName;
        private readonly String expressionText;
        private readonly bool isPattern;
        private UpdateDispatchViewBase dispatchChildView;
        private StatementLifecycleSvc statementLifecycleSvc;
        private readonly VariableService variableService;

        private long timeLastStateChange;
        private Viewable parentView;
        private EPStatementState currentState;
        private EventType eventType;
        private readonly EPStatementHandle epStatementHandle;
        private readonly StatementResultService statementResultService;

        private readonly Object internalRouterLock = new Object();
        private InternalRouter internalRouter;

        /// <summary>
        /// Gets the internal router.
        /// </summary>
        /// <returns></returns>
        private InternalRouter CheckInternalRouter()
        {
            lock (internalRouterLock) {
                if (internalRouter == null) {
                    internalRouter = new InternalRouter(this);
                }
            }

            return internalRouter;
        }

        /// <summary>
        /// Occurs whenever new events are available or old events are removed.
        /// </summary>
        private UpdateEventHandler _internalEvents;
        public event UpdateEventHandler Events
        {
            add
            {
                CheckInternalRouter();
                _internalEvents += value;
            }

            remove
            {
                _internalEvents -= value;
            }
        }

        /// <summary>
        /// Occurs whenever new events are available or old events are removed.
        /// </summary>
        private event StatementAwareUpdateEventHandler _internalAwareEvents;
        public event StatementAwareUpdateEventHandler AwareEvents
        {
            add
            {
                CheckInternalRouter();
                _internalAwareEvents += value;
            }

            remove
            {
                _internalAwareEvents -= value;
            }
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="statementId">is a unique ID assigned by the engine for the statement</param>
        /// <param name="statementName">is the statement name assigned during creation, or the statement id if none was assigned</param>
        /// <param name="expressionText">is the EPL and/or pattern expression</param>
        /// <param name="isPattern">is true to indicate this is a pure pattern expression</param>
        /// <param name="dispatchService">for dispatching events to listeners to the statement</param>
        /// <param name="statementLifecycleSvc">handles lifecycle transitions for the statement</param>
        /// <param name="timeLastStateChange">the timestamp the statement was created and started</param>
        /// <param name="isBlockingDispatch">is true if the dispatch to listeners should block to preserve event generation order</param>
        /// <param name="isSpinBlockingDispatch">true to use spin locks blocking to deliver results, as locks are usually uncontended</param>
        /// <param name="msecBlockingTimeout">is the max number of milliseconds of block time</param>
        /// <param name="epStatementHandle">the handle and statement lock associated with the statement</param>
        /// <param name="variableService">provides access to variable values</param>
        /// <param name="statementResultService">handles statement result generation</param>
        /// <param name="timeSourceService">The time source service.</param>
        public EPStatementImpl(String statementId,
                               String statementName,
                               String expressionText,
                               bool isPattern,
                               DispatchService dispatchService,
                               StatementLifecycleSvc statementLifecycleSvc,
                               long timeLastStateChange,
                               bool isBlockingDispatch,
                               bool isSpinBlockingDispatch,
                               long msecBlockingTimeout,
                               EPStatementHandle epStatementHandle,
                               VariableService variableService,
                               StatementResultService statementResultService,
                               TimeSourceService timeSourceService)

        {
            this.isPattern = isPattern;
            this.statementId = statementId;
            this.statementName = statementName;
            this.expressionText = expressionText;
            this.statementLifecycleSvc = statementLifecycleSvc;
            statementListenerSet = new EPStatementListenerSet();
            if (isBlockingDispatch)
            {
                if (isSpinBlockingDispatch)
                {
                    this.dispatchChildView =
                        new UpdateDispatchViewBlockingSpin(statementResultService,
                                                           dispatchService,
                                                           msecBlockingTimeout,
                                                           timeSourceService);

                }
                else
                {
                    this.dispatchChildView =
                        new UpdateDispatchViewBlockingWait(statementResultService,
                                                           dispatchService,
                                                           msecBlockingTimeout);
                }
            }
            else
            {
                this.dispatchChildView =
                    new UpdateDispatchViewNonBlocking(statementResultService, dispatchService);
            }
            this.currentState = EPStatementState.STOPPED;
            this.timeLastStateChange = timeLastStateChange;
            this.epStatementHandle = epStatementHandle;
            this.variableService = variableService;
            this.statementResultService = statementResultService;
            statementResultService.SetUpdateListeners(statementListenerSet);
        }

        /// <summary>
        /// Returns the statement id.
        /// </summary>
        /// <value></value>
        /// <returns>statement id</returns>
        public String StatementId
        {
            get { return statementId; }
        }

        /// <summary>
        /// Start the statement.
        /// </summary>
        public void Start()
        {
            if (statementLifecycleSvc == null)
            {
                throw new IllegalStateException("Cannot start statement, statement is in destroyed state");
            }
            statementLifecycleSvc.Start(statementId);
        }

        /// <summary>
        /// Stop the statement.
        /// </summary>
        public void Stop()
        {
            if (statementLifecycleSvc == null)
            {
                throw new IllegalStateException("Cannot stop statement, statement is in destroyed state");
            }
            statementLifecycleSvc.Stop(statementId);
            // On stop, we give the dispatch view a chance to dispatch final results, if any
            statementResultService.DispatchOnStop();

            dispatchChildView.Clear();
        }

        /// <summary>
        /// Destroy the statement releasing all statement resources.
        /// <p>A destroyed statement cannot be started again.</p>
        /// </summary>
        public void Destroy()
        {
            if (currentState == EPStatementState.DESTROYED)
            {
                throw new IllegalStateException("Statement already destroyed");
            }
            statementLifecycleSvc.Destroy(statementId);
            parentView = null;
            eventType = null;
            dispatchChildView = null;
            statementLifecycleSvc = null;
        }

        /// <summary>
        /// Gets the statement's current state
        /// </summary>
        /// <value></value>
        public EPStatementState State
        {
            get { return currentState; }
        }

        /// <summary>
        /// Set statement state.
        /// </summary>
        /// <value></value>
        public void SetCurrentState(EPStatementState currentState, long timeLastStateChange)
        {
            this.currentState = currentState;
            this.timeLastStateChange = timeLastStateChange;
        }

        /// <summary>
        /// Sets the parent view.
        /// </summary>
        /// <value></value>
        public Viewable ParentView
        {
            set
            {
                if (value == null)
                {
                    if (parentView != null)
                    {
                        parentView.RemoveView(dispatchChildView);
                        parentView = null;
                    }
                }
                else
                {
                    parentView = value;
                    parentView.AddView(dispatchChildView);
                    eventType = parentView.EventType;
                }
            }
        }

        /// <summary>
        /// Returns the underlying expression text or XML.
        /// </summary>
        /// <value></value>
        /// <returns> expression text</returns>
        public String Text
        {
            get { return expressionText; }
        }

        /// <summary>
        /// Returns the statement name.
        /// </summary>
        /// <value></value>
        /// <returns> statement name</returns>
        public String Name
        {
            get { return statementName; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            // Return null if not started
            variableService.SetLocalVersion();
            if (parentView == null)
            {
                return null;
            }

            if (isPattern)
            {
                return EnumerationHelper<Object>.CreateSingletonEnumerator(statementResultService.LastIterableEvent);
            }
            else
            {
                return parentView.GetEnumerator();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<EventBean> GetEnumerator()
        {
            // Return null if not started
            variableService.SetLocalVersion();
            if (parentView == null)
            {
                return null;
            }

            if (isPattern)
            {
                EventBean last = statementResultService.LastIterableEvent;
                if (last == null)
                    return EnumerationHelper<EventBean>.CreateEmptyEnumerator();
                else
                    return EnumerationHelper<EventBean>.CreateSingletonEnumerator(last);
            }
            else
            {
                return parentView.GetEnumerator();
            }
        }

        /// <summary>
        /// Returns a concurrency-safe iterator that iterates over events representing statement results (pull API)
        /// in the face of concurrent event processing by further threads.
        /// <para>
        /// In comparison to the regular iterator, the safe iterator guarantees correct results even
        /// as events are being processed by other threads. The cost is that the iterator holds
        /// one or more locks that must be released. Any locks are acquired at the time this method
        /// is called.
        /// </para>
        /// 	<para>
        /// This method is a blocking method. It may block until statement processing locks are released
        /// such that the safe iterator can acquire any required locks.
        /// </para>
        /// 	<para>
        /// An application MUST explicitly close the safe iterator instance using the close method, to release locks held by the
        /// iterator. The call to the close method should be done in a finally block to make sure
        /// the iterator gets closed.
        /// </para>
        /// 	<para>
        /// Multiple safe iterators may be not be used at the same time by different application threads.
        /// A single application thread may hold and use multiple safe iterators however this is discouraged.
        /// </para>
        /// </summary>
        /// <returns>safe iterator;</returns>
        public IEnumerator<EventBean> GetSafeEnumerator()
        {
            // Return null if not started
            if (parentView == null)
            {
                return null;
            }

            return GetSafeEnumeratorImpl();
        }

        private IEnumerator<EventBean> GetSafeEnumeratorImpl()
        {
            // Set variable version and acquire the lock first
            using (epStatementHandle.StatementLock.AcquireLock(null))
            {
                variableService.SetLocalVersion();

                // Provide iterator - that iterator MUST be closed else the lock is not released
                IEnumerator<EventBean> tempEnum;

                if (isPattern)
                {
                    tempEnum = dispatchChildView.GetEnumerator();
                }
                else
                {
                    tempEnum = parentView.GetEnumerator();
                }

                while( tempEnum.MoveNext() )
                {
                    yield return tempEnum.Current;
                }
            }
        }

        /// <summary>
        /// Returns the type of events the iterable returns.
        /// </summary>
        /// <value></value>
        /// <returns> event type of events the iterator returns
        /// </returns>
        public EventType EventType
        {
            get { return eventType; }
        }

        /// <summary>Returns the set of listeners to the statement.</summary>
        /// <returns>statement listeners</returns>
        public EPStatementListenerSet ListenerSet
        {
            get { return statementListenerSet; }
            set
            {
                statementListenerSet.Copy(value);
                statementResultService.SetUpdateListeners(value);
            }
        }

        /// <summary>Add a listener to the statement.</summary>
        /// <param name="listener">to add</param>
        public void AddListener(UpdateListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentException("Null listener reference supplied");
            }

            statementListenerSet.AddListener(listener);
            statementResultService.SetUpdateListeners(statementListenerSet);
            statementLifecycleSvc.DispatchStatementLifecycleEvent(
                new StatementLifecycleEvent(this, StatementLifecycleEvent.LifecycleEventType.LISTENER_ADD, listener));
        }

        /// <summary>Remove a listeners to a statement.</summary>
        /// <param name="listener">to remove</param>
        public void RemoveListener(UpdateListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentException("Null listener reference supplied");
            }

            statementListenerSet.RemoveListener(listener);
            statementResultService.SetUpdateListeners(statementListenerSet);
            statementLifecycleSvc.DispatchStatementLifecycleEvent(
                new StatementLifecycleEvent(this, StatementLifecycleEvent.LifecycleEventType.LISTENER_REMOVE, listener));
        }

        /// <summary>Remove all listeners to a statement.</summary>
        public void RemoveAllListeners()
        {
            statementListenerSet.RemoveAllListeners();
            statementResultService.SetUpdateListeners(statementListenerSet);
            statementLifecycleSvc.DispatchStatementLifecycleEvent(
                new StatementLifecycleEvent(this, StatementLifecycleEvent.LifecycleEventType.LISTENER_REMOVE_ALL));
        }

        public void AddListener(StatementAwareUpdateListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentException("Null listener reference supplied");
            }

            statementListenerSet.AddListener(listener);
            statementResultService.SetUpdateListeners(statementListenerSet);
            //statementLifecycleSvc.updatedListeners(statementId, statementName, statementListenerSet);
            statementLifecycleSvc.DispatchStatementLifecycleEvent(
                new StatementLifecycleEvent(this, StatementLifecycleEvent.LifecycleEventType.LISTENER_ADD, listener));
        }

        public void RemoveListener(StatementAwareUpdateListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentException("Null listener reference supplied");
            }

            statementListenerSet.RemoveListener(listener);
            statementResultService.SetUpdateListeners(statementListenerSet);
            statementLifecycleSvc.DispatchStatementLifecycleEvent(
                new StatementLifecycleEvent(this, StatementLifecycleEvent.LifecycleEventType.LISTENER_REMOVE, listener));
        }

        public IEnumerable<StatementAwareUpdateListener> StatementAwareListeners
        {
            get { return statementListenerSet.StmtAwareListeners; }
        }

        public IEnumerable<UpdateListener> UpdateListeners
        {
            get { return statementListenerSet.Listeners; }
        }

        public long TimeLastStateChange
        {
            get { return timeLastStateChange; }
        }

        public bool IsStarted
        {
            get { return currentState == EPStatementState.STARTED; }
        }

        public bool IsStopped
        {
            get { return currentState == EPStatementState.STOPPED; }
        }

        public bool IsDestroyed
        {
            get { return currentState == EPStatementState.DESTROYED; }
        }

        public Object Subscriber
        {
            get { return statementListenerSet.Subscriber; }
            set
            {
                statementListenerSet.Subscriber = value;
                statementResultService.SetUpdateListeners(statementListenerSet);
            }
        }

        /// <summary>Returns true if statement is a pattern</summary>
        /// <returns>true if statement is a pattern</returns>
        public bool IsPattern
        {
            get { return isPattern; }
        }

        private void FireEvents(EventBean[] newEvents, EventBean[] oldEvents)
        {
            if ( _internalEvents != null ) {
                _internalEvents(newEvents, oldEvents);
            }
        }

        private void FireEvents(EventBean[] newEvents, EventBean[] oldEvents, EPStatement statement,
                                     EPServiceProvider epServiceProvider)
        {
            if (_internalAwareEvents != null) {
                _internalAwareEvents(newEvents, oldEvents, statement, epServiceProvider);
            }
        }

        /// <summary>
        /// An internal listener that operates on behalf of the statement and routes
        /// events out to their event handlers.
        /// </summary>

        class InternalRouter : UpdateListener, StatementAwareUpdateListener
        {
            private readonly EPStatementImpl eStatement;

            internal InternalRouter(EPStatementImpl eStatement)
            {
                this.eStatement = eStatement;
            }

            public void Update(EventBean[] newEvents, EventBean[] oldEvents)
            {
                eStatement.FireEvents(newEvents, oldEvents);
            }

            public void Update(EventBean[] newEvents, EventBean[] oldEvents, EPStatement statement,
                               EPServiceProvider epServiceProvider)
            {
                eStatement.FireEvents(newEvents, oldEvents, statement, epServiceProvider);
            }
        }
    }
} // End of namespace
