// ---------------------------------------------------------------------------------- /
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
// ---------------------------------------------------------------------------------- /

using System;
using System.Collections.Generic;

using net.esper.client;
using net.esper.collection;
using net.esper.compat;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.eql
{
	[TestFixture]
	public class TestModifiedWildcardSelect
	{
		private EPServiceProvider epService;
		private SupportUpdateListener listener;
		private SupportUpdateListener insertListener;
		private EDictionary<string, object> properties;

        [SetUp]
		protected void SetUp()
		{
            EPServiceProviderManager.PurgeAllProviders();
            epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
            epService.Initialize();
			listener = new SupportUpdateListener();
			insertListener = new SupportUpdateListener();
			properties = new HashDictionary<String, Object>();
		}

		[Test]
		public void testSingle()
		{
			String eventName = typeof(SupportBeanSimple).FullName;
			String text = "select *, myString||myString as concat from " + eventName + ".win:length(5)";

			EPStatement statement = epService.EPAdministrator.CreateEQL(text);
			statement.AddListener(listener);
			AssertSimple();
		}

		[Test]
		public void testSingleInsertInto()
		{
			String eventName = typeof(SupportBeanSimple).FullName;
			String text = "insert into someEvent select *, myString||myString as concat from " + eventName + ".win:length(5)";
			String textTwo = "select * from someEvent.win:length(5)";

			EPStatement statement = epService.EPAdministrator.CreateEQL(text);
			statement.AddListener(listener);

			statement = epService.EPAdministrator.CreateEQL(textTwo);
			statement.AddListener(insertListener);
			AssertSimple();
			AssertProperties(insertListener);
		}

		[Test]
		public void testJoinInsertInto()
		{
			String eventNameOne = typeof(SupportBeanSimple).FullName;
			String eventNameTwo = typeof(SupportMarketDataBean).FullName;
			String text = "insert into someJoinEvent select *, myString||myString as concat " +
					"from " + eventNameOne + ".win:length(5) as eventOne, "
					+ eventNameTwo + ".win:length(5) as eventTwo";
			String textTwo = "select * from someJoinEvent.win:length(5)";

			EPStatement statement = epService.EPAdministrator.CreateEQL(text);
			statement.AddListener(listener);

			statement = epService.EPAdministrator.CreateEQL(textTwo);
			statement.AddListener(insertListener);

			AssertNoCommonProperties();
			AssertProperties(insertListener);
		}

		[Test]
		public void testJoinNoCommonProperties()
		{
			String eventNameOne = typeof(SupportBeanSimple).FullName;
			String eventNameTwo = typeof(SupportMarketDataBean).FullName;
			String text = "select *, myString||myString as concat " +
					"from " + eventNameOne + ".win:length(5) as eventOne, "
					+ eventNameTwo + ".win:length(5) as eventTwo";

			EPStatement statement = epService.EPAdministrator.CreateEQL(text);
			statement.AddListener(listener);

			AssertNoCommonProperties();

			listener.Reset();
			epService.Initialize();

			text = "select *, myString||myString as concat " +
			"from " + eventNameOne + ".win:length(5) as eventOne, " +
					eventNameTwo + ".win:length(5) as eventTwo " +
					"where eventOne.myString = eventTwo.symbol";

			statement = epService.EPAdministrator.CreateEQL(text);
			statement.AddListener(listener);

			AssertNoCommonProperties();
		}

		[Test]
		public void testJoinCommonProperties()
		{
			String eventNameOne = typeof(SupportBean_A).FullName;
			String eventNameTwo = typeof(SupportBean_B).FullName;
			String text = "select *, eventOne.id||eventTwo.id as concat " +
					"from " + eventNameOne + ".win:length(5) as eventOne, " +
							eventNameTwo + ".win:length(5) as eventTwo ";

			EPStatement statement = epService.EPAdministrator.CreateEQL(text);
			statement.AddListener(listener);

			AssertCommonProperties();

			listener.Reset();
			epService.Initialize();

			text = "select *, eventOne.id||eventTwo.id as concat " +
				"from " + eventNameOne + ".win:length(5) as eventOne, " +
					eventNameTwo + ".win:length(5) as eventTwo " +
					"where eventOne.id = eventTwo.id";

			statement = epService.EPAdministrator.CreateEQL(text);
			statement.AddListener(listener);

			AssertCommonProperties();
		}

		[Test]
		public void testCombinedProperties()
		{
			String eventName = typeof(SupportBeanCombinedProps).FullName;
			String text = "select *, indexed[0].mapped('0ma').value||indexed[0].mapped('0mb').value as concat from " + eventName + ".win:length(5)";

			EPStatement statement = epService.EPAdministrator.CreateEQL(text);
			statement.AddListener(listener);
			AssertCombinedProps();
		}

		[Test]
		public void testMapEvents()
		{
			Configuration configuration = new Configuration();
			EDictionary<String, Type> typeMap = new HashDictionary<String, Type>();
			typeMap.Put("int", typeof(int?));
			typeMap.Put("string", typeof(string));
			configuration.AddEventTypeAlias("mapEvent", typeMap);
			epService = EPServiceProviderManager.GetProvider("wildcard map event", configuration);

			String text = "select *, string||string as concat from mapEvent.win:length(5)";

			EPStatement statement = epService.EPAdministrator.CreateEQL(text);
			statement.AddListener(listener);

			// The map to send into the runtime
			DataDictionary props = new DataDictionary();
			props.Put("int", 1);
			props.Put("string", "xx");
			epService.EPRuntime.SendEvent(props, "mapEvent");

			// The map of expected results
			properties.Put("int", 1);
			properties.Put("string", "xx");
			properties.Put("concat", "xxxx");

			AssertProperties(listener);
		}

		[Test]
		public void testInvalidRepeatedProperties()
		{
			String eventName = typeof(SupportBeanSimple).FullName;
			String text = "select *, myString||myString as myString from " + eventName + ".win:length(5)";

			try
			{
				epService.EPAdministrator.CreateEQL(text);
				Assert.Fail();
			}
			catch(EPException ex)
			{
				//Expected
			}
		}

		private void AssertNoCommonProperties()
		{
			SupportBeanSimple eventSimple = SendSimpleEvent("string");
			SupportMarketDataBean eventMarket = SendMarketEvent("string");

			EventBean _event = listener.LastNewData[0];
			properties.Put("concat", "stringstring");
			AssertProperties(listener);
			Assert.AreSame(eventSimple, _event["eventOne"]);
			Assert.AreSame(eventMarket, _event["eventTwo"]);
		}

		private void AssertSimple()
		{
			SupportBeanSimple _event = SendSimpleEvent("string");

	        Assert.AreEqual("stringstring", listener.LastNewData[0]["concat"]);
			properties.Put("concat", "stringstring");
			properties.Put("myString", "string");
			properties.Put("myInt", 0);
			AssertProperties(listener);

			Assert.AreEqual(typeof(Pair<object, IDictionary<string, object>>), listener.LastNewData[0].EventType.UnderlyingType);
	        Assert.IsTrue(listener.LastNewData[0].Underlying is Pair<object, IDictionary<string, object>>);
	        Pair<object, IDictionary<string, object>> pair = (Pair<object, IDictionary<string, object>>) listener.LastNewData[0].Underlying;
	        Assert.AreEqual(_event, pair.First);
	        Assert.AreEqual("stringstring", ((IDataDictionary)pair.Second).Get("concat"));
	    }

		private void AssertCommonProperties()
		{
			SendABEvents("string");
			EventBean _event = listener.LastNewData[0];
			properties.Put("concat", "stringstring");
			AssertProperties(listener);
			Assert.IsNotNull(_event["eventOne"]);
			Assert.IsNotNull(_event["eventTwo"]);
		}

		private void AssertCombinedProps()
		{
			SendCombinedProps();
			EventBean _eventBean = listener.LastNewData[0];

	        Assert.AreEqual("0ma0", _eventBean["indexed[0].mapped('0ma').value"]);
	        Assert.AreEqual("0ma1", _eventBean["indexed[0].mapped('0mb').value"]);
	        Assert.AreEqual("1ma0", _eventBean["indexed[1].mapped('1ma').value"]);
	        Assert.AreEqual("1ma1", _eventBean["indexed[1].mapped('1mb').value"]);

	        Assert.AreEqual("0ma0", _eventBean["array[0].mapped('0ma').value"]);
	        Assert.AreEqual("1ma1", _eventBean["array[1].mapped('1mb').value"]);

	        Assert.AreEqual("0ma00ma1", _eventBean["concat"]);
		}

		private void AssertProperties(SupportUpdateListener listener)
		{
			EventBean _event = listener.LastNewData[0];
			foreach (String property in properties.Keys)
			{
				Assert.AreEqual(properties.Get(property), _event[property]);
			}
		}

		private SupportBeanSimple SendSimpleEvent(String s)
		{
		    SupportBeanSimple bean = new SupportBeanSimple(s, 0);
		    epService.EPRuntime.SendEvent(bean);
	        return bean;
	    }

		private SupportMarketDataBean SendMarketEvent(String symbol)
		{
			SupportMarketDataBean bean = new SupportMarketDataBean(symbol, 0.0, 0L, null);
			epService.EPRuntime.SendEvent(bean);
	        return bean;
	    }

		private void SendABEvents(String id)
		{
			SupportBean_A beanOne = new SupportBean_A(id);
			SupportBean_B beanTwo = new SupportBean_B(id);
			epService.EPRuntime.SendEvent(beanOne);
			epService.EPRuntime.SendEvent(beanTwo);
		}

		private void SendCombinedProps()
		{
			epService.EPRuntime.SendEvent(SupportBeanCombinedProps.MakeDefaultBean());
		}
	}
} // End of namespace
