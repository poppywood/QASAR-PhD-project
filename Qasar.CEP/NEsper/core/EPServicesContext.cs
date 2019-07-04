using System;
using com.espertech.esper.dispatch;
using com.espertech.esper.emit;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.db;
using com.espertech.esper.epl.named;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.variable;
using com.espertech.esper.epl.view;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.filter;
using com.espertech.esper.schedule;
using com.espertech.esper.timer;
using com.espertech.esper.util;
using com.espertech.esper.view;
using com.espertech.esper.view.stream;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Convenience class to hold implementations for all services.
    /// </summary>

    public sealed class EPServicesContext
    {
        private String engineURI;
        private String engineInstanceId;
        private FilterService filterService;
        private TimerService timerService;
        private SchedulingService schedulingService;
        private EmitService emitService;
        private DispatchService dispatchService;
        private ViewService viewService;
        private StreamFactoryService streamFactoryService;
        private EventAdapterService eventAdapterService;
        private EngineImportService engineImportService;
        private EngineSettingsService engineSettingsService;
        private DatabaseConfigService databaseConfigService;
        private PluggableObjectCollection plugInViews;
        private StatementLockFactory statementLockFactory;
        private ManagedReadWriteLock eventProcessingRWLock;
        private ExtensionServicesContext extensionServicesContext;
        private Directory engineDirectory;
        private StatementContextFactory statementContextFactory;
        private PluggableObjectCollection plugInPatternObjects;
        private readonly OutputConditionFactory outputConditionFactory;
        private NamedWindowService namedWindowService;
        private readonly VariableService variableService;
        private readonly TimeSourceService timeSourceService;
        private ValueAddEventService valueAddEventService;

        // Supplied after construction to avoid circular dependency
        private StatementLifecycleSvc statementLifecycleSvc;
        private InternalEventRouter internalEventRouter;

        /// <summary>
        /// Gets the engine instance id.
        /// </summary>
        /// <value>The engine instance id.</value>
        public string EngineInstanceId
        {
            get { return engineInstanceId; }
        }

        /// <summary>
        /// Gets or sets router for internal event processing.
        /// </summary>
        /// <value>The internal event router.</value>
        public InternalEventRouter InternalEventRouter
        {
            get { return internalEventRouter; }
            set { this.internalEventRouter = value; }
        }
        /// <summary> Returns filter evaluation service implementation.</summary>
        /// <returns> filter evaluation service
        /// </returns>
        public FilterService FilterService
        {
            get { return filterService; }
        }
        /// <summary> Returns time provider service implementation.</summary>
        /// <returns> time provider service
        /// </returns>
        public TimerService TimerService
        {
            get { return timerService; }
        }
        /// <summary> Returns scheduling service implementation.</summary>
        /// <returns> scheduling service
        /// </returns>
        public SchedulingService SchedulingService
        {
            get { return schedulingService; }
        }
        /// <summary> Returns service for emitting events.</summary>
        /// <returns> emit event service
        /// </returns>
        public EmitService EmitService
        {
            get { return emitService; }
        }
        /// <summary> Returns dispatch service responsible for dispatching events to listeners.</summary>
        /// <returns> dispatch service.
        /// </returns>
        public DispatchService DispatchService
        {
            get { return dispatchService; }
        }
        /// <summary> Returns services for view creation, sharing and removal.</summary>
        /// <returns> view service
        /// </returns>
        public ViewService ViewService
        {
            get { return viewService; }
        }
        /// <summary> Returns stream service.</summary>
        /// <returns> stream service
        /// </returns>
        public StreamFactoryService StreamService
        {
            get { return streamFactoryService; }
        }
        /// <summary> Returns event type resolution service.</summary>
        /// <returns> service resolving event type
        /// </returns>
        public EventAdapterService EventAdapterService
        {
            get { return eventAdapterService; }
        }
        /// <summary> Returns the import and class name resolution service.</summary>
        /// <returns> import service
        /// </returns>
        public EngineImportService EngineImportService
        {
            get { return engineImportService; }
        }
        /// <summary> Returns the database settings service.</summary>
        /// <returns> database info service
        /// </returns>
        public DatabaseConfigService DatabaseRefService
        {
            get { return databaseConfigService; }
        }

        /// <summary>
        /// Information to resolve plug-in view namespace and name.
        /// </summary>

        public PluggableObjectCollection PlugInViews
        {
            get { return plugInViews; }
        }

        /// <summary>
        /// Information to resolve plug-in pattern object namespace and name.
        /// </summary>

        public PluggableObjectCollection PlugInPatternObjects
        {
            get { return plugInPatternObjects; }
        }

        /// <summary>
        /// Factory for statement-level locks.
        /// </summary>
        /// <returns> factory</returns>
        public StatementLockFactory StatementLockFactory
        {
            get { return statementLockFactory; }
        }

        /// <summary>
        /// Returns the event processing lock for coordinating statement administration with event processing.
        /// </summary>
        /// <returns> lock</returns>
        public ManagedReadWriteLock EventProcessingRWLock
        {
            get { return eventProcessingRWLock; }
        }

        /// <summary>
        /// Gets or sets the service dealing with starting and stopping statements.
        /// </summary>
        /// <returns> service for statement start and stop</returns>
        public StatementLifecycleSvc StatementLifecycleSvc
        {
            get { return statementLifecycleSvc; }
            set { this.statementLifecycleSvc = value; }
        }

        /// <summary>
        /// Returns extension service for adding custom the services.
        /// </summary>
        /// <returns> extension service context</returns>
        public ExtensionServicesContext ExtensionServicesContext
        {
            get { return extensionServicesContext; }
        }

        /// <summary>
        /// Returns the engine directory for getting access to engine-external resources, such as adapters
        /// </summary>
        /// <returns> engine directory</returns>
        public Directory EngineDirectory
        {
            get { return engineDirectory; }
        }

        /// <summary>
        /// Destroy services.
        /// </summary>
        public void Destroy()
        {
            if (statementLifecycleSvc != null)
            {
                statementLifecycleSvc.Destroy();
            }
            if (extensionServicesContext != null)
            {
                extensionServicesContext.Destroy();
            }
            if (filterService != null)
            {
                filterService.Destroy();
            }
            if (schedulingService != null)
            {
                schedulingService.Destroy();
            }
            if (streamFactoryService != null)
            {
                streamFactoryService.Destroy();
            }
            if (namedWindowService != null)
            {
                namedWindowService.Destroy();
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            this.statementLifecycleSvc = null;
            this.engineURI = null;
            this.schedulingService = null;
            this.eventAdapterService = null;
            this.engineImportService = null;
            this.engineSettingsService = null;
            this.databaseConfigService = null;
            this.filterService = null;
            this.timerService = null;
            this.emitService = null;
            this.dispatchService = null;
            this.viewService = null;
            this.streamFactoryService = null;
            this.plugInViews = null;
            this.statementLockFactory = null;
            this.eventProcessingRWLock = null;
            this.extensionServicesContext = null;
            this.engineDirectory = null;
            this.statementContextFactory = null;
            this.plugInPatternObjects = null;
            this.namedWindowService = null;
            this.valueAddEventService = null;
        }

        /// <summary>
        /// Returns the factory to use for creating a statement context.
        /// </summary>
        /// <returns> statement context factory</returns>
        public StatementContextFactory StatementContextFactory
        {
            get { return statementContextFactory; }
        }

        /// <summary>
        /// Returns the engine URI.
        /// </summary>
        /// <returns> engine URI</returns>
        public String EngineURI
        {
            get { return engineURI; }
        }

        /// <summary>
        /// Gets the engine settings service.
        /// </summary>
        /// <value>The engine settings service.</value>
        public EngineSettingsService EngineSettingsService
        {
            get { return engineSettingsService; }
        }

        /// <summary>
        /// Gets the output condition factory.
        /// </summary>
        /// <value>The output condition factory.</value>
        public OutputConditionFactory OutputConditionFactory
        {
            get { return outputConditionFactory; }
        }

        /// <summary>Returns the named window management service.</summary>
        /// <returns>service for managing named windows</returns>
        public NamedWindowService NamedWindowService
        {
            get { return namedWindowService; }
        }

        /// <summary>Returns the variable service.</summary>
        /// <returns>variable service</returns>
        public VariableService VariableService
        {
            get { return variableService; }
        }

        /// <summary>
        /// Returns the time source provider class.
        /// </summary>
        /// <value>The time source.</value>
        /// <returns>time source</returns>
        public TimeSourceService TimeSource
        {
            get { return timeSourceService; }
        }

        /// <summary>
        /// Returns the service for handling updates to events.
        /// </summary>
        /// <value>The value add event service.</value>
        /// <returns>revision service</returns>
        public ValueAddEventService ValueAddEventService
        {
            get { return valueAddEventService; }
        }

        /// <summary>Constructor - sets up new set of services.</summary>
        /// <param name="engineURI">is the engine URI</param>
        /// <param name="schedulingService">service to get time and schedule callbacks</param>
        /// <param name="eventAdapterService">service to resolve event types</param>
        /// <param name="databaseConfigService">service to resolve a database name to database connection factory and configs</param>
        /// <param name="plugInViews">resolves view namespace and name to view factory class</param>
        /// <param name="statementLockFactory">creates statement-level locks</param>
        /// <param name="eventProcessingRWLock">is the engine lock for statement management</param>
        /// <param name="extensionServicesContext">marker interface allows adding additional services</param>
        /// <param name="engineImportService">is engine imported static func packages and aggregation functions</param>
        /// <param name="engineSettingsService">provides engine settings</param>
        /// <param name="statementContextFactory">is the factory to use to create statement context objects</param>
        /// <param name="engineDirectory">is engine environment/directory information for use with adapters and external env</param>
        /// <param name="plugInPatternObjects">resolves plug-in pattern objects</param>
        /// <param name="outputConditionFactory">factory for output condition objects</param>
        /// <param name="timerService">is the timer service</param>
        /// <param name="filterService">the filter service</param>
        /// <param name="streamFactoryService">is hooking up filters to streams</param>
        /// <param name="namedWindowService">is holding information about the named windows active in the system</param>
        /// <param name="variableService">provides access to variable values</param>
        /// <param name="valueAddEventService">handles update events</param>
        /// <param name="timeSourceService">time source provider class</param>
        public EPServicesContext( String engineURI,
                                  SchedulingService schedulingService,
                                  EventAdapterService eventAdapterService,
                                  EngineImportService engineImportService,
                                  EngineSettingsService engineSettingsService,
                                  DatabaseConfigService databaseConfigService,
                                  PluggableObjectCollection plugInViews,
                                  StatementLockFactory statementLockFactory,
                                  ManagedReadWriteLock eventProcessingRWLock,
                                  ExtensionServicesContext extensionServicesContext,
                                  Directory engineDirectory,
                                  StatementContextFactory statementContextFactory,
                                  PluggableObjectCollection plugInPatternObjects,
                                  OutputConditionFactory outputConditionFactory,
                                  TimerService timerService,
                                  FilterService filterService,
                                  StreamFactoryService streamFactoryService,
                                  NamedWindowService namedWindowService,
                                  VariableService variableService,
                                  TimeSourceService timeSourceService,
                                  ValueAddEventService valueAddEventService)
        {
            this.engineURI = engineURI;
            this.schedulingService = schedulingService;
            this.eventAdapterService = eventAdapterService;
            this.engineImportService = engineImportService;
            this.engineSettingsService = engineSettingsService;
            this.databaseConfigService = databaseConfigService;
            this.filterService = filterService;
            this.timerService = timerService;
            this.emitService = EmitServiceProvider.NewService();
            this.dispatchService = DispatchServiceProvider.NewService();
            this.viewService = ViewServiceProvider.NewService();
            this.streamFactoryService = streamFactoryService;
            this.plugInViews = plugInViews;
            this.statementLockFactory = statementLockFactory;
            this.eventProcessingRWLock = eventProcessingRWLock;
            this.extensionServicesContext = extensionServicesContext;
            this.engineDirectory = engineDirectory;
            this.statementContextFactory = statementContextFactory;
            this.plugInPatternObjects = plugInPatternObjects;
            this.outputConditionFactory = outputConditionFactory;
            this.namedWindowService = namedWindowService;
            this.variableService = variableService;
            this.timeSourceService = timeSourceService;
            this.valueAddEventService = valueAddEventService;
        }
    }
}
