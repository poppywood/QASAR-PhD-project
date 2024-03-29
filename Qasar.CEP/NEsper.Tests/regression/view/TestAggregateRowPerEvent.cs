using System;

using net.esper.client;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.regression.view
{
    [TestFixture]
    public class TestAggregateRowPerEvent
    {
        private const String JOIN_KEY = "KEY";

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
        public void testSumOneView()
        {
            String viewExpr = "select longPrimitive, sum(longBoxed) as mySum " + "from " + typeof(SupportBean).FullName + ".win:length(3)";
            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            runAssert();
        }

        [Test]
        public void testSumJoin()
        {
            String viewExpr = "select longPrimitive, sum(longBoxed) as mySum " + "from " + typeof(SupportBeanString).FullName + ".win:length(3) as one, " + typeof(SupportBean).FullName + ".win:length(3) as two " + "where one.string = two.string";

            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            epService.EPRuntime.SendEvent(new SupportBeanString(JOIN_KEY));

            runAssert();
        }

        [Test]
        public void testSumAvgWithWhere()
        {
            String viewExpr = "select 'IBM stats' as title, volume, avg(volume) as myAvg, sum(volume) as mySum " + "from " + typeof(SupportMarketDataBean).FullName + ".win:length(3)" + "where symbol='IBM'";
            selectTestView = epService.EPAdministrator.CreateEQL(viewExpr);
            selectTestView.AddListener(testListener);

            sendMarketDataEvent("GE", 10L);
            Assert.IsFalse(testListener.IsInvoked);

            sendMarketDataEvent("IBM", 20L);
            assertPostedNew(20d, 20L);

            sendMarketDataEvent("XXX", 10000L);
            Assert.IsFalse(testListener.IsInvoked);

            sendMarketDataEvent("IBM", 30L);
            assertPostedNew(25d, 50L);
        }

        private void assertPostedNew(double? newAvg, Int64 newSum)
        {
            EventBean[] oldData = testListener.LastOldData;
            EventBean[] newData = testListener.LastNewData;

            Assert.IsNull(oldData);
            Assert.AreEqual(1, newData.Length);

            Assert.AreEqual("IBM stats", newData[0]["title"]);
            Assert.AreEqual(newAvg, newData[0]["myAvg"]);
            Assert.AreEqual(newSum, newData[0]["mySum"]);

            testListener.Reset();
        }

        private void runAssert()
        {
            // assert select result type
            Assert.AreEqual(typeof(long?), selectTestView.EventType.GetPropertyType("mySum"));

            SendEvent(10);
            Assert.AreEqual(10L, testListener.GetAndResetLastNewData()[0]["mySum"]);
            SendEvent(15);
            Assert.AreEqual(25L, testListener.GetAndResetLastNewData()[0]["mySum"]);
            SendEvent(-5);
            Assert.AreEqual(20L, testListener.GetAndResetLastNewData()[0]["mySum"]);
            Assert.IsNull(testListener.LastOldData);

            SendEvent(-2);
            Assert.AreEqual(20L, testListener.LastOldData[0]["mySum"]);
            Assert.AreEqual(8L, testListener.GetAndResetLastNewData()[0]["mySum"]);

            SendEvent(100);
            Assert.AreEqual(8L, testListener.LastOldData[0]["mySum"]);
            Assert.AreEqual(93L, testListener.GetAndResetLastNewData()[0]["mySum"]);

            SendEvent(1000);
            Assert.AreEqual(93L, testListener.LastOldData[0]["mySum"]);
            Assert.AreEqual(1098L, testListener.GetAndResetLastNewData()[0]["mySum"]);
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

        private void sendMarketDataEvent(String symbol, Int64 volume)
        {
            SupportMarketDataBean bean = new SupportMarketDataBean(symbol, 0, volume, null);
            epService.EPRuntime.SendEvent(bean);
        }

        private void SendEvent(long longBoxed)
        {
            SendEvent(longBoxed, 0, (short)0);
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
