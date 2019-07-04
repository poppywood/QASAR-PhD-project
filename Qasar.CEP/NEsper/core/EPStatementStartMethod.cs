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
using com.espertech.esper.epl.agg;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.db;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.join;
using com.espertech.esper.epl.join.plan;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.epl.lookup;
using com.espertech.esper.epl.named;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.subquery;
using com.espertech.esper.epl.variable;
using com.espertech.esper.epl.view;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.pattern;
using com.espertech.esper.util;
using com.espertech.esper.view;
using com.espertech.esper.view.internals;
using log4net;

namespace com.espertech.esper.core
{
    /// <summary>Starts and provides the stop method for EPL statements. </summary>
    public class EPStatementStartMethod
    {
        private readonly StatementSpecCompiled statementSpec;
        private readonly EPServicesContext services;
        private readonly StatementContext statementContext;

        /// <summary>Ctor. </summary>
        /// <param name="statementSpec">is a container for the definition of all statement constructs thatmay have been used in the statement, i.e. if defines the select clauses, insert into, outer joins etc. </param>
        /// <param name="services">is the service instances for dependency injection</param>
        /// <param name="statementContext">is statement-level information and statement services</param>
        public EPStatementStartMethod(StatementSpecCompiled statementSpec,
                                    EPServicesContext services,
                                    StatementContext statementContext)
        {
            this.statementSpec = statementSpec;
            this.services = services;
            this.statementContext = statementContext;
        }

        /// <summary>Starts the EPL statement. </summary>
        /// <returns>a viewable to attach to for listening to events, and a stop method to invoke to clean up</returns>
        /// <param name="isNewStatement">indicator whether the statement is new or a stop-restart statement</param>
        /// <throws>ExprValidationException when the expression validation fails</throws>
        /// <throws>ViewProcessingException when views cannot be started</throws>
        public Pair<Viewable, EPStatementStopMethod> Start(bool isNewStatement)
        {
            statementContext.VariableService.SetLocalVersion();    // get current version of variables

            if (statementSpec.OnTriggerDesc != null)
            {
                return StartOnTrigger();
            }
            else if (statementSpec.CreateWindowDesc != null)
            {
                return StartCreateWindow();
            }
            else if (statementSpec.CreateVariableDesc != null)
            {
                return StartCreateVariable(isNewStatement);
            }
            else
            {
                return StartSelect();
            }
        }

        private Pair<Viewable, EPStatementStopMethod> StartOnTrigger()
        {
            List<StopCallback> stopCallbacks = new List<StopCallback>();

            // Create streams
            Viewable eventStreamParentViewable;
            StreamSpecCompiled streamSpec = statementSpec.StreamSpecs[0];
            String triggerEventTypeAlias = null;
            if (streamSpec is FilterStreamSpecCompiled)
            {
                FilterStreamSpecCompiled filterStreamSpec = (FilterStreamSpecCompiled) streamSpec;
                triggerEventTypeAlias = filterStreamSpec.FilterSpec.EventTypeAlias;

                // Since only for non-joins we get the existing stream's lock and try to reuse it's views
                Pair<EventStream, ManagedLock> streamLockPair = services.StreamService.CreateStream(filterStreamSpec.FilterSpec,
                        services.FilterService, statementContext.EpStatementHandle, false);
                eventStreamParentViewable = streamLockPair.First;

                // Use the re-used stream's lock for all this statement's locking needs
                if (streamLockPair.Second != null)
                {
                    statementContext.EpStatementHandle.StatementLock = streamLockPair.Second;
                }
            }
            else if (streamSpec is PatternStreamSpecCompiled)
            {
                PatternStreamSpecCompiled patternStreamSpec = (PatternStreamSpecCompiled) streamSpec;
                EventType eventType = services.EventAdapterService.CreateAnonymousCompositeType(patternStreamSpec.TaggedEventTypes);
                EventStream sourceEventStream = new ZeroDepthStream(eventType);
                eventStreamParentViewable = sourceEventStream;

                EvalRootNode rootNode = new EvalRootNode();
                rootNode.AddChildNode(patternStreamSpec.EvalNode);

                PatternMatchCallback callback = new ProxyPatternMatchCallback(
                    delegate(Map<String, EventBean> matchEvent) {
                        EventBean compositeEvent =
                            statementContext.EventAdapterService.AdapterForCompositeEvent(eventType, matchEvent);
                        sourceEventStream.Insert(compositeEvent);
                    });

                PatternContext patternContext = statementContext.PatternContextFactory.CreateContext(statementContext, 0, rootNode);
                PatternStopCallback patternStopCallback = rootNode.Start(callback, patternContext);
                stopCallbacks.Add(patternStopCallback);
            }
            else if (streamSpec is NamedWindowConsumerStreamSpec)
            {
                NamedWindowConsumerStreamSpec namedSpec = (NamedWindowConsumerStreamSpec) streamSpec;
                NamedWindowProcessor processor = services.NamedWindowService.GetProcessor(namedSpec.WindowName);
                eventStreamParentViewable = processor.AddConsumer(namedSpec.FilterExpressions, statementContext.EpStatementHandle, statementContext.StatementStopService);
                triggerEventTypeAlias = namedSpec.WindowName;
            }
            else
            {
                throw new ExprValidationException("Unknown stream specification type: " + streamSpec);
            }

            // create stop method using statement stream specs
            EPStatementStopMethod stopMethod =
                delegate {
                    statementContext.StatementStopService.FireStatementStopped();

                    if (streamSpec is FilterStreamSpecCompiled) {
                        FilterStreamSpecCompiled filterStreamSpec = (FilterStreamSpecCompiled) streamSpec;
                        services.StreamService.DropStream(filterStreamSpec.FilterSpec, services.FilterService, false);
                    }
                    foreach (StopCallback stopCallback in stopCallbacks) {
                        stopCallback.Stop();
                    }
                };

            View onExprView;
            EventType streamEventType = eventStreamParentViewable.EventType;

            // For on-delete and on-select triggers
            if (statementSpec.OnTriggerDesc is OnTriggerWindowDesc)
            {
                // Determine event types
                OnTriggerWindowDesc onTriggerDesc = (OnTriggerWindowDesc) statementSpec.OnTriggerDesc;
                NamedWindowProcessor processor = services.NamedWindowService.GetProcessor(onTriggerDesc.WindowName);
                EventType namedWindowType = processor.NamedWindowType;

                String namedWindowAlias = onTriggerDesc.OptionalAsName;
                if (namedWindowAlias == null)
                {
                    namedWindowAlias = "stream_0";
                }
                String streamAlias = streamSpec.OptionalStreamName;
                if (streamAlias == null)
                {
                    streamAlias = "stream_1";
                }

                String namedWindowTypeAlias = onTriggerDesc.WindowName;

                StreamTypeService typeService =
                    new StreamTypeServiceImpl(new EventType[] {namedWindowType, streamEventType},
                                              new String[] {namedWindowAlias, streamAlias},
                                              services.EngineURI,
                                              new String[] {onTriggerDesc.WindowName, triggerEventTypeAlias});

                // validate join expression
                ExprNode validatedJoin = ValidateJoinNamedWindow(
                    statementSpec.FilterRootNode,
                    namedWindowType,
                    namedWindowAlias,
                    namedWindowTypeAlias,
                    streamEventType,
                    streamAlias,
                    triggerEventTypeAlias);

                // Construct a processor for results; for use in on-select to process selection results
                // Use a wildcard select if the select-clause is empty, such as for on-delete.
                // For on-select the select clause is not empty.
                if (statementSpec.SelectClauseSpec.SelectExprList.Count == 0)
                {
                    statementSpec.SelectClauseSpec.Add(new SelectClauseElementWildcard());
                }
                ResultSetProcessor resultSetProcessor = ResultSetProcessorFactory.GetProcessor(
                    statementSpec, statementContext, typeService, null);

                InternalEventRouter routerService = (statementSpec.InsertIntoDesc == null)?  null : services.InternalEventRouter;
                onExprView =
                    processor.AddOnExpr(onTriggerDesc,
                                        validatedJoin,
                                        streamEventType,
                                        statementContext.StatementStopService,
                                        routerService,
                                        resultSetProcessor,
                                        statementContext.EpStatementHandle,
                                        statementContext.StatementResultService);
                eventStreamParentViewable.AddView(onExprView);
            }
            else
            {
                OnTriggerSetDesc desc = (OnTriggerSetDesc) statementSpec.OnTriggerDesc;
                StreamTypeService typeService =
                    new StreamTypeServiceImpl(new EventType[] {streamEventType},
                                              new String[] {streamSpec.OptionalStreamName},
                                              services.EngineURI,
                                              new String[] {triggerEventTypeAlias});

                foreach (OnTriggerSetAssignment assignment in desc.Assignments)
                {
                    ExprNode validated =
                        assignment.Expression.GetValidatedSubtree(typeService,
                                                                  statementContext.MethodResolutionService,
                                                                  null,
                                                                  statementContext.SchedulingService,
                                                                  statementContext.VariableService);
                    assignment.Expression = validated;
                }

                onExprView =
                    new OnSetVariableView(desc,
                                          statementContext.EventAdapterService,
                                          statementContext.VariableService,
                                          statementContext.StatementResultService);
                eventStreamParentViewable.AddView(onExprView);
            }

            // For on-delete, create an output processor that passes on as a wildcard the underlying event
            if ((statementSpec.OnTriggerDesc.OnTriggerType == OnTriggerType.ON_DELETE) ||
                (statementSpec.OnTriggerDesc.OnTriggerType == OnTriggerType.ON_SET))
            {
                StatementSpecCompiled defaultSelectAllSpec = new StatementSpecCompiled();
                defaultSelectAllSpec.SelectClauseSpec.Add(new SelectClauseElementWildcard());

                StreamTypeService streamTypeService =
                    new StreamTypeServiceImpl(new EventType[] {onExprView.EventType},
                                              new String[] {"trigger_stream"},
                                              services.EngineURI,
                                              new String[] {triggerEventTypeAlias});
                ResultSetProcessor outputResultSetProcessor = ResultSetProcessorFactory.GetProcessor(
                        defaultSelectAllSpec, statementContext, streamTypeService, null);

                // Attach output view
                OutputProcessView outputView =
                    OutputProcessViewFactory.MakeView(outputResultSetProcessor,
                                                      defaultSelectAllSpec,
                                                      statementContext,
                                                      services.InternalEventRouter);
                onExprView.AddView(outputView);
                onExprView = outputView;
            }

            log.Debug(".start Statement start completed");

            return new Pair<Viewable, EPStatementStopMethod>(onExprView, stopMethod);
        }

        private Pair<Viewable, EPStatementStopMethod> StartCreateWindow()
        {
            FilterStreamSpecCompiled filterStreamSpec = (FilterStreamSpecCompiled) statementSpec.StreamSpecs[0];
            String windowName = statementSpec.CreateWindowDesc.WindowName;
            EventType windowType = filterStreamSpec.FilterSpec.EventType;

            ValueAddEventProcessor revisionProcessor = statementContext.ValueAddEventService.GetValueAddProcessor(windowName);
            services.NamedWindowService.AddProcessor(windowName, windowType, statementContext.EpStatementHandle, statementContext.StatementResultService, revisionProcessor);

            // Create streams and views
            Viewable eventStreamParentViewable;
            ViewFactoryChain unmaterializedViewChain;

            // Create view factories and parent view based on a filter specification
            // Since only for non-joins we get the existing stream's lock and try to reuse it's views
            Pair<EventStream, ManagedLock> streamLockPair = services.StreamService.CreateStream(filterStreamSpec.FilterSpec,
                    services.FilterService, statementContext.EpStatementHandle, false);
            eventStreamParentViewable = streamLockPair.First;

            // Use the re-used stream's lock for all this statement's locking needs
            if (streamLockPair.Second != null)
            {
                statementContext.EpStatementHandle.StatementLock = streamLockPair.Second;
            }

            // Create data window view factories
            unmaterializedViewChain = services.ViewService.CreateFactories(0, eventStreamParentViewable.EventType, filterStreamSpec.ViewSpecs, statementContext);

            // The root view of the named window
            NamedWindowProcessor processor = services.NamedWindowService.GetProcessor(statementSpec.CreateWindowDesc.WindowName);
            View rootView = processor.RootView;
            eventStreamParentViewable.AddView(rootView);

            // request remove stream capability from views
            ViewResourceDelegate viewResourceDelegate = new ViewResourceDelegateImpl(new ViewFactoryChain[] {unmaterializedViewChain}, statementContext);
            if (!viewResourceDelegate.RequestCapability(0, new RemoveStreamViewCapability(), null))
            {
                throw new ExprValidationException(NamedWindowServiceConstants.ERROR_MSG_DATAWINDOWS);
            }

            // create stop method using statement stream specs
            EPStatementStopMethod stopMethod =
                delegate {
                    statementContext.StatementStopService.FireStatementStopped();
                    services.StreamService.DropStream(filterStreamSpec.FilterSpec, services.FilterService, false);
                    String _windowName = statementSpec.CreateWindowDesc.WindowName;
                    services.NamedWindowService.RemoveProcessor(_windowName);
                };

            // Materialize views
            Viewable finalView =
                services.ViewService.CreateViews(rootView, unmaterializedViewChain.FactoryChain, statementContext);

            // Attach tail view
            NamedWindowTailView tailView = processor.TailView;
            finalView.AddView(tailView);
            finalView = tailView;

            // Add a wildcard to the select clause as subscribers received the window contents
            statementSpec.SelectClauseSpec.SelectExprList.Clear();
            statementSpec.SelectClauseSpec.Add(new SelectClauseElementWildcard());
            statementSpec.SelectStreamDirEnum = SelectClauseStreamSelectorEnum.RSTREAM_ISTREAM_BOTH;

            StreamTypeService typeService =
                new StreamTypeServiceImpl(new EventType[] {windowType},
                                          new String[] {windowName},
                                          services.EngineURI,
                                          new String[] {windowName});
            ResultSetProcessor resultSetProcessor = ResultSetProcessorFactory.GetProcessor(
                    statementSpec, statementContext, typeService, null);

            // Attach output view
            OutputProcessView outputView = OutputProcessViewFactory.MakeView(resultSetProcessor, statementSpec, statementContext, services.InternalEventRouter);
            finalView.AddView(outputView);
            finalView = outputView;

            log.Debug(".start Statement start completed");

            return new Pair<Viewable, EPStatementStopMethod>(finalView, stopMethod);
        }

        private Pair<Viewable, EPStatementStopMethod> StartCreateVariable(bool isNewStatement)
        {
            CreateVariableDesc createDesc = statementSpec.CreateVariableDesc;

            // Determime the variable type
            Type type;
            try
            {
                type = TypeHelper.GetTypeForSimpleName(createDesc.VariableType);
            }
            catch (Exception)
            {
                throw new ExprValidationException("Cannot create variable '" + createDesc.VariableName + "', type '" +
                    createDesc.VariableType + "' is not a recognized type");
            }

            // Get assignment value
            Object value = null;
            StreamTypeService typeService;
            if (createDesc.Assignment != null)
            {
                // Evaluate assignment expression
                typeService = new StreamTypeServiceImpl(new EventType[0], new String[0], services.EngineURI, new String[0]);
                ExprNode validated = createDesc.Assignment.GetValidatedSubtree(typeService, statementContext.MethodResolutionService, null, statementContext.SchedulingService, statementContext.VariableService);
                value = validated.Evaluate(null, true);
            }

            // Create variable
            try
            {
                services.VariableService.CreateNewVariable(createDesc.VariableName, type, value, statementContext.ExtensionServicesContext);
            }
            catch (VariableExistsException ex)
            {
                // for new statement we don't allow creating the same variable
                if (isNewStatement)
                {
                    throw new ExprValidationException("Cannot create variable: " + ex.Message);
                }

                // compare the type
                if (services.VariableService.GetReader(createDesc.VariableName).VariableType != type)
                {
                    throw new ExprValidationException("Cannot create variable: " + ex.Message);
                }
            }
            catch (VariableDeclarationException ex)
            {
                throw new ExprValidationException("Cannot create variable: " + ex.Message);
            }

            CreateVariableView createView =
                new CreateVariableView(services.EventAdapterService,
                                       services.VariableService,
                                       createDesc.VariableName,
                                       statementContext.StatementResultService);
            services.VariableService.RegisterCallback(services.VariableService.GetReader(createDesc.VariableName).VariableNumber, createView);

            // Create result set processor, use wildcard selection
            statementSpec.SelectClauseSpec.SelectExprList.Clear();
            statementSpec.SelectClauseSpec.Add(new SelectClauseElementWildcard());
            statementSpec.SelectStreamDirEnum = SelectClauseStreamSelectorEnum.RSTREAM_ISTREAM_BOTH;
            typeService = new StreamTypeServiceImpl(new EventType[] { createView.EventType }, new String[] { "create_variable" }, services.EngineURI, new String[] { "create_variable" });
            ResultSetProcessor resultSetProcessor = ResultSetProcessorFactory.GetProcessor(
                    statementSpec, statementContext, typeService, null);

            // Attach output view
            OutputProcessView outputView =
                OutputProcessViewFactory.MakeView(resultSetProcessor,
                                                  statementSpec,
                                                  statementContext,
                                                  services.InternalEventRouter);
            createView.AddView(outputView);

            return new Pair<Viewable, EPStatementStopMethod>(outputView, delegate {});
        }

        private Pair<Viewable, EPStatementStopMethod> StartSelect()
        {
            // Determine stream names for each stream - some streams may not have a name given
            String[] streamNames = DetermineStreamNames(statementSpec.StreamSpecs);
            bool isJoin = statementSpec.StreamSpecs.Count > 1;

            // First we create streams for subselects, if there are any
            SubSelectStreamCollection subSelectStreamDesc = CreateSubSelectStreams(isJoin);

            int numStreams = streamNames.Length;
            List<StopCallback> stopCallbacks = new List<StopCallback>();

            // Create streams and views
            Viewable[] eventStreamParentViewable = new Viewable[numStreams];
            ViewFactoryChain[] unmaterializedViewChain = new ViewFactoryChain[numStreams];
            bool[] isUnidirectional = new bool[numStreams];
            bool[] hasChildViews = new bool[numStreams];
            bool[] isNamedWindow = new bool[numStreams];
            String[] eventTypeAliases = new String[numStreams];
            ViewResourceDelegate viewResourceDelegate;
            for (int i = 0; i < statementSpec.StreamSpecs.Count; i++)
            {
                StreamSpecCompiled streamSpec = statementSpec.StreamSpecs[i];
                isUnidirectional[i] = streamSpec.IsUnidirectional;
                hasChildViews[i] = CollectionHelper.IsNotEmpty(streamSpec.ViewSpecs);

                // Create view factories and parent view based on a filter specification
                if (streamSpec is FilterStreamSpecCompiled)
                {
                    FilterStreamSpecCompiled filterStreamSpec = (FilterStreamSpecCompiled) streamSpec;
                    eventTypeAliases[i] = filterStreamSpec.FilterSpec.EventTypeAlias;

                    // Since only for non-joins we get the existing stream's lock and try to reuse it's views
                    Pair<EventStream, ManagedLock> streamLockPair = services.StreamService.CreateStream(filterStreamSpec.FilterSpec,
                            services.FilterService, statementContext.EpStatementHandle, isJoin);
                    eventStreamParentViewable[i] = streamLockPair.First;

                    // Use the re-used stream's lock for all this statement's locking needs
                    if (streamLockPair.Second != null)
                    {
                        statementContext.EpStatementHandle.StatementLock = streamLockPair.Second;
                    }

                    unmaterializedViewChain[i] = services.ViewService.CreateFactories(i, eventStreamParentViewable[i].EventType, streamSpec.ViewSpecs, statementContext);
                }
                // Create view factories and parent view based on a pattern expression
                else if (streamSpec is PatternStreamSpecCompiled)
                {
                    PatternStreamSpecCompiled patternStreamSpec = (PatternStreamSpecCompiled) streamSpec;
                    EventType eventType = services.EventAdapterService.CreateAnonymousCompositeType(patternStreamSpec.TaggedEventTypes);
                    EventStream sourceEventStream = new ZeroDepthStream(eventType);
                    eventStreamParentViewable[i] = sourceEventStream;
                    unmaterializedViewChain[i] = services.ViewService.CreateFactories(i, sourceEventStream.EventType, streamSpec.ViewSpecs, statementContext);

                    EvalRootNode rootNode = new EvalRootNode();
                    rootNode.AddChildNode(patternStreamSpec.EvalNode);

                    PatternMatchCallback callback = new ProxyPatternMatchCallback(
                        delegate(Map<String, EventBean> matchEvent) {
                            EventBean compositeEvent =
                                statementContext.EventAdapterService.AdapterForCompositeEvent(eventType, matchEvent);
                            sourceEventStream.Insert(compositeEvent);
                        });

                    PatternContext patternContext =
                        statementContext.PatternContextFactory.CreateContext(statementContext,
                                                                             i,
                                                                             rootNode);
                    PatternStopCallback patternStopCallback = rootNode.Start(callback, patternContext);
                    stopCallbacks.Add(patternStopCallback);
                }
                // Create view factories and parent view based on a database SQL statement
                else if (streamSpec is DBStatementStreamSpec)
                {
                    if (CollectionHelper.IsNotEmpty(streamSpec.ViewSpecs))
                    {
                        throw new ExprValidationException("Historical data joins do not allow views onto the data, view '" +
                                                          streamSpec.ViewSpecs[0].ObjectNamespace + ':' +
                                                          streamSpec.ViewSpecs[0].ObjectName +
                                                          "' is not valid in this context");
                    }

                    DBStatementStreamSpec sqlStreamSpec = (DBStatementStreamSpec) streamSpec;
                    HistoricalEventViewable historicalEventViewable = DatabasePollingViewableFactory.CreateDBStatementView(i, sqlStreamSpec, services.DatabaseRefService, services.EventAdapterService, statementContext.EpStatementHandle);
                    unmaterializedViewChain[i] = new ViewFactoryChain(historicalEventViewable.EventType, new List<ViewFactory>());
                    eventStreamParentViewable[i] = historicalEventViewable;
                    stopCallbacks.Add(historicalEventViewable);
                }
                else if (streamSpec is MethodStreamSpec)
                {
                    if (CollectionHelper.IsNotEmpty(streamSpec.ViewSpecs))
                    {
                        throw new ExprValidationException("Method data joins do not allow views onto the data, view '"
                                + streamSpec.ViewSpecs[0].ObjectNamespace + ':' + streamSpec.ViewSpecs[0].ObjectName + "' is not valid in this context");
                    }

                    MethodStreamSpec methodStreamSpec = (MethodStreamSpec) streamSpec;
                    HistoricalEventViewable historicalEventViewable = MethodPollingViewableFactory.CreatePollMethodView(i, methodStreamSpec, services.EventAdapterService, statementContext.EpStatementHandle, statementContext.MethodResolutionService, services.EngineImportService, services.SchedulingService, statementContext.ScheduleBucket);
                    unmaterializedViewChain[i] = new ViewFactoryChain(historicalEventViewable.EventType, new List<ViewFactory>());
                    eventStreamParentViewable[i] = historicalEventViewable;
                    stopCallbacks.Add(historicalEventViewable);
                }
                else if (streamSpec is NamedWindowConsumerStreamSpec)
                {
                    NamedWindowConsumerStreamSpec namedSpec = (NamedWindowConsumerStreamSpec) streamSpec;
                    NamedWindowProcessor processor = services.NamedWindowService.GetProcessor(namedSpec.WindowName);
                    NamedWindowConsumerView consumerView = processor.AddConsumer(namedSpec.FilterExpressions, statementContext.EpStatementHandle, statementContext.StatementStopService);
                    eventStreamParentViewable[i] = consumerView;
                    unmaterializedViewChain[i] = services.ViewService.CreateFactories(i, consumerView.EventType, namedSpec.ViewSpecs, statementContext);
                    isNamedWindow[i] = true;
                    eventTypeAliases[i] = namedSpec.WindowName;

                    // Consumers to named windows cannot declare a data window view onto the named window to avoid duplicate remove streams
                    viewResourceDelegate = new ViewResourceDelegateImpl(unmaterializedViewChain, statementContext);
                    viewResourceDelegate.RequestCapability(i, new NotADataWindowViewCapability(), null);
                }
                else
                {
                    throw new ExprValidationException("Unknown stream specification type: " + streamSpec);
                }
            }

            // Obtain event types from ViewFactoryChains
            EventType[] streamEventTypes = new EventType[statementSpec.StreamSpecs.Count];
            for (int i = 0; i < unmaterializedViewChain.Length; i++)
            {
                streamEventTypes[i] = unmaterializedViewChain[i].EventType;
            }

            // Materialize sub-select views
            StartSubSelect(subSelectStreamDesc, streamNames, streamEventTypes, eventTypeAliases, stopCallbacks);

            // List of statement streams
            List<StreamSpecCompiled> statementStreamSpecs = new List<StreamSpecCompiled>();
            statementStreamSpecs.AddRange(statementSpec.StreamSpecs);

            // Construct type information per stream
            StreamTypeService typeService = new StreamTypeServiceImpl(streamEventTypes, streamNames, services.EngineURI, eventTypeAliases);
            viewResourceDelegate = new ViewResourceDelegateImpl(unmaterializedViewChain, statementContext);

            // create stop method using statement stream specs
            EPStatementStopMethod stopMethod =
                delegate {
                    statementContext.StatementStopService.FireStatementStopped();

                    foreach (StreamSpecCompiled streamSpec in statementStreamSpecs) {
                        if (streamSpec is FilterStreamSpecCompiled) {
                            FilterStreamSpecCompiled filterStreamSpec = (FilterStreamSpecCompiled) streamSpec;
                            services.StreamService.DropStream(filterStreamSpec.FilterSpec,
                                                              services.FilterService,
                                                              isJoin);
                        }
                    }
                    foreach (StopCallback stopCallback in stopCallbacks) {
                        stopCallback.Stop();
                    }
                    foreach (ExprSubselectNode subselect in statementSpec.SubSelectExpressions) {
                        StreamSpecCompiled subqueryStreamSpec = subselect.StatementSpecCompiled.StreamSpecs[0];
                        if (subqueryStreamSpec is FilterStreamSpecCompiled) {
                            FilterStreamSpecCompiled filterStreamSpec =
                                (FilterStreamSpecCompiled) subselect.StatementSpecCompiled.StreamSpecs[0];
                            services.StreamService.DropStream(filterStreamSpec.FilterSpec,
                                                              services.FilterService,
                                                              isJoin);
                        }
                    }
                };

            // Validate views that require validation, specifically streams that don't have
            // sub-views such as DB SQL joins
            foreach (Viewable viewable in eventStreamParentViewable)
            {
                if (viewable is ValidatedView)
                {
                    ValidatedView validatedView = (ValidatedView) viewable;
                    validatedView.Validate(typeService,
                            statementContext.MethodResolutionService,
                            statementContext.SchedulingService,
                            statementContext.VariableService);
                }
            }

            // Construct a processor for results posted by views and joins, which takes care of aggregation if required.
            // May return null if we don't need to post-process results posted by views or joins.
            ResultSetProcessor resultSetProcessor = ResultSetProcessorFactory.GetProcessor(
                    statementSpec, statementContext, typeService, viewResourceDelegate);

            // Validate where-clause filter tree and outer join clause
            ValidateNodes(statementSpec, statementContext, typeService, viewResourceDelegate);

            // Materialize views
            Viewable[] streamViews = new Viewable[streamEventTypes.Length];
            for (int i = 0; i < streamViews.Length; i++)
            {
                streamViews[i] = services.ViewService.CreateViews(eventStreamParentViewable[i], unmaterializedViewChain[i].FactoryChain, statementContext);
            }

            // For just 1 event stream without joins, handle the one-table process separatly.
            Viewable finalView;
            JoinPreloadMethod joinPreloadMethod = null;
            if (streamNames.Length == 1)
            {
                finalView = HandleSimpleSelect(streamViews[0], resultSetProcessor, statementContext);
            }
            else
            {
                Pair<Viewable, JoinPreloadMethod> pair = HandleJoin(streamNames, streamEventTypes, streamViews, resultSetProcessor, statementSpec.SelectStreamSelectorEnum, statementContext, stopCallbacks, isUnidirectional, hasChildViews, isNamedWindow);
                finalView = pair.First;
                joinPreloadMethod = pair.Second;
            }

            // Replay any named window data, for later consumers of named data windows
            bool hasNamedWindow = false;
            for (int i = 0; i < statementSpec.StreamSpecs.Count; i++)
            {
                StreamSpecCompiled streamSpec = statementSpec.StreamSpecs[i];
                if (streamSpec is NamedWindowConsumerStreamSpec)
                {
                    hasNamedWindow = true;
                    NamedWindowConsumerStreamSpec namedSpec = (NamedWindowConsumerStreamSpec) streamSpec;
                    NamedWindowProcessor processor = services.NamedWindowService.GetProcessor(namedSpec.WindowName);
                    NamedWindowTailView consumerView = processor.TailView;
                    NamedWindowConsumerView view = (NamedWindowConsumerView) eventStreamParentViewable[i];

                    // preload view for stream unless the expiry policy is batch window
                    List<EventBean> eventsInWindow = new List<EventBean>();
                    if (!consumerView.IsParentBatchWindow) {
                        foreach (EventBean aConsumerView in consumerView) {
                            eventsInWindow.Add(aConsumerView);
                        }
                    }

                    if (CollectionHelper.IsNotEmpty(eventsInWindow))
                    {
                        EventBean[] newEvents = eventsInWindow.ToArray();
                        view.Update(newEvents, null);
                    }

                    // in a join, preload indexes, if any
                    if (joinPreloadMethod != null)
                    {
                        joinPreloadMethod.PreloadFromBuffer(i);
                    }
                }
            }
            // last, for aggregation we need to send the current join results to the result set processor
            if ((hasNamedWindow) && (joinPreloadMethod != null))
            {
                joinPreloadMethod.PreloadAggregation(resultSetProcessor);
            }

            log.Debug(".start Statement start completed");

            return new Pair<Viewable, EPStatementStopMethod>(finalView, stopMethod);
        }

        private Pair<Viewable, JoinPreloadMethod> HandleJoin(String[] streamNames,
                                                             EventType[] streamTypes,
                                                             Viewable[] streamViews,
                                                             ResultSetProcessor resultSetProcessor,
                                                             SelectClauseStreamSelectorEnum selectStreamSelectorEnum,
                                                             StatementContext _statementContext,
                                                             ICollection<StopCallback> stopCallbacks,
                                                             bool[] isUnidirectional,
                                                             bool[] hasChildViews,
                                                             bool[] isNamedWindow)
        {
            // Handle joins
            JoinSetComposer composer =
                _statementContext.JoinSetComposerFactory.MakeComposer(statementSpec.OuterJoinDescList,
                                                                      statementSpec.FilterRootNode,
                                                                      streamTypes,
                                                                      streamNames,
                                                                      streamViews,
                                                                      selectStreamSelectorEnum,
                                                                      isUnidirectional,
                                                                      hasChildViews,
                                                                      isNamedWindow);

            stopCallbacks.Add(new ProxyStopCallback( delegate { composer.Destroy(); }));

            JoinSetFilter filter = new JoinSetFilter(statementSpec.FilterRootNode);
            OutputProcessView indicatorView = OutputProcessViewFactory.MakeView(resultSetProcessor,
                                                                                statementSpec,
                                                                                _statementContext,
                                                                                services.InternalEventRouter);

            // Create strategy for join execution
            JoinExecutionStrategy execution = new JoinExecutionStrategyImpl(composer, filter, indicatorView);

            // The view needs a reference to the join execution to pull iterator values
            indicatorView.JoinExecutionStrategy = execution;

            // Hook up dispatchable with buffer and execution strategy
            JoinExecStrategyDispatchable joinStatementDispatch = new JoinExecStrategyDispatchable(execution, statementSpec.StreamSpecs.Count);
            _statementContext.EpStatementHandle.OptionalDispatchable = joinStatementDispatch;

            JoinPreloadMethodImpl preloadMethod = new JoinPreloadMethodImpl(streamNames.Length, composer);

            // Create buffer for each view. Point buffer to dispatchable for join.
            for (int i = 0; i < statementSpec.StreamSpecs.Count; i++)
            {
                BufferView buffer = new BufferView(i);
                streamViews[i].AddView(buffer);
                buffer.Observer = joinStatementDispatch;
                preloadMethod.SetBuffer(buffer, i);
            }

            return new Pair<Viewable, JoinPreloadMethod>(indicatorView, preloadMethod);
        }

        /// <summary>Returns a stream name assigned for each stream, generated if none was supplied. </summary>
        /// <param name="streams">stream specifications</param>
        /// <returns>array of stream names</returns>
        protected static String[] DetermineStreamNames(IList<StreamSpecCompiled> streams)
        {
            String[] streamNames = new String[streams.Count];
            for (int i = 0; i < streams.Count; i++)
            {
                // Assign a stream name for joins, if not supplied
                streamNames[i] = streams[i].OptionalStreamName;
                if (streamNames[i] == null)
                {
                    streamNames[i] = "stream_" + i;
                }
            }
            return streamNames;
        }

        /// <summary>Validate filter and join expression nodes.</summary>
        /// <param name="statementSpec">the compiled statement</param>
        /// <param name="statementContext">the statement services</param>
        /// <param name="typeService">the event types for streams</param>
        /// <param name="viewResourceDelegate">the delegate to verify expressions that use view resources</param>
        protected internal static void ValidateNodes(StatementSpecCompiled statementSpec,
                                            StatementContext statementContext,
                                            StreamTypeService typeService,
                                            ViewResourceDelegate viewResourceDelegate)
        {
            MethodResolutionService methodResolutionService = statementContext.MethodResolutionService;

            if (statementSpec.FilterRootNode != null)
            {
                ExprNode optionalFilterNode = statementSpec.FilterRootNode;

                // Validate where clause, initializing nodes to the stream ids used
                try
                {
                    optionalFilterNode = optionalFilterNode.GetValidatedSubtree(typeService, methodResolutionService, viewResourceDelegate, statementContext.SchedulingService, statementContext.VariableService);
                    statementSpec.FilterExprRootNode = optionalFilterNode;

                    // Make sure there is no aggregation in the where clause
                    List<ExprAggregateNode> aggregateNodes = new List<ExprAggregateNode>();
                    ExprAggregateNode.GetAggregatesBottomUp(optionalFilterNode, aggregateNodes);
                    if (CollectionHelper.IsNotEmpty(aggregateNodes))
                    {
                        throw new ExprValidationException("An aggregate function may not appear in a WHERE clause (use the HAVING clause)");
                    }
                }
                catch (ExprValidationException ex)
                {
                    log.Debug(".validateNodes Validation exception for filter=" + optionalFilterNode.ExpressionString, ex);
                    throw new EPStatementException("Error validating expression: " + ex.Message, statementContext.Expression);
                }
            }

            for (int outerJoinCount = 0; outerJoinCount < statementSpec.OuterJoinDescList.Count; outerJoinCount++)
            {
                OuterJoinDesc outerJoinDesc = statementSpec.OuterJoinDescList[outerJoinCount];

                UniformPair<int> streamIdPair = ValidateOuterJoinPropertyPair(
                    statementContext,
                    outerJoinDesc.LeftNode,
                    outerJoinDesc.RightNode,
                    outerJoinCount,
                    typeService,
                    viewResourceDelegate);

                if (outerJoinDesc.AdditionalLeftNodes != null)
                {
                    Set<int> streamSet = new HashSet<int>();
                    streamSet.Add(streamIdPair.First);
                    streamSet.Add(streamIdPair.Second);
                    for (int i = 0; i < outerJoinDesc.AdditionalLeftNodes.Length; i++)
                    {
                        UniformPair<int> streamIdPairAdd =
                            ValidateOuterJoinPropertyPair(statementContext,
                                                          outerJoinDesc.AdditionalLeftNodes[i],
                                                          outerJoinDesc.AdditionalRightNodes[i],
                                                          outerJoinCount,
                                                          typeService,
                                                          viewResourceDelegate);

                        // make sure all additional properties point to the same two streams
                        if ((!streamSet.Contains(streamIdPairAdd.First) || (!streamSet.Contains(streamIdPairAdd.Second))))
                        {
                            String message = "Outer join ON-clause columns must refer to properties of the same joined streams" +
                                    " when using multiple columns in the on-clause";
                            throw new EPStatementException("Error validating expression: " + message, statementContext.Expression);
                        }

                    }
                }
            }
        }

        private static UniformPair<int> ValidateOuterJoinPropertyPair(
                StatementContext statementContext,
                ExprIdentNode leftNode,
                ExprIdentNode rightNode,
                int outerJoinCount,
                StreamTypeService typeService,
                ViewResourceDelegate viewResourceDelegate)
        {
            // Validate the outer join clause using an artificial equals-node on top.
            // Thus types are checked via equals.
            // Sets stream ids used for validated nodes.
            ExprNode equalsNode = new ExprEqualsNode(false);
            equalsNode.AddChildNode(leftNode);
            equalsNode.AddChildNode(rightNode);
            try
            {
                equalsNode = equalsNode.GetValidatedSubtree(typeService, statementContext.MethodResolutionService, viewResourceDelegate, statementContext.SchedulingService, statementContext.VariableService);
            }
            catch (ExprValidationException ex)
            {
                log.Debug("Validation exception for outer join node=" + equalsNode.ExpressionString, ex);
                throw new EPStatementException("Error validating expression: " + ex.Message, statementContext.Expression);
            }

            // Make sure we have left-hand-side and right-hand-side refering to different streams
            int streamIdLeft = leftNode.StreamId;
            int streamIdRight = rightNode.StreamId;
            if (streamIdLeft == streamIdRight)
            {
                String message = "Outer join ON-clause cannot refer to properties of the same stream";
                throw new EPStatementException("Error validating expression: " + message, statementContext.Expression);
            }

            // Make sure one of the properties refers to the acutual stream currently being joined
            int expectedStreamJoined = outerJoinCount + 1;
            if ((streamIdLeft != expectedStreamJoined) && (streamIdRight != expectedStreamJoined))
            {
                String message = "Outer join ON-clause must refer to at least one property of the joined stream" +
                        " for stream " + expectedStreamJoined;
                throw new EPStatementException("Error validating expression: " + message, statementContext.Expression);
            }

            // Make sure neither of the streams refer to a 'future' stream
            String badPropertyName = null;
            if (streamIdLeft > outerJoinCount + 1)
            {
                badPropertyName = leftNode.ResolvedPropertyName;
            }
            if (streamIdRight > outerJoinCount + 1)
            {
                badPropertyName = rightNode.ResolvedPropertyName;
            }
            if (badPropertyName != null)
            {
                String message = "Outer join ON-clause invalid scope for property" +
                        " '" + badPropertyName + "', expecting the current or a prior stream scope";
                throw new EPStatementException("Error validating expression: " + message, statementContext.Expression);
            }

            return new UniformPair<int>(streamIdLeft, streamIdRight);
        }


        private Viewable HandleSimpleSelect(Viewable view,
                                            ResultSetProcessor resultSetProcessor,
                                            StatementContext _statementContext)
        {
            Viewable finalView = view;

            // Add filter view that evaluates the filter expression
            if (statementSpec.FilterRootNode != null)
            {
                FilterExprView filterView = new FilterExprView(statementSpec.FilterRootNode);
                finalView.AddView(filterView);
                finalView = filterView;
            }

            OutputProcessView selectView = OutputProcessViewFactory.MakeView(resultSetProcessor, statementSpec,
                    _statementContext, services.InternalEventRouter);
            finalView.AddView(selectView);
            finalView = selectView;

            return finalView;
        }

        private SubSelectStreamCollection CreateSubSelectStreams(bool isJoin)
        {
            SubSelectStreamCollection subSelectStreamDesc = new SubSelectStreamCollection();
            int subselectStreamNumber = 1024;

            // Process all subselect expression nodes
            foreach (ExprSubselectNode subselect in statementSpec.SubSelectExpressions)
            {
                StatementSpecCompiled lstatementSpec = subselect.StatementSpecCompiled;

                if (lstatementSpec.StreamSpecs[0] is FilterStreamSpecCompiled)
                {
                    FilterStreamSpecCompiled filterStreamSpec = (FilterStreamSpecCompiled) lstatementSpec.StreamSpecs[0];

                    // A child view is required to limit the stream
                    if (filterStreamSpec.ViewSpecs.Count == 0)
                    {
                        throw new ExprValidationException("Subqueries require one or more views to limit the stream, consider declaring a length or time window");
                    }

                    subselectStreamNumber++;

                    // Register filter, create view factories
                    Pair<EventStream, ManagedLock> streamLockPair = services.StreamService.CreateStream(filterStreamSpec.FilterSpec,
                            services.FilterService, statementContext.EpStatementHandle, isJoin);
                    Viewable viewable = streamLockPair.First;
                    ViewFactoryChain viewFactoryChain = services.ViewService.CreateFactories(subselectStreamNumber, viewable.EventType, filterStreamSpec.ViewSpecs, statementContext);
                    subselect.RawEventType = viewFactoryChain.EventType;

                    // Add lookup to list, for later starts
                    subSelectStreamDesc.Add(subselect, subselectStreamNumber, viewable, viewFactoryChain);
                }
                else
                {
                    NamedWindowConsumerStreamSpec namedSpec =
                        (NamedWindowConsumerStreamSpec) lstatementSpec.StreamSpecs[0];
                    NamedWindowProcessor processor = services.NamedWindowService.GetProcessor(namedSpec.WindowName);
                    NamedWindowConsumerView consumerView = processor.AddConsumer(namedSpec.FilterExpressions, statementContext.EpStatementHandle, statementContext.StatementStopService);
                    ViewFactoryChain viewFactoryChain = services.ViewService.CreateFactories(0, consumerView.EventType, namedSpec.ViewSpecs, statementContext);
                    subSelectStreamDesc.Add(subselect, subselectStreamNumber, consumerView, viewFactoryChain);
                }
            }

            return subSelectStreamDesc;
        }

        private void StartSubSelect(SubSelectStreamCollection subSelectStreamDesc,
                                    String[] outerStreamNames,
                                    EventType[] outerEventTypes,
                                    String[] outerEventTypeAliases,
                                    ICollection<StopCallback> stopCallbacks)
        {
            foreach (ExprSubselectNode subselect in this.statementSpec.SubSelectExpressions)
            {
                StatementSpecCompiled lstatementSpec = subselect.StatementSpecCompiled;
                StreamSpecCompiled filterStreamSpec = lstatementSpec.StreamSpecs[0];
                
                String subselectEventTypeAlias = null;
                if (filterStreamSpec is FilterStreamSpecCompiled)
                {
                    subselectEventTypeAlias = ((FilterStreamSpecCompiled) filterStreamSpec).FilterSpec.EventTypeAlias;
                }
                else if (filterStreamSpec is NamedWindowConsumerStreamSpec)
                {
                    subselectEventTypeAlias = ((NamedWindowConsumerStreamSpec) filterStreamSpec).WindowName;
                }

                ViewFactoryChain viewFactoryChain = subSelectStreamDesc.GetViewFactoryChain(subselect);
                EventType eventType = viewFactoryChain.EventType;

                // determine a stream name unless one was supplied
                String subexpressionStreamName = filterStreamSpec.OptionalStreamName;
                int subselectStreamNumber = subSelectStreamDesc.GetStreamNumber(subselect);
                if (subexpressionStreamName == null)
                {
                    subexpressionStreamName = "$subselect_" + subselectStreamNumber;
                }

                // Named windows don't allow data views
                if (filterStreamSpec is NamedWindowConsumerStreamSpec)
                {
                    ViewResourceDelegate viewResourceDelegate = new ViewResourceDelegateImpl(new ViewFactoryChain[] {viewFactoryChain}, statementContext);
                    viewResourceDelegate.RequestCapability(0, new NotADataWindowViewCapability(), null);
                }

                // Streams event types are the original stream types with the stream zero the subselect stream
                LinkedHashMap<String, Pair<EventType, String>> namesAndTypes = new LinkedHashMap<String, Pair<EventType, String>>();
                namesAndTypes.Put(subexpressionStreamName, new Pair<EventType, String>(eventType, subselectEventTypeAlias));
                for (int i = 0; i < outerEventTypes.Length; i++)
                {
                    Pair<EventType, String> pair = new Pair<EventType, String>(outerEventTypes[i], outerEventTypeAliases[i]);
                    namesAndTypes.Put(outerStreamNames[i], pair);
                }
                StreamTypeService subselectTypeService = new StreamTypeServiceImpl(namesAndTypes, services.EngineURI, true, true);
                ViewResourceDelegate viewResourceDelegateSubselect = new ViewResourceDelegateImpl(new ViewFactoryChain[] { viewFactoryChain }, statementContext);

                // Validate select expression
                SelectClauseSpecCompiled selectClauseSpec = subselect.StatementSpecCompiled.SelectClauseSpec;
                AggregationService aggregationService = null;
                if (selectClauseSpec.SelectExprList.Count > 0)
                {
                    SelectClauseElementCompiled element = selectClauseSpec.SelectExprList[0];
                    if (element is SelectClauseExprCompiledSpec)
                    {
                        // validate
                        SelectClauseExprCompiledSpec compiled = (SelectClauseExprCompiledSpec) element;
                        ExprNode selectExpression = compiled.SelectExpression;
                        selectExpression = selectExpression.GetValidatedSubtree(subselectTypeService, statementContext.MethodResolutionService, viewResourceDelegateSubselect, statementContext.SchedulingService, statementContext.VariableService);
                        subselect.SelectClause = selectExpression;
                        subselect.SelectAsName = compiled.AssignedName;

                        // handle aggregation
                        List<ExprAggregateNode> aggExprNodes = new List<ExprAggregateNode>();
                        ExprAggregateNode.GetAggregatesBottomUp(selectExpression, aggExprNodes);
                        if (aggExprNodes.Count > 0)
                        {
                            IList<ExprAggregateNode> havingAgg = CollectionHelper.GetEmptyList<ExprAggregateNode>();
                            IList<ExprAggregateNode> orderByAgg = CollectionHelper.GetEmptyList<ExprAggregateNode>();
                            aggregationService = AggregationServiceFactory.GetService(aggExprNodes, havingAgg, orderByAgg, false, null);

                            // Other stream properties, if there is aggregation, cannot be under aggregation.
                            foreach (ExprAggregateNode aggNode in aggExprNodes)
                            {
                                IList<Pair<int, String>> propertiesNodesAggregated = GetExpressionProperties(aggNode, true);
                                foreach (Pair<int, String> pair in propertiesNodesAggregated)
                                {
                                    if (pair.First != 0)
                                    {
                                        throw new ExprValidationException("Subselect aggregation function cannot aggregate across correlated properties");
                                    }
                                }
                            }

                            // This stream (stream 0) properties must either all be under aggregation, or all not be.
                            IList<Pair<int, String>> propertiesNotAggregated = GetExpressionProperties(selectExpression, false);
                            foreach (Pair<int, String> pair in propertiesNotAggregated)
                            {
                                if (pair.First == 0)
                                {
                                    throw new ExprValidationException("Subselect properties must all be within aggregation functions");
                                }
                            }
                        }
                    }
                }

                // no aggregation functions allowed in filter
                if (lstatementSpec.FilterRootNode != null)
                {
                    List<ExprAggregateNode> aggExprNodesFilter = new List<ExprAggregateNode>();
                    ExprAggregateNode.GetAggregatesBottomUp(lstatementSpec.FilterRootNode, aggExprNodesFilter);
                    if (aggExprNodesFilter.Count > 0)
                    {
                        throw new ExprValidationException("Aggregation functions are not supported within subquery filters, consider using insert-into instead");
                    }
                }

                // Validate filter expression, if there is one
                ExprNode filterExpr = lstatementSpec.FilterRootNode;
                if (filterExpr != null)
                {
                    filterExpr = filterExpr.GetValidatedSubtree(subselectTypeService, statementContext.MethodResolutionService, viewResourceDelegateSubselect, statementContext.SchedulingService, statementContext.VariableService);
                    if (TypeHelper.GetBoxedType(filterExpr.ReturnType) != typeof(bool?))
                    {
                        throw new ExprValidationException("Subselect filter expression must return a boolean value");
                    }

                    // check the presence of a correlated filter, not allowed with aggregation
                    ExprNodeIdentifierVisitor visitor = new ExprNodeIdentifierVisitor(true);
                    filterExpr.Accept(visitor);
                    IList<Pair<int, String>> propertiesNodes = visitor.ExprProperties;
                    foreach (Pair<int, String> pair in propertiesNodes)
                    {
                        if ((pair.First != 0) && (aggregationService != null))
                        {
                            throw new ExprValidationException("Subselect filter expression cannot be a correlated expression when aggregating properties via aggregation function");
                        }
                    }
                }

                // Finally create views
                Viewable viewableRoot = subSelectStreamDesc.GetRootViewable(subselect);
                Viewable subselectView = services.ViewService.CreateViews(viewableRoot, viewFactoryChain.FactoryChain, statementContext);

                // If we do aggregation, then the view results must be added and removed from aggregation
                EventTable eventIndex;
                // Under aggregation conditions, there is no lookup/corelated subquery strategy, and
                // the view-supplied events are simply aggregated, a null-event supplied to the stream for the select-clause, and not kept in index.
                // Note that "var1 + Max(var2)" is not allowed as some properties are not under aggregation (which event to use?).
                if (aggregationService != null)
                {
                    SubselectAggregatorView aggregatorView = new SubselectAggregatorView(aggregationService, filterExpr);
                    subselectView.AddView(aggregatorView);
                    subselectView = aggregatorView;

                    eventIndex = null;
                    subselect.Strategy = new TableLookupStrategyNullRow();
                    subselect.FilterExpr = null;      // filter not evaluated by subselect expression as not correlated
                }
                else
                {
                    // Determine indexing of the filter expression
                    Pair<EventTable, TableLookupStrategy> indexPair = DetermineSubqueryIndex(filterExpr,
                                                                                             eventType,
                                                                                             outerEventTypes,
                                                                                             subselectTypeService);
                    subselect.Strategy = indexPair.Second;
                    subselect.FilterExpr = filterExpr;
                    eventIndex = indexPair.First;
                }

                // Clear out index on statement stop
                stopCallbacks.Add(new SubqueryStopCallback(eventIndex));

                // Preload
                if (filterStreamSpec is NamedWindowConsumerStreamSpec)
                {
                    NamedWindowConsumerStreamSpec namedSpec = (NamedWindowConsumerStreamSpec) filterStreamSpec ;
                    NamedWindowProcessor processor = services.NamedWindowService.GetProcessor(namedSpec.WindowName);
                    NamedWindowTailView consumerView = processor.TailView;

                    // preload view for stream
                    List<EventBean> eventsInWindow = new List<EventBean>();
                    eventsInWindow.AddRange(consumerView);

                    EventBean[] newEvents = eventsInWindow.ToArray();
                    ((View)viewableRoot).Update(newEvents, null); // fill view
                    if (eventIndex != null)
                    {
                        eventIndex.Add(newEvents);  // fill index
                    }
                }
                else        // preload from the data window that site on top
                {
                    // Start up event table from the iterator
                    IEnumerator<EventBean> it = subselectView.GetEnumerator();
                    if ((it != null) && (it.MoveNext())) {
                        List<EventBean> preloadEvents = new List<EventBean>();

                        do {
                            preloadEvents.Add(it.Current);
                        } while (it.MoveNext());

                        if (eventIndex != null) {
                            eventIndex.Add(preloadEvents.ToArray());
                        }
                    }
                }

                // hook up subselect viewable and event table
                BufferView bufferView = new BufferView(subselectStreamNumber);
                bufferView.Observer = new SubselectBufferObserver(eventIndex);
                subselectView.AddView(bufferView);
            }
        }

        private static Pair<EventTable, TableLookupStrategy> DetermineSubqueryIndex(
            ExprNode filterExpr,
            EventType viewableEventType,
            EventType[] outerEventTypes,
            StreamTypeService subselectTypeService)
        {
            // No filter expression means full table scan
            if (filterExpr == null)
            {
                UnindexedEventTable table = new UnindexedEventTable(0);
                FullTableScanLookupStrategy strategy = new FullTableScanLookupStrategy(table);
                return new Pair<EventTable, TableLookupStrategy>(table, strategy);
            }

            // analyze query graph
            QueryGraph queryGraph = new QueryGraph(outerEventTypes.Length + 1);
            FilterExprAnalyzer.Analyze(filterExpr, queryGraph);

            // Build a list of streams and indexes
            Map<String, JoinedPropDesc> joinProps = new LinkedHashMap<String, JoinedPropDesc>();
            bool mustCoerce = false;
            for (int stream = 0; stream <  outerEventTypes.Length; stream++)
            {
                int lookupStream = stream + 1;
                String[] keyPropertiesJoin = queryGraph.GetKeyProperties(lookupStream, 0);
                String[] indexPropertiesJoin = queryGraph.GetIndexProperties(lookupStream, 0);
                if ((keyPropertiesJoin == null) || (keyPropertiesJoin.Length == 0))
                {
                    continue;
                }
                if (keyPropertiesJoin.Length != indexPropertiesJoin.Length)
                {
                    throw new IllegalStateException("Invalid query key and index property collection for stream " + stream);
                }

                for (int i = 0; i < keyPropertiesJoin.Length; i++)
                {
                    Type keyPropType = TypeHelper.GetBoxedType(subselectTypeService.EventTypes[lookupStream].GetPropertyType(keyPropertiesJoin[i]));
                    Type indexedPropType = TypeHelper.GetBoxedType(subselectTypeService.EventTypes[0].GetPropertyType(indexPropertiesJoin[i]));
                    Type coercionType = indexedPropType;
                    if (keyPropType != indexedPropType)
                    {
                        coercionType = TypeHelper.GetCompareToCoercionType(keyPropType, keyPropType);
                        mustCoerce = true;
                    }

                    JoinedPropDesc desc = new JoinedPropDesc(indexPropertiesJoin[i],
                            coercionType, keyPropertiesJoin[i], stream);
                    joinProps.Put(indexPropertiesJoin[i], desc);
                }
            }

            if (joinProps.Count != 0)
            {
                String[] indexedProps = CollectionHelper.ToArray(joinProps.Keys);
                int[] keyStreamNums = JoinedPropDesc.GetKeyStreamNums(joinProps.Values);
                String[] keyProps = JoinedPropDesc.GetKeyProperties(joinProps.Values);

                if (!mustCoerce)
                {
                    PropertyIndexedEventTable table = new PropertyIndexedEventTable(0, viewableEventType, indexedProps);
                    TableLookupStrategy strategy = new IndexedTableLookupStrategy( outerEventTypes,
                            keyStreamNums, keyProps, table);
                    return new Pair<EventTable, TableLookupStrategy>(table, strategy);
                }
                else
                {
                    Type[] coercionTypes = JoinedPropDesc.GetCoercionTypes(joinProps.Values);
                    PropertyIndTableCoerceAdd table = new PropertyIndTableCoerceAdd(0, viewableEventType, indexedProps, coercionTypes);
                    TableLookupStrategy strategy = new IndexedTableLookupStrategyCoercing( outerEventTypes, keyStreamNums, keyProps, table, coercionTypes);
                    return new Pair<EventTable, TableLookupStrategy>(table, strategy);
                }
            }
            else
            {
                UnindexedEventTable table = new UnindexedEventTable(0);
                return new Pair<EventTable, TableLookupStrategy>(table, new FullTableScanLookupStrategy(table));
            }
        }

        // For delete actions from named windows
        private ExprNode ValidateJoinNamedWindow(ExprNode deleteJoinExpr,
                                                 EventType namedWindowType,
                                                 String namedWindowStreamName,
                                                 String namedWindowName,
                                                 EventType filteredType,
                                                 String filterStreamName,
                                                 String filteredTypeAlias)
        {
            if (deleteJoinExpr == null)
            {
                return null;
            }

            LinkedHashMap<String, Pair<EventType, String>> namesAndTypes = new LinkedHashMap<String, Pair<EventType, String>>();
            namesAndTypes.Put(namedWindowStreamName, new Pair<EventType, String>(namedWindowType, namedWindowName));
            namesAndTypes.Put(filterStreamName, new Pair<EventType, String>(filteredType, filteredTypeAlias));
            StreamTypeService typeService = new StreamTypeServiceImpl(namesAndTypes, services.EngineURI, false, false);

            return deleteJoinExpr.GetValidatedSubtree(typeService, statementContext.MethodResolutionService, null, statementContext.SchedulingService, statementContext.VariableService);
        }

        private static IList<Pair<int, String>> GetExpressionProperties(ExprNode exprNode, bool visitAggregateNodes)
        {
            ExprNodeIdentifierVisitor visitor = new ExprNodeIdentifierVisitor(visitAggregateNodes);
            exprNode.Accept(visitor);
            return visitor.ExprProperties;
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
