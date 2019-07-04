using System;
using System.Collections.Generic;
using System.Xml;

using com.espertech.esper.client;
using com.espertech.esper.client.time;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.filter;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

using log4net;

using DataMap = com.espertech.esper.compat.Map<string, object>;
using VariableNotFoundException = com.espertech.esper.client.VariableNotFoundException;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Implements runtime interface. Also accepts timer callbacks for synchronizing time
    /// events with regular events sent in.
    /// </summary>

    public class EPRuntimeImpl 
        : EPRuntimeSPI
        , EPRuntimeEventSender
        , InternalEventRouter
    {
        /// <summary>
        /// Number of events received over the lifetime of the event stream processing runtime.
        /// </summary>
        /// <value></value>
        /// <returns> number of events received
        /// </returns>
        public long NumEventsReceived
        {
            get { return services.FilterService.NumEventsEvaluated; }
        }

        /// <summary>
        /// Number of events emitted over the lifetime of the event stream processing runtime.
        /// </summary>
        /// <value></value>
        /// <returns> number of events emitted
        /// </returns>
        public long NumEventsEmitted
        {
            get { return services.EmitService.NumEventsEmitted; }
        }

        /// <summary>
        /// Resets the statistics.
        /// </summary>
        public void ResetStats()
        {
            services.FilterService.ResetStats();
            services.EmitService.ResetStats();
        }

        /// <summary>
        /// Group of data that is associated with the thread.
        /// </summary>

        private class ThreadLocalData
        {
            internal Guid guid;
            internal List<FilterHandle> matchesArrayThreadLocal;
            internal Map<EPStatementHandle, Object> matchesPerStmtThreadLocal;
            internal ArrayBackedCollection<ScheduleHandle> scheduleArrayThreadLocal;
            internal Map<EPStatementHandle, Object> schedulePerStmtThreadLocal;
        }

        /// <summary>
        /// Creates a local data object.
        /// </summary>
        /// <returns></returns>
        private static ThreadLocalData CreateLocalData()
        {
            ThreadLocalData _threadLocalData = new ThreadLocalData();
            _threadLocalData.guid = Guid.NewGuid();
            _threadLocalData.matchesArrayThreadLocal = new List<FilterHandle>(100);
            _threadLocalData.matchesPerStmtThreadLocal = new HashMap<EPStatementHandle, Object>(10000);
            _threadLocalData.scheduleArrayThreadLocal = new ArrayBackedCollection<ScheduleHandle>(100);
            _threadLocalData.schedulePerStmtThreadLocal = new HashMap<EPStatementHandle, Object>(10000);

            return _threadLocalData;
        }

        /// <summary>
        /// Data that remains local to the thread.
        /// </summary>

        //[ThreadStatic]
        private ThreadLocal<ThreadLocalData> threadLocalData;

        /// <summary>
        /// Gets the local data.
        /// </summary>
        /// <value>The local data.</value>
        private ThreadLocalData LocalData
        {
            get { return threadLocalData.GetOrCreate(); }
        }
        
    	private List<FilterHandle> MatchesArray
		{
            get { return LocalData.matchesArrayThreadLocal; }
		}

		private Map<EPStatementHandle, Object> MatchesPerStmt
		{
            get { return LocalData.matchesPerStmtThreadLocal; }
		}

		private ArrayBackedCollection<ScheduleHandle> ScheduleArray
		{
            get { return LocalData.scheduleArrayThreadLocal; }
        }

		private Map<EPStatementHandle, Object> SchedulePerStmt
		{
            get { return LocalData.schedulePerStmtThreadLocal; }
        }
		
        private EPServicesContext services;
        private readonly bool isLatchStatementInsertStream;
        private bool isUsingExternalClocking;
        private volatile UnmatchedListener unmatchedListener;

        private readonly bool isDebugEnabled;

        /// <summary> Constructor.</summary>
        /// <param name="services">references to services
        /// </param>
        public EPRuntimeImpl(EPServicesContext services)
        {
            this.isDebugEnabled = log.IsDebugEnabled;
            this.services = services;
            this.isLatchStatementInsertStream = services.EngineSettingsService.EngineSettings.Threading.IsInsertIntoDispatchPreserveOrder;
            this.isUsingExternalClocking = !this.services.EngineSettingsService.EngineSettings.Threading.IsInternalTimerEnabled;
            this.threadLocalData = new FastThreadLocal<ThreadLocalData>(CreateLocalData);
        }

        /// <summary>
        /// Invoked by the internal clocking service at regular intervals.
        /// </summary>
        public virtual void TimerCallback()
        {
            if (ExecutionPathDebugLog.isDebugEnabled && isDebugEnabled)
            {
                log.Debug(".TimerCallback Evaluating scheduled callbacks");
            }

            long msec = services.TimeSource.GetTimeMillis();
            CurrentTimeEvent currentTimeEvent = new CurrentTimeEvent(msec);
            SendEvent(currentTimeEvent);
        }

        /// <summary>
        /// Sends the event.
        /// </summary>
        /// <param name="_event">The _event.</param>
        public virtual void SendEvent(Object _event)
        {
            if (_event == null)
            {
                log.Fatal(".SendEvent Null object supplied");
                return;
            }

            if (ExecutionPathDebugLog.isDebugEnabled && isDebugEnabled)
            {
                log.Debug(".SendEvent Processing event " + _event);
            }

            // Process event
            ProcessEvent(_event);
        }

        /// <summary>
        /// Sends the event.
        /// </summary>
        /// <param name="document">The document.</param>
        public virtual void SendEvent(XmlNode document)
        {
            if (document == null)
            {
                log.Fatal(".SendEvent Null object supplied");
                return;
            }

            if (ExecutionPathDebugLog.isDebugEnabled && isDebugEnabled)
            {
                log.Debug(".SendEvent Processing DOM node event " + document);
            }

            // Get it wrapped up, process event
            EventBean eventBean = services.EventAdapterService.AdapterForDOM(document);
            ProcessEvent(eventBean);
        }

        /// <summary>
        /// Send a map containing event property values to the event stream processing runtime.
        /// Use the route method for sending events into the runtime from within UpdateListener code.
        /// </summary>
        /// <param name="map">map that contains event property values. Keys are expected to be of type String while values
        /// can be of any type. Keys and values should match those declared via Configuration for the given eventTypeAlias.</param>
        /// <param name="eventTypeAlias">the alias for the (property name, property type) information for this map</param>
        /// <throws>  EPException - when the processing of the event leads to an error </throws>
        public virtual void SendEvent(DataMap map, String eventTypeAlias)
        {
            if (map == null)
            {
                throw new ArgumentException("Invalid null event object");
            }

            if (ExecutionPathDebugLog.isDebugEnabled && isDebugEnabled)
            {
                log.Debug(string.Format(".SendEvent Processing event {0}", map));
            }

            // Process event
            EventBean eventBean = services.EventAdapterService.AdapterForMap(map, eventTypeAlias);
            ProcessEvent(eventBean);
        }

        /// <summary>
        /// Creates a delegate that can be used to send mapped events to the runtime.  This method
        /// eliminates the costs associated with the lookup of an event type or any other form of
        /// initialization that would normally be incurred.
        /// </summary>
        /// <param name="eventTypeAlias"></param>
        /// <returns></returns>

        public virtual EPSender GetSender(String eventTypeAlias)
        {
            EventType eventType = services.EventAdapterService.GetEventTypeByAlias(eventTypeAlias);
            EPSender eventSender = 
                delegate(DataMap mappedEvent)
                    {
                        EventBean eventbean = services.EventAdapterService.CreateMapFromValues(mappedEvent, eventType);
                        ProcessEvent(eventbean);
                    };

            return eventSender;
        }

        /// <summary>
        /// Route the event object back to the event stream processing runtime for internal dispatching.
        /// The route event is processed just like it was sent to the runtime, that is any
        /// active expressions seeking that event receive it. The routed event has priority over other
        /// events sent to the runtime. In a single-threaded application the routed event is
        /// processed before the next event is sent to the runtime through the
        /// EPRuntime.sendEvent method.
        /// </summary>
        /// <param name="_event"></param>
        public virtual void Route(Object _event)
        {
            ThreadWorkQueue.Add(_event);
        }

        /// <summary>
        /// Internal route of events via insert-into, holds a statement lock
        /// </summary>
        public virtual void Route(EventBean @event, EPStatementHandle epStatementHandle)
        {
            if (isLatchStatementInsertStream)
            {
                InsertIntoLatchFactory insertIntoLatchFactory = epStatementHandle.InsertIntoLatchFactory;
                Object latch = insertIntoLatchFactory.NewLatch(@event);
                ThreadWorkQueue.Add(latch);
            }
            else
            {
                ThreadWorkQueue.Add(@event);
            }
        }

        /// <summary>
        /// Emit an event object to any registered EmittedListener instances listening to the default channel.
        /// </summary>
        /// <param name="_object"></param>
        public virtual void Emit(Object _object)
        {
            services.EmitService.EmitEvent(_object, null);
        }

        /// <summary>
        /// Emit an event object to any registered EmittedListener instances on the specified channel.
        /// Event listeners listening to all channels as well as those listening to the specific channel
        /// are called. Supplying a null value in the channel has the same result as the Emit(Object object) method.
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="channel">channel to emit the object to, or null if emitting to the default channel</param>
        public virtual void Emit(Object _object, String channel)
        {
            services.EmitService.EmitEvent(_object, channel);
        }

        /// <summary>
        /// Register an object that listens for events emitted from the event stream processing runtime on the
        /// specified channel. A null value can be supplied for the channel in which case the
        /// emit listener will be invoked for events emitted an any channel.
        /// </summary>
        /// <param name="listener">called when an event is emitted by the runtime.</param>
        /// <param name="channel">is the channel to add the listener to, a null value can be used to listen to events emitted
        /// on all channels</param>
        public virtual void AddEmittedListener(EmittedListener listener, String channel)
        {
            services.EmitService.AddListener(listener, channel);
        }

        /// <summary>
        /// Deregister all emitted event listeners.
        /// </summary>
        public virtual void ClearEmittedListeners()
        {
            services.EmitService.ClearListeners();
        }

        /// <summary>
        /// Processes the event.
        /// </summary>
        /// <param name="_event">The _event.</param>
        private void ProcessEvent(Object _event)
        {
            if (_event is TimerEvent)
            {
                ProcessTimeEvent((TimerEvent) _event);
                return;
            }

            EventBean eventBean;

            if (_event is EventBean)
            {
                eventBean = (EventBean) _event;
            }
            else
            {
                eventBean = services.EventAdapterService.AdapterForBean(_event);
            }
            ProcessWrappedEvent(eventBean);
        }

        public void ProcessWrappedEvent(EventBean eventBean)
        {
            ManagedReadWriteLock lockObj = services.EventProcessingRWLock;

            // Acquire main processing lock which locks out statement management
            using (new ReaderLock(lockObj))
            {
                try
                {
                    ProcessMatches(eventBean);
                }
                catch (EPException) 
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new EPException(ex);
                }
            }

            // Dispatch results to listeners
            // Done outside of the read-lock to prevent lockups when listeners create statements
            Dispatch();

            // Work off the event queue if any events accumulated in there via a route() or insert-into
            ProcessThreadWorkQueue();
        }

        /// <summary>
        /// Processes the time event.
        /// </summary>
        /// <param name="_event">The _event.</param>
        private void ProcessTimeEvent(TimerEvent _event)
        {
            if (_event is TimerControlEvent)
            {
                TimerControlEvent timerControlEvent = (TimerControlEvent)_event;
                if (timerControlEvent.ClockType == TimerControlEvent.ClockTypeEnum.CLOCK_INTERNAL)
                {
                    // Start internal clock which supplies CurrentTimeEvent events every 100ms
                    // This may be done without delay thus the write lock indeed must be reentrant.
                    services.TimerService.StartInternalClock();
                    isUsingExternalClocking = false;
                }
                else
                {
                    // Stop internal clock, for unit testing and for external clocking
                    services.TimerService.StopInternalClock(true);
                    isUsingExternalClocking = true;
                }

                return;
            }

            // Evaluation of all time events is protected from regular event stream processing
            if (ExecutionPathDebugLog.isDebugEnabled && isDebugEnabled)
            {
				log.Debug(".ProcessTimeEvent Setting time and evaluating schedules");
			}

			CurrentTimeEvent current = (CurrentTimeEvent)_event;
			long currentTime = current.TimeInMillis;
            if (isUsingExternalClocking && (currentTime == services.SchedulingService.Time)) {
                if (log.IsWarnEnabled) {
                    log.Warn("Duplicate time event received for currentTime " + currentTime);
                }
            }

            services.SchedulingService.Time = currentTime;

			ProcessSchedule();

			// Let listeners know of results
			Dispatch();

			// Work off the event queue if any events accumulated in there via a Route()
			ProcessThreadWorkQueue();
        }

	    private void ProcessSchedule()
	    {
	        ArrayBackedCollection<ScheduleHandle> handles = ScheduleArray;

	        // Evaluation of schedules is protected by an optional scheduling service lock and then the
            // engine lock.  We want to stay in this order for allowing the engine lock as a second-order
            // lock to the services own lock, if it has one.

            using (new ReaderLock(services.EventProcessingRWLock))
            {
                services.SchedulingService.Evaluate(handles);
	            ProcessScheduleHandles(handles);
	        }
	    }

	    private void ProcessScheduleHandles(ArrayBackedCollection<ScheduleHandle> handles)
	    {
	        if (ThreadLogUtil.ENABLED_TRACE)
	        {
	            ThreadLogUtil.Trace("Found schedules for", handles.Count);
	        }

	        if (handles.Count == 0)
	        {
	            return;
	        }

	        // handle 1 result separatly for performance reasons
	        if (handles.Count == 1)
	        {
	            Object[] handleArray = handles.Array;
	            EPStatementHandleCallback handle = (EPStatementHandleCallback) handleArray[0];
	            ManagedLock statementLock = handle.EpStatementHandle.StatementLock;
                using (statementLock.AcquireLock(services.StatementLockFactory))
                {
                    if (handle.EpStatementHandle.HasVariables)
                    {
                        services.VariableService.SetLocalVersion();
                    }

                    handle.ScheduleCallback.ScheduledTrigger(services.ExtensionServicesContext);
                    handle.EpStatementHandle.InternalDispatch();
                }
	            handles.Clear();
	            return;
	        }

	        Object[] matchArray = handles.Array;
	        int entryCount = handles.Count;

	        // sort multiple matches for the event into statements
	        Map<EPStatementHandle, Object> stmtCallbacks = SchedulePerStmt;
	        stmtCallbacks.Clear();
	        for (int i = 0; i < entryCount; i++)    // need to use the size of the collection
	        {
	            EPStatementHandleCallback handleCallback = (EPStatementHandleCallback) matchArray[i];
	            EPStatementHandle handle = handleCallback.EpStatementHandle;
	            ScheduleHandleCallback callback = handleCallback.ScheduleCallback;

	            Object entry = stmtCallbacks.Get(handle);

	            // This statement has not been encountered before
	            if (entry == null)
	            {
	                stmtCallbacks[handle] = callback;
	                continue;
	            }

	            // This statement has been encountered once before
	            if (entry is ScheduleHandleCallback)
	            {
	                ScheduleHandleCallback existingCallback = (ScheduleHandleCallback) entry;
	                LinkedList<ScheduleHandleCallback> entries = new LinkedList<ScheduleHandleCallback>();
	                entries.AddLast(existingCallback);
	                entries.AddLast(callback);
	                stmtCallbacks[handle] = entries;
	                continue;
	            }

	            // This statement has been encountered more then once before
	            LinkedList<ScheduleHandleCallback> _entries = (LinkedList<ScheduleHandleCallback>) entry;
	            _entries.AddLast(callback);
	        }
	        handles.Clear();

            foreach (KeyValuePair<EPStatementHandle, Object> entry in stmtCallbacks)
	        {
	            EPStatementHandle handle = entry.Key;
	            Object callbackObject = entry.Value;

                using(handle.StatementLock.AcquireLock(services.StatementLockFactory))
                {
                    if (handle.HasVariables)
                    {
                        services.VariableService.SetLocalVersion();
                    }

                    LinkedList<ScheduleHandleCallback> callbackList = callbackObject as LinkedList<ScheduleHandleCallback>;
                    if (callbackList != null)
	                {
	                    foreach (ScheduleHandleCallback callback in callbackList)
	                    {
	                        callback.ScheduledTrigger(services.ExtensionServicesContext);
	                    }
	                }
	                else
	                {
	                    ScheduleHandleCallback callback = (ScheduleHandleCallback) callbackObject;
	                    callback.ScheduledTrigger(services.ExtensionServicesContext);
	                }

	                // internal join processing, if applicable
	                handle.InternalDispatch();
	            }
	        }
	    }

        private void ProcessThreadWorkQueue()
        {
            Object item;
            while ((item = ThreadWorkQueue.Next()) != null)
            {
                if (item is InsertIntoLatchSpin)
                {
                    ProcessThreadWorkQueueLatchedSpin((InsertIntoLatchSpin)item);
                }
                else if (item is InsertIntoLatchWait)
                {
                    ProcessThreadWorkQueueLatchedWait((InsertIntoLatchWait)item);
                }
                else
                {
                    ProcessThreadWorkQueueUnlatched(item);
                }
            }

            // Process named window deltas
            bool haveDispatched = services.NamedWindowService.Dispatch();
            if (haveDispatched)
            {
                // Dispatch results to listeners
                Dispatch();
            }

            if (!ThreadWorkQueue.IsEmpty)
            {
                ProcessThreadWorkQueue();
            }
        }

        private void ProcessThreadWorkQueueLatchedWait(InsertIntoLatchWait insertIntoLatch)
        {
            // wait for the latch to complete
            Object item = insertIntoLatch.Await();

            EventBean eventBean;
            if (item is EventBean)
            {
                eventBean = (EventBean) item;
            }
            else
            {
                eventBean = services.EventAdapterService.AdapterForBean(item);
            }

            services.EventProcessingRWLock.AcquireReadLock();
            try
            {
                ProcessMatches(eventBean);
            }
            finally
            {
                insertIntoLatch.Done();
                services.EventProcessingRWLock.ReleaseReadLock();
            }

            Dispatch();
        }

        private void ProcessThreadWorkQueueLatchedSpin(InsertIntoLatchSpin insertIntoLatch)
        {
            // wait for the latch to complete
            Object item = insertIntoLatch.Await();

            EventBean eventBean;
            if (item is EventBean)
            {
                eventBean = (EventBean) item;
            }
            else
            {
                eventBean = services.EventAdapterService.AdapterForBean(item);
            }

            services.EventProcessingRWLock.AcquireReadLock();
            try
            {
                ProcessMatches(eventBean);
            }
            finally
            {
                insertIntoLatch.Done();
                services.EventProcessingRWLock.ReleaseReadLock();
            }

            Dispatch();
        }

        private void ProcessThreadWorkQueueUnlatched(Object item)
        {
            EventBean eventBean;
            if (item is EventBean)
            {
                eventBean = (EventBean) item;
            }
            else
            {
                eventBean = services.EventAdapterService.AdapterForBean(item);
            }

            services.EventProcessingRWLock.AcquireReadLock();
            try
            {
                ProcessMatches(eventBean);
            }
            finally
            {
                services.EventProcessingRWLock.ReleaseReadLock();
            }

            Dispatch();
        }

        private void ProcessMatches(EventBean _event)
	    {
	        // get matching filters
            IList<FilterHandle> matches = MatchesArray;
            // clear the array
            matches.Clear();
            // get matches
	        services.FilterService.Evaluate(_event, matches);

	        if (ThreadLogUtil.ENABLED_TRACE)
	        {
	            ThreadLogUtil.Trace("Found matches for underlying ", matches.Count, _event.Underlying);
	        }

	        if (matches.Count == 0)
	        {
                if (unmatchedListener != null)
                {
                    unmatchedListener.Update(_event);
                }
	            return;
	        }

	        Map<EPStatementHandle, Object> stmtCallbacks = MatchesPerStmt;

            foreach( EPStatementHandleCallback handleCallback in matches)
            {
                EPStatementHandle handle = handleCallback.EpStatementHandle;

                // Self-joins require that the internal dispatch happens after all streams are evaluated
                if (handle.IsCanSelfJoin)
                {
                    List<FilterHandleCallback> callbacks = (List<FilterHandleCallback>) stmtCallbacks.Get(handle);
                    if (callbacks == null)
                    {
                        callbacks = new List<FilterHandleCallback>();
                        stmtCallbacks[handle] = callbacks;
                    }
                    callbacks.Add(handleCallback.FilterCallback);
                    continue;
                }

                using (handle.StatementLock.AcquireLock(services.StatementLockFactory))
                {
                    if (handle.HasVariables)
                    {
                        services.VariableService.SetLocalVersion();
                    }

                    handleCallback.FilterCallback.MatchFound(_event);

                    // internal join processing, if applicable
                    handle.InternalDispatch();
                }
            }

            matches.Clear();
	        if (stmtCallbacks.Count == 0)
	        {
	            return;
	        }

	        foreach (KeyValuePair<EPStatementHandle,Object> entry in stmtCallbacks)
	        {
	            EPStatementHandle handle = entry.Key;
	            
	            using(handle.StatementLock.AcquireLock(services.StatementLockFactory))
	            {
                    if (handle.HasVariables)
                    {
                        services.VariableService.SetLocalVersion();
                    }

	                List<FilterHandleCallback> callbackList = (List<FilterHandleCallback>) entry.Value;
	                int callbackListCount = callbackList.Count;
                    for (int ii = 0; ii < callbackListCount; ii++)
                    {
                        callbackList[ii].MatchFound(_event);
                    }

	                // internal join processing, if applicable
	                handle.InternalDispatch();
	            }
	        }
	        stmtCallbacks.Clear();
	    }

        private void Dispatch()
        {
            try
            {
                services.DispatchService.Dispatch();
            }
            catch (SystemException ex)
            {
                throw new EPException(ex);
            }
        }

        /// <summary>
        /// Destroy for destroying an engine instance: sets references to null and clears thread-locals
        /// </summary>

        public void Destroy()
        {
            services = null;
            threadLocalData = null;
        }

        /// <summary>
        /// Gets or sets the unmatched listener.
        /// </summary>
        /// <value>The unmatched listener.</value>
        public UnmatchedListener UnmatchedListener
        {
            get { return this.unmatchedListener; }
            set { this.unmatchedListener = value; }
        }

        public void SetVariableValue(String variableName, Object variableValue)
        {
            VariableReader reader = services.VariableService.GetReader(variableName);
            if (reader == null) {
                throw new VariableNotFoundException("Variable by name '" + variableName + "' has not been declared");
            }

            services.VariableService.CheckAndWrite(reader.VariableNumber, variableValue);
            services.VariableService.Commit();
        }

        public void SetVariableValue(DataMap variableValues)
        {
            foreach (KeyValuePair<String, Object> entry in variableValues) {
                String variableName = entry.Key;
                VariableReader reader = services.VariableService.GetReader(variableName);
                if (reader == null) {
                    services.VariableService.Rollback();
                    throw new VariableNotFoundException("Variable by name '" + variableName + "' has not been declared");
                }

                try {
                    services.VariableService.CheckAndWrite(reader.VariableNumber, entry.Value);
                } catch (Exception) {
                    services.VariableService.Rollback();
                    throw;
                }
            }

            services.VariableService.Commit();
        }

        public Object GetVariableValue(String variableName)
        {
            services.VariableService.SetLocalVersion();
            VariableReader reader = services.VariableService.GetReader(variableName);
            if (reader == null) {
                throw new VariableNotFoundException("Variable by name '" + variableName + "' has not been declared");
            }
            return reader.Value;
        }

        public DataMap GetVariableValue(Set<String> variableNames)
        {
            services.VariableService.SetLocalVersion();
            DataMap values = new HashMap<String, Object>();
            foreach (String variableName in variableNames) {
                VariableReader reader = services.VariableService.GetReader(variableName);
                if (reader == null) {
                    throw new VariableNotFoundException("Variable by name '" + variableName + "' has not been declared");
                }

                Object value = reader.Value;
                values.Put(variableName, value);
            }
            return values;
        }

        public DataMap GetVariableValueAll()
        {
            services.VariableService.SetLocalVersion();
            Map<String, VariableReader> variables = services.VariableService.Variables;
            Map<String, Object> values = new HashMap<String, Object>();
            foreach (KeyValuePair<String, VariableReader> entry in variables) {
                Object value = entry.Value.Value;
                values.Put(entry.Value.VariableName, value);
            }
            return values;
        }

        public EPQueryResult ExecuteQuery(String epl)
        {
            try {
                EPPreparedExecuteMethod executeMethod = GetExecuteMethod(epl);
                EPPreparedQueryResult result = executeMethod.Execute();
                return new EPQueryResultImpl(result);
            } catch (EPStatementException) {
                throw;
            } catch (Exception ex) {
                String message = "Error executing statement: " + ex.Message;
                log.Debug(message, ex);
                throw new EPStatementException(message, epl);
            }
        }

        public EPPreparedQuery PrepareQuery(String epl)
        {
            try {
                EPPreparedExecuteMethod startMethod = GetExecuteMethod(epl);
                return new EPPreparedQueryImpl(startMethod, epl);
            } catch (EPStatementException) {
                throw;
            } catch (Exception ex) {
                String message = "Error executing statement: " + ex.Message;
                log.Debug(message, ex);
                throw new EPStatementException(message, epl);
            }
        }

        private EPPreparedExecuteMethod GetExecuteMethod(String epl)
        {
            String stmtName = UuidGenerator.Generate(epl);
            String stmtId = UuidGenerator.Generate(epl + " ");

            try {
                StatementSpecRaw spec =
                    EPAdministratorImpl.CompileEPL(epl, stmtName, services, SelectClauseStreamSelectorEnum.ISTREAM_ONLY);
                StatementContext statementContext =
                    services.StatementContextFactory.MakeContext(stmtId,
                                                                 stmtName,
                                                                 epl,
                                                                 false,
                                                                 services,
                                                                 null,
                                                                 null,
                                                                 null);
                StatementSpecCompiled compiledSpec = StatementLifecycleSvcImpl.Compile(spec, epl, statementContext);
                return new EPPreparedExecuteMethod(compiledSpec, services, statementContext);
            } catch (EPStatementException) {
                throw;
            } catch (Exception ex) {
                String message = "Error executing statement: " + ex.Message;
                log.Debug(message, ex);
                throw new EPStatementException(message, epl);
            }
        }

        public EventSender GetEventSender(String eventTypeAlias)
        {
            return services.EventAdapterService.GetStaticTypeEventSender(this, eventTypeAlias);
        }

        public EventSender GetEventSender(IEnumerable<Uri> uri)
        {
            return services.EventAdapterService.GetDynamicTypeEventSender(this, uri);
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
