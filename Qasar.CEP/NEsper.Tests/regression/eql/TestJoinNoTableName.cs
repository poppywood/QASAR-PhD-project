using System;

using net.esper.client;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.eql
{
	[TestFixture]
	public class TestJoinNoTableName
	{
		private EPServiceProvider epService;
		private EPStatement joinView;
		private SupportUpdateListener updateListener;

		private Object[] setOne = new Object[5];
		private Object[] setTwo = new Object[5];

		[SetUp]
		public virtual void setUp()
		{
            EPServiceProviderManager.PurgeAllProviders();
            epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
            epService.Initialize();
			updateListener = new SupportUpdateListener();

			String joinStatement =
				"select * from " +
                typeof(SupportMarketDataBean).FullName + ".win:length(3)," +
                typeof(SupportBean).FullName + ".win:length(3)" +
				" where symbol=string and volume=longBoxed";

			joinView = epService.EPAdministrator.CreateEQL( joinStatement );
			joinView.AddListener(updateListener);

			for ( int i = 0 ; i < setOne.Length ; i++ )
			{
				setOne[i] = new SupportMarketDataBean( "IBM", 0, (long) i, "" );

				SupportBean _event = new SupportBean();
                _event.SetString("IBM");
                _event.SetLongBoxed((long)i);
				setTwo[i] = _event;
			}
		}

		[Test]
		public void testJoinUniquePerId()
		{
			SendEvent( setOne[0] );
			SendEvent( setTwo[0] );
			Assert.IsNotNull( updateListener.LastNewData );
		}

		private void SendEvent( Object _event )
		{
			epService.EPRuntime.SendEvent( _event );
		}
	}
}