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
    public class TestOutputLimitlEventPerGroup
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

            String viewExpr = "select symbol, max(price) as maxVol" +
                              " from " + typeof (SupportMarketDataBean).FullName + ".ext:sort('volume', true, 1) as s0," +
                              typeof (SupportBean).FullName + " as s1 " +
                              "group by symbol output every 1 seconds";
            EPStatement stmt = epService.EPAdministrator.CreateEQL(viewExpr);
            stmt.AddListener(testListener);
            epService.EPRuntime.SendEvent(new SupportBean("JOIN_KEY", -1));

            SendEvent("JOIN_KEY", 1d);
            SendEvent("JOIN_KEY", 2d);
            testListener.Reset();

            // moves all events out of the window,
            SendTimer(1000); // newdata is 2 eventa, old data is the same 2 events, therefore the sum is null
            UniformPair<EventBean[]> result = testListener.GetDataListsFlattened();
            Assert.AreEqual(1, result.First.Length);
            Assert.AreEqual(2.0, result.First[0].Get("maxVol"));
            Assert.AreEqual(1, result.Second.Length);
            Assert.AreEqual(null, result.Second[0].Get("maxVol"));
        }

        [Test]
        public void testWithGroupBy()
        {
            String eventName = typeof (SupportMarketDataBean).FullName;
            String statementString = "select symbol, sum(price) from " + eventName +
                                     ".win:length(5) group by symbol output every 5 events";
            EPStatement statement = epService.EPAdministrator.CreateEQL(statementString);
            SupportUpdateListener updateListener = new SupportUpdateListener();
            statement.AddListener(updateListener);

            // send some events and check that only the most recent
            // ones are kept
            SendEvent("IBM", 1D);
            SendEvent("IBM", 2D);
            SendEvent("HP", 1D);
            SendEvent("IBM", 3D);
            SendEvent("MAC", 1D);

            Assert.IsTrue(updateListener.GetAndClearIsInvoked());
            EventBean[] newData = updateListener.LastNewData;
            Assert.AreEqual(3, newData.Length);
            AssertSingleInstance(newData, "IBM");
            AssertSingleInstance(newData, "HP");
            AssertSingleInstance(newData, "MAC");
            EventBean[] oldData = updateListener.LastOldData;
            AssertSingleInstance(oldData, "IBM");
            AssertSingleInstance(oldData, "HP");
            AssertSingleInstance(oldData, "MAC");
        }

        [Test]
        public void testMaxTimeWindow()
        {
            epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
            SendTimer(0);

            String viewExpr = "select symbol, " +
                              "max(price) as maxVol" +
                              " from " + typeof (SupportMarketDataBean).FullName + ".win:time(1 sec) " +
                              "group by symbol output every 1 seconds";
            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            SendEvent("SYM1", 1d);
            SendEvent("SYM1", 2d);
            testListener.Reset();

            // moves all events out of the window,
            SendTimer(1000); // newdata is 2 eventa, old data is the same 2 events, therefore the sum is null
            UniformPair<EventBean[]> result = testListener.GetDataListsFlattened();
            Assert.AreEqual(1, result.First.Length);
            Assert.AreEqual(null, result.First[0].Get("maxVol"));
            Assert.AreEqual(1, result.Second.Length);
            Assert.AreEqual(null, result.Second[0].Get("maxVol"));
        }

        [Test]
        public void testNoJoinLast()
        {
            String viewExpr = "select symbol," + "sum(price) as mySum," + "avg(price) as myAvg " + "from " + typeof(SupportMarketDataBean).FullName + ".win:length(3) " + "where symbol='DELL' or symbol='IBM' or symbol='GE' " + "group by symbol " + "output last every 2 events";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            runAssertionLast();
        }

        [Test]
        public void testNoOutputClauseView()
        {
            String viewExpr = "select symbol," + "sum(price) as mySum," + "avg(price) as myAvg " + "from " + typeof(SupportMarketDataBean).FullName + ".win:length(3) " + "where symbol='DELL' or symbol='IBM' or symbol='GE' " + "group by symbol";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            runAssertionSingle();
        }

        [Test]
        public void testNoOutputClauseJoin()
        {
            String viewExpr = "select symbol," + "sum(price) as mySum," + "avg(price) as myAvg " + "from " + typeof(SupportBeanString).FullName + ".win:length(100) as one, " + typeof(SupportMarketDataBean).FullName + ".win:length(3) as two " + "where (symbol='DELL' or symbol='IBM' or symbol='GE') " + "       and one.string = two.symbol " + "group by symbol";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_DELL));
            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_IBM));
            epService.EPRuntime.SendEvent(new SupportBeanString("AAA"));

            runAssertionSingle();
        }

        [Test]
        public void testNoJoinAll()
        {
            String viewExpr =
                "select symbol," + "sum(price) as mySum," + "avg(price) as myAvg " +
                "from " + typeof(SupportMarketDataBean).FullName + ".win:length(5) " + 
                "where symbol='DELL' or symbol='IBM' or symbol='GE' " + 
                "group by symbol " +
                "output all every 2 events";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            runAssertionAll();
        }

        [Test]
        public void testJoinLast()
        {
            String viewExpr =
                "select symbol," + "sum(price) as mySum," + "avg(price) as myAvg " + 
                "from " +
                typeof(SupportBeanString).FullName + ".win:length(100) as one, " + 
                typeof(SupportMarketDataBean).FullName + ".win:length(3) as two " + 
                "where (symbol='DELL' or symbol='IBM' or symbol='GE') " + 
                "       and one.string = two.symbol " +
                "group by symbol " +
                "output last every 2 events";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_DELL));
            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_IBM));
            epService.EPRuntime.SendEvent(new SupportBeanString("AAA"));

            runAssertionLast();
        }

        [Test]
        public void testJoinAll()
        {
            String viewExpr =
                "select symbol," + "sum(price) as mySum," + "avg(price) as myAvg " + 
                "from " + 
                typeof(SupportBeanString).FullName + ".win:length(100) as one, " + 
                typeof(SupportMarketDataBean).FullName + ".win:length(5) as two " + 
                "where (symbol='DELL' or symbol='IBM' or symbol='GE') " + 
                "       and one.string = two.symbol " +
                "group by symbol " +
                "output all every 2 events";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_DELL));
            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_IBM));
            epService.EPRuntime.SendEvent(new SupportBeanString("AAA"));

            runAssertionAll();
        }

        private void runAssertionLast()
        {
            // assert select result type
            Assert.AreEqual(typeof(String), selectTestView.EventType.GetPropertyType("symbol"));
            Assert.AreEqual(typeof(double?), selectTestView.EventType.GetPropertyType("mySum"));
            Assert.AreEqual(typeof(double?), selectTestView.EventType.GetPropertyType("myAvg"));

            SendEvent(SYMBOL_DELL, 10);
            Assert.IsFalse(testListener.IsInvoked);

            SendEvent(SYMBOL_DELL, 20);
            assertEvent(SYMBOL_DELL, null, null, 30d, 15d);
            testListener.Reset();

            SendEvent(SYMBOL_DELL, 100);
            Assert.IsFalse(testListener.IsInvoked);

            SendEvent(SYMBOL_DELL, 50);
            assertEvent(SYMBOL_DELL, 30d, 15d, 170d, 170 / 3d);
        }

        private void runAssertionSingle()
        {
            // assert select result type
            Assert.AreEqual(typeof(String), selectTestView.EventType.GetPropertyType("symbol"));
            Assert.AreEqual(typeof(double?), selectTestView.EventType.GetPropertyType("mySum"));
            Assert.AreEqual(typeof(double?), selectTestView.EventType.GetPropertyType("myAvg"));

            SendEvent(SYMBOL_DELL, 10);
            Assert.IsTrue(testListener.IsInvoked);
            assertEvent(SYMBOL_DELL, null, null, 10d, 10d);

            SendEvent(SYMBOL_IBM, 20);
            Assert.IsTrue(testListener.IsInvoked);
            assertEvent(SYMBOL_IBM, null, null, 20d, 20d);
        }

        private void runAssertionAll()
        {
            // assert select result type
            Assert.AreEqual(typeof(String), selectTestView.EventType.GetPropertyType("symbol"));
            Assert.AreEqual(typeof(double?), selectTestView.EventType.GetPropertyType("mySum"));
            Assert.AreEqual(typeof(double?), selectTestView.EventType.GetPropertyType("myAvg"));

            SendEvent(SYMBOL_IBM, 70);
            Assert.IsFalse(testListener.IsInvoked);

            SendEvent(SYMBOL_DELL, 10);
            assertEvents(
                SYMBOL_IBM, null, null, 70d, 70d,
                SYMBOL_DELL, null, null, 10d, 10d);
            testListener.Reset();

            SendEvent(SYMBOL_DELL, 20);
            Assert.IsFalse(testListener.IsInvoked);

            SendEvent(SYMBOL_DELL, 100);
            assertEvents(
                SYMBOL_IBM, null, null, 70d, 70d,
                SYMBOL_DELL, 10d, 10d, 130d, 130d / 3d);
        }

        private void assertEvent(
            String symbol,
            double? oldSum,
            double? oldAvg,
            double? newSum,
            double? newAvg)
        {
            EventBean[] oldData = testListener.LastOldData;
            EventBean[] newData = testListener.LastNewData;

            Assert.AreEqual(1, oldData.Length);
            Assert.AreEqual(1, newData.Length);

            Assert.AreEqual(symbol, oldData[0]["symbol"]);
            Assert.AreEqual(oldSum, oldData[0]["mySum"]);
            Assert.AreEqual(oldAvg, oldData[0]["myAvg"]);

            Assert.AreEqual(symbol, newData[0]["symbol"]);
            Assert.AreEqual(newSum, newData[0]["mySum"]);
            Assert.AreEqual(newAvg, newData[0]["myAvg"], "newData myAvg wrong");

            testListener.Reset();
            Assert.IsFalse(testListener.IsInvoked);
        }

        private void assertEvents(
            String symbolOne,
            double? oldSumOne,
            double? oldAvgOne,
            double newSumOne,
            double newAvgOne,
            String symbolTwo,
            double? oldSumTwo,
            double? oldAvgTwo,
            double newSumTwo,
            double newAvgTwo)
        {
            EventBean[] oldData = testListener.LastOldData;
            EventBean[] newData = testListener.LastNewData;

            Assert.AreEqual(2, oldData.Length);
            Assert.AreEqual(2, newData.Length);

            int indexOne = 0;
            int indexTwo = 1;
            if (oldData[0]["symbol"].Equals(symbolTwo))
            {
                indexTwo = 0;
                indexOne = 1;
            }
            Assert.AreEqual(newSumOne, newData[indexOne]["mySum"]);
            Assert.AreEqual(newSumTwo, newData[indexTwo]["mySum"]);
            Assert.AreEqual(oldSumOne, oldData[indexOne]["mySum"]);
            Assert.AreEqual(oldSumTwo, oldData[indexTwo]["mySum"]);

            Assert.AreEqual(newAvgOne, newData[indexOne]["myAvg"]);
            Assert.AreEqual(newAvgTwo, newData[indexTwo]["myAvg"]);
            Assert.AreEqual(oldAvgOne, oldData[indexOne]["myAvg"]);
            Assert.AreEqual(oldAvgTwo, oldData[indexTwo]["myAvg"]);

            testListener.Reset();
            Assert.IsFalse(testListener.IsInvoked);
        }

        private void SendEvent(String symbol, double price)
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

        private void AssertSingleInstance(EventBean[] data, String symbol)
        {
            int instanceCount = 0;
            foreach (EventBean _event in data)
            {
                if (_event.Get("symbol").Equals(symbol))
                {
                    instanceCount++;
                }
            }

            Assert.AreEqual(1, instanceCount);
        }
    }
}
