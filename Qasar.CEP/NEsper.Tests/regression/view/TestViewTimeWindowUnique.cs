using System;

using net.esper.client;
using net.esper.client.time;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.view
{
	
	[TestFixture]
	public class TestViewTimeWindowUnique 
	{
		private EPServiceProvider epService;
		private SupportUpdateListener testListener;
		private EPStatement windowUniqueView;
		
		[SetUp]
		public virtual void  setUp()
		{
			testListener = new SupportUpdateListener();

            EPServiceProviderManager.PurgeAllProviders();
			epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
			epService.Initialize();
			
			// Set up a time window with a unique view attached
			windowUniqueView = epService.EPAdministrator.CreateEQL("select * from " + typeof(SupportMarketDataBean).FullName + ".win:time(3.0).std:unique('symbol')");
			windowUniqueView.AddListener(testListener);
			
			// External clocking
			epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
		}
		
		// Make sure the timer and dispatch works for externally timed events and views
		[Test]
		public virtual void  testWindowUnique()
		{
			sendTimer(0);
			
			SendEvent("IBM");
			
			Assert.IsNull(testListener.LastOldData);
			sendTimer(4000);
			Assert.AreEqual(1, testListener.LastOldData.Length);
		}
		
		private void  SendEvent(String symbol)
		{
			SupportMarketDataBean _event = new SupportMarketDataBean(symbol, 0, 0L, "");
			epService.EPRuntime.SendEvent(_event);
		}
		
		private void  sendTimer(long time)
		{
			CurrentTimeEvent _event = new CurrentTimeEvent(time);
			EPRuntime runtime = epService.EPRuntime;
			runtime.SendEvent(_event);
		}
	}
}
