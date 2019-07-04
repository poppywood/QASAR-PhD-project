using System;
using System.Collections.Generic;
using System.Threading;

using net.esper.client;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;
using net.esper.view;

using org.apache.commons.logging;

using NUnit.Framework;

namespace net.esper.regression.view
{
    [TestFixture]
    public class TestViewTimeBatchMean
    {
        private const String SYMBOL = "CSCO.O";

        private EPServiceProvider epService;
        private SupportUpdateListener testListener;
        private EPStatement timeBatchMean;

        [SetUp]
        public virtual void setUp()
        {
            testListener = new SupportUpdateListener();

            EPServiceProviderManager.PurgeAllProviders();
            epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
            epService.Initialize();

            // Set up a 2 second time window
            timeBatchMean = epService.EPAdministrator.CreateEQL("select * from " + typeof(SupportMarketDataBean).FullName + "(symbol='" + SYMBOL + "').win:time_batch(2).stat:uni('volume')");
            timeBatchMean.AddListener(testListener);
        }

        [Test]
        public void testTimeBatchMean()
        {
            testListener.Reset();
            CheckMeanIterator(Double.NaN);
            Assert.IsFalse(testListener.IsInvoked);

            // Send a couple of events, check mean
            SendEvent(SYMBOL, 500);
            SendEvent(SYMBOL, 1000);
            CheckMeanIterator(Double.NaN); // The iterator is still showing no result yet as no batch was released
            Assert.IsFalse(testListener.IsInvoked); // No new data posted to the iterator, yet

            // Sleep for 1 seconds
            Thread.Sleep(1000);

            // Send more events
            SendEvent(SYMBOL, 1000);
            SendEvent(SYMBOL, 1200);
            CheckMeanIterator(Double.NaN); // The iterator is still showing no result yet as no batch was released
            Assert.IsFalse(testListener.IsInvoked);

            // Sleep for 1.5 seconds, thus triggering a new batch
            Thread.Sleep(1500);
            CheckMeanIterator(925); // Now the statistics view received the first batch
            Assert.IsTrue(testListener.IsInvoked); // Listener has been invoked
            CheckMeanListener(925);

            // Send more events
            SendEvent(SYMBOL, 500);
            SendEvent(SYMBOL, 600);
            SendEvent(SYMBOL, 1000);
            CheckMeanIterator(925); // The iterator is still showing the old result as next batch not released
            Assert.IsFalse(testListener.IsInvoked);

            // Sleep for 1 seconds
            Thread.Sleep(1000);

            // Send more events
            SendEvent(SYMBOL, 200);
            CheckMeanIterator(925);
            Assert.IsFalse(testListener.IsInvoked);

            // Sleep for 1.5 seconds, thus triggering a new batch
            Thread.Sleep(1500);
            CheckMeanIterator(2300d / 4d); // Now the statistics view received the second batch, the mean now is over all events
            Assert.IsTrue(testListener.IsInvoked); // Listener has been invoked
            CheckMeanListener(2300d / 4d);

            // Send more events
            SendEvent(SYMBOL, 1200);
            CheckMeanIterator(2300d / 4d);
            Assert.IsFalse(testListener.IsInvoked);

            // Sleep for 2 seconds, no events received anymore
            Thread.Sleep(2000);
            CheckMeanIterator(1200); // statistics view received the third batch
            Assert.IsTrue(testListener.IsInvoked); // Listener has been invoked
            CheckMeanListener(1200);
        }

        private void SendEvent(String symbol, long volume)
        {
            SupportMarketDataBean _event = new SupportMarketDataBean(symbol, 0, volume, "");
            epService.EPRuntime.SendEvent(_event);
        }

        private void CheckMeanListener(double meanExpected)
        {
            Assert.IsTrue(testListener.LastNewData.Length == 1);
            EventBean listenerValues = testListener.LastNewData[0];
            CheckValue(listenerValues, meanExpected);
            testListener.Reset();
        }

        private void CheckMeanIterator(double meanExpected)
        {
            IEnumerator<EventBean> iterator = timeBatchMean.GetEnumerator();
            Assert.IsTrue(iterator.MoveNext());
            CheckValue(iterator.Current, meanExpected);
            Assert.IsFalse(iterator.MoveNext());
        }

        private static void CheckValue(EventBean values, double avgE)
        {
            double avg = GetDoubleValue(ViewFieldEnum.WEIGHTED_AVERAGE__AVERAGE, values);
            Assert.IsTrue(DoubleValueAssertionUtil.Equals(avg, avgE, 6));
        }

        private static double GetDoubleValue(ViewFieldEnum field, EventBean _event)
        {
            double value = (double)_event[field.Name];
            log.Debug("GetDoubleValue = " + value);
            return value;
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
