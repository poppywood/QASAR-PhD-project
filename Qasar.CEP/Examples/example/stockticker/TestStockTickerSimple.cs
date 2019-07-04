///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using net.esper.client;
using net.esper.client.time;
using net.esper.example.stockticker.eventbean;
using net.esper.example.stockticker.monitor;

using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.example.stockticker
{
	[TestFixture]
	public class TestStockTickerSimple
	{
	    private StockTickerEmittedListener listener;
	    private EPServiceProvider epService;

        [SetUp]
	    protected void SetUp()
	    {
	        listener = new StockTickerEmittedListener();

	        Configuration configuration = new Configuration();
            configuration.EngineDefaults.EventMeta.ClassPropertyResolutionStyle = PropertyResolutionStyle.CASE_INSENSITIVE;
	        configuration.AddEventTypeAlias("PriceLimit", typeof(PriceLimit).FullName);
	        configuration.AddEventTypeAlias("StockTick", typeof(StockTick).FullName);

	        epService = EPServiceProviderManager.GetProvider("TestStockTickerSimple", configuration);
	        epService.EPRuntime.AddEmittedListener(listener, null);

	        // To reduce logging noise and get max performance
	        epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
	    }

	    [Test]
	    public void TestStockTicker()
	    {
	        log.Info(".testStockTicker");

	        new StockTickerMonitor(epService);

	        PerformEventFlowTest();
	        PerformBoundaryTest();
	    }

	    public void PerformEventFlowTest()
	    {
	        String STOCK_NAME = "IBM.N";
	        double STOCK_PRICE = 50;
	        double LIMIT_PERCENT = 10;
	        double LIMIT_PERCENT_LARGE = 20;
	        String USER_ID_ONE = "junit";
	        String USER_ID_TWO = "jack";
	        String USER_ID_THREE = "anna";

	        double STOCK_PRICE_WITHIN_LIMIT_LOW = 46.0;
	        double STOCK_PRICE_OUTSIDE_LIMIT_LOW = 44.9;
	        double STOCK_PRICE_WITHIN_LIMIT_HIGH = 51.0;
	        double STOCK_PRICE_OUTSIDE_LIMIT_HIGH = 55.01;

	        log.Debug(".testEvents");
	        listener.ClearMatched();

	        // Set a limit
	        SendEvent(new PriceLimit(USER_ID_ONE, STOCK_NAME, LIMIT_PERCENT));
	        Assert.IsTrue(listener.Size == 0);

	        // First stock ticker sets the initial price
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE));

	        // Go within the limit, expect no response
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE_WITHIN_LIMIT_LOW));
	        Assert.IsTrue(listener.Size == 0);

	        // Go outside the limit, expect an event
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE_OUTSIDE_LIMIT_LOW));
	        Sleep(500);
	        Assert.IsTrue(listener.Size == 1);
	        listener.ClearMatched();

	        // Go within the limit, expect no response
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE_WITHIN_LIMIT_HIGH));
	        Assert.IsTrue(listener.Size == 0);

	        // Go outside the limit, expect an event
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE_OUTSIDE_LIMIT_HIGH));
	        Sleep(500);
	        Assert.IsTrue(listener.Size == 1);
	        LimitAlert alert = (LimitAlert) listener.MatchEvents[0];
	        listener.ClearMatched();
	        Assert.IsTrue(alert.InitialPrice == STOCK_PRICE);
	        Assert.IsTrue(alert.Limit.UserId.Equals(USER_ID_ONE));
	        Assert.IsTrue(alert.Limit.StockSymbol.Equals(STOCK_NAME));
	        Assert.IsTrue(alert.Limit.LimitPct == LIMIT_PERCENT);
	        Assert.IsTrue(alert.Tick.StockSymbol.Equals(STOCK_NAME));
	        Assert.IsTrue(alert.Tick.Price == STOCK_PRICE_OUTSIDE_LIMIT_HIGH);

	        // Set a new limit for the same stock
	        // With the new limit none of these should fire
	        SendEvent(new PriceLimit(USER_ID_ONE, STOCK_NAME, LIMIT_PERCENT_LARGE));
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE_WITHIN_LIMIT_LOW));
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE_OUTSIDE_LIMIT_LOW));
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE_WITHIN_LIMIT_HIGH));
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE_OUTSIDE_LIMIT_HIGH));
	        Sleep(500);
	        Assert.IsTrue(listener.Size == 0);

	        // Set a smaller limit for another couple of users
	        SendEvent(new PriceLimit(USER_ID_TWO, STOCK_NAME, LIMIT_PERCENT));
	        SendEvent(new PriceLimit(USER_ID_THREE, STOCK_NAME, LIMIT_PERCENT_LARGE));

	        // Set limit back to original limit, send same prices, expect exactly 2 event
	        SendEvent(new PriceLimit(USER_ID_ONE, STOCK_NAME, LIMIT_PERCENT));
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE_WITHIN_LIMIT_LOW));
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE_OUTSIDE_LIMIT_LOW));
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE_WITHIN_LIMIT_HIGH));
	        SendEvent(new StockTick(STOCK_NAME, STOCK_PRICE_OUTSIDE_LIMIT_HIGH));
	        Sleep(500);

	        log.Info(".performEventFlowTest listSize=" + listener.Size);
	        Assert.IsTrue(listener.Size == 4);
	    }

	    public void PerformBoundaryTest()
	    {
	        String STOCK_NAME = "BOUNDARY_TEST";

	        listener.ClearMatched();
	        SendEvent(new PriceLimit("junit", STOCK_NAME, 25.0));
	        SendEvent(new StockTick(STOCK_NAME, 46.0));
	        SendEvent(new StockTick(STOCK_NAME, 46.0 - 11.5));
	        SendEvent(new StockTick(STOCK_NAME, 46.0 + 11.5));
	        Sleep(500);
	        Assert.IsTrue(listener.Size == 0);

	        SendEvent(new StockTick(STOCK_NAME, 46.0 - 11.5001));
	        SendEvent(new StockTick(STOCK_NAME, 46.0 + 11.5001));
	        Sleep(500);
	        Assert.IsTrue(listener.Size == 2);
	    }

        private void Sleep(int msec)
        {
            Thread.Sleep(msec);
        }

	    private void SendEvent(Object @event)
	    {
	        epService.EPRuntime.SendEvent(@event);
	    }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
} // End of namespace
