using System;

using net.esper.client;
using net.esper.client.time;
using net.esper.regression.support;
using net.esper.support.bean;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.pattern
{
    [TestFixture]
    public class TestEveryOperator
    {
        [Test]
        public virtual void testOp()
        {
            EventCollection events = EventCollectionFactory.GetEventSetOne(0, 1000);
            CaseList testCaseList = new CaseList();
            EventExpressionCase testCase = null;

            testCase = new EventExpressionCase("every b=" + SupportBeanConstants.EVENT_B_CLASS);
            testCase.Add("B1", "b", events.GetEvent("B1"));
            testCase.Add("B2", "b", events.GetEvent("B2"));
            testCase.Add("B3", "b", events.GetEvent("B3"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("b=" + SupportBeanConstants.EVENT_B_CLASS);
            testCase.Add("B1", "b", events.GetEvent("B1"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("every (every (every b=" + SupportBeanConstants.EVENT_B_CLASS + "))");
            testCase.Add("B1", "b", events.GetEvent("B1"));
            for (int i = 0; i < 3; i++)
            {
                testCase.Add("B2", "b", events.GetEvent("B2"));
            }
            for (int i = 0; i < 9; i++)
            {
                testCase.Add("B3", "b", events.GetEvent("B3"));
            }
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("every (every b=" + SupportBeanConstants.EVENT_B_CLASS + "())");
            testCase.Add("B1", "b", events.GetEvent("B1"));
            testCase.Add("B2", "b", events.GetEvent("B2"));
            testCase.Add("B2", "b", events.GetEvent("B2"));
            for (int i = 0; i < 4; i++)
            {
                testCase.Add("B3", "b", events.GetEvent("B3"));
            }
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("every( every (every (every b=" + SupportBeanConstants.EVENT_B_CLASS + "())))");
            testCase.Add("B1", "b", events.GetEvent("B1"));
            for (int i = 0; i < 4; i++)
            {
                testCase.Add("B2", "b", events.GetEvent("B2"));
            }
            for (int i = 0; i < 16; i++)
            {
                testCase.Add("B3", "b", events.GetEvent("B3"));
            }
            testCaseList.AddTest(testCase);

            PatternTestHarness util = new PatternTestHarness(events, testCaseList);
            util.RunTest();
        }

        public void testEveryAndNot()
        {
            Configuration config = new Configuration();
            EPServiceProvider engine = EPServiceProviderManager.GetProvider("testRFIDZoneExit", config);
            engine.Initialize();
            engine.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));

            sendTimer(engine, 0);
            String expression =
                "select 'No event within 6 seconds' as alert\n" +
                        "from pattern [ every (timer:interval(6) and not " + typeof(SupportBean).FullName + ") ]";

            EPStatement statement = engine.EPAdministrator.CreateEQL(expression);
            SupportUpdateListener listener = new SupportUpdateListener();
            statement.AddListener(listener);

            sendTimer(engine, 2000);
            engine.EPRuntime.SendEvent(new SupportBean());

            sendTimer(engine, 6000);
            sendTimer(engine, 7000);
            sendTimer(engine, 7999);
            Assert.IsFalse(listener.IsInvoked);

            sendTimer(engine, 8000);
            Assert.AreEqual("No event within 6 seconds", listener.AssertOneGetNewAndReset().Get("alert"));

            sendTimer(engine, 12000);
            engine.EPRuntime.SendEvent(new SupportBean());
            sendTimer(engine, 13000);
            engine.EPRuntime.SendEvent(new SupportBean());

            sendTimer(engine, 18999);
            Assert.IsFalse(listener.IsInvoked);

            sendTimer(engine, 19000);
            Assert.AreEqual("No event within 6 seconds", listener.AssertOneGetNewAndReset().Get("alert"));
        }

        private void sendTimer(EPServiceProvider engine, long timeInMSec)
        {
            CurrentTimeEvent _event = new CurrentTimeEvent(timeInMSec);
            EPRuntime runtime = engine.EPRuntime;
            runtime.SendEvent(_event);
        }
    }
}
