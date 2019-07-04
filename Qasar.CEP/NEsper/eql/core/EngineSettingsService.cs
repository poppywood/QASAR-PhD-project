///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using net.esper.client;

namespace net.esper.eql.core
{
    /// <summary>
    /// Service for engine-level settings around threading and concurrency.
    /// </summary>
    public class EngineSettingsService
    {
        private readonly ConfigurationEngineDefaults config;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineSettingsService"/> class.
        /// </summary>
        /// <param name="config">The config.</param>
        public EngineSettingsService(ConfigurationEngineDefaults config)
        {
            this.config = config;
        }

        /// <summary>
        /// Gets the engine settings.
        /// </summary>
        /// <value>The engine settings.</value>
        public ConfigurationEngineDefaults EngineSettings
        {
            get { return config; }
        }
    }
}