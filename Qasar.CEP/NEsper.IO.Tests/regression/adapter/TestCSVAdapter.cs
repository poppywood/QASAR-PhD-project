///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;

using net.esper.adapter;
using net.esper.adapter.csv;
using net.esper.client;
using net.esper.client.time;
using net.esper.compat;
using net.esper.events;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.adapter
{
    [TestFixture]
	public class TestCSVAdapter
	{
		private SupportUpdateListener listener;
		private String eventTypeAlias;
		private EPServiceProvider epService;
		private long currentTime;
		private InputAdapter adapter;
		private String[] propertyOrderTimestamps;
		private String[] propertyOrderNoTimestamps;
		private EDictionary<String, Type> propertyTypes;

        [SetUp]
		protected void SetUp()
		{
			propertyTypes = new HashDictionary<String, Type>();
            propertyTypes.Put("myInt", typeof (int));
            propertyTypes.Put("myDouble", typeof (double));
            propertyTypes.Put("myString", typeof (string));

			eventTypeAlias = "mapEvent";
			Configuration configuration = new Configuration();
			configuration.AddEventTypeAlias(eventTypeAlias, propertyTypes);
            configuration.AddEventTypeAlias("myNonMapEvent", typeof (Type).FullName);

			epService = EPServiceProviderManager.GetProvider("CSVProvider", configuration);
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

	    	propertyOrderNoTimestamps = new String[] { "myInt", "myDouble", "myString" };
	    	propertyOrderTimestamps = new String[] { "timestamp", "myInt", "myDouble", "myString" };
		}

        [Test]
		public void TestNullEPService()
		{
			CSVInputAdapter adapter = new CSVInputAdapter(null, new AdapterInputSource("regression/titleRow.csv"), eventTypeAlias);
			RunNullEPService(adapter);

			listener.Reset();

			adapter = new CSVInputAdapter(new AdapterInputSource("regression/titleRow.csv"), eventTypeAlias);
			RunNullEPService(adapter);
		}

        [Test]
		public void TestInputStream()
		{
            Stream stream = ResourceManager.GetResourceAsStream("regression/noTimestampOne.csv");
			CSVInputAdapterSpec adapterSpec  = new CSVInputAdapterSpec(new AdapterInputSource(stream), eventTypeAlias);
			adapterSpec.PropertyOrder = propertyOrderNoTimestamps;

			new CSVInputAdapter(epService, adapterSpec);

			adapterSpec.IsLooping = true;
			try
			{
				new CSVInputAdapter(epService, adapterSpec);
				Assert.Fail();
			}
			catch(EPException ex)
			{
				// Expected
			}
		}

        [Test]
		public void TestFewerPropertiesToSend()
		{
			String filename =  "regression/moreProperties.csv";
			int eventsPerSec = 10;

			List<Object[]> events = new List<Object[]>();
			events.Add(new Object[] { 100, 1, 1.1, "moreProperties.one" });
			events.Add(new Object[] { 100, 2, 2.2, "moreProperties.two" });
			events.Add(new Object[] { 100, 3, 3.3, "moreProperties.three" });
			String[] propertyOrder = new String[] { "someString", "myInt", "someInt", "myDouble", "myString" };

			bool isLooping = false;
			StartAdapter(filename, eventsPerSec, isLooping, true, null, propertyOrder);
			AssertEvents(isLooping, events);
		}

        [Test]
		public void TestConflictingPropertyOrder()
		{
			CSVInputAdapterSpec adapterSpec = new CSVInputAdapterSpec(new AdapterInputSource("regression/intsTitleRow.csv"), "intsTitleRowEvent");
			adapterSpec.EventsPerSec = 10;
			adapterSpec.PropertyOrder = new String[] { "intTwo", "intOne" };
			adapterSpec.IsUsingEngineThread = true;
			adapter = new CSVInputAdapter(epService, adapterSpec);

			String statementText = "select * from intsTitleRowEvent.win:length(5)";
			EPStatement statement = epService.EPAdministrator.CreateEQL(statementText);
			statement.AddListener(listener);

			adapter.Start();

			SendTimeEvent(100);

			Assert.IsTrue(listener.GetAndClearIsInvoked());
			Assert.AreEqual(1, listener.LastNewData.Length);
			Assert.AreEqual("1", listener.LastNewData[0].Get("intTwo"));
			Assert.AreEqual("0", listener.LastNewData[0].Get("intOne"));
		}

        [Test]
		public void TestEventsPerSecAndTimestamp()
		{
			String filename =  "regression/timestampOne.csv";
			int eventsPerSec = 5;

			List<Object[]> events = new List<Object[]>();
			events.Add(new Object[] { 200, 1, 1.1, "timestampOne.one"});
			events.Add(new Object[] { 200, 3, 3.3, "timestampOne.three"});
			events.Add(new Object[] { 200, 5, 5.5, "timestampOne.five"});

			bool isLooping = false;
			StartAdapter(filename, eventsPerSec, isLooping, true, "timestamp", propertyOrderTimestamps);
			AssertEvents(isLooping, events);
		}

        [Test]
		public void TestNoTimestampNoEventsPerSec()
		{
			String filename = "regression/timestampOne.csv";

			StartAdapter(filename, -1, false, true, null, propertyOrderTimestamps);

			Assert.AreEqual(3, listener.NewDataList.Count);
			AssertEvent(0, 1, 1.1, "timestampOne.one");
			AssertEvent(1, 3, 3.3, "timestampOne.three");
			AssertEvent(2, 5, 5.5, "timestampOne.five");
		}

        [Test]
		public void TestNoPropertyTypes()
		{
			CSVInputAdapterSpec adapterSpec = new CSVInputAdapterSpec(new AdapterInputSource("regression/noTimestampOne.csv"), "allStringEvent");
			adapterSpec.EventsPerSec = 10;
			adapterSpec.PropertyOrder = new String[] { "myInt", "myDouble", "myString" };
			adapterSpec.IsUsingEngineThread = true;
			adapter = new CSVInputAdapter(epService, adapterSpec);

			String statementText = "select * from allStringEvent.win:length(5)";
			EPStatement statement = epService.EPAdministrator.CreateEQL(statementText);
			statement.AddListener(listener);

			adapter.Start();

			SendTimeEvent(100);
			AssertEvent("1", "1.1", "noTimestampOne.one");

			SendTimeEvent(100);
			AssertEvent("2", "2.2", "noTimestampOne.two");

			SendTimeEvent(100);
			AssertEvent("3", "3.3", "noTimestampOne.three");
		}

        [Test]
		public void TestRuntimePropertyTypes()
		{
			CSVInputAdapterSpec adapterSpec = new CSVInputAdapterSpec(new AdapterInputSource("regression/noTimestampOne.csv"), "propertyTypeEvent");
			adapterSpec.EventsPerSec = 10;
			adapterSpec.PropertyOrder = new String[] { "myInt", "myDouble", "myString" };
			adapterSpec.PropertyTypes = propertyTypes;
			adapterSpec.IsUsingEngineThread = true;
			adapter = new CSVInputAdapter(epService, adapterSpec);

			String statementText = "select * from propertyTypeEvent.win:length(5)";
			EPStatement statement = epService.EPAdministrator.CreateEQL(statementText);
			statement.AddListener(listener);

			adapter.Start();

			SendTimeEvent(100);
			AssertEvent(1, 1.1, "noTimestampOne.one");

			SendTimeEvent(100);
			AssertEvent(2, 2.2, "noTimestampOne.two");

			SendTimeEvent(100);
			AssertEvent(3, 3.3, "noTimestampOne.three");
		}

        [Test]
		public void TestRuntimePropertyTypesInvalid()
		{
            EDictionary<String, Type> propertyTypesInvalid = new HashDictionary<String, Type>();
            propertyTypesInvalid.PutAll(propertyTypes);
			propertyTypesInvalid.Put("anotherProperty", typeof(String));
			try
			{
				CSVInputAdapterSpec adapterSpec = new CSVInputAdapterSpec(new AdapterInputSource("regression/noTimestampOne.csv"), "mapEvent");
				adapterSpec.PropertyTypes = propertyTypesInvalid;
				(new CSVInputAdapter(epService, adapterSpec)).Start();
				Assert.Fail();
			}
			catch(EPException er)
			{
				// Expected
			}

            propertyTypesInvalid = new HashDictionary<String, Type>();
            propertyTypesInvalid.PutAll(propertyTypes);
			propertyTypesInvalid.Put("myInt", typeof(String));
			try
			{
				CSVInputAdapterSpec adapterSpec = new CSVInputAdapterSpec(new AdapterInputSource("regression/noTimestampOne.csv"), "mapEvent");
				adapterSpec.PropertyTypes = propertyTypesInvalid;
				(new CSVInputAdapter(epService, adapterSpec)).Start();
				Assert.Fail();
			}
			catch(EPException er)
			{
				// Expected
			}

            propertyTypesInvalid = new HashDictionary<String, Type>();
            propertyTypesInvalid.PutAll(propertyTypes);
			propertyTypesInvalid.Remove("myInt");
            propertyTypesInvalid.Put("anotherInt", typeof (int));
			try
			{
				CSVInputAdapterSpec adapterSpec = new CSVInputAdapterSpec(new AdapterInputSource("regression/noTimestampOne.csv"), "mapEvent");
				adapterSpec.PropertyTypes = propertyTypesInvalid;
				(new CSVInputAdapter(epService, adapterSpec)).Start();
				Assert.Fail();
			}
			catch(EPException er)
			{
				// Expected
			}
		}

        [Test]
        public void TestRunWrongAlias()
		{
			String filename = "regression/noTimestampOne.csv";
			AssertFailedConstruction(filename, "myNonMapEvent");
		}

        [Test]
        public void TestRunWrongMapType()
		{
			String filename = "regression/differentMap.csv";
			AssertFailedConstruction(filename, eventTypeAlias);
		}

        [Test]
        public void TestRunNonExistentFile()
		{
			String filename = "someNonExistentFile";
			AssertFailedConstruction(filename, eventTypeAlias);
		}

        [Test]
        public void TestRunEmptyFile()
		{
			String filename = "regression/emptyFile.csv";
			StartAdapter(filename, -1, true, true, null, propertyOrderTimestamps);
			Assert.IsFalse(listener.GetAndClearIsInvoked());
		}

        [Test]
        public void TestRunTitleRowOnly()
		{
			String filename = "regression/titleRowOnly.csv";
			propertyOrderNoTimestamps = null;
			StartAdapter(filename, -1, true, true, "timestamp", null);
			Assert.IsFalse(listener.GetAndClearIsInvoked());
		}

        [Test]
        public void TestRunDecreasingTimestamps()
		{
			String filename = "regression/decreasingTimestamps.csv";
			try
			{
				StartAdapter(filename, -1, false, true, null, null);

				SendTimeEvent(100);
				AssertEvent(1, 1.1, "one");

				SendTimeEvent(200);
				Assert.Fail();
			}
			catch(EPException e)
			{
				// Expected
			}
		}

        [Test]
        public void TestRunNegativeTimestamps()
		{
			String filename = "regression/negativeTimestamps.csv";
			try
			{
				StartAdapter(filename, -1, false, true, null, null);

				SendTimeEvent(100);
				AssertEvent(1, 1.1, "one");

				SendTimeEvent(200);
				Assert.Fail();
			}
			catch(EPException ex)
			{
				// Expected
			}
		}

        [Test]
        public void TestRunTimestamps()
		{
			String filename =  "regression/timestampOne.csv";
			int eventsPerSec = -1;

			List<Object[]> events = new List<Object[]>();
			events.Add(new Object[] { 100, 1, 1.1, "timestampOne.one"});
			events.Add(new Object[] { 200, 3, 3.3, "timestampOne.three"});
			events.Add(new Object[] { 200, 5, 5.5, "timestampOne.five"});

			bool isLooping = false;
			StartAdapter(filename, eventsPerSec, isLooping, true, "timestamp", propertyOrderTimestamps);
			AssertEvents(isLooping, events);

			isLooping = true;
			StartAdapter(filename, eventsPerSec, isLooping, true, "timestamp", propertyOrderTimestamps);
			AssertEvents(isLooping, events);
		}

        [Test]
        public void TestStartOneRow()
		{
			String filename = "regression/oneRow.csv";
			StartAdapter(filename, -1, false, true, "timestamp", propertyOrderTimestamps);

			SendTimeEvent(100);
			AssertEvent(1, 1.1, "one");
		}

        [Test]
        public void TestPause()
		{
			String filename = "regression/noTimestampOne.csv";
			StartAdapter(filename, 10, false, true, "timestamp", propertyOrderNoTimestamps);

			SendTimeEvent(100);
			AssertEvent(1, 1.1, "noTimestampOne.one");

			adapter.Pause();

			SendTimeEvent(100);
		    Assert.AreEqual(AdapterState.PAUSED, adapter.State);
			Assert.IsFalse(listener.GetAndClearIsInvoked());
		}

        [Test]
        public void TestResumeWholeInterval()
		{
			String filename = "regression/noTimestampOne.csv";
			StartAdapter(filename, 10, false, true, null, propertyOrderNoTimestamps);

			SendTimeEvent(100);
			AssertEvent(1, 1.1, "noTimestampOne.one");

			adapter.Pause();
			SendTimeEvent(100);
			Assert.IsFalse(listener.GetAndClearIsInvoked());
			adapter.Resume();


			AssertEvent(2, 2.2, "noTimestampOne.two");
		}

        [Test]
        public void TestResumePartialInterval()
		{
			String filename = "regression/noTimestampOne.csv";
			StartAdapter(filename, 10, false, true, null, propertyOrderNoTimestamps);

			// time is 100
			SendTimeEvent(100);
			AssertEvent(1, 1.1, "noTimestampOne.one");

			// time is 150
			SendTimeEvent(50);

			adapter.Pause();
			// time is 200
			SendTimeEvent(50);
			Assert.IsFalse(listener.GetAndClearIsInvoked());
			adapter.Resume();

			AssertEvent(2, 2.2, "noTimestampOne.two");
		}

        [Test]
        public void TestEventsPerSecInvalid()
		{
			String filename = "regression/timestampOne.csv";

			try
			{
				StartAdapter(filename, 0, true, true, null, null);
				Assert.Fail();
			}
			catch(ArgumentException e)
			{
				// Expected
			}

			try
			{
				StartAdapter(filename, 1001, true, true, null, null);
				Assert.Fail();
			}
			catch(ArgumentException e)
			{
				// Expected
			}
		}

        [Test]
        public void TestIsLoopingTitleRow()
		{
			String filename =  "regression/titleRow.csv";
			int eventsPerSec = -1;

			List<Object[]> events = new List<Object[]>();
			events.Add(new Object[] { 100, 1, 1.1, "one"});
			events.Add(new Object[] { 200, 3, 3.3, "three"});
			events.Add(new Object[] { 200, 5, 5.5, "five"});

			bool isLooping = true;
			propertyOrderNoTimestamps = null;
			StartAdapter(filename, eventsPerSec, isLooping, true, "timestamp", null);
			AssertLoopingEvents(events);
		}

        [Test]
        public void TestIsLoopingNoTitleRow()
		{
			String filename =  "regression/timestampOne.csv";
			int eventsPerSec = -1;

			List<Object[]> events = new List<Object[]>();
			events.Add(new Object[] { 100, 1, 1.1, "timestampOne.one"});
			events.Add(new Object[] { 200, 3, 3.3, "timestampOne.three"});
			events.Add(new Object[] { 200, 5, 5.5, "timestampOne.five"});

			bool isLooping = true;
			StartAdapter(filename, eventsPerSec, isLooping, true, "timestamp", propertyOrderTimestamps);
			AssertLoopingEvents(events);
		}

        [Test]
        public void TestTitleRowNoTimestamp()
		{
			String filename =  "regression/titleRowNoTimestamp.csv";
			int eventsPerSec = 10;

			List<Object[]> events = new List<Object[]>();
			events.Add(new Object[] { 100, 1, 1.1, "one"});
			events.Add(new Object[] { 100, 3, 3.3, "three"});
			events.Add(new Object[] { 100, 5, 5.5, "five"});

			bool isLooping = true;
			propertyOrderNoTimestamps = null;
			StartAdapter(filename, eventsPerSec, isLooping, true, null, null);
			AssertLoopingEvents(events);
		}

        [Test]
        public void TestComments()
		{
			String filename =  "regression/comments.csv";
			int eventsPerSec = -1;

			List<Object[]> events = new List<Object[]>();
			events.Add(new Object[] { 100, 1, 1.1, "one"});
			events.Add(new Object[] { 200, 3, 3.3, "three"});
			events.Add(new Object[] { 200, 5, 5.5, "five"});

			bool isLooping = false;
			StartAdapter(filename, eventsPerSec, isLooping, true, "timestamp", propertyOrderTimestamps);
			AssertEvents(isLooping, events);

			isLooping = true;
			StartAdapter(filename, eventsPerSec, isLooping, true, "timestamp", propertyOrderTimestamps);
			AssertEvents(isLooping, events);
		}

        [Test]
        public void TestDestroy()
		{
			String filename = "regression/timestampOne.csv";
			StartAdapter(filename, -1, false, true, "timestamp", propertyOrderTimestamps);
			adapter.Destroy();
			Assert.AreEqual(AdapterState.DESTROYED, adapter.State);
		}

        [Test]
        public void TestStop()
		{
			String filename =  "regression/timestampOne.csv";
			int eventsPerSec = -1;

			List<Object[]> events = new List<Object[]>();
			events.Add(new Object[] { 100, 1, 1.1, "timestampOne.one"});
			events.Add(new Object[] { 200, 3, 3.3, "timestampOne.three"});

			bool isLooping = false;
			StartAdapter(filename, eventsPerSec, isLooping, true, "timestamp", propertyOrderTimestamps);

			AssertFlatEvents(events);

			adapter.Stop();

			SendTimeEvent(1000);
			Assert.IsFalse(listener.GetAndClearIsInvoked());

			adapter.Start();
			AssertFlatEvents(events);
		}

        [Test]
        public void TestStopAfterEOF()
		{
			String filename =  "regression/timestampOne.csv";
			StartAdapter(filename, -1, false, false, "timestamp", propertyOrderTimestamps);
			Assert.AreEqual(AdapterState.OPENED, adapter.State);
		}

        [Test]
        public void TestNotUsingEngineThreadTimestamp()
		{
			String filename = "regression/timestampOne.csv";

			long startTime = Environment.TickCount;
			StartAdapter(filename, -1, false, false, "timestamp", propertyOrderTimestamps);
			long endTime = Environment.TickCount;

			// The last event should be sent after 500 ms
			Assert.IsTrue(endTime - startTime > 500);

			Assert.AreEqual(3, listener.NewDataList.Count);
			AssertEvent(0, 1, 1.1, "timestampOne.one");
			AssertEvent(1, 3, 3.3, "timestampOne.three");
			AssertEvent(2, 5, 5.5, "timestampOne.five");
		}

        [Test]
		public void TestNotUsingEngineThreadNoTimestamp()
		{
			String filename = "regression/noTimestampOne.csv";

			long startTime = Environment.TickCount;
			StartAdapter(filename, 5, false, false, null, propertyOrderNoTimestamps);
			long endTime = Environment.TickCount;

			// The last event should be sent after 600 ms
			Assert.IsTrue(endTime - startTime > 600);

			Assert.AreEqual(3, listener.NewDataList.Count);
			AssertEvent(0, 1, 1.1, "noTimestampOne.one");
			AssertEvent(1, 2, 2.2, "noTimestampOne.two");
			AssertEvent(2, 3, 3.3, "noTimestampOne.three");
		}

		private void RunNullEPService(CSVInputAdapter adapter)
		{
			try
			{
				adapter.Start();
				Assert.Fail();
			}
			catch(EPException ex)
			{
				// Expected
			}

			try
			{
			    adapter.EPService = null;
				Assert.Fail();
			}
			catch(ArgumentException ex)
			{
				// Expected
			}

			adapter.EPService = epService;
			adapter.Start();
			Assert.AreEqual(3, listener.NewDataList.Count);
		}

		private void AssertEvent(int howManyBack, int? myInt, double? myDouble, String myString)
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
        
		private void AssertEvents(bool isLooping, List<Object[]> events)
		{
			if(isLooping)
			{
				AssertLoopingEvents(events);
			}
			else
			{
				AssertNonLoopingEvents(events);
			}
		}
        
		private void AssertEvent(Object[] properties)
		{
			if(properties.Length == 1)
			{
				Assert.IsFalse(listener.GetAndClearIsInvoked());
			}
			else if(properties.Length == 4)
			{
				// properties = [callbackDelay, myInt, myDouble, myString]
				AssertEvent((int)properties[1], (Double)properties[2], (String)properties[3]);
			}
			else
			{
				// properties = [callbackDelay, intOne, doubleOne, StringOne, intTwo, doubleTwo, stringTwo]
				AssertTwoEvents((int)properties[1], (Double)properties[2], (String)properties[3], (int)properties[4], (Double)properties[5], (String)properties[6]);
			}
		}

		private void AssertEvent(Object myInt, Object myDouble, Object myString)
		{
			Assert.IsTrue(listener.GetAndClearIsInvoked());
			Assert.AreEqual(1, listener.LastNewData.Length);
			EventBean @event = listener.LastNewData[0];
			Assert.AreEqual(myInt, @event.Get("myInt"));
			Assert.AreEqual(myDouble, @event.Get("myDouble"));
			Assert.AreEqual(myString, @event.Get("myString"));
			listener.Reset();
		}

		private void AssertTwoEvents(int? intOne, double? doubleOne, String stringOne,
									 int? intTwo, double? doubleTwo, String stringTwo)
		{
			Assert.IsTrue(listener.IsInvoked);
			Assert.AreEqual(2, listener.NewDataList.Count);

			Assert.AreEqual(1, listener.NewDataList[0].Length);
            EventBean @event = listener.NewDataList[0][0];
			Assert.AreEqual(intOne, @event.Get("myInt"));
			Assert.AreEqual(doubleOne, @event.Get("myDouble"));
			Assert.AreEqual(stringOne, @event.Get("myString"));

			Assert.AreEqual(1, listener.NewDataList[1].Length);
			@event = listener.NewDataList[1][0];
			Assert.AreEqual(intTwo, @event.Get("myInt"));
			Assert.AreEqual(doubleTwo, @event.Get("myDouble"));
			Assert.AreEqual(stringTwo, @event.Get("myString"));
		}
        
		private void AssertNonLoopingEvents(List<Object[]> events)
		{
			AssertFlatEvents(events);

			SendTimeEvent(1000);
			AssertEvent(new Object[] { 1000 });
		}

        private void AssertLoopingEvents(IEnumerable<Object[]> events)
		{
			AssertFlatEvents(events);
			AssertFlatEvents(events);
			AssertFlatEvents(events);
		}

        private void AssertFlatEvents(IEnumerable<Object[]> events)
		{
			foreach (Object[] @event in events)
			{
				SendTimeEvent((int)@event[0]);
				AssertEvent(@event);
				listener.Reset();
			}
		}

		private void StartAdapter(String filename, int eventsPerSec, bool isLooping, bool usingEngineThread, String timestampColumn, String[] propertyOrder)
		{
			CSVInputAdapterSpec adapterSpec = new CSVInputAdapterSpec(new AdapterInputSource(filename), eventTypeAlias);
			if(eventsPerSec != -1)
			{
				adapterSpec.EventsPerSec = eventsPerSec;
			}
			adapterSpec.IsLooping = isLooping;
			adapterSpec.PropertyOrder = propertyOrder;
			adapterSpec.IsUsingEngineThread = usingEngineThread;
			adapterSpec.TimestampColumn = timestampColumn;

			adapter = new CSVInputAdapter(epService, adapterSpec);
			adapter.Start();
		}

		private void AssertFailedConstruction(String filename, String eventTypeAlias)
		{
		    try
		    {
		        (new CSVInputAdapter(epService, new AdapterInputSource(filename), eventTypeAlias)).Start();
		        Assert.Fail();
		    }
        	catch(EPException ex)
			{
				// Expected
			}
		}

	}
} // End of namespace
