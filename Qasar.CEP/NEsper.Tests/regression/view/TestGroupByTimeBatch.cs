///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using net.esper.client;
using net.esper.client.time;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.view
{
    [TestFixture]
	public class TestGroupByTimeBatch
	{
	    private EPServiceProvider epService;
	    private SupportUpdateListener listener;

        [SetUp]
	    public void SetUp()
	    {
	        Configuration config = SupportConfigFactory.Configuration;
	        config.AddEventTypeAlias("MarketData", typeof(SupportMarketDataBean));
	        config.AddEventTypeAlias("SupportBean", typeof(SupportBean));
	        epService = EPServiceProviderManager.GetDefaultProvider(config);
	        epService.Initialize();
	        epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
	        listener = new SupportUpdateListener();
	    }

        [Test]
	    public void testTimeBatchRowForAllNoJoin()
	    {
	        SendTimer(0);
	        String stmtText = "select Sum(price) as sumPrice from MarketData.win:time_batch(1 sec)";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        stmt.AddListener(listener);

	        // send first batch
	        SendMDEvent("DELL", 10, 0L);
	        SendMDEvent("IBM", 15, 0L);
	        SendMDEvent("DELL", 20, 0L);
	        SendTimer(1000);

	        EventBean[] newEvents = listener.LastNewData;
	        Assert.AreEqual(1, newEvents.Length);
	        AssertEvent(newEvents[0], 45d);

	        // send second batch
	        SendMDEvent("IBM", 20, 600L);
	        SendTimer(2000);

	        newEvents = listener.LastNewData;
	        Assert.AreEqual(1, newEvents.Length);
	        AssertEvent(newEvents[0], 20d);

	        EventBean[] oldEvents = listener.LastOldData;
	        Assert.AreEqual(1, oldEvents.Length);
	        AssertEvent(oldEvents[0], 45d);
	    }

        [Test]
        public void testTimeBatchRowForAllJoin()
	    {
	        SendTimer(0);
	        String stmtText = "select Sum(price) as sumPrice from MarketData.win:time_batch(1 sec) as S0, SupportBean as S1 where S0.symbol = S1.string";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        stmt.AddListener(listener);

	        SendSupportEvent("DELL");
	        SendSupportEvent("IBM");

	        // send first batch
	        SendMDEvent("DELL", 10, 0L);
	        SendMDEvent("IBM", 15, 0L);
	        SendMDEvent("DELL", 20, 0L);
	        SendTimer(1000);

	        EventBean[] newEvents = listener.LastNewData;
	        Assert.AreEqual(1, newEvents.Length);
	        AssertEvent(newEvents[0], 45d);

	        // send second batch
	        SendMDEvent("IBM", 20, 600L);
	        SendTimer(2000);

	        newEvents = listener.LastNewData;
	        Assert.AreEqual(1, newEvents.Length);
	        AssertEvent(newEvents[0], 20d);

	        EventBean[] oldEvents = listener.LastOldData;
	        Assert.AreEqual(1, oldEvents.Length);
	        AssertEvent(oldEvents[0], 45d);
	    }

        [Test]
	    public void testTimeBatchAggregateAllNoJoin()
	    {
	        SendTimer(0);
	        String stmtText = "select symbol, Sum(price) as sumPrice from MarketData.win:time_batch(1 sec)";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        stmt.AddListener(listener);

	        // send first batch
	        SendMDEvent("DELL", 10, 0L);
	        SendMDEvent("IBM", 15, 0L);
	        SendMDEvent("DELL", 20, 0L);
	        SendTimer(1000);

	        EventBean[] newEvents = listener.LastNewData;
	        Assert.AreEqual(3, newEvents.Length);
	        AssertEvent(newEvents[0], "DELL", 45d);
	        AssertEvent(newEvents[1], "IBM", 45d);
	        AssertEvent(newEvents[2], "DELL", 45d);

	        // send second batch
	        SendMDEvent("IBM", 20, 600L);
	        SendTimer(2000);

	        newEvents = listener.LastNewData;
	        Assert.AreEqual(1, newEvents.Length);
	        AssertEvent(newEvents[0], "IBM", 20d);

	        EventBean[] oldEvents = listener.LastOldData;
	        Assert.AreEqual(3, oldEvents.Length);
	        AssertEvent(oldEvents[0], "DELL", 45d);
	        AssertEvent(oldEvents[1], "IBM", 45d);
	        AssertEvent(oldEvents[2], "DELL", 45d);
	    }

        [Test]
	    public void testTimeBatchAggregateAllJoin()
	    {
	        SendTimer(0);
	        String stmtText = "select symbol, Sum(price) as sumPrice from MarketData.win:time_batch(1 sec) as S0, SupportBean as S1 where S0.symbol = S1.string";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        stmt.AddListener(listener);

	        SendSupportEvent("DELL");
	        SendSupportEvent("IBM");

	        // send first batch
	        SendMDEvent("DELL", 10, 0L);
	        SendMDEvent("IBM", 15, 0L);
	        SendMDEvent("DELL", 20, 0L);
	        SendTimer(1000);

	        EventBean[] newEvents = listener.LastNewData;
	        Assert.AreEqual(3, newEvents.Length);
	        AssertEvent(newEvents[0], "DELL", 45d);
	        AssertEvent(newEvents[1], "IBM", 45d);
	        AssertEvent(newEvents[2], "DELL", 45d);

	        // send second batch
	        SendMDEvent("IBM", 20, 600L);
	        SendTimer(2000);

	        newEvents = listener.LastNewData;
	        Assert.AreEqual(1, newEvents.Length);
	        AssertEvent(newEvents[0], "IBM", 20d);

	        EventBean[] oldEvents = listener.LastOldData;
	        Assert.AreEqual(3, oldEvents.Length);
	        AssertEvent(oldEvents[0], "DELL", 45d);
	        AssertEvent(oldEvents[1], "IBM", 45d);
	        AssertEvent(oldEvents[2], "DELL", 45d);
	    }

        [Test]
	    public void testTimeBatchRowPerGroupNoJoin()
	    {
	        SendTimer(0);
	        String stmtText = "select symbol, Sum(price) as sumPrice from MarketData.win:time_batch(1 sec) group by symbol";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        stmt.AddListener(listener);

	        // send first batch
	        SendMDEvent("DELL", 10, 0L);
	        SendMDEvent("IBM", 15, 0L);
	        SendMDEvent("DELL", 20, 0L);
	        SendTimer(1000);

	        EventBean[] newEvents = listener.LastNewData;
	        Assert.AreEqual(2, newEvents.Length);
	        AssertEvent(newEvents[0], "DELL", 30d);
	        AssertEvent(newEvents[1], "IBM", 15d);

	        // send second batch
	        SendMDEvent("IBM", 20, 600L);
	        SendTimer(2000);

	        newEvents = listener.LastNewData;
	        Assert.AreEqual(2, newEvents.Length);
	        AssertEvent(newEvents[1], "DELL", null);
	        AssertEvent(newEvents[0], "IBM", 20d);

	        EventBean[] oldEvents = listener.LastOldData;
	        Assert.AreEqual(2, oldEvents.Length);
	        AssertEvent(oldEvents[1], "DELL", 30d);
	        AssertEvent(oldEvents[0], "IBM", 15d);
	    }

        [Test]
        public void testTimeBatchRowPerGroupJoin()
	    {
	        SendTimer(0);
	        String stmtText = "select symbol, sum(price) as sumPrice " +
	                         " from MarketData.win:time_batch(1 sec) as S0, SupportBean as S1" +
	                         " where S0.symbol = S1.string " +
	                         " group by symbol";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        stmt.AddListener(listener);

	        SendSupportEvent("DELL");
	        SendSupportEvent("IBM");

	        // send first batch
	        SendMDEvent("DELL", 10, 0L);
	        SendMDEvent("IBM", 15, 0L);
	        SendMDEvent("DELL", 20, 0L);
	        SendTimer(1000);

	        EventBean[] newEvents = listener.LastNewData;
	        Assert.AreEqual(2, newEvents.Length);
	        AssertEvent(newEvents[0], "DELL", 30d);
	        AssertEvent(newEvents[1], "IBM", 15d);

	        // send second batch
	        SendMDEvent("IBM", 20, 600L);
	        SendTimer(2000);

	        newEvents = listener.LastNewData;
	        Assert.AreEqual(2, newEvents.Length);
	        AssertEvent(newEvents[1], "DELL", null);
	        AssertEvent(newEvents[0], "IBM", 20d);

	        EventBean[] oldEvents = listener.LastOldData;
	        Assert.AreEqual(2, oldEvents.Length);
	        AssertEvent(oldEvents[1], "DELL", 30d);
	        AssertEvent(oldEvents[0], "IBM", 15d);
	    }

        [Test]
	    public void testTimeBatchAggrGroupedNoJoin()
	    {
	        SendTimer(0);
	        String stmtText = "select symbol, Sum(price) as sumPrice, volume from MarketData.win:time_batch(1 sec) group by symbol";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        stmt.AddListener(listener);

	        SendMDEvent("DELL", 10, 200L);
	        SendMDEvent("IBM", 15, 500L);
	        SendMDEvent("DELL", 20, 250L);

	        SendTimer(1000);
	        EventBean[] newEvents = listener.LastNewData;
	        Assert.AreEqual(3, newEvents.Length);
	        AssertEvent(newEvents[0], "DELL", 30d, 200L);
	        AssertEvent(newEvents[1], "IBM", 15d, 500L);
	        AssertEvent(newEvents[2], "DELL", 30d, 250L);

	        SendMDEvent("IBM", 20, 600L);
	        SendTimer(2000);
	        newEvents = listener.LastNewData;
	        Assert.AreEqual(1, newEvents.Length);
	        AssertEvent(newEvents[0], "IBM", 20d, 600L);
	        EventBean[] oldEvents = listener.LastOldData;
	        Assert.AreEqual(3, oldEvents.Length);
	        AssertEvent(oldEvents[0], "DELL", 30d, 200L);
	        AssertEvent(oldEvents[1], "IBM", 15d, 500L);
	        AssertEvent(oldEvents[2], "DELL", 30d, 250L);
	    }

        [Test]
	    public void testTimeBatchAggrGroupedJoin()
	    {
	        SendTimer(0);
	        String stmtText = "select symbol, Sum(price) as sumPrice, volume " +
	                          "from MarketData.win:time_batch(1 sec) as S0, SupportBean as S1" +
	                          " where S0.symbol = S1.string " +
	                          " group by symbol";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        stmt.AddListener(listener);

	        SendSupportEvent("DELL");
	        SendSupportEvent("IBM");

	        SendMDEvent("DELL", 10, 200L);
	        SendMDEvent("IBM", 15, 500L);
	        SendMDEvent("DELL", 20, 250L);

	        SendTimer(1000);
	        EventBean[] newEvents = listener.LastNewData;
	        Assert.AreEqual(3, newEvents.Length);
	        AssertEvent(newEvents[0], "DELL", 30d, 200L);
	        AssertEvent(newEvents[1], "IBM", 15d, 500L);
	        AssertEvent(newEvents[2], "DELL", 30d, 250L);

	        SendMDEvent("IBM", 20, 600L);
	        SendTimer(2000);
	        newEvents = listener.LastNewData;
	        Assert.AreEqual(1, newEvents.Length);
	        AssertEvent(newEvents[0], "IBM", 20d, 600L);
	        EventBean[] oldEvents = listener.LastOldData;
	        Assert.AreEqual(3, oldEvents.Length);
	        AssertEvent(oldEvents[0], "DELL", 30d, 200L);
	        AssertEvent(oldEvents[1], "IBM", 15d, 500L);
	        AssertEvent(oldEvents[2], "DELL", 30d, 250L);
	    }

	    private void SendSupportEvent(String _string)
	    {
	        epService.EPRuntime.SendEvent(new SupportBean(_string, -1));
	    }

	    private void SendMDEvent(String symbol, double price, long? volume)
	    {
	        epService.EPRuntime.SendEvent(new SupportMarketDataBean(symbol, price, volume, null));
	    }

	    private static void AssertEvent(EventBean _event, String symbol, double? sumPrice, long? volume)
	    {
	        Assert.AreEqual(symbol, _event.Get("symbol"));
	        Assert.AreEqual(sumPrice, _event.Get("sumPrice"));
	        Assert.AreEqual(volume, _event.Get("volume"));
	    }

	    private static void AssertEvent(EventBean _event, String symbol, double? sumPrice)
	    {
	        Assert.AreEqual(symbol, _event.Get("symbol"));
	        Assert.AreEqual(sumPrice, _event.Get("sumPrice"));
	    }

	    private static void AssertEvent(EventBean _event, Double sumPrice)
	    {
	        Assert.AreEqual(sumPrice, _event.Get("sumPrice"));
	    }

	    private void SendTimer(long time)
	    {
	        CurrentTimeEvent _event = new CurrentTimeEvent(time);
	        EPRuntime runtime = epService.EPRuntime;
	        runtime.SendEvent(_event);
	    }
	}
} // End of namespace
