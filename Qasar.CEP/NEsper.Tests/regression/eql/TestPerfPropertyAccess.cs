using System;
using System.Diagnostics;

using net.esper.client;
using net.esper.compat;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Core;
using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.regression.eql
{
    [TestFixture]
    public class TestPerfPropertyAccess
    {
        private EPServiceProvider epService;
        private EPStatement joinView;
        private SupportUpdateListener updateListener;

        [SetUp]
        public virtual void setUp()
        {
            EPServiceProviderManager.PurgeAllProviders();
            epService = EPServiceProviderManager.GetDefaultProvider();
            epService.Initialize();
            updateListener = new SupportUpdateListener();
        }

        [Test]
        public virtual void testPerfPropertyAccess()
        {
            String methodName = ".testPerfPropertyAccess";

            String joinStatement =
                "select * from " +
                typeof(SupportBeanCombinedProps).FullName + ".win:length(1)" +
                " where indexed[0].mapped('a').value = 'dummy'";

            joinView = epService.EPAdministrator.CreateEQL(joinStatement);
            joinView.AddListener(updateListener);

            // Send events for each stream
            SupportBeanCombinedProps _event = SupportBeanCombinedProps.MakeDefaultBean();
            log.Info(methodName + " Sending events");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 10000; i++)
            {
                SendEvent(_event);
            }
            log.Info(methodName + " Done sending events");

            stopwatch.Stop();

            long delta = stopwatch.ElapsedMilliseconds;
            log.Info(methodName + " delta=" + delta);

            // Stays at 250, below 500ms
            Assert.Less((int) delta, 1000);
        }

        private void SendEvent(Object _event)
        {
            epService.EPRuntime.SendEvent(_event);
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}