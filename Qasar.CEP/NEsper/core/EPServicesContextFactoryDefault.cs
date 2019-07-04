///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;
using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.db;
using com.espertech.esper.epl.named;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.variable;
using com.espertech.esper.epl.view;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.filter;
using com.espertech.esper.plugin;
using com.espertech.esper.schedule;
using com.espertech.esper.timer;
using com.espertech.esper.util;
using com.espertech.esper.view.stream;

namespace com.espertech.esper.core
{
	/// <summary>Factory for services context.</summary>
	public class EPServicesContextFactoryDefault : EPServicesContextFactory
	{
        /// <summary>
        /// Creates the services context.
        /// </summary>
        /// <param name="epServiceProvider">The ep service provider.</param>
        /// <param name="configSnapshot">The config snapshot.</param>
        /// <returns></returns>
        public EPServicesContext CreateServicesContext(EPServiceProvider epServiceProvider, ConfigurationInformation configSnapshot)
        {
	        // Make services that depend on snapshot config entries
	        EventAdapterServiceImpl eventAdapterService = new EventAdapterServiceImpl();
	        Init(eventAdapterService, configSnapshot);

	        // New read-write lock for concurrent event processing
	        ManagedReadWriteLock eventProcessingRWLock = new ManagedReadWriteLock("EventProcLock", false);

            TimeSourceService timeSourceService = MakeTimeSource(configSnapshot);
            SchedulingService schedulingService = SchedulingServiceProvider.NewService(timeSourceService);
            EngineImportService engineImportService = MakeEngineImportService(configSnapshot);
            EngineSettingsService engineSettingsService = new EngineSettingsService(configSnapshot.EngineDefaults, configSnapshot.PlugInEventTypeAliasResolutionURIs);
            DatabaseConfigService databaseConfigService = MakeDatabaseRefService(configSnapshot, schedulingService);

            PluggableObjectCollection plugInViews = new PluggableObjectCollection();
            plugInViews.AddViews(configSnapshot.PlugInViews);
            PluggableObjectCollection plugInPatternObj = new PluggableObjectCollection();
            plugInPatternObj.AddPatternObjects(configSnapshot.PlugInPatternObjects);

	        // Directory for binding resources
	        Directory resourceDirectory = new SimpleServiceDirectory();

	        // Statement context factory
            StatementContextFactory statementContextFactory = new StatementContextFactoryDefault(plugInViews, plugInPatternObj);

            OutputConditionFactory outputConditionFactory = new OutputConditionFactoryDefault();

            long msecTimerResolution = configSnapshot.EngineDefaults.Threading.InternalTimerMsecResolution;
            if (msecTimerResolution <= 0)
            {
                throw new ConfigurationException("Timer resolution configuration not set to a valid value, expecting a non-zero value");
            }
            TimerService timerService = new TimerServiceImpl(msecTimerResolution);

            VariableService variableService = new VariableServiceImpl(configSnapshot.EngineDefaults.Variables.MsecVersionRelease, schedulingService, null);
            InitVariables(variableService, configSnapshot.Variables);

            StatementLockFactory statementLockFactory = new StatementLockFactoryImpl();
            StreamFactoryService streamFactoryService = StreamFactoryServiceProvider.NewService(configSnapshot.EngineDefaults.ViewResources.IsShareViews);
            FilterService filterService = FilterServiceProvider.NewService();
            NamedWindowService namedWindowService = new NamedWindowServiceImpl(statementLockFactory, variableService);

            ValueAddEventService valueAddEventService = new ValueAddEventServiceImpl();
            valueAddEventService.Init(configSnapshot.RevisionEventTypes, configSnapshot.VariantStreams, eventAdapterService);

            // New services context
            EPServicesContext services = new EPServicesContext(epServiceProvider.URI, schedulingService,
                    eventAdapterService, engineImportService, engineSettingsService, databaseConfigService, plugInViews,
                    statementLockFactory, eventProcessingRWLock, null, resourceDirectory, statementContextFactory,
                    plugInPatternObj, outputConditionFactory, timerService, filterService, streamFactoryService,
                    namedWindowService, variableService, timeSourceService, valueAddEventService);

            // Circular dependency
            StatementLifecycleSvc statementLifecycleSvc = new StatementLifecycleSvcImpl(epServiceProvider, services);
            services.StatementLifecycleSvc = statementLifecycleSvc;

	        return services;
	    }

        /// <summary>
        /// Creates the thread factory.
        /// </summary>
        /// <param name="threadLocalStyle">The thread local style.</param>
        /// <returns></returns>
        protected static ThreadLocalFactory CreateThreadLocalFactory(ConfigurationEngineDefaults.ThreadLocal threadLocalStyle)
        {
            switch (threadLocalStyle)
            {
                case ConfigurationEngineDefaults.ThreadLocal.FAST:
                    return new FastThreadLocalFactory();
                case ConfigurationEngineDefaults.ThreadLocal.SYSTEM:
                    return new SystemThreadLocalFactory();
                default:
                    throw new ArgumentException("invalid thread local style " + threadLocalStyle, "threadLocalStyle");
            }
        }

        /// <summary>Makes the time source provider.</summary>
        /// <param name="configSnapshot">the configuration</param>
        /// <returns>time source provider</returns>
        protected static TimeSourceService MakeTimeSource(ConfigurationInformation configSnapshot)
        {
            if (configSnapshot.EngineDefaults.TimeSource.TimeSourceType == TimeSourceType.NANO)
            {
                // this is a static variable to keep overhead down for getting a current time
                TimeSourceService.IS_SYSTEM_CURRENT_TIME = false;
            }
            return new TimeSourceService();
        }

        /// <summary>
        /// Adds configured variables to the variable service.
        /// </summary>
        /// <param name="variableService">service to add to</param>
        /// <param name="variables">configured variables</param>
        protected static void InitVariables(VariableService variableService, Map<String, ConfigurationVariable> variables)
        {
            foreach (KeyValuePair<String, ConfigurationVariable> entry in variables)
            {
                try
                {
                    variableService.CreateNewVariable(entry.Key, entry.Value.VariableType,
                                                      entry.Value.InitializationValue, null);
                }
                catch (VariableExistsException e)
                {
                    throw new ConfigurationException("Error configuring variables: " + e.Message, e);
                }
                catch (VariableTypeException e)
                {
                    throw new ConfigurationException("Error configuring variables: " + e.Message, e);
                }
            }
        }

	    /// <summary>Initialize event adapter service for config snapshot.</summary>
	    /// <param name="eventAdapterService">is events adapter</param>
	    /// <param name="configSnapshot">is the config snapshot</param>
        protected static void Init(EventAdapterService eventAdapterService, ConfigurationInformation configSnapshot)
	    {
	        // Extract legacy event type definitions for each event type alias, if supplied.
	        //
	        // We supply this information as setup information to the event adapter service
	        // to allow discovery of superclasses and interfaces during event type construction for bean events,
	        // such that superclasses and interfaces can use the legacy type definitions.
	        Map<String, ConfigurationEventTypeLegacy> classLegacyInfo = new HashMap<String, ConfigurationEventTypeLegacy>();
	        foreach (KeyValuePair<String, String> entry in configSnapshot.EventTypeAliases) {
	            String aliasName = entry.Key;
	            String className = entry.Value;
	            ConfigurationEventTypeLegacy legacyDef = configSnapshot.EventTypesLegacy.Get(aliasName);
	            if (legacyDef != null) {
	                classLegacyInfo[className] = legacyDef;
	            }
	        }
	        eventAdapterService.TypeLegacyConfigs = classLegacyInfo;
	        eventAdapterService.DefaultPropertyResolutionStyle =
	            configSnapshot.EngineDefaults.EventMeta.ClassPropertyResolutionStyle;
	        eventAdapterService.AliasResolver = configSnapshot.EngineDefaults.EventMeta.AliasResolver;
	        foreach (string @namespace in configSnapshot.EventTypeAutoAliasPackages) {
	            eventAdapterService.AddAutoAliasPackage(@namespace);
	        }

	        // Add from the configuration the event class aliases
	        IDictionary<String, String> typeAliases = configSnapshot.EventTypeAliases;
	        foreach (KeyValuePair<String, String> entry in typeAliases) {
	            // Add type alias
	            try {
	                String aliasName = entry.Key;
	                eventAdapterService.AddBeanType(aliasName, entry.Value, false);
	            } catch (EventAdapterException ex) {
	                throw new ConfigurationException("Error configuring engine: " + ex.Message, ex);
	            }
	        }

	        // Add from the configuration the XML DOM aliases and type def
	        IDictionary<String, ConfigurationEventTypeXMLDOM> xmlDOMAliases = configSnapshot.EventTypesXMLDOM;
	        foreach (KeyValuePair<String, ConfigurationEventTypeXMLDOM> entry in xmlDOMAliases) {
	            // Add type alias
	            try {
	                eventAdapterService.AddXMLDOMType(entry.Key, entry.Value);
	            } catch (EventAdapterException ex) {
	                throw new ConfigurationException("Error configuring engine: " + ex.Message, ex);
	            }
	        }

	        // Add map event types
	        IDictionary<String, Properties> mapAliases = configSnapshot.EventTypesMapEvents;
	        foreach (KeyValuePair<String, Properties> entry in mapAliases) {
	            try {
	                Map<String, Type> propertyTypes = CreatePropertyTypes(entry.Value);
	                eventAdapterService.AddMapType(entry.Key, propertyTypes);
	            } catch (EventAdapterException ex) {
	                throw new ConfigurationException("Error configuring engine: " + ex.Message, ex);
	            }
	        }

	        // Add nestable map event types
	        Map<String, Map<String, Object>> nestableMapAliases = configSnapshot.EventTypesNestableMapEvents;
	        foreach (KeyValuePair<String, Map<String, Object>> entry in nestableMapAliases) {
	            try {
	                eventAdapterService.AddNestableMapType(entry.Key, entry.Value);
	            } catch (EventAdapterException ex) {
	                throw new ConfigurationException("Error configuring engine: " + ex.Message, ex);
	            }
	        }


	        // Add plug-in event representations
	        Map<Uri, ConfigurationPlugInEventRepresentation> plugInReps = configSnapshot.PlugInEventRepresentation;
	        foreach (KeyValuePair<Uri, ConfigurationPlugInEventRepresentation> entry in plugInReps) {
	            String className = entry.Value.EventRepresentationTypeName;
	            Type eventRepClass;
	            try {
	                eventRepClass = TypeHelper.ResolveType(className, true);
	            } catch (TypeLoadException ex) {
	                throw new ConfigurationException(
	                    "Failed to load plug-in event representation class '" + className + "'", ex);
	            }

	            Object pluginEventRepObj;
	            try {
	                pluginEventRepObj = Activator.CreateInstance(eventRepClass);
	            } catch (TargetInvocationException ex) {
	                throw new ConfigurationException(
	                    "Failed to instantiate plug-in event representation class '" + className +
	                    "' via default constructor",
	                    ex);
	            } catch (MethodAccessException ex) {
	                throw new ConfigurationException(
	                    "Illegal access to instantiate plug-in event representation class '" + className +
	                    "' via default constructor",
	                    ex);
	            } catch (MemberAccessException ex) {
	                throw new ConfigurationException(
	                    "Illegal access to instantiate plug-in event representation class '" + className +
	                    "' via default constructor",
	                    ex);
	            }

	            if (!(pluginEventRepObj is PlugInEventRepresentation)) {
	                throw new ConfigurationException("Plug-in event representation class '" + className +
	                                                 "' does not implement the required interface " +
	                                                 typeof (PlugInEventRepresentation).FullName);
	            }

	            Uri eventRepURI = entry.Key;
	            PlugInEventRepresentation pluginEventRep = (PlugInEventRepresentation) pluginEventRepObj;
	            Object initializer = entry.Value.Initializer;
	            PlugInEventRepresentationContext context =
	                new PlugInEventRepresentationContext(eventAdapterService, eventRepURI, initializer);

	            try {
	                pluginEventRep.Init(context);
	                eventAdapterService.AddEventRepresentation(eventRepURI, pluginEventRep);
	            } catch (Exception ex) {
	                throw new ConfigurationException(
	                    "Plug-in event representation class '" + className + "' and URI '" + eventRepURI +
	                    "' did not initialize correctly : " + ex.Message,
	                    ex);
	            }
	        }

	        // Add plug-in event type aliases
	        Map<String, ConfigurationPlugInEventType> plugInAliases = configSnapshot.PlugInEventTypes;
	        foreach (KeyValuePair<String, ConfigurationPlugInEventType> entry in plugInAliases)
	        {
	            String alias = entry.Key;
	            ConfigurationPlugInEventType config = entry.Value;
	            eventAdapterService.AddPlugInEventType(alias, config.EventRepresentationResolutionURIs, config.Initializer);
	        }
	    }

	    /// <summary>Constructs the auto import service.</summary>
	    /// <param name="configSnapshot">config info</param>
	    /// <returns>service</returns>
        protected static EngineImportService MakeEngineImportService(ConfigurationInformation configSnapshot)
	    {
            EngineImportServiceImpl engineImportService = new EngineImportServiceImpl();
            engineImportService.AddMethodRefs(configSnapshot.MethodInvocationReferences);

	        // Add auto-imports
	        try
	        {
	            foreach (String importName in configSnapshot.Imports)
	            {
	                engineImportService.AddImport(importName);
	            }

                foreach (ConfigurationPlugInAggregationFunction config in configSnapshot.PlugInAggregationFunctions)
	            {
	                engineImportService.AddAggregation(config.Name, config.FunctionClassName);
	            }
	        }
	        catch (EngineImportException ex)
	        {
	            throw new ConfigurationException("Error configuring engine: " + ex.Message, ex);
	        }

	        return engineImportService;
	    }

	    /// <summary>Creates the database config service.</summary>
	    /// <param name="configSnapshot">is the config snapshot</param>
	    /// <param name="schedulingService">is the timer stuff</param>
	    /// <returns>database config svc</returns>
        protected static DatabaseConfigService MakeDatabaseRefService(ConfigurationInformation configSnapshot,
	                                                                  SchedulingService schedulingService)
	    {
	        DatabaseConfigService databaseConfigService = null;

	        // Add auto-imports
	        try
	        {
	            ScheduleBucket allStatementsBucket = schedulingService.AllocateBucket();
                databaseConfigService = new DatabaseConfigServiceImpl(configSnapshot.DatabaseReferences, schedulingService, allStatementsBucket);
	        }
	        catch (ArgumentException ex)
	        {
	            throw new ConfigurationException("Error configuring engine: " + ex.Message, ex);
	        }

	        return databaseConfigService;
	    }

	    private static Map<String, Type> CreatePropertyTypes(Properties properties)
	    {
	        Map<string, Type> propertyTypes = new HashMap<string, Type>();
            foreach ( KeyValuePair<string, string> propEntry in properties )
	        {
	            string property = propEntry.Key;
	            string typeName = propEntry.Value;
                Type type = TypeHelper.GetTypeForSimpleName(typeName);
                propertyTypes[property] = type;
	        }
	        return propertyTypes;
	    }
	}
} // End of namespace
