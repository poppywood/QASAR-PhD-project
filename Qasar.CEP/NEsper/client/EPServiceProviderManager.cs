// ************************************************************************************
// Copyright (C) 2006 Thomas Bernhardt. All rights reserved.                          *
// http://esper.codehaus.org                                                          *
// ---------------------------------------------------------------------------------- *
// The software in this package is published under the terms of the GPL license       *
// a copy of which has been included with this distribution in the license.txt file.  *
// ************************************************************************************

using System;
using System.Collections.Generic;

using net.esper.compat;
using net.esper.core;

namespace net.esper.client
{
    /// <summary>
    /// Factory for instances of <seealso cref="EPServiceProvider" />.
    /// </summary>

    public sealed class EPServiceProviderManager
    {
        private static MonitorLock lockObj;
        private static readonly Dictionary<String, EPServiceProviderImpl> runtimes;
        private static EPServiceProviderImpl defaultServiceProvider;

        static EPServiceProviderManager()
        {
            lockObj = new MonitorLock();
            runtimes = new Dictionary<string, EPServiceProviderImpl>();
            defaultServiceProvider = null;
        }

        /// <summary> Returns the default EPServiceProvider.</summary>
        /// <returns> default instance of the service.
        /// </returns>

        public static EPServiceProvider GetDefaultProvider()
        {
            return GetProvider(null, new Configuration());
        }

        /// <summary> Returns the default EPServiceProvider.</summary>
        /// <param name="configuration">is the configuration for the service
        /// </param>
        /// <returns> default instance of the service.
        /// </returns>
        /// <throws>  ConfigurationException to indicate a configuration problem </throws>

        public static EPServiceProvider GetDefaultProvider(Configuration configuration)
        {
            return GetProvider(null, configuration);
        }

        /// <summary> Returns an EPServiceProvider for a given registration URI.</summary>
        /// <param name="uri">the registration URI
        /// </param>
        /// <returns> EPServiceProvider for the given registration URI.
        /// </returns>

        public static EPServiceProvider GetProvider(String uri)
        {
            return GetProvider(uri, new Configuration());
        }

        /// <summary> Returns an EPServiceProvider for a given registration URI.</summary>
        /// <param name="uri">the registration URI
        /// </param>
        /// <param name="configuration">is the configuration for the service
        /// </param>
        /// <returns> EPServiceProvider for the given registration URI.
        /// </returns>
        /// <throws>  ConfigurationException to indicate a configuration problem </throws>

        public static EPServiceProvider GetProvider(String uri, Configuration configuration)
        {
            using(lockObj.Acquire())
            {
                if (String.IsNullOrEmpty(uri))
                {
                    if (defaultServiceProvider == null)
                    {
                        defaultServiceProvider = new EPServiceProviderImpl(configuration, uri);
                    }

                    defaultServiceProvider.Configuration = configuration;
                    return defaultServiceProvider;
                }

                if (runtimes.ContainsKey(uri))
                {
					EPServiceProviderImpl provider = runtimes[uri];
					provider.Configuration = configuration;
					return provider;					
                }

                // New runtime
                EPServiceProviderImpl runtime = new EPServiceProviderImpl(configuration, uri);
                runtimes[uri] = runtime;

                return runtime;
            }
        }

        /// <summary>
        /// Clears references to the provider.
        /// </summary>
        /// <param name="uri"></param>

        public static void PurgeProvider( String uri )
        {
            using(lockObj.Acquire())
            {
                if (String.IsNullOrEmpty(uri))
                {
                    defaultServiceProvider = null;
                    return ;
                }

                runtimes.Remove(uri);
            }
        }

        /// <summary>
        /// Clears references to the default provider.
        /// </summary>

        public static void PurgeDefaultProvider()
        {
            PurgeProvider(null);
        }

        /// <summary>
        /// Purges all providers.
        /// </summary>
        public static void PurgeAllProviders()
        {
            using (lockObj.Acquire())
            {
                runtimes.Clear();
                defaultServiceProvider = null;
            }
        }
    }
}
