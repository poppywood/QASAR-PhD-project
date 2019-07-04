///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.named;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.filter;
using com.espertech.esper.pattern;
using com.espertech.esper.util;
using com.espertech.esper.view;

using log4net;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.core
{
    /// <summary>Provides statement lifecycle services. </summary>
    public class StatementLifecycleSvcImpl : StatementLifecycleSvc
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Internal lock for object.
        /// </summary>
        protected readonly MonitorLock objLock = new MonitorLock();

        /// <summary>Services context for statement lifecycle management. </summary>
        protected readonly EPServicesContext services;

        /// <summary>Maps of statement id to descriptor. </summary>
        protected readonly Map<String, EPStatementDesc> stmtIdToDescMap;

        /// <summary>Map of statement name to statement. </summary>
        protected readonly Map<String, EPStatement> stmtNameToStmtMap;

        private readonly EPServiceProvider epServiceProvider;
        private readonly ManagedReadWriteLock eventProcessingRWLock;

        private readonly Map<String, String> stmtNameToIdMap;

        // Observers to statement-related events
        private readonly Set<StatementLifecycleObserver> observers;

        /// <summary>Ctor. </summary>
        /// <param name="epServiceProvider">is the engine instance to hand to statement-aware listeners</param>
        /// <param name="services">is engine services</param>
        public StatementLifecycleSvcImpl(EPServiceProvider epServiceProvider, EPServicesContext services)
        {
            this.services = services;
            this.epServiceProvider = epServiceProvider;

            // lock for starting and stopping statements
            this.eventProcessingRWLock = services.EventProcessingRWLock;

            this.stmtIdToDescMap = new HashMap<String, EPStatementDesc>();
            this.stmtNameToStmtMap = new HashMap<String, EPStatement>();
            this.stmtNameToIdMap = new HashMap<String, String>();

            observers = new CopyOnWriteArraySet<StatementLifecycleObserver>();
        }

        /// <summary>
        /// Add an observer to be called back when statement-state or listener/subscriber changes are registered.
        /// <para/>
        /// The observers list is backed by a Set.
        /// </summary>
        /// <param name="observer">to add</param>
        public void AddObserver(StatementLifecycleObserver observer)
        {
            observers.Add(observer);
        }

        /// <summary>
        /// Destroy the service.
        /// </summary>
        public void Destroy()
        {
            this.DestroyAllStatements();
        }

        /// <summary>
        /// Initialized the service before use.
        /// </summary>
        public void Init()
        {
            // called after services are activated, to begin statement loading from store
        }

        /// <summary>
        /// Create and start the statement.
        /// </summary>
        /// <param name="statementSpec">is the statement definition in bean object form, raw unvalidated and unoptimized.</param>
        /// <param name="expression">is the expression text</param>
        /// <param name="isPattern">is an indicator on whether this is a pattern statement and thus the iterator must return the last result,
        /// versus for non-pattern statements the iterator returns view content.</param>
        /// <param name="optStatementName">is an optional statement name, null if none was supplied</param>
        /// <returns>started statement</returns>
        public EPStatement CreateAndStart(StatementSpecRaw statementSpec, String expression, bool isPattern, String optStatementName)
        {
            using( objLock.Acquire() ) {
                // Generate statement id
                String statementId = UuidGenerator.Generate(expression);
                return CreateAndStart(statementSpec, expression, isPattern, optStatementName, statementId, null);
            }
        }

        /// <summary>Creates and starts statement. </summary>
        /// <param name="statementSpec">defines the statement</param>
        /// <param name="expression">is the EPL</param>
        /// <param name="isPattern">is true for patterns</param>
        /// <param name="optStatementName">is the optional statement name</param>
        /// <param name="statementId">is the statement id</param>
        /// <param name="optAdditionalContext">additional context for use by the statement context</param>
        /// <returns>started statement</returns>
        protected EPStatement CreateAndStart(StatementSpecRaw statementSpec,
                                             String expression,
                                             bool isPattern,
                                             String optStatementName,
                                             String statementId,
                                             DataMap optAdditionalContext)
        {
            using( objLock.Acquire() ) {
                // Determine a statement name, i.e. use the id or use/generate one for the name passed in
                String statementName = statementId;
                if (optStatementName != null) {
                    statementName = GetUniqueStatementName(optStatementName, statementId);
                }

                EPStatementDesc desc =
                    CreateStopped(statementSpec,
                                  expression,
                                  isPattern,
                                  statementName,
                                  statementId,
                                  optAdditionalContext);
                Start(statementId, desc, true);
                return desc.EpStatement;
            }
        }

        /// <summary>Creates a started statement. </summary>
        /// <param name="statementSpec">is the statement def</param>
        /// <param name="expression">is the expression text</param>
        /// <param name="isPattern">is true for patterns,</param>
        /// <param name="statementName">is the statement name</param>
        /// <param name="statementId">is the statement id</param>
        /// <param name="optAdditionalContext">additional context for use by the statement context</param>
        /// <returns>statement</returns>
        protected EPStatement CreateStarted(StatementSpecRaw statementSpec,
                                            String expression,
                                            bool isPattern,
                                            String statementName,
                                            String statementId,
                                            DataMap optAdditionalContext)
        {
            using( objLock.Acquire() ) {
                if (log.IsDebugEnabled) {
                    log.Debug(".start Creating and starting statement " + statementId);
                }
                EPStatementDesc desc =
                    CreateStopped(statementSpec, expression, isPattern, statementName, statementId, optAdditionalContext);
                Start(statementId, desc, true);
                return desc.EpStatement;
            }
        }

        /// <summary>Create stopped statement. </summary>
        /// <param name="statementSpec">statement definition</param>
        /// <param name="expression">is the expression text</param>
        /// <param name="isPattern">is true for patterns, false for non-patterns</param>
        /// <param name="statementName">is the statement name assigned or given</param>
        /// <param name="statementId">is the statement id</param>
        /// <param name="optAdditionalContext">additional context for use by the statement context</param>
        /// <returns>stopped statement</returns>
        protected EPStatementDesc CreateStopped(StatementSpecRaw statementSpec,
                                                String expression,
                                                bool isPattern,
                                                String statementName,
                                                String statementId,
                                                DataMap optAdditionalContext)
        {
            using( objLock.Acquire() ) {
                EPStatementDesc statementDesc;

                StatementContext statementContext =
                    services.StatementContextFactory.MakeContext(statementId,
                                                                 statementName,
                                                                 expression,
                                                                 statementSpec.HasVariables,
                                                                 services,
                                                                 optAdditionalContext,
                                                                 statementSpec.OnTriggerDesc,
                                                                 statementSpec.CreateWindowDesc);
                StatementSpecCompiled compiledSpec;
                try {
                    compiledSpec = Compile(statementSpec, expression, statementContext);
                } catch (EPStatementException) {
                    stmtNameToIdMap.Remove(statementName); // Clean out the statement name as it's already assigned
                    throw;
                }

                // For insert-into streams, create a lock taken out as soon as an event is inserted
                // Makes the processing between chained statements more predictable.
                if (statementSpec.InsertIntoDesc != null) {
                    String insertIntoStreamName = statementSpec.InsertIntoDesc.EventTypeAlias;
                    String latchFactoryName = "insert_stream_" + insertIntoStreamName + "_" + statementId;
                    long msecTimeout = services.EngineSettingsService.EngineSettings.Threading.InsertIntoDispatchTimeout;
                    ConfigurationEngineDefaults.Locking locking =
                        services.EngineSettingsService.EngineSettings.Threading.InsertIntoDispatchLocking;
                    InsertIntoLatchFactory latchFactory =
                        new InsertIntoLatchFactory(latchFactoryName, msecTimeout, locking, services.TimeSource);
                    statementContext.EpStatementHandle.InsertIntoLatchFactory = latchFactory;
                }

                // In a join statements if the same event type or it's deep super types are used in the join more then once,
                // then this is a self-join and the statement handle must know to dispatch the results together
                bool canSelfJoin = IsPotentialSelfJoin(compiledSpec.StreamSpecs);
                statementContext.EpStatementHandle.CanSelfJoin = canSelfJoin;

                using( new WriterLock(eventProcessingRWLock)) {
                    try {
                        // create statement - may fail for parser and simple validation errors
                        bool preserveDispatchOrder =
                            services.EngineSettingsService.EngineSettings.Threading.IsListenerDispatchPreserveOrder;
                        bool isSpinLocks =
                            services.EngineSettingsService.EngineSettings.Threading.ListenerDispatchLocking ==
                            ConfigurationEngineDefaults.Locking.SPIN;
                        long blockingTimeout =
                            services.EngineSettingsService.EngineSettings.Threading.ListenerDispatchTimeout;
                        long timeLastStateChange = services.SchedulingService.Time;
                        EPStatementSPI statement = new EPStatementImpl(
                            statementId,
                            statementName,
                            expression,
                            isPattern,
                            services.DispatchService,
                            this,
                            timeLastStateChange,
                            preserveDispatchOrder,
                            isSpinLocks,
                            blockingTimeout,
                            statementContext.EpStatementHandle,
                            statementContext.VariableService,
                            statementContext.StatementResultService,
                            services.TimeSource);

                        bool isInsertInto = statementSpec.InsertIntoDesc != null;
                        statementContext.StatementResultService.SetContext(statement,
                                                                           epServiceProvider,
                                                                           isInsertInto,
                                                                           isPattern);

                        // create start method
                        EPStatementStartMethod startMethod =
                            new EPStatementStartMethod(compiledSpec, services, statementContext);

                        // keep track of the insert-into statements supplying streams.
                        // these may need to lock to get more predictable behavior for multithreaded processing.
                        String insertIntoStreamName = null;
                        if (statementSpec.InsertIntoDesc != null) {
                            insertIntoStreamName = statementSpec.InsertIntoDesc.EventTypeAlias;
                        }

                        statementDesc =
                            new EPStatementDesc(statement,
                                                startMethod,
                                                null,
                                                insertIntoStreamName,
                                                statementContext.EpStatementHandle);
                        stmtIdToDescMap.Put(statementId, statementDesc);
                        stmtNameToStmtMap.Put(statementName, statement);
                        stmtNameToIdMap.Put(statementName, statementId);

                        DispatchStatementLifecycleEvent(
                            new StatementLifecycleEvent(statement, StatementLifecycleEvent.LifecycleEventType.CREATE));
                    } catch (Exception) {
                        stmtIdToDescMap.Remove(statementId);
                        stmtNameToIdMap.Remove(statementName);
                        stmtNameToStmtMap.Remove(statementName);
                        throw;
                    }
                }

                return statementDesc;
            }
        }

        private static bool IsPotentialSelfJoin(ICollection<StreamSpecCompiled> streamSpecs)
        {
            // not a join (pattern doesn't count)
            if (streamSpecs.Count == 1)
            {
                return false;
            }

            // join - determine types joined
            List<EventType> filteredTypes = new List<EventType>();
            bool hasFilterStream = false;
            foreach (StreamSpecCompiled streamSpec in streamSpecs)
            {
                if (streamSpec is FilterStreamSpecCompiled)
                {
                    EventType type = ((FilterStreamSpecCompiled) streamSpec).FilterSpec.EventType;
                    filteredTypes.Add(type);
                    hasFilterStream = true;
                }
                else if (streamSpec is PatternStreamSpecCompiled)
                {
                    EvalNodeAnalysisResult evalNodeAnalysisResult = EvalNode.RecursiveAnalyzeChildNodes(((PatternStreamSpecCompiled)streamSpec).EvalNode);
                    IList<EvalFilterNode> filterNodes = evalNodeAnalysisResult.FilterNodes;
                    foreach (EvalFilterNode filterNode in filterNodes)
                    {
                        filteredTypes.Add(filterNode.FilterSpec.EventType);
                    }
                }
            }

            if (filteredTypes.Count == 1)
            {
                return false;
            }
            // pattern-only streams are not self-joins
            if (!hasFilterStream)
            {
                return false;
            }

            // is type overlap
            for (int i = 0; i < filteredTypes.Count; i++)
            {
                for (int j = i + 1; j < filteredTypes.Count; j++)
                {
                    EventType typeOne = filteredTypes[i];
                    EventType typeTwo = filteredTypes[j];
                    if (typeOne == typeTwo)
                    {
                        return true;
                    }

                    if (typeOne.SuperTypes != null)
                    {
                        foreach (EventType typeOneSuper in typeOne.SuperTypes)
                        {
                            if (typeOneSuper == typeTwo)
                            {
                                return true;
                            }
                        }
                    }
                    if (typeTwo.SuperTypes != null)
                    {
                        foreach (EventType typeTwoSuper in typeTwo.SuperTypes)
                        {
                            if (typeOne == typeTwoSuper)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Start statement by statement id.
        /// </summary>
        /// <param name="statementId">of the statement to start.</param>
        public void Start(String statementId)
        {
            using( objLock.Acquire() ) {
                if (log.IsDebugEnabled) {
                    log.Debug(".start Starting statement " + statementId);
                }

                // Acquire a lock for event processing as threads may be in the views used by the statement
                // and that could conflict with the destroy of views
                eventProcessingRWLock.AcquireWriteLock();
                try {
                    EPStatementDesc desc = stmtIdToDescMap.Get(statementId);
                    if (desc == null) {
                        throw new IllegalStateException("Cannot start statement, statement is in destroyed state");
                    }
                    StartInternal(statementId, desc, false);
                } finally {
                    eventProcessingRWLock.ReleaseWriteLock();
                }
            }
        }

        /// <summary>Start the given statement. </summary>
        /// <param name="statementId">is the statement id</param>
        /// <param name="desc">is the cached statement info</param>
        /// <param name="isNewStatement">indicator whether the statement is new or a stop-restart statement</param>
        public void Start(String statementId, EPStatementDesc desc, bool isNewStatement)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(".start Starting statement " + statementId + " from desc=" + desc);
            }

            // Acquire a lock for event processing as threads may be in the views used by the statement
            // and that could conflict with the destroy of views
            eventProcessingRWLock.AcquireWriteLock();
            try
            {
                StartInternal(statementId, desc, isNewStatement);
            }
            finally
            {
                eventProcessingRWLock.ReleaseWriteLock();
            }
        }

        private void StartInternal(String statementId, EPStatementDesc desc, bool isNewStatement)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(".startInternal Starting statement " + statementId + " from desc=" + desc);
            }

            if (desc.StartMethod == null)
            {
                throw new IllegalStateException("Statement start method not found for id " + statementId);
            }

            EPStatementSPI statement = desc.EpStatement;
            if (statement.State == EPStatementState.STARTED)
            {
                log.Debug(".startInternal - Statement already started");
                return;
            }

            Pair<Viewable, EPStatementStopMethod> pair;
            try
            {
                pair = desc.StartMethod.Start(isNewStatement);
            }
            catch (ExprValidationException ex)
            {
                stmtIdToDescMap.Remove(statementId);
                stmtNameToIdMap.Remove(statement.Name);
                stmtNameToStmtMap.Remove(statement.Name);
                log.Debug(".start Error starting view", ex);
                throw new EPStatementException("Error starting view: " + ex.Message, statement.Text, ex);
            }
            catch (ViewProcessingException ex)
            {
                stmtIdToDescMap.Remove(statementId);
                stmtNameToIdMap.Remove(statement.Name);
                stmtNameToStmtMap.Remove(statement.Name);
                log.Debug(".start Error starting view", ex);
                throw new EPStatementException("Error starting view: " + ex.Message, statement.Text, ex);
            }

            Viewable parentView = pair.First;
            EPStatementStopMethod stopMethod = pair.Second;
            desc.StopMethod = stopMethod;
            statement.ParentView = parentView;
            long timeLastStateChange = services.SchedulingService.Time;
            statement.SetCurrentState(EPStatementState.STARTED, timeLastStateChange);

            DispatchStatementLifecycleEvent(new StatementLifecycleEvent(statement, StatementLifecycleEvent.LifecycleEventType.STATECHANGE));
        }

        public void Stop(String statementId)
        {
            using( objLock.Acquire() ) {
                // Acquire a lock for event processing as threads may be in the views used by the statement
                // and that could conflict with the destroy of views
                eventProcessingRWLock.AcquireWriteLock();
                try {
                    EPStatementDesc desc = stmtIdToDescMap.Get(statementId);
                    if (desc == null) {
                        throw new IllegalStateException("Cannot stop statement, statement is in destroyed state");
                    }

                    EPStatementSPI statement = desc.EpStatement;
                    EPStatementStopMethod stopMethod = desc.StopMethod;
                    if (stopMethod == null) {
                        throw new IllegalStateException("Stop method not found for statement " + statementId);
                    }

                    if (statement.State ==
                        EPStatementState.STOPPED) {
                        log.Debug(".startInternal - Statement already stopped");
                        return;
                    }

                    stopMethod.Invoke();
                    statement.ParentView = null;
                    desc.StopMethod = null;

                    long timeLastStateChange = services.SchedulingService.Time;
                    statement.SetCurrentState(EPStatementState.STOPPED, timeLastStateChange);

                    DispatchStatementLifecycleEvent(
                        new StatementLifecycleEvent(statement, StatementLifecycleEvent.LifecycleEventType.STATECHANGE));
                } finally {
                    eventProcessingRWLock.ReleaseWriteLock();
                }
            }
        }

        public void Destroy(String statementId)
        {
            using( objLock.Acquire() ) {
                // Acquire a lock for event processing as threads may be in the views used by the statement
                // and that could conflict with the destroy of views
                eventProcessingRWLock.AcquireWriteLock();
                try {
                    EPStatementDesc desc = stmtIdToDescMap.Get(statementId);
                    if (desc == null) {
                        log.Debug(".startInternal - Statement already destroyed");
                        return;
                    }

                    EPStatementSPI statement = desc.EpStatement;
                    if (statement.State ==
                        EPStatementState.STARTED) {
                        EPStatementStopMethod stopMethod = desc.StopMethod;
                        statement.ParentView = null;
                        stopMethod.Invoke();
                    }

                    long timeLastStateChange = services.SchedulingService.Time;
                    statement.SetCurrentState(EPStatementState.DESTROYED, timeLastStateChange);

                    stmtNameToStmtMap.Remove(statement.Name);
                    stmtNameToIdMap.Remove(statement.Name);
                    stmtIdToDescMap.Remove(statementId);

                    DispatchStatementLifecycleEvent(
                        new StatementLifecycleEvent(statement, StatementLifecycleEvent.LifecycleEventType.STATECHANGE));
                } finally {
                    eventProcessingRWLock.ReleaseWriteLock();
                }
            }
        }

        public EPStatement GetStatementByName(String name)
        {
            using( objLock.Acquire() ) {
                return stmtNameToStmtMap.Get(name);
            }
        }

        /// <summary>Returns the statement given a statement id. </summary>
        /// <param name="id">is the statement id</param>
        /// <returns>statement</returns>
        public EPStatementSPI GetStatementById(String id)
        {
            return this.stmtIdToDescMap.Get(id).EpStatement;
        }

        /// <summary>
        /// Returns an array of statement names. If no statement has been created, an empty array is returned.
        /// <para>
        /// Only returns started and stopped statements.
        /// </para>
        /// </summary>
        /// <value>The statement names.</value>
        /// <returns>statement names</returns>
        public IList<string> StatementNames
        {
            get
            {
                using (objLock.Acquire()) {
                    String[] statements = new String[stmtNameToStmtMap.Count];
                    int count = 0;
                    foreach (String key in stmtNameToStmtMap.Keys) {
                        statements[count++] = key;
                    }
                    return statements;
                }
            }
        }

        /// <summary>
        /// Starts all stopped statements. First statement to fail supplies the exception.
        /// </summary>
        /// <throws>EPException to indicate a start error.</throws>
        public void StartAllStatements()
        {
            using( objLock.Acquire() ) {
                String[] statementIds = GetStatementIds();
                for (int i = 0; i < statementIds.Length; i++) {
                    EPStatement statement = stmtIdToDescMap.Get(statementIds[i]).EpStatement;
                    if (statement.State ==
                        EPStatementState.STOPPED) {
                        Start(statementIds[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Stops all started statements. First statement to fail supplies the exception.
        /// </summary>
        /// <throws>EPException to indicate a start error.</throws>
        public void StopAllStatements()
        {
            using( objLock.Acquire() ) {
                String[] statementIds = GetStatementIds();
                for (int i = 0; i < statementIds.Length; i++) {
                    EPStatement statement = stmtIdToDescMap.Get(statementIds[i]).EpStatement;
                    if (statement.State ==
                        EPStatementState.STARTED) {
                        Stop(statementIds[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Destroys all started statements. First statement to fail supplies the exception.
        /// </summary>
        /// <throws>EPException to indicate a start error.</throws>
        public void DestroyAllStatements()
        {
            using( objLock.Acquire() ) {
                String[] statementIds = GetStatementIds();
                for (int i = 0; i < statementIds.Length; i++) {
                    Destroy(statementIds[i]);
                }
            }
        }

        private String[] GetStatementIds()
        {
            String[] statementIds = new String[stmtNameToIdMap.Count];
            int count = 0;
            foreach (String id in stmtNameToIdMap.Values)
            {
                statementIds[count++] = id;
            }
            return statementIds;
        }

        private String GetUniqueStatementName(String statementName, String statementId)
        {
            String finalStatementName;

            if (stmtNameToIdMap.ContainsKey(statementName))
            {
                int count = 0;
                while(true)
                {
                    finalStatementName = statementName + "--" + count;
                    if (!(stmtNameToIdMap.ContainsKey(finalStatementName)))
                    {
                        break;
                    }
                    if (count > Int32.MaxValue - 2)
                    {
                        throw new EPException("Failed to establish a unique statement name");
                    }
                    count++;
                }
            }
            else
            {
                finalStatementName = statementName;
            }

            stmtNameToIdMap.Put(finalStatementName, statementId);
            return finalStatementName;
        }

        public void UpdatedListeners(String statementId, String statementName, EPStatementListenerSet listeners)
        {
            log.Debug(".updatedListeners No action for base implementation");
        }

        /// <summary>Statement information. </summary>
        public class EPStatementDesc
        {
            private readonly EPStatementSPI epStatement;
            private readonly EPStatementStartMethod startMethod;
            private EPStatementStopMethod stopMethod;
            private readonly String optInsertIntoStream;
            private readonly EPStatementHandle statementHandle;

            /// <summary>Ctor. </summary>
            /// <param name="epStatement">the statement</param>
            /// <param name="startMethod">the start method</param>
            /// <param name="stopMethod">the stop method</param>
            /// <param name="optInsertIntoStream">is the insert-into stream name, or null if not using insert-into</param>
            /// <param name="statementHandle">is the locking handle for the statement</param>
            public EPStatementDesc(EPStatementSPI epStatement, EPStatementStartMethod startMethod, EPStatementStopMethod stopMethod, String optInsertIntoStream, EPStatementHandle statementHandle)
            {
                this.epStatement = epStatement;
                this.startMethod = startMethod;
                this.stopMethod = stopMethod;
                this.optInsertIntoStream = optInsertIntoStream;
                this.statementHandle = statementHandle;
            }

            /// <summary>Returns the statement. </summary>
            /// <returns>statement.</returns>
            public EPStatementSPI EpStatement
            {
                get { return epStatement; }
            }

            /// <summary>Returns the start method. </summary>
            /// <returns>start method</returns>
            public EPStatementStartMethod StartMethod
            {
                get { return startMethod; }
            }

            /// <summary>Returns the stop method. </summary>
            /// <returns>stop method</returns>
            public EPStatementStopMethod StopMethod
            {
                get { return stopMethod; }
                set { this.stopMethod = value; }
            }

            /// <summary>Return the insert-into stream name, or null if no insert-into </summary>
            /// <returns>stream name</returns>
            public string OptInsertIntoStream
            {
                get { return optInsertIntoStream; }
            }

            /// <summary>Returns the statements handle. </summary>
            /// <returns>statement handle</returns>
            public EPStatementHandle StatementHandle
            {
                get { return statementHandle; }
            }
        }

        /// <summary>Compiles a statement returning the compile (verified, non-serializable) form of a statement.</summary>
        /// <param name="spec">is the statement specification</param>
        /// <param name="eplStatement">the statement to compile</param>
        /// <param name="statementContext">the statement services</param>
        /// <returns>compiled statement</returns>
        /// <throws>EPStatementException if the statement cannot be compiled</throws>
        protected internal static StatementSpecCompiled Compile(StatementSpecRaw spec, String eplStatement, StatementContext statementContext)
        {
            List<StreamSpecCompiled> compiledStreams;

            try
            {
                compiledStreams = new List<StreamSpecCompiled>();
                foreach (StreamSpecRaw rawSpec in spec.StreamSpecs)
                {
                    StreamSpecCompiled compiled =
                        rawSpec.Compile(statementContext.EventAdapterService,
                                        statementContext.MethodResolutionService,
                                        statementContext.PatternResolutionService,
                                        statementContext.SchedulingService,
                                        statementContext.NamedWindowService,
                                        statementContext.ValueAddEventService,
                                        statementContext.VariableService,
                                        statementContext.EngineURI,
                                        statementContext.PlugInTypeResolutionURIs);
                    compiledStreams.Add(compiled);
                }
            }
            catch (ExprValidationException ex)
            {
                throw new EPStatementException(ex.Message, eplStatement);
            }
            catch (Exception ex)
            {
                String text = "Unexpected error compiling statement";
                log.Error(".compile " + text, ex);
                throw new EPStatementException(text + ":" + ex.GetType().FullName + ":" + ex.Message, eplStatement);
            }

            // for create window statements, we switch the filter to a new event type
            if (spec.CreateWindowDesc != null)
            {
                try
                {
                    FilterStreamSpecCompiled filterStreamSpec = (FilterStreamSpecCompiled) compiledStreams[0];
                    EventType selectFromType = filterStreamSpec.FilterSpec.EventType;
                    String selectFromTypeAlias = filterStreamSpec.FilterSpec.EventTypeAlias;
                    Pair<FilterSpecCompiled, SelectClauseSpecRaw> newFilter = HandleCreateWindow(selectFromType, selectFromTypeAlias, spec, eplStatement, statementContext);
                    filterStreamSpec.FilterSpec = newFilter.First;
                    spec.SelectClauseSpec = newFilter.Second;

                    // view must be non-empty list
                    if (CollectionHelper.IsEmpty(spec.CreateWindowDesc.ViewSpecs))
                    {
                        throw new ExprValidationException(NamedWindowServiceConstants.ERROR_MSG_DATAWINDOWS);
                    }

                    CollectionHelper.AddAll(filterStreamSpec.ViewSpecs, spec.CreateWindowDesc.ViewSpecs);
                }
                catch (ExprValidationException e)
                {
                    throw new EPStatementException(e.Message, eplStatement);
                }
            }

            // Look for expressions with sub-selects in select expression list and filter expression
            // Recursively compile the statement within the statement.
            ExprNodeSubselectVisitor visitor = new ExprNodeSubselectVisitor();
            List<SelectClauseElementCompiled> selectElements = new List<SelectClauseElementCompiled>();
            SelectClauseSpecCompiled selectClauseCompiled = new SelectClauseSpecCompiled(selectElements);
            foreach (SelectClauseElementRaw raw in spec.SelectClauseSpec.SelectExprList)
            {
                if (raw is SelectClauseExprRawSpec)
                {
                    SelectClauseExprRawSpec rawExpr = (SelectClauseExprRawSpec) raw;
                    rawExpr.SelectExpression.Accept(visitor);
                    selectElements.Add(new SelectClauseExprCompiledSpec(rawExpr.SelectExpression, rawExpr.OptionalAsName));
                }
                else if (raw is SelectClauseStreamRawSpec)
                {
                    SelectClauseStreamRawSpec rawExpr = (SelectClauseStreamRawSpec) raw;
                    selectElements.Add(new SelectClauseStreamCompiledSpec(rawExpr.StreamAliasName, rawExpr.OptionalAsName));
                }
                else if (raw is SelectClauseElementWildcard)
                {
                    SelectClauseElementWildcard wildcard = (SelectClauseElementWildcard) raw;
                    selectElements.Add(wildcard);
                }
                else
                {
                    throw new IllegalStateException("Unexpected select clause element class : " + raw.GetType().FullName);
                }
            }
            if (spec.FilterRootNode != null)
            {
                spec.FilterRootNode.Accept(visitor);
            }
            foreach (ExprSubselectNode subselect in visitor.Subselects)
            {
                StatementSpecRaw raw = subselect.StatementSpecRaw;
                StatementSpecCompiled compiled = Compile(raw, eplStatement, statementContext);
                subselect.StatementSpecCompiled = compiled;
            }

            return new StatementSpecCompiled(
                    spec.OnTriggerDesc,
                    spec.CreateWindowDesc,
                    spec.CreateVariableDesc,
                    spec.InsertIntoDesc,
                    spec.SelectStreamSelectorEnum,
                    selectClauseCompiled,
                    compiledStreams,
                    spec.OuterJoinDescList,
                    spec.FilterRootNode,
                    spec.GroupByExpressions,
                    spec.HavingExprRootNode,
                    spec.OutputLimitSpec,
                    spec.OrderByList,
                    visitor.Subselects,
                    spec.HasVariables
                    );
        }

        // The create window command:
        //      create window windowName[.window_view_list] as [select properties from] type
        //
        // This section expected s single FilterStreamSpecCompiled representing the selected type.
        // It creates a new event type representing the window type and a sets the type selected on the filter stream spec.
        private static Pair<FilterSpecCompiled, SelectClauseSpecRaw> HandleCreateWindow(
            EventType selectFromType,
            String selectFromTypeAlias,
            StatementSpecRaw spec,
            String eplStatement,
            StatementContext statementContext)
        {
            String typeName = spec.CreateWindowDesc.WindowName;
            EventType targetType;

            // Validate the select expressions which consists of properties only
            List<SelectClauseExprCompiledSpec> select = CompileLimitedSelect(spec.SelectClauseSpec, eplStatement, selectFromType, selectFromTypeAlias, statementContext.EngineURI);

            // Create Map or Wrapper event type from the select clause of the window.
            // If no columns selected, simply create a wrapper type
            // Build a list of properties
            SelectClauseSpecRaw newSelectClauseSpecRaw = new SelectClauseSpecRaw();
            DataMap properties = new HashMap<String, Object>();
            foreach (SelectClauseExprCompiledSpec selectElement in select)
            {
                properties.Put(selectElement.AssignedName, selectElement.SelectExpression.ReturnType);

                // Add any properties to the new select clause for use by consumers to the statement itself
                newSelectClauseSpecRaw.Add(new SelectClauseExprRawSpec(new ExprIdentNode(selectElement.AssignedName), null));
            }

            // Create Map or Wrapper event type from the select clause of the window.
            // If no columns selected, simply create a wrapper type
            bool isWildcard = spec.SelectClauseSpec.IsUsingWildcard;
            if (statementContext.ValueAddEventService.IsRevisionTypeAlias(selectFromTypeAlias))
            {
                targetType = statementContext.ValueAddEventService.CreateRevisionType(typeName, selectFromTypeAlias, statementContext.StatementStopService, statementContext.EventAdapterService);
            }
            else if (isWildcard)
            {
                targetType = statementContext.EventAdapterService.AddWrapperType(typeName, selectFromType, properties);
            }
            else
            {
                // Some columns selected, use the types of the columns
                if (spec.SelectClauseSpec.SelectExprList.Count > 0)
                {
                    targetType = statementContext.EventAdapterService.AddNestableMapType(typeName, properties);
                }
                else
                {
                    // No columns selected, no wildcard, use the type as is or as a wrapped type
                    if (selectFromType is MapEventType)
                    {
                        MapEventType mapType = (MapEventType) selectFromType;
                        targetType = statementContext.EventAdapterService.AddNestableMapType(typeName, mapType.Types);
                    }
                    else
                    {
                        DataMap addOnTypes = new HashMap<String, Object>();
                        targetType = statementContext.EventAdapterService.AddWrapperType(typeName, selectFromType, addOnTypes);
                    }
                }
            }

            FilterSpecCompiled filter = new FilterSpecCompiled(targetType, typeName, new List<FilterSpecParam>());
            return new Pair<FilterSpecCompiled, SelectClauseSpecRaw>(filter, newSelectClauseSpecRaw);
        }

        private static List<SelectClauseExprCompiledSpec> CompileLimitedSelect(SelectClauseSpecRaw spec, String eplStatement, EventType singleType, String selectFromTypeAlias, String engineURI)
        {
            List<SelectClauseExprCompiledSpec> selectProps = new List<SelectClauseExprCompiledSpec>();
            StreamTypeService streams = new StreamTypeServiceImpl(new EventType[] { singleType }, new String[] { "stream_0" }, engineURI, new String[] { selectFromTypeAlias });

            foreach (SelectClauseElementRaw raw in spec.SelectExprList)
            {
                if (!(raw is SelectClauseExprRawSpec))
                {
                    continue;
                }
                SelectClauseExprRawSpec exprSpec = (SelectClauseExprRawSpec) raw;
                ExprNode validatedExpression;
                try
                {
                    validatedExpression = exprSpec.SelectExpression.GetValidatedSubtree(streams, null, null, null, null);
                }
                catch (ExprValidationException e)
                {
                    throw new EPStatementException(e.Message, eplStatement);
                }

                // determine an element name if none assigned
                String asName = exprSpec.OptionalAsName;
                if (asName == null)
                {
                    asName = validatedExpression.ExpressionString;
                }

                SelectClauseExprCompiledSpec validatedElement = new SelectClauseExprCompiledSpec(validatedExpression, asName);
                selectProps.Add(validatedElement);
            }

            return selectProps;
        }

        public void DispatchStatementLifecycleEvent(StatementLifecycleEvent @event)
        {
            foreach (StatementLifecycleObserver observer in observers)
            {
                observer.Observe(@event);
            }
        }
    }
}
