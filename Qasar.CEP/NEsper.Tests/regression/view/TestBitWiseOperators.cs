using System;

using net.esper.compat;
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
    public class TestBitWiseOperators
    {
        internal const sbyte FIRST_EVENT = 1;
        internal const short SECOND_EVENT = 2;
        internal const int THIRD_EVENT = FIRST_EVENT | SECOND_EVENT;
        internal const long FOURTH_EVENT = 4;
        internal const bool FITH_EVENT = false;

        private EPServiceProvider _epService;
        private SupportUpdateListener _testListener;
        private EPStatement _selectTestView;

        [SetUp]
        public virtual void SetUp()
        {
            _testListener = new SupportUpdateListener();

            EPServiceProviderManager.PurgeAllProviders();
            _epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
            _epService.Initialize();
        }

        [Test]
        public void testBitWiseOperators()
        {
            setUpBitWiseStmt();
            _testListener.Reset();

            SendEvent(
                FIRST_EVENT, FIRST_EVENT,
                SECOND_EVENT, SECOND_EVENT,
                FIRST_EVENT, THIRD_EVENT,
                3L, FOURTH_EVENT,
                FITH_EVENT, FITH_EVENT);

            EventBean received = _testListener.GetAndResetLastNewData()[0];
            Assert.AreEqual((byte) 1, (received.Get("myFirstProperty")));
            Assert.IsTrue(((short) (received.Get("mySecondProperty")) & SECOND_EVENT) == SECOND_EVENT);
            Assert.IsTrue(((int) (received.Get("myThirdProperty")) & FIRST_EVENT) == FIRST_EVENT);
            Assert.AreEqual(7L, (received.Get("myFourthProperty")));
            Assert.AreEqual(false, (received.Get("myFithProperty")));
        }

        private void setUpBitWiseStmt()
        {
            String viewExpr = "select (bytePrimitive & byteBoxed) as myFirstProperty, " +
                              "(shortPrimitive | shortBoxed) as mySecondProperty, " +
                              "(intPrimitive | intBoxed) as myThirdProperty, " +
                              "(longPrimitive ^ longBoxed) as myFourthProperty, " +
                              "(boolPrimitive & boolBoxed) as myFithProperty " +
                              " from " + typeof (SupportBean).FullName + ".win:length(3) ";
            _selectTestView = _epService.EPAdministrator.CreateEQL(viewExpr);
            _selectTestView.AddListener(_testListener);
        }

        protected void SendEvent(
            sbyte bytePrimitive_,
            sbyte? byteBoxed_,
            short shortPrimitive_,
            short? shortBoxed,
            int intPrimitive_,
            int? intBoxed_,
            long longPrimitive_,
            long? longBoxed_,
            bool boolPrimitive_,
            bool? boolBoxed_)
        {
            SupportBean bean = new SupportBean();
            bean.SetBytePrimitive(bytePrimitive_);
            bean.SetByteBoxed(byteBoxed_);
            bean.SetShortPrimitive(shortPrimitive_);
            bean.SetShortBoxed(shortBoxed);
            bean.SetIntPrimitive(intPrimitive_);
            bean.SetIntBoxed(intBoxed_);
            bean.SetLongPrimitive(longPrimitive_);
            bean.SetLongBoxed(longBoxed_);
            bean.SetBoolPrimitive(boolPrimitive_);
            bean.SetBoolBoxed(boolBoxed_);
            _epService.EPRuntime.SendEvent(bean);
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
