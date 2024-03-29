///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Service for engine-level settings around threading and concurrency.
    /// </summary>
    public class EngineSettingsService
    {
        private readonly ConfigurationEngineDefaults config;
        private IList<Uri> plugInEventTypeResolutionURIs;

        /// <summary>Ctor.</summary>
        /// <param name="config">is the configured defaults</param>
        /// <param name="plugInEventTypeResolutionURIs">is URIs for resolving the event name against plug-inn event representations, if any</param>
        public EngineSettingsService(ConfigurationEngineDefaults config, IList<Uri> plugInEventTypeResolutionURIs)
        {
            this.config = config;
            this.plugInEventTypeResolutionURIs = plugInEventTypeResolutionURIs;
        }

        /// <summary>
        /// Gets the engine settings.
        /// </summary>
        /// <value>The engine settings.</value>
        public ConfigurationEngineDefaults EngineSettings
        {
            get { return config; }
        }

        /// <summary>
        /// Gets or sets the URIs for resolving the event name against plug-in event representations, if any.
        /// </summary>
        /// <value>The plug in event type resolution UR is.</value>
        /// <returns>URIs</returns>
        public IList<Uri> PlugInEventTypeResolutionURIs
        {
            get { return plugInEventTypeResolutionURIs; }
            set { this.plugInEventTypeResolutionURIs = value; }
        }
    }
}
