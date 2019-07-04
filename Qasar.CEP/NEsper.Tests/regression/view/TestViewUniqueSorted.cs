using System;
using System.Collections.Generic;

using net.esper.client;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.view
{
    /// <summary> This test uses unique and sort views to obtain from a set of market data events the 3 currently most expensive stocks
    /// and their symbols.
    /// The unique view plays the role of filtering only the most recent events and making prior events for a symbol 'old'
    /// data to the sort view, which removes these prior events for a symbol from the sorted window.
    /// </summary>
	[TestFixture]
    public class TestViewUniqueSorted 
    {
        private const String SYMBOL_CSCO = "CSCO.O";
        private const String SYMBOL_IBM = "IBM.N";
        private const String SYMBOL_MSFT = "MSFT.O";
        private const String SYMBOL_C = "C.N";

        private EPServiceProvider epService;
        private SupportUpdateListener testListener;
        private EPStatement top3Prices;

        [SetUp]
        public virtual void setUp()
        {
            testListener = new SupportUpdateListener();

            EPServiceProviderManager.PurgeAllProviders();
            epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
            epService.Initialize();

            // Get the top 3 volumes for each symbol
            top3Prices = epService.EPAdministrator.CreateEQL("select * from " + typeof(SupportMarketDataBean).FullName + ".std:unique('symbol').ext:sort('price', true, 3)");
            top3Prices.AddListener(testListener);
        }

        [Test]
        public void testWindowStats()
        {
            testListener.Reset();

            Object[] beans = new Object[10];

            beans[0] = MakeEvent(SYMBOL_CSCO, 50);
            epService.EPRuntime.SendEvent(beans[0]);

            Object[] result = toObjectArray(top3Prices.GetEnumerator());
            ArrayAssertionUtil.AreEqualExactOrder(result, new Object[] { beans[0] });
            Assert.IsTrue(testListener.IsInvoked);
            ArrayAssertionUtil.AreEqualExactOrder(testListener.LastOldData, null);
            ArrayAssertionUtil.AreEqualExactOrder(new Object[] { testListener.LastNewData[0].Underlying }, new Object[] { beans[0] });
            testListener.Reset();

            beans[1] = MakeEvent(SYMBOL_CSCO, 20);
            beans[2] = MakeEvent(SYMBOL_IBM, 50);
            beans[3] = MakeEvent(SYMBOL_MSFT, 40);
            beans[4] = MakeEvent(SYMBOL_C, 100);
            beans[5] = MakeEvent(SYMBOL_IBM, 10);

            epService.EPRuntime.SendEvent(beans[1]);
            epService.EPRuntime.SendEvent(beans[2]);
            epService.EPRuntime.SendEvent(beans[3]);
            epService.EPRuntime.SendEvent(beans[4]);
            epService.EPRuntime.SendEvent(beans[5]);

            result = toObjectArray(top3Prices.GetEnumerator());
            ArrayAssertionUtil.AreEqualExactOrder(result, new Object[] { beans[4], beans[3], beans[5] });

            beans[6] = MakeEvent(SYMBOL_CSCO, 110);
            beans[7] = MakeEvent(SYMBOL_C, 30);
            beans[8] = MakeEvent(SYMBOL_CSCO, 30);

            epService.EPRuntime.SendEvent(beans[6]);
            epService.EPRuntime.SendEvent(beans[7]);
            epService.EPRuntime.SendEvent(beans[8]);

            result = toObjectArray(top3Prices.GetEnumerator());
            ArrayAssertionUtil.AreEqualExactOrder(result, new Object[] { beans[3], beans[8], beans[7] });
        }

        [Test]
        public void testSensorPerEvent()
        {
            String stmtString =
                "SELECT * " +
                "FROM\n " +
                typeof (SupportSensorEvent).FullName +
                ".std:groupby('type').win:time(1 hour).std:unique('device').ext:sort('measurement',true,1) as high ";

            EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtString);
            stmt.AddListener(testListener);

            EPRuntime runtime = epService.EPRuntime;

            SupportSensorEvent eventOne = new SupportSensorEvent(1, "Temperature", "Device1", 5.0, 96.5);
            runtime.SendEvent(eventOne);
            testListener.AssertUnderlyingAndReset(new Object[] {eventOne}, null);

            SupportSensorEvent eventTwo = new SupportSensorEvent(2, "Temperature", "Device2", 7.0, 98.5);
            runtime.SendEvent(eventTwo);
            testListener.AssertUnderlyingAndReset(new Object[] {eventTwo}, new Object[] {eventOne});

            SupportSensorEvent eventThree = new SupportSensorEvent(3, "Temperature", "Device2", 4.0, 99.5);
            runtime.SendEvent(eventThree);
            testListener.AssertUnderlyingAndReset(new Object[] {eventThree}, new Object[] {eventTwo});

            IEnumerator<EventBean> it = stmt.GetEnumerator();
            Assert.IsTrue(it.MoveNext());
            SupportSensorEvent _event = (SupportSensorEvent) it.Current.Underlying;
            Assert.AreEqual(3, _event.Id);
        }

        private Object MakeEvent(String symbol, double price)
        {
            SupportMarketDataBean _event = new SupportMarketDataBean(symbol, price, 0L, "");
            return _event;
        }

        private Object[] toObjectArray(IEnumerator<EventBean> it)
        {
            List<Object> result = new List<Object>();
            while ( it.MoveNext() )
            {
                EventBean _event = it.Current;
                result.Add(_event.Underlying);
            }
            return result.ToArray();
        }
    }
}
