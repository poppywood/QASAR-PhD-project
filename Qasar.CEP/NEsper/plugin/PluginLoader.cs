///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.compat;
using com.espertech.esper.core;

namespace com.espertech.esper.plugin
{
    /// <summary>
    /// Interface for loaders of input/output adapters.
    /// </summary>
	public interface PluginLoader
	{
	    /// <summary>Initializes the adapter loader.</summary>
	    /// <param name="name">is the loader name</param>
	    /// <param name="properties">is a set of properties from the configuration</param>
	    /// <param name="epService">is the SPI of the engine itself for sending events to</param>
	    void Init(String name, Properties properties, EPServiceProviderSPI epService);

	    /// <summary>Destroys adapter loader and adapters loaded.</summary>
	    void Destroy();
	}
} // End of namespace
