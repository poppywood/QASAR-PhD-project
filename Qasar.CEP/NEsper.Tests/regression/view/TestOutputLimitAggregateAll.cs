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
using net.esper.collection;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.view
{
    [TestFixture]
	public class TestOutputLimitAggregateAll
	{
	    private static readonly String EVENT_NAME = typeof(SupportMarketDataBean).FullName;
	    private const String JOIN_KEY = "KEY";

	    private SupportUpdateListener listener;
		private EPServiceProvider epService;
	    private long currentTime;

	    [SetUp]
        public void SetUp()
	    {
	        epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
	        epService.Initialize();
	        listener = new SupportUpdateListener();
	    }

        [Test]
        public void testMaxTimeWindow()
	    {
	        epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
	        SendTimer(0);

	        String viewExpr = "select volume, max(price) as maxVol" +
	                          " from " + typeof(SupportMarketDataBean).FullName + ".win:time(1 sec) " +
	                          "output every 1 seconds";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(viewExpr);
	        stmt.AddListener(listener);

	        SendEvent("SYM1", 1d);
	        SendEvent("SYM1", 2d);
	        listener.Reset();

	        // moves all events out of the window,
	        SendTimer(1000);        // newdata is 2 eventa, old data is the same 2 events, therefore the sum is null
	        UniformPair<EventBean[]> result = listener.GetDataListsFlattened();
	        Assert.AreEqual(2, result.First.Length);
	        Assert.AreEqual(null, result.First[0].Get("maxVol"));
	        Assert.AreEqual(null, result.First[1].Get("maxVol"));
	        Assert.AreEqual(2, result.Second.Length);
	        Assert.AreEqual(null, result.Second[0].Get("maxVol"));
	        Assert.AreEqual(null, result.Second[1].Get("maxVol"));
	    }

        [Test]
        public void testJoinSortWindow()
	    {
	        epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
	        SendTimer(0);

	        String viewExpr = "select volume, max(price) as maxVol" +
	                          " from " + typeof(SupportMarketDataBean).FullName + ".ext:sort('volume', true, 1) as s0," +
	                          typeof(SupportBean).FullName + " as s1 " +
	                          "output every 1 seconds";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(viewExpr);
	        stmt.AddListener(listener);
	        epService.EPRuntime.SendEvent(new SupportBean("JOIN_KEY", -1));

	        SendEvent("JOIN_KEY", 1d);
	        SendEvent("JOIN_KEY", 2d);
	        listener.Reset();

	        // moves all events out of the window,
	        SendTimer(1000);        // newdata is 2 eventa, old data is the same 2 events, therefore the sum is null
            UniformPair<EventBean[]> result = listener.GetDataListsFlattened();
	        Assert.AreEqual(2, result.First.Length);
	        Assert.AreEqual(2.0, result.First[0].Get("maxVol"));
	        Assert.AreEqual(2.0, result.First[1].Get("maxVol"));
	        Assert.AreEqual(1, result.Second.Length);
	        Assert.AreEqual(null, result.Second[0].Get("maxVol"));
	    }

        [Test]
        public void testAggregateAllNoJoinLast()
		{
		    String viewExpr = "select longBoxed, Sum(longBoxed) as result " +
		    "from " + typeof(SupportBean).FullName + ".win:length(3) " +
		    "having Sum(longBoxed) > 0 " +
		    "output last every 2 events";

		    RunAssertLastSum(CreateStmtAndListenerNoJoin(viewExpr));

		    viewExpr = "select longBoxed, Sum(longBoxed) as result " +
		    "from " + typeof(SupportBean).FullName + ".win:length(3) " +
		    "output last every 2 events";

		    RunAssertLastSum(CreateStmtAndListenerNoJoin(viewExpr));
		}

        [Test]
        public void testAggregateAllJoinAll()
		{
		    String viewExpr = "select longBoxed, Sum(longBoxed) as result " +
	                        "from " + typeof(SupportBeanString).FullName + ".win:length(3) as one, " +
	                        typeof(SupportBean).FullName + ".win:length(3) as two " +
	                        "having Sum(longBoxed) > 0 " +
	                        "output all every 2 events";

		    RunAssertAllSum(CreateStmtAndListenerJoin(viewExpr));

		    viewExpr = "select longBoxed, Sum(longBoxed) as result " +
	                    "from " + typeof(SupportBeanString).FullName + ".win:length(3) as one, " +
	                    typeof(SupportBean).FullName + ".win:length(3) as two " +
	                    "output every 2 events";

		    RunAssertAllSum(CreateStmtAndListenerJoin(viewExpr));
		}

        [Test]
        public void testAggregateAllJoinLast()
	    {
	        String viewExpr = "select longBoxed, Sum(longBoxed) as result " +
	        "from " + typeof(SupportBeanString).FullName + ".win:length(3) as one, " +
	        typeof(SupportBean).FullName + ".win:length(3) as two " +
	        "having Sum(longBoxed) > 0 " +
	        "output last every 2 events";

	        RunAssertLastSum(CreateStmtAndListenerJoin(viewExpr));

	        viewExpr = "select longBoxed, Sum(longBoxed) as result " +
	        "from " + typeof(SupportBeanString).FullName + ".win:length(3) as one, " +
	        typeof(SupportBean).FullName + ".win:length(3) as two " +
	        "output last every 2 events";

	        RunAssertLastSum(CreateStmtAndListenerJoin(viewExpr));
	    }

        [Test]
        public void testTime()
	    {
	        // Clear any old events
	        epService.Initialize();

	        // Turn off external clocking
	        epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));

	        // Set the clock to 0
	        currentTime = 0;
	        SendTimeEvent(0);

	        // Create the eql statement and add a listener
	        String statementText = "select symbol, sum(volume) from " + EVENT_NAME + ".win:length(5) output first every 3 seconds";
	        EPStatement statement = epService.EPAdministrator.CreateEQL(statementText);
	        SupportUpdateListener updateListener = new SupportUpdateListener();
	        statement.AddListener(updateListener);
	        updateListener.Reset();

	        // Send the first event of the batch; should be output
	        SendMarketDataEvent(10L);
	        AssertEvent(updateListener, 10L);

	        // Send another event, not the first, for aggregation
	        // update only, no output
	        SendMarketDataEvent(20L);
	        Assert.IsFalse(updateListener.GetAndClearIsInvoked());

	        // Update time
	        SendTimeEvent(3000);
	        Assert.IsFalse(updateListener.GetAndClearIsInvoked());

	        // Send first event of the next batch, should be output.
	        // The aggregate value is computed over all events
	        // received: 10 + 20 + 30 = 60
	        SendMarketDataEvent(30L);
	        AssertEvent(updateListener, 60L);

	        // Send the next event of the batch, no output
	        SendMarketDataEvent(40L);
	        Assert.IsFalse(updateListener.GetAndClearIsInvoked());

	        // Update time
	        SendTimeEvent(3000);
	        Assert.IsFalse(updateListener.GetAndClearIsInvoked());

	        // Send first event of third batch
	        SendMarketDataEvent(1L);
	        AssertEvent(updateListener, 101L);

	        // Update time
	        SendTimeEvent(3000);
	        Assert.IsFalse(updateListener.GetAndClearIsInvoked());

	        // Update time: no first event this batch, so a callback
	        // is made at the end of the interval
	        SendTimeEvent(3000);
	        Assert.IsTrue(updateListener.GetAndClearIsInvoked());
	        Assert.IsNull(updateListener.LastNewData);
	        Assert.IsNull(updateListener.LastOldData);
	    }

        [Test]
	    public void testCount()
	    {
	        // Create the eql statement and add a listener
	        String statementText = "select symbol, Sum(volume) from " + EVENT_NAME + ".win:length(5) output first every 3 events";
	        EPStatement statement = epService.EPAdministrator.CreateEQL(statementText);
	        SupportUpdateListener updateListener = new SupportUpdateListener();
	        statement.AddListener(updateListener);
	        updateListener.Reset();

	        // Send the first event of the batch, should be output
	        SendEventLong(10L);
	        AssertEvent(updateListener, 10L);

	        // Send the second event of the batch, not output, used
	        // for updating the aggregate value only
	        SendEventLong(20L);
	        Assert.IsFalse(updateListener.GetAndClearIsInvoked());

	        // Send the third event of the batch, still not output,
	        // but should reset the batch
	        SendEventLong(30L);
	        Assert.IsFalse(updateListener.GetAndClearIsInvoked());

	        // First event, next batch, aggregate value should be
	        // 10 + 20 + 30 + 40 = 100
	        SendEventLong(40L);
	        AssertEvent(updateListener, 100L);

	        // Next event again not output
	        SendEventLong(50L);
	        Assert.IsFalse(updateListener.GetAndClearIsInvoked());
	    }

	    private void SendEventLong(long volume)
	    {
	        epService.EPRuntime.SendEvent(new SupportMarketDataBean("DELL", 0.0, volume, null));
	    }

	    private SupportUpdateListener CreateStmtAndListenerNoJoin(String viewExpr) {
			epService.Initialize();
			SupportUpdateListener updateListener = new SupportUpdateListener();
			EPStatement view = epService.EPAdministrator.CreateEQL(viewExpr);
		    view.AddListener(updateListener);

		    return updateListener;
		}

		private void RunAssertAllSum(SupportUpdateListener updateListener)
		{
			// send an event
		    SendEvent(1);

		    // check no update
		    Assert.IsFalse(updateListener.GetAndClearIsInvoked());

		    // send another event
		    SendEvent(2);

		    // check update, all events present
		    Assert.IsTrue(updateListener.GetAndClearIsInvoked());
		    Assert.AreEqual(2, updateListener.LastNewData.Length);
		    Assert.AreEqual(1L, updateListener.LastNewData[0].Get("longBoxed"));
		    Assert.AreEqual(3L, updateListener.LastNewData[0].Get("result"));
		    Assert.AreEqual(2L, updateListener.LastNewData[1].Get("longBoxed"));
		    Assert.AreEqual(3L, updateListener.LastNewData[1].Get("result"));
		    Assert.IsNull(updateListener.LastOldData);
		}

		private void RunAssertLastSum(SupportUpdateListener updateListener)
		{
			// send an event
		    SendEvent(1);

		    // check no update
		    Assert.IsFalse(updateListener.GetAndClearIsInvoked());

		    // send another event
		    SendEvent(2);

		    // check update, all events present
		    Assert.IsTrue(updateListener.GetAndClearIsInvoked());
		    Assert.AreEqual(1, updateListener.LastNewData.Length);
		    Assert.AreEqual(2L, updateListener.LastNewData[0].Get("longBoxed"));
		    Assert.AreEqual(3L, updateListener.LastNewData[0].Get("result"));
		    Assert.IsNull(updateListener.LastOldData);
		}

	    private void SendEvent(long longBoxed, int intBoxed, short shortBoxed)
		{
		    SupportBean bean = new SupportBean();
		    bean.SetString(JOIN_KEY);
		    bean.SetLongBoxed(longBoxed);
		    bean.SetIntBoxed(intBoxed);
		    bean.SetShortBoxed(shortBoxed);
		    epService.EPRuntime.SendEvent(bean);
		}

		private void SendEvent(long longBoxed)
		{
		    SendEvent(longBoxed, 0, (short)0);
		}

	    private void SendMarketDataEvent(long volume)
	    {
	        epService.EPRuntime.SendEvent(new SupportMarketDataBean("SYM1", 0, volume, null));
	    }

	    private void SendTimeEvent(int timeIncrement){
	        currentTime += timeIncrement;
	        CurrentTimeEvent _event = new CurrentTimeEvent(currentTime);
	        epService.EPRuntime.SendEvent(_event);
	    }

		private SupportUpdateListener CreateStmtAndListenerJoin(String viewExpr) {
			epService.Initialize();

			SupportUpdateListener updateListener = new SupportUpdateListener();
			EPStatement view = epService.EPAdministrator.CreateEQL(viewExpr);
		    view.AddListener(updateListener);

		    epService.EPRuntime.SendEvent(new SupportBeanString(JOIN_KEY));

		    return updateListener;
		}

	    private static void AssertEvent(SupportUpdateListener updateListener, long volume)
	    {
	        Assert.IsTrue(updateListener.GetAndClearIsInvoked());
	        Assert.IsTrue(updateListener.LastNewData != null);
	        Assert.AreEqual(1, updateListener.LastNewData.Length);
	        Assert.AreEqual(volume, updateListener.LastNewData[0].Get("sum(volume)"));
	    }

	    private void SendEvent(String symbol, double price)
		{
		    SupportMarketDataBean bean = new SupportMarketDataBean(symbol, price, 0L, null);
		    epService.EPRuntime.SendEvent(bean);
		}

	    private void SendTimer(long time)
	    {
	        CurrentTimeEvent _event = new CurrentTimeEvent(time);
	        EPRuntime runtime = epService.EPRuntime;
	        runtime.SendEvent(_event);
	    }
	}
} // End of namespace
