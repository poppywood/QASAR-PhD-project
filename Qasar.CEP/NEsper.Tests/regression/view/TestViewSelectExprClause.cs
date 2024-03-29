using System;
using System.Collections.Generic;

using net.esper.client;
using net.esper.client.time;
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
	public class TestViewSelectExprClause
	{
		private EPServiceProvider epService;
		private SupportUpdateListener testListener;
		private EPStatement selectTestView;

		[SetUp]
		public virtual void SetUp()
		{
			testListener = new SupportUpdateListener();

            EPServiceProviderManager.PurgeAllProviders();
			epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
			epService.Initialize();
			epService.EPRuntime.SendEvent( new TimerControlEvent( TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL ) );

            String viewExpr = "select string, boolBoxed as aBool, 3*intPrimitive, floatBoxed+floatPrimitive as result" + " from " + typeof(SupportBean).FullName + ".win:length(3) " + " where boolBoxed = true";
			selectTestView = epService.EPAdministrator.CreateEQL( viewExpr );
            selectTestView.AddListener(testListener);
		}

		[Test]
		public void testEventType()
		{
			EventType type = selectTestView.EventType;
            IList<String> testList = new String[]{ "(3*intPrimitive)", "string", "result", "aBool" } ;

			log.Debug( ".testGetEventType properties=" + CollectionHelper.Render( type.PropertyNames ) );
			ArrayAssertionUtil.AreEqualAnyOrder( type.PropertyNames, testList ) ;
			Assert.AreEqual( typeof( String ), type.GetPropertyType( "string" ) );
			Assert.AreEqual( typeof( bool? ), type.GetPropertyType( "aBool" ) );
			Assert.AreEqual( typeof( float? ), type.GetPropertyType( "result" ) );
			Assert.AreEqual( typeof( int? ), type.GetPropertyType( "(3*intPrimitive)" ) );
		}

		[Test]
		public void testWindowStats()
		{
			testListener.Reset();

			SendEvent( "a", false, 0, 0, 0 );
			SendEvent( "b", false, 0, 0, 0 );
			Assert.IsTrue( testListener.LastNewData == null );
			SendEvent( "c", true, 3, 10, 20 );

			EventBean received = testListener.GetAndResetLastNewData()[0];
			Assert.AreEqual( "c", received["string"] );
			Assert.AreEqual( true, received["aBool"] );
			Assert.AreEqual( 30f, received["result"] );
		}

		private void SendEvent( String s, bool b, int i, float f1, float f2 )
		{
			SupportBean bean = new SupportBean();
			bean.SetString(s);
			bean.SetBoolBoxed(b );
			bean.SetIntPrimitive(i);
			bean.SetFloatPrimitive(f1);
			bean.SetFloatBoxed(f2 );
			epService.EPRuntime.SendEvent( bean );
		}

		private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
}
