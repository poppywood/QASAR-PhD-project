using System;
using System.Collections.Generic;

using net.esper.client;
using net.esper.compat;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.regression.view
{
    [TestFixture]
    public class TestOrderByAggregation
    {
        private EPServiceProvider epService;
        private IList<String> symbols;
        private IList<double?> prices;
        private IList<long> volumes;
        private SupportUpdateListener testListener;

        [SetUp]
        public virtual void setUp()
        {
            EPServiceProviderManager.PurgeAllProviders();
            epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
            epService.Initialize();
            symbols = new List<String>();
            prices = new List<double?>();
            volumes = new List<long>();
        }

        [Test]
        public void testRowPerGroup()
        {
            String statementString = "select symbol, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) " + "group by symbol " + "output every 6 events " + "order by sum(price)";
            createAndSendAggregate(statementString);
            orderValuesBySumPriceGroup();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            sendAdditionalAggregate();
            orderValuesBySumPriceAdditionalGroup();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            clearValues();

            epService.Initialize();

            statementString = "select symbol, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) " + "group by symbol " + "having sum(price) > 0 " + "output every 6 events " + "order by sum(price)";
            createAndSendAggregate(statementString);
            orderValuesBySumPriceGroup();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            sendAdditionalAggregate();
            orderValuesBySumPriceAdditionalGroup();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            clearValues();
        }

        [Test]
        public void testRowPerGroupJoin()
        {
            String statementString = "select symbol, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) as one, " + typeof(SupportBeanString).FullName + ".win:length(100) as two " + "where one.symbol = two.string " + "group by symbol " + "output every 6 events " + "order by sum(price)";
            createAndSendAggregate(statementString);
            sendJoinEvents();
            orderValuesBySumPriceGroup();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            sendAdditionalAggregate();
            orderValuesBySumPriceAdditionalGroup();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            clearValues();

            epService.Initialize();

            statementString = "select symbol, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) as one, " + typeof(SupportBeanString).FullName + ".win:length(100) as two " + "where one.symbol = two.string " + "group by symbol " + "having sum(price) > 0 " + "output every 6 events " + "order by sum(price)";
            createAndSendAggregate(statementString);
            sendJoinEvents();
            orderValuesBySumPriceGroup();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            sendAdditionalAggregate();
            orderValuesBySumPriceAdditionalGroup();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            clearValues();
        }

        [Test]
        public void testAliases()
        {
            String statementString = "select symbol, volume, sum(price) as mySum from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) " + "group by symbol " + "output every 6 events " + "order by mySum";
            createAndSendAggregate(statementString);
            orderValuesBySumPriceEvent();
            assertValues(prices, "mySum");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "mySum", "volume" });
            clearValues();

            statementString = "select symbol as mySymbol, sum(price) as mySum from " + typeof(SupportMarketDataBean).FullName + ".win:length(10) " + "output every 6 events " + "order by mySymbol";
            createAndSendAggregate(statementString);
            orderValuesBySymbolAggregateAll();
            assertValues(symbols, "mySymbol");
            assertValues(prices, "mySum");
            clearValues();

            statementString = "select symbol, sum(price) as mySum from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) " + "group by symbol " + "output every 6 events " + "order by mySum";
            createAndSendAggregate(statementString);
            orderValuesBySumPriceGroup();
            assertValues(prices, "mySum");
            assertValues(symbols, "symbol");
            assertOnlyProperties(new String[] { "symbol", "mySum" });
            clearValues();
        }

        [Test]
        public void testGroupBySwitch()
        {
            // Instead of the row-per-group behavior, these should
            // get row-per-event behavior since there are properties 
            // in the order-by that are not in the select expression.
            String statementString = "select symbol, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) " + "group by symbol " + "output every 6 events " + "order by sum(price), volume";
            createAndSendAggregate(statementString);
            orderValuesBySumPriceEvent();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            statementString = "select symbol, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) as one, " + typeof(SupportBeanString).FullName + ".win:length(100) as two " + "where one.symbol = two.string " + "group by symbol " + "output every 6 events " + "order by sum(price), volume";
            createAndSendAggregate(statementString);
            sendJoinEvents();
            orderValuesBySumPriceEvent();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();
        }

        [Test]
        public void testLast()
        {
            String statementString = "select symbol, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) " + "group by symbol " + "output last every 6 events " + "order by sum(price)";
            createAndSendAggregate(statementString);
            orderValuesBySumPriceGroup();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            sendAdditionalAggregate();
            orderValuesBySumPriceAdditionalGroupLast();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) as one, " + typeof(SupportBeanString).FullName + ".win:length(100) as two " + "where one.symbol = two.string " + "group by symbol " + "output last every 6 events " + "order by sum(price)";
            createAndSendAggregate(statementString);
            sendJoinEvents();
            orderValuesBySumPriceGroup();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            sendAdditionalAggregate();
            orderValuesBySumPriceAdditionalGroupLast();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, volume, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) " + "group by symbol " + "output last every 6 events " + "order by sum(price)";
            createAndSendAggregate(statementString);
            orderValuesBySumPriceEvent();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "sum(price)", "volume" });
            clearValues();

            sendAdditionalAggregate();
            orderValuesBySumPriceAdditionalEventGroup();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "sum(price)", "volume" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, volume, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) as one, " + typeof(SupportBeanString).FullName + ".win:length(100) as two " + "where one.symbol = two.string " + "group by symbol " + "output last every 6 events " + "order by sum(price)";
            createAndSendAggregate(statementString);
            sendJoinEvents();
            orderValuesBySumPriceEvent();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "sum(price)", "volume" });
            clearValues();

            sendAdditionalAggregate();
            orderValuesBySumPriceAdditionalEventGroup();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "sum(price)", "volume" });
            clearValues();

            epService.Initialize();
        }

        [Test]
        public void testAggregateGrouped()
        {
            String statementString = "select symbol, volume, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) " + "group by symbol " + "output every 6 events " + "order by sum(price)";
            createAndSendAggregate(statementString);
            orderValuesBySumPriceEvent();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "sum(price)", "volume" });
            clearValues();

            sendAdditionalAggregate();
            orderValuesBySumPriceAdditionalEvent();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "sum(price)", "volume" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, volume, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) " + "group by symbol " + "having sum(price) > 0 " + "output every 6 events " + "order by sum(price)";
            createAndSendAggregate(statementString);
            orderValuesBySumPriceEvent();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "sum(price)", "volume" });
            clearValues();

            sendAdditionalAggregate();
            orderValuesBySumPriceAdditionalEvent();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "sum(price)", "volume" });
            clearValues();
        }

        [Test]
        public void testAggregateGroupedJoin()
        {
            String statementString = "select symbol, volume, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) as one, " + typeof(SupportBeanString).FullName + ".win:length(100) as two " + "where one.symbol = two.string " + "group by symbol " + "output every 6 events " + "order by sum(price)";
            createAndSendAggregate(statementString);
            sendJoinEvents();
            orderValuesBySumPriceEvent();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "sum(price)", "volume" });
            clearValues();

            sendAdditionalAggregate();
            orderValuesBySumPriceAdditionalEvent();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "sum(price)", "volume" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, volume, sum(price) from " + typeof(SupportMarketDataBean).FullName + ".win:length(20) as one, " + typeof(SupportBeanString).FullName + ".win:length(100) as two " + "where one.symbol = two.string " + "group by symbol " + "having sum(price) > 0 " + "output every 6 events " + "order by sum(price)";
            createAndSendAggregate(statementString);
            sendJoinEvents();
            orderValuesBySumPriceEvent();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "sum(price)", "volume" });
            clearValues();

            sendAdditionalAggregate();
            orderValuesBySumPriceAdditionalEvent();
            assertValues(prices, "sum(price)");
            assertValues(symbols, "symbol");
            assertValues(volumes, "volume");
            assertOnlyProperties(new String[] { "symbol", "sum(price)", "volume" });
            clearValues();
        }

        [Test]
        public void testAggregateAll()
        {
            String statementString = "select symbol, sum(price) from " +
            typeof(SupportMarketDataBean).FullName + ".win:length(10) " +
            "output every 6 events " +
            "order by symbol";
            createAndSendAggregate(statementString);
            orderValuesBySymbolAggregateAll();
            assertValues(symbols, "symbol");
            assertValues(prices, "sum(price)");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, max(sum(price)) from " +
                                typeof(SupportMarketDataBean).FullName + ".win:length(10) " +
                                "output every 6 events " +
                                "order by symbol";
            createAndSendAggregate(statementString);
            orderValuesBySymbolAggregateAll();
            assertValues(symbols, "symbol");
            assertValues(prices, "max(sum(price))");
            assertOnlyProperties(new String[] { "symbol", "max(sum(price))" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, sum(price) from " +
            typeof(SupportMarketDataBean).FullName + ".win:length(10) " +
            "having sum(price) > 0 " +
            "output every 6 events " +
            "order by symbol";
            createAndSendAggregate(statementString);
            orderValuesBySymbolAggregateAll();
            assertValues(symbols, "symbol");
            assertValues(prices, "sum(price)");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, sum(price) from " +
            typeof(SupportMarketDataBean).FullName + ".win:length(10) " +
            "output every 6 events " +
            "order by symbol, sum(price)";
            createAndSendAggregate(statementString);
            orderValuesBySymbolAggregateAll();
            assertValues(symbols, "symbol");
            assertValues(prices, "sum(price)");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, 21+sum(price)*0 from " +
            typeof(SupportMarketDataBean).FullName + ".win:length(10) " +
            "output every 6 events " +
            "order by symbol, 2*sum(price)+1";
            createAndSendAggregate(statementString);
            orderValuesBySymbolAggregateAll();
            assertValues(symbols, "symbol");
            assertValues(prices, "(21+(sum(price)*0))");
            assertOnlyProperties(new String[] { "symbol", "(21+(sum(price)*0))" });
            clearValues();
        }

        public void testAggregateAllJoin()
        {
            String statementString = "select symbol, sum(price) from " +
            typeof(SupportMarketDataBean).FullName + ".win:length(10) as one, " +
            typeof(SupportBeanString).FullName + ".win:length(100) as two " +
            "where one.symbol = two.string " +
            "output every 6 events " +
            "order by symbol";
            createAndSendAggregate(statementString);
            sendJoinEvents();
            orderValuesBySymbolAggregateAll();
            assertValues(symbols, "symbol");
            assertValues(prices, "sum(price)");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, max(sum(price)) from " +
            typeof(SupportMarketDataBean).FullName + ".win:length(10) as one, " +
            typeof(SupportBeanString).FullName + ".win:length(100) as two " +
            "where one.symbol = two.string " +
            "output every 6 events " +
            "order by symbol";
            createAndSendAggregate(statementString);
            sendJoinEvents();
            orderValuesBySymbolAggregateAll();
            assertValues(symbols, "symbol");
            assertValues(prices, "max(sum(price))");
            assertOnlyProperties(new String[] { "symbol", "max(sum(price))" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, sum(price) from " +
            typeof(SupportMarketDataBean).FullName + ".win:length(10) as one, " +
            typeof(SupportBeanString).FullName + ".win:length(100) as two " +
            "where one.symbol = two.string " +
            "having sum(price) > 0 " +
            "output every 6 events " +
            "order by symbol";
            createAndSendAggregate(statementString);
            sendJoinEvents();
            orderValuesBySymbolAggregateAll();
            assertValues(symbols, "symbol");
            assertValues(prices, "sum(price)");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, sum(price) from " +
            typeof(SupportMarketDataBean).FullName + ".win:length(10) as one, " +
            typeof(SupportBeanString).FullName + ".win:length(100) as two " +
            "where one.symbol = two.string " +
            "output every 6 events " +
            "order by symbol, sum(price)";
            createAndSendAggregate(statementString);
            sendJoinEvents();
            orderValuesBySymbolAggregateAll();
            assertValues(symbols, "symbol");
            assertValues(prices, "sum(price)");
            assertOnlyProperties(new String[] { "symbol", "sum(price)" });
            clearValues();

            epService.Initialize();

            statementString = "select symbol, 21+sum(price)*0 from " +
            typeof(SupportMarketDataBean).FullName + ".win:length(10) as one, " +
            typeof(SupportBeanString).FullName + ".win:length(100) as two " +
            "where one.symbol = two.string " +
            "output every 6 events " +
            "order by symbol, 2*sum(price)+1";
            createAndSendAggregate(statementString);
            sendJoinEvents();
            orderValuesBySymbolAggregateAll();
            assertValues(symbols, "symbol");
            assertValues(prices, "(21+(sum(price)*0))");
            assertOnlyProperties(new String[] { "symbol", "(21+(sum(price)*0))" });
            clearValues();
        }

        private void SendEvent(String symbol, double price)
        {
            SupportMarketDataBean bean = new SupportMarketDataBean(symbol, price, 0L, null);
            epService.EPRuntime.SendEvent(bean);
        }

        private void assertOnlyProperties(IEnumerable<String> requiredProperties)
        {
            EventBean[] events = testListener.LastNewData;
            if (events == null || events.Length == 0)
            {
                return;
            }
            EventType type = events[0].EventType;
            List<String> actualProperties = new List<String>(type.PropertyNames);
            log.Debug(".assertOnlyProperties actualProperties==" + actualProperties);
            Assert.IsTrue(CollectionHelper.ContainsAll(actualProperties, requiredProperties));
            CollectionHelper.RemoveAll(actualProperties, requiredProperties);
            Assert.IsTrue(actualProperties.Count == 0);
        }

        private void assertValues<T>(IList<T> values, String valueName)
        {
            EventBean[] events = testListener.LastNewData;
            Assert.AreEqual(values.Count, events.Length);
            log.Debug(".assertValues values: " + values);
            for (int i = 0; i < events.Length; i++)
            {
                log.Debug(".assertValues events[" + i + "]==" + events[i].Get(valueName));
                Assert.AreEqual(values[i], events[i].Get(valueName));
            }
        }

        private void clearValues()
        {
            prices.Clear();
            volumes.Clear();
            symbols.Clear();
        }


        private void sendJoinEvents()
        {
            epService.EPRuntime.SendEvent(new SupportBeanString("CAT"));
            epService.EPRuntime.SendEvent(new SupportBeanString("IBM"));
            epService.EPRuntime.SendEvent(new SupportBeanString("CMU"));
            epService.EPRuntime.SendEvent(new SupportBeanString("KGB"));
            epService.EPRuntime.SendEvent(new SupportBeanString("DOG"));
        }

        private void orderValuesBySumPriceAdditionalEvent()
        {
            symbols.Add("DOG");
            symbols.Add("DOG");
            symbols.Add("CAT");
            symbols.Add("CMU");
            symbols.Add("CMU");
            symbols.Add("IBM");
            symbols.Add("IBM");
            prices.Add(1d);
            prices.Add(1d);
            prices.Add(11d);
            prices.Add(13d);
            prices.Add(13d);
            prices.Add(14d);
            prices.Add(14d);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
        }

        private void orderValuesBySumPriceAdditionalEventGroup()
        {
            symbols.Add("DOG");
            symbols.Add("DOG");
            symbols.Add("CMU");
            symbols.Add("CMU");
            symbols.Add("IBM");
            symbols.Add("IBM");
            prices.Add(1d);
            prices.Add(1d);
            prices.Add(13d);
            prices.Add(13d);
            prices.Add(14d);
            prices.Add(14d);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
        }

        private void orderValuesBySumPriceEvent()
        {
            symbols.Add("CMU");
            symbols.Add("CMU");
            symbols.Add("IBM");
            symbols.Add("IBM");
            symbols.Add("CAT");
            symbols.Add("CAT");
            prices.Add(3d);
            prices.Add(3d);
            prices.Add(7d);
            prices.Add(7d);
            prices.Add(11d);
            prices.Add(11d);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
        }

        private void orderValuesBySumPriceAdditionalGroup()
        {
            symbols.Add("DOG");
            symbols.Add("CAT");
            symbols.Add("CMU");
            symbols.Add("IBM");
            prices.Add(1d);
            prices.Add(11d);
            prices.Add(13d);
            prices.Add(14d);
        }

        private void orderValuesBySumPriceAdditionalGroupLast()
        {
            symbols.Add("DOG");
            symbols.Add("CMU");
            symbols.Add("IBM");
            prices.Add(1d);
            prices.Add(13d);
            prices.Add(14d);
        }

        private void sendAdditionalAggregate()
        {
            SendEvent("IBM", 3);
            SendEvent("IBM", 4);
            SendEvent("CMU", 5);
            SendEvent("CMU", 5);
            SendEvent("DOG", 0);
            SendEvent("DOG", 1);
        }

        private void orderValuesBySymbolAggregateAll()
        {
            symbols.Add("CAT");
            symbols.Add("CAT");
            symbols.Add("CMU");
            symbols.Add("CMU");
            symbols.Add("IBM");
            symbols.Add("IBM");
            prices.Add(21d);
            prices.Add(21d);
            prices.Add(21d);
            prices.Add(21d);
            prices.Add(21d);
            prices.Add(21d);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
            volumes.Add(0L);
        }

        private void createAndSendAggregate(String statementString)
        {
            testListener = new SupportUpdateListener();
            EPStatement statement = epService.EPAdministrator.CreateEQL(statementString);
            statement.AddListener(testListener);
            SendEvent("IBM", 3);
            SendEvent("IBM", 4);
            SendEvent("CMU", 1);
            SendEvent("CMU", 2);
            SendEvent("CAT", 5);
            SendEvent("CAT", 6);
        }

        private void orderValuesBySumPriceGroup()
        {
            symbols.Add("CMU");
            symbols.Add("IBM");
            symbols.Add("CAT");
            prices.Add(3d);
            prices.Add(7d);
            prices.Add(11d);
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
