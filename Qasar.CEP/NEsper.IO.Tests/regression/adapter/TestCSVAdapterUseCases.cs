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
using System.Text;
using System.Threading;

using net.esper.adapter;
using net.esper.adapter.csv;
using net.esper.client;
using net.esper.client.time;
using net.esper.compat;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.adapter
{
    [TestFixture]
	public class TestCSVAdapterUseCases
	{
        private static readonly String NEW_LINE = Environment.NewLine;
	    private const String CSV_FILENAME_ONELINE_TRADE = "regression/csvtest_tradedata.csv";
	    private const String CSV_FILENAME_TIMESTAMPED_PRICES = "regression/csvtest_timestamp_prices.csv";
	    private const String CSV_FILENAME_TIMESTAMPED_TRADES = "regression/csvtest_timestamp_trades.csv";

	    private EPServiceProvider epService;

	    /// <summary>
	    /// Play a CSV file using an existing event type definition (no timestamps).
	    /// Should not require a timestamp column, should block thread until played in.
	    /// </summary>
        [Test]
        public void TestExistingTypeNoOptions()
	    {
	        epService = EPServiceProviderManager.GetProvider("testExistingTypeNoOptions", MakeConfig("TypeA"));
	        epService.Initialize();

	        EPStatement stmt = epService.EPAdministrator.CreateEQL("select symbol, price, volume from TypeA.win:length(100)");
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmt.AddListener(listener);

	        (new CSVInputAdapter(epService, new AdapterInputSource(CSV_FILENAME_ONELINE_TRADE), "TypeA")).Start();

	        Assert.AreEqual(1, listener.NewDataList.Count);
	    }

	    /// <summary>Play a CSV file that is from memory.</summary>
        [Test]
        public void TestPlayFromInputStream()
	    {
	        String myCSV = "symbol, price, volume" + NEW_LINE + "IBM, 10.2, 10000";
	        MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(myCSV));
	        TrySource(new AdapterInputSource(stream));
	    }

	    /// <summary>Play a CSV file that is from memory.</summary>
        [Test]
        public void TestPlayFromStringReader()
	    {
	        String myCSV = "symbol, price, volume" + NEW_LINE + "IBM, 10.2, 10000";
	        StringReader reader = new StringReader(myCSV);
	        TrySource(new AdapterInputSource(reader));
	    }

	    /// <summary>Play a CSV file using an engine thread</summary>
        [Test]
        public void TestEngineThread()
	    {
	        epService = EPServiceProviderManager.GetProvider("testExistingTypeNoOptions", MakeConfig("TypeA"));
	        epService.Initialize();

	        EPStatement stmt = epService.EPAdministrator.CreateEQL("select symbol, price, volume from TypeA.win:length(100)");
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmt.AddListener(listener);

	        CSVInputAdapterSpec spec = new CSVInputAdapterSpec(new AdapterInputSource(CSV_FILENAME_ONELINE_TRADE), "TypeA");
	        spec.EventsPerSec = 1000;
	//        spec.SetLooping(true);
	        spec.IsUsingEngineThread = true;

	        InputAdapter inputAdapter = new CSVInputAdapter(epService, spec);
	        inputAdapter.Start();
	        Thread.Sleep(1000);
	//        inputAdapter.Stop();

	        Assert.AreEqual(1, listener.NewDataList.Count);
	    }

	    /// <summary>Play a CSV file using the application thread</summary>
        [Test]
        public void TestAppThread() 
	    {
	        epService = EPServiceProviderManager.GetProvider("testExistingTypeNoOptions", MakeConfig("TypeA"));
	        epService.Initialize();

	        EPStatement stmt = epService.EPAdministrator.CreateEQL("select symbol, price, volume from TypeA.win:length(100)");
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmt.AddListener(listener);

	        CSVInputAdapterSpec spec = new CSVInputAdapterSpec(new AdapterInputSource(CSV_FILENAME_ONELINE_TRADE), "TypeA");
	        spec.EventsPerSec = 1000;

	        InputAdapter inputAdapter = new CSVInputAdapter(epService, spec);
	        inputAdapter.Start();

	        Assert.AreEqual(1, listener.NewDataList.Count);
	    }

	    /// <summary>
	    /// Play a CSV file using no existing (dynamic) event type (no timestamp)
	    /// </summary>
        [Test]
        public void TestDynamicType()
	    {
	        CSVInputAdapterSpec spec = new CSVInputAdapterSpec(new AdapterInputSource(CSV_FILENAME_ONELINE_TRADE), "TypeB");

	        epService = EPServiceProviderManager.GetDefaultProvider();
	        epService.Initialize();

	        InputAdapter feed = new CSVInputAdapter(epService, spec);

	        EPStatement stmt = epService.EPAdministrator.CreateEQL("select symbol, price, volume from TypeB.win:length(100)");
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmt.AddListener(listener);

	        Assert.AreEqual(typeof(string), stmt.EventType.GetPropertyType("symbol"));
	        Assert.AreEqual(typeof(string), stmt.EventType.GetPropertyType("price"));
	        Assert.AreEqual(typeof(string), stmt.EventType.GetPropertyType("volume"));

	        feed.Start();
	        Assert.AreEqual(1, listener.NewDataList.Count);
	    }

        [Test]
	    public void TestCoordinated()
	    {
            EDictionary<String, Type> priceProps = new HashDictionary<String, Type>();
	        priceProps.Put("timestamp", typeof(long));
	        priceProps.Put("symbol", typeof(string));
	        priceProps.Put("price", typeof(double));

            EDictionary<String, Type> tradeProps = new HashDictionary<String, Type>();
	        tradeProps.Put("timestamp", typeof(long));
	        tradeProps.Put("symbol", typeof(string));
	        tradeProps.Put("notional", typeof(double));

	        Configuration config = new Configuration();
	        config.AddEventTypeAlias("TradeEvent", tradeProps);
	        config.AddEventTypeAlias("PriceEvent", priceProps);

	        epService = EPServiceProviderManager.GetProvider("testCoordinated", config);
	        epService.Initialize();
	        epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
	        epService.EPRuntime.SendEvent(new CurrentTimeEvent(0));

	        AdapterInputSource sourcePrices = new AdapterInputSource(CSV_FILENAME_TIMESTAMPED_PRICES);
	        CSVInputAdapterSpec inputPricesSpec = new CSVInputAdapterSpec(sourcePrices, "PriceEvent");
	        inputPricesSpec.TimestampColumn = "timestamp";
	        inputPricesSpec.PropertyTypes = priceProps;
	        CSVInputAdapter inputPrices = new CSVInputAdapter(inputPricesSpec);

	        AdapterInputSource sourceTrades = new AdapterInputSource(CSV_FILENAME_TIMESTAMPED_TRADES);
	        CSVInputAdapterSpec inputTradesSpec = new CSVInputAdapterSpec(sourceTrades, "TradeEvent");
	        inputTradesSpec.TimestampColumn = "timestamp";
	        inputTradesSpec.PropertyTypes = tradeProps;
	        CSVInputAdapter inputTrades = new CSVInputAdapter(inputTradesSpec);

	        EPStatement stmtPrices = epService.EPAdministrator.CreateEQL("select symbol, price from PriceEvent.win:length(100)");
	        SupportUpdateListener listenerPrice = new SupportUpdateListener();
	        stmtPrices.AddListener(listenerPrice);
	        EPStatement stmtTrade = epService.EPAdministrator.CreateEQL("select symbol, notional from TradeEvent.win:length(100)");
	        SupportUpdateListener listenerTrade = new SupportUpdateListener();
	        stmtTrade.AddListener(listenerTrade);

	        AdapterCoordinator coordinator = new AdapterCoordinatorImpl(epService, true);
	        coordinator.Coordinate(inputPrices);
	        coordinator.Coordinate(inputTrades);
	        coordinator.Start();

	        epService.EPRuntime.SendEvent(new CurrentTimeEvent(400));
	        Assert.IsFalse(listenerTrade.IsInvoked);
	        Assert.IsFalse(listenerPrice.IsInvoked);

	        // invoke read of events at 500 (see CSV)
	        epService.EPRuntime.SendEvent(new CurrentTimeEvent(1000));
	        Assert.AreEqual(1, listenerTrade.NewDataList.Count);
	        Assert.AreEqual(1, listenerPrice.NewDataList.Count);
	        listenerTrade.Reset(); listenerPrice.Reset();

	        // invoke read of price events at 1500 (see CSV)
	        epService.EPRuntime.SendEvent(new CurrentTimeEvent(2000));
	        Assert.AreEqual(0, listenerTrade.NewDataList.Count);
	        Assert.AreEqual(1, listenerPrice.NewDataList.Count);
	        listenerTrade.Reset(); listenerPrice.Reset();

	        // invoke read of trade events at 2500 (see CSV)
	        epService.EPRuntime.SendEvent(new CurrentTimeEvent(3000));
	        Assert.AreEqual(1, listenerTrade.NewDataList.Count);
	        Assert.AreEqual(0, listenerPrice.NewDataList.Count);
	        listenerTrade.Reset(); listenerPrice.Reset();
	    }

	    private static Configuration MakeConfig(String typeName)
	    {
            EDictionary<String, Type> eventProperties = new HashDictionary<String, Type>();
	        eventProperties.Put("symbol", typeof(string));
	        eventProperties.Put("price", typeof(double));
	        eventProperties.Put("volume", typeof (int));

	        Configuration configuration = new Configuration();
	        configuration.AddEventTypeAlias(typeName, eventProperties);

	        return configuration;
	    }

	    private void TrySource(AdapterInputSource source)
	    {
	        CSVInputAdapterSpec spec = new CSVInputAdapterSpec(source, "TypeC");

	        epService = EPServiceProviderManager.GetProvider("testPlayFromInputStream", MakeConfig("TypeC"));
	        epService.Initialize();
	        InputAdapter feed = new CSVInputAdapter(epService, spec);

	        EPStatement stmt = epService.EPAdministrator.CreateEQL("select * from TypeC.win:length(100)");
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmt.AddListener(listener);

	        feed.Start();
	        Assert.AreEqual(1, listener.NewDataList.Count);
	    }
	}
} // End of namespace
