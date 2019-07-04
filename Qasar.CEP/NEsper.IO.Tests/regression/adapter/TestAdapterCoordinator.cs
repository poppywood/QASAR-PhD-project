///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using net.esper.adapter;
using net.esper.adapter.csv;
using net.esper.client;
using net.esper.client.time;
using net.esper.compat;
using net.esper.events;
using net.esper.support.util;

using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.regression.adapter
{
    [TestFixture]
	public class TestAdapterCoordinator
	{
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private SupportUpdateListener listener;
		private String eventTypeAlias;
		private EPServiceProvider epService;
		private long currentTime;
		private AdapterCoordinator coordinator;
		private CSVInputAdapterSpec timestampsLooping;
		private CSVInputAdapterSpec noTimestampsLooping;
		private CSVInputAdapterSpec noTimestampsNotLooping;
		private CSVInputAdapterSpec timestampsNotLooping;
		private String[] propertyOrderNoTimestamp;

        [SetUp]
		protected void SetUp()
		{
			EDictionary<String, Type> propertyTypes = new LinkedDictionary<String, Type>();
            propertyTypes.Put("myInt", typeof (int));
            propertyTypes.Put("myDouble", typeof (double));
            propertyTypes.Put("myString", typeof (string));

			eventTypeAlias = "mapEvent";
			Configuration configuration = new Configuration();
			configuration.AddEventTypeAlias(eventTypeAlias, propertyTypes);

			epService = EPServiceProviderManager.GetProvider("Adapter", configuration);
			epService.Initialize();
			EPAdministrator administrator = epService.EPAdministrator;
			String statementText = "select * from mapEvent.win:length(5)";
			EPStatement statement = administrator.CreateEQL(statementText);
			listener = new SupportUpdateListener();
			statement.AddListener(listener);

			// Turn off external clocking
			epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));

			// Set the clock to 0
			currentTime = 0;
			SendTimeEvent(0);

			coordinator = new AdapterCoordinatorImpl(epService, true);

	    	propertyOrderNoTimestamp = new String[] { "myInt", "myDouble", "myString" };
	    	String[] propertyOrderTimestamp = new String[] { "timestamp", "myInt", "myDouble", "myString" };

			// A CSVPlayer for a file with timestamps, not looping
			timestampsNotLooping = new CSVInputAdapterSpec(new AdapterInputSource("/regression/timestampOne.csv"), eventTypeAlias);
			timestampsNotLooping.IsUsingEngineThread = true;
			timestampsNotLooping.PropertyOrder = propertyOrderTimestamp;
			timestampsNotLooping.TimestampColumn = "timestamp";

			// A CSVAdapter for a file with timestamps, looping
			timestampsLooping = new CSVInputAdapterSpec(new AdapterInputSource("/regression/timestampTwo.csv"), eventTypeAlias);
			timestampsLooping.IsLooping = true;
			timestampsLooping.IsUsingEngineThread = true;
			timestampsLooping.PropertyOrder = propertyOrderTimestamp;
			timestampsLooping.TimestampColumn = "timestamp";

			// A CSVAdapter that sends 10 events per sec, not looping
			noTimestampsNotLooping = new CSVInputAdapterSpec(new AdapterInputSource("/regression/noTimestampOne.csv"), eventTypeAlias);
			noTimestampsNotLooping.EventsPerSec = 10;
			noTimestampsNotLooping.PropertyOrder = propertyOrderNoTimestamp;
			noTimestampsNotLooping.IsUsingEngineThread =true;

			// A CSVAdapter that sends 5 events per sec, looping
			noTimestampsLooping = new CSVInputAdapterSpec(new AdapterInputSource("/regression/noTimestampTwo.csv"), eventTypeAlias);
			noTimestampsLooping.EventsPerSec = 5;
			noTimestampsLooping.IsLooping = true;
			noTimestampsLooping.PropertyOrder = propertyOrderNoTimestamp;
			noTimestampsLooping.IsUsingEngineThread = true;
		}

        [Test]
		public void testRun()
		{
			coordinator.Coordinate(new CSVInputAdapter(timestampsNotLooping));
			coordinator.Coordinate(new CSVInputAdapter(timestampsLooping));
			coordinator.Coordinate(new CSVInputAdapter(noTimestampsNotLooping));
			coordinator.Coordinate(new CSVInputAdapter(noTimestampsLooping));

			// Time is 0
			Assert.IsFalse(listener.GetAndClearIsInvoked());
			coordinator.Start();

			// Time is 50
			SendTimeEvent(50);

			// Time is 100
			SendTimeEvent(50);
			AssertEvent(0, 1, 1.1, "timestampOne.one");
			AssertEvent(1, 1, 1.1, "noTimestampOne.one");
			AssertSizeAndReset(2);

			// Time is 150
			SendTimeEvent(50);
			Assert.IsFalse(listener.GetAndClearIsInvoked());

			// Time is 200
			SendTimeEvent(50);
			AssertEvent(0, 2, 2.2, "timestampTwo.two");
			AssertEvent(1, 2, 2.2, "noTimestampOne.two");
			AssertEvent(2, 2, 2.2, "noTimestampTwo.two");
			AssertSizeAndReset(3);

			// Time is 250
			SendTimeEvent(50);

			// Time is 300
			SendTimeEvent(50);
			AssertEvent(0, 3, 3.3, "timestampOne.three");
			AssertEvent(1, 3, 3.3, "noTimestampOne.three");
			AssertSizeAndReset(2);

			// Time is 350
			SendTimeEvent(50);
			Assert.IsFalse(listener.GetAndClearIsInvoked());

			coordinator.Pause();

			// Time is 400
			SendTimeEvent(50);
			Assert.IsFalse(listener.GetAndClearIsInvoked());

			// Time is 450
			SendTimeEvent(50);
			Assert.IsFalse(listener.GetAndClearIsInvoked());

			coordinator.Resume();

			AssertEvent(0, 4, 4.4, "timestampTwo.four");
			AssertEvent(1, 4, 4.4, "noTimestampTwo.four");
			AssertSizeAndReset(2);

			// Time is 500
			SendTimeEvent(50);
			AssertEvent(0, 5, 5.5, "timestampOne.five");
			AssertSizeAndReset(1);

			// Time is 600
			SendTimeEvent(100);
			AssertEvent(0, 6, 6.6, "timestampTwo.six");
			AssertEvent(1, 2, 2.2, "noTimestampTwo.two");
			AssertSizeAndReset(2);

			// Time is 800
			SendTimeEvent(200);
			AssertEvent(0, 2, 2.2, "timestampTwo.two");
			AssertEvent(1, 4, 4.4, "noTimestampTwo.four");
			AssertSizeAndReset(2);

			coordinator.Stop();
			SendTimeEvent(1000);
			Assert.IsFalse(listener.GetAndClearIsInvoked());
		}

        [Test]
		public void testRunTillNull()
		{
			coordinator.Coordinate(new CSVInputAdapter(epService, timestampsNotLooping));
			coordinator.Start();

			// Time is 100
			SendTimeEvent(100);
			log.Debug(".testRunTillNull time==100");
			AssertEvent(0, 1, 1.1, "timestampOne.one");
			AssertSizeAndReset(1);

			// Time is 300
			SendTimeEvent(200);
			log.Debug(".testRunTillNull time==300");
			AssertEvent(0, 3, 3.3, "timestampOne.three");
			AssertSizeAndReset(1);

			// Time is 500
			SendTimeEvent(200);
			log.Debug(".testRunTillNull time==500");
			AssertEvent(0, 5, 5.5, "timestampOne.five");
			AssertSizeAndReset(1);

			// Time is 600
			SendTimeEvent(100);
			log.Debug(".testRunTillNull time==600");
			Assert.IsFalse(listener.GetAndClearIsInvoked());

			// Time is 700
			SendTimeEvent(100);
			log.Debug(".testRunTillNull time==700");
			Assert.IsFalse(listener.GetAndClearIsInvoked());

			// Time is 800
			SendTimeEvent(100);
			log.Debug(".testRunTillNull time==800");
		}

        [Test]
		public void testNotUsingEngineThread()
		{
			coordinator = new AdapterCoordinatorImpl(epService, false);
			coordinator.Coordinate(new CSVInputAdapter(epService, noTimestampsNotLooping));
			coordinator.Coordinate(new CSVInputAdapter(epService, timestampsNotLooping));

            long startTime = Environment.TickCount;
			coordinator.Start();
            long endTime = Environment.TickCount;

			// The last event should be sent after 500 ms
			Assert.IsTrue(endTime - startTime > 500);

			Assert.AreEqual(6, listener.NewDataList.Count);
			AssertEvent(0, 1, 1.1, "noTimestampOne.one");
			AssertEvent(1, 1, 1.1, "timestampOne.one");
			AssertEvent(2, 2, 2.2, "noTimestampOne.two");
			AssertEvent(3, 3, 3.3, "noTimestampOne.three");
			AssertEvent(4, 3, 3.3, "timestampOne.three");
			AssertEvent(5, 5, 5.5, "timestampOne.five");
		}

		private void AssertEvent(int howManyBack, int? myInt, Double myDouble, String myString)
		{
			Assert.IsTrue(listener.IsInvoked);
			Assert.IsTrue(howManyBack < listener.NewDataList.Count);
			EventBean[] data = listener.NewDataList[howManyBack];
			Assert.AreEqual(1, data.Length);
			EventBean @event = data[0];
			Assert.AreEqual(myInt, @event.Get("myInt"));
			Assert.AreEqual(myDouble, @event.Get("myDouble"));
			Assert.AreEqual(myString, @event.Get("myString"));
		}


		private void SendTimeEvent(int timeIncrement){
			currentTime += timeIncrement;
		    CurrentTimeEvent @event = new CurrentTimeEvent(currentTime);
		    epService.EPRuntime.SendEvent(@event);
		}

		private void AssertSizeAndReset(int size)
		{
			List<EventBean[]> list = listener.NewDataList;
			Assert.AreEqual(size, list.Count);
			list.Clear();
			listener.GetAndClearIsInvoked();
		}

	}
} // End of namespace
