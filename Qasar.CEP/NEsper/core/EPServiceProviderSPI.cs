///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using com.espertech.esper.client;
using com.espertech.esper.epl.named;
using com.espertech.esper.events;
using com.espertech.esper.filter;
using com.espertech.esper.schedule;
using com.espertech.esper.timer;

namespace com.espertech.esper.core
{
	/// <summary>
	/// A service provider interface that makes available internal engine services.
	/// </summary>
	public interface EPServiceProviderSPI : EPServiceProvider
	{
        /// <summary>Returns statement management service for the engine.</summary>
        /// <returns>the StatementLifecycleSvc</returns>
        StatementLifecycleSvc StatementLifecycleSvc { get; }

	    /// <summary>Get the EventAdapterService for this engine.</summary>
	    /// <returns>the EventAdapterService</returns>
        EventAdapterService EventAdapterService { get; }

	    /// <summary>Get the SchedulingService for this engine.</summary>
	    /// <returns>the SchedulingService</returns>
        SchedulingService SchedulingService { get; }

	    /// <summary>Returns the filter service.</summary>
	    /// <returns>filter service</returns>
        FilterService FilterService { get ; }

        /// <summary>Returns the timer service.</summary>
        /// <returns>timer service</returns>
        TimerService TimerService { get; }

	    /// <summary>Returns the named window service.</summary>
	    /// <returns>named window service</returns>
	    NamedWindowService NamedWindowService { get; }

	    /// <summary>Returns the current configuration.</summary>
        /// <returns>configuration information</returns>
        ConfigurationInformation ConfigurationInformation { get; }

#if false
	    /// <summary>
	    /// Returns the engine environment directory for engine-external
	    /// resources such as adapters.
	    /// </summary>
	    /// <returns>engine environment directory</returns>
	    //Directory EnvDirectory { get; }
#endif

        /// <summary>
        /// Gets the extension services context.
        /// </summary>
        ExtensionServicesContext ExtensionServicesContext { get; }
	}

    public class EPServiceProviderConstants
    {
        /// <summary>
        /// For the default provider instance, which carries a null provider URI,
        /// the property name qualification and stream name qualification may use
        /// "default".</summary>
        public const String DEFAULT_ENGINE_URI__QUALIFIER = "default";
    }
} // End of namespace
