///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;

using net.esper.client;
using net.esper.util;

namespace net.esper.support.client
{
	public class SupportConfigFactory
	{
	    private const String TEST_CONFIG_FACTORY_CLASS = "TEST_CONFIG_FACTORY_CLASS";

	    public static Configuration Configuration
	    {
            get
	        {
	            Configuration config;
	            String configFactoryClass =
	                System.Configuration.ConfigurationManager.AppSettings[TEST_CONFIG_FACTORY_CLASS];
	            if (configFactoryClass != null)
	            {
	                try
	                {
	                    Type type = TypeHelper.ResolveType(configFactoryClass);
	                    Object instance = Activator.CreateInstance(type);
	                    PropertyInfo p = type.GetProperty("Configuration");
	                    Object result = p.GetValue(instance, null);
	                    config = (Configuration) result;
	                }
	                catch (Exception e)
	                {
	                    throw new ApplicationException(
	                        "Error using configuration factory class '" + configFactoryClass + "'", e);
	                }
	            }
	            else
	            {
	                config = new Configuration();
	                config.EngineDefaults.Logging.IsEnableExecutionDebug = true;
	            }
	            return config;
	        }
	    }
	}
} // End of namespace
