using System;

using net.esper.client;
using net.esper.client.time;
using net.esper.events;
using net.esper.regression.support;
using net.esper.support.bean;

using NUnit.Core;
using NUnit.Framework;

namespace net.esper.regression.pattern
{
    [TestFixture]
    public class TestUseResultPattern : SupportBeanConstants
    {
        [Test]
        public virtual void testNumeric()
        {
            String _event = typeof(SupportBean_N).FullName;

            EventCollection events = EventCollectionFactory.GetSetThreeExternalClock(0, 1000);
            CaseList testCaseList = new CaseList();
            EventExpressionCase testCase = null;

            testCase = new EventExpressionCase("na=" + _event + " -> nb=" + _event + "(doublePrimitive = na.doublePrimitive)");
            testCase.Add("N6", "na", events.GetEvent("N1"), "nb", events.GetEvent("N6"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "(intPrimitive=87) -> nb=" + _event + "(intPrimitive > na.intPrimitive)");
            testCase.Add("N8", "na", events.GetEvent("N3"), "nb", events.GetEvent("N8"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "(intPrimitive=87) -> nb=" + _event + "(intPrimitive < na.intPrimitive)");
            testCase.Add("N4", "na", events.GetEvent("N3"), "nb", events.GetEvent("N4"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "(intPrimitive=66) -> every nb=" + _event + "(intPrimitive >= na.intPrimitive)");
            testCase.Add("N3", "na", events.GetEvent("N2"), "nb", events.GetEvent("N3"));
            testCase.Add("N4", "na", events.GetEvent("N2"), "nb", events.GetEvent("N4"));
            testCase.Add("N8", "na", events.GetEvent("N2"), "nb", events.GetEvent("N8"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "(intPrimitive=66) -> every nb=" + _event + "(intPrimitive >= na.intPrimitive)");
            testCase.Add("N3", "na", events.GetEvent("N2"), "nb", events.GetEvent("N3"));
            testCase.Add("N4", "na", events.GetEvent("N2"), "nb", events.GetEvent("N4"));
            testCase.Add("N8", "na", events.GetEvent("N2"), "nb", events.GetEvent("N8"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "(boolBoxed=false) -> every nb=" + _event + "(boolPrimitive = na.boolPrimitive)");
            testCase.Add("N4", "na", events.GetEvent("N2"), "nb", events.GetEvent("N4"));
            testCase.Add("N5", "na", events.GetEvent("N2"), "nb", events.GetEvent("N5"));
            testCase.Add("N8", "na", events.GetEvent("N2"), "nb", events.GetEvent("N8"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("every na=" + _event + " -> every nb=" + _event + "(intPrimitive=na.intPrimitive)");
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("every na=" + _event + "() -> every nb=" + _event + "(doublePrimitive=na.doublePrimitive)");
            testCase.Add("N5", "na", events.GetEvent("N2"), "nb", events.GetEvent("N5"));
            testCase.Add("N6", "na", events.GetEvent("N1"), "nb", events.GetEvent("N6"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("every na=" + _event + "(boolBoxed=false) -> every nb=" + _event + "(boolBoxed=na.boolBoxed)");
            testCase.Add("N5", "na", events.GetEvent("N2"), "nb", events.GetEvent("N5"));
            testCase.Add("N8", "na", events.GetEvent("N2"), "nb", events.GetEvent("N8"));
            testCase.Add("N8", "na", events.GetEvent("N5"), "nb", events.GetEvent("N8"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "(boolBoxed=false) -> nb=" + _event + "(intPrimitive<na.intPrimitive)" + " -> nc=" + _event + "(intPrimitive > nb.intPrimitive)");
            testCase.Add("N6", "na", events.GetEvent("N2"), "nb", events.GetEvent("N5"), "nc", events.GetEvent("N6"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "(intPrimitive=86) -> nb=" + _event + "(intPrimitive<na.intPrimitive)" + " -> nc=" + _event + "(intPrimitive > na.intPrimitive)");
            testCase.Add("N8", "na", events.GetEvent("N4"), "nb", events.GetEvent("N5"), "nc", events.GetEvent("N8"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "(intPrimitive=86) -> (nb=" + _event + "(intPrimitive<na.intPrimitive)" + " or nc=" + _event + "(intPrimitive > na.intPrimitive))");
            testCase.Add("N5", "na", events.GetEvent("N4"), "nb", events.GetEvent("N5"), "nc", (Object)null);
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "(intPrimitive=86) -> (nb=" + _event + "(intPrimitive>na.intPrimitive)" + " or nc=" + _event + "(intBoxed < na.intBoxed))");
            testCase.Add("N8", "na", events.GetEvent("N4"), "nb", events.GetEvent("N8"), "nc", (Object)null);
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "(intPrimitive=86) -> (nb=" + _event + "(intPrimitive>na.intPrimitive)" + " and nc=" + _event + "(intBoxed < na.intBoxed))");
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "() -> every nb=" + _event + "(doublePrimitive in [0:na.doublePrimitive])");
            testCase.Add("N4", "na", events.GetEvent("N1"), "nb", events.GetEvent("N4"));
            testCase.Add("N6", "na", events.GetEvent("N1"), "nb", events.GetEvent("N6"));
            testCase.Add("N7", "na", events.GetEvent("N1"), "nb", events.GetEvent("N7"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "() -> every nb=" + _event + "(doublePrimitive in (0:na.doublePrimitive))");
            testCase.Add("N4", "na", events.GetEvent("N1"), "nb", events.GetEvent("N4"));
            testCase.Add("N7", "na", events.GetEvent("N1"), "nb", events.GetEvent("N7"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "() -> every nb=" + _event + "(intPrimitive in (na.intPrimitive:na.doublePrimitive))");
            testCase.Add("N7", "na", events.GetEvent("N1"), "nb", events.GetEvent("N7"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("na=" + _event + "() -> every nb=" + _event + "(intPrimitive in (na.intPrimitive:60))");
            testCase.Add("N6", "na", events.GetEvent("N1"), "nb", events.GetEvent("N6"));
            testCase.Add("N7", "na", events.GetEvent("N1"), "nb", events.GetEvent("N7"));
            testCaseList.AddTest(testCase);

            PatternTestHarness util = new PatternTestHarness(events, testCaseList);
            util.RunTest();
        }

        [Test]
        public virtual void testObjectId()
        {
            String _event = typeof(SupportBean_S0).FullName;

            EventCollection events = EventCollectionFactory.GetSetFourExternalClock(0, 1000);
            CaseList testCaseList = new CaseList();
            EventExpressionCase testCase = null;

            testCase = new EventExpressionCase("X1=" + _event + "() -> X2=" + _event + "(p00=X1.p00)");
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("X1=" + _event + "(p00='B') -> X2=" + _event + "(p00=X1.p00)");
            testCase.Add("e6", "X1", events.GetEvent("e2"), "X2", events.GetEvent("e6"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("X1=" + _event + "(p00='B') -> every X2=" + _event + "(p00=X1.p00)");
            testCase.Add("e6", "X1", events.GetEvent("e2"), "X2", events.GetEvent("e6"));
            testCase.Add("e11", "X1", events.GetEvent("e2"), "X2", events.GetEvent("e11"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("every X1=" + _event + "(p00='B') -> every X2=" + _event + "(p00=X1.p00)");
            testCase.Add("e6", "X1", events.GetEvent("e2"), "X2", events.GetEvent("e6"));
            testCase.Add("e11", "X1", events.GetEvent("e2"), "X2", events.GetEvent("e11"));
            testCase.Add("e11", "X1", events.GetEvent("e6"), "X2", events.GetEvent("e11"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("every X1=" + _event + "() -> X2=" + _event + "(p00=X1.p00)");
            testCase.Add("e6", "X1", events.GetEvent("e2"), "X2", events.GetEvent("e6"));
            testCase.Add("e8", "X1", events.GetEvent("e3"), "X2", events.GetEvent("e8"));
            testCase.Add("e10", "X1", events.GetEvent("e9"), "X2", events.GetEvent("e10"));
            testCase.Add("e11", "X1", events.GetEvent("e6"), "X2", events.GetEvent("e11"));
            testCase.Add("e12", "X1", events.GetEvent("e7"), "X2", events.GetEvent("e12"));
            testCaseList.AddTest(testCase);

            testCase = new EventExpressionCase("every X1=" + _event + "() -> every X2=" + _event + "(p00=X1.p00)");
            testCase.Add("e6", "X1", events.GetEvent("e2"), "X2", events.GetEvent("e6"));
            testCase.Add("e8", "X1", events.GetEvent("e3"), "X2", events.GetEvent("e8"));
            testCase.Add("e10", "X1", events.GetEvent("e9"), "X2", events.GetEvent("e10"));
            testCase.Add("e11", "X1", events.GetEvent("e2"), "X2", events.GetEvent("e11"));
            testCase.Add("e11", "X1", events.GetEvent("e6"), "X2", events.GetEvent("e11"));
            testCase.Add("e12", "X1", events.GetEvent("e7"), "X2", events.GetEvent("e12"));
            testCaseList.AddTest(testCase);

            PatternTestHarness util = new PatternTestHarness(events, testCaseList);
            util.RunTest();
        }

        public void testFollowedByFilter()
        {
            // Test for ESPER-121
            Configuration config = new Configuration();
            config.AddEventTypeAlias("FxTradeEvent", typeof(SupportTradeEvent).FullName);
            EPServiceProvider epService = EPServiceProviderManager.GetProvider(
                    "testRFIDZoneEnter", config);
            epService.Initialize();
            epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));

            String expression = "every tradeevent1=FxTradeEvent(userId in ('U1000','U1001','U1002') ) -> " +
                    "(tradeevent2=FxTradeEvent(userId in ('U1000','U1001','U1002') and " +
                    "  userId != tradeevent1.userId and " +
                    "  ccypair = tradeevent1.ccypair and " +
                    "  direction = tradeevent1.direction) -> " +
                    " tradeevent3=FxTradeEvent(userId in ('U1000','U1001','U1002') and " +
                    "  userId != tradeevent1.userId and " +
                    "  userId != tradeevent2.userId and " +
                    "  ccypair = tradeevent1.ccypair and " +
                    "  direction = tradeevent1.direction)" +
                    ") where timer:within(600 sec)";

            EPStatement statement = epService.EPAdministrator.CreatePattern(expression);
            MyUpdateListener listener = new MyUpdateListener();
            statement.AddListener(listener);

            Random random = new Random();
            String[] users = { "U1000", "U1001", "U1002" };
            String[] ccy = { "USD", "JPY", "EUR" };
            String[] direction = { "B", "S" };

            for (int i = 0; i < 100; i++)
            {
                SupportTradeEvent _event = new SupportTradeEvent(i,
                    users[random.Next(users.Length)],
                    ccy[random.Next(ccy.Length)],
                    direction[random.Next(direction.Length)]);
                epService.EPRuntime.SendEvent(_event);
            }

            Assert.AreEqual(0, listener.badMatchCount);
        }

        private class MyUpdateListener : UpdateListener
        {
            internal int badMatchCount;
            internal int goodMatchCount;

            public void Update(EventBean[] newEvents, EventBean[] oldEvents)
            {
                if (newEvents != null)
                {
                    foreach (EventBean eventBean in newEvents)
                    {
                        handleEvent(eventBean);
                    }
                }
            }

            private void handleEvent(EventBean eventBean)
            {
                SupportTradeEvent tradeevent1 = (SupportTradeEvent)
                        eventBean.Get("tradeevent1");
                SupportTradeEvent tradeevent2 = (SupportTradeEvent)
                        eventBean.Get("tradeevent2");
                SupportTradeEvent tradeevent3 = (SupportTradeEvent)
                        eventBean.Get("tradeevent3");

                if (Equals(tradeevent1.UserId, tradeevent2.UserId) ||
                    Equals(tradeevent1.UserId, tradeevent3.UserId) ||
                    Equals(tradeevent2.UserId, tradeevent3.UserId))
                {
                    Console.Out.WriteLine("Bad Match : ");
                    Console.Out.WriteLine(tradeevent1);
                    Console.Out.WriteLine(tradeevent2);
                    Console.Out.WriteLine(tradeevent3);
                    Console.Out.WriteLine();
                    badMatchCount++;
                }
                else
                {
                    goodMatchCount++;
                }
            }
        }
    }
}