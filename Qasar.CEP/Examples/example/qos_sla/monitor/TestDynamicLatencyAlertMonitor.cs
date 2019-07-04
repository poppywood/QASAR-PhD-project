///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using NUnit.Framework;

using net.esper.client;
using net.esper.example.qos_sla.eventbean;

namespace net.esper.example.qos_sla.monitor
{
	[TestFixture]
	public class TestDynamicLatencyAlertMonitor
	{
	    private EPRuntime runtime;

	    [SetUp]
	    public void SetUp()
	    {
            Configuration configuration = new Configuration();
            configuration.EngineDefaults.EventMeta.ClassPropertyResolutionStyle = PropertyResolutionStyle.CASE_INSENSITIVE;

            EPServiceProviderManager.PurgeDefaultProvider();
            EPServiceProvider epService = EPServiceProviderManager.GetDefaultProvider(configuration);

	        DynaLatencySpikeMonitor.Start();
	        runtime = epService.EPRuntime;
	    }

	    [Test]
	    public void TestLatencyAlert()
	    {
	        String[] services = {"s0", "s1", "s2"};
	        String[] customers = {"c0", "c1", "c2"};
	        long[] limitSpike = {15000, 10000, 10040};
	        OperationMeasurement measurement;
	        LatencyLimit limit;

	        // Set up limits for 3 services/customer combinations
	        for (int i = 0; i < services.Length; i++)
	        {
	            limit = new LatencyLimit(services[i], customers[i], limitSpike[i]);
	            runtime.SendEvent(limit);
	        }

	        // Send events
	        for (int i = 0; i < 100; i++)
	        {
                for (int index = 0; index < services.Length; index++)
	            {
	                measurement = new OperationMeasurement(services[index], customers[index],
	                        9950 + i, true);
	                runtime.SendEvent(measurement);
	            }
	        }

	        // Send a new limit
	        limit = new LatencyLimit(services[1], customers[1], 8000);
	        runtime.SendEvent(limit);

	        // Send a new spike
	        measurement = new OperationMeasurement(services[1], customers[1], 8001, true);
	        runtime.SendEvent(measurement);
	    }
	}
} // End of namespace
