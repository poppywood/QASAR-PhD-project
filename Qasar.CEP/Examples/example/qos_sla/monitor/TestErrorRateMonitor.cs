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
	public class TestErrorRateMonitor
	{
	    private EPRuntime runtime;

	    [SetUp]
	    public void SetUp()
	    {
            Configuration configuration = new Configuration();
            configuration.EngineDefaults.EventMeta.ClassPropertyResolutionStyle = PropertyResolutionStyle.CASE_INSENSITIVE;

            EPServiceProviderManager.PurgeDefaultProvider();
            EPServiceProvider epService = EPServiceProviderManager.GetDefaultProvider(configuration);

	        new ErrorRateMonitor();
	        runtime = epService.EPRuntime;
	    }

	    [Test]
	    public void TestAlert()
	    {
	        for (int i= 0; i < 5; i++)
	        {
	            SendEvent(false);
	        }

	        //sleep(11000);

	        for (int i= 0; i < 4; i++)
	        {
	            SendEvent(false);
	        }

	        //sleep(11000);
	        //sleep(11000);
	    }

	    private void SendEvent(bool success)
	    {
	        OperationMeasurement measurement = new OperationMeasurement("myService", "myCustomer", 10000, success);
	        runtime.SendEvent(measurement);
	    }
	}
} // End of namespace
