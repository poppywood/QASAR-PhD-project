using System;

using net.esper.client;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.view
{
	[TestFixture]
	public class TestViewPropertyAccess
	{
		private EPServiceProvider epService;
		private SupportUpdateListener testListener;

		[SetUp]
		public virtual void  setUp()
		{
            EPServiceProviderManager.PurgeAllProviders();
			epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
			epService.Initialize();
		}

		[Test]
		public virtual void  testWhereAndSelect()
		{
			String viewExpr = "select mapped('keyOne') as a," + "indexed[1] as b, nested.nestedNested.nestedNestedValue as c, mapProperty, " + "arrayProperty[0] " + "  from " + typeof(SupportBeanComplexProps).FullName + ".win:length(3) " + " where mapped('keyOne') = 'valueOne' and " + " indexed[1] = 2 and " + " nested.nestedNested.nestedNestedValue = 'nestedNestedValue'";

			EPStatement testView = epService.EPAdministrator.CreateEQL(viewExpr);
			testListener = new SupportUpdateListener();
			testView.AddListener(testListener);

			SupportBeanComplexProps eventObject = SupportBeanComplexProps.MakeDefaultBean();
			epService.EPRuntime.SendEvent(eventObject);
			EventBean _event = testListener.GetAndResetLastNewData()[0];
			Assert.AreEqual(eventObject.GetMapped("keyOne"), _event["a"]);
			Assert.AreEqual(eventObject.GetIndexed(1), _event["b"]);
			Assert.AreEqual(eventObject.Nested.NestedNested.NestedNestedValue, _event["c"]);
            Assert.AreEqual(eventObject.MapProperty, _event["mapProperty"]);
			Assert.AreEqual(eventObject.ArrayProperty[0], _event["arrayProperty[0]"]);

			eventObject.SetIndexed(1, Int32.MinValue);
			Assert.IsFalse(testListener.IsInvoked);
			epService.EPRuntime.SendEvent(eventObject);
			Assert.IsFalse(testListener.IsInvoked);

			eventObject.SetIndexed(1, 2);
			epService.EPRuntime.SendEvent(eventObject);
			Assert.IsTrue(testListener.IsInvoked);
		}
	}
}