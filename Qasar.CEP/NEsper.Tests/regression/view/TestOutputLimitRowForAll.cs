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

using org.apache.commons.logging;

namespace net.esper.regression.view
{
    [TestFixture]
	public class TestOutputLimitRowForAll
	{
	    private EPServiceProvider epService;
	    private SupportUpdateListener listener;

        [SetUp]
	    public void SetUp()
	    {
	        epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
	        epService.Initialize();
	        listener = new SupportUpdateListener();
	    }

	    [Test]
        public void testJoinSortWindow()
	    {
	        epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
	        SendTimer(0);

	        String viewExpr = "select max(price) as maxVol" +
	                          " from " + typeof(SupportMarketDataBean).FullName + ".ext:sort('volume', true, 1) as s0," +
	                          typeof(SupportBean).FullName + " as s1 where s1.string = s0.symbol " +
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
	        Assert.AreEqual(1, result.First.Length);
	        Assert.AreEqual(2.0, result.First[0].Get("maxVol"));
	        Assert.AreEqual(1, result.Second.Length);
	        Assert.AreEqual(null, result.Second[0].Get("maxVol"));
	    }

   	    [Test]
	    public void testMaxTimeWindow()
	    {
	        epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
	        SendTimer(0);

	        String viewExpr = "select max(price) as maxVol" +
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
	        Assert.AreEqual(1, result.First.Length);
	        Assert.AreEqual(null, result.First[0].Get("maxVol"));
	        Assert.AreEqual(1, result.Second.Length);
	        Assert.AreEqual(null, result.Second[0].Get("maxVol"));
	    }

        [Test]
	    public void testTimeWindowOutputCount()
	    {
	        epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));

	        String stmtText = "select Count(*) as cnt from " + typeof(SupportBean).FullName + ".win:time(10 seconds) output every 10 seconds";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmt.AddListener(listener);

	        SendTimer(0);
	        SendTimer(10000);
	        Assert.IsFalse(listener.IsInvoked);
	        SendTimer(20000);
	        Assert.IsFalse(listener.IsInvoked);

	        SendEvent("e1");
	        SendTimer(30000);
	        EventBean[] newEvents = listener.GetAndResetLastNewData();
	        Assert.AreEqual(1, newEvents.Length);
	        Assert.AreEqual(0L, newEvents[0].Get("cnt"));

	        SendTimer(31000);

	        SendEvent("e2");
	        SendEvent("e3");
	        SendTimer(40000);
	        newEvents = listener.GetAndResetLastNewData();
	        Assert.AreEqual(1, newEvents.Length);
	        Assert.AreEqual(2L, newEvents[0].Get("cnt"));
	    }

        [Test]
	    public void testTimeBatchOutputCount()
	    {
	        epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));

	        String stmtText = "select Count(*) as cnt from " + typeof(SupportBean).FullName + ".win:time_batch(10 seconds) output every 10 seconds";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmt.AddListener(listener);

	        SendTimer(0);
	        SendTimer(10000);
	        Assert.IsFalse(listener.IsInvoked);
	        SendTimer(20000);
	        Assert.IsFalse(listener.IsInvoked);

	        SendEvent("e1");
	        SendTimer(30000);
	        Assert.IsFalse(listener.IsInvoked);
	        SendTimer(40000);
	        EventBean[] newEvents = listener.GetAndResetLastNewData();
	        Assert.AreEqual(1, newEvents.Length);
	        // output limiting starts 10 seconds after, therefore the old batch was posted already and the cnt is zero
	        Assert.AreEqual(0L, newEvents[0].Get("cnt"));

	        SendTimer(50000);
	        EventBean[] newData = listener.LastNewData;
	        Assert.AreEqual(0L, newData[0].Get("cnt"));
	        listener.Reset();

	        SendEvent("e2");
	        SendEvent("e3");
	        SendTimer(60000);
	        newEvents = listener.GetAndResetLastNewData();
	        Assert.AreEqual(1, newEvents.Length);
	        Assert.AreEqual(2L, newEvents[0].Get("cnt"));
	    }

	    private void SendEvent(String s)
		{
		    SupportBean bean = new SupportBean();
		    bean.SetString(s);
		    bean.SetDoubleBoxed(0.0);
		    bean.SetIntPrimitive(0);
		    bean.SetIntBoxed(0);
		    epService.EPRuntime.SendEvent(bean);
		}

	    private void SendTimer(long time)
	    {
	        CurrentTimeEvent _event = new CurrentTimeEvent(time);
	        EPRuntime runtime = epService.EPRuntime;
	        runtime.SendEvent(_event);
	    }

	    private void SendEvent(String symbol, double price)
		{
		    SupportMarketDataBean bean = new SupportMarketDataBean(symbol, price, 0L, null);
		    epService.EPRuntime.SendEvent(bean);
		}

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}


} // End of namespace
