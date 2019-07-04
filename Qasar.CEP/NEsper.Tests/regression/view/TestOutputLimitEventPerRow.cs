using System;

using net.esper.client;
using net.esper.client.time;
using net.esper.collection;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Core;
using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.regression.view
{
    [TestFixture]
    public class TestOutputLimitEventPerRow
    {
        private const String SYMBOL_DELL = "DELL";
        private const String SYMBOL_IBM = "IBM";

        private EPServiceProvider epService;
        private SupportUpdateListener testListener;
        private EPStatement selectTestView;

        [SetUp]
        public virtual void setUp()
        {
            testListener = new SupportUpdateListener();

            EPServiceProviderManager.PurgeAllProviders();
            epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
            epService.Initialize();
        }

        [Test]
        public void testJoinSortWindow()
        {
            epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
            SendTimer(0);

            String viewExpr = "select symbol, volume, max(price) as maxVol" +
                              " from " + typeof (SupportMarketDataBean).FullName + ".ext:sort('volume', true, 1) as s0," +
                              typeof (SupportBean).FullName + " as s1 where s1.string = s0.symbol " +
                              "group by symbol output every 1 seconds";
            EPStatement stmt = epService.EPAdministrator.CreateEQL(viewExpr);
            stmt.AddListener(testListener);
            epService.EPRuntime.SendEvent(new SupportBean("JOIN_KEY", -1));

            sendEvent("JOIN_KEY", 1d);
            sendEvent("JOIN_KEY", 2d);
            testListener.Reset();

            // moves all events out of the window,
            SendTimer(1000); // newdata is 2 eventa, old data is the same 2 events, therefore the sum is null
            UniformPair<EventBean[]> result = testListener.GetDataListsFlattened();
            Assert.AreEqual(2, result.First.Length);
            Assert.AreEqual(2.0, result.First[0].Get("maxVol"));
            Assert.AreEqual(2.0, result.First[1].Get("maxVol"));
            Assert.AreEqual(1, result.Second.Length);
            Assert.AreEqual(null, result.Second[0].Get("maxVol"));
        }

        [Test]
        public void testMaxTimeWindow()
        {
            epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
            SendTimer(0);

            String viewExpr = "select symbol, " +
                              "volume, max(price) as maxVol" +
                              " from " + typeof (SupportMarketDataBean).FullName + ".win:time(1 sec) " +
                              "group by symbol output every 1 seconds";
            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            sendEvent("SYM1", 1d);
            sendEvent("SYM1", 2d);
            testListener.Reset();

            // moves all events out of the window,
            SendTimer(1000); // newdata is 2 eventa, old data is the same 2 events, therefore the sum is null
            UniformPair<EventBean[]> result = testListener.GetDataListsFlattened();
            Assert.AreEqual(2, result.First.Length);
            Assert.AreEqual(null, result.First[0].Get("maxVol"));
            Assert.AreEqual(null, result.First[1].Get("maxVol"));
            Assert.AreEqual(2, result.Second.Length);
            Assert.AreEqual(null, result.Second[0].Get("maxVol"));
            Assert.AreEqual(null, result.Second[1].Get("maxVol"));
        }

        [Test]
        public void testNoJoinLast()
        {
            // Every event generates a new row, this time we sum the price by symbol and output volume
            String viewExpr =
                "select symbol, volume, sum(price) as mySum " +
                "from " + typeof(SupportMarketDataBean).FullName + ".win:length(5) " +
                "where symbol='DELL' or symbol='IBM' or symbol='GE' " +
                "group by symbol " +
                "output last every 2 events";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            runAssertionLast();
        }

        private void assertEvent(String symbol, double? mySum, Int64 volume)
        {
            EventBean[] newData = testListener.LastNewData;

            Assert.AreEqual(1, newData.Length);

            Assert.AreEqual(symbol, newData[0]["symbol"]);
            Assert.AreEqual(mySum, newData[0]["mySum"]);
            Assert.AreEqual(volume, newData[0]["volume"]);

            testListener.Reset();
            Assert.IsFalse(testListener.IsInvoked);
        }

        private void runAssertionSingle()
        {
            // assert select result type
            Assert.AreEqual(typeof(String), selectTestView.EventType.GetPropertyType("symbol"));
            Assert.AreEqual(typeof(double?), selectTestView.EventType.GetPropertyType("mySum"));
            Assert.AreEqual(typeof(long?), selectTestView.EventType.GetPropertyType("volume"));

            SendEvent(SYMBOL_DELL, 10, 100);
            Assert.IsTrue(testListener.IsInvoked);
            assertEvent(SYMBOL_DELL, 100d, 10L);

            SendEvent(SYMBOL_IBM, 15, 50);
            assertEvent(SYMBOL_IBM, 50d, 15L);
        }

        [Test]
        public void testNoOutputClauseView()
        {
            String viewExpr =
                "select symbol, volume, sum(price) as mySum " +
                "from " + typeof(SupportMarketDataBean).FullName + ".win:length(5) " +
                "where symbol='DELL' or symbol='IBM' or symbol='GE' " +
                "group by symbol ";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            runAssertionSingle();
        }

        [Test]
        public void testNoJoinAll()
        {
            // Every event generates a new row, this time we sum the price by symbol and output volume
            String viewExpr =
                "select symbol, volume, sum(price) as mySum " +
                "from " + typeof(SupportMarketDataBean).FullName + ".win:length(5) " +
                "where symbol='DELL' or symbol='IBM' or symbol='GE' " +
                "group by symbol " +
                "output all every 2 events";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            runAssertionAll();
        }

        [Test]
        public void testJoinAll()
        {
            // Every event generates a new row, this time we sum the price by symbol and output volume
            String viewExpr =
                "select symbol, volume, sum(price) as mySum " +
                "from " +
                typeof(SupportBeanString).FullName + ".win:length(100) as one, " +
                typeof(SupportMarketDataBean).FullName + ".win:length(5) as two " +
                "where (symbol='DELL' or symbol='IBM' or symbol='GE') " +
                "  and one.string = two.symbol " +
                "group by symbol " +
                "output all every 2 events";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_DELL));
            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_IBM));

            runAssertionAll();
        }

        [Test]
        public void testJoinLast()
        {
            // Every event generates a new row, this time we sum the price by symbol and output volume
            String viewExpr =
                "select symbol, volume, sum(price) as mySum " +
                "from " +
                typeof(SupportBeanString).FullName + ".win:length(100) as one, " +
                typeof(SupportMarketDataBean).FullName + ".win:length(5) as two " +
                "where (symbol='DELL' or symbol='IBM' or symbol='GE') " +
                "  and one.string = two.symbol " +
                "group by symbol " +
                "output last every 2 events";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_DELL));
            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_IBM));

            runAssertionLast();
        }

        [Test]
        public void testNoOutputClauseJoin()
        {
            // Every event generates a new row, this time we sum the price by symbol and output volume
            String viewExpr =
                "select symbol, volume, sum(price) as mySum " +
                "from " +
                typeof(SupportBeanString).FullName + ".win:length(100) as one, " +
                typeof(SupportMarketDataBean).FullName + ".win:length(5) as two " +
                "where (symbol='DELL' or symbol='IBM' or symbol='GE') " +
                "  and one.string = two.symbol " +
                "group by symbol " +
                "output last every 2 events";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_DELL));
            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_IBM));

            runAssertionLast();
        }

        private void runAssertionAll()
        {
            // assert select result type
            Assert.AreEqual(typeof(String), selectTestView.EventType.GetPropertyType("symbol"));
            Assert.AreEqual(typeof(long?), selectTestView.EventType.GetPropertyType("volume"));
            Assert.AreEqual(typeof(double?), selectTestView.EventType.GetPropertyType("mySum"));

            SendEvent(SYMBOL_IBM, 10000, 20);
            Assert.IsFalse(testListener.GetAndClearIsInvoked());

            SendEvent(SYMBOL_DELL, 10000, 51);
            assertTwoEvents(SYMBOL_IBM, 10000, 20, SYMBOL_DELL, 10000, 51);

            SendEvent(SYMBOL_DELL, 20000, 52);
            Assert.IsFalse(testListener.GetAndClearIsInvoked());

            SendEvent(SYMBOL_DELL, 40000, 45);
            assertThreeEvents(SYMBOL_IBM, 10000, 20, SYMBOL_DELL, 20000, 51 + 52 + 45, SYMBOL_DELL, 40000, 51 + 52 + 45);
        }

        private void runAssertionLast()
        {
            // assert select result type
            Assert.AreEqual(typeof(String), selectTestView.EventType.GetPropertyType("symbol"));
            Assert.AreEqual(typeof(long?), selectTestView.EventType.GetPropertyType("volume"));
            Assert.AreEqual(typeof(double?), selectTestView.EventType.GetPropertyType("mySum"));

            SendEvent(SYMBOL_DELL, 10000, 51);
            Assert.IsFalse(testListener.GetAndClearIsInvoked());

            SendEvent(SYMBOL_DELL, 20000, 52);
            assertTwoEvents(SYMBOL_DELL, 10000, 103, SYMBOL_DELL, 20000, 103);

            SendEvent(SYMBOL_DELL, 30000, 70);
            Assert.IsFalse(testListener.GetAndClearIsInvoked());

            SendEvent(SYMBOL_IBM, 10000, 20);
            assertTwoEvents(SYMBOL_DELL, 30000, 173, SYMBOL_IBM, 10000, 20);
        }

        private void assertTwoEvents(String symbol1, long volume1, double sum1, String symbol2, long volume2, double sum2)
        {
            EventBean[] oldData = testListener.LastOldData;
            EventBean[] newData = testListener.LastNewData;

            Assert.IsNull(oldData);
            Assert.AreEqual(2, newData.Length);

            if (matchesEvent(newData[0], symbol1, volume1, sum1))
            {
                Assert.IsTrue(matchesEvent(newData[1], symbol2, volume2, sum2));
            }
            else
            {
                Assert.IsTrue(matchesEvent(newData[0], symbol2, volume2, sum2));
                Assert.IsTrue(matchesEvent(newData[1], symbol1, volume1, sum1));
            }

            testListener.Reset();
            Assert.IsFalse(testListener.IsInvoked);
        }

        private void assertThreeEvents(String symbol1, long volume1, double sum1, String symbol2, long volume2, double sum2, String symbol3, long volume3, double sum3)
        {
            EventBean[] oldData = testListener.LastOldData;
            EventBean[] newData = testListener.LastNewData;

            Assert.IsNull(oldData);
            Assert.AreEqual(3, newData.Length);

            if (matchesEvent(newData[0], symbol1, volume1, sum1))
            {
                if (matchesEvent(newData[1], symbol2, volume2, sum2))
                {
                    Assert.IsTrue(matchesEvent(newData[2], symbol3, volume3, sum3));
                }
                else
                {
                    Assert.IsTrue(matchesEvent(newData[1], symbol3, volume3, sum3));
                    Assert.IsTrue(matchesEvent(newData[2], symbol2, volume2, sum2));
                }
            }
            else if (matchesEvent(newData[0], symbol2, volume2, sum2))
            {
                if (matchesEvent(newData[1], symbol1, volume1, sum1))
                {
                    Assert.IsTrue(matchesEvent(newData[2], symbol3, volume3, sum3));
                }
                else
                {
                    Assert.IsTrue(matchesEvent(newData[1], symbol3, volume3, sum3));
                    Assert.IsTrue(matchesEvent(newData[2], symbol1, volume1, sum1));
                }
            }
            else
            {
                if (matchesEvent(newData[1], symbol1, volume1, sum1))
                {
                    Assert.IsTrue(matchesEvent(newData[2], symbol2, volume2, sum2));
                }
                else
                {
                    Assert.IsTrue(matchesEvent(newData[1], symbol2, volume2, sum2));
                    Assert.IsTrue(matchesEvent(newData[2], symbol1, volume1, sum1));
                }
            }


            testListener.Reset();
            Assert.IsFalse(testListener.IsInvoked);
        }

        private bool matchesEvent(EventBean _event, String symbol, long volume, double sum)
        {
            return
                symbol.Equals(_event["symbol"]) &&
                Object.Equals(_event["volume"], volume) &&
                Object.Equals(_event["mySum"], sum);
        }

        private void SendEvent(String symbol, long volume, double price)
        {
            SupportMarketDataBean bean = new SupportMarketDataBean(symbol, price, volume, null);
            epService.EPRuntime.SendEvent(bean);
        }

        private void sendEvent(String symbol, double price)
        {
            SupportMarketDataBean bean = new SupportMarketDataBean(symbol, price, 0L, null);
            epService.EPRuntime.SendEvent(bean);
        }

        private void SendTimer(long timeInMSec)
        {
            CurrentTimeEvent _event = new CurrentTimeEvent(timeInMSec);
            EPRuntime runtime = epService.EPRuntime;
            runtime.SendEvent(_event);
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}