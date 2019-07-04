using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using com.espertech.esper.client;
using com.espertech.esper.epl.named;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.filter;
using com.espertech.esper.plugin;
using com.espertech.esper.schedule;
using com.espertech.esper.timer;
using com.espertech.esper.util;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Service provider encapsulates the engine's services for runtime and administration interfaces.
    /// </summary>

    public class EPServiceProviderImpl : EPServiceProviderSPI
    {
        private EPServiceEngine engine;
        private ConfigurationInformation configSnapshot;
		private readonly string engineURI;

		/// <summary>
		/// Gets the engine URI
		/// </summary>

        public virtual string URI
		{
			get { return engineURI; }
		}
		
		/// <summary>
		/// Sets the configuration
		/// </summary>

        public virtual Configuration Configuration
		{
			set { configSnapshot = TakeSnapshot( value ) ; }
		}

        /// <summary>
        /// Get the EventAdapterService for this engine.
        /// </summary>
        /// <value></value>
        /// <returns>the EventAdapterService</returns>
		public EventAdapterService EventAdapterService
	    {
	        get { return engine.Services.EventAdapterService; }
	    }

        /// <summary>
        /// Get the SchedulingService for this engine.
        /// </summary>
        /// <value></value>
        /// <returns>the SchedulingService</returns>
	    public SchedulingService SchedulingService
	    {
	        get { return engine.Services.SchedulingService; }
	    }

        /// <summary>
        /// Returns the filter service.
        /// </summary>
        /// <value></value>
        /// <returns>filter service</returns>
	    public FilterService FilterService
	    {
	        get { return engine.Services.FilterService; }
	    }

        public TimerService TimerService
        {
            get { return engine.Services.TimerService; }
        }

        public ConfigurationInformation ConfigurationInformation
        {
            get { return configSnapshot; }
        }

        public NamedWindowService NamedWindowService
        {
            get { return engine.Services.NamedWindowService; }
        }

        /// <summary>
        /// Gets the extension services context.
        /// </summary>
        /// <value>The extension services context.</value>
        public ExtensionServicesContext ExtensionServicesContext
        {
            get { return engine.Services.ExtensionServicesContext; }
        }

        public StatementLifecycleSvc StatementLifecycleSvc
        {
            get { return engine.Services.StatementLifecycleSvc; }
        }

        /// <summary>
        /// Returns the engine environment directory for engine-external
        /// resources such as adapters.
        /// </summary>
        /// <value></value>
        /// <returns>engine environment directory</returns>
	    public Directory EnvDirectory
	    {
	        get { return engine.Services.EngineDirectory; }
	    }

        /// <summary>
        /// Returns a class instance of EPRuntime.
        /// </summary>
        /// <value></value>
        /// <returns> an instance of EPRuntime
        /// </returns>
        virtual public EPRuntime EPRuntime
        {
            get { return engine.Runtime; }
        }

        /// <summary>
        /// Returns a class instance of EPAdministrator.
        /// </summary>
        /// <value></value>
        /// <returns> an instance of EPAdministrator
        /// </returns>
        virtual public EPAdministrator EPAdministrator
        {
            get { return engine.Admin; }
        }

        /// <summary> Constructor - initializes services.</summary>
        /// <param name="configuration">is the engine configuration</param>
		/// <param name="engineURI">is the engine URI or null if this is the default provider</param>
        /// <throws>  ConfigurationException is thrown to indicate a configuraton error </throws>

        public EPServiceProviderImpl(Configuration configuration, String engineURI)
        {
			this.engineURI = engineURI;
            this.configSnapshot = TakeSnapshot(configuration);
            Initialize();
        }

        public virtual void Destroy()
        {
            if (engine != null)
            {
                engine.Services.TimerService.StopInternalClock(false);
                // Give the timer thread a little moment to catch up
                Thread.Sleep(100);

                engine.Runtime.Destroy();

                // plugin-loaders
                IList<ConfigurationPluginLoader> pluginLoaders = configSnapshot.PluginLoaders;
                foreach (ConfigurationPluginLoader config in pluginLoaders) {
                    PluginLoader plugin;
                    plugin = (PluginLoader) engine.Services.EngineDirectory.Lookup("plugin-loader/" + config.LoaderName);
                    plugin.Destroy();
                }

                engine.Admin.Destroy();
                engine.Services.Destroy();
                engine.Services.Initialize();
            }

            engine = null;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is destroyed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is destroyed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDestroyed
        {
            get { return engine == null; }
        }

        /// <summary>
        /// Frees any resources associated with this runtime instance.
        /// Stops and destroys any event filters, patterns, expressions, views.
        /// </summary>
        public void Initialize()
        {
            // This setting applies to all engines in a given AppDomain
            ExecutionPathDebugLog.isDebugEnabled = configSnapshot.EngineDefaults.Logging.IsEnableExecutionDebug;

            if (engine != null)
            {
                engine.Services.TimerService.StopInternalClock(false);

                // Give the timer thread a little moment to catch up
                // AJ: The TimerService already sleeps to allow for catch up.
                //try
                //{
                //   Thread.Sleep( 100 ) ;
                //}
                //catch (ThreadInterruptedException)
                //{
                    // No logic required here
                //}
				
				engine.Services.Destroy();
            }

	        // Make EP services context factory
	        String epServicesContextFactoryClassName = configSnapshot.EPServicesContextFactoryClassName;
	        EPServicesContextFactory epServicesContextFactory;
	        if (epServicesContextFactoryClassName == null)
	        {
	            // Check system properties
	            epServicesContextFactoryClassName = Environment.GetEnvironmentVariable("ESPER_EPSERVICE_CONTEXT_FACTORY_CLASS");
	        }

	        if (epServicesContextFactoryClassName == null)
	        {
	            epServicesContextFactory = new EPServicesContextFactoryDefault();
	        }
	        else
	        {
	            Type type;
	            try
	            {
                    type = TypeHelper.ResolveType(epServicesContextFactoryClassName);
	            }
	            catch (TypeLoadException)
	            {
	                throw new ConfigurationException("Class '" + epServicesContextFactoryClassName + "' cannot be loaded");
	            }

	            try
	            {
                    epServicesContextFactory = (EPServicesContextFactory)Activator.CreateInstance(type);
	            }
	            catch (TypeLoadException)
	            {
	                throw new ConfigurationException("Type '" + type + "' cannot be instantiated");
	            }
	            catch (MethodAccessException)
	            {
	                throw new ConfigurationException("Illegal access instantiating type '" + type);
	            }
	        }

            EPServicesContext services = epServicesContextFactory.CreateServicesContext(this, configSnapshot);

            // New runtime
            EPRuntimeImpl runtime = new EPRuntimeImpl(services);

            // Configure services to use the new runtime
            services.InternalEventRouter = runtime;
            services.TimerService.Callback = runtime.TimerCallback;

            // Statement lifycycle init
            services.StatementLifecycleSvc.Init();

            // New admin
            ConfigurationOperations configOps = new ConfigurationOperationsImpl(services.EventAdapterService, services.EngineImportService, services.VariableService, services.EngineSettingsService, services.ValueAddEventService);
            SelectClauseStreamSelectorEnum defaultStreamSelector = SelectClauseStreamSelectorHelper.MapFromSODA(configSnapshot.EngineDefaults.StreamSelection.DefaultStreamSelector);
            EPAdministratorImpl admin = new EPAdministratorImpl(services, configOps, defaultStreamSelector);

            // Start clocking
            if (configSnapshot.EngineDefaults.Threading.IsInternalTimerEnabled)
            {
                services.TimerService.StartInternalClock();
            }

            // Give the timer thread a little moment to Start up
            try
            {
                Thread.Sleep( 100 ) ;
            }
            catch (ThreadInterruptedException)
            {
                // No logic required here
            }

            // Save engine instance
            engine = new EPServiceEngine(services, runtime, admin);

            // Load and initialize adapter loader classes
            LoadAdapters(services);

            // Initialize extension services
            if (services.ExtensionServicesContext != null)
            {
                services.ExtensionServicesContext.Init();
            }
        }

        /// <summary>
        /// Loads and initializes adapter loaders.
        /// </summary>
        /// <param name="services">the engine instance services</param>
        private void LoadAdapters(EPServicesContext services)
	    {
            IList<ConfigurationPluginLoader> pluginLoaders = configSnapshot.PluginLoaders;
	        if ((pluginLoaders == null) || (pluginLoaders.Count == 0))
	        {
	            return;
	        }
            foreach (ConfigurationPluginLoader config in pluginLoaders)
	        {
	            String typeName = config.TypeName;
	            Type pluginLoaderClass;
	            try
	            {
	                pluginLoaderClass = TypeHelper.ResolveType(typeName);
	            }
	            catch (TypeLoadException ex)
	            {
	                throw new ConfigurationException("Failed to load adapter loader class '" + typeName + "'", ex);
	            }

	            Object pluginLoaderObj;
	            try
	            {
	                pluginLoaderObj = Activator.CreateInstance(pluginLoaderClass);
	            }
	            catch (MethodAccessException ex)
	            {
	                throw new ConfigurationException("Illegal access to instantiate adapter loader class '" + typeName + "' via default constructor", ex);
	            }

                PluginLoader pluginLoader = (PluginLoader)pluginLoaderObj;
	            pluginLoader.Init(config.LoaderName, config.ConfigProperties, this);

	            // register adapter loader in JNDI context tree
	            try
	            {
	                services.EngineDirectory.Bind("adapter-loader/" + config.LoaderName, pluginLoader);
	            }
	            catch (DirectoryException e)
	            {
	                throw new EPException("Failed to use context to bind adapter loader", e);
	            }
	        }
		}


        private static ConfigurationInformation TakeSnapshot(Configuration configuration)
        {
            try
            {
                return (ConfigurationInformation)SerializableObjectCopier.Copy(configuration);
            }
            catch (IOException e)
            {
                throw new ConfigurationException("Failed to snapshot configuration instance through serialization : " + e.Message, e);
            }
            catch (TypeLoadException e)
            {
                throw new ConfigurationException("Failed to snapshot configuration instance through serialization : " + e.Message, e);
            }
        }

        private class EPServiceEngine
        {
            virtual public EPServicesContext Services
            {
                get { return services; }
            }

            virtual public EPRuntimeImpl Runtime
            {
                get { return runtime; }
            }

            virtual public EPAdministratorImpl Admin
            {
                get { return admin; }
            }

            private readonly EPServicesContext services;
            private readonly EPRuntimeImpl runtime;
            private readonly EPAdministratorImpl admin;

            public EPServiceEngine(EPServicesContext services, EPRuntimeImpl runtime, EPAdministratorImpl admin)
            {
                this.services = services;
                this.runtime = runtime;
                this.admin = admin;
            }
        }
    }
}
