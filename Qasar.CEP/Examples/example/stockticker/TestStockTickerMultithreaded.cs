using System;
using System.Collections.Generic;

using net.esper.client;
using net.esper.example.stockticker.eventbean;
using net.esper.example.stockticker.monitor;
using net.esper.support;
using net.esper.support.util;

using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.example.stockticker
{
    [TestFixture]
    public class TestStockTickerMultithreaded
    {
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        StockTickerEmittedListener listener;
        private EPServiceProvider epService;

        [SetUp]
        protected void SetUp()
        {
            listener = new StockTickerEmittedListener();

            Configuration configuration = new Configuration();
            configuration.EngineDefaults.EventMeta.ClassPropertyResolutionStyle = PropertyResolutionStyle.CASE_INSENSITIVE;
            configuration.AddEventTypeAlias("PriceLimit", typeof (PriceLimit).FullName);
            configuration.AddEventTypeAlias("StockTick", typeof (StockTick).FullName);

            epService = EPServiceProviderManager.GetProvider("TestStockTickerMultithreaded", configuration);
            epService.Initialize();
            epService.EPRuntime.AddEmittedListener(listener, null);

            new StockTickerMonitor(epService);
        }

        [Test]
        public void testMultithreaded()
        {
            // performTest(3, 500000, 100000, 60);  // on fast systems
            performTest(3, 50000, 10000, 15); // for unit tests on slow machines
        }

        public void performTest(int numberOfThreads,
                                int numberOfTicksToSend,
                                int ratioPriceOutOfLimit,
                                int numberOfSecondsWaitForCompletion)
        {
            int totalNumTicks = numberOfTicksToSend + 2*TestStockTickerGenerator.NUM_STOCK_NAMES;

            log.Info(".performTest Generating data, numberOfTicksToSend=" + numberOfTicksToSend +
                     "  ratioPriceOutOfLimit=" + ratioPriceOutOfLimit);

            StockTickerEventGenerator generator = new StockTickerEventGenerator();
            List<Object> stream =
                generator.MakeEventStream(numberOfTicksToSend, ratioPriceOutOfLimit,
                                          TestStockTickerGenerator.NUM_STOCK_NAMES);

            log.Info(".performTest Send limit and initial tick events - singlethreaded");
            for (int i = 0; i < TestStockTickerGenerator.NUM_STOCK_NAMES*2; i++)
            {
                Object _event = stream[0];
                stream.RemoveAt(0);
                epService.EPRuntime.SendEvent(_event);
            }

            log.Info(".performTest Loading thread pool work queue, numberOfRunnables=" + stream.Count);

            ExecutorService executorService = new ExecutorService(numberOfThreads);
            foreach (Object _event in stream)
            {
                SendEventRunnable runnable = new SendEventRunnable(epService, _event);
                executorService.Submit(runnable);
            }

            log.Info(".performTest Listening for completion");
            EPRuntimeUtil.AwaitCompletion(epService.EPRuntime, totalNumTicks, numberOfSecondsWaitForCompletion, 1, 10);

            executorService.Shutdown();
            executorService.AwaitTermination(new TimeSpan(0, 0, numberOfSecondsWaitForCompletion));

            // Check results : make sure the given ratio of out-of-limit stock prices was reported
            int expectedNumEmitted = (numberOfTicksToSend/ratioPriceOutOfLimit) + 1;
            Assert.AreEqual(listener.Size, expectedNumEmitted);

            log.Info(".performTest Done test");
        }
    }
}
