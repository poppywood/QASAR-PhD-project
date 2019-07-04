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
    public class TestGroupByEventPerGroup
    {
        private static String SYMBOL_DELL = "DELL";
        private static String SYMBOL_IBM = "IBM";

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
            epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
        }

        [Test]
        public void testSumOneView()
        {
            String viewExpr = "select symbol," + "sum(price) as mySum," + "avg(price) as myAvg " + "from " + typeof(SupportMarketDataBean).FullName + ".win:length(3) " + "where symbol='DELL' or symbol='IBM' or symbol='GE' " + "group by symbol";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            runAssertion();
        }

        [Test]
        public void testSumJoin()
        {
            String viewExpr = "select symbol," + "sum(price) as mySum," + "avg(price) as myAvg " + "from " + typeof(SupportBeanString).FullName + ".win:length(100) as one, " + typeof(SupportMarketDataBean).FullName + ".win:length(3) as two " + "where (symbol='DELL' or symbol='IBM' or symbol='GE') " + "       and one.string = two.symbol " + "group by symbol";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_DELL));
            epService.EPRuntime.SendEvent(new SupportBeanString(SYMBOL_IBM));
            epService.EPRuntime.SendEvent(new SupportBeanString("AAA"));

            runAssertion();
        }

        [Test]
        public void testUniqueInBatch()
        {
            String stmtOne = "insert into MyStream select symbol, price from " +
                             typeof (SupportMarketDataBean).FullName + ".win:time_batch(1 sec)";
            epService.EPAdministrator.CreateEQL(stmtOne);
            SendTimer(0);

            String viewExpr = "select symbol " +
                              "from MyStream.win:time_batch(1 sec).std:unique('symbol') " +
                              "group by symbol";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            SendEvent("IBM", 100);
            SendEvent("IBM", 101);
            SendEvent("IBM", 102);
            SendTimer(1000);
            Assert.IsFalse(testListener.IsInvoked);

            SendTimer(2000);
            UniformPair<EventBean[]> received = testListener.GetDataListsFlattened();
            Assert.AreEqual("IBM", received.First[0].Get("symbol"));
        }

        private void runAssertion()
        {
            // assert select result type
            Assert.AreEqual(typeof(String), selectTestView.EventType.GetPropertyType("symbol"));
            Assert.AreEqual(typeof(double?), selectTestView.EventType.GetPropertyType("mySum"));
            Assert.AreEqual(typeof(double?), selectTestView.EventType.GetPropertyType("myAvg"));

            SendEvent(SYMBOL_DELL, 10);
            assertEvents(SYMBOL_DELL, null, null, 10d, 10d);

            SendEvent(SYMBOL_DELL, 20);
            assertEvents(SYMBOL_DELL, 10d, 10d, 30d, 15d);

            SendEvent(SYMBOL_DELL, 100);
            assertEvents(SYMBOL_DELL, 30d, 15d, 130d, 130d / 3d);

            SendEvent(SYMBOL_DELL, 50);
            assertEvents(SYMBOL_DELL, 130d, 130 / 3d, 170d, 170 / 3d); // 20 + 100 + 50

            SendEvent(SYMBOL_DELL, 5);
            assertEvents(SYMBOL_DELL, 170d, 170 / 3d, 155d, 155 / 3d); // 100 + 50 + 5

            SendEvent("AAA", 1000);
            assertEvents(SYMBOL_DELL, 155d, 155d / 3, 55d, 55d / 2); // 50 + 5

            SendEvent(SYMBOL_IBM, 70);
            assertEvents(SYMBOL_DELL, 55d, 55 / 2d, 5, 5, SYMBOL_IBM, null, null, 70, 70); // Dell:5

            SendEvent("AAA", 2000);
            assertEvents(SYMBOL_DELL, 5d, 5d, null, null);

            SendEvent("AAA", 3000);
            Assert.IsFalse(testListener.IsInvoked);

            SendEvent("AAA", 4000);
            assertEvents(SYMBOL_IBM, 70d, 70d, null, null);
        }

        private void assertEvents(
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
                double? newSumOne,
                double? newAvgOne,
                string symbolTwo,
                double? oldSumTwo,
                double? oldAvgTwo,
                double? newSumTwo,
                double? newAvgTwo)
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

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}