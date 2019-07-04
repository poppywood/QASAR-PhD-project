///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using net.esper.client;
using net.esper.events;

using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.regression.view
{
    [TestFixture]
	public class TestGroupedTimeWinUniqueSortMinMax
    {
        [SetUp]
	    private Configuration Setup()
	    {
	        Configuration config = new Configuration();
	        config.AddEventTypeAlias("Sensor", typeof(Sensor));
	        return config;
	    }

	    private void logEvent (Object _event) {
	        log.Info("Sending " + _event);
	    }

        [Test]
	    public void testSensorQuery()
        {
	        log.Info ("testSensorQuery...........");
	        // TODO: Esper 125
	        /*
	        Configuration configuration = Setup();

	        EPServiceProvider epService = EPServiceProviderManager.GetProvider("testSensorQuery", configuration);
	        MatchListener listener = new MatchListener();

	        String stmtString =
	              "SELECT Max(high.type) as type, \n" +
	              " Max(high.measurement) as highMeasurement, Max(high.confidence) as confidenceOfHigh, Max(high.device) as deviceOfHigh\n" +
	              ",Min(low.measurement) as lowMeasurement, Min(low.confidence) as confidenceOfLow, Min(low.device) as deviceOfLow\n" +
	              "FROM\n" +
	              " Sensor.std:groupby('type').win:time(1 hour).std:unique('device').ext:sort('measurement',true,1) as high " +
	              ",Sensor.std:groupby('type').win:time(1 hour).std:unique('device').ext:sort('measurement',false,1) as low ";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtString);
	        log.Info(stmtString);
	        stmt.AddListener(listener);

	        EPRuntime runtime = epService.EPRuntime;
	        List<Sensor> events = new ArrayList<Sensor>();
	        events.Add(new Sensor("Temperature", "Device1", 68.0, 96.5));
	        events.Add(new Sensor("Temperature", "Device2", 65.0, 98.5));
	        events.Add(new Sensor("Temperature", "Device1", 62.0, 95.3));
	        events.Add(new Sensor("Temperature", "Device2", 71.3, 99.3));
	        foreach (Sensor event in events) {
	            logEvent (event);
	            runtime.SendEvent(event);
	        }
	        Map lastEvent = (Map) listener.GetLastEvent();
	        assertTrue (lastEvent != null);
	        assertEquals (62.0,lastEvent.Get("lowMeasurement"));
	        assertEquals ("Device1",lastEvent.Get("deviceOfLow"));
	        assertEquals (95.3,lastEvent.Get("confidenceOfLow"));
	        assertEquals (71.3,lastEvent.Get("highMeasurement"));
	        assertEquals ("Device2",lastEvent.Get("deviceOfHigh"));
	        assertEquals (99.3,lastEvent.Get("confidenceOfHigh"));
	    */
	    }

        public class Sensor
        {
            public Sensor()
            {
            }

            public Sensor(String type, String device, Double measurement, Double confidence)
            {
                this.type = type;
                this.device = device;
                this.measurement = measurement;
                this.confidence = confidence;
            }

            public string Type
            {
                get { return type; }
                set { type = value; }
            }

            public string Device
            {
                get { return device; }
                set { device = value; }
            }

            public double? Measurement
            {
                get { return measurement; }
                set { measurement = value; }
            }

            public double? Confidence
            {
                get { return confidence; }
                set { confidence = value; }
            }

            private String type;
            private String device;
            private double? measurement;
            private double? confidence;
        }

        public class MatchListener : UpdateListener
        {
            private int count = 0;
            private Object lastEvent = null;

            public void Update(EventBean[] newEvents, EventBean[] oldEvents)
            {
                log.Info("New events.................");
                if (newEvents != null)
                {
                    for (int i = 0; i < newEvents.Length; i++)
                    {
                        EventBean e = newEvents[i];
                        EventType t = e.EventType;
                        List<string> propNames = new List<string>(t.PropertyNames);
                        log.Info("event[" + i + "] of type " + t);
                        for (int j = 0; j < propNames.Count; j++)
                        {
                            log.Info("    " + propNames[j] + ": " + e.Get(propNames[j]));
                        }
                        count++;
                        lastEvent = e.Underlying;
                    }
                }
                log.Info("Removing events.................");
                if (oldEvents != null)
                {
                    for (int i = 0; i < oldEvents.Length; i++)
                    {
                        EventBean e = oldEvents[i];
                        EventType t = e.EventType;
                        List<String> propNames = new List<string>(t.PropertyNames);
                        log.Info("event[" + i + "] of type " + t);
                        for (int j = 0; j < propNames.Count; j++)
                        {
                            log.Info("    " + propNames[j] + ": " + e.Get(propNames[j]));
                        }
                        count--;
                    }
                }
                log.Info("......................................");
            }

            public int GetCount()
            {
                return count;
            }

            public Object GetLastEvent()
            {
                return lastEvent;
            }
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
} // End of namespace
