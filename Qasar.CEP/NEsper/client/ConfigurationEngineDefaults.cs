using System;
using System.Collections.Generic;
using System.Text;

namespace net.esper.client
{
    /// <summary>
    /// Provides access to engine configuration defaults for modification.
    /// </summary>

    public class ConfigurationEngineDefaults
    {
        private readonly ThreadSettings threading;
        private readonly ViewResourceSettings viewResources;
        private readonly EventMetaSettings eventMeta;
        private readonly LogSettings logging;

        /// <summary>Ctor.</summary>
        internal ConfigurationEngineDefaults()
        {
            threading = new ThreadSettings();
            viewResources = new ViewResourceSettings();
            eventMeta = new EventMetaSettings();
            logging = new LogSettings();
        }

        /// <summary>Returns threading settings.</summary>
        /// <returns>threading settings object</returns>
        public ThreadSettings Threading
        {
            get { return threading; }
        }

        /// <summary>Returns view resources defaults.</summary>
        /// <returns>view resources defaults</returns>
        public ViewResourceSettings ViewResources
        {
            get { return viewResources; }
        }

        /// <summary>Returns event representation default settings.</summary>
        /// <returns>event representation default settings</returns>
        public EventMetaSettings EventMeta
        {
            get { return eventMeta; }
        }

        /// <summary>
        /// Returns logging settings applicable to the engine, other then Log4J settings.
        /// </summary>
        /// <returns>logging settings</returns>
        public LogSettings Logging
        {
            get { return logging; }
        }

        /// <summary>Holds thread settings.</summary>
        public class ThreadSettings
        {
            private bool isListenerDispatchPreserveOrder;
            private long listenerDispatchTimeout;
            private bool isInsertIntoDispatchPreserveOrder;
            private long internalTimerMsecResolution;
            private bool internalTimerEnabled;

            /// <summary>Ctor - sets up defaults.</summary>
            internal ThreadSettings()
            {
                listenerDispatchTimeout = 1000;
                isListenerDispatchPreserveOrder = true;
                isInsertIntoDispatchPreserveOrder = true;
                internalTimerEnabled = true;
                internalTimerMsecResolution = 100;
            }

            /// <summary>
            /// In multithreaded environments, this setting controls whether dispatches to listeners preserve
            /// the ordering in which the statement processes events.
            /// </summary>
            /// <param name="value">is true to preserve ordering, or false if not</param>
            public bool IsListenerDispatchPreserveOrder
            {
                get { return isListenerDispatchPreserveOrder; }
                set { isListenerDispatchPreserveOrder = value; }
            }

            /// <summary>
            /// In multithreaded environments, this setting controls when dispatches to listeners preserve
            /// the ordering the timeout to complete any outstanding dispatches.
            /// </summary>
            /// <value>The listener dispatch timeout.</value>
            public long ListenerDispatchTimeout
            {
                get { return listenerDispatchTimeout; }
                set { listenerDispatchTimeout = value; }
            }

            /// <summary>
            /// In multithreaded environments, this setting controls whether insert-into streams preserve
            /// the order of events inserted into them by one or more statements
            /// such that statements that consume other statement's events behave deterministic
            /// </summary>
            public bool IsInsertIntoDispatchPreserveOrder
            {
                get { return isInsertIntoDispatchPreserveOrder; }
                set { isInsertIntoDispatchPreserveOrder = value; }
            }

            /// <summary>
            /// Sets the use of internal timer.
            /// <para>
            /// By setting internal timer to true (the default) the engine starts the internal timer thread
            /// and relies on internal timer events to supply the time.
            /// </para>
            /// <para>
            /// By setting internal timer to false the engine does not start the internal timer thread
            /// and relies on external application-supplied timer events to supply the time.
            /// </para>
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is internal timer enabled; otherwise, <c>false</c>.
            /// </value>
            public bool IsInternalTimerEnabled
            {
                get { return this.internalTimerEnabled; }
                set { this.internalTimerEnabled = value; }
            }

            /// <summary>Returns the millisecond resolutuion of the internal timer thread.</summary>
            /// <returns>number of msec between timer processing intervals</returns>
            public long InternalTimerMsecResolution
            {
                get { return internalTimerMsecResolution; }
                set { internalTimerMsecResolution = value; }
            }
        }

        /// <summary>Holds view resources settings.</summary>
        public class ViewResourceSettings
        {
            private bool shareViews;

            /// <summary>Ctor - sets up defaults.</summary>
            internal ViewResourceSettings()
            {
                shareViews = true;
            }

            /// <summary>
            /// Gets or sets the flag to instruct the engine shares view resources between statements,
            /// or if false to instruct the engine to not share view resources between statements.
            /// </summary>
            /// <returns>
            /// indicator whether view resources are shared between statements if
            /// statements share same-views and the engine sees opportunity to reuse an existing view.
            /// </returns>
            public bool IsShareViews
            {
                get { return shareViews; }
                set { this.shareViews = value; }
            }
        }

        /// <summary>Event representation metadata.</summary>
        public class EventMetaSettings
        {
            private PropertyResolutionStyle classPropertyResolutionStyle;

            /// <summary>Ctor.</summary>
            public EventMetaSettings()
            {
                this.classPropertyResolutionStyle = PropertyResolutionStyleHelper.DefaultPropertyResolutionStyle;
            }

            /// <summary>
            /// Gets or sets the property resolution style to use for resolving property names
            /// of types.
            /// </summary>
            public PropertyResolutionStyle ClassPropertyResolutionStyle
            {
                get { return classPropertyResolutionStyle; }
                set { classPropertyResolutionStyle = value; }
            }
        }

        /// <summary>
        /// Holds view logging settings other then the Apache commons or Log4net settings.
        /// </summary>
        public class LogSettings
        {
            private bool enableExecutionDebug;

            /// <summary>Ctor - sets up defaults.</summary>
            internal LogSettings()
            {
                enableExecutionDebug = false;
            }

            /// <summary>
            /// Gets or sets a value indicating if execution path debug logging is enabled.
            /// <para>
            /// Only if this flag is set to true, in addition to log4net settings set to DEBUG,
            /// does an engine instance, produce debug out.
            /// </para>
            /// </summary>
            public bool IsEnableExecutionDebug
            {
                get { return enableExecutionDebug; }
                set { enableExecutionDebug = value; }
            }
        }
    }
}
